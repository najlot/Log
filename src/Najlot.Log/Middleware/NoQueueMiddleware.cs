// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(NoQueueMiddleware))]
	public sealed class NoQueueMiddleware : IQueueMiddleware
	{
		public ILogDestination Destination { get; set; }
		public IFormatMiddleware FormatMiddleware { get; set; }

		private readonly LogMessage[] _msgArr = new LogMessage[1];

		public void QueueWriteMessage(LogMessage message)
		{
			_msgArr[0] = message;
			Destination.Log(_msgArr, FormatMiddleware);
		}

		public void Flush()
		{
			// Nothing to flush
		}

		public void Dispose()
		{
			// Nothing to dispose
		}
	}
}