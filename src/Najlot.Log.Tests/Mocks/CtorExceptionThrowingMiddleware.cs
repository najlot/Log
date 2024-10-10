// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Tests.Mocks;

[LogConfigurationName(nameof(CtorExceptionThrowingMiddleware))]
public sealed class CtorExceptionThrowingMiddleware : IMiddleware
{
	public IMiddleware NextMiddleware { get; set; }

	public CtorExceptionThrowingMiddleware()
	{
		throw new Exception();
	}

	public void Execute(IEnumerable<LogMessage> messages)
	{
	}

	public void Flush()
	{
	}

	public void Dispose() => Flush();
}