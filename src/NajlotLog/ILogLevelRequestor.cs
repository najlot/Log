namespace NajlotLog
{
	public interface ILogLevelRequestor
	{
		ILoggerRequestor SetLogLevel(LogLevel debugLogLevel, LogLevel releaseLogLevel);
	}
}
