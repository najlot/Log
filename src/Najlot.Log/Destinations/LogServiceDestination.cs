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
		private readonly string _username;
		private readonly string _password;

		public LogServiceDestination(string uri, string username, string password)
		{
			_username = username;
			_password = password;

			var uriString = "";

			if (uri.Length > 5 && uri.Substring(0, 4).ToLower() == "http")
			{
				uriString = uri;
			}

			if (uriString == "")
			{
				uriString = $"https://{uri}";
			}

			_client = new HttpClient()
			{
				BaseAddress = new Uri(uriString)
			};

			TryLogin();
		}

		private bool TryLogin()
		{
			_client.DefaultRequestHeaders.Accept.Clear();
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("username", _username),
				new KeyValuePair<string, string>("password", _password),
			});

			var result = _client.PostAsync("api/Login", formContent).Result;

			if (!result.IsSuccessStatusCode)
			{
				LogErrorHandler.Instance.Handle(result.ReasonPhrase);
			}

			return result.IsSuccessStatusCode;
		}

		public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
		{
			int count = messages.Count();

			for (int i = 0; i < count; i += 10000)
			{
				var messageStrings = messages.Take(10000).Select(m => formatMiddleware.Format(m));
				messages = messages.Skip(10000);

				var requestString = "[" + string.Join(",", messageStrings) + "]";
				var content = new StringContent(requestString, Encoding.UTF8, "application/json");
				var result = _client.PutAsync("api/LogMessage", content).Result;

				if (!result.IsSuccessStatusCode)
				{
					if (TryLogin())
					{
						result = _client.PutAsync("api/LogMessage", content).Result;
					}

					if (!result.IsSuccessStatusCode)
					{
						LogErrorHandler.Instance.Handle(result.ReasonPhrase);
					}
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