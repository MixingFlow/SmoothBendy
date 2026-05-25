using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InControl;

public class InControlManager : SingletonMonoBehavior<InControlManager>
{
	public bool logDebugInfo = true;

	public bool invertYAxis;

	public bool useFixedUpdate;

	public bool dontDestroyOnLoad = true;

	public bool suspendInBackground;

	public bool enableICade;

	public bool enableXInput;

	public bool xInputOverrideUpdateRate;

	public int xInputUpdateRate;

	public bool xInputOverrideBufferSize;

	public int xInputBufferSize;

	public bool enableNativeInput = true;

	public bool nativeInputEnableXInput = true;

	public bool nativeInputPreventSleep;

	public bool nativeInputOverrideUpdateRate;

	public int nativeInputUpdateRate;

	public List<string> customProfiles = new List<string>();

	private void OnEnable()
	{
		if (base.EnforceSingleton)
		{
			return;
		}
		InputManager.InvertYAxis = invertYAxis;
		InputManager.SuspendInBackground = suspendInBackground;
		InputManager.EnableICade = enableICade;
		InputManager.EnableXInput = enableXInput;
		InputManager.XInputUpdateRate = (uint)Mathf.Max(xInputUpdateRate, 0);
		InputManager.XInputBufferSize = (uint)Mathf.Max(xInputBufferSize, 0);
		InputManager.EnableNativeInput = enableNativeInput;
		InputManager.NativeInputEnableXInput = nativeInputEnableXInput;
		InputManager.NativeInputUpdateRate = (uint)Mathf.Max(nativeInputUpdateRate, 0);
		InputManager.NativeInputPreventSleep = nativeInputPreventSleep;
		if (InputManager.SetupInternal())
		{
			if (logDebugInfo)
			{
				Debug.Log((object)string.Concat("InControl (version ", InputManager.Version, ")"));
				Logger.OnLogMessage -= LogMessage;
				Logger.OnLogMessage += LogMessage;
			}
			foreach (string customProfile in customProfiles)
			{
				Type type = Type.GetType(customProfile);
				if (type == null)
				{
					Debug.LogError((object)("Cannot find class for custom profile: " + customProfile));
				}
				else if (Activator.CreateInstance(type) is UnityInputDeviceProfileBase deviceProfile)
				{
					InputManager.AttachDevice(new UnityInputDevice(deviceProfile));
				}
			}
		}
		SceneManager.sceneLoaded -= OnSceneWasLoaded;
		SceneManager.sceneLoaded += OnSceneWasLoaded;
		if (dontDestroyOnLoad)
		{
			Object.DontDestroyOnLoad((Object)(object)this);
		}
	}

	private void OnDisable()
	{
		if (!base.IsNotTheSingleton)
		{
			SceneManager.sceneLoaded -= OnSceneWasLoaded;
			InputManager.ResetInternal();
		}
	}

	private void Update()
	{
		if (!base.IsNotTheSingleton && (!useFixedUpdate || Utility.IsZero(Time.timeScale)))
		{
			InputManager.UpdateInternal();
		}
	}

	private void FixedUpdate()
	{
		if (!base.IsNotTheSingleton && useFixedUpdate)
		{
			InputManager.UpdateInternal();
		}
	}

	private void OnApplicationFocus(bool focusState)
	{
		if (!base.IsNotTheSingleton)
		{
			InputManager.OnApplicationFocus(focusState);
		}
	}

	private void OnApplicationPause(bool pauseState)
	{
		if (!base.IsNotTheSingleton)
		{
			InputManager.OnApplicationPause(pauseState);
		}
	}

	private void OnApplicationQuit()
	{
		if (!base.IsNotTheSingleton)
		{
			InputManager.OnApplicationQuit();
		}
	}

	private void OnSceneWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (!base.IsNotTheSingleton)
		{
			InputManager.OnLevelWasLoaded();
		}
	}

	private static void LogMessage(LogMessage logMessage)
	{
		switch (logMessage.type)
		{
		case LogMessageType.Info:
			Debug.Log((object)logMessage.text);
			break;
		case LogMessageType.Warning:
			Debug.LogWarning((object)logMessage.text);
			break;
		case LogMessageType.Error:
			Debug.LogError((object)logMessage.text);
			break;
		}
	}
}
