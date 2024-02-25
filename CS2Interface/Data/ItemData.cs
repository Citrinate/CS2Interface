using System;
using System.Text.Json.Serialization;
using ArchiSteamFarm.Core;
using SteamKit2;

namespace CS2Interface {
	internal class ItemData {
		[JsonInclude]
		[JsonPropertyName("item_def")]
		internal ItemDef ItemDef { get; private init; }

		[JsonInclude]
		[JsonPropertyName("paint_kit_def")]
		internal ItemDef? PaintKitDef { get; private init; }

		[JsonInclude]
		[JsonPropertyName("sticker_kit_def")]
		internal ItemDef? StickerKitDef { get; private init; }

		[JsonInclude]
		[JsonPropertyName("music_def")]
		internal ItemDef? MusicDef { get; private init; }

		public bool ShouldSerializeItemDef() => ItemDef != null;
		public bool ShouldSerializePaintKitDef() => PaintKitDef != null;
		public bool ShouldSerializeStickerKitDef() => StickerKitDef != null;
		public bool ShouldSerializeMusicDef() => MusicDef != null;

		internal ItemData(Item item) {
			ItemDef = CreateItemDef(item);
			PaintKitDef = CreatePaintKitDef(item);
			StickerKitDef = CreateStickerKitDef(item);
			MusicDef = CreateMusicDef(item);
		}

		private ItemDef CreateItemDef(Item item) {
			ItemDef itemDef = new(GameData.ItemsGame.GetDef("items", item.DefIndex.ToString()));

			// Add prefab values			
			if (!MergePrefab(itemDef, itemDef.GetValue("prefab"))) {
				throw new InvalidOperationException();
			}

			// Add default values
			itemDef.AddDef(GameData.ItemsGame.GetDef("items", "default"));

			return itemDef;
		}

		private bool MergePrefab(ItemDef itemDef, string? prefab) {
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
				itemDef.AddDef(prefabDef);
				if (!MergePrefab(itemDef, prefabDef["prefab"].Value)) {
					return false;
				};
			}

			if (!foundValid) {
				ASF.ArchiLogger.LogGenericError(String.Format("Couldn't find definition: prefabs[{0}]", prefab));
			}

			return foundValid;
		}

		private ItemDef? CreatePaintKitDef(Item item) {
			if (item.PaintIndex == 0 && item.Wear == null) {
				// This item has no paint kit
				return null;
			}

			ItemDef paintKitDef = new(GameData.ItemsGame.GetDef("paint_kits", item.PaintIndex.ToString()));

			// Add default values
			paintKitDef.AddDef(GameData.ItemsGame.GetDef("paint_kits", "0"));

			return paintKitDef;
		}

		private ItemDef? CreateStickerKitDef(Item item) {
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

			ItemDef stickerKitDef = new(GameData.ItemsGame.GetDef("sticker_kits", item.StickerID.ToString()!));

			// Add default values
			stickerKitDef.AddDef(GameData.ItemsGame.GetDef("sticker_kits", "0"));

			return stickerKitDef;
		}

		private ItemDef? CreateMusicDef(Item item) {
			if (item.MusicID == null) {
				// This item has no music definition
				return null;
			}

			ItemDef musicDef = new (GameData.ItemsGame.GetDef("music_definitions", item.MusicID.ToString()!));

			return musicDef;
		}
	}
}