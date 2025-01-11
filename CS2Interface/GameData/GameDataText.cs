using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	internal class GameDataText : GameDataResource {
		private List<KeyValue>? Data;

		internal GameDataText(string url) : base(url) {}

		internal override async Task<bool> Update() {
			KeyValue? data = await FetchKVResource().ConfigureAwait(false);
			if (data == null) {
				ASF.ArchiLogger.LogGenericError(String.Format("Couldn't load game data from: {0}", Url));

				return false;
			}

			Data = data.Children.Where(x => x.Name == "Tokens").SelectMany(x => x.Children).ToList();
			Updated = true;

			return true;
		}

		internal string? this[string? key] {
			get {
				if (key == null || Data == null) {
					return null;
				}

				return Data.Where(x => x.Name?.ToUpper().Trim() == key.ToUpper().Trim()).FirstOrDefault()?.Value;
			}
		}

		internal string? Format(string? key, params string?[] inserts) {
			string? value = this[key];
			if (value == null) {
				return null;
			}

			// Convert strings used in C++ printf into strings we can use for C# String.Format
			// Ex: "%s1 %s2 %s3" to "{0} {1} {2}"
			// Only care about the %s format specifier, which can only go up to %s9? https://github.com/nillerusr/source-engine/blob/29985681a18508e78dc79ad863952f830be237b6/tier1/ilocalize.cpp#L74

			try {
				StringBuilder sb = new(value);

				sb.Replace("%s1", "{0}");
				sb.Replace("%s2", "{1}");
				sb.Replace("%s3", "{2}");
				sb.Replace("%s4", "{3}");
				sb.Replace("%s5", "{4}");
				sb.Replace("%s6", "{5}");
				sb.Replace("%s7", "{6}");
				sb.Replace("%s8", "{7}");
				sb.Replace("%s9", "{8}");

				return String.Format(sb.ToString(), inserts);
			} catch {
				return null;
			}
		}

		internal string? GetWearName(double wear) {
			return wear switch {
				<= .07 => this["SFUI_InvTooltip_Wear_Amount_0"], // Factory New
				<= .15 => this["SFUI_InvTooltip_Wear_Amount_1"], // Minimal Wear
				<= .38 => this["SFUI_InvTooltip_Wear_Amount_2"], // Field-Tested
				<= .45 => this["SFUI_InvTooltip_Wear_Amount_3"], // Well-Worn
				_ => this["SFUI_InvTooltip_Wear_Amount_4"] // Battle-Scarred
			};
		}

		internal static string? GetOriginName(uint origin) {
			// https://raw.githubusercontent.com/SteamDatabase/SteamTracking/b5cba7a22ab899d6d423380cff21cec707b7c947/ItemSchema/CounterStrikeGlobalOffensive.json
			return origin switch {
				0 => Strings.ItemOrigin0, // Timed Drop
				1 => Strings.ItemOrigin1, // Achievement
				2 => Strings.ItemOrigin2, // Purchased
				3 => Strings.ItemOrigin3, // Traded
				4 => Strings.ItemOrigin4, // Crafted
				5 => Strings.ItemOrigin5, // Store Promotion
				6 => Strings.ItemOrigin6, // Gifted
				7 => Strings.ItemOrigin7, // Support Granted
				8 => Strings.ItemOrigin8, // Found in Crate
				9 => Strings.ItemOrigin9, // Earned
				10 => Strings.ItemOrigin10, // "Third-Party Promotion
				11 => Strings.ItemOrigin11, // "Wrapped Gift
				12 => Strings.ItemOrigin12, // "Halloween Drop
				13 => Strings.ItemOrigin13, // "Steam Purchase
				14 => Strings.ItemOrigin14, // "Foreign Item
				15 => Strings.ItemOrigin15, // "CD Key
				16 => Strings.ItemOrigin16, // "Collection Reward
				17 => Strings.ItemOrigin17, // "Preview Item
				18 => Strings.ItemOrigin18, // "Steam Workshop Contribution
				19 => Strings.ItemOrigin19, // "Periodic Score Reward
				20 => Strings.ItemOrigin20, // "Recycling
				21 => Strings.ItemOrigin21, // "Tournament Drop
				22 => Strings.ItemOrigin22, // "Stock Item
				23 => Strings.ItemOrigin23, // "Quest Reward
				24 => Strings.ItemOrigin24, // "Level Up Reward
				_ => null
			};
		}
	}
}