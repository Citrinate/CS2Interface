using System;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	internal class GameDataItems : GameDataResource {
		internal KeyValue? Data {get; private set;}

		internal GameDataItems(string url) : base(url) {}

		internal override async Task<bool> Update() {
			KeyValue? data = await FetchKVResource().ConfigureAwait(false);
			if (data == null) {
				ASF.ArchiLogger.LogGenericError(String.Format(Strings.GameDataSourceFailed, Url));

				return false;
			}

			// Combine any duplicated top level names
			KeyValue mergedData = new();
			foreach (KeyValue kv in data.Children) {
				if (kv.Name == null) {
					continue;
				}

				if (mergedData[kv.Name] == KeyValue.Invalid) {
					mergedData[kv.Name] = kv.Clone();
				} else {
					mergedData[kv.Name].Merge(kv, mergeDepth: 0);
				}
			}

			Data = mergedData;
			Updated = true;

			return true;
		}

		internal KeyValue this[string? key] {
			get {
				if (key == null || Data == null) {
					return KeyValue.Invalid;
				}

				return Data[key];
			}
		}
	}
}