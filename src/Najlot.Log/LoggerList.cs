using Najlot.Log.Util;
using System;
using System.Collections.Generic;

namespace Najlot.Log
{
	/// <summary>
	/// Internal class for multiple log destinations
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

		public void Trace<T>(T o, Exception ex)
		{
			foreach (var item in this)
			{
				item.Trace(o, ex);
			}
		}

		public void Debug<T>(T o, Exception ex)
		{
			foreach (var item in this)
			{
				item.Debug(o, ex);
			}
		}

		public void Error<T>(T o, Exception ex)
		{
			foreach (var item in this)
			{
				item.Error(o, ex);
			}
		}

		public void Fatal<T>(T o, Exception ex)
		{
			foreach (var item in this)
			{
				item.Fatal(o, ex);
			}
		}

		public void Info<T>(T o, Exception ex)
		{
			foreach (var item in this)
			{
				item.Info(o, ex);
			}
		}

		public void Warn<T>(T o, Exception ex)
		{
			foreach (var item in this)
			{
				item.Warn(o, ex);
			}
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					foreach (var item in this)
					{
						item.Dispose();
					}
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