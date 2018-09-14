using Microsoft.Extensions.Logging;
using System;
using Najlot.Log.Extensions.Logging;
using Xunit;
using Najlot.Log.Middleware;
using System.IO;

namespace Najlot.Log.Tests
{
	public class ExtensionsLoggingTests
	{
		[Fact]
		public void LoggerFactoryExtensionShouldLogToFile()
		{
			var logFile = "LoggerFactoryExtension.log";

			using (var loggerFactory = new LoggerFactory())
			{
				loggerFactory.AddNajlotLog((configurator) =>
				{
					configurator
						.SetLogLevel(LogLevel.Trace)
						.SetExecutionMiddleware<SyncExecutionMiddleware>()
						.AddConsoleLogDestination()
						.AddFileLogDestination(logFile);
				});

				var logger = loggerFactory.CreateLogger("default");

				logger.LogCritical("Critical logged!");
				logger.LogDebug("Debug logged!");
				logger.LogError("Error logged!");
				logger.LogInformation("Info logged!");
				logger.LogTrace("Trace logged!");
				logger.LogWarning("Warning logged!");

				using (logger.BeginScope("My Scope"))
				{
					logger.LogCritical("Critical logged with scope!");
					logger.LogDebug("Debug logged with scope!");
					logger.LogError("Error logged with scope!");
					logger.LogInformation("Info logged with scope!");
					logger.LogTrace("Trace logged with scope!");
					logger.LogWarning("Warning logged with scope!");
				}

				var content = File.ReadAllText(logFile);

				Assert.NotEqual(-1, content.IndexOf("Critical logged!"));
				Assert.NotEqual(-1, content.IndexOf("Debug logged!"));
				Assert.NotEqual(-1, content.IndexOf("Error logged!"));
				Assert.NotEqual(-1, content.IndexOf("Info logged!"));
				Assert.NotEqual(-1, content.IndexOf("Trace logged!"));
				Assert.NotEqual(-1, content.IndexOf("Warning logged!"));

				Assert.NotEqual(-1, content.IndexOf("My Scope"));

				Assert.NotEqual(-1, content.IndexOf("Critical logged with scope!"));
				Assert.NotEqual(-1, content.IndexOf("Debug logged with scope!"));
				Assert.NotEqual(-1, content.IndexOf("Error logged with scope!"));
				Assert.NotEqual(-1, content.IndexOf("Info logged with scope!"));
				Assert.NotEqual(-1, content.IndexOf("Trace logged with scope!"));
				Assert.NotEqual(-1, content.IndexOf("Warning logged with scope!"));
			}
		}
	}
}
