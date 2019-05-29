// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(LogDestinationFormatFunctionMock))]
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

		public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
		{
			foreach (var message in messages)
			{
				Log(message, formatMiddleware);
			}
		}

		public void Log(LogMessage message, IFormatMiddleware formatMiddleware)
		{
			_logAction(formatMiddleware.Format(message));
		}
	}
}