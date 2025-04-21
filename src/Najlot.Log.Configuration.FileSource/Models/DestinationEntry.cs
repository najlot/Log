// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Najlot.Log.Configuration.FileSource.Models;

internal class DestinationEntry
{
	public string Name { get; set; } = string.Empty;

	public Dictionary<string, string> Parameters { get; set; } = [];

	public MiddlewareEntry CollectMiddleware { get; set; }

	public List<MiddlewareEntry> Middlewares { get; set; }
}