// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

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

			var formatMiddlewareName = configuration.GetFormatMiddlewareName(LogDestinationName);
			var queueMiddlewareName = configuration.GetQueueMiddlewareName(LogDestinationName);
			var filterMiddlewareName = configuration.GetFilterMiddlewareName(LogDestinationName);

			if (mapper.GetName(ExecutionMiddleware) != configuration.ExecutionMiddlewareName)
			{
				var executionMiddlewareType = mapper.GetType(configuration.ExecutionMiddlewareName);
				ExecutionMiddleware = (IExecutionMiddleware)Activator.CreateInstance(executionMiddlewareType);
			}

			if (mapper.GetName(FilterMiddleware) != filterMiddlewareName)
			{
				var filterMiddlewareType = mapper.GetType(filterMiddlewareName);
				FilterMiddleware = (IFilterMiddleware)Activator.CreateInstance(filterMiddlewareType);
			}

			if (mapper.GetName(QueueMiddleware) != queueMiddlewareName)
			{
				var queueMiddlewareType = mapper.GetType(queueMiddlewareName);
				var priviousFormatMiddleware = QueueMiddleware.FormatMiddleware;
				QueueMiddleware = (IQueueMiddleware)Activator.CreateInstance(queueMiddlewareType);
				QueueMiddleware.FormatMiddleware = priviousFormatMiddleware;
				QueueMiddleware.Destination = LogDestination;
			}

			if (mapper.GetName(FormatMiddleware) != formatMiddlewareName)
			{
				var formatMiddlewareType = mapper.GetType(formatMiddlewareName);
				FormatMiddleware = (IFormatMiddleware)Activator.CreateInstance(formatMiddlewareType);
				QueueMiddleware.FormatMiddleware = FormatMiddleware;
			}
		}
	}
}