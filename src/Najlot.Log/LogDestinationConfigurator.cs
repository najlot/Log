// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Najlot.Log;

internal static class LogDestinationConfigurator
{
	internal static IDictionary<string, string> GetDestinationConfiguration(LoggerPool loggerPool, string destinationName)
	{
		var configuration = new Dictionary<string, string>();

		var destination = loggerPool
			.GetDestinations()
			.FirstOrDefault(d => d.DestinationName == destinationName)?
			.Destination;

		if (destination == null)
		{
			return configuration;
		}

		var type = destination.GetType();
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

		foreach (var property in properties)
		{
			var attribute = property?.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).FirstOrDefault();

			if (attribute is LogConfigurationNameAttribute nameAttribute)
			{
				var value = property.GetValue(destination);
				configuration[nameAttribute.Name] = value?.ToString();
			}
		}

		return configuration;
	}

	internal static void SetDestinationConfiguration(LoggerPool loggerPool, string destinationName, IDictionary<string, string> configuration)
	{
		var destination = loggerPool
			.GetDestinations()
			.FirstOrDefault(d => d.DestinationName == destinationName)?
			.Destination;

		if (destination == null)
		{
			return;
		}

		var type = destination.GetType();
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

		foreach (var property in properties)
		{
			var attribute = property?.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).FirstOrDefault();

			if (attribute is LogConfigurationNameAttribute nameAttribute
			    && configuration.TryGetValue(nameAttribute.Name, out var val))
			{
				if (val == null)
				{
					property.SetValue(destination, null);
				}
				else
				{
					var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(property.PropertyType);
					var propVal = typeConverter.ConvertFromString(val);
					property.SetValue(destination, propVal);
				}
			}
		}
	}
}