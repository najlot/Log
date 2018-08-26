using NajlotLog.Configuration;
using System;

namespace NajlotLog.Tests.Mocks
{
	public class SecondConfigurationChangedObserverMock : IConfigurationChangedObserver
	{
		private Action<ILogConfiguration> _configurationChangedAction;

		public SecondConfigurationChangedObserverMock(Action<ILogConfiguration> configurationChangedAction)
		{
			_configurationChangedAction = configurationChangedAction;
		}

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			_configurationChangedAction(configuration);
		}
	}
}
