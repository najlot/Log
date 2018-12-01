using Microsoft.Extensions.Logging;
using Najlot.Log.Configuration;
using System;

namespace Najlot.Log.Extensions.Logging
{
	public static class LoggingBuilderExtension
	{
		public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, Action<LogAdminitrator> configure)
		{
			var configurator = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.GetLoggerPool(out LoggerPool loggerPool);

			configure(configurator);

			return builder.AddNajlotLog(configurator);
		}

		public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, LogAdminitrator logConfigurator)
		{
			builder.AddProvider(new NajlotLogProvider(logConfigurator));
			return builder;
		}
	}

	public static class LoggerFactoryExtension
	{
		public static ILoggerFactory AddNajlotLog(this ILoggerFactory builder, Action<LogAdminitrator> configure)
		{
			var configurator = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.GetLoggerPool(out LoggerPool loggerPool);

			configure(configurator);

			return builder.AddNajlotLog(configurator);
		}

		public static ILoggerFactory AddNajlotLog(this ILoggerFactory loggerFactory, LogAdminitrator logConfigurator)
		{
			loggerFactory.AddProvider(new NajlotLogProvider(logConfigurator));
			return loggerFactory;
		}
	}
}