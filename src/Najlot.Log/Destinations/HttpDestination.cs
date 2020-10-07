// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Najlot.Log.Destinations
{
	[LogConfigurationName(nameof(HttpDestination))]
	public sealed class HttpDestination : IDestination
	{
		private readonly Uri _uri;
		private readonly string _token;

		public HttpDestination(string url, string token = null)
		{
			_uri = new Uri(url);
			_token = token;
		}

		public void Log(IEnumerable<LogMessage> messages)
		{
			int slice = 0;
			int sliceSize = 200;

			while (LogSlice(messages, slice, sliceSize))
			{
				slice += sliceSize;
			}
		}

		private bool LogSlice(IEnumerable<LogMessage> messages, int from, int count)
		{
			var messagesSlice = messages
				.Skip(from)
				.Take(count)
				.Select(m => m.Message);

			if (messagesSlice.Any())
			{
				LogSlice(messagesSlice);

				return true;
			}

			return false;
		}

		private void LogSlice(IEnumerable<string> messages)
		{
			var json = $"[{string.Join(", ", messages).TrimEnd(' ', ',')}]";
			var bytes = Encoding.UTF8.GetBytes(json);

			var request = WebRequest.CreateHttp(_uri);
			request.Method = "PUT";
			request.ContentType = "application/json";
			request.ContentLength = bytes.Length;

			if (!string.IsNullOrWhiteSpace(_token))
			{
				request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {_token}");
			}

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}

			using (var response = (HttpWebResponse)request.GetResponse())
			{
				if ((int)response.StatusCode >= 400)
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						using (var myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
						{
							var responseString = myStreamReader.ReadToEnd();
							var ex = new Exception(responseString);
							LogErrorHandler.Instance.Handle("Error writing log messages to " + _uri, ex);
						}
					}
				}
			}
		}

		#region IDisposable Support

		private bool _disposedValue;
		
		private void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				_disposedValue = true;

				if (disposing)
				{
					
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}