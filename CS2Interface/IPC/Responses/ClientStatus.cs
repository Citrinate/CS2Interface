using System.Text.Json.Serialization;

namespace CS2Interface.IPC {
	public sealed class ClientStatus {
		[JsonInclude]
		[JsonPropertyName("Connected")]
		public bool Connected;

		[JsonInclude]
		[JsonPropertyName("Connecting")]
		public bool Connecting;

		[JsonInclude]
		[JsonPropertyName("InventoryLoaded")]
		public bool InventoryLoaded;

		[JsonInclude]
		[JsonPropertyName("Message")]
		public string Message;

		internal ClientStatus((Client? client, string message) @paramsA, (EClientStatus status, string message) @paramsB) : this(@paramsA.client, @paramsB.status, @paramsB.message) {}

		internal ClientStatus(Client? client, EClientStatus status, string message) {
			Connected = (status & EClientStatus.Connected) == EClientStatus.Connected;
			bool ready = (status & EClientStatus.Ready) == EClientStatus.Ready;
			bool botOffline = (status & EClientStatus.BotOffline) == EClientStatus.BotOffline;
			Connecting = !Connected && !ready && !botOffline;
			InventoryLoaded = client != null && client.InventoryLoaded;
			Message = message;
		}
	}
}
