using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Allows to queue messages for bulk-write to improve performance.
	/// QueueMiddlewares will be registered per LogDestination. 
	/// So that it is possible that console writes direct and file collects for a second before writing.
	/// </summary>
	public interface IQueueMiddleware : IDisposable
	{
		ILogDestination Destination { get; set; }
		IFormatMiddleware FormatMiddleware { get; set; }

		void QueueWriteMessage(LogMessage message);

		void Flush();
	}
}