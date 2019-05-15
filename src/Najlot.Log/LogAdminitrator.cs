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
			return this.SetExecutionMiddlewareByType(typeof(TExecutionMiddleware));
		}

		/// <summary>
		/// Sets the type of the execution middleware and notifies observing components
		/// </summary>
		/// <param name="middlewareType">Type of the execution middleware</param>
		/// <returns></returns>
		public LogAdminitrator SetExecutionMiddlewareByType(Type middlewareType)
		{
			if (middlewareType == null)
			{
				Console.WriteLine("Najlot.Log: New execution middleware type is null.");
				return this;
			}

			this.Flush();
			_logConfiguration.ExecutionMiddlewareType = middlewareType;
			return this;
		}

		#region Format middleware
		/// <summary>
		/// Sets the type of the format middleware and notifies observing components
		/// </summary>
		/// <typeparam name="TMiddleware">Type of the format middleware</typeparam>
		/// <param name="type">Target destination</param>
		/// <returns></returns>
		public LogAdminitrator SetFormatMiddlewareForType<TMiddleware>(Type type) where TMiddleware : IFormatMiddleware, new()
		{
			this.Flush();
			_logConfiguration.SetFormatMiddlewareForType<TMiddleware>(type);
			return this;
		}

		/// <summary>
		/// Gets the format middleware type for a destination
		/// </summary>
		/// <param name="type">Type of the destination</param>
		/// <param name="middlewareType">Type of the middleware</param>
		/// <returns></returns>
		public LogAdminitrator GetFormatMiddlewareTypeForType(Type type, out Type middlewareType)
		{
			_logConfiguration.GetFormatMiddlewareTypeForType(type, out middlewareType);
			return this;
		}

		/// <summary>
		/// Returns all destination types and their registered format middleware type
		/// </summary>
		/// <returns></returns>
		public LogAdminitrator GetFormatMiddlewares(out IReadOnlyCollection<KeyValuePair<Type, Type>> formatMiddlewares)
		{
			formatMiddlewares = _logConfiguration.GetFormatMiddlewares();
			return this;
		}
		#endregion

		#region Queue middleware
		/// <summary>
		/// Sets the type of the queue middleware and notifies observing components
		/// </summary>
		/// <param name="middlewareType">Type of the queue middleware</param>
		/// <returns></returns>
		public LogAdminitrator SetQueueMiddlewareForType<TMiddleware>(Type type) where TMiddleware : IQueueMiddleware, new()
		{
			this.Flush();
			_logConfiguration.SetQueueMiddlewareForType<TMiddleware>(type);
			return this;
		}

		/// <summary>
		/// Gets the queue middleware type for a destination
		/// </summary>
		/// <param name="type">Type of the destination</param>
		/// <param name="middlewareType">Type of the middleware</param>
		/// <returns></returns>
		public LogAdminitrator GetQueueMiddlewareTypeForType(Type type, out Type middlewareType)
		{
			_logConfiguration.GetQueueMiddlewareTypeForType(type, out middlewareType);
			return this;
		}

		/// <summary>
		/// Returns all destination types and their registered queue middleware type
		/// </summary>
		/// <returns></returns>
		public LogAdminitrator GetQueueMiddlewares(out IReadOnlyCollection<KeyValuePair<Type, Type>> queueMiddlewares)
		{
			queueMiddlewares = _logConfiguration.GetQueueMiddlewares();
			return this;
		}
		#endregion

		#region Filter middleware
		/// <summary>
		/// Sets the type of the filter middleware and notifies observing components
		/// </summary>
		/// <param name="middlewareType">Type of the filter middleware</param>
		/// <returns></returns>
		public LogAdminitrator SetFilterMiddlewareForType<TMiddleware>(Type type) where TMiddleware : IFilterMiddleware, new()
		{
			this.Flush();
			_logConfiguration.SetFilterMiddlewareForType<TMiddleware>(type);
			return this;
		}

		/// <summary>
		/// Gets the filter middleware type for a destination
		/// </summary>
		/// <param name="type">Type of the destination</param>
		/// <param name="middlewareType">Type of the middleware</param>
		/// <returns></returns>
		public LogAdminitrator GetFilterMiddlewareTypeForType(Type type, out Type middlewareType)
		{
			_logConfiguration.GetFilterMiddlewareTypeForType(type, out middlewareType);
			return this;
		}

		/// <summary>
		/// Returns all destination types and their registered filter middleware type
		/// </summary>
		/// <returns></returns>
		public LogAdminitrator GetFilterMiddlewares(out IReadOnlyCollection<KeyValuePair<Type, Type>> filterMiddlewares)
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
		public Logger GetLogger(Type sourceType) => GetLogger(sourceType.FullName);

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