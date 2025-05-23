﻿// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Najlot.Log.Tests.Mocks;

public class DependencyInjectionLoggerService
{
	private readonly ILogger<DependencyInjectionLoggerService> _logger;

	public DependencyInjectionLoggerService(ILogger<DependencyInjectionLoggerService> logger)
	{
		_logger = logger;
	}

	public ILogger<DependencyInjectionLoggerService> GetLogger()
	{
		return _logger;
	}
}