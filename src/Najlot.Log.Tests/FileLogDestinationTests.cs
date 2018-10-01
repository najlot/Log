
using Najlot.Log.Middleware;
using System;
using System.Linq;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests
{
	public class FileLogDestinationTests
	{
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

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(() =>
				{
					var fileInfo = new FileInfo(fileName1);

					if(fileInfo.Exists)
					{
						if (fileInfo.Length > 0)
						{
							return fileName2;
						}
					}
					
					return fileName1;
				})
				.GetLoggerPool(out LoggerPool loggerPool);

			var logForThis = loggerPool.GetLogger(this.GetType());
			var logForPool = loggerPool.GetLogger(loggerPool.GetType());

			var contentThis = "logForThis . Info";
			var contentPool = "logForPool.Warn";

			logForThis.Info(contentThis);
			logForPool.Warn(contentPool);

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

			if(File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(fileName)
				.GetLoggerPool(out LoggerPool loggerPool);

			var logForThis = loggerPool.GetLogger(this.GetType());
			var logForPool = loggerPool.GetLogger(loggerPool.GetType());

			var contentThis = "logForThis . Info";
			var contentPool = "logForPool.Warn";

			logForThis.Info(contentThis);
			logForPool.Warn(contentPool);

			Assert.True(File.Exists(fileName), $"File {fileName} not found.");

			var content = File.ReadAllText(fileName);

			Assert.NotEqual(-1, content.IndexOf(contentThis));
			Assert.NotEqual(-1, content.IndexOf(contentPool));
		}

		[Fact]
		public void FileLoggerMustCreateDirectory()
		{
			var dir = $"logs_for_{nameof(FileLoggerMustCreateDirectory)}";
			var fileName = Path.Combine(dir, "TestFile.log");
			
			if(Directory.Exists(dir))
			{
				Directory.Delete(dir, true);
			}

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(fileName)
				.GetLoggerPool(out LoggerPool loggerPool);

			var logForThis = loggerPool.GetLogger(this.GetType());
			var logForPool = loggerPool.GetLogger(loggerPool.GetType());

			var contentThis = "logForThis . Info";
			var contentPool = "logForPool.Warn";

			logForThis.Info(contentThis);
			logForPool.Warn(contentPool);

			Assert.True(File.Exists(fileName), $"File {fileName} not found.");

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

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(fileName)
				.GetLoggerPool(out LoggerPool loggerPool);

			var logger = loggerPool.GetLogger(nameof(FileLoggerMustRecreateDirectory));
			
			logger.Info("...");

			Directory.Delete(dir, true);

			logger.Info("This must recreate the directory.");

			Assert.True(Directory.Exists(dir));
		}

		[Fact]
		public void FileLoggerMustCleanUpFiles()
		{
			const int maxFiles = 5;
			const string logFilePaths = ".FilesToCleanUp";
			string logsDir = $"logs_for_{nameof(FileLoggerMustCleanUpFiles)}";
			
			if (File.Exists(logFilePaths)) File.Delete(logFilePaths);
			if (Directory.Exists(logsDir)) Directory.Delete(logsDir, true);

			int i = 0;

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddFileLogDestination(() => Path.Combine(logsDir, i.ToString()), maxFiles: maxFiles, logFilePaths: logFilePaths)
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());

			for (; i < 100; i++)
			{
				log.Info(Guid.NewGuid());
			}

			var files = Directory.GetFiles(logsDir);

			Assert.Equal(maxFiles, files.Length);

			foreach(var file in files.Select(p => Path.GetFileNameWithoutExtension(p)))
			{
				Assert.True(int.Parse(file) > 99 - maxFiles);
			}
		}
	}
}
