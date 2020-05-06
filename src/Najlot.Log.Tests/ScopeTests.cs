// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Tests.Mocks;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ScopeTests
	{
		public ScopeTests()
		{
			foreach (var type in GetType().Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void NestedScopesMustBeLogged()
		{
			bool scopesAreCorrect = true;
			object state = null;

			using (var logAdminitrator = LogAdministrator.CreateNew())
			{
				logAdminitrator.AddCustomDestination(new LogDestinationMock((msg) =>
				{
					if (!scopesAreCorrect) return;
					state = msg.State;
					scopesAreCorrect = msg.RawMessage == state?.ToString();
				}));

				var log = logAdminitrator.GetLogger(GetType());

				using (log.BeginScope("scope 1"))
				{
					log.Info("scope 1");

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

				Assert.True(scopesAreCorrect, "scopes are not correct");

				log.Info("out of scope");
				Assert.Null(state);
			}
		}

		[Fact]
		public void ScopesMustNotBeSharedBetweenThreads()
		{
			bool error = false;

			using (var logAdminitrator = LogAdministrator.CreateNew())
			{
				logAdminitrator.SetLogLevel(LogLevel.Info)
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
						for (int i = 0; i < 10; i++) log.Info("");
					}
				}

				var actions = Enumerable.Range(0, 20).Select<int, Action>(i => action).ToArray();
				Parallel.Invoke(actions);
			}

			Assert.False(error);
		}

		[Fact]
		public void ScopesMustNotBeSharedBetweenThreadsInTask()
		{
			bool error = false;

			using (var logAdminitrator = LogAdministrator.CreateNew())
			{
				logAdminitrator.SetLogLevel(LogLevel.Info)
					.AddCustomDestination(new LogDestinationMock((msg) =>
					{
						// state must be the same thread id the message comes from
						if (msg.RawMessage != msg.State.ToString())
						{
							error = true;
						}
					}));

				void action()
				{
					Task.Factory.StartNew(() =>
					{
						var log = logAdminitrator.GetLogger("test");

						using (log.BeginScope(Environment.CurrentManagedThreadId))
						{
							for (int i = 0; i < 10; i++) log.Info(Environment.CurrentManagedThreadId);
						}
					}).Wait();
				}

				var actions = Enumerable.Range(0, 20).Select<int, Action>(i => action).ToArray();
				Parallel.Invoke(actions);
			}

			Assert.False(error);
		}
	}
}