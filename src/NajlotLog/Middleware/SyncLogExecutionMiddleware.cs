using System;

namespace NajlotLog.Middleware
{
    public class SyncLogExecutionMiddleware : ILogExecutionMiddleware
    {
		public void Execute(Action execute)
        {
            execute();
        }

        public void Dispose()
        {
            
        }

        public void Flush()
        {
            
        }
    }
}