using NajlotLog.Configuration;
using NajlotLog.Implementation;
using System;

namespace NajlotLog.Tests.Mocks
{
	public class LoggerImplementationMock : LoggerImplementationBase
	{
		Action<LogMessage> _logAction;

		public LoggerImplementationMock(ILogConfiguration configuration,Action<LogMessage> logAction) : base(configuration)
		{
			_logAction = logAction;
		}

		protected override void Log(LogMessage message)
		{
			_logAction(message);
		}
	}
}
