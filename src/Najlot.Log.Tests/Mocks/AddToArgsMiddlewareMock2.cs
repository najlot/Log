// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(AddToArgsMiddlewareMock2))]
	public sealed class AddToArgsMiddlewareMock2 : IMiddleware
	{
		public IMiddleware NextMiddleware { get; set; }

		public AddToArgsMiddlewareMock2()
		{
		}

		public void Execute(IEnumerable<LogMessage> messages)
		{
			foreach (var message in messages)
			{
				var args = new List<object>(message.RawArguments);
				args.Add(2);
				message.RawArguments = args;
			}

			NextMiddleware.Execute(messages);
		}

		public void Flush()
		{
		}

		public void Dispose() => Flush();
	}
}