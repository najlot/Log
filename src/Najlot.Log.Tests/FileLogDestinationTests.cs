using Najlot.Log.Middleware;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Najlot.Log.Tests
{
	public class FileLogDestinationTests
	{
		[Fact]
		public void FileLoggerMustCleanUpFiles()
		{
			const int maxFiles = 5;
			const string logFilePaths = ".FilesToCleanUp";
			string logsDir = $"logs_for_{nameof(FileLoggerMustCleanUpFiles)}";

			if (File.Exists(logFilePaths)) File.Delete(logFilePaths);
			if (Directory.Exists(logsDir)) Directory.Delete(logsDir, true);

			int i = 0;

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(() => Path.Combine(logsDir, i.ToString()), maxFiles: maxFiles, logFilePaths: logFilePaths);

			var log = logAdminitrator.GetLogger(this.GetType());

			for (; i < 100; i++)
			{
				// Add some bad data
				if (i == 50)
				{
					File.AppendAllText(logFilePaths,
						Environment.NewLine + " " +
						Environment.NewLine + "not-existing-file.log" +
						Environment.NewLine + new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath +
						Environment.NewLine);
				}

				log.Info(Guid.NewGuid());
			}

			var files = Directory.GetFiles(logsDir);

			Assert.Equal(maxFiles, files.Length);

			foreach (var file in files.Select(p => Path.GetFileNameWithoutExtension(p)))
			{
				Assert.True(int.Parse(file) > 99 - maxFiles);
			}
		}

		[Fact]
		public void FileLoggerMustCreateDirectory()
		{
			var dir = $"logs_for_{nameof(FileLoggerMustCreateDirectory)}";
			var fileName = Path.Combine(dir, "TestFile.log");

			if (Directory.Exists(dir))
			{
				Directory.Delete(dir, true);
			}

			var contentThis = "logForThis . Info";
			var contentPool = "logForPool.Warn";

			using (var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(fileName))
			{
				var logForThis = logAdminitrator.GetLogger(this.GetType());
				var logForPool = logAdminitrator.GetLogger(logAdminitrator.GetType());
				
				logForThis.Info(contentThis);
				logForPool.Warn(contentPool);

				Assert.True(File.Exists(fileName), $"File {fileName} not found.");
			}
			
			var content = File.ReadAllText(fileName);

			Assert.NotEqual(-1, content.IndexOf(contentThis));
			Assert.NotEqual(-1, content.IndexOf(contentPool));
		}

		[Fact]
		public void FileLoggerMustRecreateDirectory()
		{
			var dir = $"logs_for_{nameof(FileLoggerMustRecreateDirectory)}";
			var fileName = Path.Combine(dir, "TestFile.log");

			if (Directory.Exists(dir))
			{
				Directory.Delete(dir, true);
			}

			using (var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(fileName))
			{
				var logger = logAdminitrator.GetLogger(nameof(FileLoggerMustRecreateDirectory));

				logger.Info("This must create the directory.");

				Assert.True(Directory.Exists(dir));
			}
		}

		[Fact]
		public void FileLoggerPrototypesMustWriteToDifferentFilesInDifferentDirectories()
		{
			var dir1 = "logs_dir_1";
			var dir2 = "logs_dir_2";

			var fileName1 = Path.Combine(dir1, "TestDifferentFile1.log");
			var fileName2 = Path.Combine(dir2, "TestDifferentFile2.log");

			if (Directory.Exists(dir1))
			{
				Directory.Delete(dir1, true);
			}

			if (Directory.Exists(dir2))
			{
				Directory.Delete(dir2, true);
			}

			var contentThis = "logForThis . Info";
			var contentPool = "logForPool.Warn";

			bool logged = false;

			using (var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(() =>
				{
					var fileInfo = new FileInfo(fileName1);

					if (logged)
					{
						return fileName2;
					}

					return fileName1;
				}))
			{
				var logForThis = logAdminitrator.GetLogger(this.GetType());
				var logForPool = logAdminitrator.GetLogger(logAdminitrator.GetType());
				
				logForThis.Info(contentThis);
				logged = true;
				logForPool.Warn(contentPool);
			}
			
			Assert.True(File.Exists(fileName1), $"File {fileName1} not found.");
			Assert.True(File.Exists(fileName2), $"File {fileName2} not found.");

			var content1 = File.ReadAllText(fileName1);
			var content2 = File.ReadAllText(fileName2);

			Assert.NotEqual(-1, content1.IndexOf(contentThis));
			Assert.NotEqual(-1, content2.IndexOf(contentPool));
		}

		[Fact]
		public void FileLoggerPrototypesMustWriteToSameFile()
		{
			var fileName = "TestFile.log";

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			var contentThis = "logForThis . Info";
			var contentPool = "logForPool.Warn";

			using (var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(fileName))
			{
				var logForThis = logAdminitrator.GetLogger(this.GetType());
				var logForPool = logAdminitrator.GetLogger(logAdminitrator.GetType());
				
				logForThis.Info(contentThis);
				logForPool.Warn(contentPool);
			}
			
			Assert.True(File.Exists(fileName), $"File {fileName} not found.");

			var content = File.ReadAllText(fileName);

			Assert.NotEqual(-1, content.IndexOf(contentThis));
			Assert.NotEqual(-1, content.IndexOf(contentPool));
		}
	}
}