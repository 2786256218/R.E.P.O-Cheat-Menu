using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Cheat.Config;

public static class ConfigManager
{
	[Serializable]
	private class MetaData
	{
		public string LastProfile = "default";
	}

	private static string _configDirectory;

	private static string _currentProfile = "default";

	private const string MetaFileName = "meta.json";

	public static ConfigData Config { get; private set; }

	public static void Init()
	{
		_configDirectory = Path.Combine(Application.persistentDataPath, "CheatConfigs");
		if (!Directory.Exists(_configDirectory))
		{
			Directory.CreateDirectory(_configDirectory);
		}
		LoadLastConfig();
	}

	public static void LoadLastConfig()
	{
		string path = Path.Combine(_configDirectory, "meta.json");
		string profileName = "default";
		if (File.Exists(path))
		{
			try
			{
				string text = File.ReadAllText(path);
				MetaData metaData = JsonUtility.FromJson<MetaData>(text);
				if (!string.IsNullOrEmpty(metaData.LastProfile))
				{
					profileName = metaData.LastProfile;
				}
			}
			catch
			{
			}
		}
		LoadConfig(profileName);
	}

	private static void SaveMeta()
	{
		try
		{
			MetaData metaData = new MetaData
			{
				LastProfile = _currentProfile
			};
			string contents = JsonUtility.ToJson((object)metaData, true);
			File.WriteAllText(Path.Combine(_configDirectory, "meta.json"), contents);
		}
		catch
		{
		}
	}

	public static void SaveConfig(string profileName)
	{
		if (Config == null)
		{
			Config = new ConfigData();
		}
		string path = Path.Combine(_configDirectory, profileName + ".json");
		string contents = JsonUtility.ToJson((object)Config, true);
		File.WriteAllText(path, contents);
		_currentProfile = profileName;
		SaveMeta();
	}

	public static void LoadConfig(string profileName)
	{
		string path = Path.Combine(_configDirectory, profileName + ".json");
		if (File.Exists(path))
		{
			string text = File.ReadAllText(path);
			Config = JsonUtility.FromJson<ConfigData>(text);
		}
		else
		{
			Config = new ConfigData();
			SaveConfig(profileName);
		}
		_currentProfile = profileName;
	}

	public static List<string> GetProfiles()
	{
		List<string> list = new List<string>();
		if (Directory.Exists(_configDirectory))
		{
			string[] files = Directory.GetFiles(_configDirectory, "*.json");
			string[] array = files;
			foreach (string path in array)
			{
				list.Add(Path.GetFileNameWithoutExtension(path));
			}
		}
		return list;
	}

	public static string GetCurrentProfile()
	{
		return _currentProfile;
	}
}
