using System;
using System.Collections.Generic;
using System.Reflection;
using Cheat.Config;
using Cheat.Features.Enemies;
using Cheat.Features.ItemSpawner;
using Cheat.Features.LocalPlayer;
using Cheat.Features.Loot;
using Cheat.Features.MonsterSpawner;
using Cheat.Features.Visuals;
using Cheat.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cheat;

public class CheatMenu : MonoBehaviour
{
	public static CheatMenu Instance;

	private bool _visible = false;

	private Rect _windowRect;

	private bool _isDragging = false;

	private Vector2 _dragOffset;

	private SidebarManager _sidebar;

	private Vector2 _scrollPosition;

	private string _newProfileName = "";

	private bool _configDirty = false;

	private float _saveTimer = 0f;

	private const float SaveDelay = 2f;

	private float _originalAimSpeedMouse = 1f;

	private float _originalAimSpeedGamepad = 1f;

	private float _lastContentHeight = 0f;

	public bool Visible => _visible;

	private void Awake()
	{
		Instance = this;
		_sidebar = new SidebarManager();
	}

	private void Start()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Theme.Init();
		ConfigManager.Init();
		ConfigManager.LoadLastConfig();
		float num = ((float)Screen.width - Theme.WindowWidth) / 2f;
		float num2 = ((float)Screen.height - Theme.WindowHeight) / 2f;
		_windowRect = new Rect(num, num2, Theme.WindowWidth, Theme.WindowHeight);
		InitializeSidebar();
		UpdateMenuVisibility();
		if ((UnityEngine.Object)(object)((Component)this).GetComponent<MonsterSpawner>() == (UnityEngine.Object)null)
		{
			((Component)this).gameObject.AddComponent<MonsterSpawner>();
		}
		if ((UnityEngine.Object)(object)((Component)this).GetComponent<ItemSpawner>() == (UnityEngine.Object)null)
		{
			((Component)this).gameObject.AddComponent<ItemSpawner>();
		}
		if ((UnityEngine.Object)(object)((Component)this).GetComponent<LightingManager>() == (UnityEngine.Object)null)
		{
			((Component)this).gameObject.AddComponent<LightingManager>();
		}
	}

	private void InitializeSidebar()
	{
		_sidebar.AddSection("透视", "\ud83d\udc41", 0, null, expandedByDefault: true);
		_sidebar.AddSubTab(0, "玩家", 0, DrawPlayerVisualsTab);
		_sidebar.AddSubTab(0, "怪物", 1, DrawMonsterVisualsTab);
		_sidebar.AddSubTab(0, "贵重物品", 2, DrawValuablesTab);
		_sidebar.AddSubTab(0, "杂项", 3, DrawVisualsMiscTab);
		_sidebar.AddSection("生成", "\ud83d\udc80", 1, null, expandedByDefault: true);
		_sidebar.AddSubTab(1, "怪物", 0, DrawMonsterSpawnerTab);
		_sidebar.AddSubTab(1, "物品", 1, DrawItemSpawnerTab);
		_sidebar.AddSection("本地", "\ud83d\udc64", 2, DrawLocalTab);
		_sidebar.AddSection("杂项", "⚙", 3, DrawMiscTab);
		_sidebar.AddSection("设置", "\ud83d\udcbe", 4, DrawSettingsTab);
		_sidebar.Select(0, 0);
	}

	private void Update()
	{
		try
		{
			UIState.UpdateBoot(Time.unscaledDeltaTime);
			EnemyManager.Update();
			LootManager.Update();
			LocalPlayerManager.Update();
			FreeCam.Update();
			LaserSight.Update();
			HandleHotkeys();
			if (Input.GetKeyDown((KeyCode)277))
			{
				ToggleMenu();
			}
			if (_visible)
			{
				Cursor.lockState = (CursorLockMode)0;
				Cursor.visible = false;
				CursorManager val = UnityEngine.Object.FindObjectOfType<CursorManager>();
				if ((UnityEngine.Object)(object)val != (UnityEngine.Object)null)
				{
					val.Unlock(999f);
				}
				if ((UnityEngine.Object)(object)MenuCursor.instance != (UnityEngine.Object)null)
				{
					((Component)MenuCursor.instance).gameObject.SetActive(false);
				}
			}
			if (_visible)
			{
				if ((UnityEngine.Object)(object)CameraAim.Instance != (UnityEngine.Object)null)
				{
					CameraAim.Instance.OverrideAimStop();
					CameraAim.Instance.AimSpeedMouse = 0f;
					CameraAim.Instance.AimSpeedGamepad = 0f;
				}
				SetInputManagerDisabled(disabled: true);
			}
			else if (FreeCam.Enabled)
			{
				if ((UnityEngine.Object)(object)CameraAim.Instance != (UnityEngine.Object)null)
				{
					CameraAim.Instance.OverrideAimStop();
					CameraAim.Instance.AimSpeedMouse = 0f;
					CameraAim.Instance.AimSpeedGamepad = 0f;
				}
				SetInputManagerDisabled(disabled: false);
			}
			else
			{
				SetInputManagerDisabled(disabled: false);
			}
			if (Input.GetKeyDown((KeyCode)279))
			{
				Loader.Unload();
			}
			if (_configDirty)
			{
				_saveTimer -= Time.unscaledDeltaTime;
				if (_saveTimer <= 0f)
				{
					ConfigManager.SaveConfig(ConfigManager.GetCurrentProfile());
					_configDirty = false;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("[CheatMenu Update Error] " + ex.Message);
		}
	}

	private void ToggleMenu()
	{
		_visible = !_visible;
		UpdateMenuVisibility();
		if (_visible)
		{
			if (FreeCam.Enabled)
			{
				FreeCam.Toggle();
			}
			StoreGameValues();
			Input.ResetInputAxes();
			ToggleInputActions(enable: false);
			UIState.StartBoot();
			return;
		}
		Cursor.lockState = (CursorLockMode)1;
		Cursor.visible = false;
		RestoreGameValues();
		ToggleInputActions(enable: true);
		CursorManager val = UnityEngine.Object.FindObjectOfType<CursorManager>();
		if ((UnityEngine.Object)(object)val != (UnityEngine.Object)null)
		{
			val.Unlock(0f);
		}
		if ((UnityEngine.Object)(object)MenuCursor.instance != (UnityEngine.Object)null)
		{
			((Component)MenuCursor.instance).gameObject.SetActive(true);
		}
		UIState.Clear();
	}

	private void UpdateMenuVisibility()
	{
	}

	private void OnGUI()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			DrawESP();
			EnemyPreview.Update();
			if (FreeCam.Enabled)
			{
				FreeCam.DrawOverlay();
			}
			if (UIState.IsBooting)
			{
				BootRenderer.Draw();
			}
			if (_visible && UIState.WindowOpacity > 0.01f)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, UIState.WindowOpacity);
				DrawMenu();
				Elements.DrawActiveDropdownPopup();
				Elements.DrawColorPickerPopup();
				RenderQueue.Draw();
				RenderQueue.Clear();
				GUI.color = color;
				if (ConfigManager.Config.Misc.ShowKeybinds)
				{
					KeybindsUI.Draw();
				}
				if ((UnityEngine.Object)(object)Theme.CursorTexture != (UnityEngine.Object)null)
				{
					GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 24f, 24f), (Texture)(object)Theme.CursorTexture);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("[OnGUI Error] " + ex.Message);
		}
	}

	private void DrawESP()
	{
		int depth = GUI.depth;
		GUI.depth = 100;
		try
		{
			EnemyEsp.Draw();
			LootEsp.Draw();
			LootEsp.DrawCartUI();
			PlayerEsp.Draw();
			LaserSight.Draw();
		}
		catch
		{
		}
		GUI.depth = depth;
	}

	private void DrawMenu()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Invalid comparison between Unknown and I4
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		Elements.Window(_windowRect, "Cheat Menu", null, ref _isDragging, ref _dragOffset);
		if (_isDragging)
		{
			_windowRect.position = Event.current.mousePosition - _dragOffset;
		}
		float num = 50f;
		float sidebarWidth = Theme.SidebarWidth;
		float num2 = _windowRect.x + sidebarWidth + Theme.ContentMargin;
		float num3 = _windowRect.y + num + Theme.ContentMargin;
		float num4 = _windowRect.width - sidebarWidth - Theme.ContentMargin * 2f;
		float num5 = _windowRect.height - num - Theme.ContentMargin * 2f;
		float y = _windowRect.y + num + 20f;
		_sidebar.DrawSidebar(_windowRect.x, y, sidebarWidth);
		Rect val = default(Rect);
		val = new Rect(num2, num3, num4, num5);
		float num6 = 1000f;
		try
		{
			GUI.BeginGroup(val);
			float num7 = 0f - _scrollPosition.y;
			if (val.Contains(Event.current.mousePosition + new Vector2(_windowRect.x, _windowRect.y)) && (int)Event.current.type == 6)
			{
				_scrollPosition.y += Event.current.delta.y * 20f;
				Event.current.Use();
			}
			Action activeRenderAction = _sidebar.GetActiveRenderAction();
			if (activeRenderAction != null)
			{
				activeRenderAction();
				num6 = _lastContentHeight;
			}
			_scrollPosition.y = Mathf.Clamp(_scrollPosition.y, 0f, Mathf.Max(0f, num6 - val.height));
		}
		finally
		{
			GUI.EndGroup();
		}
		if (num6 > val.height)
		{
			Rect position = default(Rect);
			position = new Rect(num2 + num4 - 8f, num3, 6f, num5);
			_scrollPosition.y = Render.DrawCustomScrollbar(position, _scrollPosition.y, num5, 0f, num6 - num5, vertical: true);
		}
	}

	private void DrawValuablesTab()
	{
		_lastContentHeight = DrawLootTab(0f, 0f - _scrollPosition.y);
	}

	private void DrawMonsterVisualsTab()
	{
		_lastContentHeight = DrawMonsterVisualsTabInner(0f, 0f - _scrollPosition.y);
	}

	private void DrawMonsterSpawnerTab()
	{
		_lastContentHeight = DrawMonsterSpawnerTabInner(0f, 0f - _scrollPosition.y);
	}

	private void DrawItemSpawnerTab()
	{
		_lastContentHeight = DrawItemSpawnerTabInner(0f, 0f - _scrollPosition.y);
	}

	private float DrawMonsterSpawnerTabInner(float x, float y)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		float num = 250f;
		float num2 = x + num + 20f;
		float num3 = num2 + 276f + 20f;
		float num4 = y;
		Render.DrawString(new Rect(x, num4, num, 30f), "怪物列表", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		if ((UnityEngine.Object)(object)MonsterSpawner.Instance != (UnityEngine.Object)null)
		{
			List<EnemySetup> spawnableEnemies = MonsterSpawner.Instance.SpawnableEnemies;
			for (int i = 0; i < spawnableEnemies.Count; i++)
			{
				EnemySetup val = spawnableEnemies[i];
				string text = ((UnityEngine.Object)val).name.Replace("Enemy - ", "");
				if (Elements.Button(((UnityEngine.Object)(object)MonsterSpawner.Instance.SelectedEnemy == (UnityEngine.Object)(object)val) ? ("> " + text + " <") : text, x, num4, num))
				{
					MonsterSpawner.Instance.SelectEnemy(val);
				}
				num4 += 40f;
			}
		}
		float num5 = y;
		Render.DrawString(new Rect(num2, num5, 256f, 30f), "预览", Theme.Accent, center: false, 16, bold: true);
		num5 += 40f;
		if ((UnityEngine.Object)(object)MonsterSpawner.Instance != (UnityEngine.Object)null && (UnityEngine.Object)(object)MonsterSpawner.Instance.PreviewTexture != (UnityEngine.Object)null)
		{
			GUI.Box(new Rect(num2, num5, 256f, 256f), "");
			GUI.DrawTexture(new Rect(num2, num5, 256f, 256f), (Texture)(object)MonsterSpawner.Instance.PreviewTexture, (ScaleMode)2);
			num5 += 266f;
			if (Elements.Button("生成 (仅主机)", num2, num5, 256f))
			{
				MonsterSpawner.Instance.SpawnSelectedEnemy();
			}
			num5 += 40f;
		}
		float num6 = y;
		Render.DrawString(new Rect(num3, num6, num, 30f), "活跃的敌人", Theme.Accent, center: false, 16, bold: true);
		num6 += 40f;
		if ((UnityEngine.Object)(object)MonsterSpawner.Instance != (UnityEngine.Object)null)
		{
			List<Enemy> activeEnemies = MonsterSpawner.Instance.ActiveEnemies;
			if (activeEnemies.Count == 0)
			{
				Render.DrawString(new Rect(num3, num6, num, 30f), "未发现活跃敌人.", Color.gray);
				num6 += 30f;
			}
			else
			{
				for (int j = 0; j < activeEnemies.Count; j++)
				{
					Enemy val2 = activeEnemies[j];
					if ((UnityEngine.Object)(object)val2 == (UnityEngine.Object)null)
					{
						continue;
					}
					string text2 = ((UnityEngine.Object)val2).name.Replace("(Clone)", "");
					EnemyHealth component = ((Component)val2).GetComponent<EnemyHealth>();
					string text3 = "Invuln";
					if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
					{
						try
						{
							FieldInfo field = typeof(EnemyHealth).GetField("healthCurrent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							if (field != null)
							{
								text3 = $"{field.GetValue(component)} HP";
							}
						}
						catch
						{
						}
					}
					Render.DrawString(new Rect(num3, num6, num, 20f), text2 + " [" + text3 + "]", Color.white);
					num6 += 25f;
					if (Elements.Button("杀死", num3, num6, 70f))
					{
						MonsterSpawner.Instance.KillEnemy(val2);
					}
					if (Elements.Button("TP 到", num3 + 75f, num6, 70f))
					{
						MonsterSpawner.Instance.TeleportToEnemy(val2);
					}
					num6 += 35f;
				}
			}
		}
		return Mathf.Max(num4, Mathf.Max(num5, num6)) - y + _scrollPosition.y;
	}

	private float DrawItemSpawnerTabInner(float x, float y)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		float num = 250f;
		float num2 = x + num + 20f;
		float num3 = y;
		Render.DrawString(new Rect(x, num3, num, 30f), "物品生成", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if ((UnityEngine.Object)(object)ItemSpawner.Instance != (UnityEngine.Object)null)
		{
			string text = ItemSpawner.Instance.GetSearchQuery();
			if (Elements.TextField("搜索物品", ref text, x, num3, num))
			{
				ItemSpawner.Instance.UpdateSearch(text);
			}
			num3 += 40f;
			List<ItemSpawner.SpawnableItemDef> filteredItems = ItemSpawner.Instance.GetFilteredItems();
			if (filteredItems.Count == 0)
			{
				Render.DrawString(new Rect(x, num3, num, 30f), "未找到任何物品.", Color.gray);
				num3 += 30f;
			}
			else
			{
				for (int i = 0; i < filteredItems.Count; i++)
				{
					ItemSpawner.SpawnableItemDef spawnableItemDef = filteredItems[i];
					if (spawnableItemDef != null)
					{
						string name = spawnableItemDef.Name;
						if (Elements.Button((ItemSpawner.Instance.SelectedItem == spawnableItemDef) ? ("> " + name + " <") : name, x, num3, num))
						{
							ItemSpawner.Instance.SelectItem(spawnableItemDef);
						}
						num3 += 40f;
					}
				}
			}
		}
		float num4 = y;
		Render.DrawString(new Rect(num2, num4, num, 30f), "已选物品", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		if ((UnityEngine.Object)(object)ItemSpawner.Instance != (UnityEngine.Object)null && ItemSpawner.Instance.SelectedItem != null)
		{
			ItemSpawner.SpawnableItemDef selectedItem = ItemSpawner.Instance.SelectedItem;
			string name2 = selectedItem.Name;
			Render.DrawString(new Rect(num2, num4, num, 30f), name2, Color.white);
			num4 += 30f;
			string text2 = "价值: 随机 (生成于出生点)";
			Render.DrawString(new Rect(num2, num4, num, 40f), text2, Color.gray, center: false, 12);
			num4 += 40f;
			if (Elements.Button("生成物品", num2, num4, num))
			{
				ItemSpawner.Instance.SpawnSelectedItem();
			}
			num4 += 40f;
		}
		else
		{
			Render.DrawString(new Rect(num2, num4, num, 30f), "选择物品查看详情.", Color.gray);
			num4 += 30f;
		}
		num4 += 20f;
		Render.DrawString(new Rect(num2, num4, num, 30f), "生成代币箱子", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		if (Elements.Button("生成普通代币箱 (Common)", num2, num4, num))
		{
			SpawnCosmeticBox(SemiFunc.Rarity.Common);
		}
		num4 += 40f;
		if (Elements.Button("生成罕见代币箱 (Uncommon)", num2, num4, num))
		{
			SpawnCosmeticBox(SemiFunc.Rarity.Uncommon);
		}
		num4 += 40f;
		if (Elements.Button("生成稀有代币箱 (Rare)", num2, num4, num))
		{
			SpawnCosmeticBox(SemiFunc.Rarity.Rare);
		}
		num4 += 40f;
		if (Elements.Button("生成超稀有代币箱 (UltraRare)", num2, num4, num))
		{
			SpawnCosmeticBox(SemiFunc.Rarity.UltraRare);
		}
		num4 += 40f;
		return Mathf.Max(num3, num4) - y + _scrollPosition.y;
	}

	private void SpawnCosmeticBox(SemiFunc.Rarity rarity)
	{
		if (ValuableDirector.instance != null && ValuableDirector.instance.cosmeticWorldObjectSetups != null)
		{
			foreach (var setup in ValuableDirector.instance.cosmeticWorldObjectSetups)
			{
				if (setup.rarity == rarity && setup.prefab != null && setup.prefab.Prefab != null)
				{
					if (ItemSpawner.Instance != null)
					{
						ItemSpawner.SpawnableItemDef def = new ItemSpawner.SpawnableItemDef
						{
							Name = "Cosmetic Box " + rarity.ToString(),
							PrefabGetter = () => setup.prefab.Prefab
						};
						ItemSpawner.Instance.SpawnItem(def);
					}
					return;
				}
			}
		}
	}

	private float DrawMonsterVisualsTabInner(float x, float y)
	{
		return DrawEnemiesTab(x, y);
	}

	private void DrawPlayerVisualsTab()
	{
		_lastContentHeight = DrawPlayerEspTab(0f, 0f - _scrollPosition.y);
	}

	private void DrawVisualsMiscTab()
	{
		_lastContentHeight = DrawVisualsMiscTabInner(0f, 0f - _scrollPosition.y);
	}

	private void DrawLocalTab()
	{
		_lastContentHeight = DrawLocalTabInner(0f, 0f - _scrollPosition.y);
	}

	private void DrawMiscTab()
	{
		_lastContentHeight = DrawMiscTabInner(0f, 0f - _scrollPosition.y);
	}

	private void DrawSettingsTab()
	{
		_lastContentHeight = DrawSettingsTabInner(0f, 0f - _scrollPosition.y);
	}

	private float DrawPlayerEspTab(float x, float y)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		float num = 390f;
		float num2 = x + num + 20f;
		float num3 = y;
		Render.DrawString(new Rect(x, num3, num, 30f), "玩家透视", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Toggle("启用玩家透视", ref ConfigManager.Config.PlayerEsp.Enabled, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示名称", ref ConfigManager.Config.PlayerEsp.DrawName, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示血量", ref ConfigManager.Config.PlayerEsp.DrawHealth, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示距离", ref ConfigManager.Config.PlayerEsp.DrawDistance, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示已持有物品", ref ConfigManager.Config.PlayerEsp.DrawHeldItem, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		float num4 = y;
		Render.DrawString(new Rect(num2, num4, num, 30f), "小地图", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		if (Elements.Toggle("启用小地图", ref ConfigManager.Config.Minimap.Enabled, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("显示图标", ref ConfigManager.Config.Minimap.ShowIcons, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("自动居中", ref ConfigManager.Config.Minimap.AutoCenter, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("显示路径", ref ConfigManager.Config.Minimap.ShowPath, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Slider("Zoom", ref ConfigManager.Config.Minimap.Zoom, 0.1f, 5f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Slider("大小", ref ConfigManager.Config.Minimap.Size, 100f, 800f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Button("重置位置", num2, num4, num))
		{
			ConfigManager.Config.Minimap.Position = new Vector2(-1f, -1f);
			MarkDirty();
		}
		num4 += 50f;
		Render.DrawString(new Rect(num2, num4, num, 30f), "激光瞄准器", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		if (Elements.Toggle("启用激光", ref ConfigManager.Config.LaserSight.Enabled, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("显示本地", ref ConfigManager.Config.LaserSight.ShowLocal, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("显示其他人", ref ConfigManager.Config.LaserSight.ShowOthers, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("显示命中信息", ref ConfigManager.Config.LaserSight.ShowHitInfo, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Slider("宽度", ref ConfigManager.Config.LaserSight.Width, 0.01f, 0.1f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.ColorPicker("激光颜色", ref ConfigManager.Config.LaserSight.Color, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		return Mathf.Max(num3, num4) - y + _scrollPosition.y;
	}

	private float DrawVisualsMiscTabInner(float x, float y)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		float num = 390f;
		float num2 = x + num + 20f;
		float num3 = y;
		Render.DrawString(new Rect(x, num3, num, 30f), "罗盘", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Toggle("启用罗盘", ref ConfigManager.Config.Compass.Enabled, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("大小", ref ConfigManager.Config.Compass.Size, 100f, 500f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("范围", ref ConfigManager.Config.Compass.Range, 10f, 100f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("规模", ref ConfigManager.Config.Compass.Scale, 0.5f, 2f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		float num4 = y;
		Render.DrawString(new Rect(num2, num4, num, 30f), "相机", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		if (Elements.Toggle("十字准星", ref ConfigManager.Config.Misc.Crosshair, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("显示FPS", ref ConfigManager.Config.Misc.ShowFps, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("显示快捷键", ref ConfigManager.Config.Misc.ShowKeybinds, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Slider("FOV", ref ConfigManager.Config.Misc.FOV, 50f, 120f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("光亮", ref ConfigManager.Config.Misc.Fullbright, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Toggle("无雾", ref ConfigManager.Config.Misc.NoFog, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Slider("强度", ref ConfigManager.Config.Misc.FullbrightIntensity, 0.1f, 2f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 50f;
		return Mathf.Max(num3, num4) - y + _scrollPosition.y;
	}

	private float DrawLootTab(float x, float y)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		float num = 390f;
		float num2 = x + num + 20f;
		float num3 = y;
		Render.DrawString(new Rect(x, num3, num, 30f), "战利品透视", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Toggle("启用战利品透视", ref ConfigManager.Config.Loot.Enabled, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示名称", ref ConfigManager.Config.Loot.DrawName, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示线", ref ConfigManager.Config.Loot.DrawTracers, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("使用聚类", ref ConfigManager.Config.Loot.UseClustering, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("动态透明度", ref ConfigManager.Config.Loot.DynamicOpacity, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示购物车界面", ref ConfigManager.Config.Loot.ShowCartUI, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 50f;
		Render.DrawString(new Rect(x, num3, num, 30f), "发光", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Toggle("近距轮廓", ref ConfigManager.Config.Loot.HighlightEnabled, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("轮廓距离", ref ConfigManager.Config.Loot.HighlightDistance, 1f, 20f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		float num4 = y;
		Render.DrawString(new Rect(num2, num4, num, 30f), "过滤器", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		if (Elements.Slider("最大距离", ref ConfigManager.Config.Loot.MaxDistance, 0f, 1000f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		int value = ConfigManager.Config.Loot.MinValue;
		if (Elements.Slider("最小价值", ref value, 0, 1000, num2, num4, num))
		{
			ConfigManager.Config.Loot.MinValue = value;
			MarkDirty();
		}
		num4 += 50f;
		return Mathf.Max(num3, num4) - y + _scrollPosition.y;
	}

	private float DrawEnemiesTab(float x, float y)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Invalid comparison between Unknown and I4
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		float num = 390f;
		float num2 = x + num + 20f;
		float num3 = y;
		Render.DrawString(new Rect(x, num3, num, 30f), "透视设置", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Toggle("启用透视", ref ConfigManager.Config.Enemies.EspEnabled, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示方框", ref ConfigManager.Config.Enemies.DrawBox, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示血量", ref ConfigManager.Config.Enemies.DrawHealth, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示信息", ref ConfigManager.Config.Enemies.DrawInfo, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示状态", ref ConfigManager.Config.Enemies.DrawStatus, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("显示路径", ref ConfigManager.Config.Enemies.DrawPath, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("目标警告", ref ConfigManager.Config.Enemies.TargetWarning, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 50f;
		Render.DrawString(new Rect(x, num3, num, 30f), "高级", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Slider("最大距离", ref ConfigManager.Config.Enemies.MaxDistance, 0f, 1000f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		bool value = ConfigManager.Config.Enemies.HighlightEnabled;
		if (Elements.Toggle("敌人上色", ref value, x, num3, num))
		{
			EnemyChams.Instance.ToggleChams(value);
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.ColorPicker("上色颜色", ref ConfigManager.Config.Enemies.HighlightColor, x, num3, num))
		{
			EnemyChams.Instance.Refresh();
			MarkDirty();
		}
		num3 += 40f;
		float value2 = ConfigManager.Config.Enemies.HighlightColor.a;
		if (Elements.Slider("上色透明度", ref value2, 0f, 1f, x, num3, num))
		{
			Color highlightColor = ConfigManager.Config.Enemies.HighlightColor;
			highlightColor.a = value2;
			ConfigManager.Config.Enemies.HighlightColor = highlightColor;
			EnemyChams.Instance.Refresh();
			MarkDirty();
		}
		num3 += 50f;
		float num4 = y;
		Render.DrawString(new Rect(num2, num4, num, 30f), "预览", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		RenderTexture previewTexture = EnemyPreview.GetPreviewTexture();
		if ((UnityEngine.Object)(object)previewTexture != (UnityEngine.Object)null)
		{
			GUI.Box(new Rect(num2, num4, 256f, 256f), "");
			GUI.DrawTexture(new Rect(num2, num4, 256f, 256f), (Texture)(object)previewTexture, (ScaleMode)2);
			if (!EnemyPreview.HasTarget || ConfigManager.Config.Enemies.EspEnabled)
			{
			}
			Rect val = default(Rect);
			val = new Rect(num2, num4, 256f, 256f);
			if (val.Contains(Event.current.mousePosition) && (int)Event.current.type == 3)
			{
				EnemyPreview.HandleMouseInput(Event.current.delta.x);
				Event.current.Use();
			}
		}
		num4 += 266f;
		if (Elements.Button("捕捉最近的敌人", num2, num4, num))
		{
			EnemyParent nearestEnemy = GetNearestEnemy();
			if ((UnityEngine.Object)(object)nearestEnemy != (UnityEngine.Object)null)
			{
				EnemyPreview.SetPreviewTarget(nearestEnemy);
			}
		}
		num4 += 40f;
		return Mathf.Max(num3, num4) - y + _scrollPosition.y;
	}

	private float DrawLocalTabInner(float x, float y)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		float num = 390f;
		float num2 = x + num + 20f;
		float num3 = y;
		Render.DrawString(new Rect(x, num3, num, 30f), "仅限客户端 (INVISIBLE)", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Toggle("无敌模式", ref ConfigManager.Config.Local.GodMode, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("无限耐力", ref ConfigManager.Config.Local.InfiniteStamina, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("无限电池", ref ConfigManager.Config.Local.InfiniteBattery, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("抓取范围", ref ConfigManager.Config.Local.GrabRange, 1f, 5f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("抓握力", ref ConfigManager.Config.Local.GrabStrength, 1f, 5f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 50f;
		Render.DrawString(new Rect(x, num3, num, 30f), "同步 (他人可见)", Theme.StatusWarning, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Toggle("穿墙模式 [!]", ref ConfigManager.Config.Local.NoClip, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("穿墙速度", ref ConfigManager.Config.Local.NoClipSpeed, 1f, 30f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Toggle("禁用布娃娃效果 [!]", ref ConfigManager.Config.Local.NoRagdoll, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("奔跑速度 [!]", ref ConfigManager.Config.Local.RunSpeed, 1f, 5f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("跳跃力量 [!]", ref ConfigManager.Config.Local.JumpForce, 1f, 5f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Slider("重力 [!]", ref ConfigManager.Config.Local.Gravity, 0.1f, 2f, x, num3, num))
		{
			MarkDirty();
		}
		num3 += 50f;
		float num4 = y;
		Render.DrawString(new Rect(num2, num4, num, 30f), "自由视角 (仅限客户端)", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		bool freeCamEnabled = FreeCam.Enabled;
		if (Elements.Toggle("启用灵魂出窍", ref freeCamEnabled, num2, num4, num))
		{
			FreeCam.Toggle();
		}
		num4 += 40f;
		if (Elements.Slider("速度", ref ConfigManager.Config.Local.FreeCamSpeed, 1f, 50f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Slider("快速", ref ConfigManager.Config.Local.FreeCamFastMultiplier, 1f, 10f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 40f;
		if (Elements.Slider("敏感性", ref ConfigManager.Config.Local.FreeCamSensitivity, 0.5f, 5f, num2, num4, num))
		{
			MarkDirty();
		}
		num4 += 50f;
		return Mathf.Max(num3, num4) - y + _scrollPosition.y;
	}

	private float DrawMiscTabInner(float x, float y)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		float num = 390f;
		float num2 = x + num + 20f;
		float num3 = y;
		Render.DrawString(new Rect(x, num3, num, 30f), "仅限主机", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		int value = ConfigManager.Config.Misc.SetItemValue;
		if (Elements.Slider("物品价值 ($)", ref value, 100, 50000, x, num3, num))
		{
			ConfigManager.Config.Misc.SetItemValue = value;
			MarkDirty();
		}
		num3 += 40f;
		if (Elements.Button("设置持有物品价值", x, num3, num))
		{
			SetHeldItemValue(ConfigManager.Config.Misc.SetItemValue);
		}
		num3 += 40f;
		if (Elements.Button("消灭所有敌人", x, num3, num))
		{
			KillAllEnemies();
		}
		num3 += 50f;
		Render.DrawString(new Rect(x, num3, num, 30f), "代币修改", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Button("添加普通代币 (Common)", x, num3, num))
		{
			if (MetaManager.instance != null) MetaManager.instance.CosmeticTokenAdd(SemiFunc.Rarity.Common);
		}
		num3 += 40f;
		if (Elements.Button("添加罕见代币 (Uncommon)", x, num3, num))
		{
			if (MetaManager.instance != null) MetaManager.instance.CosmeticTokenAdd(SemiFunc.Rarity.Uncommon);
		}
		num3 += 40f;
		if (Elements.Button("添加稀有代币 (Rare)", x, num3, num))
		{
			if (MetaManager.instance != null) MetaManager.instance.CosmeticTokenAdd(SemiFunc.Rarity.Rare);
		}
		num3 += 40f;
		if (Elements.Button("添加超稀有代币 (UltraRare)", x, num3, num))
		{
			if (MetaManager.instance != null) MetaManager.instance.CosmeticTokenAdd(SemiFunc.Rarity.UltraRare);
		}
		num3 += 40f;
		return num3 - y + _scrollPosition.y;
	}

	private float DrawSettingsTabInner(float x, float y)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		float num = 390f;
		float num2 = x + num + 20f;
		float num3 = y;
		Render.DrawString(new Rect(x, num3, num, 30f), "配置管理器", Theme.Accent, center: false, 16, bold: true);
		num3 += 40f;
		if (Elements.Button("保存配置", x, num3, num))
		{
			ConfigManager.SaveConfig(ConfigManager.GetCurrentProfile());
		}
		num3 += 50f;
		float num4 = y;
		Render.DrawString(new Rect(num2, num4, num, 30f), "创建个人资料", Theme.Accent, center: false, 16, bold: true);
		num4 += 40f;
		if (Elements.TextField("个人资料名称", ref _newProfileName, num2, num4, num))
		{
		}
		num4 += 40f;
		if (Elements.Button("创建 & 开关", num2, num4, num) && !string.IsNullOrEmpty(_newProfileName))
		{
			ConfigManager.SaveConfig(_newProfileName);
		}
		num4 += 40f;
		return Mathf.Max(num3, num4) - y + _scrollPosition.y;
	}

	public void ForceClose()
	{
		if (FreeCam.Enabled)
		{
			FreeCam.Toggle();
		}
		if (_visible)
		{
			_visible = false;
			UpdateMenuVisibility();
			Cursor.lockState = (CursorLockMode)1;
			Cursor.visible = false;
			RestoreGameValues();
			ToggleInputActions(enable: true);
			SetInputManagerDisabled(disabled: false);
			CursorManager val = UnityEngine.Object.FindObjectOfType<CursorManager>();
			if ((UnityEngine.Object)(object)val != (UnityEngine.Object)null)
			{
				val.Unlock(0f);
			}
			if ((UnityEngine.Object)(object)MenuCursor.instance != (UnityEngine.Object)null)
			{
				((Component)MenuCursor.instance).gameObject.SetActive(true);
			}
		}
	}

	private void MarkDirty()
	{
		_configDirty = true;
		_saveTimer = 2f;
	}

	private void SetInputManagerDisabled(bool disabled)
	{
		if ((UnityEngine.Object)(object)InputManager.instance == (UnityEngine.Object)null)
		{
			return;
		}
		FieldInfo field = typeof(InputManager).GetField("disableAimingTimer", BindingFlags.Instance | BindingFlags.NonPublic);
		FieldInfo field2 = typeof(InputManager).GetField("disableMovementTimer", BindingFlags.Instance | BindingFlags.NonPublic);
		if (disabled)
		{
			if (field != null)
			{
				field.SetValue(InputManager.instance, 999f);
			}
			if (field2 != null)
			{
				field2.SetValue(InputManager.instance, 999f);
			}
		}
		else
		{
			if (field != null)
			{
				field.SetValue(InputManager.instance, 0f);
			}
			if (field2 != null)
			{
				field2.SetValue(InputManager.instance, 0f);
			}
		}
	}

	private void ToggleInputActions(bool enable)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((UnityEngine.Object)(object)InputManager.instance == (UnityEngine.Object)null)
		{
			return;
		}
		foreach (InputKey value in Enum.GetValues(typeof(InputKey)))
		{
			InputAction action = InputManager.instance.GetAction(value);
			if (action != null)
			{
				if (enable)
				{
					action.Enable();
				}
				else
				{
					action.Disable();
				}
			}
		}
	}

	private void StoreGameValues()
	{
		if ((UnityEngine.Object)(object)CameraAim.Instance != (UnityEngine.Object)null)
		{
			_originalAimSpeedMouse = CameraAim.Instance.AimSpeedMouse;
			_originalAimSpeedGamepad = CameraAim.Instance.AimSpeedGamepad;
		}
	}

	private void RestoreGameValues()
	{
		if ((UnityEngine.Object)(object)CameraAim.Instance != (UnityEngine.Object)null)
		{
			CameraAim.Instance.AimSpeedMouse = _originalAimSpeedMouse;
			CameraAim.Instance.AimSpeedGamepad = _originalAimSpeedGamepad;
		}
	}

	private void HandleHotkeys()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		if ((int)ConfigManager.Config.Loot.ToggleKey != 0 && Input.GetKeyDown(ConfigManager.Config.Loot.ToggleKey))
		{
			ConfigManager.Config.Loot.Enabled = !ConfigManager.Config.Loot.Enabled;
			MarkDirty();
		}
		if ((int)ConfigManager.Config.Enemies.ToggleKey != 0 && Input.GetKeyDown(ConfigManager.Config.Enemies.ToggleKey))
		{
			ConfigManager.Config.Enemies.EspEnabled = !ConfigManager.Config.Enemies.EspEnabled;
			MarkDirty();
		}
		if ((int)ConfigManager.Config.Minimap.ToggleKey != 0 && Input.GetKeyDown(ConfigManager.Config.Minimap.ToggleKey))
		{
			ConfigManager.Config.Minimap.Enabled = !ConfigManager.Config.Minimap.Enabled;
			MarkDirty();
		}
		if ((int)ConfigManager.Config.Minimap.ToggleRenderModeKey != 0 && Input.GetKeyDown(ConfigManager.Config.Minimap.ToggleRenderModeKey))
		{
			ConfigManager.Config.Minimap.RenderMode = (ConfigManager.Config.Minimap.RenderMode + 1) % 3;
			MarkDirty();
		}
		if ((int)ConfigManager.Config.Local.GodModeKey != 0 && Input.GetKeyDown(ConfigManager.Config.Local.GodModeKey))
		{
			ConfigManager.Config.Local.GodMode = !ConfigManager.Config.Local.GodMode;
			MarkDirty();
		}
		if ((int)ConfigManager.Config.Local.NoClipKey != 0 && Input.GetKeyDown(ConfigManager.Config.Local.NoClipKey))
		{
			ConfigManager.Config.Local.NoClip = !ConfigManager.Config.Local.NoClip;
			MarkDirty();
		}
	}

	private bool IsHost()
	{
		try
		{
			if ((UnityEngine.Object)(object)GameManager.instance != (UnityEngine.Object)null && GameManager.instance.gameMode == 0)
			{
				return true;
			}
			Type type = Type.GetType("Photon.Pun.PhotonNetwork, PhotonUnityNetworking");
			if (type != null)
			{
				PropertyInfo property = type.GetProperty("IsMasterClient", BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					return (bool)property.GetValue(null);
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private void SetHeldItemValue(int value)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!IsHost())
		{
			return;
		}
		try
		{
			if ((UnityEngine.Object)(object)PhysGrabber.instance == (UnityEngine.Object)null)
			{
				return;
			}
			FieldInfo field = typeof(PhysGrabber).GetField("grabbedPhysGrabObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PhysGrabObject val = ((!(field != null)) ? ((PhysGrabObject)null) : ((PhysGrabObject)field.GetValue(PhysGrabber.instance)));
			if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
			{
				return;
			}
			ValuableObject component = ((Component)val).GetComponent<ValuableObject>();
			if (!((UnityEngine.Object)(object)component == (UnityEngine.Object)null))
			{
				FieldInfo field2 = typeof(ValuableObject).GetField("dollarValueCurrent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field2 != null)
				{
					field2.SetValue(component, (float)value);
				}
			}
		}
		catch
		{
		}
	}

	private void KillAllEnemies()
	{
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (!IsHost())
		{
			return;
		}
		try
		{
			if ((UnityEngine.Object)(object)EnemyDirector.instance == (UnityEngine.Object)null)
			{
				return;
			}
			foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
			{
				if ((UnityEngine.Object)(object)item == (UnityEngine.Object)null)
				{
					continue;
				}
				FieldInfo field = typeof(EnemyParent).GetField("Enemy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null)
				{
					continue;
				}
				object value = field.GetValue(item);
				Enemy val = (Enemy)((value is Enemy) ? value : null);
				if ((UnityEngine.Object)(object)val == (UnityEngine.Object)null)
				{
					continue;
				}
				EnemyHealth component = ((Component)val).GetComponent<EnemyHealth>();
				if (!((UnityEngine.Object)(object)component == (UnityEngine.Object)null))
				{
					FieldInfo field2 = typeof(EnemyHealth).GetField("dead", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (!(field2 != null) || !(bool)field2.GetValue(component))
					{
						component.Hurt(9999, Vector3.down);
					}
				}
			}
		}
		catch
		{
		}
	}

	private EnemyParent GetNearestEnemy()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if ((UnityEngine.Object)(object)EnemyDirector.instance == (UnityEngine.Object)null)
		{
			return null;
		}
		if ((UnityEngine.Object)(object)LocalPlayerManager.LocalPlayer == (UnityEngine.Object)null)
		{
			return null;
		}
		EnemyParent result = null;
		float num = float.MaxValue;
		Vector3 position = ((Component)LocalPlayerManager.LocalPlayer).transform.position;
		foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
		{
			if (!((UnityEngine.Object)(object)item == (UnityEngine.Object)null))
			{
				float num2 = Vector3.Distance(position, ((Component)item).transform.position);
				if (num2 < num)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	private void LateUpdate()
	{
		if (_visible)
		{
			Cursor.lockState = (CursorLockMode)0;
			Cursor.visible = false;
		}
	}
}
