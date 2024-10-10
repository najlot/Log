// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks;

[LogConfigurationName(nameof(AddToArgsMiddlewareMock1))]
public sealed class AddToArgsMiddlewareMock1 : IMiddleware
{
	public IMiddleware NextMiddleware { get; set; }

	public AddToArgsMiddlewareMock1()
	{
	}

	public void Execute(IEnumerable<LogMessage> messages)
	{
		foreach (var message in messages)
		{
			var args = new List<object>(message.RawArguments)
			{
				1
			};
			message.RawArguments = args;
		}

		NextMiddleware.Execute(messages);
	}

	public void Flush()
	{
	}

	public void Dispose() => Flush();
}