using Newtonsoft.Json;
using System.IO;

namespace Najlot.Log.Tests.Configuration;

public static class ConfigurationReader
{
	public static T ReadConfiguration<T>() where T : class, new()
	{
		const string configDir = "config";
		var configPath = Path.Combine(configDir, typeof(T).Name + ".json");
		configPath = Path.GetFullPath(configPath);

		if (!File.Exists(configPath))
		{
			if (File.Exists(configPath + ".example"))
			{
				return null;
			}
			
			Directory.CreateDirectory(configDir);

			File.WriteAllText(configPath + ".example", JsonConvert.SerializeObject(new T()));

			return null;
		}

		var configContent = File.ReadAllText(configPath);

		return JsonConvert.DeserializeObject<T>(configContent);
	}
}
