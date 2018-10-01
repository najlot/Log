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
			EnsureFileExists(path);
			FilePath = path;
		}

		protected override void Log(LogMessage message)
		{
			var path = GetPath();

			if (FilePath != path)
			{
				FilePath = path;
				EnsureFileExists(path);
			}
			
			File.AppendAllText(FilePath, Format(message) + Environment.NewLine);
		}

		private void EnsureFileExists(string path)
		{
			if (!File.Exists(path))
			{
				var dir = Path.GetDirectoryName(path);

				if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

				File.WriteAllText(path, "");
			}
		}
	}
}
