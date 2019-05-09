using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class LogDestinationMock : ILogDestination
	{
		private readonly Action<LogMessage> _logAction;

		public LogDestinationMock(Action<LogMessage> logAction)
		{
			_logAction = logAction;
		}

		public void Dispose()
		{
			// Nothing to clean up
		}

		public void Log((LogMessage, IFormatMiddleware)[] logMessageFormattingPair)
		{
			foreach (var entry in logMessageFormattingPair)
			{
				Log(entry.Item1, entry.Item2);
			}
		}

		public void Log(LogMessage message, IFormatMiddleware formatMiddleware)
		{
			_logAction(message);
		}
	}
}