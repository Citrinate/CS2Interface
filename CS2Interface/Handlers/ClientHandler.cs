using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	internal class ClientHandler {
		private readonly Bot Bot;
		private readonly Client Client;
		internal static ConcurrentDictionary<string, ClientHandler> ClientHandlers = new();

		internal ClientHandler(Bot bot, CallbackManager callbackManager) {
			Bot = bot;
			Client = new Client(bot, callbackManager);
		}

		internal static void AddHandler(Bot bot, CallbackManager callbackManager) {
			if (ClientHandlers.ContainsKey(bot.BotName)) {
				ClientHandlers.TryRemove(bot.BotName, out ClientHandler? _);
			}

			ClientHandlers.TryAdd(bot.BotName, new ClientHandler(bot, callbackManager));
		}

		internal async Task<(bool Success, string Message)> Run(int numAttempts = 3) {
			if (!Bot.IsConnectedAndLoggedOn) {
				return (false, ArchiSteamFarm.Localization.Strings.BotNotConnected);
			}

			(_, _, EClientStatus status) = await VerifyConnection().ConfigureAwait(false);
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);
			bool ready = ((status & EClientStatus.Ready) == EClientStatus.Ready);

			if (connected) {
				return (true, Strings.InterfaceAlreadyRunning);
			}

			if (!connected && !ready) {
				return (false, Strings.InterfaceAlreadyStarting);
			}

			try {
				await Client.Run().ConfigureAwait(false);
				if (!await Client.VerifyConnection().ConfigureAwait(false)) {
					throw new ClientException(EClientExceptionType.Failed, Strings.InterfaceStartFailedUnexpectedly);
				}
			} catch (ClientException e) {
				Bot.ArchiLogger.LogGenericError(e.Message);
				if (numAttempts > 0 && e.Type != EClientExceptionType.FatalError) {
					await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
					Bot.ArchiLogger.LogGenericError(Strings.InterfaceStartFailedRetry);

					return await Run(numAttempts - 1).ConfigureAwait(false);
				}

				ForceStop();
				Bot.Actions.Resume();
				Bot.ArchiLogger.LogGenericError(Strings.InterfaceStartFailed);

				return (false, String.Format("{0}: {1}", Strings.InterfaceStartFailed, e.Message));
			}

			CS2Interface.AutoStart[Bot.BotName] = true;
			Bot.ArchiLogger.LogGenericInfo(Strings.InterfaceStarted);
			
			return (true, Strings.InterfaceStarted);
		}

		internal string Stop() {
			// The user has decided to stop the interface

			if (!Bot.IsConnectedAndLoggedOn) {
				return ArchiSteamFarm.Localization.Strings.BotNotConnected;
			}

			EClientStatus status = Client.Status();
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);
			if (!connected) {
				return Strings.IntefaceNotRunning;
			}

			Client.Stop();
			CS2Interface.AutoStart[Bot.BotName] = false;
			Bot.Actions.Resume();
			Bot.ArchiLogger.LogGenericInfo(Strings.InterfaceStopped);

			return Strings.InterfaceStoppedSuccessfully;
		}

		internal void ForceStop() {
			// The interface has decided to stop itself

			EClientStatus status = Client.Status();
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);
			if (connected) {
				Client.Stop(); // Stop even if bot is logged out, to update the client state
				Bot.Actions.Resume();
				Bot.ArchiLogger.LogGenericInfo(Strings.InterfaceForciblyStopped);
			}
		}

		internal (EClientStatus ClientStatus, string Message) Status() {
			if (!Bot.IsConnectedAndLoggedOn) {
				return (EClientStatus.BotOffline, ArchiSteamFarm.Localization.Strings.BotNotConnected);
			}

			EClientStatus status = Client.Status();
			bool connected = (status & EClientStatus.Connected) == EClientStatus.Connected;
			bool ready = (status & EClientStatus.Ready) == EClientStatus.Ready;

			if (!connected) {
				if (!ready) {
					return (status, Strings.InterfaceConnecting);
				}

				return (status, Strings.InterfaceNotConnected);
			}

			if (!ready) {
				return (status, Strings.InterfaceBusy);
			}

			return (status, Strings.Ready);
		}

		internal async Task<(bool Connected, string Message, EClientStatus ClientStatus)> VerifyConnection() {
			EClientStatus status = Client.Status();
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);

			if (!connected) {
				return (false, Strings.InterfaceNotConnected, status);
			}

			connected = await Client.VerifyConnection().ConfigureAwait(false);
			if (!connected) {
				ForceStop();
				Bot.ArchiLogger.LogGenericError(Strings.InterfaceStoppedUnexpectedly);

				return (false, Strings.InterfaceStoppedUnexpectedly, Client.Status());
			}

			return (true, Strings.InterfaceConnected, Client.Status());
		}

		internal static (Bot?, Client?, string) GetAvailableClient(HashSet<Bot> bots) {
			foreach (Bot bot in bots) {
				(Client? client, string status) = ClientHandlers[bot.BotName].GetClient(EClientStatus.Connected | EClientStatus.Ready);
				if (client != null || bots.Count == 1) {
					return (bot, client, status);
				}
			}

			return (null, null, Strings.NoBotsAvailable);
		}

		internal (Client?, string) GetClient(EClientStatus desiredStatus = EClientStatus.Connected | EClientStatus.Ready) {
			(EClientStatus status, string message) = Status();
			if ((status & desiredStatus) != desiredStatus) {
				return (null, message);
			}

			return (Client, message);
		}
	}
}