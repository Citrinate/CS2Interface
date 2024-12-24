using System;
using System.Text.Json.Serialization;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	public class ItemData {
		[JsonInclude]
		[JsonPropertyName("item_def")]
		[JsonConverter(typeof(KVConverter))]
		public KeyValue ItemDef { get; private init; }

		[JsonInclude]
		[JsonPropertyName("paint_kit_def")]
		[JsonConverter(typeof(KVConverter))]
		public KeyValue? PaintKitDef { get; private init; }

		[JsonInclude]
		[JsonPropertyName("sticker_kit_def")]
		[JsonConverter(typeof(KVConverter))]
		public KeyValue? StickerKitDef { get; private init; }

		[JsonInclude]
		[JsonPropertyName("music_def")]
		[JsonConverter(typeof(KVConverter))]
		public KeyValue? MusicDef { get; private init; }

		[JsonInclude]
		[JsonPropertyName("keychain_def")]
		[JsonConverter(typeof(KVConverter))]
		public KeyValue? KeychainDef { get; private init; }

		public bool ShouldSerializeItemDef() => ItemDef != null;
		public bool ShouldSerializePaintKitDef() => PaintKitDef != null;
		public bool ShouldSerializeStickerKitDef() => StickerKitDef != null;
		public bool ShouldSerializeMusicDef() => MusicDef != null;
		public bool ShouldSerializeKeychainDefDef() => KeychainDef != null;

		internal ItemData(Item item) {
			ItemDef = CreateItemDef(item);
			PaintKitDef = CreatePaintKitDef(item);
			StickerKitDef = CreateStickerKitDef(item);
			MusicDef = CreateMusicDef(item);
			KeychainDef = CreateKeychainDef(item);
		}

		private KeyValue CreateItemDef(Item item) {
			KeyValue? itemDef = GameData.ItemsGame.GetDef("items", item.DefIndex.ToString());
			if (itemDef == null) {
				throw new Exception();
			}

			KeyValue def = itemDef.Clone();

			// Add prefab values
			if (!MergePrefab(def, def["prefab"].Value)) {
				throw new Exception();
			}

			// Add default values
			KeyValue? defaultItemDef = GameData.ItemsGame.GetDef("items", "default");
			if (defaultItemDef == null) {
				throw new Exception();
			}
			
			def.Merge(defaultItemDef);

			return def;
		}

		private bool MergePrefab(KeyValue itemDef, string? prefab) {
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
				KeyValue? prefabDef = GameData.ItemsGame.GetDef("prefabs", prefabName, suppressErrorLogs: true);
				if (prefabDef == null) {
					continue;
				}

				foundValid = true;
				itemDef.Merge(prefabDef);
				if (!MergePrefab(itemDef, prefabDef["prefab"].Value)) {
					return false;
				};
			}

			if (!foundValid) {
				ASF.ArchiLogger.LogGenericError(String.Format("{0}: prefabs[{1}]", Strings.GameDataDefinitionUndefined, prefab));
			}

			return foundValid;
		}

		private KeyValue? CreatePaintKitDef(Item item) {
			if (item.PaintIndex == 0 && item.Wear == null) {
				// This item has no paint kit
				return null;
			}

			KeyValue? paintKitDef = GameData.ItemsGame.GetDef("paint_kits", item.PaintIndex.ToString());
			if (paintKitDef == null) {
				throw new Exception();
			}

			// Add default values
			KeyValue? defaultPaintKitDef = GameData.ItemsGame.GetDef("paint_kits", "0");
			if (defaultPaintKitDef == null) {
				throw new Exception();
			}

			KeyValue def = paintKitDef.Clone();
			def.Merge(defaultPaintKitDef);

			return def;
		}

		private KeyValue? CreateStickerKitDef(Item item) {
			if (item.StickerID == null 
				|| !(
					item.DefIndex == 1209 // Sticker
					|| item.DefIndex == 1348 // Sealed Graffiti
					|| item.DefIndex == 1349 // Graffiti
					|| item.DefIndex == 4609 // Patch
				)
			) {
				// This item has no sticker kit
				return null;
			}

			KeyValue? stickerKitDef = GameData.ItemsGame.GetDef("sticker_kits", item.StickerID.ToString()!);
			if (stickerKitDef == null) {
				throw new Exception();
			}

			// Add default values
			KeyValue? defaultStickerKitDef = GameData.ItemsGame.GetDef("sticker_kits", "0");
			if (defaultStickerKitDef == null) {
				throw new Exception();
			}
			
			KeyValue def = stickerKitDef.Clone();
			def.Merge(defaultStickerKitDef);

			return def;
		}

		private KeyValue? CreateMusicDef(Item item) {
			if (item.MusicID == null) {
				// This item has no music definition
				return null;
			}

			KeyValue? musicDef = GameData.ItemsGame.GetDef("music_definitions", item.MusicID.ToString()!);
			if (musicDef == null) {
				throw new Exception();
			}

			return musicDef.Clone();
		}


		private KeyValue? CreateKeychainDef(Item item) {
			if (item.KeychainID == null 
				|| !(
					item.DefIndex == 1355 // Charm
				)
			) {
				// This item has no keychain definition
				return null;
			}

			KeyValue? keychainDef = GameData.ItemsGame.GetDef("keychain_definitions", item.KeychainID.ToString()!);
			if (keychainDef == null) {
				throw new Exception();
			}

			return keychainDef.Clone();
		}
	}
}