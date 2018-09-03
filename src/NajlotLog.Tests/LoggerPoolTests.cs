using NajlotLog.Configuration;
using NajlotLog.Middleware;
using NajlotLog.Tests.Mocks;
using System;
using Xunit;

namespace NajlotLog.Tests
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
		public void PrototypesMustBeCopiedWithNewSourceType()
		{
			bool gotLogMessage = false;
			Type sourceType = null;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Debug)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					sourceType = msg.SourceType;
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var thisType = this.GetType();
			var loggerPoolType = typeof(LoggerPool);

			var logForThis = loggerPool.GetLogger(thisType);
			var logForPool = loggerPool.GetLogger(loggerPoolType);

			logForThis.Info("log this");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType, sourceType);

			gotLogMessage = false;
			sourceType = null;

			logForPool.Info("log pool");

			Assert.True(gotLogMessage, "got no log message for LoggerPool-type");
			Assert.Equal(loggerPoolType, sourceType);

			logForThis = loggerPool.GetLogger(thisType);

			gotLogMessage = false;
			sourceType = null;

			logForThis.Info("log this 2");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType, sourceType);
		}

		[Fact]
		public void MultiplePrototypesMustBeCopiedWithNewSourceType()
		{
			bool gotLogMessage = false;
			Type sourceType = null;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddConsoleLogDestination()
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					sourceType = msg.SourceType;
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var thisType = this.GetType();
			var loggerPoolType = typeof(LoggerPool);

			var logForThis = loggerPool.GetLogger(thisType);
			var logForPool = loggerPool.GetLogger(loggerPoolType);

			logForThis.Info("log this");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType, sourceType);

			gotLogMessage = false;
			sourceType = null;

			logForPool.Info("log pool");

			Assert.True(gotLogMessage, "got no log message for LoggerPool-type");
			Assert.Equal(loggerPoolType, sourceType);

			logForThis = loggerPool.GetLogger(thisType);

			gotLogMessage = false;
			sourceType = null;

			logForThis.Info("log this 2");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType, sourceType);
		}
	}
}
