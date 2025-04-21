// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
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
	public void XmlLogConfigurationsShouldBeRead()
	{
		var configName = nameof(XmlLogConfigurationsShouldBeRead) + ".config";

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
	public void XmlLogConfigurationsShouldBeListenedTo()
	{
		const string configName = nameof(XmlLogConfigurationsShouldBeListenedTo) + ".config";

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
				&& collectMiddlewareName == concurrentCollectMiddlewareName)
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

	[Fact]
	public void JsonLogConfigurationsShouldBeRead()
	{
		var configName = nameof(JsonLogConfigurationsShouldBeRead) + ".json";

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
				.ReadConfigurationFromJsonFile(configName, false, true);
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
				.ReadConfigurationFromJsonFile(configName, true, false)
				.GetCollectMiddlewareName(name, out collectMiddlewareName)
				.GetMiddlewareNames(name, out var middlewareNames);

			Assert.Equal(concurrentCollectMiddlewareName, collectMiddlewareName);
			Assert.Equal(jsonFormatMiddlewareName, middlewareNames.First(n => n == jsonFormatMiddlewareName));
		}
	}

	[Fact]
	public void JsonLogConfigurationsShouldBeListenedTo()
	{
		const string configName = nameof(JsonLogConfigurationsShouldBeListenedTo) + ".json";

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
				.ReadConfigurationFromJsonFile(configName, false, true);
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
				.ReadConfigurationFromJsonFile(configName, true, false)
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
				&& collectMiddlewareName == concurrentCollectMiddlewareName)
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

	[Fact]
	public void ExtensionsConfiguration_should_be_read()
	{
		var builder = new ConfigurationBuilder();
		var configString = @"
		{
			""NajlotLog"":
			{
				""LogLevel"": ""Debug"",
				""Destinations"": [
					{
						""Name"": ""DestinationMock"",
						""Parameters"": {},
						""CollectMiddleware"": {
							""Name"": ""ConcurrentCollectMiddleware""
						},
						""Middlewares"": [
							{
								""Name"": ""JsonFormatMiddleware""
							}
						]
					}
				]
			}
		}";

		using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(configString));
		builder.AddJsonStream(stream);
		var config = builder.Build();

		using var admin = LogAdministrator.CreateNew()
			.AddCustomDestination(new DestinationMock(m => { }))
			.SetCollectMiddleware<SyncCollectMiddleware, DestinationMock>()
			.AddMiddleware<FormatMiddleware, DestinationMock>()
			.ReadConfiguration(config);

		var name = LogConfigurationMapper.Instance.GetName<DestinationMock>();
		var concurrentCollectMiddlewareName = LogConfigurationMapper.Instance.GetName<ConcurrentCollectMiddleware>();
		var jsonFormatMiddlewareName = LogConfigurationMapper.Instance.GetName<JsonFormatMiddleware>();

		admin
			.GetCollectMiddlewareName(name, out var collectMiddlewareName)
			.GetMiddlewareNames(name, out var middlewareNames)
			.GetLogLevel(out var logLevel);

		Assert.Equal(concurrentCollectMiddlewareName, collectMiddlewareName);
		Assert.Equal(jsonFormatMiddlewareName, middlewareNames.First(n => n == jsonFormatMiddlewareName));
		Assert.Equal(LogLevel.Debug, logLevel);
	}
}