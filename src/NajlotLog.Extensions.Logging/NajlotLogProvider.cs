using Microsoft.Extensions.Logging;
using NajlotLog.Configuration;

namespace NajlotLog.Extensions.Logging
{
	[ProviderAlias("NajlotLog")]
	internal class NajlotLogProvider : ILoggerProvider
	{
		private LoggerPool _loggerPool;
		private ILogConfiguration _logConfiguration;

		public NajlotLogProvider(LoggerPool loggerPool, ILogConfiguration logConfiguration)
		{
			_loggerPool = loggerPool;
			_logConfiguration = logConfiguration;
		}

		public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
		{
			return new NajlotLogWrapper(_loggerPool.GetLogger(categoryName));
		}
		
		public void Dispose()
		{
			_logConfiguration.ExecutionMiddleware.Flush();
		}
	}
}
