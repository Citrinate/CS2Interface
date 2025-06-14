using System;
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
		[JsonPropertyName("AutoStopAt")]
		public DateTime? AutoStopAt;

		[JsonInclude]
		[JsonPropertyName("Message")]
		public string Message;

		public bool ShouldSerializeInventorySize() => InventorySize != null;
		public bool ShouldSerializeAutoStopAt() => AutoStopAt != null;

		internal ClientStatus(ClientHandler handler) {
			(Client? client, string _) = handler.GetClient();
			(EClientStatus status, string message) = handler.Status();
			bool ready = (status & EClientStatus.Ready) == EClientStatus.Ready;

			Connected = (status & EClientStatus.Connected) == EClientStatus.Connected;
			Connecting = !Connected && !ready;
			AutoStopAt = !Connected ? null : handler.AutoStop.GetScheduledTime();
			InventoryLoaded = Connected && client?.InventoryLoaded == true;
			InventorySize = !InventoryLoaded || client?.Inventory == null ? null : client.Inventory.Values.Where(x => x.IsVisible() && x.CasketID == null).Count();
			Message = message;
		}
	}
}
