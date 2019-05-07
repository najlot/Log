using Microsoft.Extensions.Logging;
using System;

namespace Najlot.Log.Extensions.Logging
{
	public static class LoggingBuilderExtension
	{
		public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, Action<LogAdminitrator> configure)
		{
			var admin = LogAdminitrator.CreateNew();
			configure(admin);
			return builder.AddNajlotLog(admin);
		}

		public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, LogAdminitrator logConfigurator)
		{
			builder.AddProvider(new NajlotLogProvider(logConfigurator));
			return builder;
		}
	}

	public static class LoggerFactoryExtension
	{
		public static ILoggerFactory AddNajlotLog(this ILoggerFactory loggerFactory, Action<LogAdminitrator> configure)
		{
			var admin = LogAdminitrator.CreateNew();
			configure(admin);
			return loggerFactory.AddNajlotLog(admin);
		}

		public static ILoggerFactory AddNajlotLog(this ILoggerFactory loggerFactory, LogAdminitrator logAdministrator)
		{
			loggerFactory.AddProvider(new NajlotLogProvider(logAdministrator));
			return loggerFactory;
		}
	}
}