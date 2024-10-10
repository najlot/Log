// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Najlot.Log.Middleware;

/// <summary>
/// Common interface for a middleware in the logging-pipeline
/// </summary>
public interface IMiddleware : IDisposable
{
	IMiddleware NextMiddleware { get; set; }

	void Execute(IEnumerable<LogMessage> messages);

	void Flush();
}