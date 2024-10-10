using System;

namespace Najlot.Log;

/// <summary>
/// Global handler for logging errors
/// </summary>
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
