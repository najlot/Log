// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
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

		void AddMiddleware<TMiddleware, TDestination>()
			where TMiddleware : IMiddleware
			where TDestination : Destinations.ILogDestination;

		void SetCollectMiddleware<TMiddleware, TDestination>()
			where TMiddleware : ICollectMiddleware
			where TDestination : Destinations.ILogDestination;

		IEnumerable<string> GetMiddlewareNames(string destinationName);

		string GetCollectMiddlewareName(string destinationName);

		/// <summary>
		/// Attaches an observer, that gets notified when changes occur
		/// </summary>
		/// <param name="observer">Observer to attach</param>
		void AttachObserver(IMiddlewareConfigurationObserver observer);

		/// <summary>
		/// Attaches an observer, that gets notified when changes occur
		/// </summary>
		/// <param name="observer">Observer to attach</param>
		void AttachObserver(ILogLevelObserver observer);

		IEnumerable<string> GetDestinationNames();

		/// <summary>
		/// Detaches the observer, so that it does not get notified anymore
		/// </summary>
		/// <param name="observer">Observer to detach</param>
		void DetachObserver(IMiddlewareConfigurationObserver observer);

		/// <summary>
		/// Detaches the observer, so that it does not get notified anymore
		/// </summary>
		/// <param name="observer">Observer to detach</param>
		void DetachObserver(ILogLevelObserver observer);

		void ClearMiddlewares(string destinationName);

		void AddMiddleware(string destinationName, string middlewareName);

		void SetCollectMiddleware(string destinationName, string middlewareName);
	}
}