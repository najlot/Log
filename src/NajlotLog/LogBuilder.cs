namespace NajlotLog
{
	public class LogBuilder
	{
		public static ILogLevelRequestor New()
		{
			return new LogLevelRequestor();
		}
	}
}
