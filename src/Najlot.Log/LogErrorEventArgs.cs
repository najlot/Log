using System;

namespace Najlot.Log;

/// <summary>
/// Arguments of an error-event
/// </summary>
public class LogErrorEventArgs : EventArgs
{
	public string Message { get; internal set; }
	public Exception Exception { get; internal set; }
}
