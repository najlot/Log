// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Najlot.Log.Configuration.FileSource.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Najlot.Log.Configuration.FileSource;

public static class LogAdministratorConfigurationExtensions
{
	public static LogAdministrator ReadConfiguration(
		this LogAdministrator logAdministrator,
		IConfiguration configuration,
		string sectionName = "NajlotLog")
	{
		try
		{
			var section = configuration.GetSection(sectionName);
			var configs = section.Get<LogConfiguration>();
			ConfigurationIoService.ApplyConfiguration(logAdministrator, configs);
		}
		catch (Exception ex)
		{
			LogErrorHandler.Instance.Handle("Error reading configuration", ex);
		}

		return logAdministrator;
	}

	/// <summary>
	/// Reads configuration from an XML file
	/// </summary>
	/// <param name="logAdministrator">LogAdministrator instance</param>
	/// <param name="path">Path to the XML file</param>
	/// <param name="listenForChanges">Should the changes happened at runtime be reflected to the logger</param>
	/// <param name="writeExampleIfSourceDoesNotExists">Should an example be written when file does not exist</param>
	/// <returns></returns>
	public static LogAdministrator ReadConfigurationFromXmlFile(
		this LogAdministrator logAdministrator,
		string path,
		bool listenForChanges = true,
		bool writeExampleIfSourceDoesNotExists = false)
	{
		var service = new Xml.XmlConfigurationService();
		ReadFromFile(logAdministrator, service, path, listenForChanges, writeExampleIfSourceDoesNotExists);
		return logAdministrator;
	}

	/// <summary>
	/// Reads configuration from an Json file
	/// </summary>
	/// <param name="logAdministrator">LogAdministrator instance</param>
	/// <param name="path">Path to the Json file</param>
	/// <param name="listenForChanges">Should the changes happened at runtime be reflected to the logger</param>
	/// <param name="writeExampleIfSourceDoesNotExists">Should an example be written when file does not exist</param>
	/// <returns></returns>
	public static LogAdministrator ReadConfigurationFromJsonFile(
		this LogAdministrator logAdministrator,
		string path,
		bool listenForChanges = true,
		bool writeExampleIfSourceDoesNotExists = false)
	{
		var service = new Json.JsonConfigurationService();
		ReadFromFile(logAdministrator, service, path, listenForChanges, writeExampleIfSourceDoesNotExists);
		return logAdministrator;
	}

	private static void ReadFromFile(
		this LogAdministrator logAdministrator,
		IConfigurationService service,
		string path,
		bool listenForChanges = true,
		bool writeExampleIfSourceDoesNotExists = false)
	{
		try
		{
			if (File.Exists(path))
			{
				ConfigurationIoService.ReadFromFile(path, service, logAdministrator);

				if (listenForChanges)
				{
					EnableChangeListener(logAdministrator, service, path);
				}
			}
			else if (writeExampleIfSourceDoesNotExists)
			{
				ConfigurationIoService.WriteToFile(path, service, logAdministrator);
			}
		}
		catch (Exception ex)
		{
			LogErrorHandler.Instance.Handle("Error reading configuration file", ex);
		}
	}

	private static readonly Dictionary<string, FileSystemWatcher> _fileSystemWatcher = [];

	private static void EnableChangeListener(LogAdministrator logAdministrator, IConfigurationService service, string path)
	{
		path = Path.GetFullPath(path);
		var directory = Path.GetDirectoryName(path);
		var fileName = Path.GetFileName(path);

		if (string.IsNullOrWhiteSpace(directory))
		{
			directory = Directory.GetCurrentDirectory();
		}

		if (_fileSystemWatcher.TryGetValue(path, out var fileSystemWatcher))
		{
			fileSystemWatcher.EnableRaisingEvents = false;
			fileSystemWatcher.Dispose();
			_fileSystemWatcher.Remove(path);
		}

		fileSystemWatcher = new FileSystemWatcher(directory, fileName);
		_fileSystemWatcher[path] = fileSystemWatcher;

		fileSystemWatcher.Changed += (object sender, FileSystemEventArgs e) =>
		{
			// Ensure the file is not accessed any more
			Thread.Sleep(100);

			try
			{
				ConfigurationIoService.ReadFromFile(path, service, logAdministrator);
			}
			catch (Exception ex)
			{
				LogErrorHandler.Instance.Handle("Error reading configuration file", ex);
			}
		};

		fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
		fileSystemWatcher.EnableRaisingEvents = true;
	}
}