using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class LogDestinationFormatFunctionMock : ILogDestination
	{
		private readonly Action<string> _logAction;

		public LogDestinationFormatFunctionMock(Action<string> formattedLogAction)
		{
			_logAction = formattedLogAction;
		}

		public void Dispose()
		{
			// Nothing to do
		}

		public void Log(LogMessage message, IFormatMiddleware formatMiddleware)
		{
			_logAction(formatMiddleware.Format(message));
		}
	}
}