// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

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
		private List<DestinationEntry> _destinations = new List<DestinationEntry>();
		private readonly List<DestinationEntry> _pendingDestinations = new List<DestinationEntry>();
		private readonly Dictionary<string, Logger> _loggerCache = new Dictionary<string, Logger>();
		private bool _hasLogdestinationsPending = false;

		internal LoggerPool(ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
		}

		internal void AddDestination<T>(T destination) where T : IDestination
		{
			var entry = CreateDestinationEntry(destination);

			_logConfiguration.AttachObserver(entry);

			lock (_pendingDestinations) _pendingDestinations.Add(entry);

			_hasLogdestinationsPending = true;
		}

		private DestinationEntry CreateDestinationEntry<T>(T destination) where T : IDestination
		{
			var mapper = LogConfigurationMapper.Instance;

			var destinationName = mapper.GetName(destination);

			var entry = new DestinationEntry()
			{
				Destination = destination,
				DestinationName = destinationName,
			};

			var collectMiddlewareName = _logConfiguration.GetCollectMiddlewareName(destinationName);
			var middlewareNames = _logConfiguration.GetMiddlewareNames(destinationName);

			entry.NotifyCollectMiddlewareChanged(destinationName, collectMiddlewareName);

			foreach (var name in middlewareNames)
			{
				entry.NotifyMiddlewareAdded(destinationName, name);
			}

			return entry;
		}

		internal IEnumerable<DestinationEntry> GetDestinations()
		{
			if (_hasLogdestinationsPending)
			{
				lock (_pendingDestinations)
				{
					if (_hasLogdestinationsPending)
					{
						_destinations = new List<DestinationEntry>(_destinations);

						foreach (var destination in _pendingDestinations)
						{
							_destinations.Add(destination);
						}

						_pendingDestinations.Clear();
						_hasLogdestinationsPending = false;
					}
				}
			}

			return _destinations;
		}

		internal void Flush()
		{
			foreach (var entry in GetDestinations())
			{
				entry.CollectMiddleware.Flush();

				IMiddleware middleware = entry.CollectMiddleware.NextMiddleware;

				while (middleware != null)
				{
					middleware.Flush();
					middleware = middleware.NextMiddleware;
				}
			}
		}

		/// <summary>
		/// Creates a logger for a category or retrieves it from the cache.
		/// </summary>
		/// <param name="category">Category to create a logger for</param>
		/// <returns></returns>
		internal Logger GetLogger(string category)
		{
			Logger logger;

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

					foreach (var destination in GetDestinations())
					{
						destination.Dispose();
					}

					_destinations.Clear();
					_destinations = null;
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