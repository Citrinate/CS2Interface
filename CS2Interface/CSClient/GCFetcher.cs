using System;
using ProtoBuf;
using SteamKit2;
using SteamKit2.GC;

namespace CS2Interface {
	internal sealed class GCFetcher<TMsg, TResponse> where TMsg : IExtensible, new() where TResponse : IExtensible, new() {
		internal int TTLSeconds { get; set; } = 30;
		internal uint GCResponseMsgType { get; set; } 
		internal Func<ClientGCMsgProtobuf<TResponse>, bool>? VerifyFunc { get; set; } 
		private bool GotMatch = false;
		IPacketGCMsg? PacketMsg;

		internal GCFetcher() {}

		internal GCFetcher(uint gcResponseMsgType, int? ttlSeconds = null, Func<ClientGCMsgProtobuf<TResponse>, bool>? verify = null) {
			GCResponseMsgType = gcResponseMsgType;
			TTLSeconds = ttlSeconds ?? TTLSeconds;
			VerifyFunc = verify;
		}

		internal ClientGCMsgProtobuf<TResponse>? Fetch(Client client, ClientGCMsgProtobuf<TMsg> msg, bool resendMsg = false) {
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
			if (!GotMatch || PacketMsg == null) {
				return null;
			}

			return new ClientGCMsgProtobuf<TResponse>(PacketMsg);
		}

		internal void CheckMatch(SteamGameCoordinator.MessageCallback callback) {
			if (callback.EMsg != GCResponseMsgType) {
				return;
			}

			if (VerifyFunc != null && !VerifyFunc(new ClientGCMsgProtobuf<TResponse>(callback.Message))) {
				return;
			}

			GotMatch = true;
			PacketMsg = callback.Message;
		}
	}
}