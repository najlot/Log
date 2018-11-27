using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using System;
using System.Collections.Generic;

namespace Najlot.Log
{
	/// <summary>
	/// Class for managing instances of loggers and log destinations.
	/// </summary>
	public sealed class LoggerPool : IDisposable
	{
		/// <summary>
		/// Static LoggerPool instance
		/// </summary>
		public static LoggerPool Instance { get; } = new LoggerPool(LogConfiguration.Instance);

		private ILogConfiguration _logConfiguration;
		private List<LogDestinationEntry> _logDestinations = new List<LogDestinationEntry>();
		private Dictionary<string, Logger> _loggerCache = new Dictionary<string, Logger>();
		private bool hasLogdestinationsAdded = false;

		internal LoggerPool(ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
		}

		private string DefaultFormatFunc(LogMessage message)
		{
			string timestamp = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var category = message.Category ?? "";
			string delimiter = " - ";
			string logLevel = message.LogLevel.ToString().ToUpper();

			if (logLevel.Length == 4)
			{
				logLevel += ' ';
			}

			var formatted = string.Concat(timestamp,
				delimiter, logLevel,
				delimiter, category,
				delimiter, message.State,
				delimiter, message.Message);

			return message.ExceptionIsValid ? formatted + message.Exception.ToString() : formatted;
		}

		internal void AddLogDestination<T>(T logDestination) where T : ILogDestination
		{
			var entry = CreateLogDestinationEntry(logDestination);

			_logConfiguration.AttachObserver(entry);

			lock (_logDestinations) _logDestinations.Add(entry);

			hasLogdestinationsAdded = true;
		}

		private LogDestinationEntry CreateLogDestinationEntry<T>(T logDestination) where T : ILogDestination
		{
			var couldGetFormatFunc = this._logConfiguration.TryGetFormatFunctionForType(typeof(T), out var formatFunc);

			if (!couldGetFormatFunc)
			{
				formatFunc = DefaultFormatFunc;
			}

			return new LogDestinationEntry()
			{
				ExecutionMiddleware = this._logConfiguration.ExecutionMiddleware,
				LogDestination = logDestination,
				FormatFunc = formatFunc
			};
		}

		internal IEnumerable<LogDestinationEntry> GetLogDestinations()
		{
			if (!hasLogdestinationsAdded)
			{
				return new List<LogDestinationEntry>()
				{
					CreateLogDestinationEntry(new ConsoleLogDestination(false))
				};
			}

			lock (_logDestinations)
			{
				return _logDestinations;
			}
		}

		/// <summary>
		/// Creates a logger for a type or retrieves it from the cache.
		/// </summary>
		/// <param name="sourceType">Type to create a logger for</param>
		/// <returns></returns>
		public Logger GetLogger(Type sourceType)
		{
			return GetLogger(sourceType.FullName);
		}

		/// <summary>
		/// Creates a logger for a category or retrieves it from the cache.
		/// </summary>
		/// <param name="category">Category to create a logger for</param>
		/// <returns></returns>
		public Logger GetLogger(string category)
		{
			Logger logger;

			lock (_loggerCache)
			{
				if (_loggerCache.TryGetValue(category, out logger))
				{
					return logger;
				}

				lock (_logDestinations)
				{
					if (_logDestinations.Count == 0)
					{
						// There are no log destinations specified: Creating console log destination
						AddLogDestination(new ConsoleLogDestination(false));
					}
				}

				var logExecutor = new LogExecutor(category, this);
				logger = new Logger(logExecutor, _logConfiguration);
				_loggerCache.Add(category, logger);
				_logConfiguration.AttachObserver(logger);
			}

			return logger;
		}

		#region IDisposable Support

		private bool disposedValue = false;

		public void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					foreach (var destination in _logDestinations)
					{
						destination.LogDestination.Dispose();
					}

					foreach (var cachedDestinationEntry in _loggerCache)
					{
						cachedDestinationEntry.Value.Dispose();
					}
				}

				_logConfiguration = null;
				_logDestinations = null;
				_loggerCache = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}