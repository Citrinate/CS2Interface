using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ValveKeyValue;

namespace CS2Interface {
	internal class GameDataResource {
        protected Uri Url;
        internal bool Updated {get; set;} = false;

        internal GameDataResource(string url) {
            Url = new Uri(url);
        }

        protected async Task<KVObject?> FetchKVResource(KVSerializerOptions? options = null) {
			HttpClient httpClient = new();
			using (Stream response = await httpClient.GetStreamAsync(Url).ConfigureAwait(false)) {
				KVSerializer serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);

				return serializer.Deserialize(response, options);
			}
		}

		protected async Task<Dictionary<string, string>?> FetchCDNResource() {
			HttpClient httpClient = new();
			Dictionary<string, string> kvp = new();
			using (Stream response = await httpClient.GetStreamAsync(Url).ConfigureAwait(false)) {
				using (StreamReader sr = new StreamReader(response)) {
					// https://github.com/dotnet/runtime/issues/68983
					while (!sr.EndOfStream) {
						string[]? kv = sr.ReadLine()?.Split("=");
						if (kv?.Length == 2) {
							kvp[kv[0]] = kv[1];
						}
					}
				}
			}

			if (kvp.Count == 0) {
				return null;
			}

			return kvp;
		}
    }
}