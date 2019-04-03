namespace Najlot.Log.Middleware
{
	public class OpenFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(LogMessage message) => true;
	}
}