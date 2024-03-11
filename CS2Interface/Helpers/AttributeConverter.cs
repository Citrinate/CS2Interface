using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArchiSteamFarm.Core;

namespace CS2Interface {
	public sealed class AttributeConverter : JsonConverter<Dictionary<string, IAttribute>> {
		
		public override Dictionary<string, IAttribute> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, Dictionary<string, IAttribute> value, JsonSerializerOptions options) {
			writer.WriteStartObject();
			foreach (var kvp in value) {
				var attribute = kvp.Value;
				try {
					if (attribute.Type == typeof(uint)) {
						writer.WritePropertyName(attribute.Name);
						writer.WriteNumberValue(attribute.ToUInt32());
					} if (attribute.Type == typeof(float)) {
						writer.WritePropertyName(attribute.Name);
						writer.WriteNumberValue(attribute.ToSingle());
					} else if (attribute.Type == typeof(string)) {
						writer.WritePropertyName(attribute.Name);
						writer.WriteStringValue(attribute.ToString());
					}
				} catch (Exception e) {
					ASF.ArchiLogger.LogGenericException(e);
				}
			}
			writer.WriteEndObject();
		}
	}
}