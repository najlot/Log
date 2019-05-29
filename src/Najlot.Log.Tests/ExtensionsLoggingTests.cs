﻿// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Najlot.Log.Extensions.Logging;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ExtensionsLoggingTests
	{
		public ExtensionsLoggingTests()
		{
			foreach (var type in typeof(ExtensionsLoggingTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void IsEnabledMustReturnCorrectLogLevelEnabled()
		{
			using (var loggerFactory = new LoggerFactory())
			{
				LogAdminitrator logAdminitrator = null;

				loggerFactory.AddNajlotLog((adminitrator) =>
				{
					logAdminitrator = adminitrator
						.SetLogLevel(LogLevel.Trace)
						.AddConsoleLogDestination();
				});

				var logger = loggerFactory.CreateLogger("default");

				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace));

				logAdminitrator.SetLogLevel(LogLevel.Debug);
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug));

				logAdminitrator.SetLogLevel(LogLevel.Info);
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information));

				logAdminitrator.SetLogLevel(LogLevel.Warn);
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning));

				logAdminitrator.SetLogLevel(LogLevel.Error);
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error));

				logAdminitrator.SetLogLevel(LogLevel.Fatal);
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical));
			}
		}

		[Fact]
		public void LoggerFactoryExtensionMustLogToFile()
		{
			var logFile = "LoggerFactoryExtension.log";

			if (File.Exists(logFile))
			{
				File.Delete(logFile);
			}

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

				logger.LogTrace("{Kind} logged!", "Structured Trace");
				logger.LogDebug("{Kind} logged!", "Structured Debug");
				logger.LogInformation("{Kind} logged!", "Structured Info");
				logger.LogWarning("{Kind} logged!", "Structured Warning");
				logger.LogError("{Kind} logged!", "Structured Error");
				logger.LogCritical("{Kind} logged!", "Structured Critical");

				logger.LogTrace("{0}, {1} logged!", "Structured Trace", 0);
				logger.LogDebug("{0}, {1} logged!", "Structured Debug", 0);
				logger.LogInformation("{0}, {1} logged!", "Structured Info", 0);
				logger.LogWarning("{0}, {1} logged!", "Structured Warning", 0);
				logger.LogError("{0}, {1} logged!", "Structured Error", 0);
				logger.LogCritical("{0}, {1} logged!", "Structured Critical", 0);

				logger.LogTrace("{0}, {1:D4} logged!", "Structured Trace", 123);
				logger.LogDebug("{0}, {1:D4} logged!", "Structured Debug", 123);
				logger.LogInformation("{0}, {1:D4} logged!", "Structured Info", 123);
				logger.LogWarning("{0}, {1:D4} logged!", "Structured Warning", 123);
				logger.LogError("{0}, {1:D4} logged!", "Structured Error", 123);
				logger.LogCritical("{0}, {1:D4} logged!", "Structured Critical", 123);
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

			Assert.NotEqual(-1, content.IndexOf("Structured Critical logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Debug logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Error logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Info logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Trace logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Warning logged!"));

			Assert.NotEqual(-1, content.IndexOf("Structured Critical, 0 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Debug, 0 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Error, 0 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Info, 0 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Trace, 0 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Warning, 0 logged!"));

			Assert.NotEqual(-1, content.IndexOf("Structured Critical, 0123 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Debug, 0123 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Error, 0123 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Info, 0123 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Trace, 0123 logged!"));
			Assert.NotEqual(-1, content.IndexOf("Structured Warning, 0123 logged!"));
		}

		[Fact]
		public void LoggingBuilderExtensionMustLogCorrect()
		{
			var logFile = "LoggingBuilderExtension.log";

			if (File.Exists(logFile))
			{
				File.Delete(logFile);
			}

			var services = new ServiceCollection();

			LogAdminitrator logAdminitrator = null;

			services.AddLogging(loggerBuilder =>
			{
				loggerBuilder.AddNajlotLog((configurator) =>
				{
					logAdminitrator = configurator;

					logAdminitrator
						.SetLogLevel(LogLevel.Debug)
						.SetExecutionMiddleware<SyncExecutionMiddleware>()
						.AddFileLogDestination(logFile);
				});
			});

			services.AddTransient<DependencyInjectionLoggerService>();

			var serviceProvider = services.BuildServiceProvider();

			var service = serviceProvider.GetService<DependencyInjectionLoggerService>();
			service.GetLogger().LogInformation("Logger created!");
			service.GetLogger().LogTrace("This should not be logged!");

			logAdminitrator.Dispose();

			var content = File.ReadAllText(logFile);

			Assert.NotEqual(-1, content.IndexOf("Logger created!"));
			Assert.Equal(-1, content.IndexOf("This should not be logged!"));
			Assert.NotEqual(-1, content.IndexOf(typeof(DependencyInjectionLoggerService).FullName));
		}
	}
}