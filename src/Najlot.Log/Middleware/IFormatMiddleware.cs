namespace Najlot.Log.Middleware
{
	public interface IFormatMiddleware
	{
		string Format(LogMessage message);
	}
}