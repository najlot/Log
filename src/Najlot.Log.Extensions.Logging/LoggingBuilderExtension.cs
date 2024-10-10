// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using System;

namespace Najlot.Log.Extensions.Logging;

public static class LoggingBuilderExtension
{
	public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, Action<LogAdministrator> configure)
	{
		var admin = LogAdministrator.CreateNew();
		configure(admin);
		return builder.AddNajlotLog(admin);
	}

	public static ILoggingBuilder AddNajlotLog(this ILoggingBuilder builder, LogAdministrator logConfigurator)
	{
		builder.AddProvider(new NajlotLogProvider(logConfigurator));
		return builder;
	}
}