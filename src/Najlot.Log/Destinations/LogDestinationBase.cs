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
		private readonly ReaderWriterLockSlim _configurationChangeLock = new ReaderWriterLockSlim();
		private readonly ILogConfiguration _logConfiguration;
		private object _currentState;
		private readonly Stack<object> _states = new Stack<object>();

		protected Func<LogMessage, string> Format = (message) =>
		{
			string timestamp = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var category = message.Category ?? "";
			string delimiter = " - ";
			string logLevel = message.LogLevel.ToString().ToUpper();
			
			if(logLevel.Length == 4)
			{
				logLevel += ' ';
			}
			
			var formatted = string.Concat(timestamp,
				delimiter, logLevel,
				delimiter, category,
				delimiter, message.State,
				delimiter, message.Message);

			return message.ExceptionIsValid ? formatted + message.Exception.ToString() : formatted;
		};
		
		protected LogDestinationBase(ILogConfiguration logConfiguration)
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Trace, Category, state, o));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Debug, Category, state, o));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Info, Category, state, o));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Warn, Category, state, o));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Error, Category, state, o));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Fatal, Category, state, o));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Trace, Category, state, o, ex));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Debug, Category, state, o, ex));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Error, Category, state, o, ex));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Fatal, Category, state, o, ex));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Info, Category, state, o, ex));
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
			var state = _currentState;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Warn, Category, state, o, ex));
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

				_middleware = configuration.ExecutionMiddleware;

				if (configuration.TryGetFormatFunctionForType(this.GetType(), out Func<LogMessage, string> format))
				{
					Format = format;
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

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_logConfiguration.DetachObserver(this);
				}
				
				disposedValue = true;
			}
		}
		
		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			Dispose(true);
		}
		#endregion

	}
}
