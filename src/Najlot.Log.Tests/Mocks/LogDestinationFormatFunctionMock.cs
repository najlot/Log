using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public class LogDestinationFormatFunctionMock : LogDestinationBase
	{
		private Action<string> _logAction;

		public LogDestinationFormatFunctionMock(ILogConfiguration configuration, Action<string> formattedLogAction) : base(configuration)
		{
			_logAction = formattedLogAction;
		}

		protected override void Log(LogMessage message)
		{
			_logAction(Format(message));
		}
	}
}