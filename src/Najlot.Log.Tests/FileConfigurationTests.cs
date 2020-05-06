// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using Najlot.Log.Configuration.FileSource;
using Xunit;
using System.Linq;
using System.IO;
using System;
using System.Threading;

namespace Najlot.Log.Tests
{
	public class FileConfigurationTests
	{
		public FileConfigurationTests()
		{
			foreach (var type in GetType().Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void LogConfigurationsShouldBeRead()
		{
			var configName = nameof(LogConfigurationsShouldBeRead) + ".config";

			if (File.Exists(configName))
			{
				File.Delete(configName);
			}

			using (var admin = LogAdministrator.CreateNew())
			{
				admin
					.AddCustomDestination(new LogDestinationMock(m => { }))
					.SetCollectMiddleware<ConcurrentCollectMiddleware, LogDestinationMock>()
					.AddMiddleware<JsonFormatMiddleware, LogDestinationMock>()
					.ReadConfigurationFromXmlFile(configName, false, true);
			}

			Assert.True(File.Exists(configName));

			using (var admin = LogAdministrator.CreateNew())
			{
				var name = LogConfigurationMapper.Instance.GetName<LogDestinationMock>();
				var concurrentCollectMiddlewareName = LogConfigurationMapper.Instance.GetName<ConcurrentCollectMiddleware>();
				var jsonFormatMiddlewareName = LogConfigurationMapper.Instance.GetName<JsonFormatMiddleware>();

				admin
					.AddCustomDestination(new LogDestinationMock(m => { }))
					.GetLogConfiguration(out var config);

				// Standard should not be ConcurrentCollectMiddleware
				Assert.NotEqual(concurrentCollectMiddlewareName, config.GetCollectMiddlewareName(name));

				admin.ReadConfigurationFromXmlFile(configName, true, false);
				
				Assert.Equal(concurrentCollectMiddlewareName, config.GetCollectMiddlewareName(name));
				Assert.Equal(jsonFormatMiddlewareName, config.GetMiddlewareNames(name).First(n => n == jsonFormatMiddlewareName));
			}
		}

		[Fact]
		public void LogConfigurationsShouldBeListenedTo()
		{
			var configName = nameof(LogConfigurationsShouldBeListenedTo) + ".config";

			if (File.Exists(configName))
			{
				File.Delete(configName);
			}

			using (var admin = LogAdministrator.CreateNew())
			{
				admin
					.AddCustomDestination(new LogDestinationMock(m => { }))
					.SetCollectMiddleware<ConcurrentCollectMiddleware, LogDestinationMock>()
					.AddMiddleware<JsonFormatMiddleware, LogDestinationMock>()
					.ReadConfigurationFromXmlFile(configName, false, true);
			}

			Assert.True(File.Exists(configName));

			using (var admin = LogAdministrator.CreateNew())
			{
				var name = LogConfigurationMapper.Instance.GetName<LogDestinationMock>();
				var concurrentCollectMiddlewareName = LogConfigurationMapper.Instance.GetName<ConcurrentCollectMiddleware>();
				var jsonFormatMiddlewareName = LogConfigurationMapper.Instance.GetName<JsonFormatMiddleware>();
				var formatMiddlewareName = LogConfigurationMapper.Instance.GetName<FormatMiddleware>();

				admin
					.AddCustomDestination(new LogDestinationMock(m => { }))
					.GetLogConfiguration(out var config);

				// Standard should not be ConcurrentCollectMiddleware
				Assert.NotEqual(concurrentCollectMiddlewareName, config.GetCollectMiddlewareName(name));

				admin.ReadConfigurationFromXmlFile(configName, true, false);

				Assert.Equal(concurrentCollectMiddlewareName, config.GetCollectMiddlewareName(name));
				Assert.Equal(jsonFormatMiddlewareName, config.GetMiddlewareNames(name).First(n => n == jsonFormatMiddlewareName));

				var content = File.ReadAllText(configName);
				var syncCollectMiddlewareName = LogConfigurationMapper.Instance.GetName<SyncCollectMiddleware>();

				content = content
					.Replace(concurrentCollectMiddlewareName, syncCollectMiddlewareName)
					.Replace(jsonFormatMiddlewareName, formatMiddlewareName)
					.Replace(LogLevel.Debug.ToString(), LogLevel.Trace.ToString());

				File.WriteAllText(configName, content);

				var endTime = DateTime.Now.AddSeconds(5);

				while (DateTime.Now < endTime && config.GetCollectMiddlewareName(name) == concurrentCollectMiddlewareName)
				{
					Thread.Sleep(25);
				}

				Assert.Equal(syncCollectMiddlewareName, config.GetCollectMiddlewareName(name));
				Assert.Equal(LogLevel.Trace, config.LogLevel);
				Assert.Equal(formatMiddlewareName, config.GetMiddlewareNames(name).First(n => n == formatMiddlewareName));
			}
		}
	}
}