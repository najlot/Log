using Microsoft.Extensions.Logging;

namespace Najlot.Log.Extensions.Logging
{
	[ProviderAlias("Najlot.Log")]
	public sealed class NajlotLogProvider : ILoggerProvider
	{
		private LogConfigurator _logConfigurator;
		private LoggerPool _loggerPool;
		private bool _disposed = false;

		public NajlotLogProvider(LogConfigurator logConfigurator)
		{
			_logConfigurator = logConfigurator;
			_logConfigurator.GetLoggerPool(out _loggerPool);
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
				_loggerPool = null;

				_logConfigurator.Dispose();
				_logConfigurator = null;
			}
		}
	}
}
