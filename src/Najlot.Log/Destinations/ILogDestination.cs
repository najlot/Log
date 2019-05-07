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
		/// Tells the destination to log the message
		/// </summary>
		/// <param name="message">Message to be logged</param>
		/// <param name="formatMiddleware">Middleware to be used for formatting</param>
		void Log(LogMessage message, IFormatMiddleware formatMiddleware);
	}
}