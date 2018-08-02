namespace NajlotLog
{
	public class LogBuilder
	{
		public static ILogLevelRequestor New
		{
			get
			{
				return new LogLevelRequestor();
			}
		}
	}
}
