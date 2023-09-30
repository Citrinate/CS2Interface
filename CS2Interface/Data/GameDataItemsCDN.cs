using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;

namespace CS2Interface {
	internal class GameDataItemsCDN : GameDataResource {
		private Dictionary<string, string>? Data;

		internal GameDataItemsCDN(string url) : base(url) {}

		internal async Task<bool> Update() {
			Dictionary<string, string>? data = await FetchCDNResource().ConfigureAwait(false);
			if (data == null) {
				ASF.ArchiLogger.LogGenericError(String.Format("Couldn't load game data from: {0}", Url));

				return false;
			}

			Data = data;
			Updated = true;

			return true;
		}

		internal string? this[string? key] {
			get {
				if (key == null || Data == null) {
					return null;
				}

				return Data[key];
			}
		}
	}
}