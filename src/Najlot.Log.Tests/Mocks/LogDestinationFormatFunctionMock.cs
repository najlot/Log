using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class LogDestinationFormatFunctionMock : ILogDestination
	{
		private Action<string> _logAction;

		public LogDestinationFormatFunctionMock(ILogConfiguration configuration, Action<string> formattedLogAction)
		{
			_logAction = formattedLogAction;
		}

		public void Dispose()
		{
		}

		public void Log(LogMessage message, Func<LogMessage, string> formatFunc)
		{
			_logAction(formatFunc(message));
		}
	}
}