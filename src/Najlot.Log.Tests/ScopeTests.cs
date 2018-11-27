using Najlot.Log.Configuration;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ScopeTests
	{
		[Fact]
		public void ScopeMustBeLogged()
		{
			object state = null;
			var scope = "testing scopes";

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					state = msg.State;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			using (log.BeginScope(scope))
			{
				log.Warn("setting scope");
			}

			Assert.Equal(scope, (string)state);

			log.Warn("scope must be null now");

			Assert.Null(state);
		}

		[Fact]
		public void ScopeMustBeLoggedToMultiple()
		{
			object state = null;
			object secondState = null;

			var scope = "testing scopes";

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					state = msg.State;
				}))
				.AddCustomDestination(new SecondLogDestinationMock(msg =>
				{
					secondState = msg.State;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			using (log.BeginScope(scope))
			{
				log.Warn("setting scope");
			}

			Assert.Equal(scope, (string)state);
			Assert.Equal(scope, (string)secondState);

			log.Warn("scope must be null now");

			Assert.Null(state);
			Assert.Null(secondState);
		}

		[Fact]
		public void NestedScopesMustBeLogged()
		{
			object state = null;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					state = msg.State;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			using (log.BeginScope("scope 1"))
			{
				log.Warn("using first scope");
				Assert.Equal("scope 1", (string)state);

				using (log.BeginScope("scope 2"))
				{
					log.Info("using second scope");
					Assert.Equal("scope 2", (string)state);
					log.Info("... two times");
					Assert.Equal("scope 2", (string)state);
				}

				log.Info("... two times");
				Assert.Equal("scope 1", (string)state);
			}

			log.Warn("scope must be null now");

			Assert.Null(state);
		}

		[Fact]
		public void NestedScopesMustBeLoggedAsync()
		{
			bool scopesAreNotCorrect = false;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<TaskExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					if (scopesAreNotCorrect) return;
					scopesAreNotCorrect = (string)msg.Message != (string)msg.State;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			using (log.BeginScope("scope 1"))
			{
				log.Warn("scope 1");

				using (log.BeginScope("scope 2"))
				{
					log.Info("scope 2");
					log.Info("scope 2");

					using (log.BeginScope("scope 3"))
					{
						log.Info("scope 3");
					}

					log.Info("scope 2");
				}

				log.Info("scope 1");
			}

			log.Flush();

			Assert.False(scopesAreNotCorrect, "scopes are not correct");
		}
	}
}