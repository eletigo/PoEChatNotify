using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace eletigo.PoeChatNotify.Utility {
	public partial class User32 {
		private const int GwlStyle = -16;
		private const int WS_MINIMIZE = 0x20000000;
		private const int WS_MAXIMIZE = 0x1000000;

		private static DateTime _lastCheck;

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		/// <summary>
		/// Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
		/// </summary>
		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

		[DllImport("user32.dll")]
		private static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		// http://www.roelvanlisdonk.nl/?p=4032
		private static void ActivateWindow(IntPtr mainWindowHandle) {
			// Guard: check if window already has focus.
			if (mainWindowHandle == GetForegroundWindow())
				return;

			// Show window maximized.
			ShowWindow(mainWindowHandle, GetPoeWindowState() == WindowState.Minimized ? SW_RESTORE : SW_SHOW);

			SimulateAlt();

			// Show window in forground.
			SetForegroundWindow(mainWindowHandle);
		}
	}
}
