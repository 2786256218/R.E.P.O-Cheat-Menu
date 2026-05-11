using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace Cheat.Features.ItemSpawner;

public class ItemSpawner : MonoBehaviour
{
	public class SpawnableItemDef
	{
		public string Name;

		public Func<GameObject> PrefabGetter;
	}

	public static ItemSpawner Instance;

	public List<SpawnableItemDef> SpawnableItems = new List<SpawnableItemDef>();

	public SpawnableItemDef SelectedItem;

	private string _searchQuery = "";

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		CacheItems();
	}

	public void CacheItems()
	{
		SpawnableItems.Clear();
		if (!((UnityEngine.Object)(object)AssetManager.instance == (UnityEngine.Object)null))
		{
			SpawnableItems.Add(new SpawnableItemDef
			{
				Name = "Surplus Valuable Small",
				PrefabGetter = () => AssetManager.instance.surplusValuableSmall
			});
			SpawnableItems.Add(new SpawnableItemDef
			{
				Name = "Surplus Valuable Medium",
				PrefabGetter = () => AssetManager.instance.surplusValuableMedium
			});
			SpawnableItems.Add(new SpawnableItemDef
			{
				Name = "Surplus Valuable Big",
				PrefabGetter = () => AssetManager.instance.surplusValuableBig
			});
			SpawnableItems.Add(new SpawnableItemDef
			{
				Name = "Enemy Valuable Small",
				PrefabGetter = () => AssetManager.instance.enemyValuableSmall
			});
			SpawnableItems.Add(new SpawnableItemDef
			{
				Name = "Enemy Valuable Medium",
				PrefabGetter = () => AssetManager.instance.enemyValuableMedium
			});
			SpawnableItems.Add(new SpawnableItemDef
			{
				Name = "Enemy Valuable Big",
				PrefabGetter = () => AssetManager.instance.enemyValuableBig
			});
			SpawnableItems.Sort((SpawnableItemDef a, SpawnableItemDef b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
		}
	}

	public void SpawnSelectedItem()
	{
		if (SelectedItem != null)
		{
			SpawnItem(SelectedItem);
		}
	}

	public void SpawnItem(SpawnableItemDef itemDef)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (itemDef != null && SemiFunc.IsMasterClientOrSingleplayer())
		{
			GameObject val = itemDef.PrefabGetter();
			if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
			{
				Debug.LogError((object)("Prefab for " + itemDef.Name + " is null in AssetManager!"));
				return;
			}
			Ray val2 = default(Ray);
			val2 = new Ray(((Component)Camera.main).transform.position, ((Component)Camera.main).transform.forward);
			RaycastHit val3 = default(RaycastHit);
			Vector3 position = ((!Physics.Raycast(val2, out val3, 10f, LayerMask.GetMask(new string[2] { "Default", "StaticGrabObject" }))) ? (((Component)Camera.main).transform.position + ((Component)Camera.main).transform.forward * 2f) : (val3.point + Vector3.up * 0.5f));
			SpawnItemAt(val, position, Quaternion.identity);
		}
	}

	private void SpawnItemAt(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!((UnityEngine.Object)(object)prefab == (UnityEngine.Object)null))
		{
			string text = "Valuables/" + ((UnityEngine.Object)prefab).name;
			if (SemiFunc.IsMultiplayer())
			{
				PhotonNetwork.Instantiate(text, position, rotation, (byte)0, null);
			}
			else
			{
				UnityEngine.Object.Instantiate<GameObject>(prefab, position, rotation);
			}
		}
	}

	public void SelectItem(SpawnableItemDef item)
	{
		SelectedItem = item;
	}

	public List<SpawnableItemDef> GetFilteredItems()
	{
		if (string.IsNullOrEmpty(_searchQuery))
		{
			return SpawnableItems;
		}
		return SpawnableItems.Where((SpawnableItemDef i) => i.Name != null && i.Name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
	}

	public void UpdateSearch(string query)
	{
		_searchQuery = query;
	}

	public string GetSearchQuery()
	{
		return _searchQuery;
	}
}
