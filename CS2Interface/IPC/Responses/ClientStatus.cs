using System.Linq;
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
		[JsonPropertyName("InventorySize")]
		public int? InventorySize;

		[JsonInclude]
		[JsonPropertyName("Message")]
		public string Message;
		
		public bool ShouldSerializeInventorySize() => InventorySize != null;

		internal ClientStatus((Client? client, string message) @paramsA, (EClientStatus status, string message) @paramsB) : this(@paramsA.client, @paramsB.status, @paramsB.message) { }

		internal ClientStatus(Client? client, EClientStatus status, string message) {
			Connected = (status & EClientStatus.Connected) == EClientStatus.Connected;
			bool ready = (status & EClientStatus.Ready) == EClientStatus.Ready;
			bool botOffline = (status & EClientStatus.BotOffline) == EClientStatus.BotOffline;
			Connecting = !Connected && !ready && !botOffline;
			InventoryLoaded = Connected && client != null && client.InventoryLoaded;
			InventorySize = !InventoryLoaded || client == null || client.Inventory == null ? null : client.Inventory.Values.Where(x => x.IsVisible() && x.CasketID == null).Count();
			Message = message;
		}
	}
}
