using System;

namespace Najlot.Log.Middleware
{
	[LogClassName(nameof(NoFilterMiddleware))]
	public class NoFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(Type destinationType, LogMessage message) => true;
	}
}