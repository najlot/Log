﻿// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Najlot.Log.Extensions.Logging;

[ProviderAlias("Najlot.Log")]
public sealed class NajlotLogProvider : ILoggerProvider
{
	private LogAdministrator _logAdministrator;
	private bool _disposed = false;

	public NajlotLogProvider(LogAdministrator logConfigurator)
	{
		_logAdministrator = logConfigurator;
	}

	public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
	{
		return new NajlotLogWrapper(_logAdministrator.GetLogger(categoryName));
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}
		
		_disposed = true;

		_logAdministrator.Dispose();
		_logAdministrator = null;
	}
}