using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NajlotLog
{
	public abstract class LogImplementationBase : ILog
	{
		private ConcurrentQueue<string> StringsToLog = new ConcurrentQueue<string>();
		private bool DequeueTaskRuns
		{
			get
			{
				lock (DequeueTaskLock)
				{
					return _dequeueTaskRuns;
				}
			}

			set
			{
				lock (DequeueTaskLock)
				{
					_dequeueTaskRuns = value;
				}
			}
		}

		private object DequeueTaskLock = new object();
		private bool _dequeueTaskRuns = false;

		private void StartDequeueTaskIfNotRuns()
		{
			if (!DequeueTaskRuns)
			{
				Task.Run(() =>
				{
					DequeueTaskRuns = true;
					
					while (StringsToLog.TryDequeue(out string msg))
					{
						if(StringsToLog.Count > 5)
						{
							string bufMsg;
							for (int i = 0; i < 5; i++)
							{
								if(StringsToLog.TryDequeue(out bufMsg))
								{
									msg += bufMsg;
								}
							}
						}

						try
						{
							Log(msg);
						}
						catch
						{
						}
					}

					DequeueTaskRuns = false;
				});
			}
		}

		public void Debug(object o)
		{
			StringsToLog.Enqueue(string.Concat(DateTime.Now, " DEBUG ", o, Environment.NewLine));
			StartDequeueTaskIfNotRuns();
		}

		public void Error(object o)
		{
			StringsToLog.Enqueue(string.Concat(DateTime.Now, " ERROR ", o, Environment.NewLine));
			StartDequeueTaskIfNotRuns();
		}

		public void Fatal(object o)
		{
			StringsToLog.Enqueue(string.Concat(DateTime.Now, " FATAL ", o, Environment.NewLine));
			StartDequeueTaskIfNotRuns();
		}

		public void Info(object o)
		{
			StringsToLog.Enqueue(string.Concat(DateTime.Now, " INFO ", o, Environment.NewLine));
			StartDequeueTaskIfNotRuns();
		}

		public void Warn(object o)
		{
			StringsToLog.Enqueue(string.Concat(DateTime.Now, " WARN ", o, Environment.NewLine));
			StartDequeueTaskIfNotRuns();
		}

		protected abstract void Log(string msg);

		public void Flush()
		{
			StartDequeueTaskIfNotRuns();
			while (DequeueTaskRuns) Thread.Sleep(10);
		}
	}
}
