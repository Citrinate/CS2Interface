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
		[JsonPropertyName("Ready")]
		public bool Ready;

		[JsonInclude]
		[JsonPropertyName("Message")]
		public string Message;

		public ClientStatus((EClientStatus status, string message) @params) : this(@params.status, @params.message) {}

		public ClientStatus(EClientStatus status, string message) {
			Connected = (status & EClientStatus.Connected) == EClientStatus.Connected;
			Ready = (status & EClientStatus.Ready) == EClientStatus.Ready;
			bool botOffline = (status & EClientStatus.BotOffline) == EClientStatus.BotOffline;
			Connecting = !Connected && !Ready && !botOffline;
			Message = message;
		}
	}
}
