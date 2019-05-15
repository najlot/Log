using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;

namespace Najlot.Log
{
	internal sealed class LogDestinationEntry : IConfigurationObserver
	{
		public ILogDestination LogDestination;

		public string LogDestinationName;

		public IFormatMiddleware FormatMiddleware;

		public IExecutionMiddleware ExecutionMiddleware;

		public IFilterMiddleware FilterMiddleware;

		public IQueueMiddleware QueueMiddleware;

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			var mapper = LogConfigurationMapper.Instance;

			configuration.GetFormatMiddlewareNameForName(LogDestinationName, out var formatMiddlewareName);
			configuration.GetQueueMiddlewareNameForName(LogDestinationName, out var queueMiddlewareName);
			configuration.GetFilterMiddlewareNameForName(LogDestinationName, out var filterMiddlewareName);

			if (mapper.GetName(ExecutionMiddleware.GetType()) != configuration.ExecutionMiddlewareName)
			{
				var executionMiddlewareType = mapper.GetType(configuration.ExecutionMiddlewareName);
				ExecutionMiddleware = (IExecutionMiddleware)Activator.CreateInstance(executionMiddlewareType);
			}

			if (mapper.GetName(FilterMiddleware.GetType()) != filterMiddlewareName)
			{
				var filterMiddlewareType = mapper.GetType(filterMiddlewareName);
				FilterMiddleware = (IFilterMiddleware)Activator.CreateInstance(filterMiddlewareType);
			}

			if (mapper.GetName(QueueMiddleware.GetType()) != queueMiddlewareName)
			{
				var queueMiddlewareType = mapper.GetType(queueMiddlewareName);
				var priviousFormatMiddleware = QueueMiddleware.FormatMiddleware;
				QueueMiddleware = (IQueueMiddleware)Activator.CreateInstance(queueMiddlewareType);
				QueueMiddleware.FormatMiddleware = priviousFormatMiddleware;
			}

			if (mapper.GetName(FormatMiddleware.GetType()) != formatMiddlewareName)
			{
				var formatMiddlewareType = mapper.GetType(formatMiddlewareName);
				FormatMiddleware = (IFormatMiddleware)Activator.CreateInstance(formatMiddlewareType);
				QueueMiddleware.FormatMiddleware = FormatMiddleware;
			}
		}
	}
}