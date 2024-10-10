// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log;

/// <summary>
/// Class for managing instances of loggers and log destinations.
/// </summary>
internal sealed class LoggerPool : ILogLevelObserver, IDisposable
{
	private readonly LogAdministrator _logAdministrator;
	private List<DestinationEntry> _destinations = new List<DestinationEntry>();
	private readonly List<DestinationEntry> _pendingDestinations = new List<DestinationEntry>();
	private readonly Dictionary<string, Logger> _loggerCache = new Dictionary<string, Logger>();
	private bool _hasDestinationsPending = false;

	internal LoggerPool(LogAdministrator logAdministrator)
	{
		_logAdministrator = logAdministrator;
		_logAdministrator.AttachObserver(this);
	}

	internal void AddDestination<T>(T destination) where T : IDestination, new()
	{
		var entry = CreateDestinationEntry(destination);

		_logAdministrator.AttachObserver(entry);

		lock (_pendingDestinations) _pendingDestinations.Add(entry);

		_hasDestinationsPending = true;
	}

	private DestinationEntry CreateDestinationEntry<T>(T destination) where T : IDestination
	{
		var destinationName = LogConfigurationMapper.Instance.GetName(destination);

		_logAdministrator
			.GetCollectMiddlewareName(destinationName, out var collectMiddlewareName)
			.GetMiddlewareNames(destinationName, out var middlewareNames);

		return new DestinationEntry(
			destination,
			destinationName,
			collectMiddlewareName,
			middlewareNames);
	}

	internal IEnumerable<DestinationEntry> GetDestinations()
	{
		if (_hasDestinationsPending)
		{
			lock (_pendingDestinations)
			{
				if (_hasDestinationsPending)
				{
					_destinations = new List<DestinationEntry>(_destinations);

					foreach (var destination in _pendingDestinations)
					{
						_destinations.Add(destination);
					}

					_pendingDestinations.Clear();
					_hasDestinationsPending = false;
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

			var middleware = entry.CollectMiddleware.NextMiddleware;

			while (middleware != null)
			{
				middleware.Flush();
				middleware = middleware.NextMiddleware;
			}

			entry.Destination.Flush();
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
			if (_loggerCache.TryGetValue(category, out logger))
			{
				return logger;
			}

			_logAdministrator.GetLogLevel(out var logLevel);

			var logExecutor = new LogExecutor(category, this);
			logger = new Logger(logExecutor);
			logger.SetupLogLevel(logLevel);
			_loggerCache.Add(category, logger);
		}

		return logger;
	}
	
	public void NotifyLogLevelChanged(LogLevel logLevel)
	{
		foreach (var entry in _loggerCache.Values.ToArray())
		{
			entry.SetupLogLevel(logLevel);
		}
	}
	
	#region IDisposable Support

	private bool _disposedValue = false;

	private void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			_disposedValue = true;

			if (disposing)
			{
				Flush();
				_logAdministrator.DetachObserver(this);
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