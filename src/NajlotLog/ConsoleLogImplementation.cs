using System;

namespace NajlotLog
{
	internal class ConsoleLogImplementation : LogImplementationBase, ILog
	{
		protected override void Log(string msg)
		{
			Console.Write(msg);
		}
	}
}
