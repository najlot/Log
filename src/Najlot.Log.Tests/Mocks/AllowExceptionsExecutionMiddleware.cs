﻿// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(AllowExceptionsExecutionMiddleware))]
	public sealed class AllowExceptionsExecutionMiddleware : IExecutionMiddleware
	{
		public void Execute(Action execute)
		{
			execute();
		}

		public void Dispose() => Flush();

		public void Flush() { }
	}
}