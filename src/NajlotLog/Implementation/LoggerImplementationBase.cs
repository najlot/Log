using System;
using System.Threading;
using NajlotLog.Configuration;
using NajlotLog.Middleware;

namespace NajlotLog.Implementation
{
	public abstract class LoggerImplementationBase : LoggerPrototype<LoggerImplementationBase>, ILogger, IConfigurationChangedObserver, IDisposable
	{
		private ILogExecutionMiddleware _middleware;
		private ReaderWriterLockSlim _configurationChangeLock = new ReaderWriterLockSlim();

		protected Func<LogMessage, string> Format = (message) =>
		{
			string timestamp = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var sourceTypeName = message.SourceType == null ? "" : message.SourceType.Name;
			return $"{timestamp} {message.LogLevel.ToString().ToUpper()} {sourceTypeName} {message.Message}";
		};

		public LoggerImplementationBase()
		{
			_middleware = LogConfiguration.Instance.LogExecutionMiddleware;

			if(_middleware == null)
			{
				System.Diagnostics.Debugger.Break();
			}

			Func<LogMessage, string> format;
			if (LogConfiguration.Instance.TryGetFormatFunctionForType(this.GetType(), out format))
			{
				Format = format;
			}

			LogConfiguration.Instance.AttachObserver(this);
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

		protected abstract void Log(LogMessage message);

		public void Debug<T>(T o)
		{
			var time = DateTime.Now;

			try
			{
				_configurationChangeLock.EnterReadLock();
				_middleware.Execute(() =>
				{
					Log(new LogMessage(time, LogLevel.Debug, SourceType, o));
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
					Log(new LogMessage(time, LogLevel.Info, SourceType, o));
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
					Log(new LogMessage(time, LogLevel.Warn, SourceType, o));
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
					Log(new LogMessage(time, LogLevel.Error, SourceType, o));
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
					Log(new LogMessage(time, LogLevel.Fatal, SourceType, o));
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

				if (_middleware != configuration.LogExecutionMiddleware)
				{
					_middleware = configuration.LogExecutionMiddleware;
				}

				Func<LogMessage, string> format;
				if (configuration.TryGetFormatFunctionForType(this.GetType(), out format))
				{
					if (Format != format)
					{
						_middleware = configuration.LogExecutionMiddleware;
					}
				}
			}
			finally
			{
				_configurationChangeLock.ExitWriteLock();
			}
		}

		public void Dispose()
		{
			LogConfiguration.Instance.DetachObserver(this);
		}
	}
}
