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

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		public void Dispose()
		{
			if (!_disposedValue)
			{
				_disposedValue = true;
				_action();
			}
		}

		#endregion IDisposable Support
	}
}