// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Najlot.Log.Destinations;

/// <summary>
/// Writes all messages to a file.
/// </summary>
[LogConfigurationName(nameof(FileDestination))]
public sealed class FileDestination : IDestination
{
	[LogConfigurationName(nameof(OutputPath))]
	public string OutputPath { get; set; }

	[LogConfigurationName(nameof(KeepFileOpen))]
	public bool KeepFileOpen { get; set; }

	[LogConfigurationName(nameof(MaxFiles))]
	public int MaxFiles { get; set; }

	[LogConfigurationName(nameof(LogFilesPath))]
	public string LogFilesPath { get; set; }

	private readonly string _newLine = Environment.NewLine;
	private Stream _stream = null;
	private readonly Encoding _encoding = Encoding.UTF8;
	private readonly Func<string> _customGetPathFunc;

	private bool ShouldAutoCleanUp()
	{
		return MaxFiles > 0 && !string.IsNullOrWhiteSpace(LogFilesPath);
	}

	private string GetPath()
	{
		if (_customGetPathFunc != null)
		{
			return _customGetPathFunc();
		}

		if (OutputPath.Contains('{'))
		{
			var now = DateTime.Now;

			return OutputPath
				.Replace("{Day}", now.Day.ToString())
				.Replace("{Month}", now.Month.ToString())
				.Replace("{Year}", now.Year.ToString())
				.Replace("{Hour}", now.Hour.ToString())
				.Replace("{Minute}", now.Minute.ToString())
				;
		}

		return OutputPath;
	}

	private string _lastFilePath;

	public FileDestination()
	{
		OutputPath = "log_{Year}-{Month}-{Year}.txt";
		KeepFileOpen = false;
		MaxFiles = 30;
		LogFilesPath = ".logs";
	}

	public FileDestination(Func<string> customGetPathFunc, int maxFiles, string logFilePaths, bool keepFileOpen)
	{
		_customGetPathFunc = customGetPathFunc;

		OutputPath = null;
		KeepFileOpen = keepFileOpen;
		MaxFiles = maxFiles;
		LogFilesPath = logFilePaths;

		var path = GetPath();
		EnsureDirectoryExists(path);
		_lastFilePath = path;

		if (KeepFileOpen)
		{
			SetStream(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough));
		}

		if (ShouldAutoCleanUp()) CleanUpOldFiles(path);
	}

	public FileDestination(string outputPath, int maxFiles, string logFilePaths, bool keepFileOpen)
	{
		_customGetPathFunc = null;

		OutputPath = outputPath;
		KeepFileOpen = keepFileOpen;
		MaxFiles = maxFiles;
		LogFilesPath = logFilePaths;

		var path = GetPath();
		EnsureDirectoryExists(path);
		_lastFilePath = path;

		if (KeepFileOpen)
		{
			SetStream(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough));
		}

		if (ShouldAutoCleanUp()) CleanUpOldFiles(path);
	}

	public void Log(IEnumerable<LogMessage> messages)
	{
		Log(messages, true);
	}

	private void Log(IEnumerable<LogMessage> messages, bool logRetry)
	{
		var cleanUp = false;
		var keepFileOpen = KeepFileOpen;
		var path = GetPath();

		// Ensure directory is created when the path changes,
		// but try to create when DirectoryNotFoundException occurs.
		// The directory could be deleted by the user in the meantime...
		try
		{
			if (_lastFilePath != path)
			{
				_lastFilePath = path;
				EnsureDirectoryExists(path);
				if (ShouldAutoCleanUp()) cleanUp = true;

				if (keepFileOpen)
				{
					SetStream(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough));
				}
			}
			else if (keepFileOpen && _stream == null)
			{
				SetStream(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough));
			}

			var sb = new StringBuilder();

			foreach (var message in messages)
			{
				sb.Append(message.Message);
				sb.Append(_newLine);
			}

			if (keepFileOpen)
			{
				Write(sb.ToString());
			}
			else
			{
				File.AppendAllText(path, sb.ToString(), _encoding);
			}

			if (cleanUp) CleanUpOldFiles(path);
		}
		catch (DirectoryNotFoundException)
		{
			EnsureDirectoryExists(path);

			if (logRetry)
			{
				Log(messages, false);
			}
		}
	}

	private void CleanUpOldFiles(string path)
	{
		List<string> logFilePathsList = null;

		if (File.Exists(LogFilesPath))
		{
			logFilePathsList = new List<string>(File.ReadAllLines(LogFilesPath));
		}
		else
		{
			logFilePathsList = new List<string>();
		}

		logFilePathsList.Add(path);

		if (logFilePathsList.Count < MaxFiles)
		{
			File.WriteAllLines(LogFilesPath, logFilePathsList.Distinct());
			return;
		}

		logFilePathsList = logFilePathsList
			.Where(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p))
			.Distinct()
			.ToList();

		while (logFilePathsList.Count > MaxFiles)
		{
			var file = logFilePathsList[0];
			logFilePathsList.Remove(file);
			File.WriteAllLines(LogFilesPath, logFilePathsList);
			if (File.Exists(file)) File.Delete(file);
		}

		File.WriteAllLines(LogFilesPath, logFilePathsList);
	}

	private static void EnsureDirectoryExists(string path)
	{
		if (File.Exists(path))
		{
			return;
		}
		
		var dir = Path.GetDirectoryName(path);

		if (!string.IsNullOrWhiteSpace(dir))
		{
			Directory.CreateDirectory(dir);
		}
	}

	private void SetStream(Stream stream)
	{
		_stream?.Dispose();
		_stream = stream;
	}

	private void Write(byte[] bytes)
	{
		_stream.Write(bytes, 0, bytes.Length);
	}

	private void Write(string msg)
	{
		Write(_encoding.GetBytes(msg));
	}

	public void Flush()
	{
		_stream?.Flush();
	}

	#region IDisposable Support

	private bool _disposedValue = false; // To detect redundant calls

	public void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			_disposedValue = true;
			
			if (disposing)
			{
				_stream?.Dispose();
				_stream = null;
			}
		}
	}

	public void Dispose()
	{
		Dispose(true);
	}

	#endregion IDisposable Support
}