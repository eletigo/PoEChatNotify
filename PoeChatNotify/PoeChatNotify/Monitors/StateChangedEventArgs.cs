using System;

namespace eletigo.PoeChatNotify.Monitors {
	public class StateChangedEventArgs : EventArgs {
		public bool IsForeground { get; set; }
	}
}
