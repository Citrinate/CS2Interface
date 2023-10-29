using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ValveKeyValue;

namespace CS2Interface {
	internal class ItemDef {
		[JsonProperty(PropertyName = "defs", ItemConverterType = typeof(KVConverter))]
		internal List<KVObject> Defs = new();

		internal ItemDef(KVObject? def) {
			AddDef(def);
		}

		internal void AddDef(KVObject? def) {
			if (def == null) {
				throw new ArgumentNullException();
			}

			Defs.Add(def);
		}

		internal KVValue? GetValue(params string[] keys) {
			return GetValue(Defs, keys);
		}

		private KVValue? GetValue(IEnumerable<KVObject> defs, IEnumerable<string> keys) {
			if (defs.Count() == 0) {
				return null;
			}

			string key = keys.First();

			if (keys.Count() == 1) {
				return defs.FirstOrDefault(def => def[key] != null)?[key];
			}

			return GetValue(defs.Where(def => def[key] != null).Select(def => new KVObject(key, def[key])), keys.Skip(1).ToArray());
		}
	}
}