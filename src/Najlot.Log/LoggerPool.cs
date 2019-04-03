﻿using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Threading;

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

		private ILogConfiguration _logConfiguration;
		private List<LogDestinationEntry> _logDestinations = new List<LogDestinationEntry>();
		private readonly List<LogDestinationEntry> _pendingLogDestinations = new List<LogDestinationEntry>();
		private ThreadLocal<Dictionary<string, Logger>> _loggerCache = new ThreadLocal<Dictionary<string, Logger>>(() => new Dictionary<string, Logger>(), true);
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
			var couldGetFormatFunc = this._logConfiguration.TryGetFormatFunctionForType(typeof(T), out var formatFunc);

			if (!couldGetFormatFunc)
			{
				formatFunc = DefaultFormatFuncHolder.DefaultFormatFunc;
			}

			return new LogDestinationEntry()
			{
				ExecutionMiddleware = (IExecutionMiddleware)Activator.CreateInstance(_logConfiguration.ExecutionMiddlewareType),
				FilterMiddleware = (IFilterMiddleware)Activator.CreateInstance(_logConfiguration.FilterMiddlewareType),
				LogDestination = logDestination,
				LogDestinationType = logDestination.GetType(),
				FormatFunc = formatFunc
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
			}
		}

		/// <summary>
		/// Creates a logger for a category or retrieves it from the cache.
		/// </summary>
		/// <param name="category">Category to create a logger for</param>
		/// <returns></returns>
		internal Logger GetLogger(string category)
		{
			var loggerCache = _loggerCache.Value;

			if (!loggerCache.TryGetValue(category, out Logger logger))
			{
				var logExecutor = new LogExecutor(category, this);
				logger = new Logger(logExecutor, _logConfiguration);
				loggerCache.Add(category, logger);
				_logConfiguration.AttachObserver(logger);
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

					foreach (var dictionary in _loggerCache.Values)
					{
						foreach (var cachedEntry in dictionary)
						{
							cachedEntry.Value.Dispose();
						}

						dictionary.Clear();
					}

					_loggerCache.Dispose();

					foreach (var destination in _logDestinations)
					{
						destination.ExecutionMiddleware.Dispose();
						destination.LogDestination.Dispose();
					}

					_logConfiguration = null;
					_logDestinations = null;
					_loggerCache = null;
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