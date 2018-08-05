using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NajlotLog
{
	public abstract class LogImplementationBase : ILog
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

		public Action<object> Debug { get; private set; }
		public Action<object> Info { get; private set; }
		public Action<object> Warn { get; private set; }
		public Action<object> Error { get; private set; }
		public Action<object> Fatal { get; private set; }
		
		public LogImplementationBase()
		{
			Debug = new Action<object>(o =>
			{
				var time = DateTime.Now;

				ActionsToExecute.Enqueue(new Action(() =>
				{
					Log(string.Concat(time, " DEBUG ", o, Environment.NewLine));
				}));
				
				StartDequeueTaskIfNotRuns();
			});

			Info = new Action<object>(o =>
			{
				var time = DateTime.Now;

				ActionsToExecute.Enqueue(new Action(() =>
				{
					Log(string.Concat(time, " INFO ", o, Environment.NewLine));
				}));
				
				StartDequeueTaskIfNotRuns();
			});

			Warn = new Action<object>(o =>
			{
				var time = DateTime.Now;

				ActionsToExecute.Enqueue(new Action(() =>
				{
					Log(string.Concat(time, " WARN ", o, Environment.NewLine));
				}));
				
				StartDequeueTaskIfNotRuns();
			});

			Error = new Action<object>(o =>
			{
				var time = DateTime.Now;

				ActionsToExecute.Enqueue(new Action(() =>
				{
					Log(string.Concat(time, " ERROR ", o, Environment.NewLine));
				}));
				
				StartDequeueTaskIfNotRuns();
			});
			
			Fatal = new Action<object>(o =>
			{
				var time = DateTime.Now;

				ActionsToExecute.Enqueue(new Action(() =>
				{
					Log(string.Concat(time, " FATAL ", o, Environment.NewLine));
				}));
				
				StartDequeueTaskIfNotRuns();
			});
		}
		
		protected abstract void Log(string msg);

		public void Flush()
		{
			StartDequeueTaskIfNotRuns();
			while (DequeueTaskRuns) Thread.Sleep(10);
		}
	}
}
