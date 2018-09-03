﻿using NajlotLog.Configuration;
using NajlotLog.Destinations;
using System;

namespace NajlotLog
{
	/// <summary>
	/// Class to help the user to cunfigure his log destinations, execution middleware, log level etc.
	/// </summary>
	public class LogConfigurator
	{
		private ILogConfiguration _logConfiguration;
		private LoggerPool _loggerPool;
		public static LogConfigurator Instance { get; } = new LogConfigurator(LogConfiguration.Instance, LoggerPool.Instance);

		internal LogConfigurator(ILogConfiguration logConfiguration, LoggerPool loggerPool)
		{
			_logConfiguration = logConfiguration;
			_loggerPool = loggerPool;
		}

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
		
		public LogConfigurator SetLogLevel(LogLevel logLevel)
		{
			_logConfiguration.LogLevel = logLevel;
			return this;
		}

		public LogConfigurator SetExecutionMiddleware(Middleware.IExecutionMiddleware middleware)
		{
			_logConfiguration.ExecutionMiddleware = middleware;
			return this;
		}

		public LogConfigurator AddCustomDestination(LogDestinationBase logDestination, Func<LogMessage, string> formatFunction = null)
		{
			if (logDestination == null)
			{
				throw new ArgumentNullException(nameof(logDestination));
			}
			
			if(formatFunction != null)
			{
				var logDestinationType = logDestination.GetType();
				if (!_logConfiguration.TrySetFormatFunctionForType(logDestinationType, formatFunction))
				{
					Console.WriteLine("NajlotLog: Could not set format function for " + logDestinationType.Name);
				}
			}

			_loggerPool.AddLogDestination(logDestination);
			return this;
		}

		public LogConfigurator AddConsoleLogDestination(Func<LogMessage, string> formatFunction = null)
		{
			var logDestination = new ConsoleLogDestination(_logConfiguration);
			return AddCustomDestination(logDestination, formatFunction);
		}

		public LogConfigurator AddFileLogDestination(string fileName, Func<LogMessage, string> formatFunction = null)
		{
			var logDestination = new FileLogDestination(_logConfiguration, fileName);
			return AddCustomDestination(logDestination, formatFunction);
		}
	}
}
