﻿using System;
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
			bool skip;

			do
			{
				do
				{
					skip = false;

					if (startIndex >= message.Length)
					{
						startIndex = -1;
						break;
					}

					startIndex = message.IndexOf('{', startIndex + 1);

					if (startIndex == message.Length - 1)
					{
						startIndex = -1;
						break;
					}

					if (message[startIndex + 1] == '{')
					{
						skip = true;
						startIndex += 1;
					}
				}
				while (skip);

				endIndex = startIndex == -1 ? -1 : message.IndexOf('}', startIndex + 1);

				if (endIndex != -1)
				{
					var key = message.Substring(startIndex + 1, endIndex - startIndex - 1);
					var splittedKey = key.Split(':');
					var keyWithoutFormat = splittedKey[0];

					startIndex = endIndex + 1;

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

		public static string InsertArguments(string message, IReadOnlyList<KeyValuePair<string, object>> arguments)
		{
			if (arguments.Count == 0 || string.IsNullOrWhiteSpace(message))
			{
				return message;
			}

			int startIndex = -1;
			int endIndex;
			bool skip;
			var argList = new List<KeyValuePair<string, object>>(arguments);

			do
			{
				do
				{
					skip = false;

					if (startIndex >= message.Length)
					{
						startIndex = -1;
						break;
					}

					startIndex = message.IndexOf('{', startIndex + 1);

					if (startIndex == -1)
					{
						break;
					}

					if (startIndex == message.Length - 1)
					{
						startIndex = -1;
						break;
					}

					if (message[startIndex + 1] == '{')
					{
						skip = true;
						message = message.Remove(startIndex, 1);
					}
				}
				while (skip);

				endIndex = startIndex == -1 ? -1 : message.IndexOf('}', startIndex + 1);

				if (endIndex != -1)
				{
					var key = message.Substring(startIndex + 1, endIndex - startIndex - 1);
					var splittedKey = key.Split(':');
					var keyWithoutFormat = splittedKey[0];

					int index = argList.FindIndex(0, p => p.Key == keyWithoutFormat);

					if (index > -1)
					{
						string valueString;
						var value = argList[index].Value;

						if (splittedKey.Length > 1 && value is IFormattable formattable)
						{
							var format = string.Join(":", splittedKey.Skip(1));
							valueString = formattable.ToString(format, null);
						}
						else
						{
							valueString = (value == null ? "" : value.ToString());
						}

						message = message.Remove(startIndex, endIndex - startIndex + 1).Insert(startIndex, valueString);
					}

					startIndex = endIndex + 1;
				}
			}
			while (endIndex != -1);
			return message;
		}
	}
}