using System.Collections.Generic;
using System.Linq;
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
			Items = inventory.Values.Where(item => ItemIDs.Contains(item.ItemInfo.id)).ToList();
		}
	}
}
