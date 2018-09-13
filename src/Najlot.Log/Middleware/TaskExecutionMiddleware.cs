using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Enqueues actions to a queue and executes them in a task.
	/// Has the advantage, that logging does not slow down the execution 
	/// and the task runs out when there are no messages to log.
	/// Do not forget to flush, to get all of your logged messages!
	/// </summary>
    public class TaskExecutionMiddleware : IExecutionMiddleware
	{
		private readonly ConcurrentQueue<Action> ActionsToExecute = new ConcurrentQueue<Action>();
		
		private volatile bool _dequeueTaskRuns = false;

		private readonly object _taskLock = new object();

		private Task _dequeueTask;

		private void StartDequeueTaskIfNotRuns(bool wait)
		{
			if (!_dequeueTaskRuns)
			{
				if(Monitor.TryEnter(_taskLock))
				{
					try
					{
						if (!_dequeueTaskRuns)
						{
							_dequeueTaskRuns = true;
							_dequeueTask = Task.Run(() => DequeueTaskMethod());

							if (wait)
							{
								_dequeueTask.Wait();
							}
						}
					}
					finally
					{
						Monitor.Exit(_taskLock);
					}
				}
			}
			else if(wait)
			{
				_dequeueTask.Wait();
			}
		}

        private void DequeueTaskMethod()
        {
            while (ActionsToExecute.TryDequeue(out Action action))
            {
                try
                {
                    action();
                }
                catch(Exception ex)
                {
					Console.Write("Najlot.Log: ");

					while(ex != null)
					{
						Console.WriteLine(ex.ToString());
						ex = ex.InnerException;
					}
                }
            }

            _dequeueTaskRuns = false;
        }
		
		public void Execute(Action execute)
        {
            ActionsToExecute.Enqueue(execute);
			StartDequeueTaskIfNotRuns(false);
        }

		public void Dispose()
        {
            this.Flush();
        }

		public void Flush()
		{
			StartDequeueTaskIfNotRuns(true);
		}
    }
}