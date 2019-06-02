// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log
{
	public static class LogArgumentsParser
	{
		public static IReadOnlyList<KeyValuePair<string, object>> ParseArguments(string message, object[] args)
		{
			var arguments = new List<KeyValuePair<string, object>>();

			int argId = 0;
			int startIndex = -1;
			int endIndex;
			
			do
			{
				FindParseStartIndex(message, ref startIndex);

				endIndex = startIndex == -1 ? -1 : message.IndexOf('}', startIndex + 1);

				if (endIndex != -1)
				{
					var key = message.Substring(startIndex + 1, endIndex - startIndex - 1);
					var splittedKey = key.Split(':');
					var keyWithoutFormat = splittedKey[0];

					startIndex = endIndex;

					if (arguments.FindIndex(0, p => p.Key == keyWithoutFormat) > -1)
					{
						continue;
					}

					arguments.Add(new KeyValuePair<string, object>(keyWithoutFormat, args[argId]));

					argId++;
					if (argId >= args.Length)
					{
						return arguments;
					}
				}
			}
			while (endIndex != -1);

			return arguments;
		}

		private static void FindParseStartIndex(string message, ref int startIndex)
		{
			if (startIndex >= message.Length)
			{
				startIndex = -1;
				return;
			}

			startIndex = message.IndexOf('{', startIndex + 1);

			if (startIndex == message.Length - 1)
			{
				startIndex = -1;
				return;
			}

			if (message[startIndex + 1] == '{')
			{
				startIndex += 1;
				FindParseStartIndex(message, ref startIndex);
			}
		}

		public static string InsertArguments(string message, IReadOnlyList<KeyValuePair<string, object>> arguments)
		{
			if (arguments.Count == 0 || string.IsNullOrWhiteSpace(message))
			{
				return message;
			}

			int startIndex = -1;
			int endIndex;
			var argList = new List<KeyValuePair<string, object>>(arguments);

			do
			{
				FindInsertStartIndex(ref message, ref startIndex);

				endIndex = startIndex == -1 ? -1 : message.IndexOf('}', startIndex + 1);

				if (endIndex == -1)
				{
					return message;
				}

				var key = message.Substring(startIndex + 1, endIndex - startIndex - 1);
				var splittedKey = key.Split(':');
				var keyWithoutFormat = splittedKey[0];

				int index = argList.FindIndex(0, p => p.Key == keyWithoutFormat);

				if (index > -1)
				{
					var value = argList[index].Value;
					var valueString = ConvertValueToString(splittedKey, value);
					message = message.Remove(startIndex, endIndex - startIndex + 1).Insert(startIndex, valueString);
					startIndex += valueString.Length - 1;
				}
			}
			while (endIndex != -1);

			return message;
		}

		private static string ConvertValueToString(string[] splittedKey, object value)
		{
			string valueString;

			if (splittedKey.Length > 1 && value is IFormattable formattable)
			{
				var format = string.Join(":", splittedKey.Skip(1));
				valueString = formattable.ToString(format, null);
			}
			else
			{
				valueString = value == null ? "" : value.ToString();
			}

			return valueString;
		}

		private static void FindInsertStartIndex(ref string message, ref int startIndex)
		{
			if (startIndex >= message.Length)
			{
				startIndex = -1;
				return;
			}

			startIndex = message.IndexOf('{', startIndex + 1);

			if (startIndex == -1)
			{
				return;
			}

			if (startIndex == message.Length - 1)
			{
				startIndex = -1;
				return;
			}

			if (message[startIndex + 1] == '{')
			{
				message = message.Remove(startIndex, 1);
				FindInsertStartIndex(ref message, ref startIndex);
			}
		}
	}
}