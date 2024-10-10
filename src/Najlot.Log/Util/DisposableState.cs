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
	private readonly ThreadLocal<object> _currentState;
	private readonly Stack<object> _states;

	public DisposableState(ThreadLocal<object> state, Stack<object> states)
	{
		_currentState = state;
		_states = states;
	}

	#region IDisposable Support

	private bool _disposedValue = false; // To detect redundant calls

	public void Dispose()
	{
		if (!_disposedValue)
		{
			_disposedValue = true;
			_currentState.Value = _states.Pop();
		}
	}

	#endregion IDisposable Support
}