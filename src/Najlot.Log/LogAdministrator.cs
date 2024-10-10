// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log;

/// <summary>
/// Class to configure destinations, middlewares, log level etc.
/// </summary>
public sealed class LogAdministrator : IDisposable
{
	private readonly LoggerPool _loggerPool;
	private readonly Dictionary<string, List<string>> _middlewares = [];
	private readonly Dictionary<string, string> _collectMiddlewares = [];
	private readonly List<IMiddlewareConfigurationObserver> _observerList = [];
	private readonly List<ILogLevelObserver> _loglevelObserverList = [];

	private volatile LogLevel _logLevel = LogLevel.Debug;

	/// <summary>
	/// Returns an static registered instance of a LogConfigurator
	/// that has static registered configuration and pool.
	/// </summary>
	/// <returns></returns>
	public static LogAdministrator Instance { get; } = new LogAdministrator();

	/// <summary>
	/// Creates a new LogConfigurator that is not static registered and
	/// has own configuration and pool.
	/// </summary>
	/// <returns></returns>
	public static LogAdministrator CreateNew()
	{
		return new LogAdministrator();
	}

	private LogAdministrator()
	{
		_loggerPool = new LoggerPool(this);
	}

	#region CollectMiddleware

	public LogAdministrator SetCollectMiddleware(string destinationName, string middlewareName)
	{
		if (_collectMiddlewares.TryGetValue(destinationName, out var currentMiddlewareName)
			&& currentMiddlewareName == middlewareName)
		{
			return this;
		}

		_collectMiddlewares[destinationName] = middlewareName;

		lock (_observerList)
		{
			foreach (var observer in _observerList)
			{
				try
				{
					observer.NotifyCollectMiddlewareChanged(destinationName, middlewareName);
				}
				catch (Exception ex)
				{
					LogErrorHandler.Instance.Handle("An error while notifying occured.", ex);
				}
			}
		}

		return this;
	}

	public LogAdministrator GetCollectMiddlewareName(string destinationName, out string collectMiddlewareName)
	{
		if (_collectMiddlewares.TryGetValue(destinationName, out var entry))
		{
			collectMiddlewareName = entry;
			return this;
		}

		collectMiddlewareName = LogConfigurationMapper.Instance.GetName<SyncCollectMiddleware>();

		return this;
	}

	#endregion CollectMiddleware

	public LogAdministrator AddMiddleware(string destinationName, string middlewareName)
	{
		if (destinationName == null) throw new ArgumentNullException(nameof(destinationName));
		if (middlewareName == null) throw new ArgumentNullException(nameof(middlewareName));

		if (_middlewares.TryGetValue(destinationName, out var list))
		{
			list.Add(middlewareName);
		}
		else
		{
			var newList = new List<string>
			{
				middlewareName
			};

			_middlewares.Add(destinationName, newList);
		}

		lock (_observerList)
		{
			foreach (var observer in _observerList)
			{
				try
				{
					observer.NotifyMiddlewareAdded(destinationName, middlewareName);
				}
				catch (Exception ex)
				{
					LogErrorHandler.Instance.Handle("An error while notifying occured.", ex);
				}
			}
		}

		return this;
	}

	public LogAdministrator GetMiddlewareNames(string destinationName, out IEnumerable<string> middlewareNames)
	{
		if (_middlewares.TryGetValue(destinationName, out var list))
		{
			middlewareNames = list;
			return this;
		}

		middlewareNames = Array.Empty<string>();
		return this;
	}

	#region Configuration observers

	internal void AttachObserver(IMiddlewareConfigurationObserver observer)
	{
		lock (_observerList)
		{
			_observerList.Add(observer);
		}
	}

	internal void AttachObserver(ILogLevelObserver observer)
	{
		lock (_loglevelObserverList)
		{
			_loglevelObserverList.Add(observer);
		}
	}

	internal void DetachObserver(IMiddlewareConfigurationObserver observer)
	{
		lock (_observerList)
		{
			while (_observerList.Remove(observer))
			{
				// Remove returns true, if it could remove.
				// -> Remove all
			}
		}
	}

	internal void DetachObserver(ILogLevelObserver observer)
	{
		lock (_loglevelObserverList)
		{
			while (_loglevelObserverList.Remove(observer))
			{
				// Remove returns true, if it could remove.
				// -> Remove all
			}
		}
	}

	#endregion Configuration observers

	public void ClearMiddlewares(string destinationName)
	{
		if (_middlewares.TryGetValue(destinationName, out var list))
		{
			list.Clear();
		}

		lock (_observerList)
		{
			foreach (var observer in _observerList)
			{
				try
				{
					observer.NotifyClearMiddlewares(destinationName);
				}
				catch (Exception ex)
				{
					LogErrorHandler.Instance.Handle("An error while notifying occured.", ex);
				}
			}
		}
	}

	public LogAdministrator GetDestinationConfiguration(string destinationName, out IDictionary<string, string> configuration)
	{
		configuration = LogDestinationConfigurator.GetDestinationConfiguration(_loggerPool, destinationName);
		return this;
	}

	public LogAdministrator SetDestinationConfiguration(string destinationName, IDictionary<string, string> configuration)
	{
		LogDestinationConfigurator.SetDestinationConfiguration(_loggerPool, destinationName, configuration);
		return this;
	}

	public LogAdministrator GetDestinationNames(out IEnumerable<string> destinationNames)
	{
		destinationNames = _loggerPool.GetDestinations().Select(d => d.DestinationName);
		return this;
	}

	/// <summary>
	/// Adds a custom destination.
	/// All destinations will be used when creating a logger from a LoggerPool.
	/// </summary>
	/// <param name="destination">Instance of the new destination</param>
	/// <returns></returns>
	public LogAdministrator AddCustomDestination<TDestination>(TDestination destination) where TDestination : IDestination, new()
	{
		if (destination == null) throw new ArgumentNullException(nameof(destination));
		
		_loggerPool.AddDestination(destination);

		return this;
	}

	public LogAdministrator SetMiddlewareNames(string destinationName, IEnumerable<string> middlewareNames)
	{
		GetMiddlewareNames(destinationName, out var prevMiddlewareNames);
		var middlewaresChanged = !prevMiddlewareNames.SequenceEqual(middlewareNames);

		if (!middlewaresChanged)
		{
			return this;
		}
		
		ClearMiddlewares(destinationName);

		foreach (var middlewareName in middlewareNames)
		{
			AddMiddleware(destinationName, middlewareName);
		}

		return this;
	}

	/// <summary>
	/// Flushes to underlying destinations
	/// </summary>
	public void Flush() => _loggerPool.Flush();

	/// <summary>
	/// Creates a logger for a type or retrieves it from the cache.
	/// </summary>
	/// <param name="sourceType">Type to create a logger for</param>
	/// <returns></returns>
	public Logger GetLogger(Type sourceType) => _loggerPool.GetLogger(sourceType?.FullName);

	/// <summary>
	/// Creates a logger for a category or retrieves it from the cache.
	/// </summary>
	/// <param name="category">Category to create a logger for</param>
	/// <returns></returns>
	public Logger GetLogger(string category) => _loggerPool.GetLogger(category);

	public LogAdministrator GetLogLevel(out LogLevel logLevel)
	{
		logLevel = _logLevel;
		return this;
	}

	/// <summary>
	/// Sets the LogLevel
	/// </summary>
	/// <param name="logLevel"></param>
	/// <returns></returns>
	public LogAdministrator SetLogLevel(LogLevel logLevel)
	{
		if (_logLevel == logLevel)
		{
			return this;
		}

		_logLevel = logLevel;

		lock (_loglevelObserverList)
		{
			foreach (var observer in _loglevelObserverList)
			{
				try
				{
					observer.NotifyLogLevelChanged(logLevel);
				}
				catch (Exception ex)
				{
					LogErrorHandler.Instance.Handle("An error while notifying occured.", ex);
				}
			}
		}

		return this;
	}

	#region IDisposable Support

	private bool _disposedValue = false; // To detect redundant calls

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			_disposedValue = true;

			if (disposing)
			{
				_loggerPool.Dispose();
			}
		}
	}

	#endregion IDisposable Support
}