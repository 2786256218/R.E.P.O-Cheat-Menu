using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cheat.Utils;
using UnityEngine;

namespace Cheat;

public static class ConsoleBase
{
	private delegate bool ConsoleCtrlDelegate(int ctrlType);

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static UnhandledExceptionEventHandler _003C0_003E__UnhandledExceptionHandler;

		public static Application.LogCallback _003C1_003E__UnityLogCallback;
	}

	private const int SW_HIDE = 0;

	private const int SW_SHOW = 5;

	private const int CTRL_CLOSE_EVENT = 2;

	private static bool _initialized;

	private static bool _isCrashing;

	private static ConsoleCtrlDelegate _consoleHandler;

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool AllocConsole();

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool FreeConsole();

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

	[DllImport("kernel32.dll")]
	private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handlerRoutine, bool add);

	public static void Init()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		if (_initialized)
		{
			return;
		}
		try
		{
			AllocConsole();
			_consoleHandler = ConsoleCtrlCheck;
			SetConsoleCtrlHandler(_consoleHandler, add: true);
			StreamWriter streamWriter = new StreamWriter(Console.OpenStandardOutput())
			{
				AutoFlush = true
			};
			Console.SetOut(streamWriter);
			Console.SetError(streamWriter);
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
			object obj = _003C_003EO._003C1_003E__UnityLogCallback;
			if (obj == null)
			{
				Application.LogCallback val = UnityLogCallback;
				_003C_003EO._003C1_003E__UnityLogCallback = val;
				obj = (object)val;
			}
			Application.logMessageReceived += (Application.LogCallback)obj;
			_initialized = true;
			Cheat.Utils.Logger.Info("Console Initialized & Global Exception Handler Registered", "Console");
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"[Cheat] Failed to initialize console: {arg}");
		}
	}

	public static void Destroy()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		if (!_initialized)
		{
			return;
		}
		Cheat.Utils.Logger.Info("Destroying Console...", "Console");
		try
		{
			AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler;
			object obj = _003C_003EO._003C1_003E__UnityLogCallback;
			if (obj == null)
			{
				Application.LogCallback val = UnityLogCallback;
				_003C_003EO._003C1_003E__UnityLogCallback = val;
				obj = (object)val;
			}
			Application.logMessageReceived -= (Application.LogCallback)obj;
		}
		catch
		{
		}
		if (_consoleHandler != null)
		{
			SetConsoleCtrlHandler(_consoleHandler, add: false);
			_consoleHandler = null;
		}
		FreeConsole();
		_initialized = false;
	}

	private static bool ConsoleCtrlCheck(int ctrlType)
	{
		if (ctrlType == 2)
		{
			Cheat.Utils.Logger.Info("Console close event received. Unloading...", "Console");
			Loader.Unload();
			return true;
		}
		return false;
	}

	private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
	{
		if (_isCrashing)
		{
			return;
		}
		_isCrashing = true;
		string text;
		if (e.ExceptionObject is Exception ex)
		{
			text = ex.Message + "\n" + ex.StackTrace;
			Cheat.Utils.Logger.Critical("UNHANDLED EXCEPTION: " + text, "Global");
		}
		else
		{
			text = e.ExceptionObject?.ToString() ?? "Unknown Error";
			Cheat.Utils.Logger.Critical("UNHANDLED EXCEPTION: " + text, "Global");
		}
		MessageBox(IntPtr.Zero, "Cheat Critical Error:\n" + text + "\n\nThe cheat will now unload to prevent a game crash.\nYou can reinject the cheat to restart it.", "Cheat Crashed", 16u);
		try
		{
			Loader.Unload();
		}
		catch (Exception arg)
		{
			Cheat.Utils.Logger.Critical($"Failed to unload after crash: {arg}", "Global");
		}
	}

	private static void UnityLogCallback(string condition, string stackTrace, LogType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected I4, but got Unknown
		switch ((int)type)
		{
		case 0:
		case 1:
		case 4:
			Cheat.Utils.Logger.Error(condition + "\n" + stackTrace, "Unity");
			break;
		case 2:
			Cheat.Utils.Logger.Warn(condition, "Unity");
			break;
		default:
			Cheat.Utils.Logger.Info(condition, "Unity");
			break;
		}
	}
}
