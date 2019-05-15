using System;

namespace Najlot.Log.Middleware
{
	public class NoFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(LogMessage message) => true;
	}
}