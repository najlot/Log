using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(DenyAllFilterMiddleware))]
	public sealed class DenyAllFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(LogMessage message) => false;
	}
}