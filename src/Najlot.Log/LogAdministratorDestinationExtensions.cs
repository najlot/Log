// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using System;

namespace Najlot.Log;

public static class LogAdministratorDestinationExtensions
{
	/// <summary>
	/// Adds a FileDestination
	/// </summary>
	/// <param name="path">Log file path.</param>
	/// <param name="maxFiles">Max count of files.</param>
	/// <param name="logFilePaths">File where to save the different logfiles to delete them when they are bigger then maxFiles</param>
	/// <param name="keepFileOpen">Should the file be kept open</param>
	/// <returns></returns>
	public static LogAdministrator AddFileDestination(
		this LogAdministrator logAdministrator,
		string path,
		int maxFiles = 30,
		string logFilePaths = null,
		bool keepFileOpen = false)
	{
		var destination = new FileDestination(path, maxFiles, logFilePaths, keepFileOpen);
		return logAdministrator.AddCustomDestination(destination);
	}

	/// <summary>
	/// Adds a FileDestination that calculates the path
	/// </summary>
	/// <param name="getFileName">Function to calculate the path</param>
	/// <param name="maxFiles">Max count of files.</param>
	/// <param name="logFilePaths">File where to save the different logfiles to delete them when they are bigger then maxFiles</param>
	/// <param name="keepFileOpen">Should the file be kept open</param>
	/// <returns></returns>
	public static LogAdministrator AddFileDestination(
		this LogAdministrator logAdministrator,
		Func<string> getFileName,
		int maxFiles = 30,
		string logFilePaths = null,
		bool keepFileOpen = true)
	{
		var destination = new FileDestination(getFileName, maxFiles, logFilePaths, keepFileOpen);
		return logAdministrator.AddCustomDestination(destination);
	}


	/// <summary>
	/// Adds a destination that writes to the console.
	/// All destinations will be used when creating a logger from a LoggerPool.
	/// </summary>
	/// <param name="useColors"></param>
	/// <returns></returns>
	public static LogAdministrator AddConsoleDestination(this LogAdministrator logAdministrator, bool useColors = false)
	{
		var destination = new ConsoleDestination(useColors);
		return logAdministrator.AddCustomDestination(destination);
	}

	/// <summary>
	/// Adds a destination that puts the requests on an HTTP server
	/// </summary>
	/// <param name="url">Url of the server</param>
	/// <param name="token">Authentication token</param>
	/// <returns></returns>
	public static LogAdministrator AddHttpDestination(this LogAdministrator logAdministrator, string url, string token = null)
	{
		var destination = new HttpDestination(url, token);
		return logAdministrator.AddCustomDestination(destination);
	}
}