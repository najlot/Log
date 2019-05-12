using System;
using System.Collections.Generic;
using System.Text;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Serialises LogMessage to a single line json string.
	/// </summary>
	public class JsonFormatMiddleware : IFormatMiddleware
	{
		private void AppendJson(StringBuilder sb, string raw)
		{
			// https://www.freeformatter.com/json-escape.html

			if (string.IsNullOrEmpty(raw))
			{
				return;
			}

			foreach (char c in raw)
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
							sb.Append(((int)c).ToString("X").PadLeft(4, '0'));
						}
						else
						{
							sb.Append(c);
						}
						break;
				}
			}
		}

		private void SerializeArgument(StringBuilder sb, KeyValuePair<string, object> arg)
		{
			sb.Append("{\"Key\":\"");
			AppendJson(sb, arg.Key.ToString());
			sb.Append("\",\"Value\":");
			AppendJson(sb, arg.Value?.ToString());
			sb.Append('}');
		}

		public string Format(LogMessage message)
		{
			var sb = new StringBuilder();

			sb.Append("{\"DateTime\":\"");
			sb.Append(message.DateTime.ToString("o"));
			sb.Append("\",\"LogLevel\":");
			sb.Append(((int)message.LogLevel).ToString());
			sb.Append(",\"Category\":\"");
			AppendJson(sb, message.Category);
			sb.Append("\",\"State\":");
			AppendJson(sb, message.State?.ToString());
			sb.Append(",\"BaseMessage\":\"");
			AppendJson(sb, message.Message);
			sb.Append(",\"Message\":\"");
			AppendJson(sb, LogArgumentsParser.InsertArguments(message.Message, message.Arguments));
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

			bool isFirst = true;

			foreach (var arg in message.Arguments)
			{
				if (isFirst) isFirst = false;
				else sb.Append(',');

				SerializeArgument(sb, arg);
			}
			
			sb.Append("]}");

			return sb.ToString();
		}
	}
}