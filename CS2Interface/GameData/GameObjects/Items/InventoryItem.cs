using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SteamKit2.GC.CSGO.Internal;
using Swashbuckle.AspNetCore.Annotations;

namespace CS2Interface {
	public sealed class InventoryItem : Item {
		[JsonInclude]
		[JsonPropertyName("iteminfo")]
		public CSOEconItem ItemInfo { get; private init; }

		[JsonInclude]
		[JsonPropertyName("attributes")]
		[JsonConverter (typeof(AttributeConverter))]
		public Dictionary<string, IAttribute>? Attributes { get; private set; }

		[JsonInclude]
		[JsonPropertyName("position")]
		public uint? Position { get; private set; }

		[JsonInclude]
		[JsonPropertyName("casket_id")]
		public ulong? CasketID { get; private set; }

		[JsonInclude]
		[JsonPropertyName("moveable")]
		[SwaggerParameter(Description = "The S value from the item's inspect link (not needed if using the url parameter)")] 
		public bool? Moveable { get; private set; }

		public bool ShouldSerializeAttributes() => Attributes != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializePosition() => Position != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCasketID() => CasketID != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeMoveable() => Moveable != null && ShouldSerializeAdditionalProperties;

		internal InventoryItem(CSOEconItem item) {
			ItemInfo = item;
			Attributes = AttributeParser.Parse(ItemInfo.attribute);

			if (Attributes != null) {
				DefIndex = ItemInfo.def_index;
				Quality = ItemInfo.quality;
				Rarity = ItemInfo.rarity;
				Origin = ItemInfo.origin;
				PaintIndex = Attributes.GetValueOrDefault("set item texture prefab")?.ToUInt32() ?? 0;
				StickerID = Attributes.GetValueOrDefault("sticker slot 0 id")?.ToUInt32();
				TintID = Attributes.GetValueOrDefault("spray tint id")?.ToUInt32();
				MusicID = Attributes.GetValueOrDefault("music id")?.ToUInt32();
				if (Attributes.GetValueOrDefault("set item texture wear") != null) {
					Wear = (double) BitConverter.UInt32BitsToSingle(BitConverter.SingleToUInt32Bits(Attributes.GetValueOrDefault("set item texture wear")!.ToSingle()));
				}

				SetDefs();

				{
					bool is_new = ((ItemInfo.inventory >>> 30) & 1) == 1;
					Position = is_new ? 0 : ItemInfo.inventory & 0xFFFF;
				}

				{
					uint? casket_low = Attributes.GetValueOrDefault("casket item id low")?.ToUInt32();
					uint? casket_high = Attributes.GetValueOrDefault("casket item id high")?.ToUInt32();
					if (casket_low != null && casket_high != null) {
						CasketID = (ulong) casket_high << 32 | casket_low;
					}
				}

				if (SetAdditionalProperties()) {
					Moveable = IsMovable();
				}				
			}
		}

		public bool IsVisible() {
			if (ItemInfo.id == 17293822569102708641 || ItemInfo.id == 17293822569110896676) {
				// https://dev.doctormckay.com/topic/4286-i-have-some-ghost-items-when-using-node-globaloffensive-to-retrieve-inventory/
				// Most inventories have these hidden items in them: "Genuine P250 | X-Ray", and "CS:GO Weapon Case"
				return false;
			}

			return true;
		}

		private bool? IsMovable() {
			if (Attributes == null) {
				return null;
			}

			if (!IsVisible()) {
				return false;
			}
			
			if (Attributes.GetValueOrDefault("cannot trade")?.ToUInt32() == 1
				|| ItemData!.ItemDef["attributes"]["cannot trade"].Value == "1"
			) {
				return false;
			}

			// Modified stock items that appear in inventory, untested, might not be necessary
			if (ItemInfo.rarity == 0) {
				return false;
			}

			// Apparently certain case keys can't be put in storage units? untested, might not be necessary
			// https://github.com/nombersDev/casemove/blob/8289ea35cb6d76c553ee4955adecdf9a02622764/src/main/helpers/classes/steam/items/index.js#L506
			// https://dev.doctormckay.com/topic/4086-inventory-and-music-kits/#comment-10610
			if (ItemInfo.flags == 10 && (ItemData.ItemDef["prefab"].Value == "valve weapon_case_key" || ItemData!.ItemDef["prefab"].Value == "weapon_case_key")) {
				return false;
			}

			return true;
		}
	}
}