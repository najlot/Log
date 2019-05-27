using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log
{
	/// <summary>
	/// Class for managing instances of loggers and log destinations.
	/// </summary>
	internal sealed class LoggerPool : IDisposable
	{
		/// <summary>
		/// Static LoggerPool instance
		/// </summary>
		public static LoggerPool Instance { get; } = new LoggerPool(LogConfiguration.Instance);

		private readonly ILogConfiguration _logConfiguration;
		private List<LogDestinationEntry> _logDestinations = new List<LogDestinationEntry>();
		private readonly List<LogDestinationEntry> _pendingLogDestinations = new List<LogDestinationEntry>();
		private readonly Dictionary<string, Logger> _loggerCache = new Dictionary<string, Logger>();
		private bool _hasLogdestinationsAdded = false;
		private bool _hasLogdestinationsPending = false;

		internal LoggerPool(ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
		}

		internal void AddLogDestination<T>(T logDestination) where T : ILogDestination
		{
			var entry = CreateLogDestinationEntry(logDestination);

			_logConfiguration.AttachObserver(entry);

			lock (_pendingLogDestinations) _pendingLogDestinations.Add(entry);

			_hasLogdestinationsAdded = true;
			_hasLogdestinationsPending = true;
		}

		private LogDestinationEntry CreateLogDestinationEntry<T>(T logDestination) where T : ILogDestination
		{
			var mapper = LogConfigurationMapper.Instance;

			var destinationName = mapper.GetName(logDestination.GetType());

			var formatMiddlewareName = _logConfiguration.GetFormatMiddlewareName(destinationName);
			var queueMiddlewareName = _logConfiguration.GetQueueMiddlewareName(destinationName);
			var filterMiddlewareName = _logConfiguration.GetFilterMiddlewareName(destinationName);

			var formatMiddlewareType = mapper.GetType(formatMiddlewareName);
			var queueMiddlewareType = mapper.GetType(queueMiddlewareName);
			var executionMiddlewareType = mapper.GetType(_logConfiguration.ExecutionMiddlewareName);
			var filterMiddlewareType = mapper.GetType(filterMiddlewareName);

			var formatMiddleware = (IFormatMiddleware)Activator.CreateInstance(formatMiddlewareType);
			var queueMiddleware = (IQueueMiddleware)Activator.CreateInstance(queueMiddlewareType);

			queueMiddleware.FormatMiddleware = formatMiddleware;
			queueMiddleware.Destination = logDestination;

			return new LogDestinationEntry()
			{
				ExecutionMiddleware = (IExecutionMiddleware)Activator.CreateInstance(executionMiddlewareType),
				FilterMiddleware = (IFilterMiddleware)Activator.CreateInstance(filterMiddlewareType),
				LogDestination = logDestination,
				LogDestinationName = destinationName,
				FormatMiddleware = formatMiddleware,
				QueueMiddleware = queueMiddleware
			};
		}

		internal IEnumerable<LogDestinationEntry> GetLogDestinations()
		{
			if (!_hasLogdestinationsAdded)
			{
				return new List<LogDestinationEntry>()
				{
					CreateLogDestinationEntry(new ConsoleLogDestination(false))
				};
			}

			if (_hasLogdestinationsPending)
			{
				lock (_pendingLogDestinations)
				{
					if (_hasLogdestinationsPending)
					{
						_logDestinations = new List<LogDestinationEntry>(_logDestinations);

						foreach (var destination in _pendingLogDestinations)
						{
							_logDestinations.Add(destination);
						}

						_pendingLogDestinations.Clear();
						_hasLogdestinationsPending = false;
					}
				}
			}

			return _logDestinations;
		}

		internal void Flush()
		{
			foreach (var destination in GetLogDestinations())
			{
				destination.ExecutionMiddleware.Flush();
				destination.QueueMiddleware.Flush();
			}
		}

		/// <summary>
		/// Creates a logger for a category or retrieves it from the cache.
		/// </summary>
		/// <param name="category">Category to create a logger for</param>
		/// <returns></returns>
		internal Logger GetLogger(string category)
		{
			if (!_loggerCache.TryGetValue(category, out Logger logger))
			{
				lock (_loggerCache)
				{
					if (!_loggerCache.TryGetValue(category, out logger))
					{
						var logExecutor = new LogExecutor(category, this);
						logger = new Logger(logExecutor, _logConfiguration);
						_loggerCache.Add(category, logger);
						_logConfiguration.AttachObserver(logger);
					}
				}
			}

			return logger;
		}

		#region IDisposable Support

		private bool _disposedValue = false;

		public void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				_disposedValue = true;

				if (disposing)
				{
					Flush();

					foreach (var cachedEntry in _loggerCache)
					{
						cachedEntry.Value.Dispose();
					}

					_loggerCache.Clear();

					foreach (var destination in _logDestinations)
					{
						destination.ExecutionMiddleware.Dispose();
						destination.QueueMiddleware.Dispose();
						destination.LogDestination.Dispose();
					}

					_logDestinations.Clear();
					_logDestinations = null;
				}
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