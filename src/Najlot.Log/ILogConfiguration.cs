using Najlot.Log.Middleware;
using System;

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
		Type ExecutionMiddlewareType { get; } // TODO log class name

		/// <summary>
		/// Type of the filter middleware for all log destinations observing this configuration
		/// </summary>
		Type FilterMiddlewareType { get; } // TODO log class name

		/// <summary>
		/// Gets a format function for a type of log destination
		/// </summary>
		/// <param name="type">Type of the log destination</param>
		/// <param name="middlewareType">Type of the middleware</param>
		void GetFormatMiddlewareTypeForType(Type type, out Type middlewareType);

		/// <summary>
		/// Sets a format middleware for a type of log destination
		/// </summary>
		/// <typeparam name="TMiddleware">Type of the format middleware</typeparam>
		/// <param name="type">Type of the log destination</param>
		void SetFormatMiddlewareForType<TMiddleware>(Type type) where TMiddleware : IFormatMiddleware, new();

		/// <summary>
		/// Attaches an observer, that gets notified when changes occur
		/// </summary>
		/// <param name="observer">Observer to attach</param>
		void AttachObserver(IConfigurationChangedObserver observer);

		/// <summary>
		/// Detaches the observer, so that it does not get notified anymore
		/// </summary>
		/// <param name="observer">Observer to detach</param>
		void DetachObserver(IConfigurationChangedObserver observer);
	}
}