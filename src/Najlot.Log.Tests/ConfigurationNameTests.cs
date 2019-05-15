using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ConfigurationNameTests
	{
		[Fact]
		public void MapperShouldHaveDefaultNames()
		{
			var mapper = LogConfigurationMapper.Instance;

			foreach (Type type in typeof(LogConfigurationMapper).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					var name = mapper.GetName(type);
					Assert.NotNull(name);
					Assert.NotNull(mapper.GetType(name));
				}
			}
		}

		[Fact]
		public void MapperShouldMapNames()
		{
			var mapper = LogConfigurationMapper.Instance;
			var type = typeof(DenyAllFilterMiddleware);
			mapper.AddToMapping(type);
			var name = mapper.GetName(type);
			Assert.NotNull(name);
			Assert.NotNull(mapper.GetType(name));
		}
	}
}