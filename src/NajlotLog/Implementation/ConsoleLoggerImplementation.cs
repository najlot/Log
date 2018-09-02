using NajlotLog.Configuration;
using System;

namespace NajlotLog.Implementation
{
	internal class ConsoleLoggerImplementation : LoggerImplementationBase
	{
		public ConsoleLoggerImplementation(ILogConfiguration configuration) : base(configuration)
		{

		}

		protected override void Log(LogMessage message)
		{
			Console.WriteLine(Format(message));
		}
	}
}
