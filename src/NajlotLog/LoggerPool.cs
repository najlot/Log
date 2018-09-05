using NajlotLog.Configuration;
using NajlotLog.Destinations;
using System;
using System.Collections.Generic;

namespace NajlotLog
{
	/// <summary>
	/// Class for managing instances of loggers and log destinations.
	/// </summary>
	public class LoggerPool
	{
		/// <summary>
		/// Static LoggerPool instance
		/// </summary>
		public static LoggerPool Instance { get; } = new LoggerPool(LogConfiguration.Instance);

		/// <summary>
		/// Creates new LoggerPool with a new configuration.
		/// </summary>
		/// <returns></returns>
		public static LoggerPool CreateNew()
		{
			return new LoggerPool(new LogConfiguration());
		}

		private ILogConfiguration _logConfiguration;
		private List<ILogger> _logDestinations = new List<ILogger>();
		private Dictionary<string, Logger> _loggerCache = new Dictionary<string, Logger>();

		public LoggerPool(ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
		}

		internal void AddLogDestination<T>(T logDestination) where T : LogDestinationPrototype<T>, ILogger
		{
			lock (_logDestinations) _logDestinations.Add(logDestination);
		}

		public Logger GetLogger(Type sourceType)
		{
			return GetLogger(sourceType.FullName);
		}

		public Logger GetLogger(string category)
		{
			Logger logger;

			lock (_loggerCache)
			{
				if (_loggerCache.TryGetValue(category, out logger))
				{
					return logger;
				}

				lock (_logDestinations)
				{
					switch (_logDestinations.Count)
					{
						case 0:
							{
								Console.WriteLine("NajlotLog: There are no log destinations specified: Creating console log destination.");
								var consoleLogger = new ConsoleLogDestination(_logConfiguration);
								_logConfiguration.AttachObserver(consoleLogger);
								logger = new Logger(consoleLogger, _logConfiguration);
							}

							break;

						case 1:
							{
								var clonedLogDestination = CloneLogDestination(category, _logDestinations[0]);
								logger = new Logger(clonedLogDestination, _logConfiguration);
							}

							break;

						default:
							{
								var loggerList = new LoggerList();

								foreach (var logDestination in _logDestinations)
								{
									var clonedLogDestination = CloneLogDestination(category, logDestination);
									loggerList.Add(clonedLogDestination);
								}

								logger = new Logger(loggerList, _logConfiguration);
							}

							break;
					}
				}

				_loggerCache.Add(category, logger);
			}

			return logger;
		}

		private ILogger CloneLogDestination(string category, ILogger LogDestination)
		{
			var logDestinationType = LogDestination.GetType();
			var cloneMethod = logDestinationType.GetMethod("Clone", new Type[] { typeof(string) });
			var clonedlogDestination = cloneMethod.Invoke(LogDestination, new object[] { category });

			_logConfiguration.AttachObserver(clonedlogDestination as IConfigurationChangedObserver);
			return clonedlogDestination as ILogger;
		}
	}
}
