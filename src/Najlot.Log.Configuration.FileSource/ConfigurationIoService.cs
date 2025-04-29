// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Configuration.FileSource.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log.Configuration.FileSource;

internal static class ConfigurationIoService
{
	public static void WriteToFile(string path, IConfigurationService service, LogAdministrator logAdministrator)
	{
		try
		{
			logAdministrator
				.GetLogLevel(out var logLevel)
				.GetDestinationNames(out var destinationNames);

			var configurations = new LogConfiguration()
			{
				LogLevel = logLevel,
				Destinations = destinationNames
					.Select(name =>
					{
						logAdministrator
							.GetDestinationConfiguration(name, out var configuration)
							.GetCollectMiddlewareName(name, out var collectMiddlewareName)
							.GetMiddlewareNames(name, out var middlewareNames);

						return new LogDestinationEntry
						{
							Name = name,
							Parameters = configuration.ToDictionary(c => c.Key, c => c.Value),
							CollectMiddleware = new LogMiddlewareEntry { Name = collectMiddlewareName },
							Middlewares = middlewareNames
								.Select(n => new LogMiddlewareEntry { Name = n })
								.ToList(),
						};
					})
					.ToList()
			};

			service.WriteToFile(path, configurations);
		}
		catch (Exception ex)
		{
			LogErrorHandler.Instance.Handle("Error writing xml configuration file", ex);
		}
	}

	public static void ReadFromFile(string path, IConfigurationService service, LogAdministrator logAdministrator)
	{
		var configs = service.ReadFromFile(path);
		logAdministrator.ApplyConfiguration(configs);
	}

	public static void ApplyConfiguration(this LogAdministrator logAdministrator, LogConfiguration configs)
	{
		logAdministrator.GetDestinationNames(out var destinationNames);
		List<string> destinationsToDrop = new(destinationNames);

		logAdministrator.SetLogLevel(configs.LogLevel);

		foreach (var config in configs.Destinations)
		{
			if (!destinationsToDrop.Contains(config.Name))
			{
				logAdministrator.AddDestination(config.Name);
			}
			else
			{
				destinationsToDrop.Remove(config.Name);
			}

			logAdministrator.SetDestinationConfiguration(config.Name, config.Parameters);

			if (!string.IsNullOrWhiteSpace(config.CollectMiddleware?.Name))
			{
				logAdministrator.SetCollectMiddleware(config.Name, config.CollectMiddleware?.Name);
			}

			if (config.Middlewares != null)
			{
				var names = config.Middlewares
					.Select(m => m.Name)
					.Where(m => !string.IsNullOrWhiteSpace(m));

				logAdministrator.SetMiddlewareNames(config.Name, names);
			}
		}

		foreach (var name in destinationsToDrop)
		{
			logAdministrator.RemoveDestination(name);
		}
	}
}