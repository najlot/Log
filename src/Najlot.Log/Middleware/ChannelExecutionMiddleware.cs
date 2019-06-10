// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Channels;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(ChannelExecutionMiddleware))]
	public sealed class ChannelExecutionMiddleware : IExecutionMiddleware
	{
		private readonly ChannelWriter<Action> writer;
		private readonly ChannelReader<Action> reader;

		private bool cancelationRequested = false;

		private Thread thread;

		public ChannelExecutionMiddleware()
		{
			var channel = Channel.CreateUnbounded<Action>();
			writer = channel.Writer;
			reader = channel.Reader;

			thread = new Thread(ThreadAction);
			thread.Start(reader);
		}

		private void ThreadAction(object reader)
		{
			if (reader is ChannelReader<Action> channelReader)
			{
				while (!cancelationRequested)
				{
					try
					{
						Action action = null;

						SpinWait.SpinUntil(() => channelReader.TryRead(out action) || cancelationRequested);

						if (cancelationRequested && action == null)
						{
							return;
						}

						do
						{
							action();
						}
						while (channelReader.TryRead(out action));
					}
					catch (Exception ex)
					{
						LogErrorHandler.Instance.Handle("Error executing a log action", ex);
					}
				}
			}
		}

		public void Execute(Action execute) => SpinWait.SpinUntil(() => writer.TryWrite(execute));

		public void Flush()
		{
			cancelationRequested = true;
			thread.Join();

			cancelationRequested = false;
			thread = new Thread(ThreadAction);
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