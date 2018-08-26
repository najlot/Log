using NajlotLog.Middleware;
using System;

namespace NajlotLog.Configuration
{
	public interface ILogConfiguration
	{
		LogLevel LogLevel { get; set; }
		ILogExecutionMiddleware LogExecutionMiddleware { get; set; }
		
		void AttachObserver(IConfigurationChangedObserver observer);
		void DetachObserver(IConfigurationChangedObserver observer);
		void NotifyObservers(Type type = null);

		bool TryGetFormatFunctionForType(Type type, out Func<LogMessage, string> function);
		bool TrySetFormatFunctionForType(Type type, Func<LogMessage, string> function);
		void ClearAllFormatFunctions();
	}
}
