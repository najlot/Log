using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;

namespace Najlot.Log
{
	internal sealed class LogDestinationEntry : IConfigurationChangedObserver
	{
		public ILogDestination LogDestination;

		public Func<LogMessage, string> FormatFunc;

		public IExecutionMiddleware ExecutionMiddleware;

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
		}
	}
}