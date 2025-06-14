using System;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.Internal;

namespace CS2Interface {
	internal sealed class GCFetcher {
		internal int TTLSeconds { private get; init; } = 30;
		internal uint GCResponseMsgType { private get; init; } 
		internal Func<IPacketGCMsg, bool>? VerifyResponse { private get; init; }
		private CancellationTokenSource? ResponseWaitCancellation;
		private IPacketGCMsg? PacketMsg;

		internal GCFetcher() {}

		internal async Task<ClientGCMsgProtobuf<TResponse>?> Fetch<TResponse>(Client client, IClientGCMsg msg) where TResponse : IExtensible, new() {
			await GetResponse(client, msg).ConfigureAwait(false);

			if (PacketMsg == null) {
				return null;
			}

			return new ClientGCMsgProtobuf<TResponse>(PacketMsg);
		}

		internal async Task<ClientGCMsg<TResponse>?> RawFetch<TResponse>(Client client, IClientGCMsg msg) where TResponse : IGCSerializableMessage, new() {
			await GetResponse(client, msg).ConfigureAwait(false);

			if (PacketMsg == null) {
				return null;
			}

			return new ClientGCMsg<TResponse>(PacketMsg);
		}

		private async Task GetResponse(Client client, IClientGCMsg msg) {
			client.OnGCMessageRecieved += CheckMatch;
			client.GameCoordinator.Send(msg, Client.AppID);

			ResponseWaitCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(TTLSeconds));

			try {
				await Task.Delay(TimeSpan.FromSeconds(TTLSeconds), ResponseWaitCancellation.Token);
			} catch (OperationCanceledException) {
				;
			} finally {
				client.OnGCMessageRecieved -= CheckMatch;

				ResponseWaitCancellation?.Dispose();
				ResponseWaitCancellation = null;
			}
		}

		internal void CheckMatch(SteamGameCoordinator.MessageCallback callback) {
			if (callback.EMsg != GCResponseMsgType) {
				return;
			}

			if (VerifyResponse != null && !VerifyResponse(callback.Message)) {
				return;
			}

			PacketMsg = callback.Message;
			ResponseWaitCancellation?.Cancel();
		}
	}
}