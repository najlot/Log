// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Configuration.FileSource.Models;
using System.IO;
using System.Text;

namespace Najlot.Log.Configuration.FileSource;

internal static class ConfigurationServiceExtensions
{
	public static LogConfiguration ReadFromFile(this IConfigurationService service, string path)
	{
		var content = File.ReadAllText(path);
		return service.ReadFromString(content);
	}

	public static void WriteToFile(this IConfigurationService service, string path, LogConfiguration configurations)
	{
		var content = service.WriteToString(configurations);
		File.WriteAllText(path, content, Encoding.UTF8);
	}
}