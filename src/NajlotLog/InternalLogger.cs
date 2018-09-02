namespace NajlotLog
{
	/// <summary>
	/// This class speeds up the execution when not logging.
	/// Implementing the ILogger interface there makes execution slower... Does not matter - it is internal
	/// </summary>
	internal class InternalLogger
	{
		private ILogger Log;

		public InternalLogger(ILogger log)
		{
			Log = log;
		}

		public void Debug<T>(T o)
		{
			Log.Debug(o);
		}

		public void Info<T>(T o)
		{
			Log.Info(o);
		}

		public void Warn<T>(T o)
		{
			Log.Warn(o);
		}

		public void Error<T>(T o)
		{
			Log.Error(o);
		}

		public void Fatal<T>(T o)
		{
			Log.Fatal(o);
		}

		public void Flush()
		{
			Log.Flush();
		}
	}
}
