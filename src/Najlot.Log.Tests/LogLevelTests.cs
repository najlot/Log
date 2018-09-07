using Najlot.Log.Configuration;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class LogLevelTests
	{
		[Fact]
		public void CheckIsLogLevelEnabled()
		{
			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Warn)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.GetLoggerPool(out LoggerPool loggerPool);

			var logger = loggerPool.GetLogger(this.GetType());

			Assert.True(logger.IsEnabled(LogLevel.Warn));
			Assert.True(logger.IsEnabled(LogLevel.Error));
			Assert.False(logger.IsEnabled(LogLevel.Info));
		}

		[Fact]
		public void LoggerMustLogTraceWithCorrectLogLevel()
		{
			var gotLogMessage = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Fatal)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.LogLevel = LogLevel.Trace;

			log.Trace("");
			Assert.True(gotLogMessage, "LogLevel.Trace, but did not got Trace message");
			gotLogMessage = false;

			log.Debug("");
			Assert.True(gotLogMessage, "LogLevel.Trace, but did not got Debug message");
			gotLogMessage = false;

			log.Info("");
			Assert.True(gotLogMessage, "LogLevel.Trace, but did not got Info message");
			gotLogMessage = false;

			log.Warn("");
			Assert.True(gotLogMessage, "LogLevel.Trace, but did not got Warn message");
			gotLogMessage = false;

			log.Error("");
			Assert.True(gotLogMessage, "LogLevel.Trace, but did not got Error message");
			gotLogMessage = false;

			log.Fatal("");
			Assert.True(gotLogMessage, "LogLevel.Trace, but did not got Fatal message");
			gotLogMessage = false;
		}

		[Fact]
		public void LoggerMustLogDebugWithCorrectLogLevel()
		{
			var gotLogMessage = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Fatal)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.LogLevel = LogLevel.Debug;

			log.Trace("");
			Assert.False(gotLogMessage, "LogLevel.Debug, but got Trace message");
			gotLogMessage = false;

			log.Debug("");
			Assert.True(gotLogMessage, "LogLevel.Debug, but did not got Debug message");
			gotLogMessage = false;

			log.Info("");
			Assert.True(gotLogMessage, "LogLevel.Debug, but did not got Info message");
			gotLogMessage = false;

			log.Warn("");
			Assert.True(gotLogMessage, "LogLevel.Debug, but did not got Warn message");
			gotLogMessage = false;

			log.Error("");
			Assert.True(gotLogMessage, "LogLevel.Debug, but did not got Error message");
			gotLogMessage = false;

			log.Fatal("");
			Assert.True(gotLogMessage, "LogLevel.Debug, but did not got Fatal message");
			gotLogMessage = false;
		}

		[Fact]
		public void LoggerMustLogInfoWithCorrectLogLevel()
		{
			var gotLogMessage = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Fatal)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.LogLevel = LogLevel.Info;

			log.Trace("");
			Assert.False(gotLogMessage, "LogLevel.Info, not got Trace message");
			gotLogMessage = false;

			log.Debug("");
			Assert.False(gotLogMessage, "LogLevel.Info, but got Debug message");
			gotLogMessage = false;

			log.Info("");
			Assert.True(gotLogMessage, "LogLevel.Info, but did not got Info message");
			gotLogMessage = false;

			log.Warn("");
			Assert.True(gotLogMessage, "LogLevel.Info, but did not got Warn message");
			gotLogMessage = false;

			log.Error("");
			Assert.True(gotLogMessage, "LogLevel.Info, but did not got Error message");
			gotLogMessage = false;

			log.Fatal("");
			Assert.True(gotLogMessage, "LogLevel.Info, but did not got Fatal message");
			gotLogMessage = false;
		}

		[Fact]
		public void LoggerMustLogWarnWithCorrectLogLevel()
		{
			var gotLogMessage = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Fatal)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.LogLevel = LogLevel.Warn;

			log.Trace("");
			Assert.False(gotLogMessage, "LogLevel.Warn, but got Trace message");
			gotLogMessage = false;

			log.Debug("");
			Assert.False(gotLogMessage, "LogLevel.Warn, but got Debug message");
			gotLogMessage = false;

			log.Info("");
			Assert.False(gotLogMessage, "LogLevel.Warn, but got Info message");
			gotLogMessage = false;

			log.Warn("");
			Assert.True(gotLogMessage, "LogLevel.Warn, but did not got Warn message");
			gotLogMessage = false;

			log.Error("");
			Assert.True(gotLogMessage, "LogLevel.Warn, but did not got Error message");
			gotLogMessage = false;

			log.Fatal("");
			Assert.True(gotLogMessage, "LogLevel.Warn, but did not got Fatal message");
			gotLogMessage = false;
		}

		[Fact]
		public void LoggerMustLogErrorWithCorrectLogLevel()
		{
			var gotLogMessage = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Fatal)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.LogLevel = LogLevel.Error;

			log.Trace("");
			Assert.False(gotLogMessage, "LogLevel.Error, but got Trace message");
			gotLogMessage = false;

			log.Debug("");
			Assert.False(gotLogMessage, "LogLevel.Error, but got Debug message");
			gotLogMessage = false;

			log.Info("");
			Assert.False(gotLogMessage, "LogLevel.Error, but got Info message");
			gotLogMessage = false;

			log.Warn("");
			Assert.False(gotLogMessage, "LogLevel.Error, but got Warn message");
			gotLogMessage = false;

			log.Error("");
			Assert.True(gotLogMessage, "LogLevel.Error, but did not got Error message");
			gotLogMessage = false;

			log.Fatal("");
			Assert.True(gotLogMessage, "LogLevel.Error, but did not got Fatal message");
			gotLogMessage = false;
		}

		[Fact]
		public void LoggerMustLogFatalWithCorrectLogLevel()
		{
			var gotLogMessage = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Fatal)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, msg =>
				{
					gotLogMessage = true;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());
			
			logConfiguration.LogLevel = LogLevel.Fatal;

			log.Trace("");
			Assert.False(gotLogMessage, "LogLevel.Fatal, but got Trace message");
			gotLogMessage = false;

			log.Debug("");
			Assert.False(gotLogMessage, "LogLevel.Fatal, but got Debug message");
			gotLogMessage = false;

			log.Info("");
			Assert.False(gotLogMessage, "LogLevel.Fatal, but got Info message");
			gotLogMessage = false;

			log.Warn("");
			Assert.False(gotLogMessage, "LogLevel.Fatal, but got Warn message");
			gotLogMessage = false;

			log.Error("");
			Assert.False(gotLogMessage, "LogLevel.Fatal, but got Error message");
			gotLogMessage = false;

			log.Fatal("");
			Assert.True(gotLogMessage, "LogLevel.Fatal, but did not got Fatal message");
			gotLogMessage = false;
		}
	}
}
