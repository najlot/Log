using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class DenyAllFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(Type destinationType, LogMessage message) => false;
	}
}