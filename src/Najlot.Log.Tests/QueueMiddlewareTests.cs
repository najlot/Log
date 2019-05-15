using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class QueueMiddlewareTests
	{
		[Fact]
		public void QueueMiddlewareCanBeSetAndGetForMultipleTypes()
		{
			var admin = LogAdminitrator.CreateNew();

			var formatMiddlewareName = LogConfigurationMapper.Instance.GetName(typeof(FormatMiddleware));
			var noFilterMiddlewareName = LogConfigurationMapper.Instance.GetName(typeof(NoFilterMiddleware));
			var noQueueMiddlewareName = LogConfigurationMapper.Instance.GetName(typeof(NoQueueMiddleware));

			admin.SetFormatMiddlewareForName<FormatToAbcMiddleware>(formatMiddlewareName);
			admin.SetFormatMiddlewareForName<FormatToEmptyMiddleware>(noFilterMiddlewareName);
			admin.SetFormatMiddlewareForName<FormatToAbcMiddleware>(noQueueMiddlewareName);

			admin.GetFormatMiddlewareNameForName(formatMiddlewareName, out var formatMiddlewareNameActual);
			admin.GetFormatMiddlewareNameForName(noFilterMiddlewareName, out var noFilterMiddlewareActual);
			admin.GetFormatMiddlewareNameForName(noQueueMiddlewareName, out var noQueueMiddlewareNameActual);

			Assert.Equal(formatMiddlewareName, formatMiddlewareNameActual);
			Assert.Equal(noFilterMiddlewareName, noFilterMiddlewareActual);
			Assert.Equal(noQueueMiddlewareName, noQueueMiddlewareNameActual);
		}

		[Fact]
		public void ThereShouldBeADefaultQueueMiddleware()
		{
			var fileDestinationName = LogConfigurationMapper.Instance.GetName(typeof(FileLogDestination));

			LogAdminitrator
				.CreateNew()
				.GetQueueMiddlewareNameForName(fileDestinationName, out var queueMiddlewareName);

			var queueMiddlewareType = LogConfigurationMapper.Instance.GetType(queueMiddlewareName);

			// Will throw if can not create
			Assert.NotNull((IQueueMiddleware)Activator.CreateInstance(queueMiddlewareType));
		}
	}
}