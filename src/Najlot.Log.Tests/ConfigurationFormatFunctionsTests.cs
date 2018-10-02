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
				LogConfigurator.CreateNew().AddCustomDestination(null);
			});
		}

		[Fact]
		public void FormatFuctionCanBeChangedAfterCreation()
		{
			var strExpected = "AA Bb cc";
			var strActual = "";

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, str =>
				{
					strActual = str;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);
			
			var log = loggerPool.GetLogger(this.GetType());

			logConfiguration.TrySetFormatFunctionForType(typeof(LogDestinationFormatFunctionMock), msg => strExpected);

			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void FormatFuctionCanBeChangedAfterRegistration()
		{
			var strExpected = "AA Bb cc";
			var strActual = "";

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, str =>
				{
					strActual = str;
				}))
				.GetLoggerPool(out LoggerPool loggerPool);

			logConfiguration.TrySetFormatFunctionForType(typeof(LogDestinationFormatFunctionMock), msg => strExpected);

			var log = loggerPool.GetLogger(this.GetType());
			
			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void FormatFuctionCanBeSetOnRegistration()
		{
			var strExpected = "AA Bb cc";
			var strActual = "";

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration)
				.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, str =>
				{
					strActual = str;
				}), 
				msg =>
				{
					return strExpected;
				})
				.GetLoggerPool(out LoggerPool loggerPool);

			var log = loggerPool.GetLogger(this.GetType());
			log.Fatal("this message should not be used");

			Assert.Equal(strExpected, strActual);
		}

		[Fact]
		public void NotSetFormattingFunctionMustReturnFalseOnGet()
		{
			LogConfigurator
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

			LogConfigurator
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

			var configurator = LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);

			bool canSetFunction = logConfiguration.TrySetFormatFunctionForType(typeof(LogDestinationFormatFunctionMock), (msg) =>
			{
				return returnString;
			});

			Assert.True(canSetFunction, "Could not set function");

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(typeof(LogDestinationFormatFunctionMock), out Func<LogMessage, string> formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatedString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test"));

			Assert.Equal(returnString, formatedString);

			configurator.AddCustomDestination(new LogDestinationFormatFunctionMock(logConfiguration, formatted =>
			{
				formatedString = formatted;
			}));

			Assert.Equal(returnString, formatedString);
		}
		
		[Fact]
		public void LastFormatFunctionCanBeRetrievedAfterMultipleSet()
		{
			var returnString = "correct string";

			LogConfigurator
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

			Func<LogMessage, string> formatFunc;

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(this.GetType(), out formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test"));

			Assert.Equal(returnString, formatString);
		}

		[Fact]
		public void FormatFunctionCanBeSetAndGetForMultipleTypes()
		{
			var thisType = this.GetType();
			var returnString = thisType.Name;
			var types = new List<Type>() { typeof(LogMessage), thisType, typeof(Logger) };

			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);
			
			foreach(var type in types)
			{
				bool canSetFunction = logConfiguration.TrySetFormatFunctionForType(type, (msg) =>
				{
					return type.Name;
				});

				Assert.True(canSetFunction, "Could not set function for " + type.Name);
			}
			
			Func<LogMessage, string> formatFunc;

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(this.GetType(), out formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test"));

			Assert.Equal(returnString, formatString);
		}
	}
}
