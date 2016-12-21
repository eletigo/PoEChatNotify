using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using eletigo.PoeChatNotify.Controls;
using eletigo.PoeChatNotify.Model;
using eletigo.PoeChatNotify.Settings;
using Microsoft.Win32;

namespace eletigo.PoeChatNotify {
	/// <summary>
	/// Логика взаимодействия для SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow {
		public event EventHandler Applied;

		public SettingsWindow() {
			InitializeComponent();

			// Application
			cbxMinimizeToTrayWhenMininized.IsChecked = Config.Instance.IsMinimizeToTrayWhenMininized;
			cbxMinimizeToTrayWhenClosing.IsChecked = Config.Instance.IsMinimizeToTrayWhenClosing;

			// Client.txt
			tbxClientPath.Text = Config.Instance.ClientFilePath;
			cbxAutoDetect.IsChecked = Config.Instance.IsAutoDetect;

			// Notify
			cbxNotifyEnable.IsChecked = Config.Instance.IsNotificationsEnabled;

			tbxNotifyDuration.Text = Config.Instance.NotificationDuration.ToString();

			cbxNotifyOnlyWhenPoEIsInactive.IsChecked = Config.Instance.IsNotifyOnlyWhenPoEIsInactive;
			cbxNotifyWhenPoeChatNotifyIsActive.IsChecked = Config.Instance.IsNotifyWhenPoeChatNotifyIsActive;
			cbxNotifyWhenDisconnect.IsChecked = Config.Instance.IsNotifyWhenDisconnect;

			cbxShowOnClick.SelectedIndex = Config.Instance.ClickShowProgram;
			cbxCopyOnClick.SelectedIndex = Config.Instance.ClickCopyToClipboard;

			cbxNotifyPlaySound.IsChecked = Config.Instance.IsPlaySoundOnMessage;
			cbxNotifyPlaySoundError.IsChecked = Config.Instance.IsPlaySoundOnDisconnect;

			cbxCustonSoundEnable.IsChecked = Config.Instance.IsCustomSound;
			tbxCustomSoundPath.Text = Config.Instance.CustomSoundPath;
			sdrCustomSoundVolume.Value = Config.Instance.CustomSoundVolume;

			// Filter
			GlobalFilter.DataContext = Config.Instance.Filters[MessageType.Global];
			PartyFilter.DataContext = Config.Instance.Filters[MessageType.Party];
			WhisperFilter.DataContext = Config.Instance.Filters[MessageType.Whisper];
			TradeFilter.DataContext = Config.Instance.Filters[MessageType.Trade];
			GuildFilter.DataContext = Config.Instance.Filters[MessageType.Guild];
		}

		private static void _setFilterItem(FilterItemControl fic, MessageType mt) {
			Config.Instance.Filters[mt].IsEnabled = fic.IsFilterEnabled;
			Config.Instance.Filters[mt].IsRegexEnabled = fic.IsRegexEnabled;
			Config.Instance.Filters[mt].RegexPattern = fic.PatternText;
		}

		private void _saveConfig() {
			// Application
			if (cbxMinimizeToTrayWhenMininized.IsChecked != null)
				Config.Instance.IsMinimizeToTrayWhenMininized = (bool)cbxMinimizeToTrayWhenMininized.IsChecked;
			if (cbxMinimizeToTrayWhenClosing.IsChecked != null)
				Config.Instance.IsMinimizeToTrayWhenClosing = (bool)cbxMinimizeToTrayWhenClosing.IsChecked;

			// Client
			Config.Instance.ClientFilePath = tbxClientPath.Text;
			if (cbxAutoDetect.IsChecked != null)
				Config.Instance.IsAutoDetect = (bool)cbxAutoDetect.IsChecked;

			// Notify
			if (cbxNotifyEnable.IsChecked != null)
				Config.Instance.IsNotificationsEnabled = (bool)cbxNotifyEnable.IsChecked;
			int dur;
			if (int.TryParse(tbxNotifyDuration.Text, out dur))
				Config.Instance.NotificationDuration = dur;
			if (cbxNotifyOnlyWhenPoEIsInactive.IsChecked != null)
				Config.Instance.IsNotifyOnlyWhenPoEIsInactive = (bool)cbxNotifyOnlyWhenPoEIsInactive.IsChecked;
			if (cbxNotifyWhenPoeChatNotifyIsActive.IsChecked != null)
				Config.Instance.IsNotifyWhenPoeChatNotifyIsActive = (bool)cbxNotifyWhenPoeChatNotifyIsActive.IsChecked;
			if (cbxNotifyWhenDisconnect.IsChecked != null)
				Config.Instance.IsNotifyWhenDisconnect = (bool)cbxNotifyWhenDisconnect.IsChecked;

			Config.Instance.ClickShowProgram = cbxShowOnClick.SelectedIndex;
			Config.Instance.ClickCopyToClipboard = cbxCopyOnClick.SelectedIndex;

			// Sound
			if (cbxNotifyPlaySound.IsChecked != null)
				Config.Instance.IsPlaySoundOnMessage = (bool)cbxNotifyPlaySound.IsChecked;
			if (cbxNotifyPlaySoundError.IsChecked != null)
				Config.Instance.IsPlaySoundOnDisconnect = (bool)cbxNotifyPlaySoundError.IsChecked;

			if (cbxCustonSoundEnable.IsChecked != null)
				Config.Instance.IsCustomSound = (bool)cbxCustonSoundEnable.IsChecked;
			Config.Instance.CustomSoundPath = tbxCustomSoundPath.Text;
			Config.Instance.CustomSoundVolume = sdrCustomSoundVolume.Value;

			// Filter
			_setFilterItem(GlobalFilter, MessageType.Global);
			_setFilterItem(PartyFilter, MessageType.Party);
			_setFilterItem(WhisperFilter, MessageType.Whisper);
			_setFilterItem(TradeFilter, MessageType.Trade);
			_setFilterItem(GuildFilter, MessageType.Guild);

			Config.Instance.Save();
		}

		private void btnOpenFileDialogClient_Click(object sender, RoutedEventArgs e) {
			var ofd = new OpenFileDialog {
				Title = "Client log file",
				FileName = "Client.txt",
				Filter = "Client Log File|Client.txt"
			};
			if (ofd.ShowDialog() == true) {
				tbxClientPath.Text = ofd.FileName;
			}
		}

		private void btnOpenFileDialogSound_Click(object sender, RoutedEventArgs e) {
			var ofd = new OpenFileDialog {
				Title = "Sound for notification",
				Filter = "MP3 files|*.mp3|All files|*.*"
			};
			if (ofd.ShowDialog() == true) {
				tbxCustomSoundPath.Text = ofd.FileName;
			}
		}

		private void btnApply_Click(object sender, RoutedEventArgs e) {
			_saveConfig();
			OnApplied(new EventArgs());
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			_saveConfig();
			Close();
		}

		// http://stackoverflow.com/a/1268648
		private static bool IsTextAllowed(string text) {
			var regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
			return !regex.IsMatch(text);
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
			e.Handled = !IsTextAllowed(e.Text);
		}

		private void sdrCustomSoundVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			tbxValueVolume.Text = sdrCustomSoundVolume.Value.ToString("F");
		}

		protected virtual void OnApplied(EventArgs e) {
			Applied?.Invoke(this, e);
		}

		// http://stackoverflow.com/a/10238715
		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
