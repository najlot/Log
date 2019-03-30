using Microsoft.Extensions.Logging;
using System;

namespace Najlot.Log.Extensions.Logging
{
	internal class NajlotLogWrapper : Microsoft.Extensions.Logging.ILogger
	{
		private readonly Logger _logger;

		public NajlotLogWrapper(Logger logger)
		{
			this._logger = logger;
		}

		public IDisposable BeginScope<TState>(TState state)
			=> _logger.BeginScope(state);

		public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
			=> _logger.IsEnabled((LogLevel)logLevel);

		public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (IsEnabled(logLevel))
			{
				switch (logLevel)
				{
					case Microsoft.Extensions.Logging.LogLevel.Trace:
						_logger.Trace(state, exception);
						return;

					case Microsoft.Extensions.Logging.LogLevel.Debug:
						_logger.Debug(state, exception);
						return;

					case Microsoft.Extensions.Logging.LogLevel.Information:
						_logger.Info(state, exception);
						return;

					case Microsoft.Extensions.Logging.LogLevel.Warning:
						_logger.Warn(state, exception);
						return;

					case Microsoft.Extensions.Logging.LogLevel.Error:
						_logger.Error(state, exception);
						return;

					case Microsoft.Extensions.Logging.LogLevel.Critical:
						_logger.Fatal(state, exception);
						return;
				}
			}
		}
	}
}