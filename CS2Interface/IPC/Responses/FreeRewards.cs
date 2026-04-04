using System.Collections.Generic;
using System.Text.Json.Serialization;
using SteamKit2.GC.CSGO.Internal;

namespace CS2Interface.IPC {
	public sealed class FreeRewards {
		[JsonInclude]
		[JsonPropertyName("generation_time")]
		public uint GenerationTime;

		[JsonInclude]
		[JsonPropertyName("redeemable_balance")]
		public uint RedeemableBalance;

		[JsonInclude]
		[JsonPropertyName("itemids")]
		public List<ulong> ItemIDs;

		[JsonInclude]
		[JsonPropertyName("items")]
		public List<InventoryItem> Items;

		public FreeRewards(IReadOnlyDictionary<ulong, InventoryItem> inventory, CSOAccountItemPersonalStore personalStore) {
			GenerationTime = personalStore.generation_time;
			RedeemableBalance = personalStore.redeemable_balance;
			ItemIDs = personalStore.items;
			Items = new List<InventoryItem>();

			foreach (ulong itemID in ItemIDs) {
				if (itemID >> 60 == 0xF) {
					// Already redeemed this free reward, skip it
					// var def_index = itemID & UInt16.MaxValue;
					// var texture_prefab = (itemID >> 16) & UInt16.MaxValue;
					continue;
				}

				if (inventory.TryGetValue(itemID, out InventoryItem? item)) {
					Items.Add(item);
				}
			}
		}
	}
}
