namespace Najlot.Log.Tests.Mocks
{
	public class FormatTo123Middleware : Middleware.IFormatMiddleware
	{
		public string Format(LogMessage message) => "123";
	}
}