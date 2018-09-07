using Microsoft.Extensions.Logging;
using Najlot.Log.Configuration;
using System;
using System.Threading.Tasks;

namespace Najlot.Log.Extensions.Logging
{
	public static class LoggingBuilderExtension
	{
		public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, Action<LogConfigurator> configure)
		{
			var configurator = LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.GetLoggerPool(out LoggerPool loggerPool);

			configure(configurator);
			
			return builder.AddNajlotLog(loggerPool, logConfiguration);
		}

		public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, LoggerPool loggerPool, ILogConfiguration logConfiguration)
		{
			builder.AddProvider(new NajlotLogProvider(loggerPool, logConfiguration));
			return builder;
		}
	}

	public static class LoggerFactoryExtension
	{
		public static ILoggerFactory AddNajlotLog(this ILoggerFactory builder, Action<LogConfigurator> configure)
		{
			var configurator = LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.GetLoggerPool(out LoggerPool loggerPool);

			configure(configurator);

			return builder.AddNajlotLog(loggerPool, logConfiguration);
		}

		public static ILoggerFactory AddNajlotLog(this ILoggerFactory loggerFactory, LoggerPool loggerPool, ILogConfiguration logConfiguration)
		{
			loggerFactory.AddProvider(new NajlotLogProvider(loggerPool, logConfiguration));
			return loggerFactory;
		}
    }
}
