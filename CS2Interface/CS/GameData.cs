using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;

namespace CS2Interface {
	internal static class GameData {
		internal static GameDataItems ItemsGame = new("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt");
		internal static GameDataItemsCDN ItemsGameCdn = new("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game_cdn.txt");
		internal static GameDataText CsgoEnglish = new("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/resource/csgo_english.txt");

		private static bool IsUpdating = false;
		private static SemaphoreSlim UpdateSemaphore = new SemaphoreSlim(1, 1);
		private static Timer UpdateTimer = new(async e => await DoUpdate().ConfigureAwait(false), null, Timeout.Infinite, Timeout.Infinite);
		private static DateTime? DoNotUpdateUntil = null;

		internal static void Update(bool forceUpdate = false) {
			if (forceUpdate && (DoNotUpdateUntil == null || DateTime.Now > DoNotUpdateUntil)) {
				ItemsGame.Updated = false;
				ItemsGameCdn.Updated = false;
				CsgoEnglish.Updated = false;
				DoNotUpdateUntil = DateTime.Now.AddMinutes(15);
				ASF.ArchiLogger.LogGenericInfo(Strings.GameDataRefreshing);
			}
			
			if (!ItemsGame.Updated || !ItemsGameCdn.Updated || !CsgoEnglish.Updated) {
				IsUpdating = true;
			}

			if (!IsUpdating) {
				return;
			}

			UpdateTimer.Change(TimeSpan.FromTicks(0), TimeSpan.FromSeconds(10));
		}

		internal static async Task<bool> IsLoaded(uint maxWaitTimeSeconds = 60) {
			Update();

			DateTime timeoutTime = DateTime.Now.AddSeconds(maxWaitTimeSeconds);
			while (IsUpdating) {
				await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

				if (maxWaitTimeSeconds != 0 && DateTime.Now > timeoutTime) {
					return false;
				}
			}

			return true;
		}

		private static async Task DoUpdate() {
			await UpdateSemaphore.WaitAsync().ConfigureAwait(false);
			try {
				if (!IsUpdating) {
					return;
				}

				List<Task<bool>> tasks = new();
				if (!ItemsGame.Updated) {
					tasks.Add(ItemsGame.Update());
				}
				if (!ItemsGameCdn.Updated) {
					tasks.Add(ItemsGameCdn.Update());
				}
				if (!CsgoEnglish.Updated) {
					tasks.Add(CsgoEnglish.Update());
				}
				await Task.WhenAll(tasks).ConfigureAwait(false);

				if (ItemsGame.Updated && ItemsGameCdn.Updated && CsgoEnglish.Updated) {
					UpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
					IsUpdating = false;
					ASF.ArchiLogger.LogGenericInfo(Strings.GameDataLoadingSuccess);
				}
			} catch (Exception e) {
				ASF.ArchiLogger.LogGenericException(e);
			} finally {
				UpdateSemaphore.Release();
			}
		}

		internal static string? GetWearName(double wear) {
			return wear switch {
				<= .07 => CsgoEnglish["SFUI_InvTooltip_Wear_Amount_0"], // Factory New
				<= .15 => CsgoEnglish["SFUI_InvTooltip_Wear_Amount_1"], // Minimal Wear
				<= .38 => CsgoEnglish["SFUI_InvTooltip_Wear_Amount_2"], // Field-Tested
				<= .45 => CsgoEnglish["SFUI_InvTooltip_Wear_Amount_3"], // Well-Worn
				_ => CsgoEnglish["SFUI_InvTooltip_Wear_Amount_4"] // Battle-Scarred
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