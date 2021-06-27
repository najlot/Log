// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log
{
	/// <summary>
	/// Class to be used for logging.
	/// Can bundle multiple destinations and decides whether the message should be logged depending on the loglevel
	/// </summary>
	public sealed class Logger : ILogger
	{
		private readonly LogExecutor _logExecutor;

		private volatile bool _logTrace;
		private volatile bool _logDebug;
		private volatile bool _logInfo;
		private volatile bool _logWarn;
		private volatile bool _logError;
		private volatile bool _logFatal;

		private static readonly object[] _emptyArgs = Array.Empty<object>();

		internal Logger(LogExecutor logExecutor)
		{
			_logExecutor = logExecutor;
		}

		public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevel;

		private volatile LogLevel _logLevel;

		internal void SetupLogLevel(LogLevel logLevel)
		{
			_logLevel = logLevel;

			_logTrace = false;
			_logFatal = false;
			_logDebug = false;
			_logInfo = false;
			_logWarn = false;
			_logError = false;

			if (logLevel == LogLevel.None)
			{
				return;
			}

			_logFatal = true;
			if (logLevel == LogLevel.Fatal)
			{
				return;
			}

			_logError = true;
			if (logLevel == LogLevel.Error)
			{
				return;
			}

			_logWarn = true;
			if (logLevel == LogLevel.Warn)
			{
				return;
			}

			_logInfo = true;
			if (logLevel == LogLevel.Info)
			{
				return;
			}

			_logDebug = true;
			if (logLevel == LogLevel.Debug)
			{
				return;
			}

			if (logLevel == LogLevel.Trace)
			{
				_logTrace = true;
			}
		}

		public IDisposable BeginScope<T>(T state) => _logExecutor.BeginScope(state);

		public void Trace<T>(T o)
		{
			if (_logTrace) _logExecutor.Log(LogLevel.Trace, null, o, _emptyArgs);
		}

		public void Trace<T>(Exception ex, T o)
		{
			if (_logTrace) _logExecutor.Log(LogLevel.Trace, ex, o, _emptyArgs);
		}

		public void Trace(string s, params object[] args)
		{
			if (_logTrace) _logExecutor.Log(LogLevel.Trace, null, s, args);
		}

		public void Trace(Exception ex, string s, params object[] args)
		{
			if (_logTrace) _logExecutor.Log(LogLevel.Trace, ex, s, args);
		}

		public void Debug<T>(T o)
		{
			if (_logDebug) _logExecutor.Log(LogLevel.Debug, null, o, _emptyArgs);
		}

		public void Debug<T>(Exception ex, T o)
		{
			if (_logDebug) _logExecutor.Log(LogLevel.Debug, ex, o, _emptyArgs);
		}

		public void Debug(string s, params object[] args)
		{
			if (_logDebug) _logExecutor.Log(LogLevel.Debug, null, s, args);
		}

		public void Debug(Exception ex, string s, params object[] args)
		{
			if (_logDebug) _logExecutor.Log(LogLevel.Debug, ex, s, args);
		}

		public void Info<T>(T o)
		{
			if (_logInfo) _logExecutor.Log(LogLevel.Info, null, o, _emptyArgs);
		}

		public void Info<T>(Exception ex, T o)
		{
			if (_logInfo) _logExecutor.Log(LogLevel.Info, ex, o, _emptyArgs);
		}

		public void Info(string s, params object[] args)
		{
			if (_logInfo) _logExecutor.Log(LogLevel.Info, null, s, args);
		}

		public void Info(Exception ex, string s, params object[] args)
		{
			if (_logInfo) _logExecutor.Log(LogLevel.Info, ex, s, args);
		}

		public void Warn<T>(T o)
		{
			if (_logWarn) _logExecutor.Log(LogLevel.Warn, null, o, _emptyArgs);
		}

		public void Warn<T>(Exception ex, T o)
		{
			if (_logWarn) _logExecutor.Log(LogLevel.Warn, ex, o, _emptyArgs);
		}

		public void Warn(string s, params object[] args)
		{
			if (_logWarn) _logExecutor.Log(LogLevel.Warn, null, s, args);
		}

		public void Warn(Exception ex, string s, params object[] args)
		{
			if (_logWarn) _logExecutor.Log(LogLevel.Warn, ex, s, args);
		}

		public void Error<T>(T o)
		{
			if (_logError) _logExecutor.Log(LogLevel.Error, null, o, _emptyArgs);
		}

		public void Error<T>(Exception ex, T o)
		{
			if (_logError) _logExecutor.Log(LogLevel.Error, ex, o, _emptyArgs);
		}

		public void Error(string s, params object[] args)
		{
			if (_logError) _logExecutor.Log(LogLevel.Error, null, s, args);
		}

		public void Error(Exception ex, string s, params object[] args)
		{
			if (_logError) _logExecutor.Log(LogLevel.Error, ex, s, args);
		}

		public void Fatal<T>(T o)
		{
			if (_logFatal) _logExecutor.Log(LogLevel.Fatal, null, o, _emptyArgs);
		}

		public void Fatal<T>(Exception ex, T o)
		{
			if (_logFatal) _logExecutor.Log(LogLevel.Fatal, ex, o, _emptyArgs);
		}

		public void Fatal(string s, params object[] args)
		{
			if (_logFatal) _logExecutor.Log(LogLevel.Fatal, null, s, args);
		}

		public void Fatal(Exception ex, string s, params object[] args)
		{
			if (_logFatal) _logExecutor.Log(LogLevel.Fatal, ex, s, args);
		}

		public void Flush() => _logExecutor.Flush();
	}
}