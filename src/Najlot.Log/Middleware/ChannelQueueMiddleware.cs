// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(ChannelQueueMiddleware))]
	public sealed class ChannelQueueMiddleware : IQueueMiddleware
	{
		public ILogDestination Destination { get; set; }
		public IFormatMiddleware FormatMiddleware { get; set; }

		private readonly ChannelWriter<LogMessage> writer;
		private readonly ChannelReader<LogMessage> reader;

		private volatile bool cancelationRequested = false;

		private Thread thread;

		public ChannelQueueMiddleware()
		{
			var channel = Channel.CreateUnbounded<LogMessage>();
			writer = channel.Writer;
			reader = channel.Reader;

			thread = new Thread(ThreadAction)
			{
				IsBackground = true
			};

			thread.Start(reader);
		}

		private void ThreadAction(object reader)
		{
			var messageList = new List<LogMessage>();

			if (reader is ChannelReader<LogMessage> channelReader)
			{
				while (!cancelationRequested)
				{
					try
					{
						LogMessage message = null;

						SpinWait.SpinUntil(() => channelReader.TryRead(out message) || cancelationRequested);

						if (cancelationRequested && message == null)
						{
							return;
						}

						do
						{
							messageList.Add(message);
						}
						while (channelReader.TryRead(out message));

						Destination.Log(messageList, FormatMiddleware);

						messageList.Clear();
					}
					catch (Exception ex)
					{
						LogErrorHandler.Instance.Handle("Error writing a log message", ex);
					}
				}
			}
		}

		public void QueueWriteMessage(LogMessage message) => SpinWait.SpinUntil(() => writer.TryWrite(message));

		public void Flush()
		{
			cancelationRequested = true;
			thread.Join();

			cancelationRequested = false;
			thread = new Thread(ThreadAction)
			{
				IsBackground = true
			};

			thread.Start(reader);
		}

		public void Dispose()
		{
			SpinWait.SpinUntil(() => writer.TryComplete());
			cancelationRequested = true;
			thread.Join();
		}
	}
}