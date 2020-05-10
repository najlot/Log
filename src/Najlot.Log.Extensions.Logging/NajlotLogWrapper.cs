// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Najlot.Log.Extensions.Logging
{
	internal class NajlotLogWrapper : Microsoft.Extensions.Logging.ILogger
	{
		private readonly Logger _logger;

		public NajlotLogWrapper(Logger logger)
		{
			_logger = logger;
		}

		public IDisposable BeginScope<TState>(TState state)
			=> _logger.BeginScope(state);

		public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
			=> _logger.IsEnabled((LogLevel)logLevel);

		public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}

			if (state is IReadOnlyList<KeyValuePair<string, object>> values && values.Count > 1)
			{
				var format = "Unknown";
				var args = new List<KeyValuePair<string, object>>(values);

				foreach (KeyValuePair<string, object> value in values)
				{
					if (value.Key == "{OriginalFormat}")
					{
						format = value.Value.ToString();
						args.Remove(value);
						break;
					}
				}

				if (format == "Unknown")
				{
					format = state.ToString();
				}

				switch (logLevel)
				{
					case Microsoft.Extensions.Logging.LogLevel.Trace:
						_logger.Trace(exception, format, args);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Debug:
						_logger.Debug(exception, format, args);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Information:
						_logger.Info(exception, format, args);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Warning:
						_logger.Warn(exception, format, args);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Error:
						_logger.Error(exception, format, args);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Critical:
						_logger.Fatal(exception, format, args);
						break;
				}
			}
			else
			{
				switch (logLevel)
				{
					case Microsoft.Extensions.Logging.LogLevel.Trace:
						_logger.Trace(exception, state);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Debug:
						_logger.Debug(exception, state);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Information:
						_logger.Info(exception, state);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Warning:
						_logger.Warn(exception, state);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Error:
						_logger.Error(exception, state);
						break;

					case Microsoft.Extensions.Logging.LogLevel.Critical:
						_logger.Fatal(exception, state);
						break;
				}
			}
		}
	}
}