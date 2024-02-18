using System;
using System.Globalization;
using ArchiSteamFarm.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace CS2Interface {
	public sealed class KVConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return objectType == typeof(KeyValue);
		}
			
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
			JToken? json = null;
			if (value is KeyValue vdf) {
				json = ConvertKVObjectToJson(vdf);
			}

			if (json == null) {
				writer.WriteNull();

				return;
			}

			json.WriteTo(writer);
		}

		private JToken? ConvertKVObjectToJson (KeyValue vdf) {
			if (vdf.Children.Count > 0) {
				JObject json = new();
				foreach (KeyValue child in vdf.Children) {
					if (child.Name == null) {
						continue;
					}

					try {
						json.Add(child.Name, ConvertKVObjectToJson(child));
					} catch (Exception e) {
						// item["523"] (Talon Knife) has duplicates of "inventory_image_data", just ignore the duplicates
						ASF.ArchiLogger.LogGenericException(e);
					}
				}

				return json;
			}

			if (int.TryParse(vdf.Value, out int intValue)) {
				return intValue;
			}

			if (long.TryParse(vdf.Value, out long longValue)) {
				return longValue;
			}

			if (float.TryParse(vdf.Value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float floatValue)) {
				return floatValue;
			}

			return vdf.Value;
		}
			
		public override bool CanRead {
			get { return false; }
		}
			
		public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
			throw new NotImplementedException();
		}			
	}
}