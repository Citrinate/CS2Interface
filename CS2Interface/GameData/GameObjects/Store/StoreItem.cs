using System;
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

		public bool ShouldSerializeTournamentID() => TournamentID != null;

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

			{ // Specify the tournament id, to be used to find the supplimentalData needed to buy certain items
				KeyValue? tournamentAttrValue = ItemData.ItemDef["attributes"]["tournament event id"]["value"];
				TournamentID = tournamentAttrValue != KeyValue.Invalid && tournamentAttrValue.Value != null ? uint.Parse(tournamentAttrValue.Value) : null;
			}
		}
	}
}
