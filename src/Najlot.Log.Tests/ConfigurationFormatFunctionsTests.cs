using Najlot.Log.Configuration;
using Najlot.Log.Tests.Mocks;
using System;
using System.Collections.Generic;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ConfigurationFormatFunctionsTests
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
		public void FormatFuctionCanBeChangedAfterCreation()
		{
			var strExpected = "AA Bb cc";
			var strActual = "";

			var logAdmin = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, str =>
				{
					strActual = str;
				}));

			var log = logAdmin.GetLogger(this.GetType());

			logConfiguration.TrySetFormatFunctionForType(typeof(LogDestinationFormatFunctionMock), msg => strExpected);

			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void FormatFuctionCanBeChangedAfterRegistration()
		{
			var strExpected = "AA Bb cc";
			var strActual = "";

			var logAdmin = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, str =>
				{
					strActual = str;
				}));

			logConfiguration.TrySetFormatFunctionForType(typeof(LogDestinationFormatFunctionMock), msg => strExpected);

			var log = logAdmin.GetLogger(this.GetType());

			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void FormatFuctionCanBeSetOnRegistration()
		{
			var strExpected = "AA Bb cc";
			var strActual = "";

			var logAdmin = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, str =>
				{
					strActual = str;
				}),
				msg =>
				{
					return strExpected;
				});

			var log = logAdmin.GetLogger(this.GetType());
			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void SameFormatFunctionCanBeSetToTwoDestinations()
		{
			var strExpected = "AA Bb cc";
			var strActual = "";
			var strActual2 = "";

			string formatFunc(LogMessage message)
			{
				return strExpected;
			}

			var logAdmin = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, str =>
				{
					strActual = str;
				}), formatFunc)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, str =>
				{
					strActual2 = str;
				}), formatFunc);

			var log = logAdmin.GetLogger(this.GetType());
			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
			Assert.Equal(strExpected, strActual2);
		}

		[Fact]
		public void FormatFunctionCanBeSetAndGetForMultipleTypes()
		{
			var thisType = this.GetType();
			var returnString = thisType.Name;
			var types = new List<Type>() { typeof(LogMessage), thisType, typeof(Logger) };

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			foreach (var type in types)
			{
				bool canSetFunction = logConfiguration.TrySetFormatFunctionForType(type, (msg) =>
				{
					return type.Name;
				});

				Assert.True(canSetFunction, "Could not set function for " + type.Name);
			}

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(this.GetType(), out Func<LogMessage, string> formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test", null));

			Assert.Equal(returnString, formatString);
		}

		[Fact]
		public void LastFormatFunctionCanBeRetrievedAfterMultipleSet()
		{
			var returnString = "correct string";

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			bool canSetFunction = logConfiguration.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return "wrong 1";
			});

			Assert.True(canSetFunction, "Could not set function 1");

			canSetFunction = logConfiguration.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return "wrong 2";
			});

			Assert.True(canSetFunction, "Could not set function 2");

			canSetFunction = logConfiguration.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return returnString;
			});

			Assert.True(canSetFunction, "Could not set function 3");

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(this.GetType(), out Func<LogMessage, string> formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test", null));

			Assert.Equal(returnString, formatString);
		}

		[Fact]
		public void NotSetFormattingFunctionMustReturnFalseOnGet()
		{
			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			bool canSetFunction = logConfiguration.TrySetFormatFunctionForType(
				this.GetType(), null);

			Assert.False(canSetFunction, "Function was set to null");

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(
				this.GetType(),
				out Func<LogMessage, string> formatFunc);

			Assert.False(canGetFunction, "Could get function, that was not set");
		}

		[Fact]
		public void RemovedFormattingFunctionMustReturnFalseOnGet()
		{
			var returnString = "some sample string";

			LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			bool canSetFunction = logConfiguration.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return returnString;
			});

			Assert.True(canSetFunction, "Could not set function");

			logConfiguration.ClearAllFormatFunctions();

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(this.GetType(),
				out Func<LogMessage, string> formatFunc);

			Assert.False(canGetFunction, "Could get not set function");
		}

		[Fact]
		public void SetFormatFunctionCanBeRetrieved()
		{
			var returnString = "some sample string";

			var configurator = LogAdminitrator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			bool canSetFunction = logConfiguration.TrySetFormatFunctionForType(typeof(LogDestinationFormatFunctionMock), (msg) =>
			{
				return returnString;
			});

			Assert.True(canSetFunction, "Could not set function");

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(typeof(LogDestinationFormatFunctionMock), out Func<LogMessage, string> formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatedString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test", null));

			Assert.Equal(returnString, formatedString);

			configurator.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, formatted =>
			{
				formatedString = formatted;
			}));

			Assert.Equal(returnString, formatedString);
		}
	}
}