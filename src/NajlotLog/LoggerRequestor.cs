using System;

namespace NajlotLog
{
	internal class LoggerRequestor : ILoggerRequestor
	{
		private LogLevel LogLevel;
		LogList LogList = new LogList();

		public LoggerRequestor(LogLevel logLevel)
		{
			LogLevel = logLevel;
		}

		public ILoggerRequestor AppendConsoleLog()
		{
			LogList.Add(new AsyncConsoleLogImplementation());
			return this;
		}

		public ILoggerRequestor AppendCustomLog(ILog log)
		{
			LogList.Add(log);
			return this;
		}

		public ILoggerRequestor AppendFileLog(string path)
		{
			LogList.Add(new AsyncFileLogImplementation(path));
			return this;
		}

		public Log Build()
		{
			if (LogList.Count == 0) throw new InvalidOperationException("No loggers appended");

			if(LogList.Count == 1)
			{
				new Log(LogLevel, LogList[0]);
			}

			return new Log(LogLevel, LogList);
		}
	}
}
