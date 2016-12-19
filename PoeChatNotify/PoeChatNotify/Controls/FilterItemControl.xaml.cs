using System.Windows;
using eletigo.PoeChatNotify.Model;
using eletigo.PoeChatNotify.Utility;

namespace eletigo.PoeChatNotify.Controls {
	/// <summary>
	/// Логика взаимодействия для FilterItemControl.xaml
	/// </summary>
	public partial class FilterItemControl {
		#region Dependency Properties
		private static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(string), typeof(FilterItemControl),
				new FrameworkPropertyMetadata("Filter Name"));
		private static readonly DependencyProperty IsFilterEnabledProperty =
			DependencyProperty.Register("IsFilterEnabled", typeof(bool), typeof(FilterItemControl));
		private static readonly DependencyProperty PatternTextProperty =
			DependencyProperty.Register("PatternText", typeof(string), typeof(FilterItemControl));
		private static readonly DependencyProperty IsRegexEnabledProperty =
			DependencyProperty.Register("IsRegexEnabled", typeof(bool), typeof(FilterItemControl));

		public string Header {
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public bool IsFilterEnabled {
			get { return (bool)GetValue(IsFilterEnabledProperty); }
			set { SetValue(IsFilterEnabledProperty, value); }
		}
		public string PatternText {
			get { return (string)GetValue(PatternTextProperty); }
			set { SetValue(PatternTextProperty, value); }
		}
		public bool IsRegexEnabled {
			get { return (bool)GetValue(IsRegexEnabledProperty); }
			set { SetValue(IsRegexEnabledProperty, value); }
		}
		#endregion

		public FilterItemControl() {
			InitializeComponent();
		}
	}
}
