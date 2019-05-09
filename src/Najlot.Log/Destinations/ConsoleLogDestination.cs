﻿using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Writes all messages to console.
	/// </summary>
	public sealed class ConsoleLogDestination : ILogDestination
	{
		public readonly bool UseColors;
		public readonly ConsoleColor DefaultColor;

		public ConsoleLogDestination(bool useColors)
		{
			UseColors = useColors;

			if (UseColors)
			{
				DefaultColor = Console.ForegroundColor;
			}
		}

		public void Dispose()
		{
			// Nothing to dispose
		}

		public void Log((LogMessage, IFormatMiddleware)[] logMessageFormattingPair)
		{
			foreach (var entry in logMessageFormattingPair)
			{
				Log(entry.Item1, entry.Item2);
			}
		}

		public void Log(LogMessage message, IFormatMiddleware formatMiddleware)
		{
			if (UseColors)
			{
				switch (message.LogLevel)
				{
					case LogLevel.Trace:
						Console.ForegroundColor = ConsoleColor.DarkGray;
						break;

					case LogLevel.Debug:
						Console.ForegroundColor = ConsoleColor.Gray;
						break;

					case LogLevel.Info:
						Console.ForegroundColor = ConsoleColor.White;
						break;

					case LogLevel.Warn:
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;

					case LogLevel.Error:
						Console.ForegroundColor = ConsoleColor.Red;
						break;

					case LogLevel.Fatal:
						Console.ForegroundColor = ConsoleColor.DarkRed;
						break;
				}
			}

			Console.Out.WriteLine(formatMiddleware.Format(message));

			if (UseColors)
			{
				Console.ForegroundColor = DefaultColor;
			}
		}
	}
}