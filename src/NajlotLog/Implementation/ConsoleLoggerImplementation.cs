using System;

namespace NajlotLog.Implementation
{
	internal class ConsoleLoggerImplementation : LoggerImplementationBase
	{
		protected override void Log(LogMessage message)
		{
			Console.WriteLine(Format(message));
		}
	}
}
