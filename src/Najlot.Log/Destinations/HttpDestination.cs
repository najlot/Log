// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Najlot.Log.Destinations;

[LogConfigurationName(nameof(HttpDestination))]
public sealed class HttpDestination : IDestination
{
	[LogConfigurationName(nameof(Uri))]
	public string Uri { get; set; }

	[LogConfigurationName(nameof(Token))]
	public string Token { get; set; }

	public HttpDestination()
	{
	}

	public HttpDestination(string url, string token = null)
	{
		Uri = url;
		Token = token;
	}

	public void Log(IEnumerable<LogMessage> messages)
	{
		var slice = 0;
		const int sliceSize = 200;

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

		var request = WebRequest.CreateHttp(new Uri(Uri));
		request.Method = "PUT";
		request.ContentType = "application/json";
		request.ContentLength = bytes.Length;

		if (!string.IsNullOrWhiteSpace(Token))
		{
			request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {Token}");
		}

		using (var requestStream = request.GetRequestStream())
		{
			requestStream.Write(bytes, 0, bytes.Length);
		}

		using (var response = (HttpWebResponse)request.GetResponse())
		{
			if ((int)response.StatusCode >= 400)
			{
				using (var responseStream = response.GetResponseStream())
				{
					using (var myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
					{
						var responseString = myStreamReader.ReadToEnd();
						var ex = new Exception(responseString);
						LogErrorHandler.Instance.Handle("Error writing log messages to " + Uri, ex);
					}
				}
			}
		}
	}

	public void Flush()
	{
		// Nothing to do
	}

	#region IDisposable Support

	public void Dispose()
	{
		// Nothing to do
	}

	#endregion IDisposable Support
}