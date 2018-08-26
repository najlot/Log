using NajlotLog.Configuration;
using System;

namespace NajlotLog
{
	public class Logger : ILogger, IConfigurationChangedObserver, IDisposable
	{
		private InternalLogger internalLogger;

		/// <summary>
		/// This class speeds up the execution when not logging.
		/// Implementing the ILogger interface there makes execution slower... Does not matter - it is internal
		/// </summary>
		private class InternalLogger
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

		private bool LogDebug = false;
		private bool LogInfo = false;
		private bool LogWarn = false;
		private bool LogError = false;
		private bool LogFatal = false;
		
		internal Logger(LogLevel logLevel, ILogger log)
		{
			LogConfiguration.Instance.AttachObserver(this);

			internalLogger = new InternalLogger(log ?? throw new ArgumentNullException(nameof(log)));
			SetupLogLevel(logLevel);
		}

		private LogLevel _logLevel;

		private void SetupLogLevel(LogLevel logLevel)
		{
			_logLevel = logLevel;

			LogDebug = false;
			LogInfo = false;
			LogWarn = false;
			LogError = false;
			LogFatal = false;

			// TODO
			/*if(logLevel >= LogLevel.Fatal)
			{

			}*/

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
		
		public void Debug<T>(T o)
		{
			if(LogDebug)
			{
				internalLogger.Debug(o);
			}
		}
		
		public void Info<T>(T o)
		{
			if (LogInfo)
			{
				internalLogger.Info(o);
			}
		}

		public void Warn<T>(T o)
		{
			if (LogWarn)
			{
				internalLogger.Warn(o);
			}
		}

		public void Error<T>(T o)
		{
			if (LogError)
			{
				internalLogger.Error(o);
			}
		}

		public void Fatal<T>(T o)
		{
			if (LogFatal)
			{
				internalLogger.Fatal(o);
			}
		}
		
		public void Flush()
		{
			internalLogger.Flush();
		}
		
		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			if(_logLevel != configuration.LogLevel)
			{
				_logLevel = configuration.LogLevel;
				SetupLogLevel(_logLevel);
			}
		}

		public void Dispose()
		{
			LogConfiguration.Instance.DetachObserver(this);
		}
	}
}
