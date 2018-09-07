﻿using NajlotLog.Configuration;
using NajlotLog.Middleware;
using NajlotLog.Tests.Mocks;
using System;
using Xunit;

namespace NajlotLog.Tests
{
	public class ExceptionLoggingTests
	{
		[Fact]
		public void CheckExceptionIsLogged()
		{
			var logged = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, (msg) =>
				{
					logged = true;
					Assert.NotNull(msg.Exception);
					Assert.True(msg.ExceptionIsValid, "msg.ExceptionIsValid != true");
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger("Excetion tests");

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
		}
	}
}
