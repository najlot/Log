using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ExecutionMiddlewareTests
	{
		[Fact]
		public void ApplicationMustNotDieFromErrorsInDestinations()
		{
			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.GetLogConfiguration(out var logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					throw new NotImplementedException();
				}))
				.AddCustomDestination(new SecondLogDestinationMock(msg =>
				{
					throw new Exception("Test!");
				}));

			var log = logAdminitrator.GetLogger("default");

			log.Fatal("We will get some exceptions now!");

			logAdminitrator.SetExecutionMiddleware<SyncExecutionMiddleware>();

			log.Fatal("We will get more exceptions now!");

			log.Flush();
		}

		[Fact]
		public void AsynchronousMessagesShouldNotGetLost()
		{
			long executionsDone = 0;
			long executionsLogged = 0;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					Interlocked.Increment(ref executionsLogged);
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			Parallel.For(0, 1000000, nr =>
			{
				Interlocked.Increment(ref executionsDone);
				log.Info(nr);
			});

			log.Flush();

			Assert.Equal(executionsDone, executionsLogged);
		}

		[Fact]
		public void AsynchronousMessagesShouldNotGetLostWhenDequeueWithMultipleDestinations()
		{
			long executionsDone = 0;
			long executionsLogged = 0;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetExecutionMiddleware<TaskExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					Interlocked.Increment(ref executionsLogged);
				}))
				.AddCustomDestination(new SecondLogDestinationMock(msg =>
				{
					Interlocked.Increment(ref executionsLogged);
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			Parallel.For(0, 100000, nr =>
			{
				Interlocked.Increment(ref executionsDone);
				log.Info(nr);
			});

			log.Flush();

			Assert.Equal(executionsDone * 2, executionsLogged);
		}

		[Fact]
		public void AsynchronousMessagesShouldNotGetLostWithTaskExecutionMiddleware()
		{
			long executionsDone = 0;
			long executionsLogged = 0;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetExecutionMiddleware<TaskExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					Interlocked.Increment(ref executionsLogged);
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			Parallel.For(0, 100000, nr =>
			{
				Interlocked.Increment(ref executionsDone);
				log.Info(nr);
			});

			log.Flush();

			Assert.Equal(executionsDone, executionsLogged);
		}

		[Fact]
		public void MiddlewareCanBeChangedWhileExecuting()
		{
			bool removingError = false;
			int executionsExpected = 10000;
			int executionsActual = 0;
			List<string> messages = new List<string>();

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;

					bool couldRemove = false;

					lock (messages) couldRemove = messages.Remove(msg.Message);

					if (removingError) return;
					removingError = !couldRemove;
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			logAdminitrator.SetExecutionMiddleware<TaskExecutionMiddleware>();

			for (int i = 0; i < executionsExpected * 2; i++)
			{
				lock (messages) messages.Add(i.ToString());
			}

			for (int i = 0; i < executionsExpected; i++)
			{
				log.Info(i.ToString());
			}

			logAdminitrator.SetExecutionMiddleware<SyncExecutionMiddleware>();

			for (int i = executionsExpected; i < executionsExpected * 2; i++)
			{
				log.Info(i.ToString());
			}

			Assert.Equal(executionsExpected * 2, executionsActual);
			Assert.False(removingError, "got removing errors");
		}

		[Fact]
		public void MiddlewareMockMustExecuteDefinedAction()
		{
			bool loggerGotAction = false;
			var logMessageExpected = "test log message";
			var logMessageActual = "";

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					loggerGotAction = true;
					logMessageActual = msg.Message;
				}));

			var logger = logAdminitrator.GetLogger(this.GetType());

			logAdminitrator.SetExecutionMiddleware<SyncExecutionMiddleware>();

			logger.Info(logMessageExpected);

			Assert.True(loggerGotAction, "Logger did not got the action");
			Assert.Equal(logMessageExpected, logMessageActual);
		}

		[Fact]
		public void SyncExecutionMiddlewareMustNotLooseMessage()
		{
			bool loggerGotAction = false;
			var logMessageExpected = "test log message";
			var logMessageActual = "";

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					loggerGotAction = true;
					logMessageActual = msg.Message;
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			logAdminitrator.SetExecutionMiddleware<SyncExecutionMiddleware>();

			log.Info(logMessageExpected);

			Assert.True(loggerGotAction, "Logger did not got the action");
			Assert.Equal(logMessageExpected, logMessageActual);
		}

		[Fact]
		public void SyncExecutionMiddlewareMustNotLooseMessages()
		{
			bool loggerGotAction = false;
			var logMessageActual = "";

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					loggerGotAction = true;
					logMessageActual = msg.Message;
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			logAdminitrator.SetExecutionMiddleware<SyncExecutionMiddleware>();

			for (int i = 0; i < 10; i++)
			{
				var logMessageExpected = i.ToString();
				loggerGotAction = false;

				log.Info(logMessageExpected);

				Assert.True(loggerGotAction, $"Logger did not got the action[{i}]");
				Assert.Equal(logMessageExpected, logMessageActual);
			}
		}

		[Fact]
		public void TaskExecutionMiddlewareMustFinishWithoutFlush()
		{
			bool removingError = false;
			int executionsExpected = 10;
			int executionsActual = 0;
			List<string> messages = new List<string>();
			var manualResetEventSlim = new ManualResetEventSlim(false);

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;
					var logMessageActual = msg.Message;
					bool couldRemove = messages.Remove(logMessageActual);

					if (removingError) return;
					removingError = !couldRemove;

					if (executionsActual == executionsExpected)
					{
						manualResetEventSlim.Set();
					}
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			logAdminitrator.SetExecutionMiddleware<TaskExecutionMiddleware>();

			for (int i = 0; i < executionsExpected; i++)
			{
				messages.Add(i.ToString());
			}

			for (int i = 0; i < executionsExpected; i++)
			{
				log.Info(i.ToString());
			}

			manualResetEventSlim.Wait(5000);

			Assert.Equal(executionsExpected, executionsActual);
			Assert.False(removingError, "got removing errors");
		}

		[Fact]
		public void TaskExecutionMiddlewareMustMustNotBreakOnException()
		{
			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					throw new Exception("this exception must not break the application");
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			logAdminitrator.SetExecutionMiddleware<TaskExecutionMiddleware>();

			log.Info("test");
		}

		[Fact]
		public void TaskExecutionMiddlewareMustNotLooseMessages()
		{
			int executionsExpected = 10;
			int executionsActual = 0;
			List<string> messages = new List<string>();

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;
					var logMessageActual = msg.Message;
					bool couldRemove = messages.Remove(logMessageActual);
					Assert.True(couldRemove, "Could not remove " + logMessageActual);
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			logAdminitrator.SetExecutionMiddleware<TaskExecutionMiddleware>();

			for (int i = 0; i < executionsExpected; i++)
			{
				messages.Add(i.ToString());
			}

			for (int i = 0; i < executionsExpected; i++)
			{
				log.Info(i.ToString());
			}

			log.Flush();

			Assert.Equal(executionsExpected, executionsActual);
		}

		[Fact]
		public void TaskExecutionMiddlewareMustRestartTask()
		{
			int executionsExpected = 10;
			int executionsActual = 0;
			List<string> messages = new List<string>();

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;
					var logMessageActual = msg.Message;
					bool couldRemove = messages.Remove(logMessageActual);
					Assert.True(couldRemove, "Could not remove " + logMessageActual);
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			logAdminitrator.SetExecutionMiddleware<TaskExecutionMiddleware>();

			for (int i = 0; i < executionsExpected; i++)
			{
				messages.Add(i.ToString());
			}

			logAdminitrator.Flush();
			Thread.Sleep(100); // Wait to restart

			for (int i = 0; i < executionsExpected; i++)
			{
				log.Info(i.ToString());
			}

			logAdminitrator.Dispose();

			Assert.Equal(executionsExpected, executionsActual);
		}

		[Fact]
		public void ExecutionMiddlewareTypeMustNotBeNull()
		{
			int executionsActual = 0;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;
				}));

			var log = logAdminitrator.GetLogger(this.GetType());

			log.Warn("Normal");

			logAdminitrator.SetExecutionMiddlewareByType(null);

			log.Warn("After set to null");

			var logTest = logAdminitrator.GetLogger("Test");

			log.Warn("After set to null and with other logger");

			logAdminitrator.Dispose();

			Assert.Equal(3, executionsActual);
		}
	}
}