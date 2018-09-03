using NajlotLog.Configuration;
using NajlotLog.Destinations;
using System;

namespace NajlotLog.Tests.Mocks
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
