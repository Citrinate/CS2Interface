using System;
using System.Threading;
using System.Threading.Tasks;

namespace CS2Interface {
	public sealed class ScheduledAction {
		private readonly Action Action;
		private TimeSpan Delay;
		private DateTime ScheduledTime;
		private CancellationTokenSource? ActionCancellation;
		private Task? DelayTask;
		private readonly object LockObject = new();

		public bool IsScheduled {
			get {
				lock (LockObject) {
					return DelayTask != null && DelayTask.IsCompleted == false;
				}
			}
		}

		public ScheduledAction(Action action) {
			Action = action;
		}

		public void Schedule(TimeSpan delay) {
			lock (LockObject) {
				Cancel();

				Delay = delay;
				ScheduledTime = DateTime.UtcNow + delay;
				ActionCancellation = new CancellationTokenSource();
				DelayTask = Run(delay, ActionCancellation.Token);
			}
		}

		public void Refresh() {
			lock (LockObject) {
				if (!IsScheduled) {
					return;
				}

				Schedule(Delay);
			}
		}

		public void Cancel() {
			lock (LockObject) {
				if (ActionCancellation == null) {
					return;
				}

				ActionCancellation.Cancel();
				ActionCancellation.Dispose();
				ActionCancellation = null;
				DelayTask = null;
			}
		}

		public TimeSpan? GetTimeRemaining() {
			lock (LockObject) {
				if (!IsScheduled) {
					return null;
				}

				TimeSpan remaining = ScheduledTime - DateTime.UtcNow;

				return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
			}
		}

		public DateTime? GetScheduledTime() {
			lock (LockObject) {
				if (!IsScheduled) {
					return null;
				}

				return ScheduledTime;
			}
		}

		private async Task Run(TimeSpan delay, CancellationToken cancellationToken) {
			try {
				await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
				cancellationToken.ThrowIfCancellationRequested();
				Action();
			} catch (OperationCanceledException) {
				;
			}
		}
	}
}
