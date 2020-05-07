// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using System.Collections.Generic;
using Xunit;

namespace Najlot.Log.Tests
{
	public class JsonFormattingMiddlewareTests
	{
		[Fact]
		public void MiddlewareMustProduceValidJson()
		{
			var actual = "";
			using var formatMiddleware = new JsonFormatMiddleware
			{
				NextMiddleware = new MiddlewareMock(msg =>
				{
					actual = msg.Message;
				})
			};

			DateTime dateTime = DateTime.Parse("2019-05-12T18:02:50.6571583+02:00");

			var message = new LogMessage
			{
				DateTime = dateTime,
				LogLevel = LogLevel.Info,
				Category = typeof(LogMessage).FullName,
				State = null,
				RawMessage = "some stuff happened {count:D3} times",
				Exception = null,
				RawArguments = Array.Empty<object>(),
				Arguments = new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("count", 10),
					new KeyValuePair<string, object>("count", 10)
				},
				ExceptionIsValid = false
			};

			formatMiddleware.Execute(new[] { message });

			var expected = "{\"DateTime\":\"" + dateTime.ToString("o") + "\",\"LogLevel\":2,\"Category\":\"Najlot.Log.LogMessage\",\"State\":null,\"RawMessage\":\"some stuff happened {count:D3} times\",\"Message\":\"some stuff happened 010 times\",\"Exception\":null,\"ExceptionIsValid\":false,\"Arguments\":[{\"Key\":\"count\",\"Value\":\"10\"},{\"Key\":\"count\",\"Value\":\"10\"}]}";

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ExceptionMustBeLogged()
		{
			var actual = "";
			Exception exc;

			using var formatMiddleware = new JsonFormatMiddleware
			{
				NextMiddleware = new MiddlewareMock(msg =>
				{
					actual = msg.Message;
				})
			};

			try
			{
				throw new Exception("No CD-ROM / cup holder avaible"); ;
			}
			catch (Exception ex)
			{
				exc = ex;
			}

			var message = new LogMessage
			{
				DateTime = DateTime.Parse("2019-05-12T18:02:50.6571583+02:00"),
				LogLevel = LogLevel.Info,
				Category = typeof(LogMessage).FullName,
				State = null,
				RawMessage = "some stuff happened {count:D3} times",
				Exception = exc,
				RawArguments = Array.Empty<object>(),
				Arguments = new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("count", 10),
					new KeyValuePair<string, object>("count", 10)
				},
				ExceptionIsValid = true
			};

			formatMiddleware.Execute(new[] { message });

			var expectedToFind = "No CD-ROM / cup holder avaible";
			Assert.True(actual.IndexOf(expectedToFind) != -1);

			expectedToFind = "\"ExceptionIsValid\":true";
			Assert.True(actual.IndexOf(expectedToFind) != -1);
		}
	}
}