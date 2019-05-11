using Najlot.Log.Destinations;
using System.Collections.Generic;

namespace Najlot.Log.Middleware
{
	public sealed class NoQueueMiddleware : IQueueMiddleware
	{
		private readonly Queue<LogMessage> _queue = new Queue<LogMessage>();

		public ILogDestination Destination { get; set; }
		public IFormatMiddleware FormatMiddleware { get; set; }

		public void QueueWriteMessage(LogMessage message)
		{
			_queue.Enqueue(message);
			Destination.Log(_queue, FormatMiddleware);
			_queue.Clear();
		}

		public void Flush()
		{
			// Nothing to do
		}

		public void Dispose() => Flush();
	}
}