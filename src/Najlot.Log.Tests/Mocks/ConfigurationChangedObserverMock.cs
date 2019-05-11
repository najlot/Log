using System;

namespace Najlot.Log.Tests.Mocks
{
	public sealed class ConfigurationChangedObserverMock : IConfigurationObserver
	{
		private Action<ILogConfiguration> _configurationChangedAction;

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