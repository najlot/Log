using NajlotLog.Configuration;
using NajlotLog.Destinations;
using System;

namespace NajlotLog.Tests.Mocks
{
	public class ConfigurationChangedObserverMock : LogDestinationBase, IConfigurationChangedObserver
	{
		private Action<ILogConfiguration> _configurationChangedAction;

		public ConfigurationChangedObserverMock(
			ILogConfiguration configuration, 
			Action<ILogConfiguration> configurationChangedAction) : base(configuration)
		{
			_configurationChangedAction = configurationChangedAction;
		}

		public new void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			_configurationChangedAction(configuration);
		}

		protected override void Log(LogMessage message)
		{
			throw new NotImplementedException();
		}
	}
}
