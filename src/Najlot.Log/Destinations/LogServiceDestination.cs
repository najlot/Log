// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Writes messages to the LogService. Not ready to use, so internal
	/// </summary>
	[LogConfigurationName(nameof(LogServiceDestination))]
	internal sealed class LogServiceDestination : ILogDestination
	{
		private readonly HttpClient _client;

		public LogServiceDestination(string uri, string token)
		{
			var uriString = "";

			if (uri.Length > 5)
			{
				if (uri.Substring(0, 4).ToLower() == "http")
				{
					uriString = uri;
				}
			}

			if (uriString == "")
			{
				uriString = $"http://{uri}";
			}

			_client = new HttpClient()
			{
				BaseAddress = new Uri(uriString)
			};

			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", "Bearer " + token);

			_client.DefaultRequestHeaders.Accept.Clear();
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
		{
			int count = messages.Count();

			for (int i = 0; i < count; i += 10000)
			{
				var messageStrings = messages.Take(10000).Select(m => formatMiddleware.Format(m));
				messages = messages.Skip(10000);

				var requestString = "[" + string.Join(",", messageStrings) + "]";
				var result = _client.PutAsync("api/LogMessage", new StringContent(requestString, Encoding.UTF8, "application/json")).Result;

				if (!result.IsSuccessStatusCode)
				{
					throw new Exception(result.ReasonPhrase);
				}
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					_client.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

	}
}