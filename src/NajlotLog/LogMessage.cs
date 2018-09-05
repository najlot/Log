using System;

namespace NajlotLog
{
	/// <summary>
	/// The message to log
	/// </summary>
    public class LogMessage
	{
		/// <summary>
		/// Timespamp the logging was requested
		/// </summary>
		public DateTime DateTime { get; private set; }

		/// <summary>
		/// Logging level it was requested to log with
		/// </summary>
		public LogLevel LogLevel { get; private set; }

		/// <summary>
		/// Category the logger was requested for
		/// </summary>
		public string Category { get; private set; }

		/// <summary>
		/// The instance that was given to the request
		/// </summary>
		public object Message { get; private set; }

		/// <summary>
		/// State set with BeginScope
		/// </summary>
		public object State { get; private set; }

		public LogMessage(DateTime dateTime, LogLevel logLevel, string category, object state, object message)
		{
			DateTime = dateTime;
			LogLevel = logLevel;
			Category = category;
			State = state;
			Message = message;
		}
	}
}
