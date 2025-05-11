using System;
using System.Threading;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ProtoBuf;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.Internal;

namespace CS2Interface {
	internal sealed class GCFetcher {
		internal int TTLSeconds { get; set; } = 30;
		internal uint GCResponseMsgType { get; set; } 
		internal Func<IPacketGCMsg, bool>? VerifyResponse { get; set; }
		private bool GotMatch = false;
		IPacketGCMsg? PacketMsg;

		internal GCFetcher() {}

		internal GCFetcher(uint gcResponseMsgType, int? ttlSeconds = null, Func<IPacketGCMsg, bool>? verifyResponse = null) {
			GCResponseMsgType = gcResponseMsgType;
			TTLSeconds = ttlSeconds ?? TTLSeconds;
			VerifyResponse = verifyResponse;
		}

		internal async Task<ClientGCMsgProtobuf<TResponse>?> Fetch<TResponse>(Client client, IClientGCMsg msg, bool resendMsg = false) where TResponse : IExtensible, new() {
			await GetResponse(client, msg, resendMsg).ConfigureAwait(false);

			if (!GotMatch || PacketMsg == null) {
				return null;
			}

			return new ClientGCMsgProtobuf<TResponse>(PacketMsg);
		}

		internal async Task<ClientGCMsg<TResponse>?> RawFetch<TResponse>(Client client, IClientGCMsg msg, bool resendMsg = false) where TResponse : IGCSerializableMessage, new() {
			await GetResponse(client, msg, resendMsg).ConfigureAwait(false);

			if (!GotMatch || PacketMsg == null) {
				return null;
			}

			return new ClientGCMsg<TResponse>(PacketMsg);
		}

		private async Task GetResponse(Client client, IClientGCMsg msg, bool resendMsg = false) {
			client.OnGCMessageRecieved += CheckMatch;
			client.GameCoordinator.Send(msg, Client.AppID);

			DateTime timeoutTime = DateTime.Now.AddSeconds(TTLSeconds);
			uint resendSeconds = 1;
			DateTime resendTime = DateTime.Now.AddSeconds(resendSeconds);
			while (!GotMatch && DateTime.Now < timeoutTime) {
				if (resendMsg && DateTime.Now > resendTime) {
					resendSeconds = Math.Min(60, resendSeconds * 2); // Exponential backoff
					resendTime = DateTime.Now.AddSeconds(resendSeconds);
					client.GameCoordinator.Send(msg, Client.AppID);
				}

				try {
					CancellationTokenSource cts = new CancellationTokenSource();
					cts.CancelAfter(TimeSpan.FromSeconds(1));
					await client.CallbackManager.RunWaitCallbackAsync(cts.Token).ConfigureAwait(false);
				}
				catch {
					// Sometimes get a "System.InvalidOperationException: Queue empty" exception here which can be ignored
				}
			}

			client.OnGCMessageRecieved -= CheckMatch;
		}

		internal void CheckMatch(SteamGameCoordinator.MessageCallback callback) {
			if (callback.EMsg != GCResponseMsgType) {
				return;
			}

			if (VerifyResponse != null && !VerifyResponse(callback.Message)) {
				return;
			}

			GotMatch = true;
			PacketMsg = callback.Message;
		}
	}
}