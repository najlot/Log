using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class DenyLogDestinationMockFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(Type destinationType, LogMessage message)
		{
			return destinationType != typeof(LogDestinationMock);
		}
	}
}