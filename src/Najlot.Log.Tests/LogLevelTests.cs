using Najlot.Log.Configuration;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class LogLevelTests
	{
		private readonly LogLevel[] logLevels = {
				LogLevel.Trace,
				LogLevel.Debug,
				LogLevel.Info,
				LogLevel.Warn,
				LogLevel.Error,
				LogLevel.Fatal,
				LogLevel.None
			};

		[Fact]
		public void CheckIsLogLevelEnabled()
		{
			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Warn)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			var logger = logAdminitrator.GetLogger(this.GetType());

			Assert.True(logger.IsEnabled(LogLevel.Warn));
			Assert.True(logger.IsEnabled(LogLevel.Error));
			Assert.False(logger.IsEnabled(LogLevel.Info));
		}

		[Fact]
		public void LoggerMustLogWithCorrectLogLevel()
		{
			var gotLogMessage = false;
			var shouldGetMessage = true;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Fatal)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out var logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					gotLogMessage = true;
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			foreach (var logLevel in logLevels)
			{
				logAdminitrator.SetLogLevel(logLevel);

				shouldGetMessage = logLevel <= LogLevel.Trace;
				log.Trace("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Trace message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Debug;
				log.Debug("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Debug message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Info;
				log.Info("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Info message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Warn;
				log.Warn("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Warn message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Error;
				log.Error("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Error message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Fatal;
				log.Fatal("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Fatal message");
				gotLogMessage = false;
			}
		}

		[Fact]
		public void LoggerMustLogMultipleWithCorrectLogLevel()
		{
			var gotLogMessage = false;
			var gotSecondLogMessage = false;
			var shouldGetMessage = true;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Fatal)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out var logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					gotLogMessage = true;
				}))
				.AddCustomDestination(new SecondLogDestinationMock(msg =>
				{
					gotSecondLogMessage = true;
				}))
				.AddConsoleLogDestination(useColors: true);

			var log = logAdminitrator.GetLogger(this.GetType());

			foreach (var logLevel in logLevels)
			{
				logAdminitrator.SetLogLevel(logLevel);

				shouldGetMessage = logLevel <= LogLevel.Trace;
				log.Trace("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Trace message");
				Assert.True(gotSecondLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got second Trace message");
				gotLogMessage = false;
				gotSecondLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Debug;
				log.Debug("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Debug message");
				Assert.True(gotSecondLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got second Debug message");
				gotLogMessage = false;
				gotSecondLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Info;
				log.Info("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Info message");
				Assert.True(gotSecondLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got second Info message");
				gotLogMessage = false;
				gotSecondLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Warn;
				log.Warn("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Warn message");
				Assert.True(gotSecondLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got second Warn message");
				gotLogMessage = false;
				gotSecondLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Error;
				log.Error("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Error message");
				Assert.True(gotSecondLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got second Error message");
				gotLogMessage = false;
				gotSecondLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Fatal;
				log.Fatal("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got Fatal message");
				Assert.True(gotSecondLogMessage == shouldGetMessage, $"{logConfiguration.LogLevel}, but did not got second Fatal message");
				gotLogMessage = false;
				gotSecondLogMessage = false;
			}
		}
	}
}