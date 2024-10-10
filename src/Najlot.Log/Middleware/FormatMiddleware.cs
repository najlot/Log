// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Najlot.Log.Middleware;

/// <summary>
/// Default middleware for formatting messages
/// </summary>
[LogConfigurationName(nameof(FormatMiddleware))]
public sealed class FormatMiddleware : IMiddleware
{
	private const string _delimiter = " - ";
	private static readonly CultureInfo _enUsCultureInfo = new("en-US");

	public IMiddleware NextMiddleware { get; set; }

	public void Execute(IEnumerable<LogMessage> messages)
	{
		foreach (var message in messages)
		{
			if (message.Message != null)
			{
				continue;
			}

			var sb = new System.Text.StringBuilder(50);

			var dateTime = message.DateTime;

			sb.Append(dateTime.Year.ToString());
			sb.Append("-");
			sb.Append(FormatD2(dateTime.Month));
			sb.Append("-");
			sb.Append(FormatD2(dateTime.Day));
			sb.Append(" ");
			sb.Append(FormatD2(dateTime.Hour));
			sb.Append(":");
			sb.Append(FormatD2(dateTime.Minute));
			sb.Append(":");
			sb.Append(FormatD2(dateTime.Second));
			sb.Append(".");
			sb.Append(dateTime.Millisecond.ToString("D3"));

			sb.Append(_delimiter);

			switch (message.LogLevel)
			{
				case LogLevel.Trace:
					sb.Append("TRACE");
					break;
				case LogLevel.Debug:
					sb.Append("DEBUG");
					break;
				case LogLevel.Info:
					sb.Append("INFO ");
					break;
				case LogLevel.Warn:
					sb.Append("WARN");
					break;
				case LogLevel.Error:
					sb.Append("ERROR");
					break;
				case LogLevel.Fatal:
					sb.Append("FATAL");
					break;
			}

			sb.Append(_delimiter);
			sb.Append(message.Category);
			sb.Append(_delimiter);

			if (message.State != null)
			{
				sb.Append(message.State.ToString());
			}

			sb.Append(_delimiter);

			var messageString = message.RawMessage;

			if (message.Arguments.Count > 0 && messageString.Length > 0)
			{
				messageString = LogArgumentsParser.InsertArguments(messageString, message.Arguments);
			}

			sb.Append(messageString);

			if (message.Exception != null)
			{
				sb.Append(message.Exception.ToString());
			}

			message.Message = sb.ToString();
		}

		NextMiddleware.Execute(messages);
	}

	public static string FormatD2(int arg)
	{
		switch (arg)
		{
			case 0: return "00";
			case 1: return "01";
			case 2: return "02";
			case 3: return "03";
			case 4: return "04";
			case 5: return "05";
			case 6: return "06";
			case 7: return "07";
			case 8: return "08";
			case 9: return "09";
		}

		return arg.ToString();
	}

	public void Flush() => NextMiddleware?.Flush();

	public void Dispose() => Flush();
}
