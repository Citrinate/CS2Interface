using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using SteamKit2;

namespace CS2Interface {
	public class Item : GameObject {
		public uint DefIndex;
		public uint PaintIndex;
		public HashSet<uint> StickerIDs = new();
		public uint? TintID;
		public uint? MusicID;
		public uint? KeychainID;
		public uint Quality;
		public uint Rarity;
		public uint? Origin;

		[JsonInclude]
		[JsonPropertyName("full_name")]
		public string? FullName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("full_type_name")]
		public string? FullTypeName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("rarity_name")]
		public string? RarityName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("quality_name")]
		public string? QualityName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("origin_name")]
		public string? OriginName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("type_name")]
		public string? TypeName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("item_name")]
		public string? ItemName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("tool_name")]
		public string? ToolName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("tint_name")]
		public string? TintName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("weapon_image_url")]
		public string? WeaponImageURL { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("weapon_name")]
		public string? WeaponName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("wear_name")]
		public string? WearName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("wear")]
		public double? Wear { get; set; }
		
		[JsonInclude]
		[JsonPropertyName("wear_min")]
		public float? WearMin { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("wear_max")]
		public float? WearMax { get; private set; }

		[JsonInclude]
		[JsonPropertyName("stattrak")]
		public bool? StatTrak { get; protected set; }

		[JsonInclude]
		[JsonPropertyName("commodity")]
		public bool? Commodity { get; private set; }

		[JsonInclude]
		[JsonPropertyName("name_id")]
		public string? NameID { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("set_name_id")]
		public string? SetNameID { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("set_name")]
		public string? SetName { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("crate_name_id")]
		public string? CrateNameID { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("crate_defindex")]
		public uint? CrateDefIndex { get; private set; }
		
		[JsonInclude]
		[JsonPropertyName("crate_name")]
		public string? CrateName { get; private set; }

		[JsonInclude]
		[JsonPropertyName("stickers")]
		public Dictionary<uint, Item> Stickers { get; private set; } = new();

		[JsonInclude]
		[JsonPropertyName("keychains")]
		public Dictionary<uint, Item> Keychains { get; private set; } = new();

		[JsonInclude]
		[JsonPropertyName("defs")]
		public ItemData? ItemData { get; private set; }

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
		public bool ShouldSerializeStatTrak() => StatTrak != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCommodity() => Commodity != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeNameID() => NameID != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeSetNameID() => SetNameID != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeSetName() => SetName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCrateNameID() => CrateNameID != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCrateDefIndex() => CrateDefIndex != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeCrateName() => CrateName != null && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeStickers() => Stickers.Count > 0 && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeKeychains() => Keychains.Count > 0 && ShouldSerializeAdditionalProperties;
		public bool ShouldSerializeItemData() => ItemData != null && ShouldSerializeDefs;

		public const uint NumStickerSlots = 6;

		protected override bool SetDefs() {
			try {
				ItemData = new ItemData(this);
			} catch (Exception) {
				GameData.Update(true);

				return false;
			}

			return true;
		}

		protected override bool SetAdditionalProperties() {
			if (ItemData == null) {
				return false;
			}

			{ // Set rarity name, which differs based on the type of item
				string locKey = "loc_key"; // General rarities
				if (ItemData.ItemDef["taxonomy"]["weapon"].Value == "1" || ItemData.ItemDef["taxonomy"]["equipment"].Value == "1") {
					locKey = "loc_key_weapon"; // Weapon skin rarities
				} else if (ItemData.ItemDef["item_slot"].Value == "customplayer") {
					locKey = "loc_key_character"; // Agent rarities
				}
				RarityName = GameData.CsgoEnglish[GameData.ItemsGame["rarities"].Children.FirstOrDefault(x => x["value"].Value == Rarity.ToString())?[locKey].Value];
			}

			TypeName = GameData.CsgoEnglish[ItemData.ItemDef["item_type_name"].Value];
			QualityName = GameData.CsgoEnglish[GameData.ItemsGame["qualities"].Children.FirstOrDefault(x => x["value"].Value == Quality.ToString())?.Name];
			if (Quality != 9 && StatTrak == true) {
				// account for ★ StatTrak™ items
				QualityName = String.Format("{0} {1}", QualityName, GameData.CsgoEnglish["strange"]);
			}
			if (Origin != null) {
				OriginName = GameDataText.GetOriginName(Origin.Value);
			}

			// Set the item name, which will be something like: what kind of sticker it is, or the name of the weapon skin, or the type of pin/coin
			// If an item has a wear value, but uses the default paint_kit (vanilla knives for example), this will be "-"
			ItemName = GameData.CsgoEnglish[(ItemData.KeychainDef?["loc_name"].Value ?? ItemData.MusicDef?["loc_name"].Value ?? ItemData.StickerKitDef?["item_name"].Value ?? ItemData.PaintKitDef?["description_tag"].Value ?? ItemData.ItemDef["item_name"].Value)];

			// Set the tool named, used for various things like differentiating between Graffiti and Sealed Graffiti
			if (ItemData.ItemDef["prefab"].Value == "csgo_tool") {
				ToolName = GameData.CsgoEnglish[ItemData.ItemDef["item_name"].Value];
			}

			// Set the graffiti color
			if ((DefIndex == 1348 || DefIndex == 1349) && TintID != null) {
				TintName = GameData.CsgoEnglish[String.Format("Attrib_SprayTintValue_{0}", TintID)];
			}

			// Set various weapon-only attributes
			if (ItemData.PaintKitDef != null) {
				WeaponName = GameData.CsgoEnglish[ItemData.ItemDef["item_name"].Value];

				if (Wear != null) {
					WearName = GameData.CsgoEnglish.GetWearName(Wear.Value);
					string? wearRemapMinValue = ItemData.PaintKitDef["wear_remap_min"].Value;
					string? wearRemapMaxValue = ItemData.PaintKitDef["wear_remap_max"].Value;
					WearMin = wearRemapMinValue == null ? null : float.Parse(wearRemapMinValue, NumberStyles.Float, CultureInfo.InvariantCulture);
					WearMax = wearRemapMaxValue == null ? null : float.Parse(wearRemapMaxValue, NumberStyles.Float, CultureInfo.InvariantCulture);
				}

				// Set the weapon image url
				string? cdnNameID;
				if (PaintIndex == 0) {
					cdnNameID = ItemData.ItemDef["name"].Value; // Vanilla Knives
				} else {
					cdnNameID = String.Format("{0}_{1}", ItemData.ItemDef["name"].Value, ItemData.PaintKitDef["name"].Value); // Everything else
				}
				WeaponImageURL = GameData.ItemsGameCdn[cdnNameID];
			}

			{ // Set the full name and type
				string? displayQualityName = Quality == 4 ? "" : QualityName; // Hide "Unique" quality from item names and types

				FullTypeName = String.Format("{0} {1} {2}", displayQualityName, RarityName, TypeName).Trim();

				if (PaintIndex == 0 && ItemData.StickerKitDef == null && ItemData.MusicDef == null && ItemData.KeychainDef == null) {
					FullName = String.Format("{0} {1}", displayQualityName, ToolName ?? WeaponName ?? ItemName).Trim(); // Collectibles (Pins, Coins), Vanilla Knives
				} else if (WearName != null || (TintName != null && TintID != 0)) {
					FullName = String.Format("{0} {1} | {2} ({3})", displayQualityName, WeaponName ?? ToolName ?? TypeName, ItemName, WearName ?? TintName).Trim(); // Weapon Skins, Gloves, Graffiti
				} else if (ItemName != null) {
					FullName = String.Format("{0} {1} | {2}", displayQualityName, WeaponName ?? ToolName ?? TypeName, ItemName).Trim(); // Stickers
				} else {
					FullName = String.Format("{0} {1}", displayQualityName, WeaponName ?? ToolName ?? TypeName).Trim(); // Agents, Cases
				}
			}

			{ // Determine if item is a commodity on the marketplace or not
				string? itemTypeName = ItemData.ItemDef["item_type_name"].Value;
				if (itemTypeName != null) {
					HashSet<string> commodityTypes = new HashSet<string>{ 
						"#CSGO_Type_Collectible", // Collectible
						"#CSGO_Type_WeaponCase", // Container
						"#CSGO_Tool_GiftTag", // Gift
						"#CSGO_Type_Spray", // Graffiti
						"#CSGO_Tool_WeaponCase_KeyTag", // Key
						"#CSGO_Type_MusicKit", // Music Kit
						"#CSGO_Type_Ticket", // Pass
						"#CSGO_Tool_Patch", // Patch
						"#CSGO_Tool_Sticker", // Sticker
						"#CSGO_Tool_Name_TagTag", // Tag
						"#CSGO_Type_Tool" // Tool
					};

					Commodity = commodityTypes.Contains(itemTypeName);

					if (Commodity.Value) {
						if (ItemData.ItemDef["inv_container_and_tools"].Value == "souvenir_case") {
							// Souvenir Packages are not commodities
							Commodity = false;
						}
					}
				}
			}

			// Set the name id, used for determining related set and crate
			if (PaintIndex == 0 && ItemData.StickerKitDef == null && ItemData.MusicDef == null && ItemData.KeychainDef == null) {
				NameID = ItemData.ItemDef["name"].Value; // Collectibles, Vanilla Knives
			} else {
				NameID = String.Format("[{0}]{1}", (ItemData.KeychainDef ?? ItemData.MusicDef ?? ItemData.StickerKitDef ?? ItemData.PaintKitDef)?["name"].Value, ItemData.ItemDef["name"].Value); // Everything else
			}

			if (NameID != null) {
				{ // Determine what set, if any, this item belongs to
					KeyValue? setItemDef = GameData.ItemsGame["item_sets"].Children.FirstOrDefault(x => x["items"][NameID] != KeyValue.Invalid);
					if (setItemDef != null) {
						SetNameID = setItemDef.Name;
						SetName = GameData.CsgoEnglish[setItemDef["name"].Value];
					}
				}

				{ // Determine what crate or armory set, if any, this item belongs to.  Doesn't work for souvenir skins, knives, or gloves
					foreach (KeyValue lootlistDef in GameData.ItemsGame["client_loot_lists"].Children.Where(x => x[NameID] != KeyValue.Invalid)) {
						string? lootListName = lootlistDef.Name;
						// Some lists in client_loot_lists are nested (2 layers at most), we want the top-most layer
						lootListName = lootListName == null ? null : GameData.ItemsGame["client_loot_lists"].Children.FirstOrDefault(x => x[lootListName] != KeyValue.Invalid)?.Name ?? lootListName;

						// Crate
						if (CrateNameID == null) {
							string? lootListID = lootListName == null ? null : GameData.ItemsGame["revolving_loot_lists"].Children.FirstOrDefault(x => x.Value == lootListName)?.Name;
							KeyValue? crateItemDef = lootListID == null ? null : GameData.ItemsGame["items"].Children.FirstOrDefault(x => x["attributes"]["set supply crate series"]["value"].Value == lootListID);
							if (crateItemDef != null && crateItemDef.Name != null) {
								CrateNameID = crateItemDef["name"].Value;
								CrateDefIndex = uint.Parse(crateItemDef.Name);
								CrateName = GameData.CsgoEnglish[crateItemDef["item_name"].Value];
							}
						}

						// Armory Set
						if (SetNameID == null) {
							KeyValue? armorySetDef = GameData.ItemsGame["seasonaloperations"].Children.SelectMany(x => x.Children).Where(x => x.Name == "operational_point_redeemable").FirstOrDefault(x => x["item_name"].Value == "lootlist:" + lootListName);
							if (armorySetDef != null) {
								SetNameID = lootListName;
								SetName = GameData.CsgoEnglish[armorySetDef["callout"].Value];
							}
						}
					}
				}
			}

			// Also set details for any attached stickers, patches, or keychains
			if (StickerIDs.Count > 0 && !IsSticker()) {
				uint stickerDefIndex = 1209; // Sticker
				if (ItemData.ItemDef["stickers"].Value == "agent") {
					stickerDefIndex = 4609; // Patch
				}

				foreach (uint stickerID in StickerIDs) {
					Item sticker = new Item() {
						DefIndex = stickerDefIndex,
						PaintIndex = 0,
						StickerIDs = [stickerID],
						Quality = 4
					};

					sticker.SetDefs();
					sticker.Rarity = GameData.ItemsGame["rarities"][sticker.ItemData?.StickerKitDef?["item_rarity"].Value ?? "default"]["value"].AsUnsignedInteger();
					sticker.SetAdditionalProperties();

					Stickers[stickerID] = sticker;
				}
			}

			if (KeychainID != null && !IsKeychain()) {
				Item keychain = new Item() {
					DefIndex = 1355,
					PaintIndex = 0,
					KeychainID = KeychainID,
					Quality = 4
				};

				keychain.SetDefs();
				keychain.Rarity = GameData.ItemsGame["rarities"][keychain.ItemData?.KeychainDef?["item_rarity"].Value ?? "default"]["value"].AsUnsignedInteger();
				keychain.SetAdditionalProperties();

				Keychains[KeychainID.Value] = keychain;
			}

			return true;
		}

		public bool IsSticker() {
			return DefIndex == 1209 // Sticker
				|| DefIndex == 4609; // Patch
		}

		public bool IsGraffiti() {
			return DefIndex == 1348 // Sealed Graffiti
				|| DefIndex == 1349; // Graffiti
		}

		public bool IsKeychain() {
			return DefIndex == 1355; // Charm
		}
	}
}