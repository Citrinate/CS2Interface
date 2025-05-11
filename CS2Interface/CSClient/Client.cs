using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
using CS2Interface.Localization;
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
		internal bool InventoryLoaded = false;
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
				throw new ClientException(EClientExceptionType.Failed, Strings.ClientAlreadyStarting);
			}

			if (FatalError) {
				throw new ClientException(EClientExceptionType.FatalError, Strings.ClientFatalError);
			}

			await ConnectionSemaphore.WaitAsync().ConfigureAwait(false);
			try {
				if (!await GameData.IsLoaded(0).ConfigureAwait(false)) {
					throw new ClientException(EClientExceptionType.Failed, Strings.GameDataLoadingFailed);
				}

				// TODO: Verify that this account owns CS2

				(bool play_success, string play_message) = await Bot.Actions.Play(new HashSet<uint> { AppID }).ConfigureAwait(false);
				Bot.ArchiLogger.LogGenericInfo(play_message);
				if (!play_success) {
					throw new ClientException(EClientExceptionType.Failed, play_message);
				}

				await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

				var msg = new ClientGCMsgProtobuf<CMsgClientHello>((uint) EGCBaseClientMsg.k_EMsgGCClientHello) { Body = {
					version = 2000244,
					client_session_need = 0,
					client_launcher = 0,
					steam_launcher = 0
				}};
				// var fetcher = new GCFetcher<CMsgClientHello, CMsgClientWelcome>((uint) EGCBaseClientMsg.k_EMsgGCClientWelcome);
				var fetcher = new GCFetcher((uint) EGCBaseClientMsg.k_EMsgGCClientWelcome);

				Bot.ArchiLogger.LogGenericDebug(Strings.SendingHello);

				if (await fetcher.Fetch<CMsgClientWelcome>(this, msg, resendMsg: true).ConfigureAwait(false) == null) {
					throw new ClientException(EClientExceptionType.Timeout, Strings.GCConnectionFailed);
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

			Bot.ArchiLogger.LogGenericDebug(Strings.VerifyingGCConnection);
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
			
#if DEBUG
			Bot.ArchiLogger.LogGenericDebug(String.Format("{0}: {1}", Strings.MessageRecieved, callback.EMsg));
#endif

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
					
					InventoryLoaded = true;
					Bot.ArchiLogger.LogGenericDebug(Strings.InventoryLoaded);
					
					return;
				}
			}
		}

		private void OnFatalLogonError(IPacketGCMsg packetMsg) {
			var msg = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientLogonFatalError>(packetMsg);

			Bot.ArchiLogger.LogGenericError(String.Format("{0}: {1}", String.Format(Strings.FatalLogonError, msg.Body.errorcode), msg.Body.message));
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
				throw new ClientException(EClientExceptionType.Failed, Strings.ClientNotConnectedToGC);
			}

			await GCSemaphore.WaitAsync().ConfigureAwait(false);

			try {
				var msg = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockRequest>((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockRequest) { Body = {
					param_s = param_s,
					param_a = param_a,
					param_d = param_d,
					param_m = param_m
				}};

				var fetcher = new GCFetcher {
					GCResponseMsgType = (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse,
					VerifyResponse = message => {
						var response = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse>(message);
					
						return response.Body.iteminfo.itemid == param_a;
					}
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format("{0}: s {1} a {2} d {3} m {4}", Strings.InspectingItem, param_s, param_a, param_d, param_m));

				var response = await fetcher.Fetch<CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse>(this, msg).ConfigureAwait(false);
				if (response == null) {
					throw new ClientException(EClientExceptionType.Timeout, Strings.RequestTimeout);
				}

				return response.Body;
			} finally {
				GCSemaphore.Release();
			}
		}

		internal async Task<CMsgGCCStrike15_v2_PlayersProfile> RequestPlayerProfile(ulong steam_id) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, Strings.ClientNotConnectedToGC);
			}

			SteamID SteamID = new(steam_id);
			if (!SteamID.IsValid || SteamID.AccountUniverse != EUniverse.Public || SteamID.AccountType != EAccountType.Individual || SteamID.AccountInstance != SteamID.DesktopInstance) {
				throw new ClientException(EClientExceptionType.BadRequest, Strings.InvalidSteamID);
			}

			await GCSemaphore.WaitAsync().ConfigureAwait(false);

			try {
				uint account_id = SteamID.AccountID;
				var msg = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientRequestPlayersProfile>((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientRequestPlayersProfile) { Body = {
					account_id = account_id,
					request_level = 32
				}};

				var fetcher = new GCFetcher{
					GCResponseMsgType = (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_PlayersProfile,
					VerifyResponse = message => {
						var response = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_PlayersProfile>(message);
						
						return response.Body.account_profiles.FirstOrDefault()?.account_id == account_id;
					}
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format("{0}: {1}", Strings.InspectingPlayer, steam_id));

				var response = await fetcher.Fetch<CMsgGCCStrike15_v2_PlayersProfile>(this, msg).ConfigureAwait(false);
				if (response == null) {
					throw new ClientException(EClientExceptionType.Timeout, Strings.RequestTimeout);
				}

				return response.Body;
			} finally {
				GCSemaphore.Release();
			}
		}

		internal async Task<List<InventoryItem>> GetCasketContents(ulong casket_id) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, Strings.ClientNotConnectedToGC);
			}

			if (!InventoryLoaded || Inventory == null) {
				throw new ClientException(EClientExceptionType.Failed, Strings.InventoryNotLoaded);
			}

			InventoryItem? casket = Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == casket_id);
			if (casket == null) {
				throw new ClientException(EClientExceptionType.BadRequest, Strings.CasketNotFound);
			}

			uint? items_count = casket.Attributes?.GetValueOrDefault("items count")?.ToUInt32();
			if (items_count == null) {
				throw new ClientException(EClientExceptionType.Failed, Strings.CasketContentsUndefined);
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

				var fetcher = new GCFetcher{
					GCResponseMsgType = (uint) EGCItemMsg.k_EMsgGCItemCustomizationNotification,
					VerifyResponse = message => {
						var response = new ClientGCMsgProtobuf<CMsgGCItemCustomizationNotification>(message);

						return response.Body.item_id.FirstOrDefault() == casket_id && response.Body.request == (uint) EGCItemCustomizationNotification.k_EGCItemCustomizationNotification_CasketContents;
					}
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format(Strings.OpeningCasket, casket_id));

				if (await fetcher.Fetch<CMsgGCItemCustomizationNotification>(this, msg).ConfigureAwait(false) == null) {
					throw new ClientException(EClientExceptionType.Timeout, Strings.RequestTimeout);
				}

				// Casket contents can sometimes continue to come in after we've recieved the k_EMsgGCItemCustomizationNotification response
				DateTime waitTime = DateTime.Now.AddSeconds(30);
				while (Inventory.Values.Where(x => x.CasketID == casket_id).Count() != items_count && DateTime.Now < waitTime) {
					Bot.ArchiLogger.LogGenericDebug(String.Format(Strings.CasketContentsLoading, casket_id));
					await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
				}

				if (Inventory.Values.Where(x => x.CasketID == casket_id).Count() != items_count) {
					throw new ClientException(EClientExceptionType.Failed, String.Format(Strings.CasketItemCountMismatch, items_count, Inventory.Values.Where(x => x.CasketID == casket_id).Count()));
				}

				return Inventory.Values.Where(x => x.CasketID == casket_id).ToList();
			} finally {
				GCSemaphore.Release();
			}
		}

		internal async Task<bool> AddItemToCasket(ulong casket_id, ulong item_id) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, Strings.ClientNotConnectedToGC);
			}

			if (!InventoryLoaded || Inventory == null) {
				throw new ClientException(EClientExceptionType.Failed, Strings.InventoryNotLoaded);
			}

			InventoryItem? item = Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == item_id);
			if (item == null) {
				throw new ClientException(EClientExceptionType.BadRequest, String.Format(Strings.InventoryItemNotFound, item_id));
			}

			if (item.CasketID != null) {
				throw new ClientException(EClientExceptionType.BadRequest, String.Format(Strings.InventoryItemFoundInCrate, item_id));
			}

			InventoryItem? casket = Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == casket_id);
			if (casket == null) {
				throw new ClientException(EClientExceptionType.BadRequest, Strings.CasketNotFound);
			}

			uint? items_count = casket.Attributes?.GetValueOrDefault("items count")?.ToUInt32();
			if (items_count == null) {
				throw new ClientException(EClientExceptionType.Failed, Strings.CasketContentsUndefined);
			} else if (items_count == 1000) {
				throw new ClientException(EClientExceptionType.BadRequest, Strings.CasketFull);
			}

			var msg = new ClientGCMsgProtobuf<CMsgCasketItem>((uint) EGCItemMsg.k_EMsgGCCasketItemAdd) { Body = {
				casket_item_id = casket_id,
				item_item_id = item_id
			}};

			var fetcher = new GCFetcher{
				GCResponseMsgType = (uint) ESOMsg.k_ESOMsg_Destroy,
				TTLSeconds = 10,
				VerifyResponse = message => {
					var response = new ClientGCMsgProtobuf<CMsgSOSingleObject>(message);

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

			Bot.ArchiLogger.LogGenericDebug(String.Format(Strings.AddingItemToCasket, item_id, casket_id));

			if (await fetcher.Fetch<CMsgSOSingleObject>(this, msg).ConfigureAwait(false) == null) {
				throw new ClientException(EClientExceptionType.Timeout, Strings.RequestTimeout);
			}

			return true;
		}
		
		internal async Task<bool> RemoveItemFromCasket(ulong casket_id, ulong item_id) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, Strings.ClientNotConnectedToGC);
			}

			if (!InventoryLoaded || Inventory == null) {
				throw new ClientException(EClientExceptionType.Failed, Strings.InventoryNotLoaded);
			}

			InventoryItem? casket = Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == casket_id);
			if (casket == null) {
				throw new ClientException(EClientExceptionType.BadRequest, Strings.CasketNotFound);
			}

			// Does not verify that the item is actually in the crate, to do that we would need to request the crate contents first
			var msg = new ClientGCMsgProtobuf<CMsgCasketItem>((uint) EGCItemMsg.k_EMsgGCCasketItemExtract) { Body = {
				casket_item_id = casket_id,
				item_item_id = item_id
			}};

			var fetcher = new GCFetcher{
				GCResponseMsgType = (uint) ESOMsg.k_ESOMsg_Create,
				TTLSeconds = 10,
				VerifyResponse = message => {
					var response = new ClientGCMsgProtobuf<CMsgSOSingleObject>(message);

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

			Bot.ArchiLogger.LogGenericDebug(String.Format(Strings.RemovingItemFromCasket, item_id, casket_id));

			if (await fetcher.Fetch<CMsgSOSingleObject>(this, msg).ConfigureAwait(false) == null) {
				throw new ClientException(EClientExceptionType.Timeout, Strings.RequestTimeout);
			}

			return true;
		}
		
		internal async Task<GCMsg.MsgCraftResponse> Craft(ushort recipe, List<ulong> item_ids) {
			if (!HasGCSession) {
				throw new ClientException(EClientExceptionType.Failed, Strings.ClientNotConnectedToGC);
			}

			if (!InventoryLoaded || Inventory == null) {
				throw new ClientException(EClientExceptionType.Failed, Strings.InventoryNotLoaded);
			}

			if (item_ids.Count > ushort.MaxValue) {
				throw new ClientException(EClientExceptionType.BadRequest, Strings.InvalidCraftTooManyInputs);
			}

			{
				HashSet<ulong> duplicateIDCheck = new();
				foreach (ulong item_id in item_ids) {
					InventoryItem? inventoryItem = Inventory.Values.FirstOrDefault(x => x.ItemInfo.id == item_id);
					
					if (inventoryItem == null) {
						throw new ClientException(EClientExceptionType.BadRequest, String.Format(Strings.InventoryItemNotFound, item_id));
					}

					if (inventoryItem.CasketID != null) {
						throw new ClientException(EClientExceptionType.BadRequest, String.Format(Strings.InventoryItemFoundInCrate, item_id));
					}

					if (inventoryItem.Moveable != true) {
						throw new ClientException(EClientExceptionType.BadRequest, String.Format(Strings.InvalidCraftInput, item_id));
					}

					if (!duplicateIDCheck.Add(item_id)) {
						throw new ClientException(EClientExceptionType.BadRequest, String.Format(Strings.InvalidCraftDuplicateInput, item_id));
					}
				}
			}

			await GCSemaphore.WaitAsync().ConfigureAwait(false);
			
			try {
				var msg = new ClientGCMsg<GCMsg.MsgCraft>() {
					Body = {
						Recipe = recipe,
						ItemCount = (ushort) item_ids.Count,
						ItemIDs = item_ids
					}
				};

				var fetcher = new GCFetcher{
					GCResponseMsgType = (uint) EGCItemMsg.k_EMsgGCCraftResponse,
					VerifyResponse = message => {
						var response = new ClientGCMsg<GCMsg.MsgCraftResponse>(message);

						return response.Body.Recipe == recipe || response.Body.Recipe == GCMsg.MsgCraft.UnknownRecipe;
					}
				};

				Bot.ArchiLogger.LogGenericDebug(String.Format(Strings.CraftingItem, recipe, String.Join(", ", item_ids)));

				var response = await fetcher.RawFetch<GCMsg.MsgCraftResponse>(this, msg).ConfigureAwait(false);
				if (response == null) {
					throw new ClientException(EClientExceptionType.Timeout, Strings.RequestTimeout);
				}

				if (response.Body.Recipe == GCMsg.MsgCraft.UnknownRecipe) {
					throw new ClientException(EClientExceptionType.BadRequest, Strings.InvalidCraftRecipe);
				}

				return response.Body;
			} finally {
				GCSemaphore.Release();
			}
		}
	}

	[Flags]
	internal enum EClientStatus : byte {
		None = 0,
		Connected = 1,
		Ready = 2,
		BotOffline = 4
	}
}