using System;
using System.Windows;
using System.Windows.Threading;

/// <summary>
/// Encapsulates a WPF dispatcher with added functionalities.
/// </summary>
public class DispatcherHelper {
	private static readonly DispatcherOperationCallback exitFrameCallback = ExitFrame;

	public static void Access(Action method) {
		try {
			if (method != null && !Application.Current.Dispatcher.CheckAccess()) {
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, method);
			} else {
				method?.Invoke();
			}
		} catch (NullReferenceException) { }
	}

	/// <summary>
	/// Processes all UI messages currently in the message queue.
	/// </summary>
	public static void DoEvents() {
		// Create new nested message pump.
		var nestedFrame = new DispatcherFrame();

		// Dispatch a callback to the current message queue, when getting called,
		// this callback will end the nested message loop.
		// note that the priority of this callback should be lower than that of UI event messages.
		var exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
			DispatcherPriority.Background, exitFrameCallback, nestedFrame);

		// pump the nested message loop, the nested message loop will immediately
		// process the messages left inside the message queue.
		Dispatcher.PushFrame(nestedFrame);

		// If the "exitFrame" callback is not finished, abort it.
		if (exitOperation.Status != DispatcherOperationStatus.Completed) {
			exitOperation.Abort();
		}
	}

	private static object ExitFrame(object state) {
		var frame = state as DispatcherFrame;

		// Exit the nested message loop.
		if (frame != null) frame.Continue = false;
		return null;
	}
}