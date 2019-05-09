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

		public void Trace<T>(T o) => Log(LogLevel.Trace, null, o, _emptyArgs);

		public void Info<T>(T o) => Log(LogLevel.Info, null, o, _emptyArgs);

		public void Warn<T>(T o) => Log(LogLevel.Warn, null, o, _emptyArgs);

		public void Debug<T>(T o) => Log(LogLevel.Debug, null, o, _emptyArgs);

		public void Error<T>(T o) => Log(LogLevel.Error, null, o, _emptyArgs);

		public void Fatal<T>(T o) => Log(LogLevel.Fatal, null, o, _emptyArgs);

		public void Trace<T>(Exception ex, T o) => Log(LogLevel.Trace, ex, o, _emptyArgs);

		public void Info<T>(Exception ex, T o) => Log(LogLevel.Info, ex, o, _emptyArgs);

		public void Warn<T>(Exception ex, T o) => Log(LogLevel.Warn, ex, o, _emptyArgs);

		public void Debug<T>(Exception ex, T o) => Log(LogLevel.Debug, ex, o, _emptyArgs);

		public void Error<T>(Exception ex, T o) => Log(LogLevel.Error, ex, o, _emptyArgs);

		public void Fatal<T>(Exception ex, T o) => Log(LogLevel.Fatal, ex, o, _emptyArgs);

		public void Trace(string s, params object[] args) => Log(LogLevel.Trace, null, s, args);

		public void Debug(string s, params object[] args) => Log(LogLevel.Debug, null, s, args);

		public void Info(string s, params object[] args) => Log(LogLevel.Info, null, s, args);

		public void Warn(string s, params object[] args) => Log(LogLevel.Warn, null, s, args);

		public void Error(string s, params object[] args) => Log(LogLevel.Error, null, s, args);

		public void Fatal(string s, params object[] args) => Log(LogLevel.Fatal, null, s, args);

		public void Trace(Exception ex, string s, params object[] args) => Log(LogLevel.Trace, ex, s, args);

		public void Debug(Exception ex, string s, params object[] args) => Log(LogLevel.Debug, ex, s, args);

		public void Info(Exception ex, string s, params object[] args) => Log(LogLevel.Info, ex, s, args);

		public void Warn(Exception ex, string s, params object[] args) => Log(LogLevel.Warn, ex, s, args);

		public void Error(Exception ex, string s, params object[] args) => Log(LogLevel.Error, ex, s, args);

		public void Fatal(Exception ex, string s, params object[] args) => Log(LogLevel.Fatal, ex, s, args);

		private void Log<T>(LogLevel logLevel, Exception ex, T msg, object[] args)
		{
			var state = _currentState.Value;
			var time = DateTime.Now;

			foreach (var entry in _loggerPool.GetLogDestinations())
			{
				var destination = entry.LogDestination;
				var destinationType = entry.LogDestinationType;
				var filterMiddleware = entry.FilterMiddleware;
				var formatMiddleware = entry.FormatMiddleware;

				entry.ExecutionMiddleware.Execute(() =>
				{
					LogMessage message;

					args = args ?? _emptyArgs;

					if (msg == null)
					{
						message = new LogMessage(time, logLevel, _category, state, "", ex, _emptyKeyValueList);
					}
					else if(args.Length == 0)
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

					if (filterMiddleware.AllowThrough(destinationType, message))
					{
						destination.Log(message, formatMiddleware);
					}
				});
			}
		}

		public void Flush()
		{
			foreach (var entry in _loggerPool.GetLogDestinations())
			{
				entry.ExecutionMiddleware.Flush();
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