using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace POETradeHelper.Utility {
	// http://ru.stackoverflow.com/a/424199
	public static class BitmapExtension {
		public static BitmapSource ToBitmapSource(this Bitmap btm) {
			return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
				btm.GetHbitmap(),
				IntPtr.Zero,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
		}
	}
}
