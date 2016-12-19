using System;
using System.ComponentModel;

namespace eletigo.PoeChatNotify.Utility {
	public static class EnumExtension {
		// http://stackoverflow.com/a/479417
		public static string GetDescription<T>(this T enumerationValue) where T : struct {
			var type = enumerationValue.GetType();
			if (!type.IsEnum) {
				throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));
			}

			//Tries to find a DescriptionAttribute for a potential friendly name
			//for the enum
			var memberInfo = type.GetMember(enumerationValue.ToString());
			if (memberInfo.Length <= 0) return enumerationValue.ToString();
			var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

			return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enumerationValue.ToString();
			//If we have no description attribute, just return the ToString of the enum
		}
	}
}
