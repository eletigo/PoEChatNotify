using System.Windows;
using eletigo.PoeChatNotify.Model;
using eletigo.PoeChatNotify.Utility;

namespace eletigo.PoeChatNotify.Controls {
	/// <summary>
	/// Логика взаимодействия для PoeListItem.xaml
	/// </summary>
	public partial class PoeListItem {
		#region Dependency Properties
		private static readonly DependencyProperty DateProperty =
			DependencyProperty.Register("Date", typeof(string), typeof(PoeListItem),
				new FrameworkPropertyMetadata("00/00/00 00:00:00"));
		private static readonly DependencyProperty TypeProperty =
			DependencyProperty.Register("Type", typeof(object), typeof(PoeListItem),
				new FrameworkPropertyMetadata("$"));
		private static readonly DependencyProperty DirectionProperty =
			DependencyProperty.Register("Direction", typeof(string), typeof(PoeListItem),
				new FrameworkPropertyMetadata("From"));
		private static readonly DependencyProperty GuildNameProperty =
			DependencyProperty.Register("GuildName", typeof(string), typeof(PoeListItem),
				new FrameworkPropertyMetadata("<GuildName>"));
		private static readonly DependencyProperty UserNameProperty =
			DependencyProperty.Register("UserName", typeof(string), typeof(PoeListItem),
				new FrameworkPropertyMetadata("UserName"));
		private static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(string), typeof(PoeListItem),
				new FrameworkPropertyMetadata("Message"));

		public string Date {
			get { return (string) GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}
		public object Type {
			get { return GetValue(TypeProperty); }
			set {
				if (value is MessageType)
					value = ((MessageType) value).GetDescription();
				SetValue(TypeProperty, value);
			}
		}
		public string Direction {
			get { return (string)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
		public string GuildName {
			get { return (string)GetValue(GuildNameProperty); }
			set { SetValue(GuildNameProperty, value); }
		}
		public string UserName {
			get { return (string)GetValue(UserNameProperty); }
			set { SetValue(UserNameProperty, value); }
		}
		public string Message {
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		#endregion

		public PoeListItem() {
			InitializeComponent();
		}
	}
}
