using System;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
using System.ComponentModel;
using ArchiSteamFarm.Core;
using System.Collections.Generic;
using ArchiSteamFarm.Localization;
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
						case "CSTART" or "CSSTART" or "CS2START" or "CRUN" or "CSRUN" or "CS2RUN":
							return await ResponseRun(bot, access).ConfigureAwait(false);

						case "CSTOP" or "CSSTOP" or "CS2STOP":
							return ResponseStop(bot, access);

						case "CSA":
							return ResponseStatus(access, steamID, "ASF");
						case "CSTATUS" or "CSSTATUS" or "CS2STATUS":
							return ResponseStatus(bot, access);
						
						default:
							return null;
					};
				default:
					switch (args[0].ToUpperInvariant()) {
						case "CSTART" or "CSSTART" or "CS2START" or "CRUN" or "CSRUN" or "CS2RUN":
							return await ResponseRun(access, steamID, Utilities.GetArgsAsText(args, 1, ",")).ConfigureAwait(false);

						case "CSTOP" or "CSSTOP" or "CS2STOP":
							return ResponseStop(access, steamID, Utilities.GetArgsAsText(args, 1, ","));

						case "CSTATUS" or "CSSTATUS" or "CS2STATUS":
							return ResponseStatus(access, steamID, Utilities.GetArgsAsText(args, 1, ","));

						default:
							return null;
					}
			}
		}

		private static async Task<string?> ResponseRun(Bot bot, EAccess access) {
			if (access < EAccess.Master) {
				return null;
			}

			if (!bot.IsConnectedAndLoggedOn) {
				return FormatBotResponse(bot, Strings.BotNotConnected);
			}

			(_, string message) = await ClientHandler.ClientHandlers[bot.BotName].Run().ConfigureAwait(false);

			return FormatBotResponse(bot, message);
		}

		private static async Task<string?> ResponseRun(EAccess access, ulong steamID, string botNames) {
			if (String.IsNullOrEmpty(botNames)) {
				throw new ArgumentNullException(nameof(botNames));
			}

			HashSet<Bot>? bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0)) {
				return access >= EAccess.Owner ? FormatStaticResponse(String.Format(Strings.BotNotFound, botNames)) : null;
			}

			IList<string?> results = await Utilities.InParallel(bots.Select(bot => ResponseRun(bot, ArchiSteamFarm.Steam.Interaction.Commands.GetProxyAccess(bot, access, steamID)))).ConfigureAwait(false);

			List<string?> responses = new(results.Where(result => !String.IsNullOrEmpty(result)));

			return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
		}

		private static string? ResponseStop(Bot bot, EAccess access) {
			if (access < EAccess.Master) {
				return null;
			}

			if (!bot.IsConnectedAndLoggedOn) {
				return FormatBotResponse(bot, Strings.BotNotConnected);
			}

			string message = ClientHandler.ClientHandlers[bot.BotName].Stop();

			return FormatBotResponse(bot, message);
		}

		private static string? ResponseStop(EAccess access, ulong steamID, string botNames) {
			if (String.IsNullOrEmpty(botNames)) {
				throw new ArgumentNullException(nameof(botNames));
			}

			HashSet<Bot>? bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0)) {
				return access >= EAccess.Owner ? FormatStaticResponse(String.Format(Strings.BotNotFound, botNames)) : null;
			}

			IEnumerable<string?> results = bots.Select(bot => ResponseStop(bot, ArchiSteamFarm.Steam.Interaction.Commands.GetProxyAccess(bot, access, steamID)));

			List<string?> responses = new(results.Where(result => !String.IsNullOrEmpty(result)));

			return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
		}

		private static string? ResponseStatus(Bot bot, EAccess access) {
			if (access < EAccess.Master) {
				return null;
			}

			if (!bot.IsConnectedAndLoggedOn) {
				return FormatBotResponse(bot, Strings.BotNotConnected);
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
				return access >= EAccess.Owner ? FormatStaticResponse(String.Format(Strings.BotNotFound, botNames)) : null;
			}

			IEnumerable<string?> results = bots.Select(bot => ResponseStatus(bot, ArchiSteamFarm.Steam.Interaction.Commands.GetProxyAccess(bot, access, steamID)));

			List<string?> responses = new(results.Where(result => !String.IsNullOrEmpty(result)));

			return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
		}

		internal static string FormatStaticResponse(string response) => ArchiSteamFarm.Steam.Interaction.Commands.FormatStaticResponse(response);
		internal static string FormatBotResponse(Bot bot, string response) => bot.Commands.FormatBotResponse(response);
	}
}
