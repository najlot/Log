using NajlotLog.Configuration;
using NajlotLog.Implementation;
using System;
using System.Collections.Generic;

namespace NajlotLog
{
	public class LoggerPool
	{
		public static LoggerPool Instance { get; } = new LoggerPool();

		private List<ILogger> _appenders = new List<ILogger>();
		private Dictionary<Type, Logger> _loggerCache = new Dictionary<Type, Logger>();
		
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
								Console.Error.WriteLine("Creating logger without appenders");
								var consoleLogger = new ConsoleLoggerImplementation();
								logger = new Logger(LogConfiguration.Instance.LogLevel, consoleLogger);
							}

							break;

						case 1:
							logger = new Logger(LogConfiguration.Instance.LogLevel, _appenders[0]);
							break;

						default:
							{
								var loggerList = new LoggerList();

								foreach (var appender in _appenders)
								{
									loggerList.Add(appender
										.GetType()
										.GetMethod("Clone", new Type[] { typeof(Type) })
										.Invoke(appender, new object[] { sourceType }) as ILogger);
								}

								logger = new Logger(LogConfiguration.Instance.LogLevel, loggerList);
							}

							break;
					}
				}

				_loggerCache.Add(sourceType, logger);
			}

			return logger;
		}
	}
}
