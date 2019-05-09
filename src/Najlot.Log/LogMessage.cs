using System;
using System.Collections.Generic;

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
		/// Logging level logging was requested with
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
		public string Message { get; }

		/// <summary>
		/// Exception, if got any, otherwise null. Check ExceptionIsValid
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Specifying whether the exception is null or not
		/// </summary>
		public bool ExceptionIsValid { get; }

		/// <summary>
		/// Arguments received through one of the Trace / Debug / Info / Warn / Error / Fatal functions
		/// </summary>
		public IReadOnlyList<KeyValuePair<string, object>> Arguments { get; }

		public LogMessage(DateTime dateTime,
			LogLevel logLevel,
			string category,
			object state,
			string message,
			Exception ex,
			IReadOnlyList<KeyValuePair<string, object>> args)
		{
			DateTime = dateTime;
			LogLevel = logLevel;
			Category = category;
			State = state;
			Message = message;
			Exception = ex;
			Arguments = args;

			ExceptionIsValid = ex != null;
		}
	}
}