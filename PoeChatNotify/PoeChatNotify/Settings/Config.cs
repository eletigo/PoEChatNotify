using System;
using System.Collections.Generic;
using System.IO;
using eletigo.PoeChatNotify.Model;
using Newtonsoft.Json;

namespace eletigo.PoeChatNotify.Settings {
	public class Config {
		private const string FileName = "config.json";
		private const string DefaultClientPath = @"C:\Program Files\Steam\SteamApps\common\Path of Exile\logs\Client.txt";

		// Application
		[JsonProperty("is_minimize_to_tray_when_mininized")]
		public bool IsMinimizeToTrayWhenMininized { get; set; } = true;

		[JsonProperty("is_minimize_to_tray_when_closing")]
		public bool IsMinimizeToTrayWhenClosing { get; set; } = true;

		// Filter Messages
		[JsonProperty("filters")]
		public Dictionary<MessageType, FilterItem> Filters { get; set; }

		// Client File
		[JsonProperty("client_file_path")]
		public string ClientFilePath { get; set; }

		[JsonProperty("is_auto_detected")]
		public bool IsAutoDetect { get; set; } = true;

		// Notification
		[JsonProperty("is_notification_enabled")]
		public bool IsNotificationsEnabled { get; set; } = true;

		[JsonProperty("is_notify_only_when_poe_is_inactive")]
		public bool IsNotifyOnlyWhenPoEIsInactive { get; set; }

		[JsonProperty("is_notify_when_poe_chat_notify_is_active")]
		public bool IsNotifyWhenPoeChatNotifyIsActive { get; set; }

		[JsonProperty("is_notify_when_disconnect")]
		public bool IsNotifyWhenDisconnect { get; set; } = true;

		/// <summary>
		/// 0 - PoE game
		/// 1 - PoE Chat Notify
		/// 2 - Nothing
		/// </summary>
		[JsonProperty("click_show_program")]
		public int ClickShowProgram { get; set; }

		/// <summary>
		/// 0 - Whisper. (@user_name)
		/// 1 - Invite. (/invite user_name)
		/// 2 - Nothing
		/// </summary>
		[JsonProperty("click_copy_to_clipboard")]
		public int ClickCopyToClipboard { get; set; }

		[JsonProperty("is_play_sound")]
		public bool IsPlaySoundOnMessage { get; set; } = true;

		[JsonProperty("is_play_sound_on_disconnect")]
		public bool IsPlaySoundOnDisconnect { get; set; } = true;

		[JsonProperty("is_custom_sound")]
		public bool IsCustomSound { get; set; }

		[JsonProperty("custom_sound_path")]
		public string CustomSoundPath { get; set; }

		[JsonProperty("custom_sound_volume")]
		public double CustomSoundVolume { get; set; }

		[JsonProperty("is_notification_duration")]
		public int NotificationDuration { get; set; }

		private static Config _config;

		public static Config Instance => _config ?? Create();

		private Config() {
			ClientFilePath = DefaultClientPath;
			NotificationDuration = 5000;
			ClickCopyToClipboard = 2;
			CustomSoundVolume = 1.0f;

			Filters = new Dictionary<MessageType, FilterItem> {
				{ MessageType.Global, new FilterItem() },
				{ MessageType.Party, new FilterItem() },
				{ MessageType.Whisper, new FilterItem {
					IsEnabled = true,
					IsRegexEnabled = true,
					RegexPattern = @"^(Hi, I would like to buy your)"
				} },
				{ MessageType.Trade, new FilterItem() },
				{ MessageType.Guild, new FilterItem() }
			};
		}

		public static Config Create() {
			_load();
			return _config ?? (_config = new Config());
		}

		private static void _load() {
			if (!File.Exists(FileName)) return;
			try {
				var json = File.ReadAllText(FileName);
				_config = JsonConvert.DeserializeObject<Config>(json);
			}
			catch (Exception) {
				// ignored
			}
		}

		public bool Save() {
			try {
				var json = JsonConvert.SerializeObject(this);
				File.WriteAllText(FileName, json);
				return true;
			} catch (Exception) {
				return false;
			}
		}
	}
}
