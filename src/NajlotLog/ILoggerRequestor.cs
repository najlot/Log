namespace NajlotLog
{
	public interface ILoggerRequestor
	{
		ILoggerRequestor AppendConsoleLog();
		ILoggerRequestor AppendFileLog(string path);
		ILoggerRequestor AppendCustomLog(ILog log);

		Log Build();
	}
}
