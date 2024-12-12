using System;
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

		internal ClientGCMsgProtobuf<TResponse>? Fetch<TResponse>(Client client, IClientGCMsg msg, bool resendMsg = false) where TResponse : IExtensible, new() {
			GetResponse(client, msg, resendMsg);

			if (!GotMatch || PacketMsg == null) {
				return null;
			}

			return new ClientGCMsgProtobuf<TResponse>(PacketMsg);
		}

		internal ClientGCMsg<TResponse>? RawFetch<TResponse>(Client client, IClientGCMsg msg, bool resendMsg = false) where TResponse : IGCSerializableMessage, new() {
			GetResponse(client, msg, resendMsg);

			if (!GotMatch || PacketMsg == null) {
				return null;
			}

			return new ClientGCMsg<TResponse>(PacketMsg);
		}

		private void GetResponse(Client client, IClientGCMsg msg, bool resendMsg = false) {
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
					client.CallbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
				} catch {
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