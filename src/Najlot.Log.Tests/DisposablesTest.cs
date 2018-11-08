using Najlot.Log.Configuration;
using Najlot.Log.Tests.Mocks;
using System.Collections.Generic;
using Xunit;

namespace Najlot.Log.Tests
{
	public class DisposablesTest
	{
		[Fact]
		public void LogConfiguratorMustBeDisposable()
		{
			var logConfigurators = new List<LogConfigurator>();

			for (int i = 0; i < 1000; i++)
			{
				logConfigurators.Add(
					LogConfigurator
						.CreateNew()
						.GetLogConfiguration(out var logConfiguration)
						.AddCustomDestination(new LogDestinationMock(logConfiguration, msg => { }))
						.AddCustomDestination(new SecondLogDestinationMock(logConfiguration, msg => { })));
			}

			foreach (var logConfigurator in logConfigurators)
			{
				logConfigurator.GetLoggerPool(out var loggerPool);
				var logger = loggerPool.GetLogger("1");
				var logger2 = loggerPool.GetLogger("2");

				// logConfigurator disposes it, too.
				// Check there are no exceptions when used wrong
				loggerPool.Dispose();

				logConfigurator.Dispose();

				// Check there are no exceptions when used wrong
				logConfigurator.Dispose();
			}
		}

		[Fact]
		public void ConfigurationMustNotNotifyDisposedPrototypes()
		{
			var observerNotified = false;

			var configurator = LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Debug)
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			logConfiguration.LogLevel = LogLevel.Info;

			Assert.True(observerNotified, "Observer was not notified before dispose");
			observerNotified = false;

			loggerPool.Dispose();

			Assert.False(observerNotified, "Observer was notified after dispose");
		}
	}
}