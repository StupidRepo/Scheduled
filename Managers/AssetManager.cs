using System.Reflection;
using BepInEx.Logging;
using UnityEngine;
using Logger = BepInEx.Logging.Logger;

namespace Scheduled.Test;

public class AssetManager
{
	public readonly AssetBundle assetBundle;
	public readonly Dictionary<string, object> assets = [];
	
	private readonly ManualLogSource logger = Logger.CreateLogSource("AssetLoader");

	public AssetManager(string name)
	{
		var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		if (stream == null)
		{
			logger.LogError($"The Stream \"{name}\" is null!");
			return;
		}

		assetBundle = AssetBundle.LoadFromStream(stream);
		stream.Close(); // pro tip: close stream to free up resources 😉
		
		foreach (var asset in assetBundle.LoadAllAssets())
		{
			if (asset == null)
			{
				logger.LogError($"Failed to load asset: {name}");
				continue;
			}
			
			assets.Add(asset.name, asset);
			logger.LogInfo($"Loaded asset: {asset.name}");
		}
	}

	public T? GetAsset<T>(string name, T? fallback = null) where T: class
	{
		assets.TryGetValue(name, out var result);
		return (T?)result ?? fallback;
	}
}