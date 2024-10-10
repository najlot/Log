// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Najlot.Log.Destinations;
using Najlot.Log.Extensions.Logging;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests;

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
		using var loggerFactory = new LoggerFactory();
		LogAdministrator logAdministrator = null;

		loggerFactory.AddNajlotLog((administrator) =>
		{
			logAdministrator = administrator.AddConsoleDestination();
		});

		logAdministrator.SetLogLevel(LogLevel.Trace);

		var logger = loggerFactory.CreateLogger("default");

		Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace));

		logAdministrator.SetLogLevel(LogLevel.Debug);
		Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace));
		Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug));

		logAdministrator.SetLogLevel(LogLevel.Info);
		Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug));
		Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information));

		logAdministrator.SetLogLevel(LogLevel.Warn);
		Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information));
		Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning));

		logAdministrator.SetLogLevel(LogLevel.Error);
		Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning));
		Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error));

		logAdministrator.SetLogLevel(LogLevel.Fatal);
		Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error));
		Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical));

		logAdministrator.SetLogLevel(LogLevel.Info);

		logger.LogInformation("Done!");
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
			loggerFactory.AddNajlotLog((admin) =>
			{
				admin
					.SetLogLevel(LogLevel.Trace)
					.AddConsoleDestination(true)
					.SetCollectMiddleware<ConcurrentCollectMiddleware, ConsoleDestination>()
					.AddFileDestination(logFile);
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

		Assert.Contains("Critical logged!", content);
		Assert.Contains("Debug logged!", content);
		Assert.Contains("Error logged!", content);
		Assert.Contains("Info logged!", content);
		Assert.Contains("Trace logged!", content);
		Assert.Contains("Warning logged!", content);
		
		Assert.Contains("My Scope", content);

		Assert.Contains("Critical logged with scope!", content);
		Assert.Contains("Debug logged with scope!", content);
		Assert.Contains("Error logged with scope!", content);
		Assert.Contains("Info logged with scope!", content);
		Assert.Contains("Trace logged with scope!", content);
		Assert.Contains("Warning logged with scope!", content);

		Assert.Contains("Structured Critical logged!", content);
		Assert.Contains("Structured Debug logged!", content);
		Assert.Contains("Structured Error logged!", content);
		Assert.Contains("Structured Info logged!", content);
		Assert.Contains("Structured Trace logged!", content);
		Assert.Contains("Structured Warning logged!", content);

		Assert.Contains("Structured Critical, 0 logged!", content);
		Assert.Contains("Structured Debug, 0 logged!", content);
		Assert.Contains("Structured Error, 0 logged!", content);
		Assert.Contains("Structured Info, 0 logged!", content);
		Assert.Contains("Structured Trace, 0 logged!", content);
		Assert.Contains("Structured Warning, 0 logged!", content);
		
		Assert.Contains("Structured Critical, 0123 logged!", content);
		Assert.Contains("Structured Debug, 0123 logged!", content);
		Assert.Contains("Structured Error, 0123 logged!", content);
		Assert.Contains("Structured Info, 0123 logged!", content);
		Assert.Contains("Structured Trace, 0123 logged!", content);
		Assert.Contains("Structured Warning, 0123 logged!", content);
	}

	[Fact]
	public void LoggerBuilderMustProduceLogger()
	{
		const string logFile = "LoggingBuilderExtension.log";

		if (File.Exists(logFile))
		{
			File.Delete(logFile);
		}

		var services = new ServiceCollection();

		LogAdministrator logAdministrator = null;

		services.AddLogging(loggerBuilder =>
		{
			loggerBuilder.AddNajlotLog((admin) =>
			{
				logAdministrator = admin;
				logAdministrator.AddFileDestination(logFile);
			});
		});

		using (logAdministrator)
		{
			services.AddTransient<DependencyInjectionLoggerService>();

			var serviceProvider = services.BuildServiceProvider();

			var service = serviceProvider.GetService<DependencyInjectionLoggerService>();
			service.GetLogger().LogInformation("Logger created!");
			service.GetLogger().LogTrace("This should not be logged!");
		}

		var content = File.ReadAllText(logFile);

		Assert.Contains("Logger created!", content);
		Assert.DoesNotContain("This should not be logged!", content);
		Assert.Contains(typeof(DependencyInjectionLoggerService).FullName, content);
	}
}