// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Najlot.Log.Configuration.FileSource
{
	public static class LogConfiguratorExtension
	{
		private static void WriteXmlConfigurationFile(LogAdministrator logAdministrator, string path)
		{
			try
			{
				logAdministrator.GetLogConfiguration(out var config);
				logAdministrator.GetDestinationNames(out var destinationNames);

				var configurations = new Configurations()
				{
					LogLevel = config.LogLevel,
					Destinations = destinationNames
						.Select(name =>
						{
							logAdministrator.GetDestinationConfiguration(name, out var configuration);

							return new DestinationEntry
							{
								Name = name,
								Parameters = configuration
									.Select(c => new Parameter { Name = c.Key, Value = c.Value })
									.ToList(),
								CollectMiddleware = new ConfigurationEntry { Name = config.GetCollectMiddlewareName(name) },
								Middlewares = config
									.GetMiddlewareNames(name)
									.Select(n => new ConfigurationEntry { Name = n })
									.ToList(),
							};
						})
						.ToList()
				};

				using (var stringWriter = new CustomStringWriter(Encoding.UTF8))
				{
					using (var xmlWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented })
					{
						var xmlSerializer = new XmlSerializer(typeof(Configurations));
						xmlSerializer.Serialize(xmlWriter, configurations);
						File.WriteAllText(path, stringWriter.ToString(), Encoding.UTF8);
					}
				}
			}
			catch (Exception ex)
			{
				LogErrorHandler.Instance.Handle("Error writing xml configuration file", ex);
			}
		}

		/// <summary>
		/// Reads configuration from an XML file
		/// </summary>
		/// <param name="logAdministrator">LogAdministrator instance</param>
		/// <param name="path">Path to the XML file</param>
		/// <param name="listenForChanges">Should the changes happened at runtime be reflected to the logger</param>
		/// <param name="writeExampleIfSourceDoesNotExists">Should an example be written when file does not exist</param>
		/// <returns></returns>
		public static LogAdministrator ReadConfigurationFromXmlFile(this LogAdministrator logAdministrator, string path, bool listenForChanges = true, bool writeExampleIfSourceDoesNotExists = false)
		{
			try
			{
				if (File.Exists(path))
				{
					ReadConfiguration(path, logAdministrator);

					if (listenForChanges)
					{
						EnableChangeListener(logAdministrator, path);
					}
				}
				else if (writeExampleIfSourceDoesNotExists)
				{
					WriteXmlConfigurationFile(logAdministrator, path);
				}
			}
			catch (Exception ex)
			{
				LogErrorHandler.Instance.Handle("Error reading xml configuration file", ex);
			}

			return logAdministrator;
		}

		private static void EnableChangeListener(LogAdministrator logAdministrator, string path)
		{
			var dir = Path.GetDirectoryName(path);

			if (string.IsNullOrWhiteSpace(dir))
			{
				dir = Directory.GetCurrentDirectory();
			}

			var fileSystemWatcher = new FileSystemWatcher(dir, Path.GetFileName(path));

			fileSystemWatcher.Changed += (object sender, FileSystemEventArgs e) =>
			{
				// Ensure the file is not accessed any more
				Thread.Sleep(75);

				try
				{
					ReadConfiguration(path, logAdministrator);
				}
				catch (Exception ex)
				{
					LogErrorHandler.Instance.Handle("Error reading xml configuration file", ex);
				}
			};

			fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
			fileSystemWatcher.EnableRaisingEvents = true;
		}

		private static void ReadConfiguration(string path, LogAdministrator logAdministrator)
		{
			var xmlSerializer = new XmlSerializer(typeof(Configurations));
			Configurations fileConfigurations;

			using (var streamReader = new StreamReader(path))
			{
				fileConfigurations = xmlSerializer.Deserialize(streamReader) as Configurations;
			}

			logAdministrator.SetLogLevel(fileConfigurations.LogLevel);

			foreach (var config in fileConfigurations.Destinations)
			{
				var parameterDictionary = config.Parameters
					.ToDictionary(parameter => parameter.Name, parameter => parameter.Value);

				logAdministrator.SetDestinationConfiguration(config.Name, parameterDictionary);

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
		}
	}
}