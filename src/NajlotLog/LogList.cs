using System;
using System.Collections.Generic;

namespace NajlotLog
{
	internal class LogList : List<ILog>, ILog
	{
		public Action<object> Debug { get; private set; }
		public Action<object> Info { get; private set; }
		public Action<object> Warn { get; private set; }
		public Action<object> Error { get; private set; }
		public Action<object> Fatal { get; private set; }

		public LogList()
		{
			Debug = new Action<object>(o =>
			{
				foreach (var item in this)
				{
					item.Debug(o);
				}
			});

			Info = new Action<object>(o =>
			{
				foreach (var item in this)
				{
					item.Info(o);
				}
			});

			Warn = new Action<object>(o =>
			{
				foreach (var item in this)
				{
					item.Warn(o);
				}
			});

			Error = new Action<object>(o =>
			{
				foreach (var item in this)
				{
					item.Error(o);
				}
			});

			Fatal = new Action<object>(o =>
			{
				foreach (var item in this)
				{
					item.Fatal(o);
				}
			});
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
