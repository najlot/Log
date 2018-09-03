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
