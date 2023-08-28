using System;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ValveKeyValue;

namespace CS2Interface {
	internal class GameDataText : GameDataResource {
        private KVObject? Data;

        internal GameDataText(string url) : base(url) {}

        internal async Task<bool> Update() {
            KVObject? data = (await FetchKVResource(new KVSerializerOptions { HasEscapeSequences = true, EnableValveNullByteBugBehavior = true }).ConfigureAwait(false))?.Search("Tokens");
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

                return Data.SearchFirst(key)?.Value.ToString();
            }
        }
    }
}