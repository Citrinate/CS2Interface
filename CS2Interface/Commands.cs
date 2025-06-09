using System;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
using System.ComponentModel;
using ArchiSteamFarm.Core;
using System.Collections.Generic;
using System.Linq;

namespace CS2Interface {
	internal static class Commands {
		internal static async Task<string?> Response(Bot bot, EAccess access, ulong steamID, string message, string[] args) {
			if (!Enum.IsDefined(access)) {
				throw new InvalidEnumArgumentException(nameof(access), (int) access, typeof(EAccess));
			}

			if (string.IsNullOrEmpty(message)) {
				return null;
			}

			switch (args.Length) {
				case 1:
					switch (args[0].ToUpperInvariant()) {
						case "CS2INTERFACE" when access >= EAccess.Master:
							return String.Format("{0} {1}", nameof(CS2Interface), (typeof(CS2Interface).Assembly.GetName().Version ?? new Version("0")).ToString());

						case "CSTART" or "CSSTART" or "CS2START" or "CRUN" or "CSRUN" or "CS2RUN":
							return await ResponseRun(bot, access).ConfigureAwait(false);

						case "CSTOP" or "CSSTOP" or "CS2STOP":
							return await ResponseStop(bot, access).ConfigureAwait(false);

						case "CSA":
							return ResponseStatus(access, steamID, "ASF");
						case "CSTATUS" or "CSSTATUS" or "CS2STATUS":
							return ResponseStatus(bot, access);
						
						default:
							return null;
					};
				default:
					switch (args[0].ToUpperInvariant()) {
						case "CSTART" or "CSSTART" or "CS2START" or "CRUN" or "CSRUN" or "CS2RUN" when args.Length > 2:
							return await ResponseRun(access, steamID, args[1], args[2]).ConfigureAwait(false);
						case "CSTART" or "CSSTART" or "CS2START" or "CRUN" or "CSRUN" or "CS2RUN":
							return await ResponseRun(access, steamID, args[1]).ConfigureAwait(false);

						case "CSTOP" or "CSSTOP" or "CS2STOP":
							return await ResponseStop(access, steamID, Utilities.GetArgsAsText(args, 1, ",")).ConfigureAwait(false);

						case "CSTATUS" or "CSSTATUS" or "CS2STATUS":
							return ResponseStatus(access, steamID, Utilities.GetArgsAsText(args, 1, ","));

						default:
							return null;
					}
			}
		}

		private static async Task<string?> ResponseRun(Bot bot, EAccess access, string? autoStopAfterMinutesText = null) {
			if (access < EAccess.Master) {
				return null;
			}

			if (!bot.IsConnectedAndLoggedOn) {
				return FormatBotResponse(bot, ArchiSteamFarm.Localization.Strings.BotNotConnected);
			}

			uint autoStopAfterMinutes = 0;
			if (autoStopAfterMinutesText != null) {
				if (uint.TryParse(autoStopAfterMinutesText, out uint outValue)) {
					autoStopAfterMinutes = outValue;
				} else {
					return FormatBotResponse(bot, String.Format(ArchiSteamFarm.Localization.Strings.ErrorIsInvalid, nameof(autoStopAfterMinutesText)));
				}
			}

			(bool success, string message) = await ClientHandler.ClientHandlers[bot.BotName].Run().ConfigureAwait(false);

			if (success) {
				ClientHandler.ClientHandlers[bot.BotName].UpdateAutoStopTimer(autoStopAfterMinutes);
			}

			return FormatBotResponse(bot, message);
		}

		private static async Task<string?> ResponseRun(EAccess access, ulong steamID, string botNames, string? autoStopAfterMinutesText = null) {
			if (String.IsNullOrEmpty(botNames)) {
				throw new ArgumentNullException(nameof(botNames));
			}

			HashSet<Bot>? bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0)) {
				return access >= EAccess.Owner ? FormatStaticResponse(String.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)) : null;
			}

			IList<string?> results = await Utilities.InParallel(bots.Select(bot => ResponseRun(bot, ArchiSteamFarm.Steam.Interaction.Commands.GetProxyAccess(bot, access, steamID), autoStopAfterMinutesText))).ConfigureAwait(false);

			List<string?> responses = new(results.Where(result => !String.IsNullOrEmpty(result)));

			return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
		}

		private static async Task<string?> ResponseStop(Bot bot, EAccess access) {
			if (access < EAccess.Master) {
				return null;
			}

			if (!bot.IsConnectedAndLoggedOn) {
				return FormatBotResponse(bot, ArchiSteamFarm.Localization.Strings.BotNotConnected);
			}

			string message = await ClientHandler.ClientHandlers[bot.BotName].Stop().ConfigureAwait(false);

			return FormatBotResponse(bot, message);
		}

		private static async Task<string?> ResponseStop(EAccess access, ulong steamID, string botNames) {
			if (String.IsNullOrEmpty(botNames)) {
				throw new ArgumentNullException(nameof(botNames));
			}

			HashSet<Bot>? bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0)) {
				return access >= EAccess.Owner ? FormatStaticResponse(String.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)) : null;
			}

			IList<string?> results = await Utilities.InParallel(bots.Select(bot => ResponseStop(bot, ArchiSteamFarm.Steam.Interaction.Commands.GetProxyAccess(bot, access, steamID)))).ConfigureAwait(false);

			List<string?> responses = new(results.Where(result => !String.IsNullOrEmpty(result)));

			return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
		}

		private static string? ResponseStatus(Bot bot, EAccess access) {
			if (access < EAccess.Master) {
				return null;
			}

			if (!bot.IsConnectedAndLoggedOn) {
				return FormatBotResponse(bot, ArchiSteamFarm.Localization.Strings.BotNotConnected);
			}

			(_, string message) = ClientHandler.ClientHandlers[bot.BotName].Status();

			return FormatBotResponse(bot, message);
		}

		private static string? ResponseStatus(EAccess access, ulong steamID, string botNames) {
			if (String.IsNullOrEmpty(botNames)) {
				throw new ArgumentNullException(nameof(botNames));
			}

			HashSet<Bot>? bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0)) {
				return access >= EAccess.Owner ? FormatStaticResponse(String.Format(ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)) : null;
			}

			IEnumerable<string?> results = bots.Select(bot => ResponseStatus(bot, ArchiSteamFarm.Steam.Interaction.Commands.GetProxyAccess(bot, access, steamID)));

			List<string?> responses = new(results.Where(result => !String.IsNullOrEmpty(result)));

			return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
		}

		internal static string FormatStaticResponse(string response) => ArchiSteamFarm.Steam.Interaction.Commands.FormatStaticResponse(response);
		internal static string FormatBotResponse(Bot bot, string response) => bot.Commands.FormatBotResponse(response);
	}
}
