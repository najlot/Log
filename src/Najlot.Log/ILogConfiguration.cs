using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

namespace Najlot.Log
{
	/// <summary>
	/// LogConfiguration interface.
	/// Contains the configuration and observers, that listen to changes.
	/// </summary>
	public interface ILogConfiguration
	{
		/// <summary>
		/// Current log level for all log destinations observing this configuration
		/// </summary>
		LogLevel LogLevel { get; }

		/// <summary>
		/// Current execution middleware for all log destinations observing this configuration
		/// </summary>
		string ExecutionMiddlewareName { get; }

		/// <summary>
		/// Gets a format middleware for a name of log destination
		/// </summary>
		/// <param name="name">Name of the log destination</param>
		/// <param name="middlewareName">Name of the middleware</param>
		string GetFormatMiddlewareName(string name);

		/// <summary>
		/// Returns all destination names and their registered format middleware name
		/// </summary>
		/// <returns></returns>
		IReadOnlyCollection<KeyValuePair<string, string>> GetFormatMiddlewares();

		/// <summary>
		/// Sets a format middleware for a name of log destination
		/// </summary>
		/// <nameparam name="TMiddleware">Name of the format middleware</nameparam>
		/// <param name="name">Name of the log destination</param>
		void SetFormatMiddleware<TMiddleware>(string name) where TMiddleware : IFormatMiddleware, new();

		/// <summary>
		/// Gets a queue middleware for a name of log destination
		/// </summary>
		/// <param name="name">Name of the log destination</param>
		/// <param name="middlewareName">Name of the middleware</param>
		string GetQueueMiddlewareName(string name);

		/// <summary>
		/// Returns all destination name and their registered queue middleware name
		/// </summary>
		/// <returns></returns>
		IReadOnlyCollection<KeyValuePair<string, string>> GetQueueMiddlewares();

		/// <summary>
		/// Sets a queue middleware for a name of log destination
		/// </summary>
		/// <nameparam name="TMiddleware">Name of the queue middleware</nameparam>
		/// <param name="name">Name of the log destination</param>
		void SetQueueMiddleware<TMiddleware>(string name) where TMiddleware : IQueueMiddleware, new();

		/// <summary>
		/// Gets a filter middleware for a name of log destination
		/// </summary>
		/// <param name="name">Name of the log destination</param>
		/// <param name="middlewareName">Name of the middleware</param>
		string GetFilterMiddlewareName(string name);

		/// <summary>
		/// Returns all destination names and their registered filter middleware name
		/// </summary>
		/// <returns></returns>
		IReadOnlyCollection<KeyValuePair<string, string>> GetFilterMiddlewares();

		/// <summary>
		/// Sets a filter middleware for a name of log destination
		/// </summary>
		/// <nameparam name="TMiddleware">Name of the filter middleware</nameparam>
		/// <param name="name">Name of the log destination</param>
		void SetFilterMiddleware<TMiddleware>(string name) where TMiddleware : IFilterMiddleware, new();

		/// <summary>
		/// Attaches an observer, that gets notified when changes occur
		/// </summary>
		/// <param name="observer">Observer to attach</param>
		void AttachObserver(IConfigurationObserver observer);

		/// <summary>
		/// Detaches the observer, so that it does not get notified anymore
		/// </summary>
		/// <param name="observer">Observer to detach</param>
		void DetachObserver(IConfigurationObserver observer);
	}
}