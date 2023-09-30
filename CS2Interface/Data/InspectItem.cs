using System;
using System.Linq;
using Newtonsoft.Json;
using SteamKit2.GC.CSGO.Internal;

namespace CS2Interface {
	internal sealed class InspectItem : Item {
		[JsonProperty(PropertyName = "iteminfo")]
		internal CEconItemPreviewDataBlock ItemInfo;
		internal string s;
		internal string a;
		internal string d;
		internal string m;

		internal InspectItem(CMsgGCCStrike15_v2_Client2GCEconPreviewDataBlockResponse item, ulong param_s, ulong param_a, ulong param_d, ulong param_m) {
			ItemInfo = item.iteminfo;
			s = param_s.ToString();
			a = param_a.ToString();
			d = param_d.ToString();
			m = param_m.ToString();

			SetAdditionalProperties();
		}

		new bool SetAdditionalProperties() {
			DefIndex = ItemInfo.defindex;
			PaintIndex = ItemInfo.paintindex;            
			StickerID = ItemInfo.stickers.FirstOrDefault()?.sticker_id;
			TintID = ItemInfo.stickers.FirstOrDefault()?.tint_id;
			Quality = ItemInfo.quality;
			Rarity = ItemInfo.rarity;
			Origin = ItemInfo.origin;

			if (ItemInfo.paintwear != 0) {
				Wear = (double) BitConverter.UInt32BitsToSingle(ItemInfo.paintwear);
			}

			return base.SetAdditionalProperties();
		}
	}
}