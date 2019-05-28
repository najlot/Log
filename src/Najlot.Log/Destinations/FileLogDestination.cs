using Najlot.Log.Middleware;
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
	[LogConfigurationName(nameof(FileLogDestination))]
	public sealed class FileLogDestination : ILogDestination
	{
		private readonly string NewLine = Environment.NewLine;
		private Stream _stream = null;
		private readonly Encoding _encoding = Encoding.UTF8;

		public readonly int MaxFiles;
		public readonly string LogFilePaths = null;
		public readonly bool AutoCleanUp;
		public readonly bool KeepFileOpen;
		public readonly Func<string> GetPath;

		public string FilePath { get; private set; }

		public FileLogDestination(Func<string> getPath, int maxFiles, string logFilePaths, bool keepFileOpen)
		{
			KeepFileOpen = keepFileOpen;
			GetPath = getPath;
			MaxFiles = maxFiles;
			LogFilePaths = logFilePaths;

			AutoCleanUp = MaxFiles > 0 && !string.IsNullOrWhiteSpace(LogFilePaths);

			var path = GetPath();
			EnsureDirectoryExists(path);
			FilePath = path;

			if (KeepFileOpen)
			{
				SetStream(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough));
			}

			if (AutoCleanUp) CleanUpOldFiles(path);
		}

		public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
		{
			Log(messages, formatMiddleware, true);
		}

		public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware, bool logRetry)
		{
			bool cleanUp = false;
			var path = GetPath();

			// Ensure directory is created when the path changes,
			// but try to create when DirectoryNotFoundException occurs.
			// The directory could be deleted by the user in the meantime...
			try
			{
				if (FilePath != path)
				{
					FilePath = path;
					EnsureDirectoryExists(path);
					if (AutoCleanUp) cleanUp = true;

					if (KeepFileOpen)
					{
						SetStream(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.WriteThrough));
					}
				}

				var sb = new StringBuilder();

				foreach (var message in messages)
				{
					sb.Append(formatMiddleware.Format(message) + NewLine);
				}

				if (KeepFileOpen)
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
					Log(messages, formatMiddleware, false);
				}
			}
		}

		private void CleanUpOldFiles(string path)
		{
			try
			{
				List<string> logFilePathsList = null;

				if (File.Exists(LogFilePaths))
				{
					logFilePathsList = new List<string>(File.ReadAllLines(LogFilePaths));
				}
				else
				{
					logFilePathsList = new List<string>();
				}

				logFilePathsList.Add(path);

				if (logFilePathsList.Count < MaxFiles)
				{
					File.WriteAllLines(LogFilePaths, logFilePathsList.Distinct());
					return;
				}

				logFilePathsList = logFilePathsList
					.Where(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p))
					.Distinct().ToList();

				while (logFilePathsList.Count > MaxFiles)
				{
					var file = logFilePathsList[0];
					logFilePathsList.Remove(file);
					File.WriteAllLines(LogFilePaths, logFilePathsList);
					File.Delete(file);
				}

				File.WriteAllLines(LogFilePaths, logFilePathsList);
			}
			catch (Exception ex)
			{
				Console.Write("Najlot.Log.Destinations.FileLogDestination (CleanUpOldFiles): ");

				while (ex != null)
				{
					Console.WriteLine($"{ex}");
					ex = ex.InnerException;
				}
			}
		}

		private void EnsureDirectoryExists(string path)
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