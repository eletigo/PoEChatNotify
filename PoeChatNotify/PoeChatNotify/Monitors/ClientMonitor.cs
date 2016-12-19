using System;
using System.IO;
using System.Text;
using System.Threading;
using eletigo.PoeChatNotify.Model;

namespace eletigo.PoeChatNotify.Monitors {
	public class ClientMonitor {
		public event EventHandler<ClientChangedEventArgs> Append;

		public int Interval { get; set; } = 250; // ms

		private FileStream _file;
		private Thread _thread;

		public bool IsScaning { get; private set; }
		public string ClientPath { get; set; }

		public void Scan(bool isBeginFromEnd = true) {
			if (IsScaning) return;
			if (string.IsNullOrEmpty(ClientPath))
				throw new FileNotFoundException("ClientPath is null or empty.");

			_file = new FileStream(ClientPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			if (isBeginFromEnd)
				_file.Seek(0, SeekOrigin.End);

			IsScaning = true;
			_thread = new Thread(_scanLoop) {IsBackground = true};
			_thread.Start();
		}

		private void _scanLoop() {
			var reader = new StreamReader(_file, Encoding.UTF8);
			while (IsScaning) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					var item = MessageItem.Parse(line);
					if (item != null)
						OnAppend(new ClientChangedEventArgs {
							ChangeType = ClientChangeType.Append,
							MessageItem = item
						});
				}
				Thread.Sleep(Interval);
			}
			reader.Dispose();
		}

		public void Stop() {
			if (!IsScaning) return;
			IsScaning = false;
			_thread?.Join();
			_file?.Dispose();
		}

		protected virtual void OnAppend(ClientChangedEventArgs e) {
			Append?.Invoke(this, e);
		}
	}
}
