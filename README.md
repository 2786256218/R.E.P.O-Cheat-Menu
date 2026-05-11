# R.E.P.O-Cheat-Menu

> Welcome to **R.E.P.O-Cheat-Menu**.

**R.E.P.O-Cheat-Menu** is a .NET Framework based Unity / Mono DLL menu for **R.E.P.O**.  
You can build the source code with `dotnet build`, or download the prebuilt release package and inject `Cheat.dll` with SharpMonoInjector.

---

## Language / 语言

- [中文说明](#中文说明)
- [English Guide](#english-guide)

---

## 中文说明

### 欢迎使用

欢迎使用 **R.E.P.O-Cheat-Menu**。

本项目是一个面向 **R.E.P.O** 的 Unity / Mono DLL 菜单项目。项目以 C# 编写，目标框架为 `.NET Framework 4.8`，构建产物为 `Cheat.dll`。你可以使用 **.NET Framework** 构建源代码，也可以直接下载我们已经构建好的发行版包。

### 技术栈

| 类型 | 内容 |
| --- | --- |
| 编程语言 | C# |
| 目标框架 | .NET Framework 4.8 / `net48` |
| 构建工具 | .NET SDK / `dotnet build` |
| 游戏运行环境 | Unity / Mono |
| UI 技术 | Unity IMGUI / `OnGUI` |
| 渲染能力 | Unity `GUI`、`GL`、`LineRenderer`、`RenderTexture`、自定义材质 |
| 配置系统 | JSON 配置档案，支持多 Profile |
| 注入方式 | SharpMonoInjector |
| 入口方法 | `Cheat.Loader.Init()` |

### 项目结构

```text
Cheat_Decompiled
├─ Cheat.csproj                 # C# 项目文件，目标框架 net48
├─ Cheat
│  ├─ Loader.cs                 # 注入入口，负责初始化与卸载
│  ├─ CheatMenu.cs              # 主菜单、热键、功能开关与 UI 调度
│  ├─ Config                    # 配置数据与 Profile 管理
│  ├─ Features                  # 功能模块
│  │  ├─ Compass                # 罗盘
│  │  ├─ Enemies                # 怪物 ESP、路径、Chams、预览
│  │  ├─ ItemSpawner            # 物品生成
│  │  ├─ LocalPlayer            # 本地玩家能力与 FreeCam
│  │  ├─ Loot                   # 战利品 ESP、聚类、购物车 UI、Chams
│  │  ├─ Minimap                # 小地图与路径显示
│  │  ├─ MonsterSpawner         # 怪物生成
│  │  └─ Visuals                # 激光、光照、雾效等视觉功能
│  ├─ UI                        # 菜单控件、主题、绘制工具、侧边栏
│  └─ Utils                     # 日志、数学、Shader、反射工具
├─ Libs                         # Unity 与游戏依赖库
└─ bin                          # 构建输出目录
```

### 核心功能

#### 菜单系统

- 使用 Unity `MonoBehaviour` 与 `OnGUI` 绘制运行时菜单。
- 支持侧边栏分组、分页、按钮、滑块、开关、颜色选择器和文本输入。
- 菜单打开时会处理光标、输入锁定和相机控制，减少与游戏原始输入的冲突。
- 支持快捷键切换功能，并提供快捷键显示界面。

#### 视觉与 ESP

- 玩家 ESP：名称、血量、距离、手持物品显示。
- 怪物 ESP：方框、血量、状态、信息、路径、目标警告。
- 战利品 ESP：名称、连线、距离过滤、价值过滤、动态透明度。
- Chams 高亮：支持怪物和战利品轮廓 / 着色显示。
- 激光辅助显示：本地玩家与其他玩家激光线、命中点和命中信息。

#### 地图与导航

- 小地图：独立 Minimap Camera、`RenderTexture` 渲染、图标显示、缩放和拖拽。
- 小地图路径：可显示怪物路径或相关移动轨迹。
- 罗盘：可配置尺寸、范围、缩放和位置偏移。

#### 生成器

- 怪物生成：读取可生成怪物列表，支持选择、预览、生成、击杀和传送到目标。
- 物品生成：支持搜索、选择、生成，并可设置物品价值。

#### 本地玩家功能

- 无敌模式。
- 无限耐力。
- 无限电池。
- 抓取范围、抓取强度、跳跃力度、重力、移动速度等参数调整。
- NoClip 穿墙模式。
- 禁用布娃娃效果。
- FreeCam 自由相机，支持速度、快速倍率和灵敏度设置。

#### 其他功能

- 十字准星。
- FPS 显示。
- Fullbright 光亮模式。
- NoFog 无雾模式。
- RPC / 装饰物相关修复模块。
- 控制台日志与 Unity 日志回调。

### 技术实现概览

项目通过 SharpMonoInjector 调用 `Cheat.Loader.Init()` 作为入口。初始化后，Loader 会创建一个持久化 `GameObject`，并挂载核心组件：

```text
CheatMenu
Minimap
Compass
RPCFixManager
```

`CheatMenu` 负责主循环中的功能更新、热键处理、菜单绘制和配置保存。ESP、Chams、小地图、FreeCam、激光等功能被拆分到独立模块中，便于维护和扩展。

配置系统由 `ConfigManager` 和 `ConfigData` 管理，支持将不同功能设置保存为 Profile。菜单修改配置后会延迟保存，减少频繁写入。

### 项目构建

你可以直接使用 `dotnet build` 构建本项目：

```powershell
dotnet build
```

如果你希望生成 Release 版本，可以使用：

```powershell
dotnet build -c Release
```

构建完成后，生成物将会是：

```text
Cheat.dll
```

常见输出路径：

```text
bin\Debug\net48\Cheat.dll
bin\Release\net48\Cheat.dll
```

### 使用发行版

如果你不想自行构建，也可以直接下载我们构建好的发行版包。

下载后请确认包内包含：

```text
Cheat.dll
```

### 注入准备

你需要提前准备好：

```text
SharpMonoInjector
```

请先启动游戏，然后使用 SharpMonoInjector 将 `Cheat.dll` 注入到游戏进程中。

### SharpMonoInjector 参数

手动填写参数时，请使用以下入口信息：

```text
class name: Loader
namespace: Cheat
method name: Init
```

### CLI 注入命令

如果你使用 SharpMonoInjector 的 CLI 版本，可以执行：

```powershell
smi.exe inject -p "REPO" -a ".\Cheat.dll" -n Cheat -c Loader -m Init
```

### 构建与注入流程

```text
1. 克隆或下载本项目
2. 使用 dotnet build 构建源代码
3. 在输出目录中找到 Cheat.dll
4. 启动 R.E.P.O
5. 使用 SharpMonoInjector 注入 Cheat.dll
6. 入口参数填写 Cheat / Loader / Init
```

### 注意事项

- 本项目目标框架为 `.NET Framework 4.8` / `net48`。
- 如果构建失败，请确认已经安装 .NET SDK 以及 .NET Framework 4.8 Developer Pack。
- 请确保 `Libs` 目录中的 Unity 与游戏依赖库路径正确。
- 请确保 `Cheat.dll` 与注入工具路径填写正确。
- 请自行承担使用第三方注入工具和游戏修改内容带来的风险。

---

## English Guide

### Welcome

Welcome to **R.E.P.O-Cheat-Menu**.

This project is a Unity / Mono DLL menu for **R.E.P.O**. It is written in C#, targets `.NET Framework 4.8`, and builds into `Cheat.dll`. You can build the source code with **.NET Framework**, or download the prebuilt release package directly.

### Tech Stack

| Category | Details |
| --- | --- |
| Language | C# |
| Target framework | .NET Framework 4.8 / `net48` |
| Build tool | .NET SDK / `dotnet build` |
| Runtime | Unity / Mono |
| UI | Unity IMGUI / `OnGUI` |
| Rendering | Unity `GUI`, `GL`, `LineRenderer`, `RenderTexture`, custom materials |
| Configuration | JSON profiles with multi-profile support |
| Injection tool | SharpMonoInjector |
| Entry point | `Cheat.Loader.Init()` |

### Project Structure

```text
Cheat_Decompiled
├─ Cheat.csproj                 # C# project file targeting net48
├─ Cheat
│  ├─ Loader.cs                 # Injection entry point and lifecycle control
│  ├─ CheatMenu.cs              # Main menu, hotkeys, feature toggles, UI flow
│  ├─ Config                    # Config data and profile management
│  ├─ Features                  # Feature modules
│  │  ├─ Compass                # Compass overlay
│  │  ├─ Enemies                # Enemy ESP, path rendering, Chams, preview
│  │  ├─ ItemSpawner            # Item spawner
│  │  ├─ LocalPlayer            # Local player features and FreeCam
│  │  ├─ Loot                   # Loot ESP, clustering, cart UI, Chams
│  │  ├─ Minimap                # Minimap and path display
│  │  ├─ MonsterSpawner         # Monster spawner
│  │  └─ Visuals                # Laser sight, lighting, fog features
│  ├─ UI                        # Controls, theme, renderer, sidebar
│  └─ Utils                     # Logger, math helpers, shaders, reflection helpers
├─ Libs                         # Unity and game dependency libraries
└─ bin                          # Build output
```

### Main Features

#### Menu System

- Runtime menu rendered with Unity `MonoBehaviour` and `OnGUI`.
- Sidebar sections, tabs, buttons, sliders, toggles, color pickers, and text fields.
- Cursor, input, and camera handling while the menu is open.
- Hotkey support with an optional keybind display overlay.

#### Visuals And ESP

- Player ESP: name, health, distance, and held item display.
- Enemy ESP: boxes, health, status, info, path rendering, and target warning.
- Loot ESP: names, tracers, distance filter, value filter, dynamic opacity.
- Chams highlighting for enemies and loot.
- Laser sight rendering for local and remote players, including hit markers and hit info.

#### Map And Navigation

- Minimap powered by a dedicated camera and `RenderTexture`.
- Icon rendering, zooming, dragging, render modes, and optional auto-centering.
- Path display for enemies or movement-related traces.
- Compass overlay with configurable size, range, scale, and offset.

#### Spawners

- Monster spawner with selectable enemy list, preview, spawn, kill, and teleport actions.
- Item spawner with search, selection, spawn, and item value controls.

#### Local Player Features

- God mode.
- Infinite stamina.
- Infinite battery.
- Grab range, grab strength, jump force, gravity, and movement speed tuning.
- NoClip mode.
- No ragdoll mode.
- FreeCam with configurable speed, fast multiplier, and sensitivity.

#### Miscellaneous

- Crosshair.
- FPS display.
- Fullbright.
- NoFog.
- RPC / decoration related fix module.
- Console logging and Unity log callback handling.

### Technical Overview

The project is loaded by SharpMonoInjector through `Cheat.Loader.Init()`. During initialization, the loader creates a persistent `GameObject` and attaches the core runtime components:

```text
CheatMenu
Minimap
Compass
RPCFixManager
```

`CheatMenu` handles feature updates, hotkeys, UI rendering, and delayed config saving. Features such as ESP, Chams, Minimap, FreeCam, and Laser Sight are split into separate modules so the codebase is easier to maintain and extend.

The configuration layer is handled by `ConfigManager` and `ConfigData`. Settings are grouped by feature and can be saved into profiles. Menu changes are saved with a short delay to reduce repeated file writes.

### Build From Source

You can build the project directly with `dotnet build`:

```powershell
dotnet build
```

For a Release build, use:

```powershell
dotnet build -c Release
```

After the build finishes, the generated output will be:

```text
Cheat.dll
```

Common output paths:

```text
bin\Debug\net48\Cheat.dll
bin\Release\net48\Cheat.dll
```

### Use The Release Package

If you do not want to build the project manually, you can download the prebuilt release package instead.

After downloading, make sure the package contains:

```text
Cheat.dll
```

### Injection Requirement

You need to prepare:

```text
SharpMonoInjector
```

Start the game first, then use SharpMonoInjector to inject `Cheat.dll` into the game process.

### SharpMonoInjector Entry Point

When filling in the injector fields manually, use the following entry point:

```text
class name: Loader
namespace: Cheat
method name: Init
```

### CLI Injection Command

If you are using the CLI version of SharpMonoInjector, run:

```powershell
smi.exe inject -p "REPO" -a ".\Cheat.dll" -n Cheat -c Loader -m Init
```

### Build And Inject Flow

```text
1. Clone or download this repository
2. Build the source code with dotnet build
3. Locate the generated Cheat.dll
4. Start R.E.P.O
5. Inject Cheat.dll with SharpMonoInjector
6. Use Cheat / Loader / Init as the entry point
```

### Notes

- The project targets `.NET Framework 4.8` / `net48`.
- If the build fails, make sure the .NET SDK and .NET Framework 4.8 Developer Pack are installed.
- Make sure the Unity and game dependency libraries in `Libs` are available.
- Make sure the `Cheat.dll` path and injector settings are correct.
- Use third-party injection tools and game modifications at your own risk.

---

## Entry Point Summary

```text
class name: Loader
namespace: Cheat
method name: Init
```

```powershell
smi.exe inject -p "REPO" -a ".\Cheat.dll" -n Cheat -c Loader -m Init
```
