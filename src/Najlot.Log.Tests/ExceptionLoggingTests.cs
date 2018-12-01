using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ExceptionLoggingTests
	{
		[Fact]
		public void ExceptionMustBeLogged()
		{
			var logged = false;
			bool fail = false;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					logged = true;

					if (fail) return;

					fail = msg.Exception == null;

					if (fail) return;

					fail = !msg.ExceptionIsValid;
				}));

			var log = logAdminitrator.GetLogger("Exception tests");

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

		[Fact]
		public void ExceptionMustBeLoggedToMultipleDestinations()
		{
			bool logged = false;
			bool loggedToSecond = false;
			bool fail = false;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					logged = true;

					if (fail) return;

					fail = msg.Exception == null;

					if (fail) return;

					fail = !msg.ExceptionIsValid;
				}))
				.AddCustomDestination(new SecondLogDestinationMock(msg =>
				{
					loggedToSecond = true;

					if (fail) return;

					fail = msg.Exception == null;

					if (fail) return;

					fail = !msg.ExceptionIsValid;
				}));

			var log = logAdminitrator.GetLogger("Exception tests");

			try
			{
				throw new Exception("This exception must be logged!");
			}
			catch (Exception ex)
			{
				log.Trace("Trace: ", ex);
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Debug("Debug: ", ex);
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Info("Info: ", ex);
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Warn("Warn: ", ex);
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Error("Error: ", ex);
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Fatal("Fatal: ", ex);
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;
			}

			Assert.False(fail, "Test failed");
		}
	}
}