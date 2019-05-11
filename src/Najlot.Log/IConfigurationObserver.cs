namespace Najlot.Log
{
	public interface IConfigurationObserver
	{
		void NotifyConfigurationChanged(ILogConfiguration configuration);
	}
}