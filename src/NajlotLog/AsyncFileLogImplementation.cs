using System.IO;

namespace NajlotLog
{
	internal class AsyncFileLogImplementation : AsyncLogImplementationBase, ILog
	{
		private static object FileLock = new object();
		private string FilePath;

		public AsyncFileLogImplementation(string path)
		{
			path = Path.GetFullPath(path);

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

		protected override void Log(string msg)
		{
			lock (FileLock)
			{
				File.AppendAllText(FilePath, msg);
			}
		}
	}
}
