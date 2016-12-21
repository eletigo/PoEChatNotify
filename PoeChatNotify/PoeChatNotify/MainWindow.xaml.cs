using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using eletigo.PoeChatNotify.Controls;
using eletigo.PoeChatNotify.Managers;
using eletigo.PoeChatNotify.Model;
using eletigo.PoeChatNotify.Monitors;
using eletigo.PoeChatNotify.Settings;
using eletigo.PoeChatNotify.Utility;

namespace eletigo.PoeChatNotify {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		private const string ExeClientPath = @"logs\Client.txt";

		private const int CopyWhisper = 0;
		private const int CopyInvite = 1;

		private bool _isForseClose;
		private bool _isNewMessage;

		private SettingsWindow _settings;
		private readonly ClientMonitor _client;
		private readonly PoeMonitor _poe;

		private readonly TrayManager _tray;
		private readonly MediaPlayer _media;

		public MainWindow() {
			InitializeComponent();

			_tray = new TrayManager(tbiTaskbarIcon) { State = TrayIconState.Disabled };

			_poe = new PoeMonitor();
			_poe.StateChanged += _poe_StateChanged;

			_client = new ClientMonitor {
				ClientPath = Config.Instance.ClientFilePath
			};

			_client.Append += _client_Append;

			_media = new MediaPlayer();

			// Controls
			_bindingFilterCheckers();

			miNitificationChecker.IsChecked = Config.Instance.IsNotificationsEnabled;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			// Client Scan
			_runScanning();

			if (_client.IsScaning) return;
			_poe.ProcessChanged += _poe_ProcessChanged;
			_poe.Scan();
		}

		private void Window_StateChanged(object sender, EventArgs e) {
			if (WindowState == WindowState.Minimized &&
				Config.Instance.IsMinimizeToTrayWhenMininized) {
				_hideToTray();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (!_isForseClose && Config.Instance.IsMinimizeToTrayWhenClosing) {
				_hideToTray();
				e.Cancel = true;
				return;
			}

			_poe?.Stop();
			_client?.Stop();

			// Save Config
			if (cbxFilterGlobal.IsChecked != null)
				Config.Instance.Filters[MessageType.Global].IsEnabled = (bool)cbxFilterGlobal.IsChecked;
			if (cbxFilterParty.IsChecked != null)
				Config.Instance.Filters[MessageType.Party].IsEnabled = (bool)cbxFilterParty.IsChecked;
			if (cbxFilterWhisper.IsChecked != null)
				Config.Instance.Filters[MessageType.Whisper].IsEnabled = (bool)cbxFilterWhisper.IsChecked;
			if (cbxFilterTrade.IsChecked != null)
				Config.Instance.Filters[MessageType.Trade].IsEnabled = (bool)cbxFilterTrade.IsChecked;
			if (cbxFilterGuild.IsChecked != null)
				Config.Instance.Filters[MessageType.Guild].IsEnabled = (bool)cbxFilterGuild.IsChecked;

			Config.Instance.Save();

			Application.Current.Shutdown();
		}

		private void Window_Activated(object sender, EventArgs e) {
			_messageReaded();
			_media.Stop();
		}

		private void _poe_StateChanged(object sender, StateChangedEventArgs e) {
			if (!e.IsForeground || !_isNewMessage) return;
			DispatcherHelper.Access(_messageReaded);
			_poe.Stop();
		}

		private void _messageReaded() {
			if (!_isNewMessage) return;
			_isNewMessage = false;
			_updateTrayIcon();
		}

		private void _poe_ProcessChanged(object sender, ProcessChangedEventArgs e) {
			if (e.ChangedType != ProcessChangedType.Opened) return;
			DispatcherHelper.Access(_runScanning);
			_poe.ProcessChanged -= _poe_ProcessChanged;
		}

		// http://stackoverflow.com/a/16204794
		public static bool IsWindowOpen<T>(string name = "") where T : Window {
			return string.IsNullOrEmpty(name)
				? Application.Current.Windows.OfType<T>().Any()
				: Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
		}

		private static bool _isMessageValid(MessageItem item) {
			if (!Config.Instance.Filters.ContainsKey(item.Type)) return false;
			var filter = Config.Instance.Filters[item.Type];
			if (!filter.IsEnabled) return false;
			return !filter.IsRegexEnabled || Regex.IsMatch(item.Message, filter.RegexPattern);
		}

		private void _client_Append(object sender, ClientChangedEventArgs e) {
			DispatcherHelper.Access(() => {
				// Filter messages from user
				if (e.MessageItem.Direction == MessageDirection.To) return;

				// Filter
				if (!e.MessageItem.IsDisconnect && !_isMessageValid(e.MessageItem)) return;

				// Chat List
				lbxChat.Items.Add(e.MessageItem);
				lbxChat.ScrollIntoView(lbxChat.Items[lbxChat.Items.Count - 1]);

				if (!Config.Instance.IsNotifyWhenPoeChatNotifyIsActive && IsActive) return;

				// Notification
				if (!Config.Instance.IsNotificationsEnabled) return;
				if (e.MessageItem.IsDisconnect && !Config.Instance.IsNotifyWhenDisconnect) return;
				if (Config.Instance.IsNotifyOnlyWhenPoEIsInactive && User32.IsPoeInForeground()) return;

				// Tray Icon Notify
				_isNewMessage = true;
				_updateTrayIcon();
				_poe.Scan();

				string title;
				if (!e.MessageItem.IsDisconnect)
					title = $"{e.MessageItem.Type.GetDescription()}" +
							$"{(!string.IsNullOrEmpty(e.MessageItem.GuildName) ? e.MessageItem.GuildName + " " : "")}" +
							$"{e.MessageItem.UserName}";
				else title = $"{e.MessageItem.UserName}";

				var balloon = new PoeBalloon {
					Title = title,
					Message = e.MessageItem.Message,
					IsDisconnect = e.MessageItem.IsDisconnect
				};
				balloon.Click += (o, args) => {
					if (Config.Instance.ClickShowProgram == 0) {
						User32.BringPoeToForeground();
					} else if (Config.Instance.ClickShowProgram == 1) {
						_toggleWindow();
					}

					_copyToClipboard(e.MessageItem, Config.Instance.ClickCopyToClipboard);

					balloon.Close();
				};

				int? dur = null;
				if (Config.Instance.NotificationDuration != 0)
					dur = Config.Instance.NotificationDuration < 500 ? 500 : Config.Instance.NotificationDuration;
				tbiTaskbarIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, dur);

				// Play Sound
				if (!e.MessageItem.IsDisconnect) {
					if (!Config.Instance.IsPlaySoundOnMessage) return;
				} else {
					if (!Config.Instance.IsPlaySoundOnDisconnect) return;
				}
				if (Config.Instance.IsCustomSound && File.Exists(Config.Instance.CustomSoundPath)) {
					try {
						_media.Open(new Uri(Config.Instance.CustomSoundPath));
						_media.Volume = Config.Instance.CustomSoundVolume;
						_media.Play();
					} catch (Exception) {
						/*custom boop*/
					}
				} else {
					try {
						if (!e.MessageItem.IsDisconnect) {
							new SoundPlayer { Stream = Properties.Resources.NotifySound }.Play();
						} else {
							new SoundPlayer { Stream = Properties.Resources.NotifySoundError }.Play();
						}
					} catch (Exception) {
						/*boop*/
					}
				}
			});
		}

		private static void _bindElement(DependencyObject dObj, DependencyProperty dProp, object source, string path) {
			BindingOperations.SetBinding(dObj, dProp, new Binding {
				Source = source,
				Path = new PropertyPath(path),
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			});
		}

		private void _bindingFilterCheckers() {
			_bindElement(cbxFilterGlobal, ToggleButton.IsCheckedProperty, Config.Instance.Filters[MessageType.Global], "IsEnabled");
			_bindElement(cbxFilterParty, ToggleButton.IsCheckedProperty, Config.Instance.Filters[MessageType.Party], "IsEnabled");
			_bindElement(cbxFilterWhisper, ToggleButton.IsCheckedProperty, Config.Instance.Filters[MessageType.Whisper], "IsEnabled");
			_bindElement(cbxFilterTrade, ToggleButton.IsCheckedProperty, Config.Instance.Filters[MessageType.Trade], "IsEnabled");
			_bindElement(cbxFilterGuild, ToggleButton.IsCheckedProperty, Config.Instance.Filters[MessageType.Guild], "IsEnabled");
		}

		private void _updateTrayIcon() {
			if (_isNewMessage) {
				_tray.State = TrayIconState.Message;
				_tray.ToolTipText = "PoE Chat Notify\nYou have unread message!";
			} else if (_client.IsScaning) {
				if (Config.Instance.IsNotificationsEnabled) {
					_tray.State = TrayIconState.Normal;
					_tray.ToolTipText = "PoE Chat Notify\nScan started...";
				} else {
					_tray.State = TrayIconState.NotificationOff;
					_tray.ToolTipText = "PoE Chat Notify\nScan started...\nNotifications is OFF";
				}
			} else if (!_client.IsScaning) {
				_tray.State = TrayIconState.Disabled;
				_tray.ToolTipText = "PoE Chat Notify\nScan stoped!";
			}
		}

		private string _getClientPath() {
			User32.GetPoeWindow();
			var process = User32.GetPoeProc();
			var dir = Path.GetDirectoryName(process?.MainModule.FileName);
			return dir != null ? Path.Combine(dir, ExeClientPath) : string.Empty;
		}

		private void _runScanning() {
			if (!_client.IsScaning)
				_toggleScan();
		}

		private void _toggleScan() {
			if (!_client.IsScaning) {
				_client.ClientPath = Config.Instance.ClientFilePath;
				if (!File.Exists(_client.ClientPath)) {
					if (Config.Instance.IsAutoDetect) {
						_client.ClientPath = _getClientPath();
						if (!File.Exists(_client.ClientPath)) {
							MessageBox.Show("Please run Path of Exile game for detect Client.txt.",
								"Cannot running scanning Client.txt",
								MessageBoxButton.OK, MessageBoxImage.Information);
							return;
						}
						Config.Instance.ClientFilePath = _client.ClientPath;
					} else {
						MessageBox.Show($"File {_client.ClientPath} not found. Please set file path or enable auto-detection in settings.",
								"Cannot running scanning Client.txt",
								MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
				}
				_client.Scan();
				Indicator.Background = Brushes.DarkRed;
				Indicator.ToolTip = "Scanning Client.txt is running...\nPress to stop.";
			} else {
				_media.Stop();
				_client.Stop();
				Indicator.Background = Brushes.LightSlateGray;
				Indicator.ToolTip = "Scanning Client.txt is stoped!\nPress to start.";
			}
			_updateTrayIcon();
		}

		private void _hideToTray() {
			Hide();
		}

		private void _toggleWindow() {
			if (!IsActive) {
				Show();
				Activate();
				if (WindowState == WindowState.Minimized)
					WindowState = WindowState.Normal;
			} else {
				Hide();
			}
		}

		private void tbiTaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e) {
			_toggleWindow();
		}

		private void _showSettings() {
			if (!IsWindowOpen<SettingsWindow>()) {
				_settings = new SettingsWindow();
				_settings.Applied += _settings_Applied;
				_settings.Closed += _settings_Applied;
			}
			_settings.Show();
			_settings.Activate();
		}

		private void _settings_Applied(object sender, EventArgs e) {
			if (!Config.Instance.IsCustomSound || !Config.Instance.IsPlaySoundOnMessage)
				_media.Stop();
			_updateTrayIcon();
			_bindingFilterCheckers();
		}

		private void btnSettings_Click(object sender, RoutedEventArgs e) {
			_showSettings();
		}

		#region Context Menu

		private void _copyToClipboard(MessageItem mi, int metod) {
			if (metod == CopyWhisper)
				Clipboard.SetText($"@{mi.UserName}");
			else if (metod == CopyInvite)
				Clipboard.SetText($"/invite {mi.UserName}");
		}

		private MessageItem _getMessageItem(FrameworkElement mi) {
			PoeListItem sp = null;
			var placementTarget = ((ContextMenu)mi?.Parent)?.PlacementTarget;
			if (placementTarget != null)
				sp = placementTarget as PoeListItem;
			return sp?.DataContext as MessageItem;
		}

		private void MenuItemSettings_Click(object sender, RoutedEventArgs e) {
			_showSettings();
		}

		private void MenuItemShowWindow_Click(object sender, RoutedEventArgs e) {
			_toggleWindow();
		}

		private void MenuItemExit_Click(object sender, RoutedEventArgs e) {
			_isForseClose = true;
			Close();
		}

		private void MenuItemClearLict_Click(object sender, RoutedEventArgs e) {
			lbxChat.Items.Clear();
		}

		private void MenuItemWhisperUser_Click(object sender, RoutedEventArgs e) {
			var mi = _getMessageItem(sender as MenuItem);
			if (mi != null)
				_copyToClipboard(mi, CopyWhisper);
		}

		private void MenuItemInviteUser_Click(object sender, RoutedEventArgs e) {
			var mi = _getMessageItem(sender as MenuItem);
			if (mi != null)
				_copyToClipboard(mi, CopyInvite);
		}

		private void miNitificationChecker_Click(object sender, RoutedEventArgs e) {
			Config.Instance.IsNotificationsEnabled = miNitificationChecker.IsChecked;
			_updateTrayIcon();
		}
		#endregion

		private void Indicator_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			_toggleScan();
		}

		private void lbxChat_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			lbxChat.SelectedIndex = -1;
		}
	}
}
