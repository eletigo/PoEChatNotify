using System;
using eletigo.PoeChatNotify.Model;

namespace eletigo.PoeChatNotify.Monitors {
	public enum ClientChangeType {
		Append
	}

	public class ClientChangedEventArgs : EventArgs {
		public ClientChangeType ChangeType { get; set; }
		public MessageItem MessageItem { get; set; }
	}
}
