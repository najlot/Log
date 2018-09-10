using Najlot.Log.Configuration;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ExceptionLoggingTests
	{
		[Fact]
		public void CheckExceptionIsLogged()
		{
			var logged = false;
			bool fail = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, (msg) =>
				{
					logged = true;

					if (fail) return;

					fail = msg.Exception == null;

					if (fail) return;

					fail = !msg.ExceptionIsValid;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger("Exception tests");

			try
			{
				throw new Exception("This exception must be logged!");
			}
			catch (Exception ex)
			{
				log.Trace("Trace: ", ex);
				Assert.True(logged);
				logged = false;

				log.Debug("Debug: ", ex);
				Assert.True(logged);
				logged = false;

				log.Info("Info: ", ex);
				Assert.True(logged);
				logged = false;

				log.Warn("Warn: ", ex);
				Assert.True(logged);
				logged = false;

				log.Error("Error: ", ex);
				Assert.True(logged);
				logged = false;

				log.Fatal("Fatal: ", ex);
				Assert.True(logged);
				logged = false;
			}

			Assert.False(fail, "failed");
		}
	}
}
