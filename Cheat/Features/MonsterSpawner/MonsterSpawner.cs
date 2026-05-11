using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

namespace Cheat.Features.MonsterSpawner;

public class MonsterSpawner : MonoBehaviour
{
	public static MonsterSpawner Instance;

	private List<EnemySetup> _spawnableEnemies = new List<EnemySetup>();

	private List<Enemy> _activeEnemies = new List<Enemy>();

	private GameObject _previewInstance;

	private Camera _previewCamera;

	private RenderTexture _previewTexture;

	private EnemySetup _currentPreviewEnemy;

	private float _updateTimer = 0f;

	private const float UpdateInterval = 1f;

	public List<EnemySetup> SpawnableEnemies => _spawnableEnemies;

	public EnemySetup SelectedEnemy { get; private set; }

	public List<Enemy> ActiveEnemies => _activeEnemies;

	public RenderTexture PreviewTexture => _previewTexture;

	private void Awake()
	{
		Instance = this;
		SetupPreviewRendering();
	}

	private void Update()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		_updateTimer += Time.deltaTime;
		if (_updateTimer >= 1f)
		{
			UpdateActiveEnemies();
			RefreshLibrary();
			_updateTimer = 0f;
		}
		if ((UnityEngine.Object)(object)_previewInstance != (UnityEngine.Object)null)
		{
			_previewInstance.transform.Rotate(Vector3.up, 30f * Time.deltaTime);
		}
	}

	private void RefreshLibrary()
	{
		if ((UnityEngine.Object)(object)EnemyDirector.instance == (UnityEngine.Object)null)
		{
			return;
		}
		List<EnemySetup> list = new List<EnemySetup>();
		if (EnemyDirector.instance.enemiesDifficulty1 != null)
		{
			list.AddRange(EnemyDirector.instance.enemiesDifficulty1);
		}
		if (EnemyDirector.instance.enemiesDifficulty2 != null)
		{
			list.AddRange(EnemyDirector.instance.enemiesDifficulty2);
		}
		if (EnemyDirector.instance.enemiesDifficulty3 != null)
		{
			list.AddRange(EnemyDirector.instance.enemiesDifficulty3);
		}
		try
		{
			FieldInfo field = typeof(EnemyDirector).GetField("debugEnemy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null && field.GetValue(EnemyDirector.instance) is EnemySetup[] collection)
			{
				list.AddRange(collection);
			}
		}
		catch
		{
		}
		_spawnableEnemies = (from e in list.Distinct()
			orderby ((UnityEngine.Object)e).name
			select e).ToList();
		if ((UnityEngine.Object)(object)SelectedEnemy == (UnityEngine.Object)null && _spawnableEnemies.Count > 0)
		{
			SelectEnemy(_spawnableEnemies[0]);
		}
	}

	private void UpdateActiveEnemies()
	{
		_activeEnemies = UnityEngine.Object.FindObjectsOfType<Enemy>().ToList();
		_activeEnemies.RemoveAll(delegate(Enemy e)
		{
			if ((UnityEngine.Object)(object)e == (UnityEngine.Object)null)
			{
				return true;
			}
			EnemyHealth component = ((Component)e).GetComponent<EnemyHealth>();
			if (!((UnityEngine.Object)(object)component == (UnityEngine.Object)null))
			{
				try
				{
					FieldInfo field = typeof(EnemyHealth).GetField("healthCurrent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					FieldInfo field2 = typeof(EnemyHealth).GetField("dead", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					int num = ((field != null) ? ((int)field.GetValue(component)) : 100);
					bool flag = field2 != null && (bool)field2.GetValue(component);
					return num <= 0 && flag;
				}
				catch
				{
					return false;
				}
			}
			return false;
		});
	}

	public void SelectEnemy(EnemySetup enemy)
	{
		SelectedEnemy = enemy;
		UpdatePreviewModel(enemy);
	}

	public void SpawnSelectedEnemy()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!((UnityEngine.Object)(object)SelectedEnemy == (UnityEngine.Object)null) && (PhotonNetwork.IsMasterClient || GameManager.instance.gameMode == 0) && !((UnityEngine.Object)(object)LevelGenerator.Instance == (UnityEngine.Object)null))
		{
			Vector3 val = Vector3.zero;
			if ((UnityEngine.Object)(object)PlayerController.instance != (UnityEngine.Object)null)
			{
				val = ((Component)PlayerController.instance).transform.position + ((Component)PlayerController.instance).transform.forward * 3f + Vector3.up * 0.5f;
			}
			LevelGenerator.Instance.EnemySpawn(SelectedEnemy, val);
		}
	}

	public void KillEnemy(Enemy enemy)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!((UnityEngine.Object)(object)enemy == (UnityEngine.Object)null))
		{
			EnemyHealth component = ((Component)enemy).GetComponent<EnemyHealth>();
			if (!((UnityEngine.Object)(object)component == (UnityEngine.Object)null))
			{
				component.Hurt(9999, Vector3.zero);
			}
		}
	}

	public void TeleportToEnemy(Enemy enemy)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (!((UnityEngine.Object)(object)enemy == (UnityEngine.Object)null) && !((UnityEngine.Object)(object)PlayerController.instance == (UnityEngine.Object)null))
		{
			Vector3 position = ((Component)enemy).transform.position + Vector3.up * 1f;
			((Component)PlayerController.instance).transform.position = position;
			if ((UnityEngine.Object)(object)PlayerController.instance.rb != (UnityEngine.Object)null)
			{
				PlayerController.instance.rb.velocity = Vector3.zero;
			}
		}
	}

	private void SetupPreviewRendering()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		_previewTexture = new RenderTexture(256, 256, 16, (RenderTextureFormat)0);
		_previewTexture.Create();
		GameObject val = new GameObject("MonsterPreviewCamera");
		_previewCamera = val.AddComponent<Camera>();
		_previewCamera.targetTexture = _previewTexture;
		_previewCamera.clearFlags = (CameraClearFlags)2;
		_previewCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0f);
		_previewCamera.cullingMask = LayerMask.GetMask(new string[1] { "Default" });
		_previewCamera.fieldOfView = 60f;
		val.transform.position = new Vector3(0f, -1000f, 0f);
		UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)(object)val);
		UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)(object)((Component)this).gameObject);
	}

	private List<GameObject> GetSpawnObjects(EnemySetup enemy)
	{
		if ((UnityEngine.Object)(object)enemy == (UnityEngine.Object)null)
		{
			return null;
		}
		try
		{
			FieldInfo field = typeof(EnemySetup).GetField("spawnObjects", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null && field.GetValue(enemy) is List<GameObject> result)
			{
				return result;
			}
		}
		catch
		{
		}
		try
		{
			List<GameObject> fieldValueByType = ReflectionUtils.GetFieldValueByType<List<GameObject>>(enemy);
			if (fieldValueByType != null)
			{
				return fieldValueByType;
			}
		}
		catch
		{
		}
		return null;
	}

	private void UpdatePreviewModel(EnemySetup enemy)
	{
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		if (((UnityEngine.Object)(object)_currentPreviewEnemy == (UnityEngine.Object)(object)enemy && (UnityEngine.Object)(object)_previewInstance != (UnityEngine.Object)null) || (UnityEngine.Object)(object)enemy == (UnityEngine.Object)null)
		{
			return;
		}
		_currentPreviewEnemy = enemy;
		if ((UnityEngine.Object)(object)_previewInstance != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy((UnityEngine.Object)(object)_previewInstance);
		}
		List<GameObject> spawnObjects = GetSpawnObjects(enemy);
		if (spawnObjects == null || spawnObjects.Count == 0)
		{
			return;
		}
		GameObject val = spawnObjects[0];
		_previewInstance = UnityEngine.Object.Instantiate<GameObject>(val);
		MonoBehaviour[] componentsInChildren = _previewInstance.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] array = componentsInChildren;
		foreach (MonoBehaviour val2 in array)
		{
			if (((object)val2).GetType() != typeof(Transform) && ((object)val2).GetType() != typeof(MeshFilter) && ((object)val2).GetType() != typeof(MeshRenderer) && ((object)val2).GetType() != typeof(SkinnedMeshRenderer))
			{
				((Behaviour)val2).enabled = false;
			}
		}
		Rigidbody[] componentsInChildren2 = _previewInstance.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array2 = componentsInChildren2;
		foreach (Rigidbody val3 in array2)
		{
			val3.isKinematic = true;
		}
		Collider[] componentsInChildren3 = _previewInstance.GetComponentsInChildren<Collider>();
		Collider[] array3 = componentsInChildren3;
		foreach (Collider val4 in array3)
		{
			val4.enabled = false;
		}
		_previewInstance.transform.position = ((Component)_previewCamera).transform.position + Vector3.forward * 5f;
		_previewInstance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		SetLayerRecursively(_previewInstance, 0);
	}

	private void SetLayerRecursively(GameObject obj, int layer)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		obj.layer = layer;
		foreach (Transform item in obj.transform)
		{
			Transform val = item;
			SetLayerRecursively(((Component)val).gameObject, layer);
		}
	}
}
