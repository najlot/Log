using NajlotLog.Middleware;
using System;

namespace NajlotLog.Configuration
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
		LogLevel LogLevel { get; set; }

		/// <summary>
		/// Current execution middleware for all log destinations observing this configuration
		/// </summary>
		IExecutionMiddleware ExecutionMiddleware { get; set; }

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

		/// <summary>
		/// Notifies observers. Can specify the type of observers to notify
		/// </summary>
		/// <param name="type">The type of observers to notify</param>
		void NotifyObservers(Type type = null);

		/// <summary>
		/// Gets a format function for a type of log destination
		/// </summary>
		/// <param name="type">Type of the log destination</param>
		/// <param name="function">Function that was determined. Will be null when failed</param>
		/// <returns>Returns true of success, false otherwise</returns>
		bool TryGetFormatFunctionForType(Type type, out Func<LogMessage, string> function);

		/// <summary>
		/// Sets a format function for a type of log destination
		/// </summary>
		/// <param name="type">Type of the log destination</param>
		/// <param name="function">Function to call</param>
		/// <returns>Returns true of success, false otherwise</returns>
		bool TrySetFormatFunctionForType(Type type, Func<LogMessage, string> function);

		/// <summary>
		/// Removes all custom format functions
		/// </summary>
		void ClearAllFormatFunctions();
	}
}
