using NajlotLog.Util;
using System;
using System.Collections.Generic;

namespace NajlotLog
{
	/// <summary>
	/// Internal class for multiple log destinations.
	/// </summary>
	internal class LoggerList : List<ILogger>, ILogger
	{
		public void Trace<T>(T o)
		{
			foreach (var item in this)
			{
				item.Trace(o);
			}
		}

		public void Debug<T>(T o)
		{
			foreach (var item in this)
			{
				item.Debug(o);
			}
		}

		public void Info<T>(T o)
		{
			foreach (var item in this)
			{
				item.Info(o);
			}
		}

		public void Warn<T>(T o)
		{
			foreach (var item in this)
			{
				item.Warn(o);
			}
		}

		public void Error<T>(T o)
		{
			foreach (var item in this)
			{
				item.Error(o);
			}
		}

		public void Fatal<T>(T o)
		{
			foreach (var item in this)
			{
				item.Fatal(o);
			}
		}

		public void Flush()
		{
			foreach (var item in this)
			{
				item.Flush();
			}
		}
		
		public IDisposable BeginScope<T>(T state)
		{
			var disposableListOfDisposables = new DisposableListOfDisposables();
			
			foreach (var item in this)
			{
				disposableListOfDisposables.Add(item.BeginScope(state));
			}

			return disposableListOfDisposables;
		}
	}
}
