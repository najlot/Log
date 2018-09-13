﻿using Najlot.Log.Configuration;
using System;

namespace Najlot.Log
{
	/// <summary>
	/// Class to be used for logging.
	/// Can bundle multiple log destinations and decides whether the message should be logged
	/// </summary>
	public sealed class Logger : ILogger, IConfigurationChangedObserver, IDisposable
	{
		private readonly InternalLogger internalLogger;

		private bool LogTrace = false;
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

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= _logLevel;
		}

		private LogLevel _logLevel;
		private readonly ILogConfiguration _logConfiguration;
		private bool _disposed = false;

		private void SetupLogLevel(LogLevel logLevel)
		{
			_logLevel = logLevel;

			LogTrace = false;
			LogFatal = false;
			LogDebug = false;
			LogInfo = false;
			LogWarn = false;
			LogError = false;

			if (logLevel == LogLevel.None)
			{
				return;
			}

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

			if (logLevel == LogLevel.Debug)
			{
				return;
			}

			if (logLevel == LogLevel.Trace)
			{
				// Must be carefull with this one...
				LogTrace = true;
			}
		}

		public void Trace<T>(T o)
		{
			if (LogTrace)
			{
				internalLogger.Trace(o);
			}
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
		
		public IDisposable BeginScope<T>(T state)
		{
			return internalLogger.BeginScope(state);
		}

		public void Trace<T>(T o, Exception ex)
		{
			internalLogger.Trace(o, ex);
		}

		public void Debug<T>(T o, Exception ex)
		{
			internalLogger.Debug(o, ex);
		}

		public void Error<T>(T o, Exception ex)
		{
			internalLogger.Error(o, ex);
		}

		public void Fatal<T>(T o, Exception ex)
		{
			internalLogger.Fatal(o, ex);
		}

		public void Info<T>(T o, Exception ex)
		{
			internalLogger.Info(o, ex);
		}

		public void Warn<T>(T o, Exception ex)
		{
			internalLogger.Warn(o, ex);
		}

		public void Dispose()
		{
			if(!_disposed)
			{
				_disposed = true;
				_logConfiguration.DetachObserver(this);
			}
		}
	}
}
