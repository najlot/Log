using Microsoft.Extensions.Logging;

namespace Najlot.Log.Extensions.Logging
{
	[ProviderAlias("Najlot.Log")]
	public sealed class NajlotLogProvider : ILoggerProvider
	{
		private LogAdminitrator _logAdminitrator;
		private bool _disposed = false;

		public NajlotLogProvider(LogAdminitrator logConfigurator)
		{
			_logAdminitrator = logConfigurator;
		}

		public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
		{
			return new NajlotLogWrapper(_logAdminitrator.GetLogger(categoryName));
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				_logAdminitrator.Dispose();
				_logAdminitrator = null;
			}
		}
	}
}