using System;

namespace NajlotLog
{
	internal class Log : ILog
	{
		private bool DebugLog = false;
		private bool InfoLog = false;
		private bool WarnLog = false;
		private bool ErrorLog = false;
		private bool FatalLog = false;
		private ILog Logger;

		public Log(LogLevel logLevel, ILog log)
		{
			Logger = log ?? throw new ArgumentNullException(nameof(log));
			SetupLogLevel(logLevel);
		}
		
		private void SetupLogLevel(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Debug:
					DebugLog = true;
					InfoLog = true;
					WarnLog = true;
					ErrorLog = true;
					FatalLog = true;
					break;
				case LogLevel.Info:
					InfoLog = true;
					WarnLog = true;
					ErrorLog = true;
					FatalLog = true;
					break;
				case LogLevel.Warn:
					WarnLog = true;
					ErrorLog = true;
					FatalLog = true;
					break;
				case LogLevel.Error:
					FatalLog = true;
					break;
				case LogLevel.Fatal:
					FatalLog = true;
					break;
			}
		}

		public void Debug(object o)
		{
			if (DebugLog)
			{
				Logger.Debug(o);
			}
		}

		public void Info(object o)
		{
			if (InfoLog)
			{
				Logger.Info(o);
			}
		}

		public void Warn(object o)
		{
			if (WarnLog)
			{
				Logger.Warn(o);
			}
		}

		public void Error(object o)
		{
			if (ErrorLog)
			{
				Logger.Error(o);
			}
		}

		public void Fatal(object o)
		{
			if (FatalLog)
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
