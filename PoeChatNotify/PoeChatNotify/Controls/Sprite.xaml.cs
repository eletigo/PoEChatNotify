using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace eletigo.PoeChatNotify.Controls {
	/// <summary>
	/// Логика взаимодействия для Sprite.xaml
	/// </summary>
	public partial class Sprite {
		private enum Part {
			Corner,
			HorizontalSide,
			VerticalSide,
			Center
		}

		public ImageSource TopLeft {
			get { return ((ImageBrush)rLeftTop.Fill).ImageSource; }
			set { rLeftTop.Fill = _createImageBrush(value, rLeftTop, Part.Corner); }
		}

		public ImageSource Top {
			get { return ((ImageBrush)rCenterTop.Fill).ImageSource; }
			set { rCenterTop.Fill = _createImageBrush(value, rLeftTop, Part.VerticalSide); }
		}

		public ImageSource TopRight {
			get { return ((ImageBrush)rRightTop.Fill).ImageSource; }
			set { rRightTop.Fill = _createImageBrush(value, rRightTop, Part.Corner); }
		}

		public ImageSource Left {
			get { return ((ImageBrush)rCenterLeft.Fill).ImageSource; }
			set { rCenterLeft.Fill = _createImageBrush(value, rCenterLeft, Part.HorizontalSide); }
		}

		public ImageSource Center {
			get { return ((ImageBrush)rCenter.Fill).ImageSource; }
			set { rCenter.Fill = _createImageBrush(value, rCenter, Part.Center); }
		}

		public ImageSource Right {
			get { return ((ImageBrush)rCenterRight.Fill).ImageSource; }
			set { rCenterRight.Fill = _createImageBrush(value, rCenterRight, Part.HorizontalSide); }
		}

		public ImageSource BottomLeft {
			get { return ((ImageBrush)rLeftBottom.Fill).ImageSource; }
			set { rLeftBottom.Fill = _createImageBrush(value, rLeftBottom, Part.Corner); }
		}

		public ImageSource Bottom {
			get { return ((ImageBrush)rCenterBottom.Fill).ImageSource; }
			set { rCenterBottom.Fill = _createImageBrush(value, rCenterBottom, Part.VerticalSide); }
		}

		public ImageSource BottomRight {
			get { return ((ImageBrush)rRightBottom.Fill).ImageSource; }
			set { rRightBottom.Fill = _createImageBrush(value, rRightBottom, Part.Corner); }
		}

		public Sprite() {
			InitializeComponent();
		}

		private ImageBrush _createImageBrush(ImageSource source, Rectangle parent, Part part) {
			var result = new ImageBrush(source);
			result.Stretch = Stretch.Uniform;
			result.Viewport = new Rect(0, 0, source.Width, source.Height);
			result.ViewportUnits = BrushMappingMode.Absolute;
			if (part != Part.Corner) {
				result.TileMode = TileMode.Tile;
			}
			_setRectangle(source, parent, part);
			return result;
		}

		private void _setRectangle(ImageSource source, Rectangle parent, Part part) {
			if (part == Part.Corner) {
				parent.Width = source.Width;
				parent.Height = source.Height;
			} else if (part == Part.HorizontalSide) {
				parent.Width = source.Width;
			} else if (part == Part.VerticalSide) {
				parent.Height = source.Height;
			}
		}
	}
}
