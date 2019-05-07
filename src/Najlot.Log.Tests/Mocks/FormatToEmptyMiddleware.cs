namespace Najlot.Log.Tests.Mocks
{
	public class FormatToEmptyMiddleware : Middleware.IFormatMiddleware
	{
		public string Format(LogMessage message) => "";
	}
}