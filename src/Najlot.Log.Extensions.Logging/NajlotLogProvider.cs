using Microsoft.Extensions.Logging;
using Najlot.Log.Configuration;

namespace Najlot.Log.Extensions.Logging
{
	[ProviderAlias("Najlot.Log")]
	public sealed class NajlotLogProvider : ILoggerProvider
	{
		private readonly LoggerPool _loggerPool;
		private readonly ILogConfiguration _logConfiguration;
		private bool _disposed = false;

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
			if(!_disposed)
			{
				_disposed = true;
				_logConfiguration.ExecutionMiddleware.Flush();
			}
		}
	}
}
