using System;
using System.Linq;
using Newtonsoft.Json;
using SteamKit2;

namespace CS2Interface {
	internal class Item {
		internal uint DefIndex;
		internal uint PaintIndex;
		internal uint? StickerID;
		internal uint? TintID;
		internal uint? MusicID;
		internal uint Quality;
		internal uint Rarity;
		internal uint Origin;

		[JsonProperty(PropertyName = "full_name")]
		internal string? FullName;
		
		[JsonProperty(PropertyName = "full_type_name")]
		internal string? FullTypeName;
		
		[JsonProperty(PropertyName = "rarity_name")]
		internal string? RarityName;
		
		[JsonProperty(PropertyName = "quality_name")]
		internal string? QualityName;
		
		[JsonProperty(PropertyName = "origin_name")]
		internal string? OriginName;
		
		[JsonProperty(PropertyName = "type_name")]
		internal string? TypeName;
		
		[JsonProperty(PropertyName = "item_name")]
		internal string? ItemName;
		
		[JsonProperty(PropertyName = "tool_name")]
		internal string? ToolName;
		
		[JsonProperty(PropertyName = "tint_name")]
		internal string? TintName;
		
		[JsonProperty(PropertyName = "weapon_image_url")]
		internal string? WeaponImageURL;
		
		[JsonProperty(PropertyName = "weapon_name")]
		internal string? WeaponName;
		
		[JsonProperty(PropertyName = "wear_name")]
		internal string? WearName;
		
		[JsonProperty(PropertyName = "wear")]
		internal double? Wear;
		
		[JsonProperty(PropertyName = "wear_min")]
		internal float? WearMin;
		
		[JsonProperty(PropertyName = "wear_max")]
		internal float? WearMax;

		[JsonProperty(PropertyName = "name_id")]
		internal string? NameID;
		
		[JsonProperty(PropertyName = "set_name_id")]
		internal string? SetNameID;
		
		[JsonProperty(PropertyName = "set_name")]
		internal string? SetName;
		
		[JsonProperty(PropertyName = "crate_name_id")]
		internal string? CrateNameID;
		
		[JsonProperty(PropertyName = "crate_defindex")]
		internal uint? CrateDefIndex;

		[JsonProperty(PropertyName = "crate_supply_series")]
		internal uint? CrateSupplySeries;
		
		[JsonProperty(PropertyName = "crate_name")]
		internal string? CrateName;

		[JsonProperty(PropertyName = "defs")]
		internal ItemData? ItemData;

		protected static bool ShouldSerializeAdditionalProperties = true;
		protected static bool ShouldSerializeDefs = true;

		public bool ShouldSerializeFullName() => FullName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeFullTypeName() => FullTypeName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeRarityName() => RarityName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeQualityName() => QualityName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeOriginName() => OriginName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeTypeName() => TypeName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeItemName() => ItemName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeToolName() => ToolName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeTintName() => TintName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeWeaponImageURL() => WeaponImageURL != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeWeaponName() => WeaponName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeWearName() => WearName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeWear() => Wear != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeWearMin() => WearMin != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeWearMax() => WearMax != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeNameID() => NameID != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeSetNameID() => SetNameID != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeSetName() => SetName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCrateNameID() => CrateNameID != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCrateDefIndex() => CrateDefIndex != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCrateSupplySeries() => CrateSupplySeries != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCrateName() => CrateName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeItemData() => ItemData != null && ShouldSerializeDefs;

		internal static void SetSerializationProperties(bool should_serialize_additional_properties, bool should_serialize_defs) {
			ShouldSerializeAdditionalProperties = should_serialize_additional_properties;
			ShouldSerializeDefs = should_serialize_defs;
		}

		protected bool SetAdditionalProperties() {
			try {
				ItemData = new ItemData(this);
			} catch (Exception) {
				GameData.Update(true);

				return false;
			}

			{ // Set rarity name, which differs based on the type of item
				string locKey = "loc_key"; // General rarities
				if (ItemData.ItemDef.GetValue("taxonomy", "weapon") == "1") {
					locKey = "loc_key_weapon"; // Weapon skin rarities
				} else if (ItemData.ItemDef.GetValue("item_slot") == "customplayer") {
					locKey = "loc_key_character"; // Agent rarities
				}
				RarityName = GameData.CsgoEnglish[GameData.ItemsGame["rarities"]?.FirstOrDefault(x => x["value"].Value == Rarity.ToString())?[locKey].Value];
			}

			TypeName = GameData.CsgoEnglish[ItemData.ItemDef.GetValue("item_type_name")?.Substring(1)];
			QualityName = GameData.CsgoEnglish[GameData.ItemsGame["qualities"]?.FirstOrDefault(x => x["value"].Value == Quality.ToString())?.Name];
			OriginName = GameData.GetOriginName(Origin);

			// Set the item name, which will be something like: what kind of sticker it is, or the name of the weapon skin, or the type of pin/coin
			// If an item has a wear value, but uses the default paint_kit (vanilla knives for example), this will be "-"
			ItemName = GameData.CsgoEnglish[(ItemData.MusicDef?.GetValue("loc_name") ?? ItemData.StickerKitDef?.GetValue("item_name") ?? ItemData.PaintKitDef?.GetValue("description_tag") ?? ItemData.ItemDef.GetValue("item_name"))?.Substring(1)];

			// Set the tool named, used for various things like differentiating between Graffiti and Sealed Graffiti
			if (ItemData.ItemDef.GetValue("prefab") == "csgo_tool") {
				ToolName = GameData.CsgoEnglish[ItemData.ItemDef.GetValue("item_name")?.Substring(1)];
			}

			// Set the graffiti color, ignore if tint_id is 0 (Multicolor)
			if ((DefIndex == 1348 || DefIndex == 1349) && TintID != null && TintID != 0) {
				TintName = GameData.CsgoEnglish[String.Format("Attrib_SprayTintValue_{0}", TintID)];
			}

			// Set various weapon-only attributes
			if (ItemData.ItemDef.GetValue("taxonomy", "weapon") == "1") {
				WeaponName = GameData.CsgoEnglish[ItemData.ItemDef.GetValue("item_name")?.Substring(1)];

				if (Wear != null) {
					WearName = GameData.GetWearName(Wear.Value);
					WearMin = Convert.ToSingle(ItemData.PaintKitDef!.GetValue("wear_remap_min"));
					WearMax = Convert.ToSingle(ItemData.PaintKitDef!.GetValue("wear_remap_max"));
				}

				// Set the weapon image url
				string? cdnNameID;
				if (PaintIndex == 0) {
					cdnNameID = ItemData.ItemDef.GetValue("name"); // Vanilla Knives
				} else {
					cdnNameID = String.Format("{0}_{1}", ItemData.ItemDef.GetValue("name"), ItemData.PaintKitDef!.GetValue("name")); // Everything else
				}
				WeaponImageURL = GameData.ItemsGameCdn[cdnNameID];
			}

			{ // Set the full name and type
				string? displayQualityName = Quality == 4 ? "" : QualityName; // Hide "Unique" quality from item names and types

				FullTypeName = String.Format("{0} {1} {2}", displayQualityName, RarityName, TypeName).Trim();

				if (PaintIndex == 0 && ItemData.StickerKitDef == null && ItemData.MusicDef == null) {
					FullName = String.Format("{0} {1}", displayQualityName, ToolName ?? WeaponName ?? ItemName).Trim(); // Collectibles (Pins, Coins), Vanilla Knives
				} else if (WearName != null || TintName != null) {
					FullName = String.Format("{0} {1} | {2} ({3})", displayQualityName, WeaponName ?? ToolName ?? TypeName, ItemName, WearName ?? TintName).Trim(); // Weapon Skins, Gloves, Graffiti
				} else if (ItemName != null) {
					FullName = String.Format("{0} {1} | {2}", displayQualityName, WeaponName ?? ToolName ?? TypeName, ItemName).Trim(); // Stickers
				} else {
					FullName = String.Format("{0} {1}", displayQualityName, WeaponName ?? ToolName ?? TypeName).Trim(); // Agents, Cases
				}
			}

			// Set the name id, used for determining related set and crate
			if (PaintIndex == 0 && ItemData.StickerKitDef == null && ItemData.MusicDef == null) {
				NameID = ItemData.ItemDef.GetValue("name"); // Collectibles, Vanilla Knives
			} else {
				NameID = String.Format("[{0}]{1}", (ItemData.MusicDef ?? ItemData.StickerKitDef ?? ItemData.PaintKitDef)?.GetValue("name"), ItemData.ItemDef.GetValue("name")); // Everything else
			}

			if (NameID != null) {
				{ // Determine what set, if any, this item belongs to
					KeyValue? setItemDef = GameData.ItemsGame["item_sets"]?.FirstOrDefault(x => x["items"][NameID] != KeyValue.Invalid);
					if (setItemDef != null) {
						SetNameID = setItemDef.Name;
						SetName = GameData.CsgoEnglish[setItemDef["name"].Value?.Substring(1)];
					}
				}

				{ // Determine what crate, if any, this item belongs to.  Doesn't work for souvenir skins, knives, or gloves
					string? lootListName = GameData.ItemsGame["client_loot_lists"]?.FirstOrDefault(x => x[NameID] != KeyValue.Invalid)?.Name;
					lootListName = lootListName == null ? null : GameData.ItemsGame["client_loot_lists"]?.FirstOrDefault(x => x[lootListName] != KeyValue.Invalid)?.Name ?? lootListName; // Some lists in client_loot_lists are nested (1 or 2 layers), we want the top-most layer
					string? lootListID = lootListName == null ? null : GameData.ItemsGame["revolving_loot_lists"]?.FirstOrDefault(x => x.Value == lootListName)?.Name;
					KeyValue? crateItemDef = lootListID == null ? null : GameData.ItemsGame["items"]?.FirstOrDefault(x => x["attributes"]["set supply crate series"]["value"].Value == lootListID);
					if (crateItemDef != null && crateItemDef.Name != null) {
						CrateNameID = crateItemDef["name"].Value;
						CrateDefIndex = uint.Parse(crateItemDef.Name);
						CrateSupplySeries = uint.Parse(lootListID!);
						CrateName = GameData.CsgoEnglish[crateItemDef["item_name"].Value?.Substring(1)];
					}
				}
			}

			return true;
		}
	}
}