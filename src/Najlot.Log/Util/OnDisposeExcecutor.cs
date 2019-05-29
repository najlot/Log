// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log.Util
{
	internal sealed class OnDisposeExcecutor : IDisposable
	{
		private readonly Action _action;

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