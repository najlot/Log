using NajlotLog.Configuration;
using NajlotLog.Tests.Mocks;
using System;
using Xunit;

namespace NajlotLog.Tests
{
	public partial class LogTests
	{
		[Fact]
		public void ConfigurationMustNotifyOnLogLevelChange()
		{
			bool observerNotified = false;

			var observerMock = new ConfigurationChangedObserverMock((config) =>
			{
				observerNotified = true;
			});

			try
			{
				LogConfiguration.Instance.AttachObserver(observerMock);
				LogConfiguration.Instance.LogLevel++;

				Assert.True(observerNotified, "Observer was not notified on log level changed");
			}
			finally
			{
				LogConfiguration.Instance.DetachObserver(observerMock);
			}
		}
		
		[Fact]
		public void ConfigurationMustNotifyOnExecutionMiddlewareChange()
		{
			bool observerNotified = false;

			var observerMock = new ConfigurationChangedObserverMock((config) =>
			{
				observerNotified = true;
			});

			try
			{
				LogConfiguration.Instance.AttachObserver(observerMock);
				LogConfiguration.Instance.LogExecutionMiddleware = new LogExecutionMiddlewareMock(null);

				Assert.True(observerNotified, "Observer was not notified on middleware changed");
			}
			finally
			{
				LogConfiguration.Instance.DetachObserver(observerMock);
			}
		}

		[Fact]
		public void ConfigurationMustNotifyOnFormatFunctionChange()
		{
			var testString = "new foo";

			bool observerNotified = false;

			var observerMock = new ConfigurationChangedObserverMock((config) =>
			{
				observerNotified = true;

				Func<LogMessage, string> format;
				Assert.True(config.TryGetFormatFunctionForType(typeof(ConfigurationChangedObserverMock), out format), 
					"Observer notified, but could not get function");
				
				Assert.Equal(testString, format(new LogMessage(DateTime.Now, LogLevel.Info, null, null)));
			});

			try
			{
				LogConfiguration.Instance.AttachObserver(observerMock);
				LogConfiguration.Instance.TrySetFormatFunctionForType(observerMock.GetType(), msg =>
				{
					return testString;
				});

				Assert.True(observerNotified, "Observer was not notified on format function changed");
			}
			finally
			{
				LogConfiguration.Instance.DetachObserver(observerMock);
			}
		}

		[Fact]
		public void ConfigurationMustNotNotifyOnFormatFunctionChangeForOther()
		{
			bool observerNotified = false;
			
			var observerMock = new ConfigurationChangedObserverMock((config) =>
			{
				observerNotified = true;
			});
			
			try
			{
				LogConfiguration.Instance.AttachObserver(observerMock);

				bool couldSet = LogConfiguration.Instance.TrySetFormatFunctionForType(typeof(SecondConfigurationChangedObserverMock), msg =>
				{
					return "";
				});

				Assert.True(couldSet, "Could not set function for " + nameof(SecondConfigurationChangedObserverMock));
				Assert.False(observerNotified, "Observer was notified on format function changed for other");
			}
			finally
			{
				LogConfiguration.Instance.DetachObserver(observerMock);
			}
		}

		[Fact]
		public void ConfigurationMustNotifyOnFormatFunctionChangeForOther()
		{
			bool wrongObserverNotified = false;
			bool rightObserverNotified = false;

			var observerMock = new ConfigurationChangedObserverMock((config) =>
			{
				wrongObserverNotified = true;
			});

			var secondObserverMock = new SecondConfigurationChangedObserverMock((config) =>
			{
				rightObserverNotified = true;
			});

			try
			{
				LogConfiguration.Instance.AttachObserver(observerMock);
				LogConfiguration.Instance.AttachObserver(secondObserverMock);

				bool couldSet = LogConfiguration.Instance.TrySetFormatFunctionForType(secondObserverMock.GetType(), msg =>
				{
					return "";
				});

				Assert.True(couldSet, "Could not set function for " + nameof(SecondConfigurationChangedObserverMock));
				Assert.False(wrongObserverNotified, "Observer was notified on format function changed for other");
				Assert.True(rightObserverNotified, "Observer was notified on format function changed for other");
			}
			finally
			{
				LogConfiguration.Instance.DetachObserver(observerMock);
				LogConfiguration.Instance.DetachObserver(secondObserverMock);
			}
		}
	}
}
