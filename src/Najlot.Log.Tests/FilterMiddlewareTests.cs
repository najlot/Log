using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests
{
	public class FilterMiddlewareTests
	{
		[Fact]
		public void FilterMiddlewareCanBlockMessages()
		{
			var fileName = nameof(FilterMiddlewareCanBlockMessages) + ".log";

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			var log = LogAdminitrator
				.CreateNew()
				.AddFileLogDestination(fileName, keepFileOpen: false)
				.SetFilterMiddleware<DenyAllFilterMiddleware>()
				.GetLogger("");

			log.Fatal("TEST!");

			Assert.False(File.Exists(fileName));
		}

		[Fact]
		public void FilterMiddlewareCanBeChanged()
		{
			var fileName = nameof(FilterMiddlewareCanBeChanged) + ".log";

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			var logAdminitrator = LogAdminitrator
				.CreateNew()
				.AddFileLogDestination(fileName, keepFileOpen: false)
				.SetFilterMiddleware<DenyAllFilterMiddleware>();

			var log = logAdminitrator.GetLogger("");

			log.Fatal("TEST!");

			Assert.False(File.Exists(fileName));

			logAdminitrator.SetFilterMiddleware<OpenFilterMiddleware>();

			log.Fatal("TEST!");

			Assert.True(File.Exists(fileName));
		}
	}
}