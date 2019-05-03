using Najlot.Log.Util;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Najlot.Log
{
	/// <summary>
	/// Internal class for multiple log destinations
	/// </summary>
	internal class LogExecutor
	{
		#region State Support

		private ThreadLocal<object> _currentState = new ThreadLocal<object>(() => null);
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

		public LogExecutor(string category, LoggerPool loggerPool)
		{
			_loggerPool = loggerPool;
			_category = category;
		}

		private void Log<T>(T o, LogLevel logLevel, Exception ex = null)
		{
			var state = _currentState.Value;
			var time = DateTime.Now;

			foreach (var entry in _loggerPool.GetLogDestinations())
			{
				var formatFunc = entry.FormatFunc;
				var destination = entry.LogDestination;
				var destinationType = entry.LogDestinationType;
				var filterMiddleware = entry.FilterMiddleware;

				entry.ExecutionMiddleware.Execute(() =>
				{
					var message = new LogMessage(time, logLevel, _category, state, o, ex);

					if (filterMiddleware.AllowThrough(destinationType, message))
					{
						destination.Log(message, formatFunc);
					}
				});
			}
		}

		public void Trace<T>(T o) => Log(o, LogLevel.Trace);

		public void Info<T>(T o) => Log(o, LogLevel.Info);

		public void Warn<T>(T o) => Log(o, LogLevel.Warn);

		public void Debug<T>(T o) => Log(o, LogLevel.Debug);

		public void Error<T>(T o) => Log(o, LogLevel.Error);

		public void Fatal<T>(T o) => Log(o, LogLevel.Fatal);

		public void Trace<T>(T o, Exception ex) => Log(o, LogLevel.Trace, ex);

		public void Info<T>(T o, Exception ex) => Log(o, LogLevel.Info, ex);

		public void Warn<T>(T o, Exception ex) => Log(o, LogLevel.Warn, ex);

		public void Debug<T>(T o, Exception ex) => Log(o, LogLevel.Debug, ex);

		public void Error<T>(T o, Exception ex) => Log(o, LogLevel.Error, ex);

		public void Fatal<T>(T o, Exception ex) => Log(o, LogLevel.Fatal, ex);

		public void Flush()
		{
			foreach (var entry in _loggerPool.GetLogDestinations())
			{
				entry.ExecutionMiddleware.Flush();
			}
		}
	}
}