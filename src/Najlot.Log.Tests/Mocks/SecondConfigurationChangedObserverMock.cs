using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using System;

namespace Najlot.Log.Tests.Mocks
{
	public class SecondConfigurationChangedObserverMock : LogDestinationBase, IConfigurationChangedObserver
	{
		private Action<ILogConfiguration> _configurationChangedAction;

		public SecondConfigurationChangedObserverMock(
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
