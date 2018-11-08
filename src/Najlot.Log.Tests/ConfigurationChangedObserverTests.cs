using Najlot.Log.Configuration;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ConfigurationObserverTests
	{
		[Fact]
		public void ConfigurationMustNotifyPrototypes()
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

			Assert.True(observerNotified, "Observer was not notified");
		}

		[Fact]
		public void ConfigurationMustNotifyOnLogLevelChanged()
		{
			bool observerNotified = false;

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.LogLevel++;

			Assert.True(observerNotified, "Observer was not notified on log level changed");
		}

		[Fact]
		public void ConfigurationMustNotifyOnExecutionMiddlewareChanged()
		{
			bool observerNotified = false;

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());
			logConfiguration.ExecutionMiddleware = new ExecutionMiddlewareMock(null);

			Assert.True(observerNotified, "Observer was not notified on middleware changed");
		}

		[Fact]
		public void ConfigurationMustNotifyOnFormatFunctionChanged()
		{
			var testString = "new foo";
			bool observerNotified = false;

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;

					Func<LogMessage, string> format;
					Assert.True(config.TryGetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), out format),
						"Observer notified, but could not get function");

					Assert.Equal(testString, format(new LogMessage(DateTime.Now, LogLevel.Info, null, null, null)));
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.TrySetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), msg =>
			{
				return testString;
			});

			Assert.True(observerNotified, "Observer was not notified on format function changed");
		}

		[Fact]
		public void ConfigurationMustNotNotifyOnFormatFunctionSetTwice()
		{
			bool observerNotified = false;
			string testFunc(LogMessage msg) => "";

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.TrySetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), testFunc);
			Assert.True(observerNotified, "Observer was not notified on format function changed");

			observerNotified = false;

			logConfiguration.TrySetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), testFunc);
			Assert.False(observerNotified, "Observer was not notified, but format funtion was the same");
		}

		[Fact]
		public void ConfigurationMustNotNotifyOnFormatFunctionAlreadySetBeforeAddingDestination()
		{
			bool observerNotified = false;
			string testFunc(LogMessage msg) => "123";

			var configurator = LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			logConfiguration.TrySetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), testFunc);

			configurator
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;
				}), testFunc)
				.GetLoggerPool(out LoggerPool loggerPool);

			Assert.False(observerNotified, "Observer was not notified, but format funtion was the same");
		}

		[Fact]
		public void ConfigurationMustNotNotifyOnFormatFunctionChangedForOther()
		{
			bool observerNotified = false;

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			bool couldSet = logConfiguration.TrySetFormatFunctionForType(typeof(SecondConfigurationChangedObserverMock), msg =>
			{
				return "";
			});

			Assert.True(couldSet, "Could not set function for " + nameof(SecondConfigurationChangedObserverMock));
			Assert.False(observerNotified, "Observer was notified on format function changed for other");
		}

		[Fact]
		public void ConfigurationMustNotifyOnFormatFunctionChangedForOther()
		{
			bool wrongObserverNotified = false;
			bool rightObserverNotified = false;

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					wrongObserverNotified = true;
				}))
				.AddCustomDestination(new SecondConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					rightObserverNotified = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			bool couldSet = logConfiguration.TrySetFormatFunctionForType(typeof(SecondConfigurationChangedObserverMock), msg =>
			{
				return "";
			});

			Assert.True(couldSet, "Could not set function for " + nameof(SecondConfigurationChangedObserverMock));
			Assert.False(wrongObserverNotified, "Observer was notified on format function changed for other");
			Assert.True(rightObserverNotified, "Observer was notified on format function changed for other");
		}

		[Fact]
		public void ConfigurationMustNotChangeMiddlewareToNull()
		{
			bool observerNotified = false;

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out var logConfiguration)
				.AddCustomDestination(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.ExecutionMiddleware = null;

			Assert.NotNull(logConfiguration.ExecutionMiddleware);
			Assert.False(observerNotified, "Observer was notified on format function changed for other");
		}
	}
}