using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Writes all messages to a file.
	/// </summary>
	public sealed class FileLogDestination : ILogDestination
	{
		private readonly string NewLine = Environment.NewLine;

		public readonly int MaxFiles;
		public readonly string LogFilePaths = null;
		public readonly bool AutoCleanUp;

		public string FilePath { get; private set; }
		public readonly Func<string> GetPath;

		public FileLogDestination(Func<string> getPath, int maxFiles, string logFilePaths)
		{
			GetPath = getPath;
			MaxFiles = maxFiles;
			LogFilePaths = logFilePaths;

			AutoCleanUp = MaxFiles > 0 && !string.IsNullOrWhiteSpace(LogFilePaths);

			var path = GetPath();
			EnsureDirectoryExists(path);
			FilePath = path;
			if (AutoCleanUp) CleanUpOldFiles(path);
		}

		public void Log(LogMessage message, Func<LogMessage, string> formatFunc)
		{
			var path = GetPath();
			bool cleanUp = false;

			if (FilePath != path)
			{
				FilePath = path;
				EnsureDirectoryExists(path);
				if (AutoCleanUp) cleanUp = true;
			}

			// Ensure directory is created when the path changes,
			// but try to create when DirectoryNotFoundException occurs
			// The directory could be deleted by the user in the meantime...
			try
			{
				File.AppendAllText(FilePath, formatFunc(message) + NewLine);
				if (cleanUp) CleanUpOldFiles(path);
			}
			catch (DirectoryNotFoundException)
			{
				EnsureDirectoryExists(path);
			}
		}

		private void CleanUpOldFiles(string path)
		{
			try
			{
				List<string> logFilePathsList = null;

				if (!File.Exists(LogFilePaths))
				{
					logFilePathsList = new List<string>();
				}
				else
				{
					logFilePathsList = new List<string>(File.ReadAllLines(LogFilePaths));
				}

				logFilePathsList.Add(path);

				if (logFilePathsList.Count < MaxFiles)
				{
					File.WriteAllLines(LogFilePaths, logFilePathsList.Distinct());
					return;
				}

				logFilePathsList = logFilePathsList.Where(p =>
				{
					if (string.IsNullOrWhiteSpace(p))
					{
						return false;
					}

					if (!File.Exists(p))
					{
						return false;
					}

					return true;
				}).Distinct().ToList();

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

		public void Dispose()
		{
		}
	}
}