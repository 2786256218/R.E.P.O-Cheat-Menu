using Cheat.Config;
using UnityEngine;

namespace Cheat.Features.Visuals;

public class LightingManager : MonoBehaviour
{
	private Color _originalAmbientLight;

	private bool _originalFog;

	private bool _overridden = false;

	private Light _fullbrightLight;

	private void LateUpdate()
	{
		bool fullbright = ConfigManager.Config.Misc.Fullbright;
		bool noFog = ConfigManager.Config.Misc.NoFog;
		
		if (fullbright)
		{
			if (_fullbrightLight == null)
			{
				GameObject lightObj = new GameObject("FullbrightLight");
				_fullbrightLight = lightObj.AddComponent<Light>();
				_fullbrightLight.type = LightType.Directional;
				_fullbrightLight.shadows = LightShadows.None;
				_fullbrightLight.renderMode = LightRenderMode.ForcePixel;
				DontDestroyOnLoad(lightObj);
			}
			_fullbrightLight.enabled = true;
			_fullbrightLight.intensity = ConfigManager.Config.Misc.FullbrightIntensity;
			_fullbrightLight.color = Color.white;
			
			if (Camera.main != null)
			{
				_fullbrightLight.transform.rotation = Camera.main.transform.rotation;
			}
		}
		else
		{
			if (_fullbrightLight != null)
			{
				_fullbrightLight.enabled = false;
			}
		}

		if (fullbright || noFog)
		{
			if (!_overridden)
			{
				_originalAmbientLight = RenderSettings.ambientLight;
				_originalFog = RenderSettings.fog;
				_overridden = true;
			}
			
			// Always override ambient light and fog if enabled
			if (fullbright && RenderSettings.ambientLight.r != ConfigManager.Config.Misc.FullbrightIntensity)
			{
				float fullbrightIntensity = ConfigManager.Config.Misc.FullbrightIntensity;
				RenderSettings.ambientLight = new Color(fullbrightIntensity, fullbrightIntensity, fullbrightIntensity, 1f);
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
			}
			if (noFog && RenderSettings.fog)
			{
				RenderSettings.fog = false;
			}
		}
		else if (_overridden)
		{
			RenderSettings.ambientLight = _originalAmbientLight;
			RenderSettings.fog = _originalFog;
			_overridden = false;
		}
	}

	public void OnDisable()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (_overridden)
		{
			RenderSettings.ambientLight = _originalAmbientLight;
			RenderSettings.fog = _originalFog;
			_overridden = false;
		}
	}
}
