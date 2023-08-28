using System;
using System.Collections.Generic;
using ArchiSteamFarm.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CS2Interface {
	public sealed class AttributeConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return objectType == typeof(Dictionary<string, IAttribute>);
		}
			
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
			JObject json = new();
			if (value is Dictionary<string, IAttribute> attributes) {
				foreach (var kvp in attributes) {
					var attribute = kvp.Value;
					try {
						if (attribute.Type == typeof(uint)) {
							json.Add(attribute.Name, attribute.ToUInt32());
						} if (attribute.Type == typeof(float)) {
							json.Add(attribute.Name, attribute.ToSingle());
						} else if (attribute.Type == typeof(string)) {
							json.Add(attribute.Name, attribute.ToString());
						}
					} catch (Exception e) {
						ASF.ArchiLogger.LogGenericError(e.Message);
					}
				}
			}

			json.WriteTo(writer);
		}

		public override bool CanRead {
			get { return false; }
		}

		public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
			throw new NotImplementedException();
		}		
	}
}