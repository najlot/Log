using System;

namespace NajlotLog
{
	internal class AsyncConsoleLogImplementation : AsyncLogImplementationBase, ILog
	{
		protected override void Log(string msg)
		{
			Console.Write(msg);
		}
	}
}
