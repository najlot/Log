// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests;

public class Configuration_can_append_middlewares
{
	static Configuration_can_append_middlewares()
	{
		LogConfigurationMapper.Instance.AddToMapping<AddToArgsMiddlewareMock1>();
		LogConfigurationMapper.Instance.AddToMapping<AddToArgsMiddlewareMock2>();
		LogConfigurationMapper.Instance.AddToMapping<DestinationMock>();
	}

	[Fact]
	public void Only_one()
	{
		LogMessage message = null;

		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddMiddleware<AddToArgsMiddlewareMock1, DestinationMock>();
		logAdministrator.AddCustomDestination(new DestinationMock((msg) =>
		{
			message = msg;
		}));

		var logger = logAdministrator.GetLogger("");

		logger.Fatal("{Nr}");

		Assert.Equal("1", message.RawArguments[0].ToString());
	}

	[Fact]
	public void Multiple()
	{
		LogMessage message = null;

		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddCustomDestination(new DestinationMock((msg) =>
		{
			message = msg;
		}));

		logAdministrator.AddMiddleware<AddToArgsMiddlewareMock1, DestinationMock>();
		logAdministrator.AddMiddleware<AddToArgsMiddlewareMock2, DestinationMock>();

		var logger = logAdministrator.GetLogger("");

		logger.Fatal("{Nr1}{Nr2}");

		Assert.Equal("1", message.RawArguments[0].ToString());
		Assert.Equal("2", message.RawArguments[1].ToString());
	}

	[Fact]
	public void After_logger_created()
	{
		LogMessage message = null;

		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddCustomDestination(new DestinationMock((msg) =>
		{
			message = msg;
		}));

		var logger = logAdministrator.GetLogger("");

		logAdministrator.AddMiddleware<AddToArgsMiddlewareMock2, DestinationMock>();
		logAdministrator.AddMiddleware<AddToArgsMiddlewareMock1, DestinationMock>();

		logger.Fatal("{Nr2}{Nr1}");

		Assert.Equal("2", message.RawArguments[0].ToString());
		Assert.Equal("1", message.RawArguments[1].ToString());
	}

	[Fact]
	public void Before_destination_added()
	{
		LogMessage message = null;

		using var logAdministrator = LogAdministrator.CreateNew();

		var logger = logAdministrator.GetLogger("");

		logAdministrator.AddMiddleware<AddToArgsMiddlewareMock2, DestinationMock>();
		logAdministrator.AddMiddleware<AddToArgsMiddlewareMock1, DestinationMock>();

		logAdministrator.AddCustomDestination(new DestinationMock((msg) =>
		{
			message = msg;
		}));

		logger.Fatal("{Nr2}{Nr1}");

		Assert.Equal("2", message.RawArguments[0].ToString());
		Assert.Equal("1", message.RawArguments[1].ToString());
	}

	[Fact]
	public void Before_collection_middleware_added()
	{
		LogMessage message = null;

		using (var logAdministrator = LogAdministrator.CreateNew())
		{
			var logger = logAdministrator.GetLogger("");

			logAdministrator.AddMiddleware<AddToArgsMiddlewareMock2, DestinationMock>();
			logAdministrator.AddMiddleware<AddToArgsMiddlewareMock1, DestinationMock>();

			logAdministrator.SetCollectMiddleware<ConcurrentCollectMiddleware, DestinationMock>();

			logAdministrator.AddCustomDestination(new DestinationMock((msg) =>
			{
				message = msg;
			}));

			logger.Fatal("{Nr2}{Nr1}");
		}

		Assert.Equal("2", message.RawArguments[0].ToString());
		Assert.Equal("1", message.RawArguments[1].ToString());
	}

	[Fact]
	public void After_collection_middleware_added()
	{
		LogMessage message = null;

		using (var logAdministrator = LogAdministrator.CreateNew())
		{
			logAdministrator.SetCollectMiddleware<ConcurrentCollectMiddleware, DestinationMock>();

			logAdministrator.AddMiddleware<AddToArgsMiddlewareMock2, DestinationMock>();
			logAdministrator.AddMiddleware<AddToArgsMiddlewareMock1, DestinationMock>();

			logAdministrator.AddCustomDestination(new DestinationMock((msg) =>
			{
				message = msg;
			}));

			logAdministrator.GetLogger("").Fatal("{Nr2}{Nr1}");
		}

		Assert.Equal("2", message.RawArguments[0].ToString());
		Assert.Equal("1", message.RawArguments[1].ToString());
	}

	[Fact]
	public void With_Configuration_File()
	{
		LogMessage message = null;
		const string path = "AddToArgsMiddlewareMock1And2.config";

		using (var logAdministrator = LogAdministrator.CreateNew())
		{
			logAdministrator.SetCollectMiddleware<ConcurrentCollectMiddleware, DestinationMock>();

			logAdministrator.AddMiddleware<AddToArgsMiddlewareMock1, DestinationMock>();
			logAdministrator.AddMiddleware<AddToArgsMiddlewareMock2, DestinationMock>();

			logAdministrator.AddCustomDestination(new DestinationMock((msg) => { }));

			if (File.Exists(path))
			{
				File.Delete(path);
			}

			logAdministrator.ReadConfigurationFromXmlFile(path, false, true);
		}

		using (var logAdministrator = LogAdministrator.CreateNew())
		{
			var logger = logAdministrator.GetLogger("");

			logAdministrator.ReadConfigurationFromXmlFile(path, false, false);

			logAdministrator.AddCustomDestination(new DestinationMock((msg) =>
			{
				message = msg;
			}));

			logger.Fatal("{Nr2}{Nr1}");

			logAdministrator.GetCollectMiddlewareName(nameof(DestinationMock), out var collectMiddlewareName);
			Assert.Equal(nameof(ConcurrentCollectMiddleware), collectMiddlewareName);
		}

		Assert.Equal("1", message.RawArguments[0].ToString());
		Assert.Equal("2", message.RawArguments[1].ToString());
	}
}