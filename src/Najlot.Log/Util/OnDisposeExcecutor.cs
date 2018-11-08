using System;

namespace Najlot.Log.Util
{
	internal sealed class OnDisposeExcecutor : IDisposable
	{
		private Action _action;

		public OnDisposeExcecutor(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			if (_action != null)
			{
				_action();
				_action = null;
			}
		}
	}
}