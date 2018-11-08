using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Najlot.Log.Configuration;
using Najlot.Log.Extensions.Logging;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests.Mocks
{
	public class DependencyInjectionLoggerService
	{
		private readonly ILogger<DependencyInjectionLoggerService> _logger;

		public DependencyInjectionLoggerService(ILogger<DependencyInjectionLoggerService> logger)
		{
			_logger = logger;
		}

		public ILogger<DependencyInjectionLoggerService> GetLogger()
		{
			return _logger;
		}
	}
}

namespace Najlot.Log.Tests
{
	public class ExtensionsLoggingTests
	{
		[Fact]
		public void IsEnabledMustReturnCorrectLogLevelEnabled()
		{
			using (var loggerFactory = new LoggerFactory())
			{
				ILogConfiguration logConfiguration = null;

				loggerFactory.AddNajlotLog((configurator) =>
				{
					configurator
						.SetLogLevel(LogLevel.Trace)
						.AddConsoleLogDestination()
						.GetLogConfiguration(out logConfiguration);
				});

				var logger = loggerFactory.CreateLogger("default");

				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace));

				logConfiguration.LogLevel = LogLevel.Debug;
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug));

				logConfiguration.LogLevel = LogLevel.Info;
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information));

				logConfiguration.LogLevel = LogLevel.Warn;
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning));

				logConfiguration.LogLevel = LogLevel.Error;
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error));

				logConfiguration.LogLevel = LogLevel.Fatal;
				Assert.False(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error));
				Assert.True(logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical));
			}
		}

		[Fact]
		public void LoggerFactoryExtensionMustLogToFile()
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

		[Fact]
		public void LoggingBuilderExtensionMustLogCorrect()
		{
			var logFile = "LoggingBuilderExtension.log";
			var services = new ServiceCollection();

			services.AddLogging(loggerBuilder =>
			{
				loggerBuilder.AddNajlotLog((configurator) =>
				{
					configurator
						.SetLogLevel(LogLevel.Trace)
						.SetExecutionMiddleware<SyncExecutionMiddleware>()
						.AddFileLogDestination(logFile);
				});
			});

			services.AddTransient<DependencyInjectionLoggerService>();

			var serviceProvider = services.BuildServiceProvider();

			var service = serviceProvider.GetService<DependencyInjectionLoggerService>();
			service.GetLogger().LogInformation("Logger created!");

			var content = File.ReadAllText(logFile);

			Assert.NotEqual(-1, content.IndexOf("Logger created!"));
			Assert.NotEqual(-1, content.IndexOf(typeof(DependencyInjectionLoggerService).FullName));
		}
	}
}