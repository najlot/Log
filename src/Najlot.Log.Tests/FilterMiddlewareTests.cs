﻿using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests
{
	public class FilterMiddlewareTests
	{
		[Fact]
		public void FilterMiddlewareCanBlockMessages()
		{
			var fileName = nameof(FilterMiddlewareCanBlockMessages) + ".log";

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			var log = LogAdminitrator
				.CreateNew()
				.AddFileLogDestination(fileName, keepFileOpen: false)
				.SetFilterMiddleware<DenyAllFilterMiddleware>()
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

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.AddFileLogDestination(fileName, keepFileOpen: false)
				.SetFilterMiddleware<DenyAllFilterMiddleware>();

			var log = logAdminitrator.GetLogger("");

			log.Fatal("TEST!");

			Assert.False(File.Exists(fileName));

			logAdminitrator.SetFilterMiddleware<OpenFilterMiddleware>();

			log.Fatal("TEST!");

			Assert.True(File.Exists(fileName));
		}

		[Fact]
		public void FilterMiddlewareGetsCorrectType()
		{
			bool loggedToFirst = false;
			bool loggedToSecond = false;

			var log = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					loggedToFirst = true;
				}))
				.AddCustomDestination(new SecondLogDestinationMock(msg =>
				{
					loggedToSecond = true;
				}))
				.SetFilterMiddleware<DenyLogDestinationMockFilterMiddleware>()
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

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.AddFileLogDestination(fileName, keepFileOpen: false)
				.SetFilterMiddleware<DenyAllFilterMiddleware>();

			var log = logAdminitrator.GetLogger("");

			log.Debug("TEST 1!");

			Assert.False(File.Exists(fileName));

			logAdminitrator.SetFilterMiddlewareByType(null);

			log.Debug("TEST 2!");

			Assert.False(File.Exists(fileName));

			logAdminitrator.SetFilterMiddlewareByType(typeof(string));

			log.Debug("TEST 3!");

			Assert.False(File.Exists(fileName));
		}
	}
}