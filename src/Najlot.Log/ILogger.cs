﻿using System;

namespace Najlot.Log
{
	public interface ILogger : IDisposable
	{
		IDisposable BeginScope<T>(T state);

		void Trace<T>(T o);

		void Debug<T>(T o);

		void Error<T>(T o);

		void Fatal<T>(T o);

		void Info<T>(T o);

		void Warn<T>(T o);

		void Trace<T>(T o, Exception ex);

		void Debug<T>(T o, Exception ex);

		void Error<T>(T o, Exception ex);

		void Fatal<T>(T o, Exception ex);

		void Info<T>(T o, Exception ex);

		void Warn<T>(T o, Exception ex);

		/// <summary>
		/// Tells the logger to write all messages to destinations, immediately.
		/// Will not return until done.
		/// </summary>
		void Flush();
	}
}