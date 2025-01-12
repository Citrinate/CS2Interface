using System.Text.Json.Serialization;
using SteamKit2;

namespace CS2Interface.IPC {
	public sealed class GameDataKV {
		[JsonInclude]
		[JsonPropertyName("ClientVersion")]
		public uint? ClientVersion;

		[JsonInclude]
		[JsonPropertyName("Data")]
		[JsonConverter(typeof(KVConverter))]
		public KeyValue Data;

		public GameDataKV(KeyValue data) {
			ClientVersion = GameData.ClientVersion;
			Data = data;
		}
	}
}
