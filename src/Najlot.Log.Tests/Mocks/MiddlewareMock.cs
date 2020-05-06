// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(LogDestinationMock))]
	public sealed class MiddlewareMock : IMiddleware
	{
		private readonly Action<LogMessage> _logAction;
		public IMiddleware NextMiddleware { get; set; }

		public MiddlewareMock(Action<LogMessage> logAction)
		{
			_logAction = logAction;
		}

		public void Execute(IEnumerable<LogMessage> messages)
		{
			foreach (var message in messages)
			{
				_logAction(message);
			}
		}

		public void Flush()
		{
		}

		public void Dispose() => Flush();
	}
}