// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.Linq;
using System.Threading;
using Xunit;

namespace Najlot.Log.Tests
{
	public class TimerQueueMiddlewareTests
	{
		public TimerQueueMiddlewareTests()
		{
			foreach (var type in typeof(TimerQueueMiddlewareTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void QueueMiddlewareShouldNotWriteDirectly()
		{
			var count = 0;

			using (var administrator = LogAdminitrator.CreateNew()
				.AddCustomDestination(new LogDestinationEnumerableMock(messages =>
				{
					count = messages.Count();
				}))
				.SetQueueMiddleware<TimerQueueMiddleware>(nameof(LogDestinationEnumerableMock)))
			{
				var logger = administrator.GetLogger("default");

				logger.Error(1);
				logger.Error(2);
				logger.Error(3);
				logger.Error(4);
				logger.Error(5);

				Assert.Equal(0, count);

				logger.Flush();

				Assert.Equal(5, count);
			}
		}

		[Fact]
		public void QueueMiddlewareShouldWriteAutomatically()
		{
			var count = 0;

			using (var administrator = LogAdminitrator.CreateNew()
				.AddCustomDestination(new LogDestinationEnumerableMock(messages =>
				{
					count = messages.Count();
				}))
				.SetQueueMiddleware<TimerQueueMiddleware>(nameof(LogDestinationEnumerableMock)))
			{
				var logger = administrator.GetLogger("default");

				logger.Error(1);
				logger.Error(2);
				logger.Error(3);
				logger.Error(4);
				logger.Error(5);

				Assert.Equal(0, count);

				Thread.Sleep(1250);

				Assert.Equal(5, count);
			}
		}
	}
}