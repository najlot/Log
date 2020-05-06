// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(ExecuteExceptionThrowingMiddleware))]
	public sealed class ExecuteExceptionThrowingMiddleware : IMiddleware
	{
		private readonly Action<LogMessage> _logAction;
		public IMiddleware NextMiddleware { get; set; }

		public ExecuteExceptionThrowingMiddleware()
		{
		}

		public void Execute(IEnumerable<LogMessage> messages)
		{
			throw new Exception();
		}

		public void Flush()
		{
		}

		public void Dispose() => Flush();
	}
}