using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

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
		/// <param name="messages">Messages to be logged</param>
		/// <param name="formatMiddleware">Middleware to be used for formatting</param>
		void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware);
	}
}