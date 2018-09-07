using NajlotLog.Configuration;
using NajlotLog.Middleware;
using System;
using System.IO;
using Xunit;

namespace NajlotLog.Tests
{
	public class FileLogDestinationTests
	{
		[Fact]
		public void FileLoggerPrototypesMustWriteToDifferentFile()
		{
			var fileName1 = "TestDifferentFile1.log";
			var fileName2 = "TestDifferentFile2.log";

			if (File.Exists(fileName1))
			{
				File.Delete(fileName1);
			}

			if (File.Exists(fileName2))
			{
				File.Delete(fileName2);
			}

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
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
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
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
	}
}
