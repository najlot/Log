using System;
using System.Collections.Generic;
using System.Threading;
using Najlot.Log.Configuration;
using Najlot.Log.Middleware;
using Najlot.Log.Util;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Base implementation for a log destination.
	/// </summary>
	public abstract class LogDestinationBase : LogDestinationPrototype<LogDestinationBase>, ILogger, IConfigurationChangedObserver, IDisposable
	{
		private IExecutionMiddleware _middleware;
		private ReaderWriterLockSlim _configurationChangeLock = new ReaderWriterLockSlim();
		private ILogConfiguration _logConfiguration;
		private object _currentState;
		private Stack<object> _states = new Stack<object>();

		protected Func<LogMessage, string> Format = (message) =>
		{
			string timestamp = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var category = message.Category ?? "";
			string delimiter = " - ";

			var formatted = string.Concat(timestamp,
				delimiter, message.LogLevel.ToString().ToUpper(),
				delimiter, category,
				delimiter, message.State,
				delimiter, message.Message);

			return message.ExceptionIsValid ? formatted + message.Exception.ToString() : formatted;
		};
		
		public LogDestinationBase(ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;

			_middleware = logConfiguration.ExecutionMiddleware;
			
			Func<LogMessage, string> format;
			if (logConfiguration.TryGetFormatFunctionForType(this.GetType(), out format))
			{
				Format = format;
			}

			logConfiguration.AttachObserver(this);
		}

		public IDisposable BeginScope<T>(T state)
		{
			lock(_states)
			{
				_states.Push(_currentState);
				_currentState = state;
			}
			
			return new OnDisposeExcecutor(() =>
			{
				lock (_states)
				{
					if (_states.Count > 0)
					{
						_currentState = _states.Pop();
					}
					else
					{
						_currentState = null;
					}
				}
			});
		}
		
		protected abstract void Log(LogMessage message);

		public void Trace<T>(T o)
		{
			var time = DateTime.Now;
			
			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Trace, Category, _currentState, o));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Debug<T>(T o)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Debug, Category, _currentState, o));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Info<T>(T o)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Info, Category, _currentState, o));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Warn<T>(T o)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Warn, Category, _currentState, o));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Error<T>(T o)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Error, Category, _currentState, o));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Fatal<T>(T o)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Fatal, Category, _currentState, o));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Trace<T>(T o, Exception ex)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Trace, Category, _currentState, o, ex));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Debug<T>(T o, Exception ex)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Debug, Category, _currentState, o, ex));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Error<T>(T o, Exception ex)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Error, Category, _currentState, o, ex));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Fatal<T>(T o, Exception ex)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Fatal, Category, _currentState, o, ex));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Info<T>(T o, Exception ex)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Info, Category, _currentState, o, ex));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Warn<T>(T o, Exception ex)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Warn, Category, _currentState, o, ex));
				});
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			try
			{
				_configurationChangeLock.EnterWriteLock();

				if (_middleware != configuration.ExecutionMiddleware)
				{
					_middleware = configuration.ExecutionMiddleware;
				}

				Func<LogMessage, string> format;
				if (configuration.TryGetFormatFunctionForType(this.GetType(), out format))
				{
					if (Format != format)
					{
						_middleware = configuration.ExecutionMiddleware;
					}
				}
			}
			finally
			{
				_configurationChangeLock.ExitWriteLock();
			}
		}

		public void Flush()
		{
			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Flush();
			}
			finally
			{
				_configurationChangeLock.ExitReadLock();
			}
		}

		public void Dispose()
		{
			_logConfiguration.DetachObserver(this);
		}
	}
}
