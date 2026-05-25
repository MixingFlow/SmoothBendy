using System;
using System.IO;
using DG.Tweening;
using InControl;
using S13Audio;
using TMG.Data;
using TMG.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMG.Core;

public class InitializeGame : TMGMonoBehaviour
{
	private GameManager m_GameManager;

	private bool m_hasNoLanguagePrefs;

	public override void Init()
	{
		base.Init();
		Application.targetFrameRate = 60;
		m_GameManager = GameManager.Instance;
		if (m_GameManager.isGameLoaded)
		{
			InitializeFramework();
		}
		else
		{
			PreloadFramework();
		}
	}

	private void PreloadFramework()
	{
		InitAssetManager();
		InitUIManager();
		InitAudioManager();
		InitDOTween();
		InitPlayerSettings();
		if (IsRunningOnEpic())
		{
			InitPlatformController();
		}
		GameManager.Instance.AssetManager.CreateAsset<IntroController>("UI/Intro/MeatlyLogo").OnCompleteEvent += HandleSplashScreenOnComplete;
	}

	private void HandleSplashScreenOnComplete(object sender, EventArgs e)
	{
		IntroController obj = sender as IntroController;
		obj.OnCompleteEvent -= HandleSplashScreenOnComplete;
		obj.Dispose();
		GameManager.Instance.UIManager.Show<PreLoaderController>("UI/Loaders/PreLoaderController", "VIEW").OnPlayOutComplete += HandlePreLoaderOnComplete;
	}

	private void HandlePreLoaderOnComplete(object sender, EventArgs e)
	{
		(sender as PreLoaderController).OnPlayOutComplete -= HandlePreLoaderOnComplete;
		FinalizeFramework();
	}

	private void InitializeFramework()
	{
		InitPlayerSettings();
		InitAssetManager();
		InitUIManager();
		InitAudioManager();
		InitDOTween();
		InitSaveData();
		FinalizeFramework();
	}

	private void FinalizeFramework()
	{
		InitCharacterManager();
		InitParticleManager();
		InitPoolingManager();
		InitAchievementManager();
		InitInControl();
		LoadTitleScreen();
	}

	private void LoadTitleScreen()
	{
		SceneManager.LoadScene("Empty");
		GameManager.Instance.UIManager.Show<TitleScreenController>("UI/Views/TitleScreen", "VIEW");
		m_GameManager.isGameLoaded = true;
		Dispose();
	}

	private void InitSaveData()
	{
		if (m_GameManager.GameDataManager != null)
		{
			Debug.LogWarning((object)"Game Data Manager already exists.", (Object)(object)this);
			return;
		}
		m_GameManager.GameDataManager = new GameDataManager();
		m_GameManager.GameDataManager.Load();
	}

	private void InitAssetManager()
	{
		if (m_GameManager.AssetManager != null)
		{
			Debug.LogWarning((object)"Asset Manager already exists.", (Object)(object)this);
		}
		else
		{
			m_GameManager.AssetManager = new AssetManager();
		}
	}

	private void InitPoolingManager()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_GameManager.PoolingManager))
		{
			Debug.LogWarning((object)"Pooling Manager already exists.", (Object)(object)this);
			return;
		}
		m_GameManager.PoolingManager = new GameObject("[POOLING MANAGER]").AddComponent<PoolingManager>();
		Object.DontDestroyOnLoad((Object)(object)m_GameManager.PoolingManager.gameObject);
	}

	private void InitPlayerSettings()
	{
		if (m_GameManager.PlayerSettings != null)
		{
			Debug.LogWarning((object)"Player Settings already exists.", (Object)(object)this);
			return;
		}
		m_GameManager.PlayerSettings = new PlayerSettings();
		m_GameManager.PlayerSettings.Initialize();
	}

	private void InitAchievementManager()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (IsRunningOnSteam())
		{
			m_GameManager.SteamManager = Object.FindObjectOfType<SteamManager>();
			if ((Object)(object)m_GameManager.SteamManager == (Object)null)
			{
				m_GameManager.SteamManager = new GameObject("[SteamManager]").AddComponent<SteamManager>();
			}
			m_GameManager.AchievementManager = new AchievementManager();
			m_GameManager.AchievementManager.InitSteam();
		}
		else if (IsRunningOnEpic())
		{
			m_GameManager.AchievementManager = new AchievementManager();
			m_GameManager.AchievementManager.InitEpic();
		}
		else
		{
			m_GameManager.AchievementManager = new AchievementManager();
			m_GameManager.AchievementManager.InitGOG();
		}
	}

	private void InitDOTween()
	{
		DOTween.Init((bool?)null, (bool?)null, (LogBehaviour?)null);
	}

	private void InitInControl()
	{
		InControlManager instance = SingletonMonoBehavior<InControlManager>.Instance;
		((Behaviour)instance).enabled = false;
		instance.dontDestroyOnLoad = true;
		((Behaviour)instance).enabled = true;
	}

	private void InitAudioManager()
	{
		if (Object.op_Implicit((Object)(object)m_GameManager.AudioManager))
		{
			Debug.LogWarning((object)"Audio Manager already exists.", (Object)(object)this);
			return;
		}
		m_GameManager.AudioManager = AudioManager.Create();
		m_GameManager.AssetManager.CreateAsset<S13AudioManager>("S13AudioManager");
	}

	private void InitUIManager()
	{
		if (Object.op_Implicit((Object)(object)m_GameManager.UIManager))
		{
			Debug.LogWarning((object)"UI Mananger already exists.", (Object)(object)this);
		}
		else
		{
			m_GameManager.UIManager = UIManager.Create();
		}
	}

	private void InitCharacterManager()
	{
		if (m_GameManager.CharacterManager != null)
		{
			Debug.LogWarning((object)"Character Manager already exists.", (Object)(object)this);
		}
		else
		{
			m_GameManager.CharacterManager = new CharacterManager();
		}
	}

	private void InitParticleManager()
	{
		if (m_GameManager.ParticleManager != null)
		{
			Debug.LogWarning((object)"Particle Manager already exists.", (Object)(object)this);
		}
		else
		{
			m_GameManager.ParticleManager = new ParticleManager();
		}
	}

	protected override void OnDisposed()
	{
		m_GameManager = null;
		base.OnDisposed();
	}

	private bool IsRunningOnSteam()
	{
		return Directory.GetFiles(Path.Combine(Application.dataPath, ".."), "*steam_api64*", SearchOption.TopDirectoryOnly).Length != 0;
	}

	private void InitPlatformController()
	{
		try
		{
			EOSController eOSController = Object.Instantiate<EOSController>(Resources.Load<EOSController>("EOSController"));
			if (Object.op_Implicit((Object)(object)eOSController))
			{
				EOSController.onUserSignin = (Action<EOSController>)Delegate.Combine(EOSController.onUserSignin, new Action<EOSController>(OnPlatformInitialized));
				eOSController.Init();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)ex);
		}
	}

	private bool IsRunningOnEpic()
	{
		return Directory.GetDirectories(Path.Combine(Application.dataPath, ".."), ".egstore", SearchOption.TopDirectoryOnly).Length != 0;
	}

	private void OnPlatformInitialized(EOSController obj)
	{
		EOSController.onUserSignin = (Action<EOSController>)Delegate.Remove(EOSController.onUserSignin, new Action<EOSController>(OnPlatformInitialized));
		if (m_hasNoLanguagePrefs)
		{
			GameManager.SetLanguage(obj.PreferredLanguage);
		}
	}
}
