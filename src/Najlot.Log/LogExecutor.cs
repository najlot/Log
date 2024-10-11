// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Util;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Najlot.Log;

/// <summary>
/// Internal class for handling multiple destinations
/// </summary>
internal sealed class LogExecutor
{
	#region State Support

	private readonly AsyncLocal<object> _currentState = new();
	private readonly AsyncLocal<Stack<object>> _states = new();

	public IDisposable BeginScope<T>(T state)
	{
		_states.Value ??= new Stack<object>();

		var states = _states.Value;

		states.Push(_currentState.Value);
		_currentState.Value = state;

		return new DisposableState(_currentState, states);
	}

	#endregion State Support

	private readonly string _category;
	private readonly LoggerPool _loggerPool;
	private static readonly object[] _emptyArgs = [];
	private static readonly IReadOnlyList<KeyValuePair<string, object>> _emptyKeyValueList = [];

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
					RawArguments = _emptyArgs,
					Arguments = _emptyKeyValueList
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
}