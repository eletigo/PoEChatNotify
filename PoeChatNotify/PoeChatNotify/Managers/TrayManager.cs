using System.Drawing;
using Hardcodet.Wpf.TaskbarNotification;

namespace eletigo.PoeChatNotify.Managers {
	public enum TrayIconState {
		Normal,
		Message,
		NotificationOff,
		Disabled
	}

	public class TrayManager {
		public TaskbarIcon TrayIcon { get; set; }

		private TrayIconState _state;
		public TrayIconState State {
			get { return _state; }
			set { SetState(value); }
		}

		public string ToolTipText {
			get { return TrayIcon.ToolTipText; }
			set {
				TrayIcon.ToolTipText = value;
				TrayIcon.ToolTip = value;
			}
		}

		public TrayManager() { }
		public TrayManager(TaskbarIcon ti) {
			TrayIcon = ti;
		}

		public void SetState(TrayIconState state) {
			_state = state;
			switch (state) {
				case TrayIconState.Disabled:
					_setIcon(Properties.Resources.InactiveTrayIcon);
					break;
				case TrayIconState.Message:
					_setIcon(Properties.Resources.MessageTrayIcon);
					break;
				case TrayIconState.NotificationOff:
					_setIcon(Properties.Resources.NotifyOffTrayIcon);
					break;
				default:
					_setIcon(Properties.Resources.TrayIcon);
					break;
			}
		}

		private void _setIcon(Icon icon) {
			if (TrayIcon != null)
				TrayIcon.Icon = icon;
		}
	}
}
