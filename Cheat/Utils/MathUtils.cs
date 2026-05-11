using System;
using UnityEngine;

namespace Cheat.Utils;

public static class MathUtils
{
	private static object _cachedRtMain;

	private static bool _rtMainCached;

	private static Type _rtMainType;

	public static Vector3 WorldToScreen(Vector3 worldPos)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		Camera main = Camera.main;
		if ((UnityEngine.Object)(object)main == (UnityEngine.Object)null)
		{
			return new Vector3(0f, 0f, -1f);
		}
		Vector3 val = main.WorldToScreenPoint(worldPos);
		if (!_rtMainCached)
		{
			_rtMainType = Type.GetType("RenderTextureMain, Assembly-CSharp");
			if (_rtMainType == null)
			{
				_rtMainType = Type.GetType("RenderTextureMain");
			}
			if (_rtMainType != null)
			{
				_cachedRtMain = UnityEngine.Object.FindObjectOfType(_rtMainType);
			}
			_rtMainCached = true;
		}
		if (_cachedRtMain != null)
		{
			object cachedRtMain = _cachedRtMain;
			if ((UnityEngine.Object)((cachedRtMain is UnityEngine.Object) ? cachedRtMain : null) == (UnityEngine.Object)null && _rtMainType != null)
			{
				_cachedRtMain = UnityEngine.Object.FindObjectOfType(_rtMainType);
			}
			if (_cachedRtMain != null)
			{
				int fieldValue = ReflectionUtils.GetFieldValue<int>(_cachedRtMain, "textureWidth");
				int fieldValue2 = ReflectionUtils.GetFieldValue<int>(_cachedRtMain, "textureHeight");
				if (fieldValue > 0 && fieldValue2 > 0)
				{
					val.x *= (float)Screen.width / (float)fieldValue;
					val.y *= (float)Screen.height / (float)fieldValue2;
				}
			}
		}
		val.y = (float)Screen.height - val.y;
		return val;
	}

	public static bool IsOnScreen(Vector3 screenPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		return screenPos.z > 0f && screenPos.x > 0f && screenPos.x < (float)Screen.width && screenPos.y > 0f && screenPos.y < (float)Screen.height;
	}
}
