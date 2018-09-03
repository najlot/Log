using NajlotLog.Configuration;
using System;

namespace NajlotLog.Destinations
{
	/// <summary>
	/// Writes all messages to console.
	/// </summary>
	internal class ConsoleLogDestination : LogDestinationBase
	{
		public ConsoleLogDestination(ILogConfiguration configuration) : base(configuration)
		{

		}

		protected override void Log(LogMessage message)
		{
			Console.WriteLine(Format(message));
		}
	}
}
