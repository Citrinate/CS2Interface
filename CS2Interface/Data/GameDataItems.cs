using System;
using System.Numerics;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ValveKeyValue;

namespace CS2Interface {
	internal class GameDataItems : GameDataResource {
        private KVObject? Data;

        internal GameDataItems(string url) : base(url) {}

        internal async Task<bool> Update() {
            KVObject? data = await FetchKVResource().ConfigureAwait(false);
            if (data == null) {
				ASF.ArchiLogger.LogGenericError(String.Format("Couldn't load game data from: {0}", Url));

                return false;
            }

            Data = data;
            Updated = true;

            return true;
        }

        internal KVObject? this[string? key] {
            get {
                if (key == null || Data == null) {
                    return null;
                }

                return Data.Search(key);
            }
        }

		internal KVObject? GetDef(string value, string index, bool suppressErrorLogs = false) {
            if (Data == null) {
                return null;
            }

            var def = this[value]?[index];
			if (def == null) {
                if (!suppressErrorLogs) {
				    ASF.ArchiLogger.LogGenericError(String.Format("Couldn't find definition: {0}[{1}]", value, index));
                }
                
				return null;
			}

			return new KVObject(value, def);
		}
    }
}