using System;

namespace NajlotLog
{
	public class Log : ILog
	{
		private InternalLogger Logger;

		private bool LogDebug = false;
		private bool LogInfo = false;
		private bool LogWarn = false;
		private bool LogError = false;
		private bool LogFatal = false;
		
		internal Log(LogLevel logLevel, ILog log)
		{
			Logger = new InternalLogger(log ?? throw new ArgumentNullException(nameof(log)));
			SetupLogLevel(logLevel);
		}
		
		private void SetupLogLevel(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Debug:
					LogDebug = true;
					LogInfo = true;
					LogWarn = true;
					LogError = true;
					LogFatal = true;
					break;
				case LogLevel.Info:
					LogInfo = true;
					LogWarn = true;
					LogError = true;
					LogFatal = true;
					break;
				case LogLevel.Warn:
					LogWarn = true;
					LogError = true;
					LogFatal = true;
					break;
				case LogLevel.Error:
					LogError = true;
					LogFatal = true;
					break;
				case LogLevel.Fatal:
					LogFatal = true;
					break;
			}
		}
		
		private class InternalLogger // That class speeds up the execution when not logging.
									 // Implementing the ILog interface there makes execution slower... Does not matter - it is internal
		{
			private ILog Log;

			public InternalLogger(ILog log)
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
		
		public void Debug<T>(T o)
		{
			if(LogDebug)
			{
				Logger.Debug(o);
			}
		}
		
		public void Info<T>(T o)
		{
			if (LogInfo)
			{
				Logger.Info(o);
			}
		}

		public void Warn<T>(T o)
		{
			if (LogWarn)
			{
				Logger.Warn(o);
			}
		}

		public void Error<T>(T o)
		{
			if (LogError)
			{
				Logger.Error(o);
			}
		}

		public void Fatal<T>(T o)
		{
			if (LogFatal)
			{
				Logger.Fatal(o);
			}
		}
		
		public void Flush()
		{
			Logger.Flush();
		}
	}
}
