using NajlotLog.Configuration;
using System;

namespace NajlotLog.Tests.Mocks
{
	public class ConfigurationChangedObserverMock : IConfigurationChangedObserver
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
