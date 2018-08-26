using NajlotLog.Middleware;
using System;

namespace NajlotLog.Tests.Mocks
{
	public class LogExecutionMiddlewareMock : ILogExecutionMiddleware
	{
		private Action<Action> _action;

		public LogExecutionMiddlewareMock(Action<Action> action)
		{
			_action = action;
		}

		public void Dispose()
		{
			Flush();
		}

		public void Execute(Action execute)
		{
			_action(execute);
		}

		public void Flush()
		{
			
		}
	}
}
