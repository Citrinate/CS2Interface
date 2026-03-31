using System;
using System.Linq;
using System.Text.Json.Serialization;
using SteamKit2.GC.CSGO.Internal;

namespace CS2Interface {
	public sealed class InspectItem : Item {
		[JsonInclude]
		[JsonPropertyName("iteminfo")]
		public CEconItemPreviewDataBlock ItemInfo { get; private init; }

		internal InspectItem(CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse item) {
			ItemInfo = item.iteminfo;

			BuildItem();
		}

		internal InspectItem(CEconItemPreviewDataBlock itemInfo) {
			ItemInfo = itemInfo;

			BuildItem();
		}

		private void BuildItem() {
			DefIndex = ItemInfo.defindex;
			PaintIndex = ItemInfo.paintindex;
			TintID = ItemInfo.stickers.FirstOrDefault()?.tint_id;
			KeychainID = ItemInfo.keychains.FirstOrDefault()?.sticker_id;
			HighlightReel = (ItemInfo.keychains.FirstOrDefault()?.ShouldSerializehighlight_reel() ?? false) ? ItemInfo.keychains.FirstOrDefault()?.highlight_reel : null;
			Quality = ItemInfo.quality;
			Rarity = ItemInfo.rarity;
			Origin = ItemInfo.origin;
			StatTrak = ItemInfo.quality == 9 || ItemInfo.ShouldSerializekilleaterscoretype();

			if (ItemInfo.paintwear != 0) {
				Wear = (double) BitConverter.UInt32BitsToSingle(ItemInfo.paintwear);
			}

			if (ItemInfo.stickers.Count > 0) {
				StickerIDs = ItemInfo.stickers.Select(sticker => sticker.sticker_id).ToHashSet();
			}

			SetDefs();
			SetAdditionalProperties();
		}
	}
}