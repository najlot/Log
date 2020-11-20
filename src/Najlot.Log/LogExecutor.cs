// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Util;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Najlot.Log
{
	/// <summary>
	/// Internal class for handling multiple destinations
	/// </summary>
	internal sealed class LogExecutor : IDisposable
	{
		#region State Support

		private readonly ThreadLocal<object> _currentState = new ThreadLocal<object>(() => null);
		private readonly ThreadLocal<Stack<object>> _states = new ThreadLocal<Stack<object>>(() => new Stack<object>());

		public IDisposable BeginScope<T>(T state)
		{
			var states = _states.Value;

			states.Push(_currentState.Value);
			_currentState.Value = state;

			return new DisposableState(_currentState, states);
		}

		#endregion State Support

		private readonly string _category;
		private readonly LoggerPool _loggerPool;
		private static readonly object[] EmptyArgs = Array.Empty<object>();
		private static readonly IReadOnlyList<KeyValuePair<string, object>> EmptyKeyValueList = Array.Empty<KeyValuePair<string, object>>();

		public LogExecutor(string category, LoggerPool loggerPool)
		{
			_loggerPool = loggerPool;
			_category = category;
		}

		internal void Log<T>(LogLevel logLevel, Exception ex, T msg, object[] args)
		{
			var state = _currentState.Value;
			var time = DateTime.Now;

			foreach (var entry in _loggerPool.GetDestinations())
			{
				try
				{
					var message = new LogMessage
					{
						DateTime = time,
						LogLevel = logLevel,
						Category = _category,
						State = state,
						RawMessage = string.Empty,
						Exception = ex,
						RawArguments = EmptyArgs,
						Arguments = EmptyKeyValueList,
						ExceptionIsValid = ex != null
					};

					if (args != null)
					{
						message.RawArguments = args;
					}

					if (msg != null) message.RawMessage = msg.ToString();

					if (message.RawArguments.Count == 1 && message.RawArguments[0] is IReadOnlyList<KeyValuePair<string, object>> pair)
					{
						message.Arguments = pair;
					}
					else if (message.RawArguments.Count > 0)
					{
						var parsedKeyValuePairs = LogArgumentsParser.ParseArguments(message.RawMessage, args);
						message.Arguments = parsedKeyValuePairs;
					}

					entry.CollectMiddleware.Execute(message);
				}
				catch (Exception exc)
				{
					LogErrorHandler.Instance.Handle("An error in the log pipeline occured.", exc);
				}
			}
		}

		public void Flush() => _loggerPool.Flush();

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		private void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				_disposedValue = true;

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