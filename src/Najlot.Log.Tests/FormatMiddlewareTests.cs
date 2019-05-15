using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class FormatMiddlewareTests
	{
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
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(str =>
				{
					strActual = str;
				}));

			var log = logAdmin.GetLogger(this.GetType());

			logConfiguration.SetFormatMiddlewareForName<FormatToAbcMiddleware>(typeof(LogDestinationFormatFunctionMock));

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

			logAdmin.SetFormatMiddlewareForName<FormatToAbcMiddleware>(typeof(LogDestinationFormatFunctionMock));

			var log = logAdmin.GetLogger(this.GetType());

			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void FormatMiddlewareCanBeSetOnRegistration()
		{
			var strExpected = "Abc";
			var strActual = "";

			var logAdmin = LogAdminitrator
				.CreateNew()
				.AddCustomDestination(new LogDestinationFormatFunctionMock(str =>
				{
					strActual = str;
				}))
				.SetFormatMiddlewareForName<FormatToAbcMiddleware>(typeof(LogDestinationFormatFunctionMock));

			var log = logAdmin.GetLogger(this.GetType());
			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void SameFormatMiddlewareCanBeSetToTwoDestinations()
		{
			var strActual = "";
			var strActual2 = "";

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
				.SetFormatMiddlewareForName<FormatToAbcMiddleware>(typeof(LogDestinationFormatFunctionMock));

			var log = logAdmin.GetLogger(this.GetType());
			log.Fatal("this message should not be used");

			Assert.Equal("Abc", strActual);
			Assert.Equal("Abc", strActual2);
		}

		[Fact]
		public void FormatMiddlewareCanBeSetAndGetForMultipleTypes()
		{
			var admin = LogAdminitrator.CreateNew();

			admin.SetFormatMiddlewareForName<FormatToAbcMiddleware>(typeof(LogMessage));
			admin.SetFormatMiddlewareForName<FormatToEmptyMiddleware>(this.GetType());
			admin.SetFormatMiddlewareForName<FormatToAbcMiddleware>(typeof(Logger));

			admin.GetFormatMiddlewareNameForName(typeof(LogMessage), out var formatMiddlewareForLogmessage);
			admin.GetFormatMiddlewareNameForName(this.GetType(), out var formatMiddlewareTypeForThis);
			admin.GetFormatMiddlewareNameForName(typeof(Logger), out var formatMiddlewareTypeForLogger);

			Assert.Equal(typeof(FormatToAbcMiddleware), formatMiddlewareForLogmessage);
			Assert.Equal(typeof(FormatToEmptyMiddleware), formatMiddlewareTypeForThis);
			Assert.Equal(typeof(FormatToAbcMiddleware), formatMiddlewareTypeForLogger);
		}

		[Fact]
		public void LastFormatMiddlewareCanBeRetrievedAfterMultipleSet()
		{
			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			logConfiguration.SetFormatMiddlewareForName<FormatToEmptyMiddleware>(this.GetType());
			logConfiguration.SetFormatMiddlewareForName<FormatToAbcMiddleware>(this.GetType());
			logConfiguration.SetFormatMiddlewareForName<FormatTo123Middleware>(this.GetType());

			logConfiguration.GetFormatMiddlewareNameForName(this.GetType(), out var formatMiddlewareType);

			var formatMiddleware = Activator.CreateInstance(formatMiddlewareType) as IFormatMiddleware;

			var formattedString = formatMiddleware.Format(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test", null, null));

			Assert.Equal("123", formattedString);
		}

		[Fact]
		public void NotSetFormatMiddlewareMustNotBreak()
		{
			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			logConfiguration.GetFormatMiddlewareNameForName(
				this.GetType(),
				out var formatMiddlewareType);

			// Will not be null, but throw if can not create
			Assert.NotNull((IFormatMiddleware)Activator.CreateInstance(formatMiddlewareType));
		}
	}
}