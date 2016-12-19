using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace eletigo.PoeChatNotify.Model {
	public enum MessageType {
		Local,
		[Description("#")]
		Global,
		[Description("%")]
		Party,
		[Description("@")]
		Whisper,
		[Description("$")]
		Trade,
		[Description("&")]
		Guild,
		[Description("^")]
		Twitch,
		Unknown
	}

	public enum MessageDirection {
		To,
		From,
		Unknown
	}

	public class MessageItem {
		private const string Pattern = @"(?<Date>\d{4}/\d{2}/\d{2}\s\d{2}:\d{2}:\d{2})[^\]]+\]\s(?<Type>[$@%#])(?<User>[^:]+):\s(?<Message>.*)";

		private const string PatternFrom = @"^(From |От кого |De )";
		private const string PatternTo = @"^(To |Кому |Para )";

		private const string PatternGuild = @"<.+>";

		public DateTime Date { get; set; }
		public MessageType Type { get; set; }
		public MessageDirection Direction { get; set; }

		public string GuildName { get; set; }
		public string UserName { get; set; }
		public string Message { get; set; }

		public static MessageItem Parse(string line) {
			if (!Regex.IsMatch(line, Pattern)) return null;

			var groups = Regex.Match(line, Pattern).Groups;

			var user = groups["User"].Value;
			var dir = GetMessageDirection(user);
			user = _parseName(user);
			var guild = _parseGuild(ref user);

			return new MessageItem {
				Date = DateTime.ParseExact(groups["Date"].Value, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture),
				Direction = dir,
				GuildName = guild,
				UserName = user,
				Message = groups["Message"].Value,
				Type = GetMessageType(groups["Type"].Value)
			};
		}

		private static string _parseGuild(ref string name) {
			if (!Regex.IsMatch(name, PatternGuild)) return "";
			var guild = Regex.Match(name, PatternGuild).Value;
			name = Regex.Replace(name, PatternGuild, "").Trim();
			return guild;
		}

		private static string _parseName(string name) {
			if (Regex.IsMatch(name, PatternFrom))
				return Regex.Replace(name, PatternFrom, "");
			if (Regex.IsMatch(name, PatternTo))
				return Regex.Replace(name, PatternTo, "");
			return name;
		}

		public static MessageDirection GetMessageDirection(string direction) {
			if (Regex.IsMatch(direction, PatternFrom))
				return MessageDirection.From;
			if (Regex.IsMatch(direction, PatternTo))
				return MessageDirection.To;
			return MessageDirection.Unknown;
		}

		public static MessageType GetMessageType(string type) {
			switch (type) {
				case "#":
					return MessageType.Global;
				case "%":
					return MessageType.Party;
				case "@":
					return MessageType.Whisper;
				case "$":
					return MessageType.Trade;
				case "&":
					return MessageType.Guild;
				case "^":
					return MessageType.Twitch;
				default:
					return MessageType.Unknown;
			}
		}

		public override string ToString() {
			return $"{Date:G} {Type} {UserName} {Message}";
		}
	}
}
