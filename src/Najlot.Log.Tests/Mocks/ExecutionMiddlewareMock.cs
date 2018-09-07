using Najlot.Log.Middleware;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public class ExecutionMiddlewareMock : IExecutionMiddleware
	{
		private Action<Action> _action;

		public ExecutionMiddlewareMock(Action<Action> action)
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
