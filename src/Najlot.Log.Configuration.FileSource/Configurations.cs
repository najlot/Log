// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Najlot.Log.Configuration.FileSource;

public class Configurations
{
	public LogLevel LogLevel { get; set; }

	public List<DestinationEntry> Destinations { get; set; } = [];
}