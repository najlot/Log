namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(FormatToAbcMiddleware))]
	public class FormatToAbcMiddleware : Middleware.IFormatMiddleware
	{
		public string Format(LogMessage message) => "Abc";
	}
}