using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log
{
	/// <summary>
	/// Class to help the user to configure log destinations, execution middleware, log level etc.
	/// </summary>
	public class LogAdminitrator : IDisposable
	{
		private LogConfiguration _logConfiguration;
		private LoggerPool _loggerPool;

		/// <summary>
		/// Returns an static registered instance of a LogConfigurator
		/// that has static resistered configuration and pool.
		/// </summary>
		/// <returns></returns>
		public static LogAdminitrator Instance { get; } = new LogAdminitrator(LogConfiguration.Instance, LoggerPool.Instance);

		internal LogAdminitrator(LogConfiguration logConfiguration, LoggerPool loggerPool)
		{
			_logConfiguration = logConfiguration;
			_loggerPool = loggerPool;
		}

		/// <summary>
		/// Creates a new LogConfigurator that is not static registered and
		/// has own configuration and pool.
		/// </summary>
		/// <returns></returns>
		public static LogAdminitrator CreateNew()
		{
			var logConfiguration = new LogConfiguration();
			var loggerPool = new LoggerPool(logConfiguration);

			return new LogAdminitrator(logConfiguration, loggerPool);
		}

		/// <summary>
		/// Retrieves a configuration created by this LogConfigurator.
		/// </summary>
		/// <param name="logConfiguration">ILogConfiguration instance</param>
		/// <returns></returns>
		public LogAdminitrator GetLogConfiguration(out ILogConfiguration logConfiguration)
		{
			logConfiguration = _logConfiguration;
			return this;
		}

		/// <summary>
		/// Sets the LogLevel of the LogConfiguration.
		/// </summary>
		/// <param name="logLevel"></param>
		/// <returns></returns>
		public LogAdminitrator SetLogLevel(LogLevel logLevel)
		{
			_logConfiguration.LogLevel = logLevel;
			return this;
		}

		/// <summary>
		/// Sets the type of the execution middleware and notifies observing components
		/// </summary>
		/// <typeparam name="TExecutionMiddleware"></typeparam>
		/// <returns></returns>
		public LogAdminitrator SetExecutionMiddleware<TExecutionMiddleware>() where TExecutionMiddleware : IExecutionMiddleware, new()
		{
			var name = LogConfigurationMapper.Instance.GetName<TExecutionMiddleware>();
			return this.SetExecutionMiddleware(name);
		}

		/// <summary>
		/// Sets the type of the execution middleware and notifies observing components
		/// </summary>
		/// <param name="middlewareType">Type of the execution middleware</param>
		/// <returns></returns>
		public LogAdminitrator SetExecutionMiddleware(string middlewareName)
		{
			if (middlewareName == null)
			{
				Console.WriteLine("Najlot.Log: New execution middleware name is null.");
				return this;
			}

			this.Flush();
			_logConfiguration.ExecutionMiddlewareName = middlewareName;
			return this;
		}

		#region Format middleware
		/// <summary>
		/// Sets the name of the format middleware and notifies observing components
		/// </summary>
		/// <nameparam name="TMiddleware">Name of the format middleware</nameparam>
		/// <param name="name">Target destination</param>
		/// <returns></returns>
		public LogAdminitrator SetFormatMiddleware<TMiddleware>(string name) where TMiddleware : IFormatMiddleware, new()
		{
			this.Flush();
			_logConfiguration.SetFormatMiddleware<TMiddleware>(name);
			return this;
		}

		/// <summary>
		/// Gets the format middleware name for a destination
		/// </summary>
		/// <param name="name">Name of the destination</param>
		/// <param name="middlewareName">Name of the middleware</param>
		/// <returns></returns>
		public LogAdminitrator GetFormatMiddleware(string name, out string middlewareName)
		{
			middlewareName = _logConfiguration.GetFormatMiddlewareName(name);
			return this;
		}

		/// <summary>
		/// Returns all destination names and their registered format middleware name
		/// </summary>
		/// <returns></returns>
		public LogAdminitrator GetFormatMiddlewares(out IReadOnlyCollection<KeyValuePair<string, string>> formatMiddlewares)
		{
			formatMiddlewares = _logConfiguration.GetFormatMiddlewares();
			return this;
		}
		#endregion

		#region Queue middleware
		/// <summary>
		/// Sets the name of the queue middleware and notifies observing components
		/// </summary>
		/// <param name="middlewareName">Name of the queue middleware</param>
		/// <returns></returns>
		public LogAdminitrator SetQueueMiddleware<TMiddleware>(string name) where TMiddleware : IQueueMiddleware, new()
		{
			this.Flush();
			_logConfiguration.SetQueueMiddleware<TMiddleware>(name);
			return this;
		}

		/// <summary>
		/// Gets the queue middleware name for a destination
		/// </summary>
		/// <param name="name">Name of the destination</param>
		/// <param name="middlewareName">Name of the middleware</param>
		/// <returns></returns>
		public LogAdminitrator GetQueueMiddlewareName(string name, out string middlewareName)
		{
			middlewareName = _logConfiguration.GetQueueMiddlewareName(name);
			return this;
		}

		/// <summary>
		/// Returns all destination names and their registered queue middleware name
		/// </summary>
		/// <returns></returns>
		public LogAdminitrator GetQueueMiddlewares(out IReadOnlyCollection<KeyValuePair<string, string>> queueMiddlewares)
		{
			queueMiddlewares = _logConfiguration.GetQueueMiddlewares();
			return this;
		}
		#endregion

		#region Filter middleware
		/// <summary>
		/// Sets the name of the filter middleware and notifies observing components
		/// </summary>
		/// <param name="name">Name of the target destination</param>
		/// <returns></returns>
		public LogAdminitrator SetFilterMiddleware<TMiddleware>(string name) where TMiddleware : IFilterMiddleware, new()
		{
			this.Flush();
			_logConfiguration.SetFilterMiddleware<TMiddleware>(name);
			return this;
		}

		/// <summary>
		/// Gets the filter middleware name for a destination
		/// </summary>
		/// <param name="name">Name of the destination</param>
		/// <param name="middlewareName">Name of the middleware</param>
		/// <returns></returns>
		public LogAdminitrator GetFilterMiddlewareName(string name, out string middlewareName)
		{
			middlewareName = _logConfiguration.GetFilterMiddlewareName(name);
			return this;
		}

		/// <summary>
		/// Returns all destination names and their registered filter middleware name
		/// </summary>
		/// <returns></returns>
		public LogAdminitrator GetFilterMiddlewares(out IReadOnlyCollection<KeyValuePair<string, string>> filterMiddlewares)
		{
			filterMiddlewares = _logConfiguration.GetFilterMiddlewares();
			return this;
		}
		#endregion

		/// <summary>
		/// Adds a custom destination.
		/// All destinations will be used when creating a logger from a LoggerPool.
		/// </summary>
		/// <param name="logDestination">Instance of the new destination</param>
		/// <param name="formatFunction">Default formatting function to pass to this destination</param>
		/// <returns></returns>
		public LogAdminitrator AddCustomDestination(ILogDestination logDestination)
		{
			if (logDestination == null)
			{
				throw new ArgumentNullException(nameof(logDestination));
			}

			_loggerPool.AddLogDestination(logDestination);

			return this;
		}

		/// <summary>
		/// Adds a destination that writes to the console.
		/// All destinations will be used when creating a logger from a LoggerPool.
		/// </summary>
		/// <param name="formatFunction"></param>
		/// <param name="useColors"></param>
		/// <returns></returns>
		public LogAdminitrator AddConsoleLogDestination(bool useColors = false)
		{
			var logDestination = new ConsoleLogDestination(useColors);
			return AddCustomDestination(logDestination);
		}

		/// Adds a FileLogDestination that calculates the path
		/// </summary>
		/// <param name="getFileName">Function to calculate the path</param>
		/// <param name="formatFunction">Function to customize the output</param>
		/// <param name="maxFiles">Max count of files.</param>
		/// <param name="logFilePaths">File where to save the different logfiles to delete them when they are bigger then maxFiles</param>
		/// <returns></returns>
		public LogAdminitrator AddFileLogDestination(Func<string> getFileName, int maxFiles = 30, string logFilePaths = null, bool keepFileOpen = true)
		{
			var logDestination = new FileLogDestination(getFileName, maxFiles, logFilePaths, keepFileOpen);
			return AddCustomDestination(logDestination);
		}

		/// <summary>
		/// Adds a FileLogDestination that uses a constant path
		/// </summary>
		/// <param name="fileName">Path to the file</param>
		/// <param name="formatFunction">Function to customize the output</param>
		/// <returns></returns>
		public LogAdminitrator AddFileLogDestination(string fileName, bool keepFileOpen = true)
		{
			return AddFileLogDestination(() => fileName, keepFileOpen: keepFileOpen);
		}

		/// <summary>
		/// Creates a logger for a type or retrieves it from the cache.
		/// </summary>
		/// <param name="sourceType">Type to create a logger for</param>
		/// <returns></returns>
		public Logger GetLogger(Type sourceType) => _loggerPool.GetLogger(sourceType.FullName);

		/// <summary>
		/// Creates a logger for a category or retrieves it from the cache.
		/// </summary>
		/// <param name="category">Category to create a logger for</param>
		/// <returns></returns>
		public Logger GetLogger(string category) => _loggerPool.GetLogger(category);

		/// <summary>
		/// Tells to flush the execution-middleware
		/// </summary>
		public void Flush()
		{
			foreach (var destination in _loggerPool.GetLogDestinations())
			{
				destination.ExecutionMiddleware.Flush();
				destination.QueueMiddleware.Flush();
			}
		}

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
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

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}