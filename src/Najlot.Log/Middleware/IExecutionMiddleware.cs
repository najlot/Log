using System;

namespace Najlot.Log.Middleware
{
	/// <summary>
	/// Fundamental interface for an execution middleware.
	/// The middleware can choose to execute immediately, later, async, in an other thread... or not to execute at all.
	/// </summary>
	public interface IExecutionMiddleware : IDisposable
	{
		/// <summary>
		/// Gives the execution middleware an action to execute.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		void Execute(Action execute);

		/// <summary>
		/// Forces the execution middleware to clean everything up.
		/// </summary>
		void Flush();
	}
}
