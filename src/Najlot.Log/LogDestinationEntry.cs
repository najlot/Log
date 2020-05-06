// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log.Util;
using System;

namespace Najlot.Log
{
	internal sealed class LogDestinationEntry : IMiddlewareConfigurationObserver, IDisposable
	{
		public string LogDestinationName;
		public ILogDestination LogDestination;

		public string CollectMiddlewareName;
		public ICollectMiddleware CollectMiddleware;

		public void NotifyCollectMiddlewareChanged(string destinationName, string middlewareName)
		{
			if (LogDestinationName != destinationName)
			{
				return;
			}

			CollectMiddlewareName = middlewareName;
			var oldCollectMiddleware = CollectMiddleware;
			CollectMiddleware = BuildInitialMiddlewarePipe(CollectMiddlewareName, LogDestination);
			oldCollectMiddleware?.Dispose();
		}

		public void NotifyMiddlewareAdded(string destinationName, string middlewareName)
		{
			if (LogDestinationName != destinationName)
			{
				return;
			}

			var mapper = LogConfigurationMapper.Instance;
			var formatMiddlewareName = mapper.GetName<FormatMiddleware>();
			var middlewareType = mapper.GetType(middlewareName);
			var middleware = (IMiddleware)Activator.CreateInstance(middlewareType);
			var currentMiddleware = CollectMiddleware.NextMiddleware;
			var currentMiddlewareName = mapper.GetName(currentMiddleware);

			/* Log pipeline consists of
			 * - 1 CollectMiddleware
			 * 0 ... N Custom Middlewares we try to insert here
			 * - 1 FormatMiddleware
			 * - 1 DestinationWrapper
			 * - Destination
			 * */

			if (currentMiddlewareName == formatMiddlewareName)
			{
				middleware.NextMiddleware = currentMiddleware;
				CollectMiddleware.NextMiddleware = middleware;
				return;
			}

			var previousMiddleware = currentMiddleware;

			while (currentMiddlewareName != formatMiddlewareName)
			{
				previousMiddleware = currentMiddleware;
				currentMiddleware = currentMiddleware.NextMiddleware;
				currentMiddlewareName = mapper.GetName(currentMiddleware);
			}

			middleware.NextMiddleware = currentMiddleware;
			previousMiddleware.NextMiddleware = middleware;
		}

		public void NotifyClearMiddlewares(string destinationName)
		{
			if (LogDestinationName != destinationName)
			{
				return;
			}

			var oldCollectMiddleware = CollectMiddleware;
			CollectMiddleware = BuildInitialMiddlewarePipe(CollectMiddlewareName, LogDestination);
			DisposeMiddlewares(oldCollectMiddleware);
		}

		private static void DisposeMiddlewares(ICollectMiddleware collectMiddleware)
		{
			var middleware = collectMiddleware.NextMiddleware;
			collectMiddleware.Dispose();

			do
			{
				middleware.Dispose();
				middleware = middleware.NextMiddleware;
			}
			while (middleware != null);
		}

		/// <summary>
		/// Creates the initial pipeline which consists of
		/// - 1 CollectMiddleware
		/// - 1 FormatMiddleware
		/// - 1 DestinationWrapper
		/// - Destination
		/// </summary>
		/// <param name="collectMiddlewareName">Name of the CollectMiddleware to create and return</param>
		/// <param name="logDestination">Destination to append at the end</param>
		/// <returns></returns>
		private static ICollectMiddleware BuildInitialMiddlewarePipe(string collectMiddlewareName, ILogDestination logDestination)
		{
			var middleware = new FormatMiddleware();
			middleware.NextMiddleware = new DestinationWrapper(logDestination);

			var collectMiddlewareType = LogConfigurationMapper.Instance.GetType(collectMiddlewareName);
			var collectMiddleware = (ICollectMiddleware)Activator.CreateInstance(collectMiddlewareType);
			collectMiddleware.NextMiddleware = middleware;

			return collectMiddleware;
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		private void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					DisposeMiddlewares(CollectMiddleware);
					LogDestination.Dispose();
				}
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}