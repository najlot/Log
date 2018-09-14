using Najlot.Log.Middleware;
using Xunit;
using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Tests
{
	public class ColorfulConsoleDestinationTests
	{
		[Fact]
		public void ColorfullConsoleDestinationMustNotFail()
		{
			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddColorfulConsoleDestination()
				.GetLoggerPool(out var loggerPool);

			var log = loggerPool.GetLogger("default");
			var exception = new Exception("test exception");

			for(int i=0;i<100;i++)
			{
				log.Trace(i);
				log.Debug(i);
				log.Info(i);
				log.Warn(i);
				log.Error(i);
				log.Fatal(i);

				log.Trace(i, exception);
				log.Debug(i, exception);
				log.Info(i, exception);
				log.Warn(i, exception);
				log.Error(i, exception);
				log.Fatal(i, exception);
			}
		}
	}
}
