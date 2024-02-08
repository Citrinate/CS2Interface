using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
using ProtoBuf;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;

namespace CS2Interface {
	internal class Client {
		internal const uint AppID = 730;
		internal readonly Bot Bot;
		internal readonly SteamGameCoordinator GameCoordinator;
		internal CallbackManager CallbackManager;
		internal event Action<SteamGameCoordinator.MessageCallback>? OnGCMessageRecieved;
		private bool HasGCSession = false;
		private const int MaxSimultaneousGCRequests = 1;
		private SemaphoreSlim GCSemaphore = new SemaphoreSlim(MaxSimultaneousGCRequests, MaxSimultaneousGCRequests);
		private SemaphoreSlim ConnectionSemaphore = new SemaphoreSlim(1, 1);
		internal ConcurrentDictionary<ulong, InventoryItem>? Inventory = null;
		private bool FatalError = false;

		internal Client(Bot bot, CallbackManager callbackManager) {
			Bot = bot;
			CallbackManager = callbackManager;
			GameCoordinator = Bot.GetHandler<SteamGameCoordinator>() ?? throw new InvalidOperationException(nameof(SteamGameCoordinator));
			callbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGCMessage);
		}

		internal async Task<bool> Run() {
			if (HasGCSession) {
				return true;
			}

			if (ConnectionSemaphore.CurrentCount != 1) {
				throw new ClientException(EClientExceptionType.Failed, "CS2 Client is already attempting to run, please wait");
			}

			if (FatalError) {
				throw new ClientException(EClientExceptionType.FatalError, "CS2 Client experienced a fatal error");
			}

			await ConnectionSemaphore.WaitAsync().ConfigureAwait(false);
			try {
				if (!await GameData.IsLoaded(0).ConfigureAwait(false)) {
					throw new ClientException(EClientExceptionType.Failed, "Failed to load Game Data");
				}

				// TODO: Verify that this account owns CS2

				(bool play_success, string play_message) = await Bot.Actions.Play(new HashSet<uint> { AppID }).ConfigureAwait(false);
				Bot.ArchiLogger.LogGenericInfo(play_message);
				if (!play_success) {
					throw new ClientException(EClientExceptionType.Failed, play_message);
				}

				await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

				var msg = new ClientGCMsgProtobuf<CMsgClientHello>((uint) EGCBaseClientMsg.k_EMsgGCClientHello);
				var fetcher = new GCFetcher<CMsgClientHello, CMsgClientWelcome>((uint) EGCBaseClientMsg.k_EMsgGCClientWelcome);

				Bot.ArchiLogger.LogGenericDebug("Sending hello message");

				if (fetcher.Fetch(this, msg, resendMsg: true) == null) {
					throw new ClientException(EClientExceptionType.Timeout, "CS2 Client wasn't able to connect to GC");
				}

				HasGCSession = true;

				return true;
			} finally {
				ConnectionSemaphore.Release();
			}
		}

		internal void Stop() {
			if (!HasGCSession) {
				return;
			}

			HasGCSession = false;

			return;
		}

		internal EClientStatus Status() {
			EClientStatus status = EClientStatus.None;			
			if (HasGCSession) {
				status |= EClientStatus.Connected;
			}
			if (GCSemaphore.CurrentCount > 0 && ConnectionSemaphore.CurrentCount == 1) {
				status |= EClientStatus.Ready;
			}

			return status;
		}

		internal async Task<bool> VerifyConnection() {
			if (!HasGCSession) {
				return false;
			}

			Bot.ArchiLogger.LogGenericDebug("Verifying CS2 Client's connection to GC");
			try {
				await RequestPlayerProfile(Bot.SteamID).ConfigureAwait(false);
			} catch {
				return false;
			}

			return true;
		}

		private void OnGCMessage(SteamGameCoordinator.MessageCallback callback) {
			if (callback.AppID != AppID) {
				return;
			}
			
			OnGCMessageRecieved?.Invoke(callback);

			var messageMap = new Dictionary<uint, Action<IPacketGCMsg>> {
				{(uint) EGCBaseClientMsg.k_EMsgGCClientWelcome, OnClientWelcome},
				{(uint) ESOMsg.k_ESOMsg_Create, OnItemCreated},
				{(uint) ESOMsg.k_ESOMsg_Destroy, OnItemDestroyed},
				{(uint) ESOMsg.k_ESOMsg_Update, OnItemUpdated},
				{(uint) ESOMsg.k_ESOMsg_UpdateMultiple, OnMultiItemUpdated},
				{(uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientLogonFatalError, OnFatalLogonError}
			};

			Action<IPacketGCMsg>? func;
			if (!messageMap.TryGetValue(callback.EMsg, out func)) {
				// this will happen when we recieve some GC messages that we're not handling
				// this is okay because we're handling every essential message, and the rest can be ignored
				return;
			}

			func(callback.Message);
		}

		private void OnClientWelcome(IPacketGCMsg packetMsg) {
			// Initialize the inventory
			var msg = new ClientGCMsgProtobuf<CMsgClientWelcome>(packetMsg);
			foreach (var cache in msg.Body.outofdate_subscribed_caches) {
				foreach (var obj in cache.objects) {
					if (obj.type_id != 1) {
						// Ignore everything that isn't the inventory
						continue;
					}

					Inventory = new(obj.object_data.Select(x => {
						using (MemoryStream ms = new MemoryStream(x)) {
							var item = Serializer.Deserialize<CSOEconItem>(ms);
							return new KeyValuePair<ulong, InventoryItem>(item.id, new InventoryItem(item));
						}
					}));

					Bot.ArchiLogger.LogGenericDebug("CS2 inventory loaded");
					
					return;
				}
			}
		}

		private void OnFatalLogonError(IPacketGCMsg packetMsg) {
			var msg = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientLogonFatalError>(packetMsg);

			Bot.ArchiLogger.LogGenericError(String.Format("Fatal CS2 logon error {0}: {1}", msg.Body.errorcode, msg.Body.message));
			FatalError = true;
		}

		private void OnItemCreated(IPacketGCMsg packetMsg) {
			if (Inventory == null) {
				return;
			}

			var msg = new ClientGCMsgProtobuf<CMsgSOSingleObject>(packetMsg);
			if (msg.Body.type_id != 1) {
				// Ignore non-inventory changes
				return;
			}

			using (MemoryStream ms = new MemoryStream(msg.Body.object_data)) {
				var item = Serializer.Deserialize<CSOEconItem>(ms);
				Inventory[item.id] = new InventoryItem(item);
			}
		}

		private void OnItemDestroyed(IPacketGCMsg packetMsg) {
			if (Inventory == null) {
				return;
			}

			var msg = new ClientGCMsgProtobuf<CMsgSOSingleObject>(packetMsg);
			if (msg.Body.type_id != 1) {
				// Ignore non-inventory changes
				return;
			}

			using (MemoryStream ms = new MemoryStream(msg.Body.object_data)) {
				var item = Serializer.Deserialize<CSOEconItem>(ms);
				Inventory.TryRemove(item.id, out _);
			}
		}

		private void OnItemUpdated(IPacketGCMsg packetMsg) {
			if (Inventory == null) {
				return;
			}

			var msg = new ClientGCMsgProtobuf<CMsgSOSingleObject>(packetMsg);
			if (msg.Body.type_id != 1) {
				// Ignore non-inventory changes
				return;
			}

			using (MemoryStream ms = new MemoryStream(msg.Body.object_data)) {
				var item = Serializer.Deserialize<CSOEconItem>(ms);
				Inventory[item.id] = new InventoryItem(item);
			}
		}

		private void OnMultiItemUpdated(IPacketGCMsg packetMsg) {
			if (Inventory == null) {
				return;
			}

			var msg = new ClientGCMsgProtobuf<CMsgSOMultipleObjects>(packetMsg);
			foreach (var object_modified in msg.Body.objects_modified) {
				if (object_modified.type_id != 1) {
					// Ignore non-inventory changes
					continue;
				}

				using (MemoryStream ms = new MemoryStream(object_modified.object_data)) {
					var item = Serializer.Deserialize<CSOEconItem>(ms);
					Inventory[item.id] = new InventoryItem(item);
				}
			}
		}

		internal async Task<CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse> InspectItem(ulong param_s, ulong param_a, ulong param_d, ulong param_m) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, "CS2 Client is not connected to GC");
			}

			await GCSemaphore.WaitAsync().ConfigureAwait(false);

			try {
				var msg = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockRequest>((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockRequest) { Body = {
					param_s = param_s,
					param_a = param_a,
					param_d = param_d,
					param_m = param_m
				}};

				var fetcher = new GCFetcher<CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockRequest, CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse> {
					GCResponseMsgType = (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse,
					VerifyFunc = response => response.Body.iteminfo.itemid == param_a
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format("Inspecting item: s {0} a {1} d {2} m {3}", param_s, param_a, param_d, param_m));

				var response = fetcher.Fetch(this, msg);
				if (response == null) {
					throw new ClientException(EClientExceptionType.Timeout, "Request timed out");
				}

				return response.Body;
			} finally {
				GCSemaphore.Release();
			}
		}

		internal async Task<CMsgGCCStrike15_v2_PlayersProfile> RequestPlayerProfile(ulong steam_id) { //uint account_id) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, "CS2 Client is not connected");
			}

			SteamID SteamID = new(steam_id);
			if (!SteamID.IsValid || SteamID.AccountUniverse != EUniverse.Public || SteamID.AccountType != EAccountType.Individual || SteamID.AccountInstance != SteamID.DesktopInstance) {
				throw new ClientException(EClientExceptionType.BadRequest, "Invalid Steam ID");
			}

			await GCSemaphore.WaitAsync().ConfigureAwait(false);

			try {
				uint account_id = SteamID.AccountID;
				var msg = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientRequestPlayersProfile>((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientRequestPlayersProfile) { Body = {
					account_id = account_id,
					request_level = 32
				}};

				var fetcher = new GCFetcher<CMsgGCCStrike15_v2_ClientRequestPlayersProfile, CMsgGCCStrike15_v2_PlayersProfile>{
					GCResponseMsgType = (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_PlayersProfile,
					VerifyFunc = response => response.Body.account_profiles.FirstOrDefault()?.account_id == account_id
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format("Getting CS2 player profile: {0}", steam_id));

				var response = fetcher.Fetch(this, msg);
				if (response == null) {
					throw new ClientException(EClientExceptionType.Timeout, "Request timed out");
				}

				return response.Body;
			} finally {
				GCSemaphore.Release();
			}
		}

		internal async Task<List<InventoryItem>> GetCasketContents(ulong casket_id) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, "CS2 Client is not connected to GC");
			}

			if (Inventory == null) {
				throw new ClientException(EClientExceptionType.Failed, "Inventory not loaded yet");
			}

			InventoryItem? casket = Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == casket_id);
			if (casket == null) {
				throw new ClientException(EClientExceptionType.BadRequest, "Storage unit not found in inventory");
			}

			uint? items_count = casket.GetAttribute("items count")?.ToUInt32();
			if (items_count == null) {
				throw new ClientException(EClientExceptionType.Failed, "Could not determine storage unit item count");
			}

			if (items_count == 0) {
				return new List<InventoryItem>();
			}

			if (Inventory.Values.Where(x => x.CasketID == casket_id).Count() == items_count) {
				// When a casket is opened, the items are added to the inventory.  In this case all of the casket items are already in the inventory.
				return Inventory.Values.Where(x => x.CasketID == casket_id).ToList();
			}

			await GCSemaphore.WaitAsync().ConfigureAwait(false);
			
			try {
				var msg = new ClientGCMsgProtobuf<CMsgCasketItem>((uint) EGCItemMsg.k_EMsgGCCasketItemLoadContents) { Body = {
					casket_item_id = casket_id,
					item_item_id = casket_id
				}};

				var fetcher = new GCFetcher<CMsgCasketItem, CMsgGCItemCustomizationNotification>{
					GCResponseMsgType = (uint) EGCItemMsg.k_EMsgGCItemCustomizationNotification,
					VerifyFunc = response => response.Body.item_id.FirstOrDefault() == casket_id && response.Body.request == (uint) EGCItemCustomizationNotification.k_EGCItemCustomizationNotification_CasketContents
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format("Opening casket {0}", casket_id));

				if (fetcher.Fetch(this, msg) == null) {
					throw new ClientException(EClientExceptionType.Timeout, "Request timed out");
				}

				// Casket contents can sometimes continue to come in after we've recieved the k_EMsgGCItemCustomizationNotification response
				DateTime waitTime = DateTime.Now.AddSeconds(30);
				while (Inventory.Values.Where(x => x.CasketID == casket_id).Count() != items_count && DateTime.Now < waitTime) {
					Bot.ArchiLogger.LogGenericDebug(String.Format("Waiting for casket {0} items", casket_id));
					await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
				}

				if (Inventory.Values.Where(x => x.CasketID == casket_id).Count() != items_count) {
					throw new ClientException(EClientExceptionType.Failed, String.Format("Casket item count mismatch, casket should have {0} items but only found {1}", items_count, Inventory.Values.Where(x => x.CasketID == casket_id).Count()));
				}

				return Inventory.Values.Where(x => x.CasketID == casket_id).ToList();
			} finally {
				GCSemaphore.Release();
			}
		}

		internal async Task<bool> AddItemToCasket(ulong casket_id, ulong item_id) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, "CS2 Client is not connected to GC");
			}

			if (Inventory == null) {
				throw new ClientException(EClientExceptionType.Failed, "Inventory not loaded yet");
			}

			if (Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == item_id) == null) {
				throw new ClientException(EClientExceptionType.BadRequest, "Item not found in inventory");
			}

			InventoryItem? casket = Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == casket_id);
			if (casket == null) {
				throw new ClientException(EClientExceptionType.BadRequest, "Storage unit not found in inventory");
			}

			uint? items_count = casket.GetAttribute("items count")?.ToUInt32();
			if (items_count == null) {
				throw new ClientException(EClientExceptionType.Failed, "Could not determine storage unit item count");
			} else if (items_count == 1000) {
				throw new ClientException(EClientExceptionType.BadRequest, "Storage unit is full");
			}

			await GCSemaphore.WaitAsync().ConfigureAwait(false);
			
			try {
				var msg = new ClientGCMsgProtobuf<CMsgCasketItem>((uint) EGCItemMsg.k_EMsgGCCasketItemAdd) { Body = {
					casket_item_id = casket_id,
					item_item_id = item_id
				}};

				var fetcher = new GCFetcher<CMsgCasketItem, CMsgSOSingleObject>{
					GCResponseMsgType = (uint) ESOMsg.k_ESOMsg_Destroy,
					VerifyFunc = response => {
						if (response.Body.type_id != 1) {
							// Ignore non-inventory changes
							return false;
						}

						using (MemoryStream ms = new MemoryStream(response.Body.object_data)) {
							var item = Serializer.Deserialize<CSOEconItem>(ms);
							if (item.id == item_id) {
								return true;
							}
						}

						return false;
					}
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format("Adding item {0} to casket {1}", item_id, casket_id));

				if (fetcher.Fetch(this, msg) == null) {
					throw new ClientException(EClientExceptionType.Timeout, "Request timed out");
				}

				return true;
			} finally {
				GCSemaphore.Release();
			}
		}
		
		internal async Task<bool> RemoveItemFromCasket(ulong casket_id, ulong item_id) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, "CS2 Client is not connected to GC");
			}

			if (Inventory == null) {
				throw new ClientException(EClientExceptionType.Failed, "Inventory not loaded yet");
			}

			InventoryItem? casket = Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == casket_id);
			if (casket == null) {
				throw new ClientException(EClientExceptionType.BadRequest, "Storage unit not found in inventory");
			}

			// Does not verify that the item is actually in the crate, to do that we would need to request the crate contents first

			await GCSemaphore.WaitAsync().ConfigureAwait(false);
			
			try {
				var msg = new ClientGCMsgProtobuf<CMsgCasketItem>((uint) EGCItemMsg.k_EMsgGCCasketItemExtract) { Body = {
					casket_item_id = casket_id,
					item_item_id = item_id
				}};

				var fetcher = new GCFetcher<CMsgCasketItem, CMsgSOSingleObject>{
					GCResponseMsgType = (uint) ESOMsg.k_ESOMsg_Create,
					VerifyFunc = response => {
						if (response.Body.type_id != 1) {
							// Ignore non-inventory changes
							return false;
						}

						using (MemoryStream ms = new MemoryStream(response.Body.object_data)) {
							var item = Serializer.Deserialize<CSOEconItem>(ms);
							if (item.id == item_id) {
								return true;
							}
						}

						return false;
					}
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format("Removing item {0} from casket {1}", item_id, casket_id));

				if (fetcher.Fetch(this, msg) == null) {
					throw new ClientException(EClientExceptionType.Timeout, "Request timed out");
				}

				return true;				
			} finally {
				GCSemaphore.Release();
			}
		}
	}

	[Flags]
	internal enum EClientStatus : byte {
		None = 0,
		Connected = 1,
		Ready = 2
	}
}