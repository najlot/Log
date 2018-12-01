using Najlot.Log.Configuration;
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
			LogAdminitrator
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
				}))
				.GetLoggerPool(out var loggerPool);

			var log = loggerPool.GetLogger("default");

			log.Fatal("We will get some exceptions now!");

			logConfiguration.ExecutionMiddleware = new TaskExecutionMiddleware();

			log.Fatal("We will get more exceptions now!");

			log.Flush();
		}

		[Fact]
		public void AsynchronousMessagesShouldNotGetLost()
		{
			long executionsDone = 0;
			long executionsLogged = 0;

			LogAdminitrator
				.CreateNew()
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					Interlocked.Increment(ref executionsLogged);
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			Parallel.For(0, 1000000, nr =>
			{
				Interlocked.Increment(ref executionsDone);
				log.Info(nr);
			});

			log.Flush();

			Assert.Equal(executionsDone, executionsLogged);
		}

		[Fact]
		public void AsynchronousMessagesShouldNotGetLostWithTaskExecutionMiddleware()
		{
			long executionsDone = 0;
			long executionsLogged = 0;

			LogAdminitrator
				.CreateNew()
				.SetExecutionMiddleware<TaskExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					Interlocked.Increment(ref executionsLogged);
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			Parallel.For(0, 100000, nr =>
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

			LogAdminitrator
				.CreateNew()
				.SetExecutionMiddleware<TaskExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					Interlocked.Increment(ref executionsLogged);
				}))
				.AddCustomDestination(new SecondLogDestinationMock(msg =>
				{
					Interlocked.Increment(ref executionsLogged);
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			Parallel.For(0, 100000, nr =>
			{
				Interlocked.Increment(ref executionsDone);
				log.Info(nr);
			});

			log.Flush();

			Assert.Equal(executionsDone * 2, executionsLogged);
		}

		[Fact]
		public void MiddlewareMockMustGetAndExecuteDefinedAction()
		{
			bool loggerGotAction = false;
			bool middlewareGotAction = false;
			var logMessageExpected = "test log message";
			var logMessageActual = "";

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					loggerGotAction = true;
					logMessageActual = msg.Message.ToString();
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var logger = loggerPool.GetLogger(this.GetType());

			logConfiguration.ExecutionMiddleware = new ExecutionMiddlewareMock(action =>
			{
				middlewareGotAction = true;
				action();
			});

			logger.Info(logMessageExpected);

			Assert.True(middlewareGotAction, "Middleware did not got the action");
			Assert.True(loggerGotAction, "Logger did not got the action");
			Assert.Equal(logMessageExpected, logMessageActual);
		}

		[Fact]
		public void SyncExecutionMiddlewareMustNotLooseMessage()
		{
			bool loggerGotAction = false;
			var logMessageExpected = "test log message";
			var logMessageActual = "";

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					loggerGotAction = true;
					logMessageActual = msg.Message.ToString();
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.ExecutionMiddleware = new SyncExecutionMiddleware();

			log.Info(logMessageExpected);

			Assert.True(loggerGotAction, "Logger did not got the action");
			Assert.Equal(logMessageExpected, logMessageActual);
		}

		[Fact]
		public void SyncExecutionMiddlewareMustNotLooseMessages()
		{
			bool loggerGotAction = false;
			var logMessageActual = "";

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					loggerGotAction = true;
					logMessageActual = msg.Message.ToString();
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.ExecutionMiddleware = new SyncExecutionMiddleware();

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
		public void TaskExecutionMiddlewareMustNotLooseMessages()
		{
			int executionsExpected = 10;
			int executionsActual = 0;
			List<string> messages = new List<string>();

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;
					var logMessageActual = msg.Message.ToString();
					bool couldRemove = messages.Remove(logMessageActual);
					Assert.True(couldRemove, "Could not remove " + logMessageActual);
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.ExecutionMiddleware = new TaskExecutionMiddleware();

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

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;
					var logMessageActual = msg.Message.ToString();
					bool couldRemove = messages.Remove(logMessageActual);
					Assert.True(couldRemove, "Could not remove " + logMessageActual);
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.ExecutionMiddleware = new TaskExecutionMiddleware();

			for (int i = 0; i < executionsExpected; i++)
			{
				messages.Add(i.ToString());
			}

			logConfiguration.ExecutionMiddleware.Flush();
			Thread.Sleep(100);

			for (int i = 0; i < executionsExpected; i++)
			{
				log.Info(i.ToString());
			}

			logConfiguration.ExecutionMiddleware.Flush();

			Assert.Equal(executionsExpected, executionsActual);
		}

		[Fact]
		public void TaskExecutionMiddlewareMustFinishWithoutFlush()
		{
			bool removingError = false;
			int executionsExpected = 10;
			int executionsActual = 0;
			List<string> messages = new List<string>();
			var manualResetEventSlim = new ManualResetEventSlim(false);

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;
					var logMessageActual = msg.Message.ToString();
					bool couldRemove = messages.Remove(logMessageActual);

					if (removingError) return;
					removingError = !couldRemove;

					if (executionsActual == executionsExpected)
					{
						manualResetEventSlim.Set();
					}
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.ExecutionMiddleware = new TaskExecutionMiddleware();

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
		public void MiddlewareCanBeChangedWhileExecuting()
		{
			bool removingError = false;
			int executionsExpected = 10000;
			int executionsActual = 0;
			List<string> messages = new List<string>();

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					executionsActual++;
					var logMessageActual = msg.Message.ToString();

					bool couldRemove = false;

					lock (messages) couldRemove = messages.Remove(logMessageActual);

					if (removingError) return;
					removingError = !couldRemove;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.ExecutionMiddleware = new TaskExecutionMiddleware();

			for (int i = 0; i < executionsExpected * 2; i++)
			{
				lock (messages) messages.Add(i.ToString());
			}

			for (int i = 0; i < executionsExpected; i++)
			{
				log.Info(i.ToString());
			}

			logConfiguration.ExecutionMiddleware = new SyncExecutionMiddleware();

			for (int i = executionsExpected; i < executionsExpected * 2; i++)
			{
				log.Info(i.ToString());
			}

			Assert.Equal(executionsExpected * 2, executionsActual);
			Assert.False(removingError, "got removing errors");
		}
	}
}