// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class FormatMiddlewareTests
	{
		public FormatMiddlewareTests()
		{
			foreach (var type in typeof(FormatMiddlewareTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void ConfiguratorMustNotAcceptNullDestinations()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				LogAdminitrator.CreateNew().AddCustomDestination(null);
			});
		}

		[Fact]
		public void FormatMiddlewareCanBeChangedAfterCreation()
		{
			var strExpected = "Abc";
			var strActual = "";

			var logAdmin = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationFormatFunctionMock(str =>
				{
					strActual = str;
				}));

			var log = logAdmin.GetLogger(this.GetType());

			var name = LogConfigurationMapper.Instance.GetName<LogDestinationFormatFunctionMock>();
			logAdmin.SetFormatMiddleware<FormatToAbcMiddleware>(name);

			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void FormatMiddlewareCanBeChangedAfterRegistration()
		{
			var strExpected = "Abc";
			var strActual = "";

			var logAdmin = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationFormatFunctionMock(str =>
				{
					strActual = str;
				}));

			var name = LogConfigurationMapper.Instance.GetName<LogDestinationFormatFunctionMock>();
			logAdmin.SetFormatMiddleware<FormatToAbcMiddleware>(name);

			var log = logAdmin.GetLogger(this.GetType());

			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void FormatMiddlewareCanBeSetOnRegistration()
		{
			var strExpected = "Abc";
			var strActual = "";

			var name = LogConfigurationMapper.Instance.GetName<LogDestinationFormatFunctionMock>();

			var logAdmin = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationFormatFunctionMock(str =>
				{
					strActual = str;
				}))
				.SetFormatMiddleware<FormatToAbcMiddleware>(name);

			var log = logAdmin.GetLogger(this.GetType());
			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void SameFormatMiddlewareCanBeSetToTwoDestinations()
		{
			var strActual = "";
			var strActual2 = "";

			var name = LogConfigurationMapper.Instance.GetName<LogDestinationFormatFunctionMock>();

			var logAdmin = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationFormatFunctionMock(str =>
				{
					strActual = str;
				}))
				.AddCustomDestination(new LogDestinationFormatFunctionMock(str =>
				{
					strActual2 = str;
				}))
				.SetFormatMiddleware<FormatToAbcMiddleware>(name);

			var log = logAdmin.GetLogger(this.GetType());
			log.Fatal("this message should not be used");

			Assert.Equal("Abc", strActual);
			Assert.Equal("Abc", strActual2);
		}

		[Fact]
		public void FormatMiddlewareCanBeSetAndGetForMultipleTypes()
		{
			var admin = LogAdminitrator.CreateNew();

			var formatMiddlewareName = LogConfigurationMapper.Instance.GetName<FormatMiddleware>();
			var noFilterMiddlewareName = LogConfigurationMapper.Instance.GetName<NoFilterMiddleware>();
			var noQueueMiddlewareName = LogConfigurationMapper.Instance.GetName<NoQueueMiddleware>();

			admin.SetFormatMiddleware<FormatToAbcMiddleware>(formatMiddlewareName);
			admin.SetFormatMiddleware<FormatToEmptyMiddleware>(noFilterMiddlewareName);
			admin.SetFormatMiddleware<FormatToAbcMiddleware>(noQueueMiddlewareName);

			admin.GetFormatMiddleware(formatMiddlewareName, out var formatMiddlewareNameActual);
			admin.GetFormatMiddleware(noFilterMiddlewareName, out var noFilterMiddlewareActual);
			admin.GetFormatMiddleware(noQueueMiddlewareName, out var noQueueMiddlewareNameActual);

			Assert.Equal(nameof(FormatToAbcMiddleware), formatMiddlewareNameActual);
			Assert.Equal(nameof(FormatToEmptyMiddleware), noFilterMiddlewareActual);
			Assert.Equal(nameof(FormatToAbcMiddleware), noQueueMiddlewareNameActual);
		}

		[Fact]
		public void LastFormatMiddlewareCanBeRetrievedAfterMultipleSet()
		{
			var admin = LogAdminitrator.CreateNew();

			var fileLogDestinationName = LogConfigurationMapper.Instance.GetName<FileLogDestination>();

			admin.SetFormatMiddleware<FormatToAbcMiddleware>(fileLogDestinationName);
			admin.SetFormatMiddleware<FormatToEmptyMiddleware>(fileLogDestinationName);
			admin.SetFormatMiddleware<FormatTo123Middleware>(fileLogDestinationName);

			admin.GetFormatMiddleware(fileLogDestinationName, out var formatMiddlewareName);
			var formatMiddlewareType = LogConfigurationMapper.Instance.GetType(formatMiddlewareName);

			var formatMiddleware = Activator.CreateInstance(formatMiddlewareType) as IFormatMiddleware;

			var formattedString = formatMiddleware.Format(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test", null, null));

			Assert.Equal("123", formattedString);
		}

		[Fact]
		public void NotSetFormatMiddlewareMustNotBreak()
		{
			var fileDestinationName = LogConfigurationMapper.Instance.GetName<FileLogDestination>();

			LogAdminitrator
				.CreateNew()
				.GetFormatMiddleware(fileDestinationName, out var formatMiddlewareName);

			var formatMiddlewareType = LogConfigurationMapper.Instance.GetType(formatMiddlewareName);

			// Will throw if can not create
			Assert.NotNull((IFormatMiddleware)Activator.CreateInstance(formatMiddlewareType));
		}
	}
}