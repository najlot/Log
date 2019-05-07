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
				log.Trace(ex, "Trace: ");
				Assert.True(logged);
				logged = false;

				log.Debug(ex, "Debug: ");
				Assert.True(logged);
				logged = false;

				log.Info(ex, "Info: ");
				Assert.True(logged);
				logged = false;

				log.Warn(ex, "Warn: ");
				Assert.True(logged);
				logged = false;

				log.Error(ex, "Error: ");
				Assert.True(logged);
				logged = false;

				log.Fatal(ex, "Fatal: ");
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
				log.Trace(ex, "Trace: ");
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Debug(ex, "Debug: ");
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Info(ex, "Info: ");
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Warn(ex, "Warn: ");
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Error(ex, "Error: ");
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;

				log.Fatal(ex, "Fatal: ");
				Assert.True(logged);
				Assert.True(loggedToSecond);
				logged = false;
				loggedToSecond = false;
			}

			Assert.False(fail, "Test failed");
		}
	}
}