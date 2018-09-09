using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using System;

namespace Najlot.Log
{
	/// <summary>
	/// Class to help the user to configure log destinations, execution middleware, log level etc.
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

		public LogConfigurator SetExecutionMiddleware<TExecutionMiddleware>() where TExecutionMiddleware: Middleware.IExecutionMiddleware, new()
		{
			_logConfiguration.ExecutionMiddleware = new TExecutionMiddleware();
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
					Console.WriteLine("Najlot.Log: Could not set format function for " + logDestinationType.Name);
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

		/// <summary>
		/// Adds a FileLogDestination that calculates the path
		/// </summary>
		/// <param name="getFileName">Function to calculate the path</param>
		/// <param name="formatFunction">Function to customize the output</param>
		/// <returns></returns>
		public LogConfigurator AddFileLogDestination(Func<string> getFileName, Func<LogMessage, string> formatFunction = null)
		{
			var logDestination = new FileLogDestination(_logConfiguration, getFileName);
			return AddCustomDestination(logDestination, formatFunction);
		}

		/// <summary>
		/// Adds a FileLogDestination that uses a constant path
		/// </summary>
		/// <param name="fileName">Path to the file</param>
		/// <param name="formatFunction">Function to customize the output</param>
		/// <returns></returns>
		public LogConfigurator AddFileLogDestination(string fileName, Func<LogMessage, string> formatFunction = null)
		{
			return AddFileLogDestination(() => fileName, formatFunction);
		}
	}
}
