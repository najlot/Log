﻿using Najlot.Log.Destinations;
using System.Collections.Generic;

namespace Najlot.Log.Middleware
{
	public sealed class NoQueueMiddleware : IQueueMiddleware
	{
		private readonly List<LogMessage> _queue = new List<LogMessage>();

		public ILogDestination Destination { get; set; }
		public IFormatMiddleware FormatMiddleware { get; set; }

		public void QueueWriteMessage(LogMessage message)
		{
			_queue.Add(message);
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