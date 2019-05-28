using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Writes all messages to console.
	/// </summary>
	[LogConfigurationName(nameof(ConsoleLogDestination))]
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

		public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
		{
			if (UseColors)
			{
				LogWithColors(messages, formatMiddleware);
			}
			else
			{
				var sb = new StringBuilder();

				foreach (var message in messages)
				{
					sb.Append(formatMiddleware.Format(message));
				}

				Console.Out.WriteLine(sb.ToString());
			}
		}

		private void LogWithColors(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
		{
			var previousLogLevel = LogLevel.None;
			bool first = true;
			var sb = new StringBuilder();

			foreach (var message in messages)
			{
				if (first)
				{
					first = false;
					previousLogLevel = message.LogLevel;
					SetColor(previousLogLevel);
				}
				else if (previousLogLevel != message.LogLevel)
				{
					Console.Out.WriteLine(sb.ToString());
					previousLogLevel = message.LogLevel;
					SetColor(message.LogLevel);

					sb.Clear();
					sb.Append(formatMiddleware.Format(message));
				}
			}

			Console.Out.WriteLine(sb.ToString());

			Console.ForegroundColor = DefaultColor;
		}

		private void SetColor(LogLevel logLevel)
		{
			switch (logLevel)
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
	}
}