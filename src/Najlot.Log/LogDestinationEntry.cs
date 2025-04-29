// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log.Util;
using System;
using System.Collections.Generic;

namespace Najlot.Log;

internal sealed class DestinationEntry : IMiddlewareConfigurationObserver, IDisposable
{
	public readonly string DestinationName;
	public readonly IDestination Destination;

	private string _collectMiddlewareName;
	public ICollectMiddleware CollectMiddleware;

	public DestinationEntry(
		IDestination destination,
		string destinationName,
		string collectMiddlewareName,
		IEnumerable<string> middlewareNames)
	{
		Destination = destination;
		DestinationName = destinationName;

		CollectMiddleware = BuildInitialMiddlewarePipe(collectMiddlewareName, destination);

		foreach (var name in middlewareNames)
		{
			NotifyMiddlewareAdded(DestinationName, name);
		}
	}

	public void NotifyCollectMiddlewareChanged(string destinationName, string middlewareName)
	{
		if (DestinationName != destinationName)
		{
			return;
		}

		_collectMiddlewareName = middlewareName;
		var oldCollectMiddleware = CollectMiddleware;
		CollectMiddleware = BuildInitialMiddlewarePipe(_collectMiddlewareName, Destination);
		CollectMiddleware.NextMiddleware = oldCollectMiddleware.NextMiddleware;

		oldCollectMiddleware.NextMiddleware = null;
		oldCollectMiddleware?.Dispose();
	}

	public void NotifyMiddlewareAdded(string destinationName, string middlewareName)
	{
		if (DestinationName != destinationName)
		{
			return;
		}

		var middlewareType = LogConfigurationMapper.Instance.GetType(middlewareName);
		var middleware = (IMiddleware)Activator.CreateInstance(middlewareType);
		var currentMiddleware = CollectMiddleware.NextMiddleware;

		/* Log pipeline consists of
		 * - 1 CollectMiddleware
		 * 0 ... N Custom Middlewares we try to insert here
		 * - 1 FormatMiddleware
		 * - 1 DestinationWrapper
		 * - 1 Destination
		 * */

		if (currentMiddleware.NextMiddleware is DestinationWrapper)
		{
			middleware.NextMiddleware = currentMiddleware;
			CollectMiddleware.NextMiddleware = middleware;
			return;
		}

		var previousMiddleware = currentMiddleware;

		while (currentMiddleware.NextMiddleware is not DestinationWrapper)
		{
			previousMiddleware = currentMiddleware;
			currentMiddleware = currentMiddleware.NextMiddleware;
		}

		middleware.NextMiddleware = currentMiddleware;
		previousMiddleware.NextMiddleware = middleware;
	}

	public void NotifyClearMiddlewares(string destinationName)
	{
		if (DestinationName != destinationName)
		{
			return;
		}

		var oldCollectMiddleware = CollectMiddleware;
		CollectMiddleware = BuildInitialMiddlewarePipe(_collectMiddlewareName, Destination);
		DisposeMiddlewares(oldCollectMiddleware);
	}

	internal static void DisposeMiddlewares(ICollectMiddleware collectMiddleware)
	{
		var middleware = collectMiddleware.NextMiddleware;
		collectMiddleware?.Dispose();

		do
		{
			middleware?.Dispose();
			middleware = middleware.NextMiddleware;
		}
		while (middleware != null);
	}

	/// <summary>
	/// Creates the initial pipeline which consists of
	/// - 1 CollectMiddleware
	/// - 1 FormatMiddleware
	/// - 1 DestinationWrapper
	/// - 1 Destination
	/// </summary>
	/// <param name="collectMiddlewareName">Name of the CollectMiddleware to create and return</param>
	/// <param name="destination">Destination to append at the end</param>
	/// <returns></returns>
	private static ICollectMiddleware BuildInitialMiddlewarePipe(string collectMiddlewareName, IDestination destination)
	{
		var collectMiddlewareType = LogConfigurationMapper.Instance.GetType(collectMiddlewareName);
		var collectMiddleware = (ICollectMiddleware)Activator.CreateInstance(collectMiddlewareType);
		collectMiddleware.NextMiddleware = new FormatMiddleware
		{
			NextMiddleware = new DestinationWrapper(destination)
		};

		return collectMiddleware;
	}

	private bool _disposedValue = false;

	private void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			_disposedValue = true;

			if (disposing)
			{
				DisposeMiddlewares(CollectMiddleware);
				Destination.Dispose();
			}
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}