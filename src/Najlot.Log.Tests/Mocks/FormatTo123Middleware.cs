namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(FormatTo123Middleware))]
	public class FormatTo123Middleware : Middleware.IFormatMiddleware
	{
		public string Format(LogMessage message) => "123";
	}
}