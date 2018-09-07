using Najlot.Log.Configuration;
using System;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Writes all messages to console.
	/// </summary>
	public class ConsoleLogDestination : LogDestinationBase
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
