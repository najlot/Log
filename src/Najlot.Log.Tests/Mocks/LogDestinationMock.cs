using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public class LogDestinationMock : LogDestinationBase
	{
		Action<LogMessage> _logAction;

		public LogDestinationMock(ILogConfiguration configuration,Action<LogMessage> logAction) : base(configuration)
		{
			_logAction = logAction;
		}

		protected override void Log(LogMessage message)
		{
			_logAction(message);
		}
	}
}