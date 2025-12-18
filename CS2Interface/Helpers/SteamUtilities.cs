using System;
using System.Globalization;

namespace CS2Interface {

	public static class SteamUtilities {
		public static int ToGCLanguageID(this CultureInfo cultureInfo) {
			ArgumentNullException.ThrowIfNull(cultureInfo);

			// https://github.com/JustArchiNET/ArchiSteamFarm/blob/8600a709c2d56d68ed0b7eca598a3d8bf34d803e/ArchiSteamFarm/Steam/Integration/SteamUtilities.cs#L21
			// https://github.com/SteamDatabase/GameTracking-Dota2/blob/master/DumpSource2/schemas/client/ELanguage.h
			return cultureInfo.TwoLetterISOLanguageName switch {
				"bg" => 23,
				"cs" => 19,
				"da" => 13,
				"de" => 1,
				"es" when cultureInfo.Name is "es-419" or "es-AR" or "es-BO" or "es-BR" or "es-BZ" or "es-CL" or "es-CO" or "es-CR" or "es-CU" or "es-DO" or "es-EC" or "es-GQ" or "es-GT" or "es-HN" or "es-MX" or "es-NI" or "es-PA" or "es-PE" or "es-PH" or "es-PR" or "es-PY" or "es-SV" or "es-US" or "es-UY" or "es-VE" => 26,
				"es" => 5,
				"el" => 24,
				"fi" => 15,
				"fr" => 2,
				"hu" => 18,
				"id" => 28,
				"it" => 3,
				"ja" => 10,
				"ko" => 4,
				"nl" => 14,
				"no" => 16,
				"pl" => 12,
				"pt" when cultureInfo.Name == "pt-BR" => 22,
				"pt" => 11,
				"ro" => 20,
				"ru" => 8,
				"sv" => 17,
				"th" => 9,
				"tr" => 21,
				"uk" => 25,
				"vi" => 27,
				"zh" when cultureInfo.Name is "zh-Hant" or "zh-HK" or "zh-MO" or "zh-TW" => 7,
				"zh" => 6,
				_ => 0 // English
			};
		}
	}
}
