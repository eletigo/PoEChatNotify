using System;
using System.Diagnostics;

namespace eletigo.PoeChatNotify.Monitors {
	public enum ProcessChangedType {
		Opened,
		Closed
	}

	public class ProcessChangedEventArgs : EventArgs {
		public ProcessChangedType ChangedType { get; set; }
		public Process ProcessOpened { get; set; }
	}
}
