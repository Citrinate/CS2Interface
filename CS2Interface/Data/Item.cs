using System;
using System.Linq;
using ArchiSteamFarm.Core;
using Newtonsoft.Json;
using ValveKeyValue;

namespace CS2Interface {
	internal class Item {
		protected uint DefIndex;
		protected uint PaintIndex;
		protected uint? StickerID;
		protected uint? TintID;
		protected uint? MusicID;
		protected uint Quality;
		protected uint Rarity;
		protected uint Origin;

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
		
		[JsonProperty(PropertyName = "item_def")]
		[JsonConverter(typeof(KVConverter))]
		internal KVObject? ItemDef = null;
		
		[JsonProperty(PropertyName = "paint_kit_def")]
		[JsonConverter(typeof(KVConverter))]
		internal KVObject? PaintKitDef = null;
		
		[JsonProperty(PropertyName = "sticker_kit_def")]
		[JsonConverter(typeof(KVConverter))]
		internal KVObject? StickerKitDef = null;
		
		[JsonProperty(PropertyName = "music_def")]
		[JsonConverter(typeof(KVConverter))]
		internal KVObject? MusicDef = null;

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
		public bool ShouldSerializeItemDef() => ItemDef != null && ShouldSerializeDefs;
		public bool ShouldSerializePaintKitDef() => PaintKitDef != null && ShouldSerializeDefs;
		public bool ShouldSerializeStickerKitDef() => StickerKitDef != null && ShouldSerializeDefs;
		public bool ShouldSerializeMusicDef() => MusicDef != null && ShouldSerializeDefs;

		internal static void SetSerializationProperties(bool should_serialize_additional_properties, bool should_serialize_defs) {
			ShouldSerializeAdditionalProperties = should_serialize_additional_properties;
			ShouldSerializeDefs = should_serialize_defs;
		}

		private bool SetItemDef() {
			ItemDef = GameData.ItemsGame.GetDef("items", DefIndex.ToString());
			if (ItemDef == null) {
				return false;
			}

			// Merge with prefab values
			if (!MergePrefab(ItemDef["prefab"].ToString())) {
				return false;
			}

			// Merge with default values
			KVObject? defaultItemDef = GameData.ItemsGame.GetDef("items", "default");
			if (defaultItemDef == null) {
				return false;
			}

			ItemDef.Merge(defaultItemDef);

			return true;
		}

		private bool MergePrefab(string? prefab) {
			if (ItemDef == null) {
				return false;
			}

			if (prefab == null) {
				return true;
			}

			// Some items have multiple prefabs separated by a space, but only one is valid (it has an entry in ItemsGame)
			// Ex: "valve weapon_case_key": "valve" isn't valid, but "weapon_case_key" is
			// Ex: "antwerp2022_sticker_capsule_prefab antwerp2022_sellable_item_with_payment_rules": "antwerp2022_sticker_capsule_prefab" is valid, but "antwerp2022_sellable_item_with_payment_rules" isn't
			string[] prefabNames = prefab.Split(" ");
			// Consider the merge successfull if at least one valid prefab was found
			bool foundValid = false;

			foreach (string prefabName in prefabNames) {
				KVObject? prefabDef = GameData.ItemsGame.GetDef("prefabs", prefabName, suppressErrorLogs: true);
				if (prefabDef == null) {
					continue;
				}

				foundValid = true;
				ItemDef.Merge(prefabDef);
				if (!MergePrefab(prefabDef["prefab"]?.ToString())) {
					return false;
				};
			}

			if (!foundValid) {
				ASF.ArchiLogger.LogGenericError(String.Format("Couldn't find definition: prefabs[{0}]", prefab));
			}

			return foundValid;
		}

		private bool SetPaintKitDef() {
			if (PaintIndex == 0 && Wear == null) {
				// This item has no paint kit
				return true;
			}

			PaintKitDef = GameData.ItemsGame.GetDef("paint_kits", PaintIndex.ToString());
			if (PaintKitDef == null) {
				return false;
			}

			// Merge with default values
			KVObject? defaultPaintKitDef = GameData.ItemsGame.GetDef("paint_kits", "0");
			if (defaultPaintKitDef == null) {
				return false;
			}

			PaintKitDef.Merge(defaultPaintKitDef);

			return true;
		}

		private bool SetStickerKitDef() {
			if (StickerID == null 
				|| !(
					DefIndex == 1209 // Sticker
					|| DefIndex == 1348 // Sealed Graffiti
					|| DefIndex == 1349 // Graffiti
					|| DefIndex == 4609 // Patch
				)
			) {
				// This item has no sticker kit
				return true;
			}

			StickerKitDef = GameData.ItemsGame.GetDef("sticker_kits", StickerID.ToString()!);
			if (StickerKitDef == null) {
				return false;
			}

			// Merge with default values
			KVObject? defaultStickerKitDef = GameData.ItemsGame.GetDef("sticker_kits", "0");
			if (defaultStickerKitDef == null) {
				return false;
			}

			StickerKitDef.Merge(defaultStickerKitDef);

			return true;
		}

		private bool SetMusicDef() {
			if (MusicID == null) {
				// This item has no music definition
				return true;
			}

			MusicDef = GameData.ItemsGame.GetDef("music_definitions", MusicID.ToString()!);
			if (MusicDef == null) {
				return false;
			}

			return true;
		}

		protected bool SetAdditionalProperties() {
			if (!(SetItemDef() && SetPaintKitDef() && SetStickerKitDef() && SetMusicDef())) {
				GameData.Update();
				
				return false;
			}

			{ // Set rarity name, which differs based on the type of item
				string locKey = "loc_key"; // General rarities
				if (ItemDef!["taxonomy"]?["weapon"]?.ToString() == "1") {
					locKey = "loc_key_weapon"; // Weapon skin rarities
				} else if (ItemDef!["item_slot"]?.ToString() == "customplayer") {
					locKey = "loc_key_character"; // Agent rarities
				}
				RarityName = GameData.CsgoEnglish[GameData.ItemsGame["rarities"]?.FirstOrDefault(x => (uint) x["value"] == Rarity)?[locKey].ToString()];
			}

			TypeName = GameData.CsgoEnglish[ItemDef!["item_type_name"]?.ToString()?.Substring(1)];
			QualityName = GameData.CsgoEnglish[GameData.ItemsGame["qualities"]?.FirstOrDefault(x => (uint) x["value"] == Quality)?.Name];
			OriginName = GameData.GetOriginName(Origin);

			// Set the item name, which will be something like: what kind of sticker it is, or the name of the weapon skin, or the type of pin/coin
			// If an item has a wear value, but uses the default paint_kit (vanilla knives for example), this will be "-"
			ItemName = GameData.CsgoEnglish[(MusicDef?["loc_name"] ?? StickerKitDef?["item_name"] ?? PaintKitDef?["description_tag"] ?? ItemDef!["item_name"])?.ToString()?.Substring(1)];

			// Set the tool named, used for various things like differentiating between Graffiti and Sealed Graffiti
			if (ItemDef!["prefab"]?.ToString() == "csgo_tool") {
				ToolName = GameData.CsgoEnglish[ItemDef!["item_name"]?.ToString()?.Substring(1)];
			}

			// Set the graffiti color, ignore if tint_id is 0 (Multicolor)
			if ((DefIndex == 1348 || DefIndex == 1349) && TintID != null && TintID != 0) {
				TintName = GameData.CsgoEnglish[String.Format("Attrib_SprayTintValue_{0}", TintID)];
			}

			// Set various weapon-only attributes
			if (ItemDef!["taxonomy"]?["weapon"]?.ToString() == "1") {
				WeaponName = GameData.CsgoEnglish[ItemDef!["item_name"]?.ToString()?.Substring(1)];

				if (Wear != null) {
					WearName = GameData.GetWearName(Wear.Value);
					WearMin = Convert.ToSingle(PaintKitDef!["wear_remap_min"]);
					WearMax = Convert.ToSingle(PaintKitDef!["wear_remap_max"]);
				}

				// Set the weapon image url
				string? cdnNameID;
				if (PaintIndex == 0) {
					cdnNameID = ItemDef!["name"].ToString(); // Vanilla Knives
				} else {
					cdnNameID = String.Format("{0}_{1}", ItemDef!["name"].ToString(), PaintKitDef!["name"].ToString()); // Everything else
				}
				WeaponImageURL = GameData.ItemsGameCdn[cdnNameID];
			}

			{ // Set the full name and type
				string? displayQualityName = Quality == 4 ? "" : QualityName; // Hide "Unique" quality from item names and types

				FullTypeName = String.Format("{0} {1} {2}", displayQualityName, RarityName, TypeName).Trim();

				if (PaintIndex == 0 && StickerKitDef == null && MusicDef == null) {
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
			if (PaintIndex == 0 && StickerKitDef == null && MusicDef == null) {
				NameID = ItemDef!["name"].ToString(); // Collectibles, Vanilla Knives
			} else {
				NameID = String.Format("[{0}]{1}", (MusicDef ?? StickerKitDef ?? PaintKitDef)?["name"].ToString(), ItemDef!["name"].ToString()); // Everything else
			}

			{ // Determine what set, if any, this item belongs to
				KVObject? setItemDef = GameData.ItemsGame["item_sets"]?.FirstOrDefault(x => x["items"][NameID] != null);
				if (setItemDef != null) {
					SetNameID = setItemDef.Name;
					SetName = GameData.CsgoEnglish[setItemDef["name"]?.ToString()?.Substring(1)];
				}
			}

			{ // Determine what crate, if any, this item belongs to.  Doesn't work for souvenir skins, knives, or gloves
				string? lootListName = GameData.ItemsGame["client_loot_lists"]?.FirstOrDefault(x => x[NameID] != null)?.Name;
				lootListName = lootListName == null ? null : GameData.ItemsGame["client_loot_lists"]?.FirstOrDefault(x => x[lootListName] != null)?.Name ?? lootListName; // Some lists in client_loot_lists are nested (1 or 2 layers), we want the top-most layer
				string? lootListID = lootListName == null ? null : GameData.ItemsGame["revolving_loot_lists"]?.FirstOrDefault(x => x.Value.ToString() == lootListName)?.Name;
				KVObject? crateItemDef = lootListID == null ? null : GameData.ItemsGame["items"]?.FirstOrDefault(x => x["attributes"]?["set supply crate series"]?["value"]?.ToString() == lootListID);
				if (crateItemDef != null) {
					CrateNameID = crateItemDef["name"]?.ToString();
					CrateDefIndex = uint.Parse(crateItemDef.Name);
					CrateSupplySeries = uint.Parse(lootListID!);
					CrateName = GameData.CsgoEnglish[crateItemDef["item_name"]?.ToString()?.Substring(1)];
				}
			}

			return true;
		}
	}
}