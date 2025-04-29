// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Linq;

namespace Najlot.Log.Configuration.FileSource.Xml;

internal class XmlConfigurationService : IConfigurationService
{
	private XmlSerializer _serializer;
	private XmlSerializer Serializer => _serializer ??= new(typeof(Configurations));

	public Models.LogConfiguration ReadFromString(string content)
	{
		using var stringReader = new StringReader(content);
		var configurations = Serializer.Deserialize(stringReader) as Configurations;

		return new Models.LogConfiguration()
		{
			LogLevel = configurations.LogLevel,
			Destinations = configurations.Destinations
				.Select(c => new Models.LogDestinationEntry
				{
					Name = c.Name,
					CollectMiddleware = new Models.LogMiddlewareEntry
					{
						Name = c.CollectMiddleware.Name,
					},
					Parameters = c.Parameters.ToDictionary(parameter => parameter.Name, parameter => parameter.Value),
					Middlewares = c.Middlewares.Select(m => new Models.LogMiddlewareEntry
					{
						Name = m.Name,
					}).ToList(),
				}).ToList(),
		};
	}

	public string WriteToString(Models.LogConfiguration configurations)
	{
		var xmlConfig = new Configurations
		{
			LogLevel = configurations.LogLevel,
			Destinations = configurations.Destinations
				.Select(c => new DestinationEntry
				{
					Name = c.Name,
					CollectMiddleware = new ConfigurationEntry
					{
						Name = c.CollectMiddleware.Name,
					},
					Parameters = c.Parameters.Select(p => new Parameter
					{
						Name = p.Key,
						Value = p.Value,
					}).ToList(),
					Middlewares = c.Middlewares.Select(m => new ConfigurationEntry
					{
						Name = m.Name,
					}).ToList(),
				}).ToList(),
		};

		using var stringWriter = new CustomStringWriter(Encoding.UTF8);
		using var xmlWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };
		Serializer.Serialize(xmlWriter, xmlConfig);
		return stringWriter.ToString();
	}
}