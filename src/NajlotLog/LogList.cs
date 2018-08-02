using System.Collections.Generic;

namespace NajlotLog
{
	internal class LogList : List<ILog>, ILog
	{
		public void Debug(object o)
		{
			foreach (var item in this)
			{
				item.Debug(o);
			}
		}

		public void Error(object o)
		{
			foreach (var item in this)
			{
				item.Error(o);
			}
		}

		public void Fatal(object o)
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

		public void Info(object o)
		{
			foreach (var item in this)
			{
				item.Info(o);
			}
		}

		public void Warn(object o)
		{
			foreach (var item in this)
			{
				item.Warn(o);
			}
		}
	}
}
