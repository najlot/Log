using Najlot.Log.Configuration;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class LoggerPoolTests
	{
		[Fact]
		public void LoggerPoolMustCacheEntries()
		{
			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddConsoleLogDestination()
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{

				}))
				.GetLoggerPool(out LoggerPool loggerPool);
			
			var logForThis1 = loggerPool.GetLogger(this.GetType());
			var logForPool1 = loggerPool.GetLogger(typeof(LoggerPool));

			var logForThis2 = loggerPool.GetLogger(this.GetType());
			var logForPool2 = loggerPool.GetLogger(typeof(LoggerPool));

			Assert.StrictEqual(logForThis1, logForThis2);
			Assert.StrictEqual(logForPool1, logForPool2);

			Assert.NotStrictEqual(logForThis1, logForPool1);
			Assert.NotStrictEqual(logForThis2, logForPool2);
		}

		[Fact]
		public void LoggerPoolMustNotCacheNotInitializedEntries()
		{
			var configurator = LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.GetLoggerPool(out LoggerPool loggerPool);

			var notInitializedLog = loggerPool.GetLogger(this.GetType());

			configurator
				.AddConsoleLogDestination()
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{

				}));

			var initializedLog = loggerPool.GetLogger(this.GetType());

			Assert.NotStrictEqual(notInitializedLog, initializedLog);
		}

		[Fact]
		public void PrototypesMustBeCopiedWithNewCategory()
		{
			bool gotLogMessage = false;
			string category = null;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Debug)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					category = msg.Category;
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var thisType = this.GetType();
			var loggerPoolType = typeof(LoggerPool);

			var logForThis = loggerPool.GetLogger(thisType);
			var logForPool = loggerPool.GetLogger(loggerPoolType);

			logForThis.Info("log this");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType.FullName, category);

			gotLogMessage = false;
			category = null;

			logForPool.Info("log pool");

			Assert.True(gotLogMessage, "got no log message for LoggerPool-type");
			Assert.Equal(loggerPoolType.FullName, category);

			logForThis = loggerPool.GetLogger(thisType);

			gotLogMessage = false;
			category = null;

			logForThis.Info("log this 2");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType.FullName, category);
		}

		[Fact]
		public void MultiplePrototypesMustBeCopiedWithNewCategory()
		{
			bool gotLogMessage = false;
			string category = null;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddConsoleLogDestination()
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					category = msg.Category;
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var thisType = this.GetType();
			var loggerPoolType = typeof(LoggerPool);

			var logForThis = loggerPool.GetLogger(thisType);
			var logForPool = loggerPool.GetLogger(loggerPoolType);

			logForThis.Info("log this");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType.FullName, category);

			gotLogMessage = false;
			category = null;

			logForPool.Info("log pool");

			Assert.True(gotLogMessage, "got no log message for LoggerPool-type");
			Assert.Equal(loggerPoolType.FullName, category);

			logForThis = loggerPool.GetLogger(thisType);

			gotLogMessage = false;
			category = null;

			logForThis.Info("log this 2");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType.FullName, category);
		}
	}
}
