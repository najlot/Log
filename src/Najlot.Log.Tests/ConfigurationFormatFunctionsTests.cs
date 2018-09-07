using Najlot.Log.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ConfigurationFormatFunctionsTests
	{
		[Fact]
		public void NotSetFormattingFunctionMustReturnFalseOnGet()
		{
			LogConfigurator
				.CreateNew()
				.GetLogConfiguration(out ILogConfiguration logConfiguration);
			
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
        public void CanSetAndGetFormattingFunction()
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

			bool canGetFunction = logConfiguration.TryGetFormatFunctionForType(this.GetType(), out Func<LogMessage, string> formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, null, "test"));

			Assert.Equal(returnString, formatString);
		}
		
		[Fact]
		public void CanSetAndGetFormattingFunctionAfterMultipleSet()
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
		public void CanSetAndGetFormattingFunctionWithMultipleTypes()
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
