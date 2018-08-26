using System;

namespace NajlotLog.Configuration
{
	public interface IConfigurationChangedObserver
	{
		void NotifyConfigurationChanged(ILogConfiguration configuration);
	}
}
