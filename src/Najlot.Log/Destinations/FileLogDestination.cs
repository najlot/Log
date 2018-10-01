using Najlot.Log.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Writes all messages to a file.
	/// </summary>
	public class FileLogDestination : LogDestinationBase
	{
		private readonly string NewLine = Environment.NewLine;

		public readonly int MaxFiles = 30;
		public readonly string LogFilePaths = null;
		public readonly bool AutoCleanUp;
		
		public string FilePath { get; protected set; }
		public readonly Func<string> GetPath;
		
		public FileLogDestination(ILogConfiguration configuration, Func<string> getPath, int maxFiles, string logFilePaths) : base(configuration)
		{
			GetPath = getPath;
			MaxFiles = maxFiles;
			LogFilePaths = logFilePaths;

			AutoCleanUp = MaxFiles > 0 && !string.IsNullOrWhiteSpace(LogFilePaths);

			var path = GetPath();
			EnsureDirectoryExists(path);
			FilePath = path;
			CleanUpOldFiles(path);
		}
		
		protected override void Log(LogMessage message)
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
				File.AppendAllText(FilePath, Format(message) + NewLine);
				if(cleanUp) CleanUpOldFiles(path);
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
				List<string> LogFilePathsList = null;

				if (!File.Exists(LogFilePaths))
				{
					LogFilePathsList = new List<string>();
				}
				else
				{
					LogFilePathsList = new List<string>(File.ReadAllLines(LogFilePaths));
				}

				LogFilePathsList.Add(path);

				if (LogFilePathsList.Count < MaxFiles)
				{
					File.WriteAllLines(LogFilePaths, LogFilePathsList);
					return;
				}
				
				LogFilePathsList = LogFilePathsList.Where(p =>
				{
					if(string.IsNullOrWhiteSpace(p))
					{
						return false;
					}

					if(!File.Exists(p))
					{
						return false;
					}

					return true;
				}).Distinct().ToList();

				while(LogFilePathsList.Count > MaxFiles)
				{
					File.Delete(LogFilePathsList[0]);
					LogFilePathsList.RemoveAt(0);
				}

				File.WriteAllLines(LogFilePaths, LogFilePathsList);
			}
			catch(Exception ex)
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
	}
}
