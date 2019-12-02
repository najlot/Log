// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log
{
	/// <summary>
	/// Class to be used for logging.
	/// Can bundle multiple log destinations and decides whether the message should be logged
	/// </summary>
	public sealed class Logger : ILogger, IConfigurationObserver
	{
		private readonly LogExecutor _logExecutor;

		private volatile bool LogTrace = false;
		private volatile bool LogDebug = false;
		private volatile bool LogInfo = false;
		private volatile bool LogWarn = false;
		private volatile bool LogError = false;
		private volatile bool LogFatal = false;

		private static readonly object[] _emptyArgs = Array.Empty<object>();

		internal Logger(LogExecutor logExecutor, ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
			_logConfiguration.AttachObserver(this);

			_logExecutor = logExecutor;
			SetupLogLevel(logConfiguration.LogLevel);
		}

		public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevel;

		private volatile LogLevel _logLevel;
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
			if (LogTrace) _logExecutor.Log(LogLevel.Trace, null, o, _emptyArgs);
		}

		public void Trace<T>(Exception ex, T o)
		{
			if (LogTrace) _logExecutor.Log(LogLevel.Trace, ex, o, _emptyArgs);
		}

		public void Trace(string s, params object[] args)
		{
			if (LogTrace) _logExecutor.Log(LogLevel.Trace, null, s, args);
		}

		public void Trace(Exception ex, string s, params object[] args)
		{
			if (LogTrace) _logExecutor.Log(LogLevel.Trace, ex, s, args);
		}

		public void Debug<T>(T o)
		{
			if (LogDebug) _logExecutor.Log(LogLevel.Debug, null, o, _emptyArgs);
		}

		public void Debug<T>(Exception ex, T o)
		{
			if (LogDebug) _logExecutor.Log(LogLevel.Debug, ex, o, _emptyArgs);
		}

		public void Debug(string s, params object[] args)
		{
			if (LogDebug) _logExecutor.Log(LogLevel.Debug, null, s, args);
		}

		public void Debug(Exception ex, string s, params object[] args)
		{
			if (LogDebug) _logExecutor.Log(LogLevel.Debug, ex, s, args);
		}

		public void Info<T>(T o)
		{
			if (LogInfo) _logExecutor.Log(LogLevel.Info, null, o, _emptyArgs);
		}

		public void Info<T>(Exception ex, T o)
		{
			if (LogInfo) _logExecutor.Log(LogLevel.Info, ex, o, _emptyArgs);
		}

		public void Info(string s, params object[] args)
		{
			if (LogInfo) _logExecutor.Log(LogLevel.Info, null, s, args);
		}

		public void Info(Exception ex, string s, params object[] args)
		{
			if (LogInfo) _logExecutor.Log(LogLevel.Info, ex, s, args);
		}

		public void Warn<T>(T o)
		{
			if (LogWarn) _logExecutor.Log(LogLevel.Warn, null, o, _emptyArgs);
		}

		public void Warn<T>(Exception ex, T o)
		{
			if (LogWarn) _logExecutor.Log(LogLevel.Warn, ex, o, _emptyArgs);
		}

		public void Warn(string s, params object[] args)
		{
			if (LogWarn) _logExecutor.Log(LogLevel.Warn, null, s, args);
		}

		public void Warn(Exception ex, string s, params object[] args)
		{
			if (LogWarn) _logExecutor.Log(LogLevel.Warn, ex, s, args);
		}

		public void Error<T>(T o)
		{
			if (LogError) _logExecutor.Log(LogLevel.Error, null, o, _emptyArgs);
		}

		public void Error<T>(Exception ex, T o)
		{
			if (LogError) _logExecutor.Log(LogLevel.Error, ex, o, _emptyArgs);
		}

		public void Error(string s, params object[] args)
		{
			if (LogError) _logExecutor.Log(LogLevel.Error, null, s, args);
		}

		public void Error(Exception ex, string s, params object[] args)
		{
			if (LogError) _logExecutor.Log(LogLevel.Error, ex, s, args);
		}

		public void Fatal<T>(T o)
		{
			if (LogFatal) _logExecutor.Log(LogLevel.Fatal, null, o, _emptyArgs);
		}

		public void Fatal<T>(Exception ex, T o)
		{
			if (LogFatal) _logExecutor.Log(LogLevel.Fatal, ex, o, _emptyArgs);
		}

		public void Fatal(string s, params object[] args)
		{
			if (LogFatal) _logExecutor.Log(LogLevel.Fatal, null, s, args);
		}

		public void Fatal(Exception ex, string s, params object[] args)
		{
			if (LogFatal) _logExecutor.Log(LogLevel.Fatal, ex, s, args);
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