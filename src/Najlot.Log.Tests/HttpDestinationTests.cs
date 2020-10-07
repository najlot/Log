// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log.Tests.Configuration;
using System.Linq;
using Xunit;

namespace Najlot.Log.Tests
{
	public class HttpDestinationTests
	{
		[Fact]
		public void HttpDestinationTest()
		{
			var config = ConfigurationReader.ReadConfiguration<HttpDestinationConfig>();

			if (config == null)
			{
				return;
			}

			LogConfigurationMapper.Instance.AddToMapping<HttpDestination>();

			using (var admin = LogAdministrator.CreateNew())
			{
				admin
					.SetLogLevel(LogLevel.Debug)
					.SetCollectMiddleware<ConcurrentCollectMiddleware, HttpDestination>()
					.AddMiddleware<JsonFormatMiddleware, HttpDestination>()
					.AddHttpDestination(config.Url, config.Token);
				
				var logger = admin.GetLogger(typeof(HttpDestinationTests));
				
				foreach (var i in Enumerable.Range(0, 500))
				{
					logger.Info(i);
				}
			}
		}
	}
}