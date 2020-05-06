using System;

namespace Najlot.Log
{
	public sealed class LogErrorHandler
	{
		public static LogErrorHandler Instance { get; } = new LogErrorHandler();

		private LogErrorHandler()
		{
		}

		public void Handle(string message, Exception ex)
		{
			var args = new LogErrorEventArgs
			{
				Message = message,
				Exception = ex
			};

			ErrorOccured?.Invoke(this, args);
		}

		public event EventHandler<LogErrorEventArgs> ErrorOccured;
	}
}
