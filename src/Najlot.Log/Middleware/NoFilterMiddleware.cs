using System;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(NoFilterMiddleware))]
	public class NoFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(LogMessage message) => true;
	}
}