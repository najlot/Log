// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class LogLevelTests
	{
		public LogLevelTests()
		{
			foreach (var type in typeof(LogLevelTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		private readonly LogLevel[] _logLevels =
			{
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
			using var logAdministrator = LogAdministrator.CreateNew();
			logAdministrator.SetLogLevel(LogLevel.Warn);

			var logger = logAdministrator.GetLogger(GetType());

			Assert.False(logger.IsEnabled(LogLevel.Info));
			Assert.True(logger.IsEnabled(LogLevel.Warn));
			Assert.True(logger.IsEnabled(LogLevel.Error));
		}

		[Fact]
		public void LoggerMustLogTextWithCorrectLogLevel()
		{
			var gotLogMessage = false;

			using var logAdministrator = LogAdministrator.CreateNew();
			logAdministrator
				.SetLogLevel(LogLevel.Fatal)
				.AddCustomDestination(new DestinationMock(msg =>
				{
					gotLogMessage = true;
				}));

			var log = logAdministrator.GetLogger(GetType());

			foreach (var logLevel in _logLevels)
			{
				logAdministrator.SetLogLevel(logLevel);

				var shouldGetMessage = logLevel <= LogLevel.Trace;
				log.Trace("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Trace message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Debug;
				log.Debug("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Debug message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Info;
				log.Info("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Info message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Warn;
				log.Warn("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Warn message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Error;
				log.Error("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Error message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Fatal;
				log.Fatal("");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Fatal message");
				gotLogMessage = false;
			}
		}

		[Fact]
		public void LoggerMustLogObjectWithCorrectLogLevel()
		{
			var gotLogMessage = false;

			using var logAdministrator = LogAdministrator.CreateNew();
			logAdministrator
				.SetLogLevel(LogLevel.Fatal)
				.AddCustomDestination(new DestinationMock(msg =>
				{
					gotLogMessage = true;
				}));

			var log = logAdministrator.GetLogger(GetType());
			var obj = new object();

			foreach (var logLevel in _logLevels)
			{
				logAdministrator.SetLogLevel(logLevel);

				var shouldGetMessage = logLevel <= LogLevel.Trace;
				log.Trace(obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Trace message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Debug;
				log.Debug(obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Debug message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Info;
				log.Info(obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Info message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Warn;
				log.Warn(obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Warn message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Error;
				log.Error(obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Error message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Fatal;
				log.Fatal(obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Fatal message");
				gotLogMessage = false;
			}
		}

		[Fact]
		public void LoggerMustLogObjectAndExceptionWithCorrectLogLevel()
		{
			var gotLogMessage = false;
			var ex = new Exception("Something bad happened!");

			using var logAdministrator = LogAdministrator.CreateNew();
			logAdministrator
				.SetLogLevel(LogLevel.Fatal)
				.AddCustomDestination(new DestinationMock(msg =>
				{
					gotLogMessage = true;
				}));

			var log = logAdministrator.GetLogger(GetType());
			var obj = new object();

			foreach (var logLevel in _logLevels)
			{
				logAdministrator.SetLogLevel(logLevel);

				var shouldGetMessage = logLevel <= LogLevel.Trace;
				log.Trace(ex, obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Trace message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Debug;
				log.Debug(ex, obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Debug message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Info;
				log.Info(ex, obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Info message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Warn;
				log.Warn(ex, obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Warn message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Error;
				log.Error(ex, obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Error message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Fatal;
				log.Fatal(ex, obj);
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Fatal message");
				gotLogMessage = false;
			}
		}

		[Fact]
		public void LoggerMustLogStringAndExceptionWithCorrectLogLevel()
		{
			var gotLogMessage = false;
			var ex = new Exception("Something bad happened!");

			using var logAdministrator = LogAdministrator.CreateNew();
			logAdministrator
				.SetLogLevel(LogLevel.Fatal)
				.AddCustomDestination(new DestinationMock(msg =>
				{
					gotLogMessage = true;
				}));

			var log = logAdministrator.GetLogger(GetType());

			foreach (var logLevel in _logLevels)
			{
				logAdministrator.SetLogLevel(logLevel);

				var shouldGetMessage = logLevel <= LogLevel.Trace;
				log.Trace(ex, "");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Trace message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Debug;
				log.Debug(ex, "");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Debug message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Info;
				log.Info(ex, "");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Info message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Warn;
				log.Warn(ex, "");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Warn message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Error;
				log.Error(ex, "");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Error message");
				gotLogMessage = false;

				shouldGetMessage = logLevel <= LogLevel.Fatal;
				log.Fatal(ex, "");
				Assert.True(gotLogMessage == shouldGetMessage, $"{logLevel}, but did not got Fatal message");
				gotLogMessage = false;
			}
		}
	}
}