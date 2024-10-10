// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using System.IO;
using Xunit;

namespace Najlot.Log.Tests;

public class FileDestinationTests
{
	[Fact]
	public void FileDestinationShouldChangePaths()
	{
		var i = 0;

		string GetPath()
		{
			return $"log_{i}.log";
		}

		for (i = 0; i < 10; i++)
		{
			var path = GetPath();
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		var destination = new FileDestination(GetPath, 5, "logs", true);

		for (i = 0; i < 10; i++)
		{
			destination.Log(new LogMessage[]
			{
				new LogMessage()
				{
					Message = i.ToString(),
				}
			});
		}

		for (i = 0; i < 5; i++)
		{
			var path = GetPath();
			Assert.False(File.Exists(path));
		}

		for (i = 5; i < 10; i++)
		{
			var path = GetPath();
			Assert.True(File.Exists(path));
		}
	}
}