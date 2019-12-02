// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests
{
	public class FilterMiddlewareTests
	{
		public FilterMiddlewareTests()
		{
			foreach (var type in typeof(FilterMiddlewareTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void FilterMiddlewareCanBlockMessages()
		{
			var fileName = nameof(FilterMiddlewareCanBlockMessages) + ".log";

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			var log = LogAdministrator
				.CreateNew()
				.AddFileLogDestination(fileName, keepFileOpen: false)
				.SetFilterMiddleware<DenyAllFilterMiddleware>(nameof(FileLogDestination))
				.GetLogger("");

			log.Fatal("TEST!");

			Assert.False(File.Exists(fileName));
		}

		[Fact]
		public void FilterMiddlewareCanBeChanged()
		{
			var fileName = nameof(FilterMiddlewareCanBeChanged) + ".log";

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			var logAdminitrator = LogAdministrator
				.CreateNew()
				.AddFileLogDestination(fileName, keepFileOpen: false)
				.SetFilterMiddleware<DenyAllFilterMiddleware>(nameof(FileLogDestination));

			var log = logAdminitrator.GetLogger("");

			log.Fatal("TEST!");

			Assert.False(File.Exists(fileName));

			logAdminitrator.SetFilterMiddleware<NoFilterMiddleware>(nameof(FileLogDestination));

			log.Fatal("TEST!");

			Assert.True(File.Exists(fileName));
		}

		[Fact]
		public void FilterMiddlewareGetsCorrectType()
		{
			bool loggedToFirst = false;
			bool loggedToSecond = false;

			var log = LogAdministrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					loggedToFirst = true;
				}))
				.AddCustomDestination(new SecondLogDestinationMock(msg =>
				{
					loggedToSecond = true;
				}))
				.SetFilterMiddleware<DenyAllFilterMiddleware>(nameof(LogDestinationMock))
				.GetLogger("");

			log.Fatal("TEST!");

			Assert.False(loggedToFirst);
			Assert.True(loggedToSecond);
		}

		[Fact]
		public void LoggerShouldNotDieBecauseOfFilterMiddleware()
		{
			var fileName = nameof(LoggerShouldNotDieBecauseOfFilterMiddleware) + ".log";

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			var logAdminitrator = LogAdministrator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.AddFileLogDestination(fileName, keepFileOpen: false)
				.SetFilterMiddleware<DenyAllFilterMiddleware>(nameof(FileLogDestination));

			var log = logAdminitrator.GetLogger("");

			log.Debug("TEST 1!");

			Assert.False(File.Exists(fileName));

			logAdminitrator.SetFilterMiddleware<DenyAllFilterMiddleware>(nameof(System.String));

			log.Debug("TEST 2!");

			Assert.False(File.Exists(fileName));
		}
	}
}