using NajlotLog.Configuration;
using NajlotLog.Implementation;
using System;

namespace NajlotLog
{
	public class LogConfigurator
	{
		private ILogConfiguration _logConfiguration;
		private LoggerPool _loggerPool;
		public static LogConfigurator Instance { get; } = new LogConfigurator(LogConfiguration.Instance, LoggerPool.Instance);

		public static LogConfigurator CreateNew()
		{
			var logConfiguration = new LogConfiguration();
			var loggerPool = new LoggerPool(logConfiguration);

			return new LogConfigurator(logConfiguration, loggerPool);
		}

		public LogConfigurator GetLogConfiguration(out ILogConfiguration logConfiguration)
		{
			logConfiguration = _logConfiguration;
			return this;
		}

		public LogConfigurator GetLoggerPool(out LoggerPool loggerPool)
		{
			loggerPool = _loggerPool;
			return this;
		}

		internal LogConfigurator(ILogConfiguration logConfiguration, LoggerPool loggerPool)
		{
			_logConfiguration = logConfiguration;
			_loggerPool = loggerPool;
		}

		public LogConfigurator SetLogLevel(LogLevel logLevel)
		{
			_logConfiguration.LogLevel = logLevel;
			return this;
		}

		public LogConfigurator SetLogExecutionMiddleware(Middleware.ILogExecutionMiddleware middleware)
		{
			_logConfiguration.LogExecutionMiddleware = middleware;
			return this;
		}

		public LogConfigurator AddCustomAppender(LoggerImplementationBase appender, Func<LogMessage, string> formatFunction = null)
		{
			if (appender == null)
			{
				throw new ArgumentNullException(nameof(appender));
			}
			
			if(formatFunction != null)
			{
				if(!_logConfiguration.TrySetFormatFunctionForType(appender.GetType(), formatFunction))
				{
					Console.WriteLine("NajlotLog: Could not set format function for " + appender.GetType());
				}
			}

			_loggerPool.AddAppender(appender);
			return this;
		}

		public LogConfigurator AddConsoleAppender(Func<LogMessage, string> formatFunction = null)
		{
			var appender = new ConsoleLoggerImplementation(_logConfiguration);
			return AddCustomAppender(appender, formatFunction);
		}

		public LogConfigurator AddFileAppender(string fileName, Func<LogMessage, string> formatFunction = null)
		{
			var appender = new FileLoggerImplementation(_logConfiguration, fileName);
			return AddCustomAppender(appender, formatFunction);
		}
	}
}
