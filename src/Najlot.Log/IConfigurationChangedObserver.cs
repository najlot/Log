namespace Najlot.Log
{
	public interface IConfigurationChangedObserver
	{
		void NotifyConfigurationChanged(ILogConfiguration configuration);
	}
}