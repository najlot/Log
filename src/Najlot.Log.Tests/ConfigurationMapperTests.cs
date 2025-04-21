// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Tests.Mocks;
using Xunit;

namespace Najlot.Log.Tests;

public class ConfigurationMapperTests
{
	[Fact]
	public void MapperShouldHaveDefaultNames()
	{
		var mapper = LogConfigurationMapper.Instance;

		foreach (var type in typeof(LogConfigurationMapper).Assembly.GetTypes())
		{
			if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length <= 0)
			{
				continue;
			}

			var name = mapper.GetName(type);
			Assert.NotNull(name);
			Assert.NotNull(mapper.GetType(name));
		}
	}

	[Fact]
	public void MapperShouldMapNamesGeneric()
	{
		var mapper = LogConfigurationMapper.Instance;
		mapper.AddToMapping<MiddlewareMock>();
		var name = mapper.GetName<MiddlewareMock>();
		Assert.NotNull(name);
		Assert.NotNull(mapper.GetType(name));
	}

	[Fact]
	public void MapperShouldMapNamesByType()
	{
		var mapper = LogConfigurationMapper.Instance;
		var type = typeof(MiddlewareMock);
		mapper.AddToMapping(type);
		var name = mapper.GetName(type);
		Assert.NotNull(name);
		Assert.NotNull(mapper.GetType(name));
	}

	[Fact]
	public void MapperShouldReturnNullIfNotFound()
	{
		var mapper = LogConfigurationMapper.Instance;
		var type = typeof(string);
		var name = mapper.GetName(type);
		Assert.Null(name);
		var unknown = mapper.GetType(type.Name);
		Assert.Null(unknown);
		Assert.Null(mapper.GetType(null));
	}
}