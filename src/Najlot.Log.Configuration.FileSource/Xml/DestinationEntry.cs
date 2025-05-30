﻿// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;
using System.Collections.Generic;

namespace Najlot.Log.Configuration.FileSource.Xml;

public class DestinationEntry : ConfigurationEntry
{
	public List<Parameter> Parameters { get; set; }

	public ConfigurationEntry CollectMiddleware { get; set; }

	[XmlArrayItem("Middleware")]
	public List<ConfigurationEntry> Middlewares { get; set; }
}