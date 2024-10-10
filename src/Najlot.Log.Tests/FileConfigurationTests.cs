// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace Najlot.Log.Tests;

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
				.AddCustomDestination(new DestinationMock(m => { }))
				.SetCollectMiddleware<ConcurrentCollectMiddleware, DestinationMock>()
				.AddMiddleware<JsonFormatMiddleware, DestinationMock>()
				.ReadConfigurationFromXmlFile(configName, false, true);
		}

		Assert.True(File.Exists(configName));

		using (var admin = LogAdministrator.CreateNew())
		{
			var name = LogConfigurationMapper.Instance.GetName<DestinationMock>();
			var concurrentCollectMiddlewareName = LogConfigurationMapper.Instance.GetName<ConcurrentCollectMiddleware>();
			var jsonFormatMiddlewareName = LogConfigurationMapper.Instance.GetName<JsonFormatMiddleware>();

			admin
				.AddCustomDestination(new DestinationMock(m => { }))
				.GetCollectMiddlewareName(name, out var collectMiddlewareName);

			// Standard should not be ConcurrentCollectMiddleware
			Assert.NotEqual(concurrentCollectMiddlewareName, collectMiddlewareName);

			admin
				.ReadConfigurationFromXmlFile(configName, true, false)
				.GetCollectMiddlewareName(name, out collectMiddlewareName)
				.GetMiddlewareNames(name, out var middlewareNames);

			Assert.Equal(concurrentCollectMiddlewareName, collectMiddlewareName);
			Assert.Equal(jsonFormatMiddlewareName, middlewareNames.First(n => n == jsonFormatMiddlewareName));
		}
	}

	[Fact]
	public void LogConfigurationsShouldBeListenedTo()
	{
		const string configName = nameof(LogConfigurationsShouldBeListenedTo) + ".config";

		if (File.Exists(configName))
		{
			File.Delete(configName);
		}

		using (var admin = LogAdministrator.CreateNew())
		{
			admin
				.AddCustomDestination(new DestinationMock(m => { }))
				.SetCollectMiddleware<ConcurrentCollectMiddleware, DestinationMock>()
				.AddMiddleware<JsonFormatMiddleware, DestinationMock>()
				.ReadConfigurationFromXmlFile(configName, false, true);
		}

		Assert.True(File.Exists(configName));

		using (var admin = LogAdministrator.CreateNew())
		{
			var name = LogConfigurationMapper.Instance.GetName<DestinationMock>();
			var concurrentCollectMiddlewareName = LogConfigurationMapper.Instance.GetName<ConcurrentCollectMiddleware>();
			var jsonFormatMiddlewareName = LogConfigurationMapper.Instance.GetName<JsonFormatMiddleware>();
			var formatMiddlewareName = LogConfigurationMapper.Instance.GetName<FormatMiddleware>();

			admin
				.AddCustomDestination(new DestinationMock(m => { }))
				.GetCollectMiddlewareName(name, out var collectMiddlewareName);

			// Standard should not be ConcurrentCollectMiddleware
			Assert.NotEqual(concurrentCollectMiddlewareName, collectMiddlewareName);

			admin
				.ReadConfigurationFromXmlFile(configName, true, false)
				.GetCollectMiddlewareName(name, out collectMiddlewareName)
				.GetMiddlewareNames(name, out var middlewareName);

			Assert.Equal(concurrentCollectMiddlewareName, collectMiddlewareName);
			Assert.Equal(jsonFormatMiddlewareName, middlewareName.First(n => n == jsonFormatMiddlewareName));

			var content = File.ReadAllText(configName);
			var syncCollectMiddlewareName = LogConfigurationMapper.Instance.GetName<SyncCollectMiddleware>();

			content = content
				.Replace(concurrentCollectMiddlewareName, syncCollectMiddlewareName)
				.Replace(jsonFormatMiddlewareName, formatMiddlewareName)
				.Replace(LogLevel.Debug.ToString(), LogLevel.Trace.ToString());

			File.WriteAllText(configName, content);

			var endTime = DateTime.Now.AddSeconds(5);

			while (DateTime.Now < endTime
				&& admin.GetCollectMiddlewareName(name, out collectMiddlewareName) != null
				&& collectMiddlewareName  == concurrentCollectMiddlewareName)
			{
				Thread.Sleep(25);
			}

			admin
				.GetCollectMiddlewareName(name, out collectMiddlewareName)
				.GetLogLevel(out var logLevel)
				.GetMiddlewareNames(name, out var middlewareNames);

			Assert.Equal(syncCollectMiddlewareName, collectMiddlewareName);
			Assert.Equal(LogLevel.Trace, logLevel);
			Assert.Equal(formatMiddlewareName, middlewareNames.First(n => n == formatMiddlewareName));
		}
	}
}