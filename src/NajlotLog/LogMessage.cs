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
		/// Type of the class logging was requested for
		/// </summary>
		public Type SourceType { get; private set; }

		/// <summary>
		/// The instance that was given to the request
		/// </summary>
		public object Message { get; private set; }

		public LogMessage(DateTime dateTime, LogLevel logLevel, Type sourceType, object message)
		{
			DateTime = dateTime;
			LogLevel = logLevel;
			SourceType = sourceType;
			Message = message;
		}
	}
}
