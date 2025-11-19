using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	public sealed class StoreItem : Item {
		[JsonInclude]
		[JsonPropertyName("def_index")]
		public uint ID;

		[JsonInclude]
		[JsonPropertyName("name")]
		public string? Name;

		[JsonInclude]
		[JsonPropertyName("tournament_id")]
		public uint? TournamentID;

		[JsonInclude]
		[JsonPropertyName("requires_supplemental_data")]
		public bool RequiresSupplementalData = false;

		[JsonInclude]
		[JsonPropertyName("loot_list")]
		public List<Item> LootList = new List<Item>();

		public bool ShouldSerializeTournamentID() => TournamentID != null;
		public bool ShouldSerializeLootList() => LootList.Count > 0;

		internal StoreItem(string name) {
			KeyValue? itemDef = GameData.ItemsGame["items"].Children.Where(x => x["name"].AsString() == name).FirstOrDefault();
			if (itemDef == null) {
				ASF.ArchiLogger.LogGenericError(String.Format("{0}: items[].name = {1}", Strings.GameDataDefinitionUndefined, name));

				throw new Exception();
			}

			DefIndex = uint.Parse(itemDef.Name!);

			SetDefs();

			ID = DefIndex;

			if (ItemData == null) {
				return;
			}

			Name = GameData.CsgoEnglish[ItemData.ItemDef["item_name"].Value];

			// Get information related to InitializePurchase supplementalData parameter 
			{
				KeyValue? tournamentAttrValue = ItemData.ItemDef["attributes"]["tournament event id"]["value"];
				TournamentID = tournamentAttrValue != KeyValue.Invalid && tournamentAttrValue.Value != null ? uint.Parse(tournamentAttrValue.Value) : null;

				// Souvenir Packages use a Match ID as supplementalData
				if (TournamentID != null && ItemData.ItemDef["tool"]["type"].Value == "fantoken"
					&& name.Contains("_charge") // differentiate between an individual Souvenir Package and a pack of Souvenir Package tokens
				) {
					RequiresSupplementalData = true;
				}
			}

			// Many shop items have a loot list which points to the item(s) that will actually end up in your inventory, ex "coupon - dren_01" -> "[dren_01]musickit"
			// Get information about the item(s) in the loot list
			{
				string? lootListName = ItemData.ItemDef["loot_list_name"].Value;

				if (lootListName != null) {
					foreach (KeyValue lootListItem in GameData.ItemsGame["client_loot_lists"][lootListName].Children) {
						string? nameID = lootListItem.Name;

						if (nameID == "public_list_contents" || nameID == null) {
							continue;
						}

						Item? item = Item.GetItemFromNameID(nameID, ItemData.ItemDef["will_produce_stattrak"].AsBoolean());

						if (item == null) {
							ASF.ArchiLogger.LogGenericError(String.Format(Strings.FailedToResolveNameID, nameID));
							continue;
						}

						LootList.Add(item);
					}
				}
			}
		}
	}
}
