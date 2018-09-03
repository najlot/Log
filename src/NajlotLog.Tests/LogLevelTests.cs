using NajlotLog.Configuration;
using NajlotLog.Middleware;
using NajlotLog.Tests.Mocks;
using System;
using Xunit;

namespace NajlotLog.Tests
{
	public class LogLevelTests
	{
		[Fact]
		public void LoggerMustLogWithCorrectLogLevel()
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

			logConfiguration.LogLevel = LogLevel.Info;

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

			logConfiguration.LogLevel = LogLevel.Warn;

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

			logConfiguration.LogLevel = LogLevel.Error;

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

			logConfiguration.LogLevel = LogLevel.Fatal;

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
