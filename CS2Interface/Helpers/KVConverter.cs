using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ArchiSteamFarm.Core;
using SteamKit2;

namespace CS2Interface {
	public sealed class KVConverter : JsonConverter<KeyValue> {
		public override KeyValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, KeyValue value, JsonSerializerOptions options) {
			ConvertKVObjectToJson(ref writer, value);
		}

		private void ConvertKVObjectToJson (ref Utf8JsonWriter writer, KeyValue vdf) {
			if (vdf.Children.Count > 0) {
				writer.WriteStartObject();
				foreach (KeyValue child in vdf.Children) {
					if (child.Name == null) {
						continue;
					}

					try {
						writer.WritePropertyName(child.Name);
						ConvertKVObjectToJson(ref writer, child);
					} catch (Exception e) {
						// item["523"] (Talon Knife) has duplicates of "inventory_image_data", just ignore the duplicates
						ASF.ArchiLogger.LogGenericException(e);
					}
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

			if (float.TryParse(vdf.Value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float floatValue)) {
				writer.WriteNumberValue(floatValue);

				return;
			}

			writer.WriteStringValue(vdf.Value);
		}
	}

	// https://github.com/dotnet/runtime/issues/54189#issuecomment-861628532
	public class JsonListItemConverter<TDatatype, TConverterType> : JsonConverter<List<TDatatype>> where TConverterType : JsonConverter {
		public override List<TDatatype> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, List<TDatatype> value, JsonSerializerOptions options) {
			if (value == null) {
				writer.WriteNullValue();
				return;
			}

			JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions(options);
			jsonSerializerOptions.Converters.Clear();
			jsonSerializerOptions.Converters.Add(Activator.CreateInstance<TConverterType>());

			writer.WriteStartArray();

			foreach (TDatatype data in value) {
				JsonSerializer.Serialize(writer, data, jsonSerializerOptions);
			}

			writer.WriteEndArray();
		}
	}
}