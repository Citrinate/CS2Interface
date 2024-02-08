using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
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
				return (false, "Bot is not connected");
			}

			(_, _, EClientStatus status) = await VerifyConnection().ConfigureAwait(false);
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);
			bool ready = ((status & EClientStatus.Ready) == EClientStatus.Ready);

			if (connected) {
				return (true, "CS2 Interface is already running");
			}

			if (!connected && !ready) {
				return (false, "CS2 Interface is already attempting to run");
			}

			try {
				await Client.Run().ConfigureAwait(false);
				if (!await Client.VerifyConnection().ConfigureAwait(false)) {
					throw new ClientException(EClientExceptionType.Failed, "CS2 Interface seemed to start, but then didn't");
				}
			} catch (ClientException e) {
				Bot.ArchiLogger.LogGenericError(e.Message);
				if (numAttempts > 0 && e.Type != EClientExceptionType.FatalError) {
					await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
					Bot.ArchiLogger.LogGenericError("CS2 Interface failed to start, retrying");

					return await Run(numAttempts - 1).ConfigureAwait(false);
				}

				ForceStop();
				Bot.Actions.Resume();
				Bot.ArchiLogger.LogGenericError("CS2 Interface failed to start");

				return (false, String.Format("CS2 Interface failed to start: {0}", e.Message));
			}

			Bot.ArchiLogger.LogGenericInfo("CS2 Interface started");
			
			return (true, "CS2 Interface started");
		}

		internal string Stop() {
			if (!Bot.IsConnectedAndLoggedOn) {
				return "Bot is not connected";
			}

			EClientStatus status = Client.Status();
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);
			if (!connected) {
				return "CS2 Interface was not running";
			}

			Client.Stop();
			Bot.Actions.Resume();
			Bot.ArchiLogger.LogGenericInfo("CS2 Interface stopped");

			return "CS2 Interface successfully stopped";
		}

		internal void ForceStop() {
			EClientStatus status = Client.Status();
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);
			if (connected) {
				Client.Stop(); // Stop even if bot is logged out, to update the client state
				Bot.Actions.Resume();
				Bot.ArchiLogger.LogGenericInfo("CS2 Interface was forcibly stopped");
			}
		}

		internal (EClientStatus ClientStatus, string Message) Status() {
			if (!Bot.IsConnectedAndLoggedOn) {
				return (EClientStatus.None, "Bot is not connected");
			}

			EClientStatus status = Client.Status();
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);
			bool ready = ((status & EClientStatus.Ready) == EClientStatus.Ready);

			if (!connected) {
				if (!ready) {
					return (status, "CS2 Interface is connecting");
				}

				return (status, "CS2 Interface is not connected");
			}

			if (!ready) {
				return (status, "CS2 Interface is busy");
			}

			return (status, "Ready");
		}

		internal async Task<(bool Connected, string Message, EClientStatus ClientStatus)> VerifyConnection() {
			EClientStatus status = Client.Status();
			bool connected = ((status & EClientStatus.Connected) == EClientStatus.Connected);

			if (!connected) {
				return (false, "CS2 Interface is not connected", status);
			}

			connected = await Client.VerifyConnection().ConfigureAwait(false);
			if (!connected) {
				ForceStop();
				Bot.ArchiLogger.LogGenericError("CS2 Interface stopped unexpectedly");

				return (false, "CS2 Interface stopped unexpectedly", Client.Status());
			}

			return (true, "CS2 Interface is connected", Client.Status());
		}

		internal static (Bot?, Client?, string) GetAvailableClient(HashSet<Bot> bots) {
			foreach (Bot bot in bots) {
				(Client? client, string status) = ClientHandlers[bot.BotName].GetClient(EClientStatus.Connected | EClientStatus.Ready);
				if (client != null || bots.Count == 1) {
					return (bot, client, status);
				}
			}

			return (null, null, "No bots are available");
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