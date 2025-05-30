﻿// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log;

/// <summary>
/// Helps to work with structured messages
/// </summary>
public static class LogArgumentsParser
{
	private static readonly ConcurrentDictionary<string, KeyValuePair<string, object>[]> _parsedKeyCache = [];

	public static IReadOnlyList<KeyValuePair<string, object>> ParseArguments(string message, object[] args)
	{
		if (args == null || args.Length == 0 || string.IsNullOrWhiteSpace(message))
		{
			return [];
		}

		if (_parsedKeyCache.TryGetValue(message, out var value))
		{
			return CopyArgsToCached(args, value);
		}

		var arguments = new List<KeyValuePair<string, object>>();

		var argId = 0;
		var startIndex = -1;

		do
		{
			startIndex = FindParseStartIndex(message, startIndex);

			var endIndex = startIndex == -1 ? -1 : message.IndexOf('}', startIndex + 1);

			if (endIndex == -1)
			{
				break;
			}

			var key = message.Substring(startIndex + 1, endIndex - startIndex - 1);
			var keyWithoutFormat = key.Split(':')[0];

			startIndex = endIndex;

			if (arguments.FindIndex(0, p => p.Key == keyWithoutFormat) > -1)
			{
				continue;
			}

			object arg = null;
			if (argId < args.Length)
			{
				arg = args[argId];
			}

			arguments.Add(new KeyValuePair<string, object>(keyWithoutFormat, arg));

			argId++;
		}
		while (true);

		CacheArgumentKeys(message, arguments);

		return arguments;
	}

	private static IReadOnlyList<KeyValuePair<string, object>> CopyArgsToCached(object[] args, KeyValuePair<string, object>[] value)
	{
		var cached = new List<KeyValuePair<string, object>>(value);

		for (var i = 0; i < cached.Count; i++)
		{
			if (i < args.Length)
			{
				cached[i] = new KeyValuePair<string, object>(cached[i].Key, args[i]);
			}
		}

		return cached;
	}

	private static void CacheArgumentKeys(string message, List<KeyValuePair<string, object>> arguments)
	{
		var cache = new KeyValuePair<string, object>[arguments.Count];

		for (var i = 0; i < cache.Length; i++)
		{
			cache[i] = new KeyValuePair<string, object>(arguments[i].Key, null);
		}

		_parsedKeyCache.TryAdd(message, cache);
	}

	private static int FindParseStartIndex(string message, int startIndex)
	{
		startIndex = message.IndexOf('{', startIndex + 1);

		if (startIndex == -1 || startIndex == message.Length - 1)
		{
			return -1;
		}

		if (message[startIndex + 1] == '{')
		{
			return FindParseStartIndex(message, startIndex + 1);
		}

		return startIndex;
	}

	public static string InsertArguments(string message, IReadOnlyList<KeyValuePair<string, object>> arguments)
	{
		if (arguments == null || arguments.Count == 0 || string.IsNullOrWhiteSpace(message))
		{
			return message;
		}

		var startIndex = -1;

		do
		{
			FindInsertStartIndex(ref message, ref startIndex);

			var endIndex = startIndex == -1 ? -1 : message.IndexOf('}', startIndex + 1);

			if (endIndex == -1)
			{
				break;
			}

			var key = message.Substring(startIndex + 1, endIndex - startIndex - 1);
			var splittedKey = key.Split(':');
			var keyWithoutFormat = splittedKey[0];

			var index = FindKeyIndex(arguments, keyWithoutFormat);

			if (index > -1)
			{
				var value = arguments[index].Value;
				var valueString = ConvertValueToString(splittedKey, value);
				message = message.Remove(startIndex, endIndex - startIndex + 1).Insert(startIndex, valueString);
				startIndex += valueString.Length - 1;
			}
		}
		while (true);

		return message;
	}

	private static int FindKeyIndex(IReadOnlyList<KeyValuePair<string, object>> list, string key)
	{
		var count = list.Count;

		for (var i = 0; i < count; i++)
		{
			if (list[i].Key == key)
			{
				return i;
			}
		}

		return -1;
	}

	private static string ConvertValueToString(string[] splittedKey, object value)
	{
		if (splittedKey.Length > 1 && value is IFormattable formattable)
		{
			var format = string.Join(":", splittedKey.Skip(1));
			return formattable.ToString(format, null);
		}

		return value == null ? string.Empty : value.ToString();
	}

	private static void FindInsertStartIndex(ref string message, ref int startIndex)
	{
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