using NajlotLog.Configuration;
using NajlotLog.Tests.Mocks;
using System;
using Xunit;

namespace NajlotLog.Tests
{
	public class ConfigurationChangedObserverTests
	{
		[Fact]
		public void ConfigurationMustNotifyOnLogLevelChanged()
		{
			bool observerNotified = false;

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomAppender(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
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
				.AddCustomAppender(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());
			logConfiguration.LogExecutionMiddleware = new LogExecutionMiddlewareMock(null);

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
				.AddCustomAppender(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					observerNotified = true;

					Func<LogMessage, string> format;
					Assert.True(config.TryGetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), out format),
						"Observer notified, but could not get function");

					Assert.Equal(testString, format(new LogMessage(DateTime.Now, LogLevel.Info, null, null)));
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
		public void ConfigurationMustNotNotifyOnFormatFunctionChangedForOther()
		{
			bool observerNotified = false;

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomAppender(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
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
				.AddCustomAppender(new ConfigurationChangedObserverMock(logConfiguration, (config) =>
				{
					wrongObserverNotified = true;
				}))
				.AddCustomAppender(new SecondConfigurationChangedObserverMock(logConfiguration, (config) =>
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
	}
}
