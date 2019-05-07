using System.Collections.Generic;

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
					startIndex = endIndex + 1;

					if (arguments.FindIndex(0, p => p.Key == key) > -1)
					{
						continue;
					}

					arguments.Add(new KeyValuePair<string, object>(key, args[argId]));

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

		public static string InsertArguments(IReadOnlyList<KeyValuePair<string, object>> arguments, string message)
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

					int index = argList.FindIndex(0, p => p.Key == key);

					if (index > -1)
					{
						message = message.Remove(startIndex, endIndex - startIndex + 1).Insert(startIndex, argList[index].Value.ToString());
					}

					startIndex = endIndex + 1;
				}
			}
			while (endIndex != -1);
			return message;
		}
	}
}