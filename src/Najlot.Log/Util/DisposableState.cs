// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Najlot.Log.Util;

/// <summary>
/// Pops states back on dispose
/// Used for managing scopes.
/// </summary>
internal sealed class DisposableState : IDisposable
{
	private readonly AsyncLocal<object> _currentState;
	private readonly Stack<object> _states;

	public DisposableState(AsyncLocal<object> state, Stack<object> states)
	{
		_currentState = state;
		_states = states;
	}

	private bool _disposedValue = false;

	public void Dispose()
	{
		if (!_disposedValue)
		{
			_disposedValue = true;

			try
			{
				_currentState.Value = _states.Pop();
			}
			catch (Exception ex)
			{
				_currentState.Value = null;
				LogErrorHandler.Instance.Handle("Error setting back the State.", ex);
			}
		}
	}
}