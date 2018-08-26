using System;

namespace NajlotLog.Middleware
{
    public interface ILogExecutionMiddleware : IDisposable
	{
		void Execute(Action execute);
		void Flush();
	}
}