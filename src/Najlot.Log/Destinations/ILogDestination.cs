using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Common interface for all destinations
	/// </summary>
	public interface ILogDestination : IDisposable
	{
		/// <summary>
		/// Tells the destination to log the messages
		/// </summary>
		/// <param name="logMessageFormattingPair">Messages to be logged</param>
		void Log((LogMessage, IFormatMiddleware)[] logMessageFormattingPair);
	}
}