// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log
{
	/// <summary>
	/// Class to help the user to configure destinations, middlewares, log level etc.
	/// </summary>
	public sealed class LogAdministrator : IDisposable
	{
		private LogConfiguration _logConfiguration;
		private LoggerPool _loggerPool;

		private LogAdministrator(LogConfiguration logConfiguration, LoggerPool loggerPool)
		{
			_logConfiguration = logConfiguration;
			_loggerPool = loggerPool;
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
		/// Returns an static registered instance of a LogConfigurator
		/// that has static registered configuration and pool.
		/// </summary>
		/// <returns></returns>
		public static LogAdministrator Instance { get; } = new LogAdministrator(LogConfiguration.Instance, LoggerPool.Instance);

		/// <summary>
		/// Creates a new LogConfigurator that is not static registered and
		/// has own configuration and pool.
		/// </summary>
		/// <returns></returns>
		public static LogAdministrator CreateNew()
		{
			var logConfiguration = new LogConfiguration();
			var loggerPool = new LoggerPool(logConfiguration);

			return new LogAdministrator(logConfiguration, loggerPool);
		}

		/// <summary>
		/// Adds a destination that writes to the console.
		/// All destinations will be used when creating a logger from a LoggerPool.
		/// </summary>
		/// <param name="useColors"></param>
		/// <returns></returns>
		public LogAdministrator AddConsoleDestination(bool useColors = false)
		{
			var destination = new ConsoleDestination(useColors);
			return AddCustomDestination(destination);
		}

		/// <summary>
		/// Adds a destination that puts the requests on an HTTP server
		/// </summary>
		/// <param name="url">Url of the server</param>
		/// <param name="token">Authentication token</param>
		/// <returns></returns>
		public LogAdministrator AddHttpDestination(string url, string token = null)
		{
			var destination = new HttpDestination(url, token);
			return AddCustomDestination(destination);
		}

		/// <summary>
		/// Adds a custom destination.
		/// All destinations will be used when creating a logger from a LoggerPool.
		/// </summary>
		/// <param name="destination">Instance of the new destination</param>
		/// <returns></returns>
		public LogAdministrator AddCustomDestination<TDestination>(TDestination destination) where TDestination : IDestination, new()
		{
			if (destination == null)
			{
				throw new ArgumentNullException(nameof(destination));
			}

			_loggerPool.AddDestination(destination);

			return this;
		}

		/// <summary>
		/// Adds a FileDestination that calculates the path
		/// </summary>
		/// <param name="getFileName">Function to calculate the path</param>
		/// <param name="maxFiles">Max count of files.</param>
		/// <param name="logFilePaths">File where to save the different logfiles to delete them when they are bigger then maxFiles</param>
		/// <param name="keepFileOpen">Should the file be kept open</param>
		/// <returns></returns>
		public LogAdministrator AddFileDestination(Func<string> getFileName, int maxFiles = 30, string logFilePaths = null, bool keepFileOpen = true)
		{
			var destination = new FileDestination(getFileName, maxFiles, logFilePaths, keepFileOpen);
			return AddCustomDestination(destination);
		}

		/// <summary>
		/// Adds a FileDestination
		/// </summary>
		/// <param name="path">Log file path.</param>
		/// <param name="maxFiles">Max count of files.</param>
		/// <param name="logFilePaths">File where to save the different logfiles to delete them when they are bigger then maxFiles</param>
		/// <param name="keepFileOpen">Should the file be kept open</param>
		/// <returns></returns>
		public LogAdministrator AddFileDestination(string path, int maxFiles = 30, string logFilePaths = null, bool keepFileOpen = false)
		{
			var destination = new FileDestination(path, maxFiles, logFilePaths, keepFileOpen);
			return AddCustomDestination(destination);
		}

		public LogAdministrator AddMiddleware<TMiddleware, TDestination>()
					where TMiddleware : IMiddleware
					where TDestination : IDestination
		{
			_logConfiguration.AddMiddleware<TMiddleware, TDestination>();
			return this;
		}

		public LogAdministrator AddMiddleware(string destinationName, string middlewareName)
		{
			_logConfiguration.AddMiddleware(destinationName, middlewareName);
			return this;
		}

		public LogAdministrator SetMiddlewareNames(string destinationName, IEnumerable<string> middlewareNames)
		{
			var middlewaresChanged = !_logConfiguration.GetMiddlewareNames(destinationName)
				.SequenceEqual(middlewareNames);

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

		public LogAdministrator ClearMiddlewares(string destinationName)
		{
			_logConfiguration.ClearMiddlewares(destinationName);
			return this;
		}

		/// <summary>
		/// Flushes to underlying destinations
		/// </summary>
		public void Flush() => _loggerPool.Flush();

		/// <summary>
		/// Retrieves a configuration created by this LogConfigurator.
		/// </summary>
		/// <param name="logConfiguration">ILogConfiguration instance</param>
		/// <returns></returns>
		public LogAdministrator GetLogConfiguration(out ILogConfiguration logConfiguration)
		{
			logConfiguration = _logConfiguration;
			return this;
		}

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

		public LogAdministrator SetCollectMiddleware<TMiddleware, TDestination>()
							where TMiddleware : ICollectMiddleware
			where TDestination : IDestination
		{
			_logConfiguration.SetCollectMiddleware<TMiddleware, TDestination>();
			return this;
		}

		public LogAdministrator SetCollectMiddleware(string destinationName, string collectMiddlewareName)
		{
			_logConfiguration.SetCollectMiddleware(destinationName, collectMiddlewareName);
			return this;
		}

		/// <summary>
		/// Sets the LogLevel of the LogConfiguration.
		/// </summary>
		/// <param name="logLevel"></param>
		/// <returns></returns>
		public LogAdministrator SetLogLevel(LogLevel logLevel)
		{
			_logConfiguration.LogLevel = logLevel;
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

				_loggerPool = null;
				_logConfiguration = null;
			}
		}

		#endregion IDisposable Support
	}
}