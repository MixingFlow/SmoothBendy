using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using I2.Loc;
using InControl;
using S13Audio;
using TMG.AssetBundles;
using TMG.Core;
using TMG.Data;
using TMG.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : TMGAbstractDisposable
{
	public bool isGameLoaded;

	public GameDataManager GameDataManager;

	public GameData GameData;

	public PlayerSettings PlayerSettings;

	public AssetBundleManager AssetBundleManager;

	public AssetManager AssetManager;

	public UIManager UIManager;

	public AudioManager AudioManager;

	public PoolingManager PoolingManager;

	public AchievementManager AchievementManager;

	public SteamManager SteamManager;

	public ParticleManager ParticleManager;

	public GameCamera GameCamera;

	public PlayerController Player;

	public ChapterController CurrentChapter;

	public GlobalInkEffectManager InkEffectManager;

	public AiGlobalNetwork AiGlobalNetwork;

	public CharacterManager CharacterManager;

	private ScreenBlockerController m_ScreenBlockerController;

	private ScreenBlockerController m_ScreenWhiteBlockerController;

	private GenericLoaderController m_GenericLoader;

	private MainCrosshairController m_MainCrosshairController;

	private GameMenuController m_PauseMenu;

	private MainSubtitlesController m_MainSubtitlesController;

	private ObjectivesController m_ObjectiveController;

	private HurtBordersController m_HurtBordersController;

	private TutorialPopupController m_TutorialPopupController;

	private ObjectiveDataVO m_CurrentObjective;

	private Action m_BlockerOnShow;

	private Action m_BlockerOnHide;

	public bool isDead;

	public Color CurrentAmbience;

	private GameState m_GameState;

	private AudioClip m_WhooshClip;

	private AudioClip m_PauseClip;

	private static GameManager m_Instance;

	public bool AudioTypeChanged;

	public S13AudioManager S13AudioManager
	{
		get
		{
			return S13AudioManager.Instance;
		}
		protected set
		{
		}
	}

	public bool isPauseReady { get; private set; }

	public bool isPaused { get; private set; }

	public ObjectiveDataVO CurrentObjective => m_CurrentObjective;

	public bool HasController
	{
		get
		{
			if (InputManager.Devices != null)
			{
				return InputManager.Devices.Count > 0;
			}
			return false;
		}
	}

	public static GameManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new GameManager();
			}
			return m_Instance;
		}
	}

	public event EventHandler OnUnpaused;

	protected GameManager()
	{
		isPaused = false;
	}

	public void InitChapter(Chapters chapter, Transform startPoint, ChapterController controller)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)m_WhooshClip))
		{
			m_WhooshClip = m_Instance.GetAudioClip("Audio/SFX/SFX_HUD_whoosh_01");
		}
		if (!Object.op_Implicit((Object)(object)m_PauseClip))
		{
			m_PauseClip = m_Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_inktoy");
		}
		ShowScreenBlocker(0f);
		Player = Instance.AssetManager.CreateAsset<PlayerController>("GamePlay/Characters/Player/PlayerController");
		Player.transform.eulerAngles = startPoint.eulerAngles;
		Player.GoToAndLookAt(startPoint);
		ParticleManager.Initialize();
		Cursor.lockState = (CursorLockMode)1;
		Cursor.visible = false;
		switch (chapter)
		{
		case Chapters.ONE:
			InitChapter1(controller);
			break;
		case Chapters.TWO:
			InitChapter2(controller);
			break;
		case Chapters.THREE:
			InitChapter3(controller);
			break;
		case Chapters.FOUR:
			InitChapter4(controller);
			break;
		case Chapters.FIVE:
			InitChapter5(controller);
			break;
		}
	}

	private void InitChapter1(ChapterController controller)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		CurrentChapter = controller;
		Player.SetCameraSway(active: true);
		Player.SetLock(active: true);
		CurrentAmbience = AmbienceColors.CH1_Base;
		S13AudioManager.InvokeEvent("evt_game_at_loading_chapter1");
		Object.Instantiate<GameObject>(AssetManager.GetAsset<GameObject>("GamePlay/Chapters/CH1_SecretMessage"), Vector3.zero, Quaternion.identity);
		BaseChapterInitializers(1);
	}

	private void InitChapter2(ChapterController controller)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		CurrentChapter = controller;
		Player.SetInteraction(active: false);
		Player.SetLock(active: true);
		CurrentAmbience = AmbienceColors.CH2_Base;
		S13AudioManager.InvokeEvent("evt_game_at_loading_chapter2");
		Object.Instantiate<GameObject>(AssetManager.GetAsset<GameObject>("GamePlay/Chapters/CH2_SecretMessage"), Vector3.zero, Quaternion.identity);
		BaseChapterInitializers(2);
	}

	private void InitChapter3(ChapterController controller)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		CurrentChapter = controller;
		Player.SetCameraSway(active: false);
		Player.SetLock(active: true);
		CurrentAmbience = AmbienceColors.CH3_Base;
		S13AudioManager.InvokeEvent("evt_game_at_loading_chapter3");
		Object.Instantiate<GameObject>(AssetManager.GetAsset<GameObject>("GamePlay/Chapters/CH3_SecretMessage"), Vector3.zero, Quaternion.identity);
		BaseChapterInitializers(3);
	}

	private void InitChapter4(ChapterController controller)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		CurrentChapter = controller;
		Player.SetCameraSway(active: true);
		Player.SetLock(active: true);
		CurrentAmbience = AmbienceColors.CH4_Base;
		S13AudioManager.InvokeEvent("evt_game_at_loading_chapter4");
		Object.Instantiate<GameObject>(AssetManager.GetAsset<GameObject>("GamePlay/Chapters/CH4_SecretMessage"), Vector3.zero, Quaternion.identity);
		BaseChapterInitializers(4);
	}

	private void InitChapter5(ChapterController controller)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		CurrentChapter = controller;
		Player.SetCameraSway(active: true);
		Player.SetLock(active: true);
		CurrentAmbience = AmbienceColors.CH5_Base;
		S13AudioManager.InvokeEvent("evt_game_at_loading_chapter5");
		Object.Instantiate<GameObject>(AssetManager.GetAsset<GameObject>("GamePlay/Chapters/CH5_SecretMessage"), Vector3.zero, Quaternion.identity);
		BaseChapterInitializers(5);
	}

	private void BaseChapterInitializers(int currentChapter)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		HideCrosshair();
		LockPause();
		RenderSettings.skybox = new Material(Shader.Find("Skybox/Skybox Color"));
		RenderSettings.ambientMode = (AmbientMode)0;
		RenderSettings.skybox.color = CurrentAmbience;
		DynamicGI.UpdateEnvironment();
		if (Object.op_Implicit((Object)(object)Instance.GameCamera))
		{
			Instance.GameCamera.Brightness.SetBrightness(Instance.PlayerSettings.Brightness);
		}
		Player.SetSensitivity(Instance.PlayerSettings.Sensitivity);
		if (AiGlobalNetwork != null)
		{
			AiGlobalNetwork.Dispose();
		}
		AiGlobalNetwork = AiGlobalNetwork.Create();
		GameData.CurrentSaveFile.CurrentChapter = currentChapter;
	}

	public void ChapterInitOnComplete()
	{
		AiGlobalNetwork.BuildPathing();
		if (InkEffectManager == null)
		{
			InkEffectManager = new GlobalInkEffectManager();
		}
		InkEffectManager.GetEffects();
		InkEffectManager.SetActive(active: false, isInit: true);
		if ((Object)(object)SeasonalController.Instance != (Object)null && (Object)(object)CurrentChapter != (Object)null)
		{
			SeasonalController.Instance.Activate(CurrentChapter.Chapter);
		}
	}

	public AudioClip GetAudioClip(string assetKey)
	{
		return Instance.AssetManager.GetAsset<AudioClip>(assetKey);
	}

	public AudioClip[] GetAudioClips(string folderKey)
	{
		return Instance.AssetManager.GetAssets<AudioClip>(folderKey);
	}

	private void CheckScreenBlocker()
	{
		if (!Object.op_Implicit((Object)(object)m_ScreenBlockerController))
		{
			m_ScreenBlockerController = Instance.UIManager.Show<ScreenBlockerController>("UI/Blocker/ScreenBlockerController", "BLOCKER");
		}
	}

	private void CheckWhiteScreenBlocker()
	{
		if (!Object.op_Implicit((Object)(object)m_ScreenWhiteBlockerController))
		{
			m_ScreenWhiteBlockerController = Instance.UIManager.Show<ScreenBlockerController>("UI/Blocker/ScreenWhiteBlockerController", "BLOCKER");
		}
	}

	public void ShowScreenBlocker(float duration = 0.5f, float delay = 0f, Action onComplete = null)
	{
		CheckScreenBlocker();
		m_BlockerOnShow = onComplete;
		m_ScreenBlockerController.Show(duration, delay);
		m_ScreenBlockerController.OnShow -= HandleScreenBlockerOnShown;
		m_ScreenBlockerController.OnShow += HandleScreenBlockerOnShown;
	}

	public void HideScreenBlocker(float duration = 0.5f, float delay = 0f, Action onComplete = null)
	{
		CheckScreenBlocker();
		m_BlockerOnHide = onComplete;
		m_ScreenBlockerController.Hide(duration, delay);
		m_ScreenBlockerController.OnHide -= HandleScreenBlockerOnHide;
		m_ScreenBlockerController.OnHide += HandleScreenBlockerOnHide;
	}

	public void KillScreenBlockerFadeTween()
	{
		m_ScreenBlockerController.KillFadeTween();
	}

	public void ShowWhiteScreenBlocker(float duration = 0.5f, float delay = 0f, Action onComplete = null)
	{
		CheckWhiteScreenBlocker();
		m_BlockerOnShow = onComplete;
		m_ScreenWhiteBlockerController.Show(duration, delay);
		m_ScreenWhiteBlockerController.OnShow -= HandleScreenWhiteBlockerOnShown;
		m_ScreenWhiteBlockerController.OnShow += HandleScreenWhiteBlockerOnShown;
	}

	public void HideWhiteScreenBlocker(float duration = 0.5f, float delay = 0f, Action onComplete = null)
	{
		CheckWhiteScreenBlocker();
		m_BlockerOnHide = onComplete;
		m_ScreenWhiteBlockerController.Hide(duration, delay);
		m_ScreenWhiteBlockerController.OnHide -= HandleScreenWhiteBlockerOnHide;
		m_ScreenWhiteBlockerController.OnHide += HandleScreenWhiteBlockerOnHide;
	}

	private void HandleScreenBlockerOnShown(object sender, EventArgs e)
	{
		if ((Object)(object)m_ScreenBlockerController != (Object)null)
		{
			m_ScreenBlockerController.OnShow -= HandleScreenBlockerOnShown;
		}
		if (m_BlockerOnShow != null)
		{
			m_BlockerOnShow();
		}
	}

	private void HandleScreenBlockerOnHide(object sender, EventArgs e)
	{
		m_ScreenBlockerController.OnHide -= HandleScreenBlockerOnHide;
		if (m_BlockerOnHide != null)
		{
			m_BlockerOnHide();
		}
	}

	private void HandleScreenWhiteBlockerOnShown(object sender, EventArgs e)
	{
		m_ScreenWhiteBlockerController.OnShow -= HandleScreenWhiteBlockerOnShown;
		if (m_BlockerOnShow != null)
		{
			m_BlockerOnShow();
		}
	}

	private void HandleScreenWhiteBlockerOnHide(object sender, EventArgs e)
	{
		m_ScreenWhiteBlockerController.OnHide -= HandleScreenWhiteBlockerOnHide;
		if (m_BlockerOnHide != null)
		{
			m_BlockerOnHide();
		}
	}

	public void ShowChapterTitle(string chapter, string title, bool showBlocker = true)
	{
		S13AudioManager.Instance.InvokeEvent("evt_game_at_chapter_titles");
		ChapterTitleDataVO data = ChapterTitleDataVO.Create(chapter, title, showBlocker);
		Instance.UIManager.Show<ChapterTitleModalController>("UI/Modals/ChapterTitleModalController", "CHAPTERTITLE", data);
	}

	public void ShowHurtBorder(bool isSilent = false)
	{
		if (!isDead)
		{
			if (!Object.op_Implicit((Object)(object)m_HurtBordersController))
			{
				m_HurtBordersController = Instance.UIManager.Show<HurtBordersController>("UI/Borders/HurtBordersController", "BORDERS");
			}
			m_HurtBordersController.OnMaxHit -= HandleHurtBordersOnHitMax;
			m_HurtBordersController.OnMaxHit += HandleHurtBordersOnHitMax;
			m_HurtBordersController.ShowBorder(isSilent);
		}
	}

	public void Heal()
	{
		isDead = false;
		if (Object.op_Implicit((Object)(object)m_HurtBordersController))
		{
			m_HurtBordersController.OnMaxHit -= HandleHurtBordersOnHitMax;
			m_HurtBordersController.Kill();
			m_HurtBordersController = null;
		}
	}

	private void HandleHurtBordersOnHitMax(object sender, EventArgs e)
	{
		m_HurtBordersController.OnMaxHit -= HandleHurtBordersOnHitMax;
		ShowScreenBlocker(0f);
		m_HurtBordersController.Kill();
		m_HurtBordersController = null;
		isDead = false;
		Player.Die();
	}

	public void ShowCrosshair()
	{
		if (!Instance.PlayerSettings.Crosshair)
		{
			KillCrosshair();
		}
		else if (!Object.op_Implicit((Object)(object)m_MainCrosshairController))
		{
			m_MainCrosshairController = Instance.UIManager.Show<MainCrosshairController>("UI/Crosshairs/MainCrosshairController", "CROSSHAIR");
		}
		else
		{
			m_MainCrosshairController.Show();
		}
	}

	public void HideCrosshair()
	{
		if (Object.op_Implicit((Object)(object)m_MainCrosshairController))
		{
			m_MainCrosshairController.Hide();
		}
	}

	public void KillCrosshair()
	{
		if (Object.op_Implicit((Object)(object)m_MainCrosshairController))
		{
			m_MainCrosshairController.Dispose();
			m_MainCrosshairController = null;
		}
	}

	public void UnlockPause()
	{
		isPauseReady = true;
	}

	public void LockPause()
	{
		isPauseReady = false;
	}

	public void PauseGame()
	{
		Time.timeScale = 0f;
		DOTween.PauseAll();
		AudioManager.PauseAll();
		if (Object.op_Implicit((Object)(object)m_PauseClip))
		{
			Instance.AudioManager.Play(m_PauseClip);
		}
		S13AudioManager.InvokeEvent("evt_game_at_paused");
	}

	public void UnpauseGame()
	{
		Time.timeScale = 1f;
		List<Tween> list = DOTween.PausedTweens((List<Tween>)null);
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Tween val = list[i];
				if (val != null)
				{
					TweenExtensions.Play<Tween>(val);
				}
			}
		}
		AudioManager.ResumeAll();
		this.OnUnpaused.Send(this);
		S13AudioManager.InvokeEvent("evt_game_at_resume");
	}

	public void Pause()
	{
		if (!Object.op_Implicit((Object)(object)m_PauseMenu) && !isPaused)
		{
			isPaused = true;
			m_PauseMenu = Instance.UIManager.Show<GameMenuController>("UI/Menus/GameMenuController", "PAUSE");
			m_PauseMenu.OnPlayOutComplete -= HandlePauseMenuPlayOutComplete;
			m_PauseMenu.OnPlayOutComplete += HandlePauseMenuPlayOutComplete;
		}
	}

	public void Unpause()
	{
		if ((!Object.op_Implicit((Object)(object)m_PauseMenu) || !m_PauseMenu.IsQuitting) && Object.op_Implicit((Object)(object)m_PauseMenu) && isPaused)
		{
			isPaused = false;
			m_PauseMenu.CloseMenu();
		}
	}

	public AudioObject ShowDialogue(DialogueDataVO dataVO)
	{
		AudioObject audioObject = ((!((Object)(object)dataVO.DialogueClip != (Object)null)) ? Instance.AudioManager.Play(dataVO.Dialogue, AudioObjectType.DIALOGUE, 0, isQueued: true) : Instance.AudioManager.Play(dataVO.DialogueClip, AudioObjectType.DIALOGUE, 0, isQueued: true));
		dataVO.Subtitles.Duration = audioObject.AudioClip.length;
		if (Instance.PlayerSettings.Subtitles)
		{
			Instance.ShowSubtitles(dataVO.Subtitles.Subtitles, dataVO.Subtitles.Duration, dataVO.Subtitles.IsTrimmed);
		}
		return audioObject;
	}

	public void ShowSubtitles(string subtitles, float duration, bool isTrimmed = false)
	{
		if (Instance.PlayerSettings.Subtitles)
		{
			if (!Object.op_Implicit((Object)(object)m_MainSubtitlesController))
			{
				m_MainSubtitlesController = Instance.UIManager.Show<MainSubtitlesController>("UI/Subtitles/MainSubtitlesController", "SUBTITLES");
			}
			m_MainSubtitlesController.ShowSubtitles(subtitles, duration, isTrimmed);
		}
	}

	public void KillSubtitles()
	{
		if ((Object)(object)m_MainSubtitlesController != (Object)null)
		{
			m_MainSubtitlesController.Dispose();
			m_MainSubtitlesController = null;
		}
	}

	public void ShowCollectable(CollectableDataVO dataVO)
	{
		Instance.UIManager.Show<CollectableModalController>("UI/Modals/CollectableModalController", "MODAL", dataVO);
	}

	public void ShowTutorial(TutorialDataVO data)
	{
		if (!Object.op_Implicit((Object)(object)m_TutorialPopupController))
		{
			m_TutorialPopupController = UIManager.Show<TutorialPopupController>("UI/Crosshairs/TutorialPopupController", "CROSSHAIR", data);
		}
		else
		{
			m_TutorialPopupController.Show();
		}
	}

	public void HideTutorial()
	{
		if (Object.op_Implicit((Object)(object)m_TutorialPopupController))
		{
			m_TutorialPopupController.Hide();
		}
	}

	public void ClearObjective()
	{
		m_CurrentObjective = null;
		m_ObjectiveController = null;
	}

	public void UpdateObjective(ObjectiveDataVO data)
	{
		m_CurrentObjective = data;
	}

	public void ShowObjective(ObjectiveDataVO data)
	{
		m_CurrentObjective = data;
		if (!Instance.PlayerSettings.Tips)
		{
			UpdateObjective(data);
			return;
		}
		Instance.AudioManager.Play(m_WhooshClip);
		if (Object.op_Implicit((Object)(object)m_ObjectiveController))
		{
			m_ObjectiveController.OnPlayOutComplete -= HandleShowAfterObjectivePlayOutComplete;
			m_ObjectiveController.OnPlayOutComplete += HandleShowAfterObjectivePlayOutComplete;
			m_ObjectiveController.Hide();
		}
		else
		{
			m_ObjectiveController = ActualShowObjective();
		}
	}

	private ObjectivesController ActualShowObjective()
	{
		return Instance.UIManager.Show<ObjectivesController>("UI/Objectives/ObjectiveController", "OBJECTIVE", m_CurrentObjective);
	}

	private void HandleObjectivePlayOutComplete(object sender, EventArgs e)
	{
		m_ObjectiveController.OnPlayOutComplete -= HandleObjectivePlayOutComplete;
		m_ObjectiveController = null;
	}

	private void HandleShowAfterObjectivePlayOutComplete(object sender, EventArgs e)
	{
		m_ObjectiveController.OnPlayOutComplete -= HandleShowAfterObjectivePlayOutComplete;
		m_ObjectiveController = ActualShowObjective();
	}

	private void HandlePauseMenuPlayOutComplete(object sender, EventArgs e)
	{
		m_PauseMenu.OnPlayOutComplete -= HandlePauseMenuPlayOutComplete;
		m_PauseMenu = null;
	}

	public void LoadScene(GenericLoaderDataVO data)
	{
		isDead = false;
		Player = null;
		GameCamera = null;
		isPaused = false;
		if (!Object.op_Implicit((Object)(object)m_GenericLoader))
		{
			m_GenericLoader = Instance.UIManager.Show<GenericLoaderController>("UI/Loaders/GenericLoaderController", "LOADER", data);
		}
		m_GenericLoader.OnLoaded -= HandleSceneOnLoaded;
		m_GenericLoader.OnLoaded += HandleSceneOnLoaded;
		m_GenericLoader.LoadScene(data);
	}

	private void HandleSceneOnLoaded(object sender, EventArgs e)
	{
		m_GenericLoader.OnLoaded -= HandleSceneOnLoaded;
		m_GenericLoader.HideLoader();
	}

	public void ShowCredits()
	{
		SceneManager.sceneLoaded += HandleCreditsSceneLoaded;
		SceneManager.LoadScene("Empty");
	}

	private void HandleCreditsSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		SceneManager.sceneLoaded -= HandleCreditsSceneLoaded;
		Instance.UIManager.Show<CreditsScreenController>("UI/Views/CreditsScreen", "VIEW");
	}

	public void ClearAllGameUI()
	{
		KillCrosshair();
		if (m_CurrentObjective != null)
		{
			m_CurrentObjective.Dispose();
			m_CurrentObjective = null;
		}
		if (Object.op_Implicit((Object)(object)m_HurtBordersController))
		{
			m_HurtBordersController.Dispose();
			m_HurtBordersController = null;
		}
		if (Object.op_Implicit((Object)(object)m_ObjectiveController))
		{
			m_ObjectiveController.Dispose();
			m_ObjectiveController = null;
		}
		if (Object.op_Implicit((Object)(object)m_PauseMenu))
		{
			m_PauseMenu.Dispose();
			m_PauseMenu = null;
		}
		if (Object.op_Implicit((Object)(object)m_MainSubtitlesController))
		{
			m_MainSubtitlesController.Dispose();
			m_MainSubtitlesController = null;
		}
	}

	public string GetProjectVersion()
	{
		VersionSettingsSO versionSettingsSO = Resources.Load<VersionSettingsSO>("VersionSettings");
		return "v" + versionSettingsSO.Major + "." + versionSettingsSO.Minor + "." + versionSettingsSO.Patch + "." + versionSettingsSO.Hotfix;
	}

	public void Quit()
	{
		Application.Quit();
	}

	protected override void OnDisposed()
	{
		isPaused = false;
		if (Object.op_Implicit((Object)(object)m_PauseMenu))
		{
			m_PauseMenu.OnPlayOutComplete -= HandlePauseMenuPlayOutComplete;
			m_PauseMenu = null;
		}
		if (AchievementManager != null)
		{
			AchievementManager.Dispose();
		}
		AchievementManager = null;
		if (InkEffectManager != null)
		{
			InkEffectManager.Dispose();
		}
		InkEffectManager = null;
		if (ParticleManager != null)
		{
			ParticleManager.Dispose();
		}
		ParticleManager = null;
		if ((Object)(object)CurrentChapter != (Object)null)
		{
			CurrentChapter.Dispose();
		}
		CurrentChapter = null;
		if (AssetManager != null)
		{
			AssetManager.Dispose();
		}
		AssetManager = null;
		if (CharacterManager != null)
		{
			CharacterManager.Dispose();
		}
		CharacterManager = null;
		if (GameDataManager != null)
		{
			GameDataManager.Dispose();
		}
		GameDataManager = null;
		GameData = null;
		PlayerSettings.Dispose();
		PlayerSettings = null;
		Player = null;
		GameCamera = null;
		SteamManager = null;
		m_ScreenBlockerController = null;
		m_ScreenWhiteBlockerController = null;
		m_GenericLoader = null;
		m_MainCrosshairController = null;
		m_ObjectiveController = null;
		m_MainSubtitlesController = null;
		m_HurtBordersController = null;
		m_TutorialPopupController = null;
		isPaused = false;
		m_BlockerOnHide = null;
		m_BlockerOnShow = null;
		base.OnDisposed();
	}

	public static void SetLanguage(string code)
	{
		if (code == "zh-CN" || code == "zh-Hans")
		{
			code = "zh";
		}
		if (code == "en-US" || code == "en-GB")
		{
			code = "en";
		}
		List<string> allLanguagesCode = LocalizationManager.GetAllLanguagesCode();
		string languageFromCode = LocalizationManager.GetLanguageFromCode(code);
		if (allLanguagesCode.Contains(code) && !string.IsNullOrEmpty(languageFromCode))
		{
			Instance.PlayerSettings.Language = code;
			LocalizationManager.SetLanguageAndCode(languageFromCode, code, RememberLanguage: true, Force: true);
		}
	}
}
