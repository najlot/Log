using Najlot.Log.Configuration;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ConfigurationObserverTests
	{
		[Fact]
		public void ConfigurationMustNotifyOnExecutionMiddlewareChanged()
		{
			bool observerNotified = false;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetExecutionMiddleware<TaskExecutionMiddleware>()
				.SetLogLevel(LogLevel.Debug)
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			var configurationChangedObserverMock = new ConfigurationChangedObserverMock(config =>
			{
				observerNotified = true;
			});

			logConfiguration.AttachObserver(configurationChangedObserverMock);
			logAdminitrator.SetExecutionMiddleware<SyncExecutionMiddleware>();

			Assert.True(observerNotified, "Observer was not notified on middleware changed");
		}

		[Fact]
		public void ConfigurationMustNotifyOnLogLevelChanged()
		{
			var observerNotified = false;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Debug)
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			var configurationChangedObserverMock = new ConfigurationChangedObserverMock(config =>
			{
				observerNotified = true;
			});

			logConfiguration.AttachObserver(configurationChangedObserverMock);
			logAdminitrator.SetLogLevel(LogLevel.Error);

			Assert.True(observerNotified, "Observer was not notified");
		}
		[Fact]
		public void ConfigurationMustNotNotifyOnFormatFunctionSetTwiceButOnce()
		{
			bool observerNotified = false;
			string testFunc(LogMessage msg) => "";

			var configurator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Debug)
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			logConfiguration.AttachObserver(new ConfigurationChangedObserverMock(config =>
			{
				observerNotified = true;
			}));

			logConfiguration.TrySetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), testFunc);
			Assert.True(observerNotified, "Observer was not notified on format function changed");

			observerNotified = false;

			logConfiguration.TrySetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), testFunc);
			Assert.False(observerNotified, "Observer was notified, but format funtion was the same");
		}
	}
}