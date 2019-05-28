using Najlot.Log.Destinations;
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
						_timer = new Timer(Flush, _queue, 1000, Timeout.Infinite);
					}
				}
			}
		}

		private void Flush(object state)
		{
			lock (_timerLock)
			{
				_queue = new Queue<LogMessage>();

				if (state is Queue<LogMessage> queue)
				{
					Destination.Log(queue, FormatMiddleware);
					_timer = new Timer(Flush, _queue, 1000, Timeout.Infinite);
				}
			}
		}

		public void Flush() => Flush(_queue);

		public void Dispose() => Flush();
	}
}