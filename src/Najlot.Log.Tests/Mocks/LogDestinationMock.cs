using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class LogDestinationMock : ILogDestination
	{
		private Action<LogMessage> _logAction;

		public LogDestinationMock(Action<LogMessage> logAction)
		{
			_logAction = logAction;
		}

		public void Dispose()
		{
		}

		public void Log(LogMessage message, Func<LogMessage, string> formatFunc)
		{
			_logAction(message);
		}
	}
}