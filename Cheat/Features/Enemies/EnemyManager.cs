using System;
using System.Collections.Generic;
using System.Reflection;
using Cheat.Config;
using UnityEngine;
using UnityEngine.AI;

namespace Cheat.Features.Enemies;

public class EnemyManager
{
	public class EnemyData
	{
		public EnemyParent Parent;

		public Enemy Enemy;

		public Vector3 SmoothedPosition;

		public Vector3 LastPosition;

		public float LastUpdateTime;

		public List<Renderer> Renderers = new List<Renderer>();

		public NavMeshAgent Agent;

		public EnemyHealth Health;

		public List<Mesh> Meshes = new List<Mesh>();

		public List<Transform> MeshTransforms = new List<Transform>();

		public List<Mesh> BakedMeshes = new List<Mesh>();

		public List<Component> Animators = new List<Component>();

		public PathVisualizer PathViz;

		public List<GameObject> ChamsObjects = new List<GameObject>();
	}

	private static Dictionary<int, EnemyData> _enemyCache = new Dictionary<int, EnemyData>();

	private static PropertyInfo _animatorCullingModeProp;

	public static void Update()
	{
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Expected O, but got Unknown
		if ((UnityEngine.Object)(object)EnemyDirector.instance == (UnityEngine.Object)null)
		{
			return;
		}
		if (!ConfigManager.Config.Enemies.EspEnabled && !ConfigManager.Config.Enemies.HighlightEnabled)
		{
			if (_enemyCache.Count > 0)
			{
				Cleanup();
			}
			return;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, EnemyData> item in _enemyCache)
		{
			if ((UnityEngine.Object)(object)item.Value.Parent == (UnityEngine.Object)null || (UnityEngine.Object)(object)item.Value.Enemy == (UnityEngine.Object)null)
			{
				list.Add(item.Key);
			}
		}
		foreach (int item2 in list)
		{
			_enemyCache.Remove(item2);
		}
		foreach (EnemyParent item3 in EnemyDirector.instance.enemiesSpawned)
		{
			if ((UnityEngine.Object)(object)item3 == (UnityEngine.Object)null)
			{
				continue;
			}
			int instanceID = ((UnityEngine.Object)item3).GetInstanceID();
			if (!_enemyCache.TryGetValue(instanceID, out var value))
			{
				Enemy fieldValue = ReflectionUtils.GetFieldValue<Enemy>(item3, "Enemy");
				if ((UnityEngine.Object)(object)fieldValue != (UnityEngine.Object)null)
				{
					value = new EnemyData
					{
						Parent = item3,
						Enemy = fieldValue,
						SmoothedPosition = ((Component)fieldValue).transform.position,
						LastPosition = ((Component)fieldValue).transform.position,
						LastUpdateTime = Time.time
					};
					Renderer[] componentsInChildren = ((Component)item3).GetComponentsInChildren<Renderer>(true);
					Renderer[] array = componentsInChildren;
					foreach (Renderer val in array)
					{
						if (!(val is SkinnedMeshRenderer) && !(val is MeshRenderer))
						{
							continue;
						}
						value.Renderers.Add(val);
						Mesh val2 = null;
						SkinnedMeshRenderer val3 = (SkinnedMeshRenderer)(object)((val is SkinnedMeshRenderer) ? val : null);
						if (val3 != null)
						{
							val2 = val3.sharedMesh;
							val3.updateWhenOffscreen = true;
							value.BakedMeshes.Add(new Mesh());
							continue;
						}
						MeshRenderer val4 = (MeshRenderer)(object)((val is MeshRenderer) ? val : null);
						if (val4 != null)
						{
							value.BakedMeshes.Add(null);
							MeshFilter component = ((Component)val).GetComponent<MeshFilter>();
							if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
							{
								val2 = component.sharedMesh;
							}
							if ((UnityEngine.Object)(object)val2 != (UnityEngine.Object)null)
							{
								value.Meshes.Add(val2);
								value.MeshTransforms.Add(((Component)val).transform);
							}
						}
					}
					Type type = Type.GetType("UnityEngine.Animator, UnityEngine.AnimationModule");
					if (type == null)
					{
						type = Type.GetType("UnityEngine.Animator, UnityEngine");
					}
					if (type != null)
					{
						Component[] componentsInChildren2 = ((Component)item3).GetComponentsInChildren(type, true);
						value.Animators.AddRange(componentsInChildren2);
						if (_animatorCullingModeProp == null)
						{
							_animatorCullingModeProp = type.GetProperty("cullingMode");
						}
						if (_animatorCullingModeProp != null)
						{
							Component[] array2 = componentsInChildren2;
							foreach (Component obj in array2)
							{
								_animatorCullingModeProp.SetValue(obj, 0, null);
							}
						}
					}
					EnemyNavMeshAgent fieldValue2 = ReflectionUtils.GetFieldValue<EnemyNavMeshAgent>(fieldValue, "NavMeshAgent");
					if ((UnityEngine.Object)(object)fieldValue2 != (UnityEngine.Object)null)
					{
						value.Agent = ReflectionUtils.GetFieldValue<NavMeshAgent>(fieldValue2, "Agent");
						if ((UnityEngine.Object)(object)value.Agent != (UnityEngine.Object)null && (UnityEngine.Object)(object)value.PathViz == (UnityEngine.Object)null)
						{
							GameObject val5 = new GameObject($"PathVisualizer_{instanceID}");
							value.PathViz = val5.AddComponent<PathVisualizer>();
							value.PathViz.Initialize(value.Agent);
						}
					}
					value.Health = ReflectionUtils.GetFieldValue<EnemyHealth>(fieldValue, "Health");
					_enemyCache[instanceID] = value;
				}
			}
			if (value != null)
			{
				EnforceVisibility(value);
				Vector3 position = ((Component)value.Enemy).transform.position;
				if ((UnityEngine.Object)(object)value.Enemy.CenterTransform != (UnityEngine.Object)null && Vector3.Distance(value.Enemy.CenterTransform.position, ((Component)value.Enemy).transform.position) < 5f)
				{
					position = value.Enemy.CenterTransform.position;
				}
				value.SmoothedPosition = Vector3.Lerp(value.SmoothedPosition, position, Time.deltaTime * 15f);
			}
		}
	}

	public static EnemyData GetEnemyData(EnemyParent parent)
	{
		if ((UnityEngine.Object)(object)parent == (UnityEngine.Object)null)
		{
			return null;
		}
		_enemyCache.TryGetValue(((UnityEngine.Object)parent).GetInstanceID(), out var value);
		return value;
	}

	public static void Cleanup()
	{
		foreach (KeyValuePair<int, EnemyData> item in _enemyCache)
		{
			if (item.Value == null)
			{
				continue;
			}
			foreach (Mesh bakedMesh in item.Value.BakedMeshes)
			{
				if ((UnityEngine.Object)(object)bakedMesh != (UnityEngine.Object)null)
				{
					UnityEngine.Object.Destroy((UnityEngine.Object)(object)bakedMesh);
				}
			}
			if ((UnityEngine.Object)(object)item.Value.PathViz != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Destroy((UnityEngine.Object)(object)((Component)item.Value.PathViz).gameObject);
			}
		}
		_enemyCache.Clear();
	}

	private static void EnforceVisibility(EnemyData data)
	{
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		// if ((UnityEngine.Object)(object)data.Parent != (UnityEngine.Object)null)
		// {
		// 	ReflectionUtils.SetFieldValue(data.Parent, "playerClose", true);
		// }
		if ((UnityEngine.Object)(object)data.Enemy != (UnityEngine.Object)null)
		{
			object fieldValue = ReflectionUtils.GetFieldValue(data.Enemy, "OnScreen");
			if (fieldValue != null)
			{
				ReflectionUtils.SetFieldValue(fieldValue, "maxDistance", 10000f);
			}
		}
		if (data.Renderers != null)
		{
			foreach (Renderer renderer in data.Renderers)
			{
				if ((UnityEngine.Object)(object)renderer == (UnityEngine.Object)null)
				{
					continue;
				}
				SkinnedMeshRenderer val = (SkinnedMeshRenderer)(object)((renderer is SkinnedMeshRenderer) ? renderer : null);
				if (val != null)
				{
					if (!val.updateWhenOffscreen)
					{
						val.updateWhenOffscreen = true;
					}
					Bounds localBounds = ((Renderer)val).localBounds;
					Vector3 extents = localBounds.extents;
					if (extents.sqrMagnitude < 5000f)
					{
						((Renderer)val).localBounds = new Bounds(Vector3.zero, Vector3.one * 10000f);
					}
				}
			}
		}
		if (data.Animators == null || !(_animatorCullingModeProp != null))
		{
			return;
		}
		foreach (Component animator in data.Animators)
		{
			if (!((UnityEngine.Object)(object)animator == (UnityEngine.Object)null))
			{
				try
				{
					_animatorCullingModeProp.SetValue(animator, 0, null);
				}
				catch
				{
				}
			}
		}
	}
}
