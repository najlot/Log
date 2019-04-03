using System;

namespace Najlot.Log.Middleware
{
	public class OpenFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(Type destinationType, LogMessage message) => true;
	}
}