using Microsoft.Extensions.Logging;
using NajlotLog.Configuration;
using System.Threading.Tasks;

namespace NajlotLog.Extensions.Logging
{
	public static class LoggerFactoryExtension
	{
		public static ILoggerFactory AddNajlotLog(this ILoggerFactory loggerFactory, LoggerPool loggerPool, ILogConfiguration logConfiguration)
		{
			loggerFactory.AddProvider(new NajlotLogProvider(loggerPool, logConfiguration));
			return loggerFactory;
		}
    }
}
