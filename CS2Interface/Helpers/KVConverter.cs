using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using SteamKit2;

namespace CS2Interface {
	public sealed class KVConverter : JsonConverter<KeyValue> {
		public override KeyValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, KeyValue value, JsonSerializerOptions options) {
			ConvertKVObjectToJson(ref writer, value);
		}

		public static void ConvertKVObjectToJson (ref Utf8JsonWriter writer, KeyValue vdf) {
			if (vdf.Children.Count > 0) {
				writer.WriteStartObject();
				
				foreach (KeyValue child in vdf.Children) {
					if (child.Name == null) {
						continue;
					}

					writer.WritePropertyName(child.Name);
					ConvertKVObjectToJson(ref writer, child);
				}

				writer.WriteEndObject();

				return;
			}

			if (int.TryParse(vdf.Value, out int intValue)) {
				writer.WriteNumberValue(intValue);

				return;
			}

			if (long.TryParse(vdf.Value, out long longValue)) {
				writer.WriteNumberValue(longValue);

				return;
			}

			if (float.TryParse(vdf.Value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float floatValue) && !float.IsInfinity(floatValue)) {
				writer.WriteNumberValue(floatValue);

				return;
			}

			writer.WriteStringValue(vdf.Value);
		}
	}
}