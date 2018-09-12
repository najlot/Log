using System;

namespace Najlot.Log.Util
{
	internal class OnDisposeExcecutor : IDisposable
	{
		private readonly Action _action;

		public OnDisposeExcecutor(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action();
		}
	}
}
