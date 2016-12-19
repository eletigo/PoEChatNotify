using Newtonsoft.Json;

namespace eletigo.PoeChatNotify.Settings {
	public class FilterItem {
		[JsonProperty("is_enabled")]
		public bool IsEnabled { get; set; }

		[JsonProperty("regex_is_enabled")]
		public bool IsRegexEnabled { get; set; }
		[JsonProperty("regex_pattern")]
		public string RegexPattern { get; set; }
	}
}
