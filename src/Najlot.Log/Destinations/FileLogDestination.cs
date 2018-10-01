using Najlot.Log.Configuration;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Writes all messages to a file.
	/// </summary>
	public class FileLogDestination : LogDestinationBase
	{
		public string FilePath { get; protected set; }
		public Func<string> GetPath { get; protected set; }
		
		public FileLogDestination(ILogConfiguration configuration, Func<string> getPath) : base(configuration)
		{
			GetPath = getPath;

			var path = GetPath();
			EnsureDirectoryExists(path);
			FilePath = path;
		}

		protected override void Log(LogMessage message)
		{
			var path = GetPath();
			
			if (FilePath != path)
			{
				FilePath = path;
				EnsureDirectoryExists(path);
			}

			// Ensure directory is created when the path changes, 
			// but try to create when DirectoryNotFoundException occurs
			// The directory could be deleted by the user in the meantime...
			try
			{
				File.AppendAllText(FilePath, Format(message) + Environment.NewLine);
			}
			catch (DirectoryNotFoundException)
			{
				EnsureDirectoryExists(path);
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
