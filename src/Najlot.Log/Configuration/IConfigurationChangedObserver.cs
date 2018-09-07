using System;

namespace Najlot.Log.Configuration
{
	public interface IConfigurationChangedObserver
	{
		void NotifyConfigurationChanged(ILogConfiguration configuration);
	}
}
