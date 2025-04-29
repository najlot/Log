// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Configuration.FileSource.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Najlot.Log.Configuration.FileSource.Json;

internal class JsonConfigurationService : IConfigurationService
{
	private static JsonSerializerOptions _options;
	private static JsonSerializerOptions Options => _options ??= new()
	{
		PropertyNameCaseInsensitive = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		NumberHandling = JsonNumberHandling.AllowReadingFromString,
		Converters = { new JsonStringEnumConverter() },
		WriteIndented = true
	};

	public LogConfiguration ReadFromString(string content)
	{
		return JsonSerializer.Deserialize<LogConfiguration>(content, Options);
	}

	public string WriteToString(LogConfiguration configurations)
	{
		return JsonSerializer.Serialize(configurations, Options);
	}
}