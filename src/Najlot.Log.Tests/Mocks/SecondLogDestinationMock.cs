using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class SecondLogDestinationMock : ILogDestination
	{
		private Action<LogMessage> _logAction;

		public SecondLogDestinationMock(Action<LogMessage> logAction)
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