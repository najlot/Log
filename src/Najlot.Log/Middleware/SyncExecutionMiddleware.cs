using System;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Executes everything syncronous.
	/// Has the advantage, that the messages are immediately there and it hat nothing to flush.
	/// </summary>
	public sealed class SyncExecutionMiddleware : IExecutionMiddleware
	{
		public void Execute(Action execute)
		{
			try
			{
				execute();
			}
			catch (Exception exception)
			{
				Console.Write("Najlot.Log.Middleware.SyncExecutionMiddleware: ");

				while (exception != null)
				{
					Console.WriteLine($"{exception}");
					exception = exception.InnerException;
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