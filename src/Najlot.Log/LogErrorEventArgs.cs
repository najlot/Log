using System;

namespace Najlot.Log
{
	public class LogErrorEventArgs : EventArgs
	{
		public string Message { get; internal set; }
		public Exception Exception { get; internal set; }
	}
}
