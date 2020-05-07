// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
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
			where TDestination : IDestination;

		void AddMiddleware(string destinationName, string middlewareName);

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

		void ClearMiddlewares(string destinationName);

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

		string GetCollectMiddlewareName(string destinationName);

		IEnumerable<string> GetDestinationNames();

		IEnumerable<string> GetMiddlewareNames(string destinationName);

		void SetCollectMiddleware<TMiddleware, TDestination>()
			where TMiddleware : ICollectMiddleware
			where TDestination : IDestination;

		void SetCollectMiddleware(string destinationName, string middlewareName);
	}
}