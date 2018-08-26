using NajlotLog.Implementation;
using System;

namespace NajlotLog.Tests.Mocks
{
	public class LoggerImplementationMock : LoggerImplementationBase
	{
		Action<LogMessage> _logAction;

		public LoggerImplementationMock(Action<LogMessage> logAction) : base()
		{
			_logAction = logAction;
		}

		protected override void Log(LogMessage message)
		{
			_logAction(message);
		}
	}
}
