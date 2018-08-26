using System;
using System.Collections.Concurrent;
using System.IO;

namespace NajlotLog.Implementation
{
	internal class FileLoggerImplementation : LoggerImplementationBase
	{
		public object FileLock { get; protected set; }
		public string FilePath { get; protected set; }

		private static ConcurrentDictionary<string, object> FileNameLockDictionary = new ConcurrentDictionary<string, object>();

		public FileLoggerImplementation(string path) : base()
		{
			path = Path.GetFullPath(path);
			
			FileLock = FileNameLockDictionary.GetOrAdd(path, new object());

			var dir = Path.GetDirectoryName(path);

			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			if (!File.Exists(path))
			{
				lock (FileLock)
				{
					File.WriteAllText(path, "");
				}
			}

			FilePath = path;
		}

		protected override void Log(LogMessage message)
		{
			lock (FileLock)
			{
				File.AppendAllText(FilePath, Format(message) + Environment.NewLine);
			}
		}
	}
}
