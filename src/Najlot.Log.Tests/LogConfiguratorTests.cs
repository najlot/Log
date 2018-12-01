using Xunit;

namespace Najlot.Log.Tests
{
	public class LogConfiguratorTests
	{
		[Fact]
		public void LogConfiguratorInstanceMustNotBreak()
		{
			var logAdminitrator = LogAdminitrator.Instance
				.GetLogConfiguration(out var logConfiguration)
				.AddConsoleLogDestination();

			var log = logAdminitrator.GetLogger("default");

			log.Fatal("Hello, World!");
		}
	}
}