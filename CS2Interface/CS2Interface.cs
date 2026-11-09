using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Plugins.Interfaces;
using SteamKit2;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Reflection;
using System.IO;

namespace CS2Interface {
	[Export(typeof(IPlugin))]
	public sealed class CS2Interface : IASF, IBotModules, IBotSteamClient, IBotCommand, IBotConnection, IBotCardsFarmerInfo, IGitHubPluginUpdates {
		internal static ConcurrentDictionary<string, bool> AutoStart = new();
		public string Name => nameof(CS2Interface);
		public string RepositoryName => "Citrinate/CS2Interface";
		public Version Version => typeof(CS2Interface).Assembly.GetName().Version ?? new Version("0");

		public Task OnLoaded(CancellationToken cancellationToken = default) {
			ASF.ArchiLogger.LogGenericInfo("Counter-Strike 2 Interface ASF Plugin by Citrinate");
			GameData.Update();
			
			return Task.CompletedTask;
		}

		public async Task<string?> OnBotCommand(Bot bot, EBotUserAccess access, string message, string[] args, ulong steamID = 0, CancellationToken cancellationToken = default) {			
			return await Commands.Response(bot, access, steamID, message, args).ConfigureAwait(false);
		}

		public Task OnASFInit(IReadOnlyDictionary<string, JsonElement>? additionalConfigProperties = null, CancellationToken cancellationToken = default) {
			if (additionalConfigProperties == null) {
				return Task.FromResult(0);
			}

			return Task.FromResult(0);
		}

		public Task OnBotInitModules(Bot bot, IReadOnlyDictionary<string, JsonElement>? additionalConfigProperties = null, CancellationToken cancellationToken = default) {
			if (additionalConfigProperties == null) {
				return Task.FromResult(0);
			}

			foreach (KeyValuePair<string, JsonElement> configProperty in additionalConfigProperties) {
				switch (configProperty.Key) {
					case "AutoStartCS2Interface" when (configProperty.Value.ValueKind == JsonValueKind.True || configProperty.Value.ValueKind == JsonValueKind.False): {
						bot.ArchiLogger.LogGenericInfo("AutoStartCS2Interface : " + configProperty.Value.GetBoolean());
						AutoStart[bot.BotName] = configProperty.Value.GetBoolean();
						break;
					}
				}
			}

			return Task.FromResult(0);
		}

		public Task OnBotSteamCallbacksInit(Bot bot, CallbackManager callbackManager, CancellationToken cancellationToken = default) {
			ClientHandler.AddHandler(bot, callbackManager);
			
			return Task.FromResult(0);
		}

		public Task<IReadOnlyCollection<ClientMsgHandler>?> OnBotSteamHandlersInit(Bot bot, CancellationToken cancellationToken = default) {
			return Task.FromResult<IReadOnlyCollection<ClientMsgHandler>?>([ new SteamCallback.SteamHandler() ]);
		}

		public async Task OnBotDisconnected(Bot bot, EResult reason, CancellationToken cancellationToken = default) {
			await ClientHandler.ClientHandlers[bot.BotName].Stop(preventAutoStart: false).ConfigureAwait(false);
		}

		public async Task OnBotLoggedOn(Bot bot, CancellationToken cancellationToken = default) {
			await TryAutoStart(bot).ConfigureAwait(false);
		}

		public async Task OnBotFarmingFinished(Bot bot, bool farmedSomething, CancellationToken cancellationToken = default) {
			await TryAutoStart(bot).ConfigureAwait(false);
		}

		public async Task OnBotFarmingStarted(Bot bot, CancellationToken cancellationToken = default) {
			await ClientHandler.ClientHandlers[bot.BotName].Stop(preventAutoStart: false).ConfigureAwait(false);
		}

		public async Task OnBotFarmingStopped(Bot bot, CancellationToken cancellationToken = default) {
			await TryAutoStart(bot).ConfigureAwait(false);
		}

		private async Task TryAutoStart(Bot bot) {
			if (!AutoStart.TryGetValue(bot.BotName, out bool autoStart)) {
				return;
			}

			if (!autoStart) {
				return;
			}

			(_, string message) = await ClientHandler.ClientHandlers[bot.BotName].Run().ConfigureAwait(false);
			bot.ArchiLogger.LogGenericInfo(message);
		}
	}
}
