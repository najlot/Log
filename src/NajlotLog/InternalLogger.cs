﻿using System;

namespace NajlotLog
{
	/// <summary>
	/// This class speeds up the execution when not logging.
	/// Implementing the ILogger interface there makes execution slower... Does not matter - it is internal
	/// </summary>
	internal class InternalLogger
	{
		private ILogger _log;

		public InternalLogger(ILogger log)
		{
			_log = log;
		}

		internal void Trace<T>(T o)
		{
			_log.Trace(o);
		}

		public void Debug<T>(T o)
		{
			_log.Debug(o);
		}

		public void Info<T>(T o)
		{
			_log.Info(o);
		}

		public void Warn<T>(T o)
		{
			_log.Warn(o);
		}

		public void Error<T>(T o)
		{
			_log.Error(o);
		}

		public void Fatal<T>(T o)
		{
			_log.Fatal(o);
		}

		public void Flush()
		{
			_log.Flush();
		}

		internal IDisposable BeginScope<T>(T state)
		{
			return _log.BeginScope(state);
		}

		internal void Trace<T>(T o, Exception ex)
		{
			_log.Trace(o, ex);
		}

		internal void Debug<T>(T o, Exception ex)
		{
			_log.Debug(o, ex);
		}

		internal void Error<T>(T o, Exception ex)
		{
			_log.Error(o, ex);
		}

		internal void Fatal<T>(T o, Exception ex)
		{
			_log.Fatal(o, ex);
		}

		internal void Info<T>(T o, Exception ex)
		{
			_log.Info(o, ex);
		}

		internal void Warn<T>(T o, Exception ex)
		{
			_log.Warn(o, ex);
		}
	}
}
