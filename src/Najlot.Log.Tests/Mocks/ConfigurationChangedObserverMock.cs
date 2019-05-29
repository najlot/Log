// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(ConfigurationChangedObserverMock))]
	public sealed class ConfigurationChangedObserverMock : IConfigurationObserver
	{
		private readonly Action<ILogConfiguration> _configurationChangedAction;

		public ConfigurationChangedObserverMock(Action<ILogConfiguration> configurationChangedAction)
		{
			_configurationChangedAction = configurationChangedAction;
		}

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			_configurationChangedAction(configuration);
		}
	}
}