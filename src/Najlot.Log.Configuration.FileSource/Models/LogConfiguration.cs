// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Najlot.Log.Configuration.FileSource.Models;

internal class LogConfiguration
{
	public LogLevel LogLevel { get; set; }

	public List<LogDestinationEntry> Destinations { get; set; } = [];
}