using Najlot.Log.Middleware;
using Najlot.Log.Util;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Najlot.Log.Configuration.FileSource
{
	[XmlRoot("NajlotLogConfiguration")]
	public class FileConfiguration
	{
		public LogLevel LogLevel { get; set; }
		public string ExecutionMiddleware { get; set; }
	}

    public static class LogConfiguratorExtension
	{
		private static void WriteXmlConfigurationFile(LogConfigurator logConfigurator, string path)
		{
			var encoding = Encoding.UTF8;

			logConfigurator.GetLogConfiguration(out ILogConfiguration logConfiguration);
			
			try
			{
				var currentExecutionMiddlewareType = logConfiguration.ExecutionMiddleware.GetType();

				var currentExecutionMiddlewareFullTypeName = currentExecutionMiddlewareType.FullName;
				if (currentExecutionMiddlewareType.Assembly != null)
				{
					currentExecutionMiddlewareFullTypeName += ", " + currentExecutionMiddlewareType.Assembly.GetName().Name;
				}

				var xmlSerializer = new XmlSerializer(typeof(FileConfiguration));

				using (var stringWriter = new CustomStringWriter(encoding))
				{
					using (var xmlWriter = XmlWriter.Create(stringWriter))
					{
						xmlSerializer.Serialize(xmlWriter, new FileConfiguration()
						{
							LogLevel = logConfiguration.LogLevel,
							ExecutionMiddleware = currentExecutionMiddlewareFullTypeName
						});

						File.WriteAllText(path, stringWriter.ToString(), encoding);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Najlot.Log: " + ex);
			}
		}

		/// <summary>
		/// Reads logconfiguration from an XML file
		/// </summary>
		/// <param name="logConfigurator">LogConfigurator instance</param>
		/// <param name="path">Path to the XML file</param>
		/// <param name="listenForChanges">Should the canges happened at runtime be reflected to the logger</param>
		/// <param name="writeExampleIfSourceDoesNotExists">Should an example be written when file does not exist</param>
		/// <returns></returns>
		public static LogConfigurator ReadConfigurationFromXmlFile(this LogConfigurator logConfigurator, string path, bool listenForChanges = true, bool writeExampleIfSourceDoesNotExists = false)
		{
			logConfigurator.GetLogConfiguration(out ILogConfiguration logConfiguration);

			try
			{
				if (!File.Exists(path))
				{
					if(writeExampleIfSourceDoesNotExists)
					{
						WriteXmlConfigurationFile(logConfigurator, path);
					}
					
					return logConfigurator;
				}

				ReadConfiguration(path, logConfiguration);

				if (listenForChanges)
				{
					var dir = Path.GetDirectoryName(path);

					if (dir == "")
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
							ReadConfiguration(path, logConfiguration);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Najlot.Log: " + ex);
						}
					};

					fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
					fileSystemWatcher.EnableRaisingEvents = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Najlot.Log: " + ex);
			}

			return logConfigurator;
		}

		private static void ReadConfiguration(string path, ILogConfiguration logConfiguration)
		{
			FileConfiguration fileConfiguration;

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileConfiguration));
			using (var streamReader = new StreamReader(path))
			{
				fileConfiguration = xmlSerializer.Deserialize(streamReader) as FileConfiguration;
			}

			logConfiguration.LogLevel = fileConfiguration.LogLevel;

			AssignExecutionMiddlewareIfChanged(logConfiguration, fileConfiguration);
		}

		private static void AssignExecutionMiddlewareIfChanged(ILogConfiguration logConfiguration, FileConfiguration fileConfiguration)
		{
			var currentExecutionMiddlewareType = logConfiguration.ExecutionMiddleware.GetType();

			var executionMiddlewareType = Type.GetType(fileConfiguration.ExecutionMiddleware, false);

			if (executionMiddlewareType == null)
			{
				Console.WriteLine($"Najlot.Log: New execution middleware of type '{fileConfiguration.ExecutionMiddleware}' not found!");
				return;
			}
			
			if(executionMiddlewareType == currentExecutionMiddlewareType)
			{
				return;
			}
			
			if (!(Activator.CreateInstance(executionMiddlewareType) is IExecutionMiddleware newExecutionMiddleware))
			{
				Console.WriteLine("Najlot.Log: New execution middleware is not " + nameof(IExecutionMiddleware));
				return;
			}

			logConfiguration.ExecutionMiddleware = newExecutionMiddleware;
		}
	}
}
