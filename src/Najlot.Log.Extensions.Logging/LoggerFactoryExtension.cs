// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using System;

namespace Najlot.Log.Extensions.Logging;

public static class LoggerFactoryExtension
{
	public static ILoggerFactory AddNajlotLog(this ILoggerFactory loggerFactory, Action<LogAdministrator> configure)
	{
		var admin = LogAdministrator.CreateNew();
		configure(admin);
		return loggerFactory.AddNajlotLog(admin);
	}

	public static ILoggerFactory AddNajlotLog(this ILoggerFactory loggerFactory, LogAdministrator logAdministrator)
	{
		loggerFactory.AddProvider(new NajlotLogProvider(logAdministrator));
		return loggerFactory;
	}
}