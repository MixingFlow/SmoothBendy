using System;
using System.Text;
using Steamworks;
using TMG.Core;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamManager : TMGMonoBehaviour
{
	private static bool s_EverInialized;

	private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	public bool IsInitialized { get; private set; }

	private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
	}

	public override void Init()
	{
		if (s_EverInialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		Object.DontDestroyOnLoad((Object)(object)base.gameObject);
		if (!Packsize.Test())
		{
			SteamDebug("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", (Object)(object)this);
		}
		if (!DllCheck.Test())
		{
			SteamDebug("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", (Object)(object)this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			SteamDebug("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + ex, (Object)(object)this);
			Application.Quit();
			return;
		}
		IsInitialized = SteamAPI.Init();
		if (!IsInitialized)
		{
			SteamDebug("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", (Object)(object)this);
			return;
		}
		SteamUserStats.RequestCurrentStats();
		s_EverInialized = true;
	}

	public override void OnEnable()
	{
		if (IsInitialized && m_SteamAPIWarningMessageHook == null)
		{
			m_SteamAPIWarningMessageHook = SteamAPIDebugTextHook;
			SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
		}
	}

	private void Update()
	{
		if (IsInitialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	private void SteamDebug(object message, Object context)
	{
	}

	protected override void OnDisposed()
	{
		if (IsInitialized)
		{
			SteamAPI.Shutdown();
		}
		m_SteamAPIWarningMessageHook = null;
		base.OnDisposed();
	}
}
