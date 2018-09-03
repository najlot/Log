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
		private Dictionary<Type, Logger> _loggerCache = new Dictionary<Type, Logger>();

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
			Logger logger;

			lock (_loggerCache)
			{
				if (_loggerCache.TryGetValue(sourceType, out logger))
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
								var clonedLogDestination = CloneLogDestination(sourceType, _logDestinations[0]);
								logger = new Logger(clonedLogDestination, _logConfiguration);
							}

							break;

						default:
							{
								var loggerList = new LoggerList();

								foreach (var logDestination in _logDestinations)
								{
									var clonedLogDestination = CloneLogDestination(sourceType, logDestination);
									loggerList.Add(clonedLogDestination);
								}

								logger = new Logger(loggerList, _logConfiguration);
							}

							break;
					}
				}

				_loggerCache.Add(sourceType, logger);
			}

			return logger;
		}

		private ILogger CloneLogDestination(Type sourceType, ILogger LogDestination)
		{
			var logDestinationType = LogDestination.GetType();
			var cloneMethod = logDestinationType.GetMethod("Clone", new Type[] { typeof(Type) });
			var clonedlogDestination = cloneMethod.Invoke(LogDestination, new object[] { sourceType });

			_logConfiguration.AttachObserver(clonedlogDestination as IConfigurationChangedObserver);
			return clonedlogDestination as ILogger;
		}
	}
}
