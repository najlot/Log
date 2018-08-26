using NajlotLog.Configuration;
using NajlotLog.Middleware;
using NajlotLog.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace NajlotLog.Tests
{
	public partial class LogTests
	{
		[Fact]
		public void MiddlewareMockMustGetAndExecuteCorrectAction()
		{
			bool loggerGotAction = false;
			bool middlewareGotAction = false;
			var logMessageExpected = "test log message";
			var logMessageActual = "";

			var logger = new LoggerImplementationMock(msg =>
			{
				loggerGotAction = true;
				logMessageActual = msg.Message.ToString();
			});

			LogConfiguration.Instance.LogExecutionMiddleware = new LogExecutionMiddlewareMock(action =>
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
		public void SyncLogExecutionMiddlewareOneTime()
		{
			bool loggerGotAction = false;
			var logMessageExpected = "test log message";
			var logMessageActual = "";

			var logger = new LoggerImplementationMock(msg =>
			{
				loggerGotAction = true;
				logMessageActual = msg.Message.ToString();
			});

			LogConfiguration.Instance.LogExecutionMiddleware = new SyncLogExecutionMiddleware();

			logger.Info(logMessageExpected);
			
			Assert.True(loggerGotAction, "Logger did not got the action");
			Assert.Equal(logMessageExpected, logMessageActual);
		}

		[Fact]
		public void SyncLogExecutionMiddlewareMustMultipleTimes()
		{
			bool loggerGotAction = false;
			var logMessageActual = "";

			var logger = new LoggerImplementationMock(msg =>
			{
				loggerGotAction = true;
				logMessageActual = msg.Message.ToString();
			});

			LogConfiguration.Instance.LogExecutionMiddleware = new SyncLogExecutionMiddleware();

			for (int i = 0; i < 10; i++)
			{
				var logMessageExpected = i.ToString();
				loggerGotAction = false;
				
				logger.Info(logMessageExpected);

				Assert.True(loggerGotAction, $"Logger did not got the action[{i}]");
				Assert.Equal(logMessageExpected, logMessageActual);
			}
		}

		[Fact]
		public void DequeueTaskLogExecutionMiddlewareMultipleTimes()
		{
			int executionsExpected = 10;
			int executionsActual = 0;

			List<string> messages = new List<string>();
			
			var logger = new LoggerImplementationMock(msg =>
			{
				executionsActual++;
				var logMessageActual = msg.Message.ToString();
				bool couldRemove = messages.Remove(logMessageActual);
				Assert.True(couldRemove, "Could not remove " + logMessageActual);
			});

			LogConfiguration.Instance.LogExecutionMiddleware = new DequeueTaskLogExecutionMiddleware();

			for (int i = 0; i < executionsExpected; i++)
			{
				messages.Add(i.ToString());
			}

			for (int i = 0; i < executionsExpected; i++)
			{
				logger.Info(i.ToString());
			}
			
			logger.Flush();

			Assert.Equal(executionsExpected, executionsActual);
		}

		[Fact]
		public void DequeueTaskLogExecutionMiddlewareWithoutFlush()
		{
			int executionsExpected = 10;
			int executionsActual = 0;

			List<string> messages = new List<string>();

			var manualResetEventSlim = new ManualResetEventSlim(false);

			var logger = new LoggerImplementationMock(msg =>
			{
				executionsActual++;
				var logMessageActual = msg.Message.ToString();
				bool couldRemove = messages.Remove(logMessageActual);
				Assert.True(couldRemove, "Could not remove " + logMessageActual);

				if(executionsActual == executionsExpected)
				{
					manualResetEventSlim.Set();
				}
			});

			LogConfiguration.Instance.LogExecutionMiddleware = new DequeueTaskLogExecutionMiddleware();

			for (int i = 0; i < executionsExpected; i++)
			{
				messages.Add(i.ToString());
			}

			for (int i = 0; i < executionsExpected; i++)
			{
				logger.Info(i.ToString());
			}

			manualResetEventSlim.Wait(5000);

			Assert.Equal(executionsExpected, executionsActual);
		}

		[Fact]
		public void ChangeMiddlewareWhileExecuting()
		{
			int executionsExpected = 10000;
			int executionsActual = 0;

			List<string> messages = new List<string>();

			var logger = new LoggerImplementationMock(msg =>
			{
				executionsActual++;
				var logMessageActual = msg.Message.ToString();

				bool couldRemove = false;

				lock(messages) couldRemove = messages.Remove(logMessageActual);

				Assert.True(couldRemove, "Could not remove " + logMessageActual);
			});

			LogConfiguration.Instance.LogExecutionMiddleware = new DequeueTaskLogExecutionMiddleware();
			
			for (int i = 0; i < executionsExpected * 2; i++)
			{
				lock (messages) messages.Add(i.ToString());
			}

			for (int i = 0; i < executionsExpected; i++)
			{
				logger.Info(i.ToString());
			}

			LogConfiguration.Instance.LogExecutionMiddleware = new SyncLogExecutionMiddleware();
			
			for (int i = executionsExpected; i < executionsExpected * 2; i++)
			{
				logger.Info(i.ToString());
			}

			Assert.Equal(executionsExpected * 2, executionsActual);
		}
	}
}
