// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log.Middleware;

/// <summary>
/// Common interface for a middleware that collects messages from different threads
/// </summary>
public interface ICollectMiddleware : IDisposable
{
	IMiddleware NextMiddleware { get; set; }

	void Execute(LogMessage message);

	void Flush();
}