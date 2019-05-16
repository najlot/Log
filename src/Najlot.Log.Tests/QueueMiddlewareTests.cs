using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class QueueMiddlewareTests
	{
		public QueueMiddlewareTests()
		{
			foreach (var type in typeof(QueueMiddlewareTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void QueueMiddlewareCanBeSetAndGetForMultipleTypes()
		{
			var admin = LogAdminitrator.CreateNew();

			var formatMiddlewareName = LogConfigurationMapper.Instance.GetName(typeof(FormatMiddleware));
			var noFilterMiddlewareName = LogConfigurationMapper.Instance.GetName(typeof(NoFilterMiddleware));
			var noQueueMiddlewareName = LogConfigurationMapper.Instance.GetName(typeof(NoQueueMiddleware));

			admin.SetQueueMiddlewareForName<NoQueueMiddleware>(formatMiddlewareName);
			admin.SetQueueMiddlewareForName<QueueMiddlewareMock>(noFilterMiddlewareName);
			admin.SetQueueMiddlewareForName<NoQueueMiddleware>(noQueueMiddlewareName);

			admin.GetQueueMiddlewareNameForName(formatMiddlewareName, out var formatMiddlewareNameActual);
			admin.GetQueueMiddlewareNameForName(noFilterMiddlewareName, out var noFilterMiddlewareActual);
			admin.GetQueueMiddlewareNameForName(noQueueMiddlewareName, out var noQueueMiddlewareNameActual);

			Assert.Equal(nameof(NoQueueMiddleware), formatMiddlewareNameActual);
			Assert.Equal(nameof(QueueMiddlewareMock), noFilterMiddlewareActual);
			Assert.Equal(nameof(NoQueueMiddleware), noQueueMiddlewareNameActual);
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