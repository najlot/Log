using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;

namespace Najlot.Log
{
	internal sealed class LogDestinationEntry : IConfigurationChangedObserver
	{
		public ILogDestination LogDestination;

		public Type LogDestinationType;

		public Func<LogMessage, string> FormatFunc;

		public IExecutionMiddleware ExecutionMiddleware;

		public IFilterMiddleware FilterMiddleware;

		public void NotifyConfigurationChanged(ILogConfiguration configuration)
		{
			if (configuration.TryGetFormatFunctionForType(this.LogDestination.GetType(), out var formatFunc))
			{
				FormatFunc = formatFunc;
			}
			else
			{
				FormatFunc = DefaultFormatFuncHolder.DefaultFormatFunc;
			}

			ExecutionMiddleware = (IExecutionMiddleware)Activator.CreateInstance(configuration.ExecutionMiddlewareType);
			FilterMiddleware = (IFilterMiddleware)Activator.CreateInstance(configuration.FilterMiddlewareType);
		}
	}
}