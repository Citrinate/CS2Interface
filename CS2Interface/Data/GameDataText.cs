using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using SteamKit2;

namespace CS2Interface {
	internal class GameDataText : GameDataResource {
		private List<KeyValue>? Data;

		internal GameDataText(string url) : base(url) {}

		internal async Task<bool> Update() {
			List<KeyValue>? data = (await FetchKVResource().ConfigureAwait(false))?.Search("Tokens");
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

				return Data.Where(x => x.Name?.ToUpper().Trim() == key.ToUpper().Trim()).FirstOrDefault()?.Value;
			}
		}
	}
}