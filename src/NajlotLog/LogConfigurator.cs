using NajlotLog.Configuration;
using NajlotLog.Implementation;
using System;

namespace NajlotLog
{
	public class LogConfigurator
	{
		public static LogConfigurator Instance { get; } = new LogConfigurator();

		public LogConfigurator SetLogLevel(LogLevel logLevel)
		{
			LogConfiguration.Instance.LogLevel = logLevel;
			return this;
		}

		public LogConfigurator SetLogExecutionMiddleware(Middleware.ILogExecutionMiddleware middleware)
		{
			LogConfiguration.Instance.LogExecutionMiddleware = middleware;
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
				if(!LogConfiguration.Instance.TrySetFormatFunctionForType(appender.GetType(), formatFunction))
				{
					Console.Error.WriteLine("Could not set format function for " + appender.GetType());
				}
			}
			
			LoggerPool.Instance.AddAppender(appender);
			return this;
		}

		public LogConfigurator AddConsoleAppender(Func<LogMessage, string> formatFunction = null)
		{
			var appender = new ConsoleLoggerImplementation();
			return AddCustomAppender(appender, formatFunction);
		}

		public LogConfigurator AddFileAppender(string fileName, Func<LogMessage, string> formatFunction = null)
		{
			var appender = new FileLoggerImplementation(fileName);
			return AddCustomAppender(appender, formatFunction);
		}
	}
}
