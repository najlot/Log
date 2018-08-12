using System;
using System.Collections.Generic;

namespace NajlotLog
{
	internal class LogList : List<ILog>, ILog
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
