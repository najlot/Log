using System;

namespace Najlot.Log
{
	/// <summary>
	/// The message to log
	/// </summary>
    public class LogMessage
	{
		/// <summary>
		/// Timestamp the logging was requested
		/// </summary>
		public DateTime DateTime { get; }

		/// <summary>
		/// Logging level it was requested to log with
		/// </summary>
		public LogLevel LogLevel { get; }

		/// <summary>
		/// Category the logger was requested for
		/// </summary>
		public string Category { get; }

		/// <summary>
		/// State set with BeginScope
		/// </summary>
		public object State { get; }

		/// <summary>
		/// The instance that was given to the request
		/// </summary>
		public object Message { get; }
		
		/// <summary>
		/// Exception, if got any, otherwise null. Check ExceptionIsValid
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Specifying whether the exception is null or not
		/// </summary>
		public bool ExceptionIsValid { get; }

		public LogMessage(DateTime dateTime, LogLevel logLevel, string category, object state, object message)
		{
			DateTime = dateTime;
			LogLevel = logLevel;
			Category = category;
			State = state;
			Message = message;

			ExceptionIsValid = false;
		}

		public LogMessage(DateTime dateTime, LogLevel logLevel, string category, object state, object message, Exception ex)
		{
			DateTime = dateTime;
			LogLevel = logLevel;
			Category = category;
			State = state;
			Message = message;
			Exception = ex;

			ExceptionIsValid = ex != null;
		}
	}
}
