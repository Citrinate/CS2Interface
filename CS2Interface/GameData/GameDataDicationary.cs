using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;

namespace CS2Interface {
	internal class GameDataDictionary : GameDataResource {
		private Dictionary<string, string>? Data;

		internal GameDataDictionary(string url) : base(url) {}

		internal override async Task<bool> Update() {
			Dictionary<string, string>? data = await FetchCDNResource().ConfigureAwait(false);
			if (data == null) {
				ASF.ArchiLogger.LogGenericError(String.Format(Strings.GameDataSourceFailed, Url));

				return false;
			}

			Data = data;
			Updated = true;

			return true;
		}

		internal string? this[string? key] {
			get {
				if (key == null || Data == null || !Data.ContainsKey(key)) {
					return null;
				}

				return Data[key];
			}
		}
	}
}