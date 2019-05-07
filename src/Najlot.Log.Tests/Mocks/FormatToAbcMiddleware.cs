namespace Najlot.Log.Tests.Mocks
{
	public class FormatToAbcMiddleware : Middleware.IFormatMiddleware
	{
		public string Format(LogMessage message) => "Abc";
	}
}