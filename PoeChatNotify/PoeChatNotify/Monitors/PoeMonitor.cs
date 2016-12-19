using System;
using System.Threading;
using eletigo.PoeChatNotify.Utility;

namespace eletigo.PoeChatNotify.Monitors {
	public class PoeMonitor {
		public event EventHandler<ProcessChangedEventArgs> ProcessChanged;
		public event EventHandler<StateChangedEventArgs> StateChanged;

		public int Interval { get; set; } = 1250; // ms

		private Thread _thread;

		public bool IsScaning { get; private set; }

		public void Scan() {
			if (IsScaning) return;

			IsScaning = true;
			_thread = new Thread(_scanLoop) { IsBackground = true };
			_thread.Start();
		}

		private void _scanLoop() {
			var last = User32.GetPoeProc();
			var isForeground = User32.IsPoeInForeground();
			while (IsScaning) {
				var proc = User32.GetPoeProc();
				if (proc != null) {
					var fg = User32.IsPoeInForeground();
					if (isForeground != fg) {
						OnStateChanged(new StateChangedEventArgs {
							IsForeground = fg
						});
					}
				}

				if (last == null && proc != null) {
					OnProcessChanged(new ProcessChangedEventArgs {
						ChangedType = ProcessChangedType.Opened,
						ProcessOpened = User32.GetPoeProc()
					});
				}
				else if (last != null && proc == null) {
					OnProcessChanged(new ProcessChangedEventArgs {
						ChangedType = ProcessChangedType.Closed,
					});
				}
				last = User32.GetPoeProc();
				isForeground = User32.IsPoeInForeground();

				Thread.Sleep(Interval);
			}
		}

		public void Stop() {
			if (!IsScaning) return;
			IsScaning = false;
			_thread?.Join();
		}

		protected virtual void OnProcessChanged(ProcessChangedEventArgs e) {
			ProcessChanged?.Invoke(this, e);
		}

		protected virtual void OnStateChanged(StateChangedEventArgs e) {
			StateChanged?.Invoke(this, e);
		}
	}
}
