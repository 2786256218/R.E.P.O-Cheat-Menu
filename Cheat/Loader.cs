using System;
using Cheat.Features.Compass;
using Cheat.Features.Enemies;
using Cheat.Features.LocalPlayer;
using Cheat.Features.Loot;
using Cheat.Features.Minimap;
using Cheat.Features.Visuals;
using Cheat.Utils;
using UnityEngine;

namespace Cheat;

public static class Loader
{
	private static GameObject _loadObject;

	public static void Init()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		ConsoleBase.Init();
		Cheat.Utils.Logger.Info("Loader Init called", "Loader");
		try
		{
			if ((UnityEngine.Object)(object)_loadObject != (UnityEngine.Object)null)
			{
				Cheat.Utils.Logger.Warn("Existing instance found. Unloading before re-initializing...", "Loader");
				Unload();
			}
			_loadObject = new GameObject();
			_loadObject.AddComponent<CheatMenu>();
			_loadObject.AddComponent<Minimap>();
			_loadObject.AddComponent<Cheat.Features.Compass.Compass>();
			_loadObject.AddComponent<Cheat.Features.RPCFixManager>();
			UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)(object)_loadObject);
			Cheat.Utils.Logger.Info("Cheat components loaded successfully", "Loader");
		}
		catch (Exception arg)
		{
			Cheat.Utils.Logger.Critical($"Failed to load cheat components: {arg}", "Loader");
		}
	}

	public static void Unload()
	{
		Cheat.Utils.Logger.Info("Unloading...", "Loader");
		try
		{
			if ((UnityEngine.Object)(object)_loadObject != (UnityEngine.Object)null)
			{
				CheatMenu component = _loadObject.GetComponent<CheatMenu>();
				if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
				{
					component.ForceClose();
				}
				UnityEngine.Object.Destroy((UnityEngine.Object)(object)_loadObject);
				_loadObject = null;
				LootChams.Cleanup();
				EnemyChams.Cleanup();
				LootManager.Cleanup();
				EnemyManager.Cleanup();
				LocalPlayerManager.Cleanup();
			}
		}
		catch (Exception arg)
		{
			Cheat.Utils.Logger.Error($"Error during unload: {arg}", "Loader");
		}
		finally
		{
			ConsoleBase.Destroy();
		}
	}
}
