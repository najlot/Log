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
		Type ExecutionMiddlewareType { get; }

		/// <summary>
		/// Gets a format middleware for a type of log destination
		/// </summary>
		/// <param name="type">Type of the log destination</param>
		/// <param name="middlewareType">Type of the middleware</param>
		void GetFormatMiddlewareTypeForType(Type type, out Type middlewareType);

		/// <summary>
		/// Returns all destination types and their registered format middleware type
		/// </summary>
		/// <returns></returns>
		IReadOnlyCollection<KeyValuePair<Type, Type>> GetFormatMiddlewares();

		/// <summary>
		/// Sets a format middleware for a type of log destination
		/// </summary>
		/// <typeparam name="TMiddleware">Type of the format middleware</typeparam>
		/// <param name="type">Type of the log destination</param>
		void SetFormatMiddlewareForType<TMiddleware>(Type type) where TMiddleware : IFormatMiddleware, new();

		/// <summary>
		/// Gets a queue middleware for a type of log destination
		/// </summary>
		/// <param name="type">Type of the log destination</param>
		/// <param name="middlewareType">Type of the middleware</param>
		void GetQueueMiddlewareTypeForType(Type type, out Type middlewareType);

		/// <summary>
		/// Returns all destination types and their registered queue middleware type
		/// </summary>
		/// <returns></returns>
		IReadOnlyCollection<KeyValuePair<Type, Type>> GetQueueMiddlewares();

		/// <summary>
		/// Sets a queue middleware for a type of log destination
		/// </summary>
		/// <typeparam name="TMiddleware">Type of the queue middleware</typeparam>
		/// <param name="type">Type of the log destination</param>
		void SetQueueMiddlewareForType<TMiddleware>(Type type) where TMiddleware : IQueueMiddleware, new();

		/// <summary>
		/// Gets a filter middleware for a type of log destination
		/// </summary>
		/// <param name="type">Type of the log destination</param>
		/// <param name="middlewareType">Type of the middleware</param>
		void GetFilterMiddlewareTypeForType(Type type, out Type middlewareType);

		/// <summary>
		/// Returns all destination types and their registered filter middleware type
		/// </summary>
		/// <returns></returns>
		IReadOnlyCollection<KeyValuePair<Type, Type>> GetFilterMiddlewares();

		/// <summary>
		/// Sets a filter middleware for a type of log destination
		/// </summary>
		/// <typeparam name="TMiddleware">Type of the filter middleware</typeparam>
		/// <param name="type">Type of the log destination</param>
		void SetFilterMiddlewareForType<TMiddleware>(Type type) where TMiddleware : IFilterMiddleware, new();

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