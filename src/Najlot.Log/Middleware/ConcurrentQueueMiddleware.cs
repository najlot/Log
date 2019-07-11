// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Najlot.Log.Destinations;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(ConcurrentQueueMiddleware))]
	public sealed class ConcurrentQueueMiddleware : IQueueMiddleware
	{
		private readonly ConcurrentQueue<LogMessage> messages = new ConcurrentQueue<LogMessage>();
		private volatile bool cancelationRequested = false;
		private Thread thread;

		public ILogDestination Destination { get; set; }
		public IFormatMiddleware FormatMiddleware { get; set; }

		public ConcurrentQueueMiddleware()
		{
			thread = new Thread(ThreadAction);
			thread.Start(messages);
		}

		private void ThreadAction(object param)
		{
			var messageList = new List<LogMessage>();

			if (param is ConcurrentQueue<LogMessage> queue)
			{
				while (!cancelationRequested)
				{
					try
					{
						LogMessage message = null;

						SpinWait.SpinUntil(() => queue.TryDequeue(out message) || cancelationRequested);

						if (cancelationRequested && message == null)
						{
							return;
						}

						do
						{
							messageList.Add(message);
						}
						while (queue.TryDequeue(out message));

						Destination.Log(messageList, FormatMiddleware);

						messageList.Clear();
					}
					catch (Exception ex)
					{
						LogErrorHandler.Instance.Handle("Error executing a log action", ex);
					}
				}
			}
		}

		public void QueueWriteMessage(LogMessage message) => messages.Enqueue(message);

		public void Flush()
		{
			cancelationRequested = true;
			thread.Join();

			cancelationRequested = false;
			thread = new Thread(ThreadAction);
			thread.Start(messages);
		}

		public void Dispose()
		{
			cancelationRequested = true;
			thread.Join();
		}
	}
}