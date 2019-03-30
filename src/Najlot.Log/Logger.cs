using Najlot.Log.Configuration;
using System;

namespace Najlot.Log
{
	/// <summary>
	/// Class to be used for logging.
	/// Can bundle multiple log destinations and decides whether the message should be logged
	/// </summary>
	public sealed class Logger : ILogger, IConfigurationChangedObserver
	{
		private LogExecutor _logExecutor;

		private bool LogTrace = false;
		private bool LogDebug = false;
		private bool LogInfo = false;
		private bool LogWarn = false;
		private bool LogError = false;
		private bool LogFatal = false;

		internal Logger(LogExecutor logExecutor, ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
			_logConfiguration.AttachObserver(this);

			_logExecutor = logExecutor ?? throw new ArgumentNullException(nameof(logExecutor));
			SetupLogLevel(logConfiguration.LogLevel);
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= _logLevel;
		}

		private LogLevel _logLevel;
		private readonly ILogConfiguration _logConfiguration;

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
			if (LogTrace) _logExecutor.Trace(o);
		}

		public void Debug<T>(T o)
		{
			if (LogDebug) _logExecutor.Debug(o);
		}

		public void Info<T>(T o)
		{
			if (LogInfo) _logExecutor.Info(o);
		}

		public void Warn<T>(T o)
		{
			if (LogWarn) _logExecutor.Warn(o);
		}

		public void Error<T>(T o)
		{
			if (LogError) _logExecutor.Error(o);
		}

		public void Fatal<T>(T o)
		{
			if (LogFatal) _logExecutor.Fatal(o);
		}

		public void Flush() => _logExecutor.Flush();

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			if (_logLevel != configuration.LogLevel)
			{
				_logLevel = configuration.LogLevel;
				SetupLogLevel(_logLevel);
			}
		}

		public IDisposable BeginScope<T>(T state) => _logExecutor.BeginScope(state);

		public void Trace<T>(T o, Exception ex)
		{
			if (LogTrace) _logExecutor.Trace(o, ex);
		}

		public void Debug<T>(T o, Exception ex)
		{
			if (LogDebug) _logExecutor.Debug(o, ex);
		}

		public void Error<T>(T o, Exception ex)
		{
			if (LogError) _logExecutor.Error(o, ex);
		}

		public void Fatal<T>(T o, Exception ex)
		{
			if (LogFatal) _logExecutor.Fatal(o, ex);
		}

		public void Info<T>(T o, Exception ex)
		{
			if (LogInfo) _logExecutor.Info(o, ex);
		}

		public void Warn<T>(T o, Exception ex)
		{
			if (LogWarn) _logExecutor.Warn(o, ex);
		}

		#region IDisposable Support

		private bool _disposedValue = false;

		private void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				_disposedValue = true;

				if (disposing)
				{
					_logConfiguration.DetachObserver(this);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}