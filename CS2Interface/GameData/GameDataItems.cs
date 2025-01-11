using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	internal class GameDataItems : GameDataResource {
		private KeyValue? Data;

		internal GameDataItems(string url) : base(url) {}

		internal override async Task<bool> Update() {
			KeyValue? data = await FetchKVResource().ConfigureAwait(false);
			if (data == null) {
				ASF.ArchiLogger.LogGenericError(String.Format(Strings.GameDataSourceFailed, Url));

				return false;
			}

			Data = data;
			Updated = true;

			return true;
		}

		internal List<KeyValue>? this[string? key] {
			get {
				if (key == null || Data == null) {
					return null;
				}

				return Data.Children.Where(x => x.Name == key).SelectMany(x => x.Children).ToList();
			}
		}

		internal KeyValue? GetDef(string value, string index, bool suppressErrorLogs = false) {
			if (Data == null) {
				return null;
			}

			KeyValue? def = this[value]?.Where(x => x.Name?.ToUpper().Trim() == index.ToUpper().Trim()).FirstOrDefault();

			if (def == null) {
				if (!suppressErrorLogs) {
					ASF.ArchiLogger.LogGenericError(String.Format("{0}: {1}[{2}]", Strings.GameDataDefinitionUndefined, value, index));
				}
				
				return null;
			}

			return def;
		}
	}
}