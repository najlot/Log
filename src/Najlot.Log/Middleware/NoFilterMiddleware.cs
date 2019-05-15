using System;

namespace Najlot.Log.Middleware
{
	[LogClassName(nameof(NoFilterMiddleware))]
	public class NoFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(LogMessage message) => true;
	}
}