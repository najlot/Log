using System;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Executes everything syncronous.
	/// Has the advantage, that the messages are immediately there and it hat nothing to flush.
	/// </summary>
    public class SyncExecutionMiddleware : IExecutionMiddleware
    {
		public void Execute(Action execute)
        {
            execute();
        }

        public void Dispose()
        {
			Flush();
		}

        public void Flush()
        {
            // As the actions are executed directly, there is nothing to flush.
        }
    }
}