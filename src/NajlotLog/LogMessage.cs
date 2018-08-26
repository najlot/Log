using System;

namespace NajlotLog
{
    public class LogMessage
	{
		public DateTime DateTime { get; private set; }
		public LogLevel LogLevel { get; private set; }
		public Type SourceType { get; private set; }
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
