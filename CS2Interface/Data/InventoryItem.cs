using System;
using System.Collections.Generic;
using System.Text;
using ArchiSteamFarm.Core;
using Newtonsoft.Json;
using SteamKit2.GC.CSGO.Internal;
using ValveKeyValue;

namespace CS2Interface {
	internal sealed class InventoryItem : Item {
		[JsonProperty(PropertyName = "iteminfo")]
		internal CSOEconItem ItemInfo;

		[JsonProperty(PropertyName = "attributes")]
		[JsonConverter (typeof(AttributeConverter))]
		Dictionary<string, IAttribute>? Attributes;

		[JsonProperty(PropertyName = "position")]
		internal uint? Position;

		[JsonProperty(PropertyName = "casket_id")]
		internal ulong? CasketID;

		[JsonProperty(PropertyName = "moveable")]
		internal bool? Moveable;

		public bool ShouldSerializeAttributes() => Attributes != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializePosition() => Position != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCasketID() => CasketID != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeMoveable() => Moveable != null && ShouldSerializeAdditionalProperties;

		internal InventoryItem(CSOEconItem item) {
			ItemInfo = item;

			SetAdditionalProperties();
		}

		new bool SetAdditionalProperties() {
			if (!ParseAttributes(ItemInfo.attribute)) {
				return false;
			}

			DefIndex = ItemInfo.def_index;
			Quality = ItemInfo.quality;
			Rarity = ItemInfo.rarity;
			Origin = ItemInfo.origin;
			PaintIndex = GetAttribute("set item texture prefab")?.ToUInt32() ?? 0;
			StickerID = GetAttribute("sticker slot 0 id")?.ToUInt32();
			TintID = GetAttribute("spray tint id")?.ToUInt32();
			MusicID = GetAttribute("music id")?.ToUInt32();
			if (GetAttribute("set item texture wear") != null) {
				Wear = (double) BitConverter.UInt32BitsToSingle(BitConverter.SingleToUInt32Bits(GetAttribute("set item texture wear")!.ToSingle()));
			}

			{
				bool is_new = ((ItemInfo.inventory >>> 30) & 1) == 1;
				Position = (is_new ? 0 : ItemInfo.inventory & 0xFFFF);
			}

			{
				uint? casket_low = GetAttribute("casket item id low")?.ToUInt32();
				uint? casket_high = GetAttribute("casket item id high")?.ToUInt32();
				if (casket_low != null && casket_high != null) {
					CasketID = (ulong) casket_high << 32 | casket_low;
				}
			}

			if (!base.SetAdditionalProperties()) {
				return false;
			}

			Moveable = CanBeMoved();

			return true;
		}

		public bool IsValid() {
			if (ItemInfo.id == 17293822569102708641 || ItemInfo.id == 17293822569110896676) {
				// https://dev.doctormckay.com/topic/4286-i-have-some-ghost-items-when-using-node-globaloffensive-to-retrieve-inventory/
				// Most inventories have these hidden items in them: "Genuine P250 | X-Ray", and "CS:GO Weapon Case"
				return false;
			}

			return true;
		}

		private bool CanBeMoved() {
			if (!IsValid()) {
				return false;
			}
			
			if (GetAttribute("cannot trade")?.ToUInt32() == 1
				|| ItemDef!["attributes"]?["cannot trade"]?.ToString() == "1"
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
			if (ItemInfo.flags == 10 && (ItemDef!["prefab"]?.ToString() == "valve weapon_case_key" || ItemDef!["prefab"]?.ToString() == "weapon_case_key")) {
				return false;
			}

			return true;
		}

		internal IAttribute? GetAttribute(string name) {
			if (Attributes == null) {
				return null;
			}

			if (Attributes.TryGetValue(name, out IAttribute? value)) {
				return value;
			}

			return null;
		}

		private bool ParseAttributes(List<CSOEconItemAttribute>? attributes) {
			Attributes = new Dictionary<string, IAttribute>();

			if (attributes == null || attributes.Count == 0) {
				return true;
			}

			foreach (CSOEconItemAttribute attribute in attributes) {
				KVObject? attribute_def = GameData.ItemsGame.GetDef("attributes", attribute.def_index.ToString());
				if (attribute_def == null) {
					return false;
				}

				string? attribute_name = attribute_def["name"].ToString();
				if (attribute_name == null) {
					ASF.ArchiLogger.LogGenericError(String.Format("Missing name for attribute: {0}", attribute.def_index.ToString()));

					return false;
				}

				if (attribute_def["attribute_type"]?.ToString() == "vector") {
					ASF.ArchiLogger.LogGenericError(String.Format("zzzz vector for: {0}", attribute_name));
				}

				switch (attribute_def["attribute_type"]?.ToString()) {
					case "uint32":
					case null when attribute_def["stored_as_integer"]?.ToString() == "1":
						Attributes.Add(attribute_name, new Attribute<uint>(attribute_name, BitConverter.ToUInt32(attribute.value_bytes)));
						break;

					case "float":
					case null when attribute_def["stored_as_integer"]?.ToString() == "0":
						Attributes.Add(attribute_name, new Attribute<float>(attribute_name, BitConverter.ToSingle(attribute.value_bytes)));
						break;

					case "string":
						Attributes.Add(attribute_name, new Attribute<string>(attribute_name, Encoding.UTF8.GetString(attribute.value_bytes, 2, attribute.value_bytes.Length - 2)));
						break;

					case "vector":
					default:
						ASF.ArchiLogger.LogGenericError(String.Format("Unknown attribute type: {0}, value: {1}", attribute_def["attribute_type"].ToString(), Convert.ToBase64String(attribute.value_bytes)));
						return false;
				}
			}

			return true;
		}
	}
}