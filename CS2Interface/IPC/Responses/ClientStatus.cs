using System;
using System.Collections.Generic;
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
		[JsonPropertyName("UnprotectedInventorySize")]
		public int? UnprotectedInventorySize;

		[JsonInclude]
		[JsonPropertyName("AutoStopAt")]
		public DateTime? AutoStopAt;

		[JsonInclude]
		[JsonPropertyName("Message")]
		public string Message;

		[JsonInclude]
		[JsonPropertyName("Version")]
		public string Version;

		public bool ShouldSerializeInventorySize() => InventorySize != null;
		public bool ShouldSerializeUnprotectedInventorySize() => UnprotectedInventorySize != null;
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
			UnprotectedInventorySize = !InventoryLoaded || client?.Inventory == null ? null : client.Inventory.Values.Where(x => x.IsVisible() && x.CasketID == null && x.Attributes?.GetValueOrDefault("trade protected escrow date")?.ToUInt32() == null).Count();
			Message = message;
			Version = (typeof(CS2Interface).Assembly.GetName().Version ?? new Version("0")).ToString();
		}
	}
}
