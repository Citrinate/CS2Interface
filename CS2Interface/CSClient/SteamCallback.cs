using System;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	public class SteamCallback {
		public sealed class ClientMicroTxnAuthRequestCallback : CallbackMsg {
			public SteamMessage.ClientMicroTxnAuthRequest Body;

			internal ClientMicroTxnAuthRequestCallback(IPacketMsg packetMsg) {
				var response = new ClientMsg<SteamMessage.ClientMicroTxnAuthRequest>(packetMsg);
				Body = response.Body;
			}
		}

		public class SteamHandler : ClientMsgHandler {
			private static CallbackMsg? GetCallback(IPacketMsg packetMsg) => packetMsg.MsgType switch {
				EMsg.ClientMicroTxnAuthRequest => new ClientMicroTxnAuthRequestCallback(packetMsg),
				_ => null
			};

			public override void HandleMsg(IPacketMsg packetMsg) {
				var callback = GetCallback(packetMsg);

#if DEBUG
				// ASF.ArchiLogger.LogGenericDebug(String.Format("Steam {0}: {1}", Strings.MessageRecieved, packetMsg.MsgType));
#endif

				if (callback == null) {
					return;
				}

				this.Client.PostCallback(callback);
			}
		}
	}
}