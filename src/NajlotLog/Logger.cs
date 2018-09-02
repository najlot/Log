using NajlotLog.Configuration;
using System;

namespace NajlotLog
{
	public class Logger : ILogger, IConfigurationChangedObserver, IDisposable
	{
		private InternalLogger internalLogger;

		private bool LogDebug = false;
		private bool LogInfo = false;
		private bool LogWarn = false;
		private bool LogError = false;
		private bool LogFatal = false;

		internal Logger(ILogger log, ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
			_logConfiguration.AttachObserver(this);

			internalLogger = new InternalLogger(log ?? throw new ArgumentNullException(nameof(log)));
			SetupLogLevel(logConfiguration.LogLevel);
		}

		private LogLevel _logLevel;
		private ILogConfiguration _logConfiguration;

		private void SetupLogLevel(LogLevel logLevel)
		{
			_logLevel = logLevel;

			LogDebug = false;
			LogInfo = false;
			LogWarn = false;
			LogError = false;

			LogFatal = true;
			if (logLevel == LogLevel.Fatal)
			{
				return;
			}

			LogError = true;
			if (logLevel == LogLevel.Error)
			{
				return;
			}

			LogWarn = true;
			if (logLevel == LogLevel.Warn)
			{
				return;
			}

			LogInfo = true;
			if (logLevel == LogLevel.Info)
			{
				return;
			}

			LogDebug = true;
		}

		public void Debug<T>(T o)
		{
			if (LogDebug)
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
			if (_logLevel != configuration.LogLevel)
			{
				_logLevel = configuration.LogLevel;
				SetupLogLevel(_logLevel);
			}
		}

		public void Dispose()
		{
			_logConfiguration.DetachObserver(this);
		}
	}
}
