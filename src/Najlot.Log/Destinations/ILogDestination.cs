using System;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Common interface for all destinations
	/// </summary>
	public interface ILogDestination : IDisposable
	{
		/// <summary>
		/// Tells the destination to take the message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="formatFunc"></param>
		void Log(LogMessage message, Func<LogMessage, string> formatFunc);
	}
}