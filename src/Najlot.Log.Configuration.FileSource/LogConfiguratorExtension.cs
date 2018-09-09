using Najlot.Log.Middleware;
using System;
using System.IO;
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
		public static LogConfigurator WriteXmlConfigurationFile(this LogConfigurator logConfigurator, string path, bool replace = false)
		{
			if (File.Exists(path) && !replace)
			{
				return logConfigurator;
			}

			logConfigurator.GetLogConfiguration(out ILogConfiguration logConfiguration);
			
			try
			{
				var currentExecutionMiddlewareType = logConfiguration.ExecutionMiddleware.GetType();

				var currentExecutionMiddlewareFullTypeName = currentExecutionMiddlewareType.FullName;
				if (currentExecutionMiddlewareType.Assembly != null)
				{
					currentExecutionMiddlewareFullTypeName += ", " + currentExecutionMiddlewareType.Assembly.FullName;
				}

				XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileConfiguration));

				using (var stringWriter = new StringWriter())
				{
					using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
					{
						xmlSerializer.Serialize(xmlWriter, new FileConfiguration()
						{
							LogLevel = logConfiguration.LogLevel,
							ExecutionMiddleware = currentExecutionMiddlewareFullTypeName
						});

						File.WriteAllText(path, stringWriter.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Najlot.Log: " + ex);
			}

			return logConfigurator;
		}

		public static LogConfigurator AssignConfigurationFromXmlFile(this LogConfigurator logConfigurator, string path, bool listenForChanges = true)
		{
			logConfigurator.GetLogConfiguration(out ILogConfiguration logConfiguration);

			try
			{
				if (!File.Exists(path))
				{
					Console.WriteLine($"Najlot.Log: {path} not found!");
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
						Thread.Sleep(100); // Ensure the file is not accessed any more

						try
						{
							Console.WriteLine("config changed");

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

			if (logConfiguration.LogLevel != fileConfiguration.LogLevel)
			{
				logConfiguration.LogLevel = fileConfiguration.LogLevel;
			}

			AssignExecutionMiddleware(logConfiguration, fileConfiguration);
		}

		private static void AssignExecutionMiddleware(ILogConfiguration logConfiguration, FileConfiguration fileConfiguration)
		{
			var currentExecutionMiddlewareType = logConfiguration.ExecutionMiddleware.GetType();

			var currentExecutionMiddlewareFullTypeName = currentExecutionMiddlewareType.FullName;
			if(currentExecutionMiddlewareType.Assembly != null)
			{
				currentExecutionMiddlewareFullTypeName += ", " + currentExecutionMiddlewareType.Assembly.FullName;
			}

			if (currentExecutionMiddlewareFullTypeName == fileConfiguration.ExecutionMiddleware)
			{
				return;
			}

			var executionMiddlewareType = Type.GetType(currentExecutionMiddlewareFullTypeName, true);

			if (executionMiddlewareType == null)
			{
				Console.WriteLine($"Najlot.Log: new execution middleware type {fileConfiguration.ExecutionMiddleware} not found!");
				return;
			}

			var newExecutionMiddleware = Activator.CreateInstance(executionMiddlewareType) as IExecutionMiddleware;

			if (newExecutionMiddleware == null)
			{
				Console.WriteLine("Najlot.Log: New execution middleware is not " + nameof(IExecutionMiddleware));
				return;
			}

			logConfiguration.ExecutionMiddleware = newExecutionMiddleware;
		}
	}
}
