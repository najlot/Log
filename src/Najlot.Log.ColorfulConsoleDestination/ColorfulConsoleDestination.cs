﻿using Najlot.Log.Configuration;
using System;
using System.Drawing;
using Console = Colorful.Console;

namespace Najlot.Log.Destinations
{
	public class ColorfulConsoleDestination : LogDestinationBase
	{
		public ColorfulConsoleDestination(ILogConfiguration logConfiguration) : base(logConfiguration)
		{
		}

		protected override void Log(LogMessage message)
		{
			var msgString = Format(message);

			switch (message.LogLevel)
			{
				case LogLevel.Trace:
					Console.WriteLine(msgString, Color.Gray);
					break;
				case LogLevel.Debug:
					Console.WriteLine(msgString, Color.LightGray);
					break;
				case LogLevel.Info:
					Console.WriteLine(msgString, Color.Green);
					break;
				case LogLevel.Warn:
					Console.WriteLine(msgString, Color.Yellow);
					break;
				case LogLevel.Error:
					Console.WriteLine(msgString, Color.DarkRed);
					break;
				case LogLevel.Fatal:
					Console.WriteLine(msgString, Color.Red);
					break;
				default:
					Console.WriteLine(msgString, Color.White);
					break;
			}
		}
	}
}