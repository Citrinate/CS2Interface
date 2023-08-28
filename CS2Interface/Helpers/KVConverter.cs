using System;
using ArchiSteamFarm.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValveKeyValue;

namespace CS2Interface {
    public sealed class KVConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return objectType == typeof(KVObject);
		}
			
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
			JToken? json = null;
			if (value is KVObject vdf) {
				json = ConvertKVObjectToJson(vdf);
			}

			if (json == null) {
				writer.WriteNull();

				return;
			}

			json.WriteTo(writer);
		}

		private JToken? ConvertKVObjectToJson (KVObject vdf) {
			if (vdf.Value.ValueType == KVValueType.Collection) {
				JObject json = new();
				foreach (KVObject child in vdf.Children) {
					try {
						json.Add(child.Name, ConvertKVObjectToJson(child));
					} catch (Exception e) {
						// item["523"] (Talon Knife) has duplicates of "inventory_image_data", just ignore the duplicates
						ASF.ArchiLogger.LogGenericError(e.Message);
					}
				}

				return json;
			}

			return vdf.Value.GetTypeCode() switch {
				TypeCode.Int32 => (int) vdf.Value,
				TypeCode.Int64 => (long) vdf.Value,
				TypeCode.UInt64 => vdf.Value.ToString(),
				TypeCode.Single => (float) vdf.Value,
				TypeCode.String => vdf.Value.ToString(),
				_ => null
			};
		}
			
		public override bool CanRead {
			get { return false; }
		}
			
		public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
			throw new NotImplementedException();
		}			
	}
}