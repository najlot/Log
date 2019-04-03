namespace Najlot.Log.Middleware
{
	public interface IFilterMiddleware
	{
		bool AllowThrough(LogMessage message);
	}
}