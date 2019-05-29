// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class LoggerPoolTests
	{
		public LoggerPoolTests()
		{
			foreach (var type in typeof(LoggerPoolTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void LoggerPoolMustCacheEntries()
		{
			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.AddConsoleLogDestination()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
				}));

			var logForThis1 = logAdminitrator.GetLogger(this.GetType());
			var logForPool1 = logAdminitrator.GetLogger(typeof(LogAdminitrator));

			var logForThis2 = logAdminitrator.GetLogger(this.GetType());
			var logForPool2 = logAdminitrator.GetLogger(typeof(LogAdminitrator));

			Assert.StrictEqual(logForThis1, logForThis2);
			Assert.StrictEqual(logForPool1, logForPool2);

			Assert.NotStrictEqual(logForThis1, logForPool1);
			Assert.NotStrictEqual(logForThis2, logForPool2);
		}

		[Fact]
		public void MultiplePrototypesMustBeCopiedWithNewCategory()
		{
			bool gotLogMessage = false;
			string category = null;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddConsoleLogDestination()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					category = msg.Category;
					gotLogMessage = true;
				}));

			var thisType = this.GetType();
			var logAdminitratorType = typeof(LogAdminitrator);

			var logForThis = logAdminitrator.GetLogger(thisType);
			var logForPool = logAdminitrator.GetLogger(logAdminitratorType);

			logForThis.Info("log this");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType.FullName, category);

			gotLogMessage = false;
			category = null;

			logForPool.Info("log pool");

			Assert.True(gotLogMessage, "got no log message for LoggerPool-type");
			Assert.Equal(logAdminitratorType.FullName, category);

			logForThis = logAdminitrator.GetLogger(thisType);

			gotLogMessage = false;
			category = null;

			logForThis.Info("log this 2");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType.FullName, category);
		}

		[Fact]
		public void PrototypesMustBeCopiedWithNewCategory()
		{
			bool gotLogMessage = false;
			string category = null;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Debug)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					category = msg.Category;
					gotLogMessage = true;
				}));

			var thisType = this.GetType();
			var syncExecutionMiddlewareType = typeof(SyncExecutionMiddleware);

			var logForThis = logAdminitrator.GetLogger(thisType);
			var logForPool = logAdminitrator.GetLogger(syncExecutionMiddlewareType);

			logForThis.Info("log this");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType.FullName, category);

			gotLogMessage = false;
			category = null;

			logForPool.Info("log pool");

			Assert.True(gotLogMessage, "got no log message for LoggerPool-type");
			Assert.Equal(syncExecutionMiddlewareType.FullName, category);

			logForThis = logAdminitrator.GetLogger(thisType);

			gotLogMessage = false;
			category = null;

			logForThis.Info("log this 2");

			Assert.True(gotLogMessage, "got no log message for this type");
			Assert.Equal(thisType.FullName, category);
		}
	}
}