using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ScopeTests
	{
		[Fact]
		public void NestedScopesMustBeLogged()
		{
			object state = null;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					state = msg.State;
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

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

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<TaskExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					if (scopesAreNotCorrect) return;
					scopesAreNotCorrect = (string)msg.Message != (string)msg.State;
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

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

		[Fact]
		public void ScopeMustBeLogged()
		{
			object state = null;
			var scope = "testing scopes";

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					state = msg.State;
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

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

			var logAdminitrator = LogAdminitrator
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
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

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
		public void ScopesMustNotBeSharedBetweenThreads()
		{
			bool error = false;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					if (Environment.CurrentManagedThreadId != (int)msg.State)
					{
						error = true;
					}
				}));

			void action()
			{
				var log = logAdminitrator.GetLogger("test");

				using (log.BeginScope(Environment.CurrentManagedThreadId))
				{
					log.Info("");
					log.Info("");
					log.Info("");
					log.Info("");
					log.Info("");
					log.Info("");
					log.Info("");
					log.Info("");
					log.Info("");
					log.Info("");
				}
			}

			System.Threading.Tasks.Parallel.Invoke(
				action, action, action, action, action, action, action, action,
				action, action, action, action, action, action, action, action,
				action, action, action, action, action, action, action, action);

			logAdminitrator.Dispose();

			Assert.False(error);
		}

		[Fact]
		public void ScopesMustNotBeSharedBetweenThreadsInTask()
		{
			bool error = false;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					// state must be the same thread id the message comes from
					if (msg.Message.ToString() != msg.State.ToString())
					{
						error = true;
					}
				}));

			void action()
			{
				var log = logAdminitrator.GetLogger("test");

				using (log.BeginScope(Environment.CurrentManagedThreadId))
				{
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
					log.Info(Environment.CurrentManagedThreadId);
				}
			}

			System.Threading.Tasks.Parallel.Invoke(
				action, action, action, action, action, action, action, action,
				action, action, action, action, action, action, action, action,
				action, action, action, action, action, action, action, action);

			logAdminitrator.Dispose();

			Assert.False(error);
		}
	}
}