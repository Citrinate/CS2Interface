using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using SteamKit2;

namespace CS2Interface {
	internal class GameDataText : GameDataResource {
		private List<KeyValue>? Data;

		internal GameDataText(string url) : base(url) {}

		internal async Task<bool> Update() {
			KeyValue? data = await FetchKVResource().ConfigureAwait(false);
			if (data == null) {
				ASF.ArchiLogger.LogGenericError(String.Format("Couldn't load game data from: {0}", Url));

				return false;
			}

			Data = data.Children.Where(x => x.Name == "Tokens").SelectMany(x => x.Children).ToList();
			Updated = true;

			return true;
		}

		internal string? this[string? key] {
			get {
				if (key == null || Data == null) {
					return null;
				}

				return Data.Where(x => x.Name?.ToUpper().Trim() == key.ToUpper().Trim()).FirstOrDefault()?.Value;
			}
		}

		internal string? Format(string? key, params string?[] inserts) {
			string? value = this[key];
			if (value == null) {
				return null;
			}

			// Convert strings used in C++ printf into strings we can use for C# String.Format
			// Ex: "%s1 %s2 %s3" to "{0} {1} {2}"
			// Only care about the %s format specifier, which can only go up to %s9? https://github.com/nillerusr/source-engine/blob/29985681a18508e78dc79ad863952f830be237b6/tier1/ilocalize.cpp#L74

			try {
				StringBuilder sb = new(value);

				sb.Replace("%s1", "{0}");
				sb.Replace("%s2", "{1}");
				sb.Replace("%s3", "{2}");
				sb.Replace("%s4", "{3}");
				sb.Replace("%s5", "{4}");
				sb.Replace("%s6", "{5}");
				sb.Replace("%s7", "{6}");
				sb.Replace("%s8", "{7}");
				sb.Replace("%s9", "{8}");

				return String.Format(sb.ToString(), inserts);
			} catch {
				return null;
			}
		}
	}
}