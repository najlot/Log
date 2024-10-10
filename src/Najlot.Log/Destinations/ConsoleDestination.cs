// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Najlot.Log.Destinations;

/// <summary>
/// Writes all messages to console.
/// </summary>
[LogConfigurationName(nameof(ConsoleDestination))]
public sealed class ConsoleDestination : IDestination
{
	[LogConfigurationName(nameof(UseColors))]
	public bool UseColors { get; set; }

	private readonly ConsoleColor _defaultColor;

	public ConsoleDestination() : this(false)
	{
	}

	public ConsoleDestination(bool useColors)
	{
		UseColors = useColors;
		_defaultColor = Console.ForegroundColor;
	}

	public void Log(IEnumerable<LogMessage> messages)
	{
		if (messages == null) return;

		if (UseColors)
		{
			LogWithColors(messages);
		}
		else
		{
			var sb = new StringBuilder();

			foreach (var message in messages)
			{
				sb.AppendLine(message.Message);
			}

			Console.Out.Write(sb.ToString());
		}
	}

	private void LogWithColors(IEnumerable<LogMessage> messages)
	{
		var previousLogLevel = LogLevel.None;
		var first = true;
		var sb = new StringBuilder();

		foreach (var message in messages)
		{
			if (first)
			{
				first = false;
				previousLogLevel = message.LogLevel;
				SetColor(message.LogLevel);
			}
			else if (previousLogLevel != message.LogLevel)
			{
				Console.Out.Write(sb.ToString());
				previousLogLevel = message.LogLevel;
				SetColor(message.LogLevel);

				sb.Clear();
			}

			sb.AppendLine(message.Message);
		}

		Console.Out.Write(sb.ToString());

		Console.ForegroundColor = _defaultColor;
	}

	private static void SetColor(LogLevel logLevel)
	{
		Console.ForegroundColor = logLevel switch
		{
			LogLevel.Trace => ConsoleColor.DarkGray,
			LogLevel.Debug => ConsoleColor.Gray,
			LogLevel.Info => ConsoleColor.White,
			LogLevel.Warn => ConsoleColor.Yellow,
			LogLevel.Error => ConsoleColor.Red,
			LogLevel.Fatal => ConsoleColor.DarkRed,
			_ => ConsoleColor.White,
		};
	}

	public void Flush()
	{
		// Nothing to do
	}

	public void Dispose()
	{
		// Nothing to dispose
	}
}