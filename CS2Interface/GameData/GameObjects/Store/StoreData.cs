using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using SteamKit2;
using SteamKit2.GC.CSGO.Internal;

namespace CS2Interface {
	public class StoreData {
		[JsonInclude]
		[JsonPropertyName("result")]
		public int Result;

		[JsonInclude]
		[JsonPropertyName("price_sheet_version")]
		public uint PriceSheetVersion;

		[JsonInclude]
		[JsonPropertyName("price_sheet")]
		[JsonConverter(typeof(KVConverter))]
		public KeyValue PriceSheet = new();

		[JsonInclude]
		[JsonPropertyName("price_sheet_items")]
		public Dictionary<string, StoreItem> PriceSheetItems = new();

		public StoreData(CMsgStoreGetUserDataResponse storeUserDataDataResponse) {
			Result = storeUserDataDataResponse.result;
			PriceSheetVersion = storeUserDataDataResponse.price_sheet_version;

			using (var compressed = new MemoryStream(storeUserDataDataResponse.price_sheet)) {
				if (LzmaUtil.TryDecompress( compressed, static capacity => new MemoryStream(capacity), out var decompressed)) {
					using (decompressed) {
						PriceSheet.TryReadAsBinary(decompressed);
					}
				}
			}

			foreach (KeyValue entry in PriceSheet["entries"].Children) {
				string? itemLink = entry["item_link"].AsString();
				if (itemLink == null) {
					continue;
				}

				try {
					PriceSheetItems[itemLink] = new StoreItem(itemLink);
				} catch {
					continue;
				}
			}
		}
	}
}
