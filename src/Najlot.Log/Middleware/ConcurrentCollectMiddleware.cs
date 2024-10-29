// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Najlot.Log.Middleware;

/// <summary>
/// Middleware that collects messages from different threads
/// and pass them to the next middleware on an other thread
/// </summary>
[LogConfigurationName(nameof(ConcurrentCollectMiddleware))]
public sealed class ConcurrentCollectMiddleware : ICollectMiddleware
{
	private readonly ConcurrentQueue<LogMessage> _messages = new();
	private readonly ManualResetEventSlim _resetEvent = new(false);
	private readonly ManualResetEventSlim _flushResetEvent = new(false);

	private volatile bool _cancellationRequested = false;
	private readonly Thread _thread;

	public IMiddleware? NextMiddleware { get; set; }

	public ConcurrentCollectMiddleware()
	{
		_thread = new Thread(ThreadAction) { IsBackground = true };
		_thread.Start();
	}

	private void ThreadAction()
	{
		while (!_cancellationRequested || !_messages.IsEmpty)
		{
			if (_messages.IsEmpty)
			{
				_resetEvent.Wait();
				_resetEvent.Reset();
			}

			try
			{
				ProcessMessages();
			}
			catch (Exception ex)
			{
				LogErrorHandler.Instance.Handle(GetType().Name + " run into an error.", ex);
			}
		}
	}

	private void ProcessMessages()
	{
		var messages = new List<LogMessage>(_messages.Count > 4 ? _messages.Count : 4);

		while (_messages.TryDequeue(out var message))
		{
			messages.Add(message);
		}

		if (messages.Count > 0)
		{
			NextMiddleware?.Execute(messages);
		}

		if (_messages.IsEmpty)
		{
			_flushResetEvent.Set();
		}
	}

	public void Execute(LogMessage message)
	{
		_messages.Enqueue(message);
		if (!_resetEvent.IsSet)
		{
			_resetEvent.Set();
		}
	}

	public void Flush()
	{
		_flushResetEvent.Reset();
		_resetEvent.Set();
		_flushResetEvent.Wait();

		NextMiddleware?.Flush();
	}

	public void Dispose()
	{
		_cancellationRequested = true;
		_resetEvent.Set();
		_thread.Join();

		_resetEvent.Dispose();
		_flushResetEvent.Dispose();
	}
}