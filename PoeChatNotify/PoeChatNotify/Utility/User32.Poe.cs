using System;
using System.Diagnostics;
using System.Windows;

namespace eletigo.PoeChatNotify.Utility {
	public partial class User32 {
		private static string WindowName = "";
		private static IntPtr _poeWindow;
		private static readonly string[] WindowNames = { "Path of Exile" };

		public const int SW_RESTORE = 9;
		public const int SW_SHOW = 5;

		public static IntPtr GetPoeWindow() {
			if (DateTime.Now - _lastCheck < new TimeSpan(0, 0, 5) && _poeWindow == IntPtr.Zero)
				return _poeWindow;
			if (_poeWindow != IntPtr.Zero && IsWindow(_poeWindow))
				return _poeWindow;
			foreach (var windowName in WindowNames) {
				_poeWindow = FindWindowByCaption(IntPtr.Zero, windowName);
				if (_poeWindow == IntPtr.Zero)
					continue;
				if (WindowName != null && WindowName != windowName) {
					WindowName = windowName;
				}
				break;
			}
			_lastCheck = DateTime.Now;
			return _poeWindow;
		}

		public static Process GetPoeProc() {
			if (_poeWindow == IntPtr.Zero)
				GetPoeWindow();
			if (_poeWindow == IntPtr.Zero)
				return null;
			try {
				uint procId;
				GetWindowThreadProcessId(_poeWindow, out procId);
				return Process.GetProcessById((int)procId);
			} catch {
				return null;
			}
		}

		public static WindowState GetPoeWindowState() {
			var hsWindow = GetPoeWindow();
			var state = GetWindowLong(hsWindow, GwlStyle);
			if ((state & WS_MAXIMIZE) == WS_MAXIMIZE)
				return WindowState.Maximized;
			return (state & WS_MINIMIZE) == WS_MINIMIZE ? WindowState.Minimized : WindowState.Normal;
		}

		public static bool IsPoeInForeground() => GetForegroundWindow() == GetPoeWindow();

		public static void BringPoeToForeground() {
			if (_poeWindow == IntPtr.Zero)
				GetPoeWindow();
			if (_poeWindow == IntPtr.Zero)
				return;
			ActivateWindow(_poeWindow);
			SetForegroundWindow(_poeWindow);
		}
	}
}
