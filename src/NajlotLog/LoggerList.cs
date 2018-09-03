using System;
using System.Collections.Generic;

namespace NajlotLog
{
	/// <summary>
	/// Internal class for multiple log destinations.
	/// </summary>
	internal class LoggerList : List<ILogger>, ILogger
	{
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
	}
}
