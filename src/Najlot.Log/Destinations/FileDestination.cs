// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Writes all messages to a file.
	/// </summary>
	[LogConfigurationName(nameof(FileDestination))]
	public sealed class FileDestination : IDestination
	{
		private readonly string _newLine = Environment.NewLine;
		private Stream _stream = null;
		private readonly Encoding _encoding = Encoding.UTF8;

		private readonly int _maxFiles;
		private readonly string _logFilePaths = null;
		private readonly bool _autoCleanUp;
		private readonly bool _keepFileOpen;
		private readonly Func<string> _getPath;

		public string FilePath { get; private set; }

		public FileDestination(Func<string> getPath, int maxFiles, string logFilePaths, bool keepFileOpen)
		{
			_keepFileOpen = keepFileOpen;
			_getPath = getPath ?? throw new NullReferenceException();
			_maxFiles = maxFiles;
			_logFilePaths = logFilePaths;

			_autoCleanUp = _maxFiles > 0 && !string.IsNullOrWhiteSpace(_logFilePaths);

			var path = _getPath();
			EnsureDirectoryExists(path);
			FilePath = path;

			if (_keepFileOpen)
			{
				SetStream(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough));
			}

			if (_autoCleanUp) CleanUpOldFiles(path);
		}

		public void Log(IEnumerable<LogMessage> messages)
		{
			Log(messages, true);
		}

		public void Log(IEnumerable<LogMessage> messages, bool logRetry)
		{
			bool cleanUp = false;
			var path = _getPath();

			// Ensure directory is created when the path changes,
			// but try to create when DirectoryNotFoundException occurs.
			// The directory could be deleted by the user in the meantime...
			try
			{
				if (FilePath != path)
				{
					FilePath = path;
					EnsureDirectoryExists(path);
					if (_autoCleanUp) cleanUp = true;

					if (_keepFileOpen)
					{
						SetStream(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough));
					}
				}

				var sb = new StringBuilder();

				foreach (var message in messages)
				{
					sb.Append(message.Message);
					sb.Append(_newLine);
				}

				if (_keepFileOpen)
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

			if (File.Exists(_logFilePaths))
			{
				logFilePathsList = new List<string>(File.ReadAllLines(_logFilePaths));
			}
			else
			{
				logFilePathsList = new List<string>();
			}

			logFilePathsList.Add(path);

			if (logFilePathsList.Count < _maxFiles)
			{
				File.WriteAllLines(_logFilePaths, logFilePathsList.Distinct());
				return;
			}

			logFilePathsList = logFilePathsList
				.Where(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p))
				.Distinct().ToList();

			while (logFilePathsList.Count > _maxFiles)
			{
				var file = logFilePathsList[0];
				logFilePathsList.Remove(file);
				File.WriteAllLines(_logFilePaths, logFilePathsList);
				if (File.Exists(file)) File.Delete(file);
			}

			File.WriteAllLines(_logFilePaths, logFilePathsList);
		}

		private static void EnsureDirectoryExists(string path)
		{
			if (!File.Exists(path))
			{
				var dir = Path.GetDirectoryName(path);

				if (!string.IsNullOrWhiteSpace(dir))
				{
					Directory.CreateDirectory(dir);
				}
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

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		public void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_stream?.Dispose();
					_stream = null;
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}