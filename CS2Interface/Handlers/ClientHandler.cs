using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
using CS2Interface.Localization;
using SteamKit2;

namespace CS2Interface {
	internal class ClientHandler {
		private readonly Bot Bot;
		private readonly Client Client;
		internal static ConcurrentDictionary<string, ClientHandler> ClientHandlers = new();
		internal readonly ScheduledAction AutoStop;
		private Task<(bool, string)>? RunTask;
		private CancellationTokenSource? RunCancellation;

		internal ClientHandler(Bot bot, CallbackManager callbackManager) {
			Bot = bot;
			Client = new Client(bot, callbackManager);
			AutoStop = new ScheduledAction(() => _ = Stop());
		}

		internal static void AddHandler(Bot bot, CallbackManager callbackManager) {
			if (ClientHandlers.ContainsKey(bot.BotName)) {
				ClientHandlers.TryRemove(bot.BotName, out ClientHandler? _);
			}

			ClientHandlers.TryAdd(bot.BotName, new ClientHandler(bot, callbackManager));
		}

		internal Task<(bool Success, string Message)> Run() {
			if (RunTask?.IsCompleted == false) {
				return RunTask;
			}

			RunCancellation = new CancellationTokenSource();
			RunTask = StartClient(RunCancellation.Token);

			return RunTask;
		}

		private async Task<(bool Success, string Message)> StartClient(CancellationToken cancellationToken) {
			try {
				if (!Bot.IsConnectedAndLoggedOn) {
					return (false, ArchiSteamFarm.Localization.Strings.BotNotConnected);
				}

				(_, _, EClientStatus status) = await VerifyClientConnection().ConfigureAwait(false);
				bool connected = (status & EClientStatus.Connected) == EClientStatus.Connected;

				if (connected) {
					return (true, Strings.InterfaceAlreadyRunning);
				}

				int maxAttempts = 3;
				for (int attempt = 1; attempt <= maxAttempts; attempt++) {
					if (!Bot.IsConnectedAndLoggedOn) {
						return (false, ArchiSteamFarm.Localization.Strings.BotNotConnected);
					}

					try {
						cancellationToken.ThrowIfCancellationRequested();
						await Client.Run().ConfigureAwait(false);

						cancellationToken.ThrowIfCancellationRequested();
						if (!await Client.VerifyConnection().ConfigureAwait(false)) {
							throw new ClientException(EClientExceptionType.Failed, Strings.InterfaceStartFailedUnexpectedly);
						}

						break;
					} catch (OperationCanceledException) {
						Bot.ArchiLogger.LogGenericInfo(Strings.InterfaceStartCancelled);
						Client.Stop();

						return (false, Strings.InterfaceStartCancelled);
					} catch (ClientException e) {
						Bot.ArchiLogger.LogGenericError(e.Message);

						if (e.Type == EClientExceptionType.FatalError || attempt == maxAttempts) {
							Bot.ArchiLogger.LogGenericError(Strings.InterfaceStartFailed);
							Client.Stop();

							return (false, String.Format("{0}: {1}", Strings.InterfaceStartFailed, e.Message));
						}

						await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);

						Bot.ArchiLogger.LogGenericError(Strings.InterfaceStartFailedRetry);
					}
				}

				CS2Interface.AutoStart[Bot.BotName] = true;
				Bot.ArchiLogger.LogGenericInfo(Strings.InterfaceStarted);

				return (true, Strings.InterfaceStarted);
			} finally {				
				RunCancellation?.Dispose();
				RunCancellation = null;
			}
		}

		internal async Task<(bool Connected, string Message, EClientStatus ClientStatus)> VerifyClientConnection() {
			EClientStatus status = Client.Status();
			bool connected = (status & EClientStatus.Connected) == EClientStatus.Connected;

			if (!connected) {
				return (false, Strings.InterfaceNotConnected, status);
			}

			connected = await Client.VerifyConnection().ConfigureAwait(false);
			if (!connected) {
				Client.Stop();
				Bot.ArchiLogger.LogGenericError(Strings.InterfaceStoppedUnexpectedly);

				return (false, Strings.InterfaceStoppedUnexpectedly, Client.Status());
			}

			return (true, Strings.InterfaceConnected, Client.Status());
		}

		internal async Task<string> Stop(bool preventAutoStart = true) {
			AutoStop.Cancel();

			RunCancellation?.Cancel();
			if (RunTask?.IsCompleted == false) {
				await RunTask.ConfigureAwait(false);
			}

			if (preventAutoStart) {
				CS2Interface.AutoStart[Bot.BotName] = false;
			}

			EClientStatus status = Client.Status();
			bool connected = (status & EClientStatus.Connected) == EClientStatus.Connected;

			Client.Stop();

			if (!connected) {
				return Strings.IntefaceNotRunning;
			} else {
				Bot.ArchiLogger.LogGenericInfo(Strings.InterfaceStopped);

				return Strings.InterfaceStopped;
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

			TimeSpan? autoStopTimeRemaining = AutoStop.GetTimeRemaining();
			if (autoStopTimeRemaining != null) {
				return (status, String.Format(Strings.InterfaceConnectedWithAutoStop, String.Format("{0:F2}", autoStopTimeRemaining.Value.TotalMinutes)));
			}

			return (status, Strings.InterfaceConnected);
		}

		internal static (Bot?, Client?, string) GetAvailableClient(HashSet<Bot> bots, EClientStatus desiredStatus = EClientStatus.Connected | EClientStatus.Ready) {
			List<Bot> shuffledBots = new List<Bot>(bots);
			shuffledBots.Shuffle();

			foreach (Bot bot in shuffledBots) {
				(Client? client, string status) = ClientHandlers[bot.BotName].GetClient(desiredStatus);
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

		internal void UpdateAutoStopTimer(uint minutes) {
			if (minutes == 0) {
				AutoStop.Cancel();
			} else {
				AutoStop.Schedule(TimeSpan.FromMinutes(minutes));
			}
		}

		internal void RefreshAutoStopTimer() {
			AutoStop.Refresh();
		}
	}
}