using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;

namespace Najlot.Log
{
	internal sealed class LogDestinationEntry : IConfigurationChangedObserver
	{
		public ILogDestination LogDestination;

		public Type LogDestinationType;

		public IFormatMiddleware FormatMiddleware;

		public IExecutionMiddleware ExecutionMiddleware;

		public IFilterMiddleware FilterMiddleware;

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			if (ExecutionMiddleware.GetType() != configuration.ExecutionMiddlewareType)
			{
				ExecutionMiddleware = (IExecutionMiddleware)Activator.CreateInstance(configuration.ExecutionMiddlewareType);
			}

			if (FilterMiddleware.GetType() != configuration.FilterMiddlewareType)
			{
				FilterMiddleware = (IFilterMiddleware)Activator.CreateInstance(configuration.FilterMiddlewareType);
			}

			configuration.GetFormatMiddlewareTypeForType(LogDestinationType, out var formatMiddlewareType);

			if (FormatMiddleware.GetType() != formatMiddlewareType)
			{
				FormatMiddleware = (IFormatMiddleware)Activator.CreateInstance(formatMiddlewareType);
			}
		}
	}
}