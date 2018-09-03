using NajlotLog.Configuration;
using NajlotLog.Implementation;
using System;
using System.Collections.Generic;

namespace NajlotLog
{
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
		private List<ILogger> _appenders = new List<ILogger>();
		private Dictionary<Type, Logger> _loggerCache = new Dictionary<Type, Logger>();

		public LoggerPool(ILogConfiguration logConfiguration)
		{
			_logConfiguration = logConfiguration;
		}

		internal void AddAppender<T>(T appender) where T : LoggerPrototype<T>, ILogger
		{
			lock (_appenders) _appenders.Add(appender);
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

				lock (_appenders)
				{
					switch (_appenders.Count)
					{
						case 0:
							{
								Console.WriteLine("NajlotLog: There are no appenders specified: Creating console appender.");
								var consoleLogger = new ConsoleLoggerImplementation(_logConfiguration);
								_logConfiguration.AttachObserver(consoleLogger);
								logger = new Logger(consoleLogger, _logConfiguration);
							}

							break;

						case 1:
							{
								var clonedAppender = CloneAppender(sourceType, _appenders[0]);
								logger = new Logger(clonedAppender, _logConfiguration);
							}

							break;

						default:
							{
								var loggerList = new LoggerList();

								foreach (var appender in _appenders)
								{
									var clonedAppender = CloneAppender(sourceType, appender);
									loggerList.Add(clonedAppender);
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

		private ILogger CloneAppender(Type sourceType, ILogger appender)
		{
			var appenderType = appender.GetType();
			var cloneMethod = appenderType.GetMethod("Clone", new Type[] { typeof(Type) });
			var clonedAppender = cloneMethod.Invoke(appender, new object[] { sourceType });

			_logConfiguration.AttachObserver(clonedAppender as IConfigurationChangedObserver);
			return clonedAppender as ILogger;
		}
	}
}
