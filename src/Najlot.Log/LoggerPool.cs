﻿// Licensed under the MIT License.
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
		private List<LogDestinationEntry> _logDestinations = new List<LogDestinationEntry>();
		private readonly List<LogDestinationEntry> _pendingLogDestinations = new List<LogDestinationEntry>();
		private readonly Dictionary<string, Logger> _loggerCache = new Dictionary<string, Logger>();
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

			_hasLogdestinationsPending = true;
		}

		private LogDestinationEntry CreateLogDestinationEntry<T>(T logDestination) where T : ILogDestination
		{
			var mapper = LogConfigurationMapper.Instance;

			var destinationName = mapper.GetName(logDestination);

			var entry = new LogDestinationEntry()
			{
				LogDestination = logDestination,
				LogDestinationName = destinationName,
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

		internal IEnumerable<LogDestinationEntry> GetLogDestinations()
		{
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
			foreach (var entry in GetLogDestinations())
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

					foreach (var destination in GetLogDestinations())
					{
						destination.Dispose();
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