// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(LogDestinationMock))]
	public sealed class LogDestinationMock : ILogDestination
	{
		private readonly Action<LogMessage> _logAction;

		public LogDestinationMock(Action<LogMessage> logAction)
		{
			_logAction = logAction;
		}

		public void Dispose()
		{
			// Nothing to clean up
		}

		public void Log(IEnumerable<LogMessage> messages)
		{
			foreach (var message in messages)
			{
				_logAction(message);
			}
		}
	}
}