using Microsoft.Extensions.Logging;
using Najlot.Log.Configuration;
using System;

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

			return builder.AddNajlotLog(configurator);
		}

		public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, LogConfigurator logConfigurator)
		{
			builder.AddProvider(new NajlotLogProvider(logConfigurator));
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

			return builder.AddNajlotLog(configurator);
		}

		public static ILoggerFactory AddNajlotLog(this ILoggerFactory loggerFactory, LogConfigurator logConfigurator)
		{
			loggerFactory.AddProvider(new NajlotLogProvider(logConfigurator));
			return loggerFactory;
		}
	}
}