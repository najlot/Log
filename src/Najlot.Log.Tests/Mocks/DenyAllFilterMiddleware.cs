using Najlot.Log.Middleware;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class DenyAllFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(LogMessage message)
		{
			return false;
		}
	}
}