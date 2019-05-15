﻿using System;
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
		public string FilterMiddleware { get; set; }
	}

	public static class LogConfiguratorExtension
	{
		private static void WriteXmlConfigurationFile(LogAdminitrator logAdminitrator, string path)
		{
			var encoding = Encoding.UTF8;

			logAdminitrator.GetLogConfiguration(out ILogConfiguration logConfiguration);

			try
			{
				var currentExecutionMiddlewareName = logConfiguration.ExecutionMiddlewareName;
				
				var xmlSerializer = new XmlSerializer(typeof(FileConfiguration));

				using (var stringWriter = new CustomStringWriter(encoding))
				{
					using (var xmlWriter = XmlWriter.Create(stringWriter))
					{
						xmlSerializer.Serialize(xmlWriter, new FileConfiguration()
						{
							LogLevel = logConfiguration.LogLevel,
							ExecutionMiddleware = currentExecutionMiddlewareName
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
		/// <param name="logAdminitrator">LogConfigurator instance</param>
		/// <param name="path">Path to the XML file</param>
		/// <param name="listenForChanges">Should the canges happened at runtime be reflected to the logger</param>
		/// <param name="writeExampleIfSourceDoesNotExists">Should an example be written when file does not exist</param>
		/// <returns></returns>
		public static LogAdminitrator ReadConfigurationFromXmlFile(this LogAdminitrator logAdminitrator, string path, bool listenForChanges = true, bool writeExampleIfSourceDoesNotExists = false)
		{
			try
			{
				if (!File.Exists(path))
				{
					if (writeExampleIfSourceDoesNotExists)
					{
						WriteXmlConfigurationFile(logAdminitrator, path);
					}

					return logAdminitrator;
				}

				ReadConfiguration(path, logAdminitrator);

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
							ReadConfiguration(path, logAdminitrator);
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

			return logAdminitrator;
		}

		private static void ReadConfiguration(string path, LogAdminitrator logAdminitrator)
		{
			FileConfiguration fileConfiguration;

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileConfiguration));
			using (var streamReader = new StreamReader(path))
			{
				fileConfiguration = xmlSerializer.Deserialize(streamReader) as FileConfiguration;
			}

			logAdminitrator.SetLogLevel(fileConfiguration.LogLevel);

			if (fileConfiguration?.ExecutionMiddleware != null)
			{
				logAdminitrator.SetExecutionMiddlewareByName(fileConfiguration.ExecutionMiddleware);
			}
		}
	}
}