using System.Text.Json.Serialization;

namespace CS2Interface.IPC {
	public sealed class GameData<T> {
		[JsonInclude]
		[JsonPropertyName("ClientVersion")]
		public uint? ClientVersion;

		[JsonInclude]
		[JsonPropertyName("Data")]
		public T Data;

		public GameData(T data) {
			ClientVersion = GameData.ClientVersion;
			Data = data;
		}
	}
}
