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
		public object FileLock { get; protected set; }
		public string FilePath { get; protected set; }
		public Func<string> GetPath { get; protected set; }

		private static ConcurrentDictionary<string, object> FileNameLockDictionary = new ConcurrentDictionary<string, object>();

		public FileLogDestination(ILogConfiguration configuration, Func<string> getPath) : base(configuration)
		{
			GetPath = getPath;

			var path = GetPath();
			
			FileLock = FileNameLockDictionary.GetOrAdd(path, new object());

			var dir = Path.GetDirectoryName(path);

			if(!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			EnsureFileExists(path);

			FilePath = path;
		}

		protected override void Log(LogMessage message)
		{
			var path = GetPath();

			if (FilePath != path)
			{
				FileLock = FileNameLockDictionary.GetOrAdd(path, new object());
				FilePath = path;
				EnsureFileExists(path);
			}

			lock (FileLock)
			{
				File.AppendAllText(FilePath, Format(message) + Environment.NewLine);
			}
		}

		private void EnsureFileExists(string path)
		{
			if (!File.Exists(path))
			{
				lock (FileLock)
				{
					File.WriteAllText(path, "");
				}
			}
		}
	}
}
