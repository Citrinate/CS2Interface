using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SteamKit2;

namespace CS2Interface {
	internal abstract class GameDataResource {
		protected Uri Url;
		internal bool Updated {get; set;} = false;

		internal GameDataResource(string url) {
			Url = new Uri(url);
		}

		internal abstract Task<bool> Update();

		protected async Task<KeyValue?> FetchKVResource() {
			HttpClient httpClient = new();
			using (Stream response = await httpClient.GetStreamAsync(Url).ConfigureAwait(false)) {
				KeyValue kv = new KeyValue();
				if (!kv.ReadAsText(response)) {
					return null;
				}

				return kv;
			}
		}

		protected async Task<Dictionary<string, string>?> FetchCDNResource() {
			HttpClient httpClient = new();
			Dictionary<string, string> kvp = new();
			using (Stream response = await httpClient.GetStreamAsync(Url).ConfigureAwait(false)) {
				using (StreamReader sr = new StreamReader(response)) {
					string? line;
					while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null) {
						string[] kv = line.Split("=");
						if (kv.Length == 2) {
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