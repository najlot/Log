// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ConfigurationObserverTests
	{
		public ConfigurationObserverTests()
		{
			foreach (var type in typeof(ConfigurationObserverTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

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

			var configurator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Debug)
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			logConfiguration.AttachObserver(new ConfigurationChangedObserverMock(config =>
			{
				observerNotified = true;
			}));

			var name = LogConfigurationMapper.Instance.GetName<ConfigurationChangedObserverMock>();

			logConfiguration.SetFormatMiddleware<FormatToEmptyMiddleware>(name);
			Assert.True(observerNotified, "Observer was not notified on format middleware changed");

			observerNotified = false;

			logConfiguration.SetFormatMiddleware<FormatToEmptyMiddleware>(name);
			Assert.False(observerNotified, "Observer was notified, but format middleware was the same");
		}
	}
}