using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using System;
using System.Collections.Generic;

namespace Najlot.Log
{
	/// <summary>
	/// Class for managing instances of loggers and log destinations.
	/// </summary>
	public class LoggerPool : IDisposable
	{
		/// <summary>
		/// Static LoggerPool instance
		/// </summary>
		public static LoggerPool Instance { get; } = new LoggerPool(LogConfiguration.Instance);

		private ILogConfiguration _logConfiguration;
		private List<ILogger> _logDestinations = new List<ILogger>();
		private Dictionary<string, Logger> _loggerCache = new Dictionary<string, Logger>();

		internal LoggerPool(ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
		}

		internal void AddLogDestination<T>(T logDestination) where T : LogDestinationPrototype<T>, ILogger
		{
			lock (_logDestinations) _logDestinations.Add(logDestination);
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
					var loggerList = new LoggerList();

					if (_logDestinations.Count == 0)
					{
						// There are no log destinations specified: Creating console log destination
						var consoleDestination = new ConsoleLogDestination(_logConfiguration, false);
						_logConfiguration.AttachObserver(consoleDestination);
						loggerList.Add(consoleDestination);

						logger = new Logger(loggerList, _logConfiguration);
					}
					else
					{
						foreach (var logDestination in _logDestinations)
						{
							var clonedLogDestination = CloneLogDestination(category, logDestination);
							loggerList.Add(clonedLogDestination);
						}

						logger = new Logger(loggerList, _logConfiguration);

						_loggerCache.Add(category, logger);
					}
				}
			}

			return logger;
		}

		private ILogger CloneLogDestination(string category, ILogger LogDestination)
		{
			var logDestinationType = LogDestination.GetType();
			var cloneMethod = logDestinationType.GetMethod("Clone", new Type[] { typeof(string) });
			var clonedlogDestination = cloneMethod.Invoke(LogDestination, new object[] { category });

			_logConfiguration.AttachObserver(clonedlogDestination as IConfigurationChangedObserver);
			return clonedlogDestination as ILogger;
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					foreach (var destination in _logDestinations)
					{
						destination.Dispose();
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