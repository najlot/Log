namespace NajlotLog
{
	internal class LogLevelRequestor : ILogLevelRequestor
	{
		public ILoggerRequestor SetLogLevel(LogLevel debugLogLevel, LogLevel releaseLogLevel)
		{
			LogLevel logLevel;
#if DEBUG
			logLevel = debugLogLevel;
#else
			logLevel = releaseLogLevel;
#endif
			return new LoggerRequestor(logLevel);
		}
	}
}
