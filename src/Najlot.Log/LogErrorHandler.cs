using System;

namespace Najlot.Log
{
	public class LogErrorHandler
	{
		public static LogErrorHandler Instance { get; } = new LogErrorHandler();

		private LogErrorHandler() { }

		public void Handle(string message)
		{
			OnErrorOccured(new LogErrorEventArgs
			{
				Message = message
			});
		}

		public void Handle(string message, Exception ex)
		{
			OnErrorOccured(new LogErrorEventArgs
			{
				Message = message,
				Exception = ex
			});
		}

		protected virtual void OnErrorOccured(LogErrorEventArgs e)
		{
			ErrorOccured?.Invoke(this, e);
		}

		public event EventHandler<LogErrorEventArgs> ErrorOccured;
	}
}
