using System;
using System.Linq;
using System.Text.Json.Serialization;
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

		[JsonInclude]
		[JsonPropertyName("full_name")]
		internal string? FullName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("full_type_name")]
		internal string? FullTypeName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("rarity_name")]
		internal string? RarityName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("quality_name")]
		internal string? QualityName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("origin_name")]
		internal string? OriginName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("type_name")]
		internal string? TypeName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("item_name")]
		internal string? ItemName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("tool_name")]
		internal string? ToolName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("tint_name")]
		internal string? TintName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("weapon_image_url")]
		internal string? WeaponImageURL { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("weapon_name")]
		internal string? WeaponName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("wear_name")]
		internal string? WearName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("wear")]
		internal double? Wear { get; set; }
		
		[JsonInclude]
		[JsonPropertyName("wear_min")]
		internal float? WearMin { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("wear_max")]
		internal float? WearMax { get; private set; }

		[JsonInclude]
		[JsonPropertyName("name_id")]
		internal string? NameID { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("set_name_id")]
		internal string? SetNameID { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("set_name")]
		internal string? SetName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("crate_name_id")]
		internal string? CrateNameID { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("crate_defindex")]
		internal uint? CrateDefIndex { get; private set; }

		[JsonInclude]
		[JsonPropertyName("crate_supply_series")]
		internal uint? CrateSupplySeries { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("crate_name")]
		internal string? CrateName { get; private set; }

		[JsonInclude]
		[JsonPropertyName("defs")]
		internal ItemData? ItemData { get; private set; }

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