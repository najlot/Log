using System;

namespace Najlot.Log
{
	/// <summary>
	/// Class to be used for logging.
	/// Can bundle multiple log destinations and decides whether the message should be logged
	/// </summary>
	public sealed class Logger : ILogger, IConfigurationChangedObserver
	{
		private readonly LogExecutor _logExecutor;

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

			_logExecutor = logExecutor;
			SetupLogLevel(logConfiguration.LogLevel);
		}

		public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevel;

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
				LogTrace = true;
			}
		}

		public IDisposable BeginScope<T>(T state) => _logExecutor.BeginScope(state);

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

		public void Trace<T>(Exception ex, T o)
		{
			if (LogTrace) _logExecutor.Trace(ex, o);
		}

		public void Debug<T>(Exception ex, T o)
		{
			if (LogDebug) _logExecutor.Debug(ex, o);
		}

		public void Info<T>(Exception ex, T o)
		{
			if (LogInfo) _logExecutor.Info(ex, o);
		}

		public void Warn<T>(Exception ex, T o)
		{
			if (LogWarn) _logExecutor.Warn(ex, o);
		}

		public void Error<T>(Exception ex, T o)
		{
			if (LogError) _logExecutor.Error(ex, o);
		}

		public void Fatal<T>(Exception ex, T o)
		{
			if (LogFatal) _logExecutor.Fatal(ex, o);
		}

		public void Trace(string s, params object[] args)
		{
			if (LogTrace) _logExecutor.Trace(s, args);
		}

		public void Debug(string s, params object[] args)
		{
			if (LogDebug) _logExecutor.Debug(s, args);
		}

		public void Info(string s, params object[] args)
		{
			if (LogInfo) _logExecutor.Info(s, args);
		}

		public void Warn(string s, params object[] args)
		{
			if (LogWarn) _logExecutor.Warn(s, args);
		}

		public void Error(string s, params object[] args)
		{
			if (LogError) _logExecutor.Error(s, args);
		}

		public void Fatal(string s, params object[] args)
		{
			if (LogFatal) _logExecutor.Fatal(s, args);
		}

		public void Trace(Exception ex, string s, params object[] args)
		{
			if (LogTrace) _logExecutor.Trace(ex, s, args);
		}

		public void Debug(Exception ex, string s, params object[] args)
		{
			if (LogDebug) _logExecutor.Debug(ex, s, args);
		}

		public void Info(Exception ex, string s, params object[] args)
		{
			if (LogInfo) _logExecutor.Info(ex, s, args);
		}

		public void Warn(Exception ex, string s, params object[] args)
		{
			if (LogWarn) _logExecutor.Warn(ex, s, args);
		}

		public void Error(Exception ex, string s, params object[] args)
		{
			if (LogError) _logExecutor.Error(ex, s, args);
		}

		public void Fatal(Exception ex, string s, params object[] args)
		{
			if (LogFatal) _logExecutor.Fatal(ex, s, args);
		}

		public void Flush() => _logExecutor.Flush();

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			if (_logLevel != configuration.LogLevel)
			{
				SetupLogLevel(configuration.LogLevel);
			}
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
					_logExecutor.Dispose();
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