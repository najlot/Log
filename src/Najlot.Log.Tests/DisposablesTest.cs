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
			var logAdminitrators = new List<LogAdminitrator>();

			for (int i = 0; i < 1000; i++)
			{
				logAdminitrators.Add(
					LogAdminitrator
						.CreateNew()
						.GetLogConfiguration(out var logConfiguration)
						.AddCustomDestination(new LogDestinationMock(msg => { }))
						.AddCustomDestination(new SecondLogDestinationMock(msg => { })));
			}

			foreach (var logAdminitrator in logAdminitrators)
			{
				var logger = logAdminitrator.GetLogger("1");
				var logger2 = logAdminitrator.GetLogger("2");
				
				// Check there are no exceptions when used wrong
				logAdminitrator.Dispose();
			}
		}

		[Fact]
		public void ConfigurationMustNotNotifyDisposedPrototypes()
		{
			var observerNotified = false;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Debug)
				.GetLogConfiguration(out var logConfiguration);

			var observer = new ConfigurationChangedObserverMock(config =>
			{
				observerNotified = true;
			});

			logConfiguration.AttachObserver(observer);
			logAdminitrator.SetLogLevel(LogLevel.Info);
			Assert.True(observerNotified, "Observer was not notified before dispose");

			observerNotified = false;

			logConfiguration.DetachObserver(observer);
			logAdminitrator.SetLogLevel(LogLevel.Warn);
			Assert.False(observerNotified, "Observer was notified after dispose");
		}
	}
}