using Najlot.Log.Util;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Najlot.Log
{
	/// <summary>
	/// Internal class for multiple log destinations
	/// </summary>
	internal class LogExecutor : IDisposable
	{
		#region State Support

		private readonly ThreadLocal<object> _currentState = new ThreadLocal<object>(() => null);
		private readonly ThreadLocal<Stack<object>> _states = new ThreadLocal<Stack<object>>(() => new Stack<object>());

		public IDisposable BeginScope<T>(T state)
		{
			var states = _states.Value;

			states.Push(_currentState.Value);
			_currentState.Value = state;

			return new OnDisposeExcecutor(() => _currentState.Value = states.Pop());
		}

		#endregion State Support

		private readonly string _category;
		private readonly LoggerPool _loggerPool;
		private static readonly object[] _emptyArgs = new object[0];
		private static readonly IReadOnlyList<KeyValuePair<string, object>> _emptyKeyValueList = new List<KeyValuePair<string, object>>();

		public LogExecutor(string category, LoggerPool loggerPool)
		{
			_loggerPool = loggerPool;
			_category = category;
		}

		internal void Log<T>(LogLevel logLevel, Exception ex, T msg, object[] args)
		{
			var state = _currentState.Value;
			var time = DateTime.Now;

			foreach (var entry in _loggerPool.GetLogDestinations())
			{
				var destinationType = entry.LogDestinationType;
				var filterMiddleware = entry.FilterMiddleware;
				var queueMiddleware = entry.QueueMiddleware;

				entry.ExecutionMiddleware.Execute(() =>
				{
					LogMessage message;

					args = args ?? _emptyArgs;

					if (msg == null)
					{
						message = new LogMessage(time, logLevel, _category, state, "", ex, _emptyKeyValueList);
					}
					else if (args.Length == 0)
					{
						message = new LogMessage(time, logLevel, _category, state, msg.ToString(), ex, _emptyKeyValueList);
					}
					else if (args.Length == 1 && args[0] is IReadOnlyList<KeyValuePair<string, object>> pair)
					{
						message = new LogMessage(time, logLevel, _category, state, msg.ToString(), ex, pair);
					}
					else
					{
						var parsedKeyValuePairs = LogArgumentsParser.ParseArguments(msg.ToString(), args);
						message = new LogMessage(time, logLevel, _category, state, msg.ToString(), ex, parsedKeyValuePairs);
					}

					if (filterMiddleware.AllowThrough(message))
					{
						queueMiddleware.QueueWriteMessage(message);
					}
				});
			}
		}

		public void Flush()
		{
			foreach (var entry in _loggerPool.GetLogDestinations())
			{
				entry.ExecutionMiddleware.Flush();
				entry.QueueMiddleware.Flush();
			}
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					_currentState.Dispose();
					_states.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}