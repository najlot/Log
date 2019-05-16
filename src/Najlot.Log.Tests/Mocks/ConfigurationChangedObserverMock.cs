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