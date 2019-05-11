using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class SecondLogDestinationMock : ILogDestination
	{
		private readonly Action<LogMessage> _logAction;

		public SecondLogDestinationMock(Action<LogMessage> logAction)
		{
			_logAction = logAction;
		}

		public void Dispose()
		{
			// Nothing to do
		}

		public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
		{
			foreach (var message in messages)
			{
				Log(message, formatMiddleware);
			}
		}

		public void Log(LogMessage message, IFormatMiddleware formatMiddleware)
		{
			_logAction(message);
		}
	}
}