// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Najlot.Log.Configuration.FileSource
{
	public class DestinationEntry
	{
		public string DestinationName { get; set; }
		public string CollectMiddlewareName { get; set; }
		public List<string> MiddlewareNames { get; set; }
	}
}