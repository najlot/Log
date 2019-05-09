using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Middleware
{
	public struct LogQueueMessage
	{
		public ILogDestination Destination;
		public IFormatMiddleware FormatMiddleware;
		public LogMessage Message;
	}

	/// <summary>
	/// Allows to queue messages for bulk-write to improve performance.
	/// QueueMiddlewares will be registered per LogDestination. 
	/// So that it is possible that console writes direct and file collects for a second before writing.
	/// </summary>
	public interface IQueueMiddleware : IDisposable
	{
		void QueueWriteMessage(LogQueueMessage queueMessage);

		void Flush();
	}

	public sealed class NoQueueMiddleware : IQueueMiddleware
	{
		public void QueueWriteMessage(LogQueueMessage queueMessage)
		{
			var entries = new (LogMessage, IFormatMiddleware)[] { (queueMessage.Message, queueMessage.FormatMiddleware) };
			queueMessage.Destination.Log(entries);
		}

		public void Flush()
		{
			// Nothing to do
		}

		public void Dispose() => Flush();
	}
}