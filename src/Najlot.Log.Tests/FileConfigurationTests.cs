﻿
using Xunit;
using Najlot.Log.Middleware;
using Najlot.Log.Configuration.FileSource;
using System.IO;
using Najlot.Log.Configuration;
using System.Diagnostics;
using System.Threading;

namespace Najlot.Log.Tests
{
	public class FileConfigurationTests
	{
		[Fact]
		public void FileConfigurationMustBeRead()
		{
			const string configPath = "LogConfigurationToRead.config";
			var content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<NajlotLogConfiguration xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
					"<LogLevel>Error</LogLevel>" +
					"<ExecutionMiddleware>Najlot.Log.Middleware.TaskExecutionMiddleware, Najlot.Log</ExecutionMiddleware>" +
				"</NajlotLogConfiguration>";

			if(File.Exists(configPath))
			{
				File.Delete(configPath);
			}

			File.WriteAllText(configPath, content);

			LogConfigurator
				.CreateNew()
				.ReadConfigurationFromXmlFile(configPath, listenForChanges: false, writeExampleIfSourceDoesNotExists: true)
				.GetLogConfiguration(out ILogConfiguration logConfiguration);
			
			Assert.Equal(LogLevel.Error, logConfiguration.LogLevel);
			Assert.Equal(typeof(TaskExecutionMiddleware).FullName, logConfiguration.ExecutionMiddleware.GetType().FullName);
		}

		[Fact]
		public void FileConfigurationMustUpdateAtRuntime()
		{
			const string configPath = "LogConfigurationToUpdate.config";

			var content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<NajlotLogConfiguration xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
					"<LogLevel>Error</LogLevel>" +
					"<ExecutionMiddleware>Najlot.Log.Middleware.TaskExecutionMiddleware, Najlot.Log</ExecutionMiddleware>" +
				"</NajlotLogConfiguration>";

			var newContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<NajlotLogConfiguration xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
					"<LogLevel>Info</LogLevel>" +
					"<ExecutionMiddleware>Najlot.Log.Middleware.SyncExecutionMiddleware, Najlot.Log</ExecutionMiddleware>" +
				"</NajlotLogConfiguration>";

			if (File.Exists(configPath))
			{
				File.Delete(configPath);
			}

			File.WriteAllText(configPath, content);

			LogConfigurator
				.CreateNew()
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.SetLogLevel(LogLevel.Trace)
				.ReadConfigurationFromXmlFile(configPath, listenForChanges: true, writeExampleIfSourceDoesNotExists: true)
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			Assert.Equal(LogLevel.Error, logConfiguration.LogLevel);
			Assert.Equal(typeof(TaskExecutionMiddleware).FullName, logConfiguration.ExecutionMiddleware.GetType().FullName);

			// Write new configuration, wait until it changes and fail if it does not
			File.WriteAllText(configPath, newContent);
			var stopwatch = Stopwatch.StartNew();

			while(stopwatch.ElapsedMilliseconds < 5000 && logConfiguration.LogLevel == LogLevel.Error)
			{
				Thread.Sleep(10);
			}

			stopwatch.Stop();

			Assert.Equal(LogLevel.Info, logConfiguration.LogLevel);
			Assert.Equal(typeof(SyncExecutionMiddleware).FullName, logConfiguration.ExecutionMiddleware.GetType().FullName);
		}

		[Fact]
		public void ExampleMustBeWritten()
		{
			const string configPath = "LogConfigurationExamle.config";

			if(File.Exists(configPath))
			{
				File.Delete(configPath);
			}

			LogConfigurator
				.CreateNew()
				.SetLogLevel(LogLevel.Info)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.ReadConfigurationFromXmlFile(configPath, listenForChanges: false, writeExampleIfSourceDoesNotExists: true);

			Assert.True(File.Exists(configPath));
		}
	}
}