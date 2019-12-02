// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(QueueExecutionMiddleware))]
	public sealed class QueueExecutionMiddleware : IExecutionMiddleware
	{
		private readonly ConcurrentQueue<Action> messages = new ConcurrentQueue<Action>();

		private volatile bool cancelationRequested = false;

		private Thread thread;

		public QueueExecutionMiddleware()
		{
			thread = new Thread(ThreadAction)
			{
				IsBackground = true
			};

			thread.Start(messages);
		}

		private void ThreadAction(object param)
		{
			if (param is ConcurrentQueue<Action> queue)
			{
				while (!cancelationRequested)
				{
					try
					{
						Action action = null;

						SpinWait.SpinUntil(() => queue.TryDequeue(out action) || cancelationRequested);

						if (cancelationRequested && action == null)
						{
							return;
						}

						do
						{
							action();
						}
						while (queue.TryDequeue(out action));
					}
					catch (Exception ex)
					{
						LogErrorHandler.Instance.Handle("Error executing a log action", ex);
					}
				}
			}
		}

		public void Execute(Action execute) => messages.Enqueue(execute);

		public void Flush()
		{
			cancelationRequested = true;
			thread.Join();

			cancelationRequested = false;
			thread = new Thread(ThreadAction)
			{
				IsBackground = true
			};

			thread.Start(messages);
		}

		public void Dispose()
		{
			cancelationRequested = true;
			thread.Join();
		}
	}
}