using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests
{
	public class JsonFormattingMiddlewareTests
	{
		private readonly JsonFormatMiddleware _middleware = new JsonFormatMiddleware();

		[Fact]
		public void MiddlewareMustProduceValidJson()
		{
			DateTime dateTime = DateTime.Parse("2019-05-12T18:02:50.6571583+02:00");

			var message = new LogMessage(
				dateTime,
				LogLevel.Info,
				typeof(LogMessage).FullName,
				null,
				"some stuff happened {count:D3} times",
				null,
				new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("count", 10),
					new KeyValuePair<string, object>("count", 10)
				}
			);

			var expected = "{\"DateTime\":\"" + dateTime.ToString("o") + "\",\"LogLevel\":2,\"Category\":\"Najlot.Log.LogMessage\",\"State\":,\"BaseMessage\":\"some stuff happened {count:D3} times,\"Message\":\"some stuff happened 010 times\",\"Exception\":null,\"ExceptionIsValid\":false,\"Arguments\":[{\"Key\":\"count\",\"Value\":10},{\"Key\":\"count\",\"Value\":10}]}";
			var actual = _middleware.Format(message);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ExceptionMustBeLogged()
		{
			Exception exc;

			try
			{
				throw new Exception("No CD-ROM / cup holder avaible"); ;
			}
			catch (Exception ex)
			{
				exc = ex;
			}

			var message = new LogMessage(
				DateTime.Parse("2019-05-12T18:02:50.6571583+02:00"),
				LogLevel.Info,
				typeof(LogMessage).FullName,
				null,
				"some stuff happened {count:D3} times",
				exc,
				new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("count", 10),
					new KeyValuePair<string, object>("count", 10)
				}
			);

			var actual = _middleware.Format(message);

			var expectedToFind = "No CD-ROM / cup holder avaible";
			Assert.True(actual.IndexOf(expectedToFind) != -1);

			expectedToFind = "\"ExceptionIsValid\":true";
			Assert.True(actual.IndexOf(expectedToFind) != -1);
		}
	}
}