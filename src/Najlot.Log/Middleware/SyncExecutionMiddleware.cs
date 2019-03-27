using System;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Executes everything syncronous.
	/// Has the advantage, that the messages are immediately there and it hat nothing to flush.
	/// </summary>
	public sealed class SyncExecutionMiddleware : IExecutionMiddleware
	{
		private readonly object _lock = new object();

		public void Execute(Action execute)
		{
			try
			{
				lock (_lock) execute();
			}
			catch (Exception ex)
			{
				Console.Write("Najlot.Log.Middleware.SyncExecutionMiddleware: ");

				while (ex != null)
				{
					Console.WriteLine(ex);
					ex = ex.InnerException;
				}
			}
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