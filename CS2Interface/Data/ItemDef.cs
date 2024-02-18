using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SteamKit2;

namespace CS2Interface {
	internal class ItemDef {
		[JsonProperty(PropertyName = "defs", ItemConverterType = typeof(KVConverter))]
		internal List<KeyValue> Defs = new();

		internal ItemDef(KeyValue? def) {
			AddDef(def);
		}

		internal void AddDef(KeyValue? def) {
			if (def == null) {
				throw new ArgumentNullException();
			}

			Defs.Add(def);
		}

		internal string? GetValue(params string[] keys) {
			return GetValue(Defs, keys)?.Value;
		}

		private KeyValue? GetValue(IEnumerable<KeyValue> defs, IEnumerable<string> keys) {
			if (defs.Count() == 0) {
				return null;
			}

			string key = keys.First();

			if (keys.Count() == 1) {
				return defs.FirstOrDefault(def => def[key] != KeyValue.Invalid)?[key];
			}

			return GetValue(defs.Where(def => def[key] != KeyValue.Invalid).Select(def => def[key]), keys.Skip(1).ToArray());
		}
	}
}