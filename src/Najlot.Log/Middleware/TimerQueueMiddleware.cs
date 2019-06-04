// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(TimerQueueMiddleware))]
	public sealed class TimerQueueMiddleware : IQueueMiddleware
	{
		private Queue<LogMessage> _queue = new Queue<LogMessage>();
		private Timer _timer = null;
		private readonly object _timerLock = new object();

		public ILogDestination Destination { get; set; }
		public IFormatMiddleware FormatMiddleware { get; set; }

		public void QueueWriteMessage(LogMessage message)
		{
			_queue.Enqueue(message);

			if (_timer == null)
			{
				lock (_timerLock)
				{
					if (_timer == null)
					{
						_timer = new Timer(TimerFlush, null, 1000, Timeout.Infinite);
					}
				}
			}
		}

		private void TimerFlush(object state)
		{
			try
			{
				Flush();
			}
			catch (Exception ex)
			{
				_timer = null;
				LogErrorHandler.Instance.Handle("Error flushing log messages", ex);
			}
		}

		public void Flush()
		{
			lock (_timerLock)
			{
				var queue = _queue;
				_queue = new Queue<LogMessage>();

				if (queue.Count > 0)
				{
					Destination.Log(queue.ToArray(), FormatMiddleware);
				}

				_timer = null;

				if (_queue.Count > 0)
				{
					_timer = new Timer(TimerFlush, null, 1000, Timeout.Infinite);
				}
			}
		}

		public void Dispose() => Flush();
	}
}