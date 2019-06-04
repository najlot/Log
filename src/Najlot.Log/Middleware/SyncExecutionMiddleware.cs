// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Executes everything syncronous.
	/// Has the advantage, that the messages are immediately there and it hat nothing to flush.
	/// </summary>
	[LogConfigurationName(nameof(SyncExecutionMiddleware))]
	public sealed class SyncExecutionMiddleware : IExecutionMiddleware
	{
		private readonly object _lock = new object();

		public void Execute(Action execute)
		{
			lock (_lock) execute();
		}

		public void Dispose()
		{
			// As the actions are executed directly, there is nothing to dispose.
		}

		public void Flush()
		{
			// As the actions are executed directly, there is nothing to flush.
		}
	}
}