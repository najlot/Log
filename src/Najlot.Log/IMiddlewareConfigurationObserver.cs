// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace Najlot.Log;

/// <summary>
/// Interface for an observer that reacts to changes to middlewares
/// </summary>
public interface IMiddlewareConfigurationObserver
{
	void NotifyCollectMiddlewareChanged(string destinationName, string middlewareName);

	void NotifyMiddlewareAdded(string destinationName, string middlewareName);

	void NotifyClearMiddlewares(string destinationName);
}