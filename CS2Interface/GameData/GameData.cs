using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using CS2Interface.Localization;

namespace CS2Interface {
	internal static class GameData {
		internal static GameDataItems ItemsGame {get; private set;} = new("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt");
		internal static GameDataDictionary ItemsGameCdn {get; private set;} = new("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/5d339d0fce93ad9d96f6385410f21453304ad4e0/game/csgo/pak01_dir/scripts/items/items_game_cdn.txt");
		internal static GameDataText CsgoEnglish {get; private set;} = new("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/resource/csgo_english.txt");
		internal static GameDataDictionary GameVersion {get; private set;} = new("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/steam.inf");
		internal static uint? ClientVersion {get; private set;}

		private static bool IsUpdating = false;
		private static SemaphoreSlim UpdateSemaphore = new SemaphoreSlim(1, 1);
		private static Timer UpdateTimer = new(async e => await DoUpdate().ConfigureAwait(false), null, Timeout.Infinite, Timeout.Infinite);
		private static DateTime? DoNotUpdateUntil = null;

		internal static void Update(bool forceUpdate = false) {
			if (forceUpdate && (DoNotUpdateUntil == null || DateTime.Now > DoNotUpdateUntil)) {
				ItemsGame.Updated = false;
				ItemsGameCdn.Updated = false;
				CsgoEnglish.Updated = false;
				GameVersion.Updated = false;
				DoNotUpdateUntil = DateTime.Now.AddMinutes(15);
				ASF.ArchiLogger.LogGenericInfo(Strings.GameDataRefreshing);
			}
			
			if (!ItemsGame.Updated || !ItemsGameCdn.Updated || !CsgoEnglish.Updated || !GameVersion.Updated) {
				IsUpdating = true;
			}

			if (!IsUpdating) {
				return;
			}

			UpdateTimer.Change(TimeSpan.FromTicks(0), TimeSpan.FromSeconds(10));
		}

		internal static async Task<bool> IsLoaded(uint maxWaitTimeSeconds = 60, bool update = true) {
			if (update) {
				Update();
			}

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
				if (!GameVersion.Updated) {
					tasks.Add(GameVersion.Update());
				}
				await Task.WhenAll(tasks).ConfigureAwait(false);

				if (ItemsGame.Updated && ItemsGameCdn.Updated && CsgoEnglish.Updated && GameVersion.Updated) {
					UpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
					IsUpdating = false;
					ASF.ArchiLogger.LogGenericInfo(Strings.GameDataLoadingSuccess);

					string? clientVersionString = GameVersion["ClientVersion"];
					if (clientVersionString != null) {
						if (uint.TryParse(clientVersionString, out uint clientVersion)) {
							ClientVersion = clientVersion;
						} else {
							ASF.ArchiLogger.LogGenericInfo(Strings.ClientVersionFail);
						}
					} else {
						ASF.ArchiLogger.LogGenericInfo(Strings.ClientVersionFail);
					}
				}
			} catch (Exception e) {
				ASF.ArchiLogger.LogGenericException(e);
			} finally {
				UpdateSemaphore.Release();
			}
		}
	}
}
