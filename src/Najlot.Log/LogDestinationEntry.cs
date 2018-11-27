using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Threading;

namespace Najlot.Log
{
	internal sealed class LogDestinationEntry : IConfigurationChangedObserver
	{
		private readonly ReaderWriterLockSlim _logDestinationLock = new ReaderWriterLockSlim();
		private readonly ReaderWriterLockSlim _formatFuncLock = new ReaderWriterLockSlim();
		private readonly ReaderWriterLockSlim _executionMiddlewareLock = new ReaderWriterLockSlim();

		private ILogDestination _logDestination;
		private Func<LogMessage, string> _formatFunc;
		private IExecutionMiddleware _executionMiddleware;

		public ILogDestination LogDestination
		{
			get
			{
				_logDestinationLock.EnterReadLock();

				try
				{
					return _logDestination;
				}
				finally
				{
					_logDestinationLock.ExitReadLock();
				}
			}
			set
			{
				_logDestinationLock.EnterWriteLock();

				try
				{
					_logDestination = value;
				}
				finally
				{
					_logDestinationLock.ExitWriteLock();
				}
			}
		}

		public Func<LogMessage, string> FormatFunc
		{
			get
			{
				_formatFuncLock.EnterReadLock();

				try
				{
					return _formatFunc;
				}
				finally
				{
					_formatFuncLock.ExitReadLock();
				}
			}
			set
			{
				_formatFuncLock.EnterWriteLock();

				try
				{
					_formatFunc = value;
				}
				finally
				{
					_formatFuncLock.ExitWriteLock();
				}
			}
		}

		public IExecutionMiddleware ExecutionMiddleware
		{
			get
			{
				_executionMiddlewareLock.EnterReadLock();

				try
				{
					return _executionMiddleware;
				}
				finally
				{
					_executionMiddlewareLock.ExitReadLock();
				}
			}
			set
			{
				_executionMiddlewareLock.EnterWriteLock();

				try
				{
					_executionMiddleware = value;
				}
				finally
				{
					_executionMiddlewareLock.ExitWriteLock();
				}
			}
		}

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			if (configuration.TryGetFormatFunctionForType(this.LogDestination.GetType(), out var formatFunc))
			{
				FormatFunc = formatFunc;
			}

			ExecutionMiddleware = configuration.ExecutionMiddleware;
		}
	}
}