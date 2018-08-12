using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NajlotLog
{
	public abstract class AsyncLogImplementationBase : ILog
	{
		private ConcurrentQueue<Action> ActionsToExecute = new ConcurrentQueue<Action>();
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
					
					while (ActionsToExecute.TryDequeue(out Action action))
					{
						try
						{
							action();
						}
						catch
						{
						}
					}

					DequeueTaskRuns = false;
				});
			}
		}
		
		protected abstract void Log(string msg);

		public void Flush()
		{
			StartDequeueTaskIfNotRuns();
			while (DequeueTaskRuns) Thread.Sleep(10);
		}

		public void Debug<T>(T o)
		{
			var time = DateTime.Now;

			ActionsToExecute.Enqueue(new Action(() =>
			{
				Log(string.Concat(time, " DEBUG ", o, Environment.NewLine));
			}));

			StartDequeueTaskIfNotRuns();
		}

		public void Info<T>(T o)
		{
			var time = DateTime.Now;

			ActionsToExecute.Enqueue(new Action(() =>
			{
				Log(string.Concat(time, " INFO ", o, Environment.NewLine));
			}));

			StartDequeueTaskIfNotRuns();
		}

		public void Warn<T>(T o)
		{
			var time = DateTime.Now;

			ActionsToExecute.Enqueue(new Action(() =>
			{
				Log(string.Concat(time, " WARN ", o, Environment.NewLine));
			}));

			StartDequeueTaskIfNotRuns();
		}

		public void Error<T>(T o)
		{
			var time = DateTime.Now;

			ActionsToExecute.Enqueue(new Action(() =>
			{
				Log(string.Concat(time, " ERROR ", o, Environment.NewLine));
			}));

			StartDequeueTaskIfNotRuns();
		}

		public void Fatal<T>(T o)
		{
			var time = DateTime.Now;

			ActionsToExecute.Enqueue(new Action(() =>
			{
				Log(string.Concat(time, " FATAL ", o, Environment.NewLine));
			}));

			StartDequeueTaskIfNotRuns();
		}
	}
}
