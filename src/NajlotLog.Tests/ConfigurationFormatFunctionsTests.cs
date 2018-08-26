using NajlotLog.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace NajlotLog.Tests
{
	public partial class LogTests
	{
		public LogTests()
		{
			LogConfiguration.Instance.ClearAllFormatFunctions();
		}

		[Fact]
		public void NotSetFormattingFunctionMustReturnFalseOnGet()
		{
			Func<LogMessage, string> formatFunc;
			bool canGetFunction = LogConfiguration.Instance.TryGetFormatFunctionForType(this.GetType(), out formatFunc);

			Assert.False(canGetFunction, "Could get not set function");
		}

		[Fact]
		public void RemovedFormattingFunctionMustReturnFalseOnGet()
		{
			var returnString = "some sample string";

			bool canSetFunction = LogConfiguration.Instance.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return returnString;
			});

			Assert.True(canSetFunction, "Could not set function");

			LogConfiguration.Instance.ClearAllFormatFunctions();

			Func<LogMessage, string> formatFunc;

			bool canGetFunction = LogConfiguration.Instance.TryGetFormatFunctionForType(this.GetType(), out formatFunc);

			Assert.False(canGetFunction, "Could get not set function");
		}

		[Fact]
        public void CanSetAndGetFormattingFunction()
        {
			var returnString = "some sample string";

			bool canSetFunction = LogConfiguration.Instance.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return returnString;
			});

			Assert.True(canSetFunction, "Could not set function");

			Func<LogMessage, string> formatFunc;

			bool canGetFunction = LogConfiguration.Instance.TryGetFormatFunctionForType(this.GetType(), out formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, "test"));

			Assert.Equal(returnString, formatString);
		}
		
		[Fact]
		public void CanSetAndGetFormattingFunctionAfterMultipleSet()
		{
			var returnString = "correct string";

			bool canSetFunction = LogConfiguration.Instance.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return "wrong 1";
			});

			Assert.True(canSetFunction, "Could not set function 1");

			canSetFunction = LogConfiguration.Instance.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return "wrong 2";
			});

			Assert.True(canSetFunction, "Could not set function 2");

			canSetFunction = LogConfiguration.Instance.TrySetFormatFunctionForType(this.GetType(), (msg) =>
			{
				return returnString;
			});

			Assert.True(canSetFunction, "Could not set function 3");

			Func<LogMessage, string> formatFunc;

			bool canGetFunction = LogConfiguration.Instance.TryGetFormatFunctionForType(this.GetType(), out formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, "test"));

			Assert.Equal(returnString, formatString);
		}

		[Fact]
		public void CanSetAndGetFormattingFunctionWithMultipleTypes()
		{
			var thisType = this.GetType();
			var returnString = thisType.Name;

			var types = new List<Type>() { typeof(LogMessage), thisType, typeof(Logger) };

			foreach(var type in types)
			{
				bool canSetFunction = LogConfiguration.Instance.TrySetFormatFunctionForType(type, (msg) =>
				{
					return type.Name;
				});

				Assert.True(canSetFunction, "Could not set function for " + type.Name);
			}
			
			Func<LogMessage, string> formatFunc;

			bool canGetFunction = LogConfiguration.Instance.TryGetFormatFunctionForType(this.GetType(), out formatFunc);

			Assert.True(canGetFunction, "Could not get function for type");

			var formatString = formatFunc(new LogMessage(DateTime.Now, LogLevel.Info, null, "test"));

			Assert.Equal(returnString, formatString);
		}
	}
}
