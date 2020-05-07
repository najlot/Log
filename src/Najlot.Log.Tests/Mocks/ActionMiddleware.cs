// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class ActionMiddleware : IMiddleware
	{
		private readonly Action<IEnumerable<LogMessage>> _action;

		public IMiddleware NextMiddleware { get; set; }

		public ActionMiddleware(Action<IEnumerable<LogMessage>> action)
		{
			_action = action;
		}

		public void Dispose()
		{
			// nothing to do
		}

		public void Execute(IEnumerable<LogMessage> messages)
		{
			_action(messages);
		}

		public void Flush()
		{
			// nothing to do
		}
	}
}