// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.Linq;
using System.Threading;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ConcurrentCollectMiddlewareTests
	{
		[Fact]
		public void MiddlewareShouldCollectAndWriteMultiple()
		{
			var maxMessages = 0;

			using var middleware = new ConcurrentCollectMiddleware
			{
				NextMiddleware = new ActionMiddleware(messages =>
				{
					Thread.Sleep(10);

					var count = messages.Count();
					if (maxMessages < count)
					{
						maxMessages = count;
					}
				})
			};

			for (var i = 0; i < 25; i++)
			{
				middleware.Execute(new LogMessage());
			}

			middleware.Flush();

			Assert.NotEqual(0, maxMessages);
			Assert.NotEqual(1, maxMessages);
		}

		[Fact]
		public void FlushShouldWriteAllMessages()
		{
			var messagesCount = 0;

			using var middleware = new ConcurrentCollectMiddleware
			{
				NextMiddleware = new ActionMiddleware(messages =>
				{
					Thread.Sleep(10);

					var count = messages.Count();
					messagesCount += count;
				})
			};

			for (var i = 0; i < 25; i++)
			{
				middleware.Execute(new LogMessage());
			}

			middleware.Flush();

			Assert.Equal(25, messagesCount);
		}

		[Fact]
		public void DisposeShouldWriteAllMessages()
		{
			var messagesCount = 0;

			{
				using var middleware = new ConcurrentCollectMiddleware
				{
					NextMiddleware = new ActionMiddleware(messages =>
					{
						Thread.Sleep(10);

						var count = messages.Count();
						messagesCount += count;
					})
				};

				for (var i = 0; i < 25; i++)
				{
					middleware.Execute(new LogMessage());
				}
			}

			Assert.Equal(25, messagesCount);
		}
	}
}