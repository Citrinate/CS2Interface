﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Plugins.Interfaces;
using SteamKit2;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Reflection;

namespace CS2Interface {
	[Export(typeof(IPlugin))]
	public sealed class CS2Interface : IASF, IBotModules, IBotSteamClient, IBotCommand2, IBotConnection, IBotCardsFarmerInfo {
		internal static ConcurrentDictionary<string, bool> AutoStart = new();
		public string Name => nameof(CS2Interface);
		public Version Version => typeof(CS2Interface).Assembly.GetName().Version ?? new Version("0");
		private bool ASFEnhanceEnabled = false;

		public Task OnLoaded() {
			ASF.ArchiLogger.LogGenericInfo("Counter-Strike 2 Interface ASF Plugin by Citrinate");
			GameData.Update();

			// ASFEnhanced Adapter https://github.com/chr233/ASFEnhanceAdapterDemoPlugin
			var flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			var handler = typeof(Commands).GetMethod(nameof(Commands.Response), flag);
			const string pluginId = nameof(CS2Interface);
			const string cmdPrefix = "CS2INTERFACE";
			const string repoName = "Citrinate/CS2Interface";
			var registered = AdapterBridge.InitAdapter(Name, pluginId, cmdPrefix, repoName, handler);
			ASFEnhanceEnabled = registered;

			return Task.CompletedTask;
		}

		public async Task<string?> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamID = 0) {
			if (ASFEnhanceEnabled) {
				return null;
			}
			
			return await Commands.Response(bot, access, steamID, message, args).ConfigureAwait(false);
		}

		public Task OnASFInit(IReadOnlyDictionary<string, JsonElement>? additionalConfigProperties = null) {
			if (additionalConfigProperties == null) {
				return Task.FromResult(0);
			}

			return Task.FromResult(0);
		}

		public Task OnBotInitModules(Bot bot, IReadOnlyDictionary<string, JsonElement>? additionalConfigProperties = null) {
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

		public Task OnBotSteamCallbacksInit(Bot bot, CallbackManager callbackManager) {
			ClientHandler.AddHandler(bot, callbackManager);
			
			return Task.FromResult(0);
		}

		public Task<IReadOnlyCollection<ClientMsgHandler>?> OnBotSteamHandlersInit(Bot bot) {
			return Task.FromResult<IReadOnlyCollection<ClientMsgHandler>?>(new HashSet<ClientMsgHandler> { });
		}

		public Task OnBotDisconnected(Bot bot, EResult reason) {
			ClientHandler.ClientHandlers[bot.BotName].ForceStop();

			return Task.FromResult(0);
		}

		private async Task TryAutoStart(Bot bot) {
			if (!AutoStart.TryGetValue(bot.BotName, out bool autoStart)) {
				return;
			}

			if (!autoStart) {
				return;
			}

			await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
			(_, string message) = await ClientHandler.ClientHandlers[bot.BotName].Run().ConfigureAwait(false);
			bot.ArchiLogger.LogGenericInfo(message);
		}

		public async Task OnBotLoggedOn(Bot bot) {
			await TryAutoStart(bot).ConfigureAwait(false);
		}

		public async Task OnBotFarmingFinished(Bot bot, bool farmedSomething) {
			if (farmedSomething) {
				await TryAutoStart(bot).ConfigureAwait(false);
			}
		}

		public Task OnBotFarmingStarted(Bot bot) {
			ClientHandler.ClientHandlers[bot.BotName].ForceStop();

			return Task.FromResult(0);
		}

		public Task OnBotFarmingStopped(Bot bot) {
			ClientHandler.ClientHandlers[bot.BotName].ForceStop();

			return Task.FromResult(0);
		}
	}
}
