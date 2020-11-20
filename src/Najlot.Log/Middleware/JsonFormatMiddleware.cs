// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Serialises a message to a single line json string
	/// </summary>
	[LogConfigurationName(nameof(JsonFormatMiddleware))]
	public sealed class JsonFormatMiddleware : IMiddleware
	{
		public IMiddleware NextMiddleware { get; set; }

		private static readonly CultureInfo _enUsCultureInfo = new CultureInfo("en-US");

		private static void AppendJson(StringBuilder sb, string raw)
		{
			// https://www.freeformatter.com/json-escape.html

			if (string.IsNullOrEmpty(raw))
			{
				return;
			}

			foreach (var c in raw)
			{
				switch (c)
				{
					case '\b':
						sb.Append("\\b");
						break;

					case '\f':
						sb.Append("\\f");
						break;

					case '\n':
						sb.Append("\\n");
						break;

					case '\r':
						sb.Append("\\r");
						break;

					case '\t':
						sb.Append("\\t");
						break;

					case '"':
						sb.Append("\\\"");
						break;

					case '\\':
						sb.Append("\\\\");
						break;

					default:
						if (c < ' ')
						{
							sb.Append("\\u");
							sb.Append(((int)c).ToString("X", _enUsCultureInfo).PadLeft(4, '0'));
						}
						else
						{
							sb.Append(c);
						}
						break;
				}
			}
		}

		private static void SerializeArgument(StringBuilder sb, KeyValuePair<string, object> arg)
		{
			sb.Append("{\"Key\":\"");
			AppendJson(sb, arg.Key);
			sb.Append("\",\"Value\":");

			if (arg.Value == null)
			{
				sb.Append("null");
			}
			else
			{
				sb.Append('\"');
				AppendJson(sb, arg.Value.ToString());
				sb.Append('\"');
			}

			sb.Append("}");
		}

		public void Execute(IEnumerable<LogMessage> messages)
		{
			if (messages == null) return;

			foreach (var message in messages)
			{
				var sb = new StringBuilder();

				sb.Append("{\"DateTime\":\"");
				sb.Append(message.DateTime.ToString("o", _enUsCultureInfo));
				sb.Append("\",\"LogLevel\":");
				sb.Append(((int)message.LogLevel).ToString(_enUsCultureInfo));
				sb.Append(",\"Category\":\"");
				AppendJson(sb, message.Category);
				sb.Append("\",\"State\":");

				if (message.State == null)
				{
					sb.Append("null");
				}
				else
				{
					sb.Append('\"');
					AppendJson(sb, message.State.ToString());
					sb.Append('\"');
				}

				sb.Append(",\"RawMessage\":\"");
				AppendJson(sb, message.RawMessage);
				sb.Append("\",\"Message\":\"");
				AppendJson(sb, LogArgumentsParser.InsertArguments(message.RawMessage, message.Arguments));
				sb.Append("\",\"Exception\":");

				if (message.ExceptionIsValid)
				{
					sb.Append("\"");
					AppendJson(sb, message.Exception.ToString());
					sb.Append("\",\"ExceptionIsValid\":true");
				}
				else
				{
					sb.Append("null,\"ExceptionIsValid\":false");
				}

				sb.Append(",\"Arguments\":[");

				var isFirst = true;

				foreach (var arg in message.Arguments)
				{
					if (isFirst) isFirst = false;
					else sb.Append(',');

					SerializeArgument(sb, arg);
				}

				sb.Append("]}");

				message.Message = sb.ToString();
			}

			NextMiddleware.Execute(messages);
		}

		public void Flush() => NextMiddleware?.Flush();

		public void Dispose() => Flush();
	}
}