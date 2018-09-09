using Najlot.Log.Configuration;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.Collections.Generic;
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
				.AddCustomDestination(new LogDestinationMock(logConfiguration, (msg) =>
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
		public void NestedScopesMustBeLogged()
		{
			object state = null;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, (msg) =>
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
			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(logConfiguration, (msg) =>
				{
					Assert.Equal((string)msg.Message, (string)msg.State);
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
				}

				log.Info("scope 1");
			}
			
			log.Flush();
		}
	}
}
