using System;

namespace NajlotLog
{
    public interface ILogger
	{
		IDisposable BeginScope<T>(T state);

		void Trace<T>(T o);
		void Debug<T>(T o);
		void Error<T>(T o);
		void Fatal<T>(T o);
		void Info<T>(T o);
		void Warn<T>(T o);
		
		void Flush();
	}
}