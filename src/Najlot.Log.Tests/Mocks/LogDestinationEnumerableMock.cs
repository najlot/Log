// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(LogDestinationEnumerableMock))]
	public sealed class LogDestinationEnumerableMock : ILogDestination
	{
		private readonly Action<IEnumerable<LogMessage>> _logAction;

		public LogDestinationEnumerableMock(Action<IEnumerable<LogMessage>> logAction)
		{
			_logAction = logAction;
		}

		public void Dispose()
		{
			// Nothing to clean up
		}

		public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
		{
			_logAction(messages);
		}
	}
}