// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(ConcurrentCollectMiddleware))]
	public sealed class ConcurrentCollectMiddleware : ICollectMiddleware
	{
		private readonly ConcurrentQueue<LogMessage> _messages = new ConcurrentQueue<LogMessage>();
		private volatile bool _cancelationRequested = false;
		private Thread _thread;

		public IMiddleware NextMiddleware { get; set; }

		public ConcurrentCollectMiddleware()
		{
			_thread = new Thread(ThreadAction)
			{
				IsBackground = true
			};

			_thread.Start(_messages);
		}

		private void ThreadAction(object param)
		{
			var messageList = new List<LogMessage>();

			if (param is ConcurrentQueue<LogMessage> queue)
			{
				do
				{
					try
					{
						LogMessage message = null;

						SpinWait.SpinUntil(() => queue.TryDequeue(out message) || _cancelationRequested);

						if (_cancelationRequested && message == null)
						{
							return;
						}

						do
						{
							messageList.Add(message);
						}
						while (queue.TryDequeue(out message));

						NextMiddleware.Execute(messageList);

						messageList.Clear();
					}
					catch (Exception ex)
					{
						LogErrorHandler.Instance.Handle("Error executing a log action.", ex);
					}
				}
				while (!_cancelationRequested || queue.TryDequeue(out _));
			}
		}

		public void Execute(LogMessage message) => _messages.Enqueue(message);

		public void Flush()
		{
			_cancelationRequested = true;
			_thread.Join();

			_cancelationRequested = false;
			_thread = new Thread(ThreadAction)
			{
				IsBackground = true
			};

			_thread.Start(_messages);
		}

		public void Dispose()
		{
			_cancelationRequested = true;
			_thread.Join();
		}
	}
}