// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using System.Collections.Generic;
using Xunit;

namespace Najlot.Log.Tests;

public class LogAdministratorConfigurationTests
{
	[Fact]
	public void DestinationConfigurationShouldBeReadCorrect()
	{
		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddConsoleDestination(true);
		logAdministrator.AddFileDestination("my logfile path", 123);

		logAdministrator.GetDestinationConfiguration(nameof(ConsoleDestination), out var consoleDestinationConfiguration);

		Assert.Equal(1, consoleDestinationConfiguration.Count);
		Assert.Contains("UseColors", consoleDestinationConfiguration.Keys);

		logAdministrator.GetDestinationConfiguration(nameof(FileDestination), out var fileDestinationConfiguration);
		Assert.Equal(4, fileDestinationConfiguration.Count);

		Assert.Contains("OutputPath", fileDestinationConfiguration.Keys);
		Assert.Contains("KeepFileOpen", fileDestinationConfiguration.Keys);
		Assert.Contains("MaxFiles", fileDestinationConfiguration.Keys);
		Assert.Contains("LogFilesPath", fileDestinationConfiguration.Keys);

		Assert.Equal("my logfile path", fileDestinationConfiguration["OutputPath"]);
		Assert.False(bool.Parse(fileDestinationConfiguration["KeepFileOpen"]));
		Assert.Equal(123, int.Parse(fileDestinationConfiguration["MaxFiles"]));
		Assert.Null(fileDestinationConfiguration["LogFilesPath"]);
	}

	[Fact]
	public void DestinationConfigurationShouldBeWrittenCorrect()
	{
		using var logAdministrator = LogAdministrator.CreateNew();
		var destination = new FileDestination();
		logAdministrator.AddCustomDestination(destination);

		var fileDestinationConfiguration = new Dictionary<string, string>
		{
			["OutputPath"] = "my test path",
			["KeepFileOpen"] = "true",
			["MaxFiles"] = "10",
			["LogFilesPath"] = null
		};

		logAdministrator.SetDestinationConfiguration(nameof(FileDestination), fileDestinationConfiguration);

		Assert.Equal("my test path", destination.OutputPath);
		Assert.True(destination.KeepFileOpen);
		Assert.Equal(10, destination.MaxFiles);
		Assert.Null(destination.LogFilesPath);
	}
}