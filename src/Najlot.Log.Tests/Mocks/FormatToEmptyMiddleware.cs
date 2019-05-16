namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(FormatToEmptyMiddleware))]
	public class FormatToEmptyMiddleware : Middleware.IFormatMiddleware
	{
		public string Format(LogMessage message) => "";
	}
}