using System.Runtime.InteropServices;

namespace eletigo.PoeChatNotify.Utility {
	public partial class User32 {
		private const byte VK_ALT = 0xA4;
		private const int KEYEVENTF_EXTENDEDKEY = 0x1;
		private const int KEYEVENTF_KEYUP = 0x2;

		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		// http://www.roelvanlisdonk.nl/?p=4032
		private static void SimulateAlt() {
			// Simulate an "ALT" key press.
			keybd_event(VK_ALT, 0x45, KEYEVENTF_EXTENDEDKEY | 0, 0);

			// Simulate an "ALT" key release.
			keybd_event(VK_ALT, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
		}
	}
}
