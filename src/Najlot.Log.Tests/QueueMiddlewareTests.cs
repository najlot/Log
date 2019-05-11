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

			admin.SetFormatMiddlewareForType<FormatToAbcMiddleware>(typeof(LogMessage));
			admin.SetFormatMiddlewareForType<FormatToEmptyMiddleware>(this.GetType());
			admin.SetFormatMiddlewareForType<FormatToAbcMiddleware>(typeof(Logger));

			admin.GetFormatMiddlewareTypeForType(typeof(LogMessage), out var formatMiddlewareForLogmessage);
			admin.GetFormatMiddlewareTypeForType(this.GetType(), out var formatMiddlewareTypeForThis);
			admin.GetFormatMiddlewareTypeForType(typeof(Logger), out var formatMiddlewareTypeForLogger);

			Assert.Equal(typeof(FormatToAbcMiddleware), formatMiddlewareForLogmessage);
			Assert.Equal(typeof(FormatToEmptyMiddleware), formatMiddlewareTypeForThis);
			Assert.Equal(typeof(FormatToAbcMiddleware), formatMiddlewareTypeForLogger);
		}

		[Fact]
		public void ThereShouldBeADefaultQueueMiddleware()
		{
			LogAdminitrator
				.CreateNew()
				.GetQueueMiddlewareTypeForType(this.GetType(),out var queueMiddlewareType);

			// Will throw if can not create
			Assert.NotNull((IQueueMiddleware)Activator.CreateInstance(queueMiddlewareType));
		}
	}
}