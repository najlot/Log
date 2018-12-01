using Xunit;

namespace Najlot.Log.Tests
{
	public class LogConfiguratorTests
	{
		[Fact]
		public void LogConfiguratorInstanceMustNotBreak()
		{
			LogAdminitrator.Instance
				.GetLogConfiguration(out var logConfiguration)
				.AddConsoleLogDestination()
				.GetLoggerPool(out var loggerPool);

			var log = loggerPool.GetLogger("default");

			log.Fatal("Hello, World!");
		}
	}
}