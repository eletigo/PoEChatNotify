﻿using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace eletigo.PoeChatNotify.Controls {
	/// <summary>
	/// Логика взаимодействия для PoeBalloon.xaml
	/// </summary>
	public partial class PoeBalloon {
		public event MouseButtonEventHandler Click;

		private bool _isClosing;

		#region Depency Properties
		private static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(PoeBalloon), new PropertyMetadata("Title"));
		private static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(string), typeof(PoeBalloon), new PropertyMetadata("Message"));

		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public string Message {
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		#endregion

		public PoeBalloon() {
			InitializeComponent();
			TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
		}

		private void OnBalloonClosing(object sender, RoutedEventArgs e) {
			_isClosing = true;
		}

		public void Close() {
			var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
			taskbarIcon?.CloseBalloon();
		}

		private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			Close();
		}

		private void Grid_MouseEnter(object sender, MouseEventArgs e) {
			if (_isClosing) return;

			//the tray icon assigned this attached property to simplify access
			var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
			taskbarIcon?.ResetBalloonCloseTimer();
		}

		protected virtual void OnClick(MouseButtonEventArgs e) {
			Click?.Invoke(this, e);
		}

		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			OnClick(e);
		}
	}
}
