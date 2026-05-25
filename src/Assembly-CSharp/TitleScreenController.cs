using System;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using InControl;
using S13Audio;
using Steamworks;
using TMG.Controls;
using TMG.Data;
using TMG.UI;
using TMG.UI.Controls;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenController : BaseUIController
{
	private enum MenuItem
	{
		MAIN,
		BEGIN,
		CHAPTERS,
		SLOT,
		OPTIONS,
		WARNING,
		LAUNCHING
	}

	[Header("RectTransforms")]
	[SerializeField]
	private RectTransform m_Visuals;

	[Header("Navigation")]
	[SerializeField]
	private GameObject m_NavigationInput;

	[SerializeField]
	private MenuItemNavInput m_SelectNav;

	[SerializeField]
	private MenuItemNavInput m_BackNav;

	[SerializeField]
	private GameObject m_NavicationKB;

	[SerializeField]
	private BaseUIButton m_KBBackBtn;

	[Header("TextMeshProUGUI")]
	[SerializeField]
	private TextMeshProUGUI m_VersionLabel;

	[Header("< Images >")]
	[SerializeField]
	private Image m_BlackOutImage;

	[SerializeField]
	private Image m_LightFlickerImage;

	[SerializeField]
	private Image m_FullFlickerImage;

	[Header("[MAIN MENU]")]
	[SerializeField]
	private GameObject m_MainMenu;

	[SerializeField]
	private MenuItemButton m_BeginBtn;

	[SerializeField]
	private MenuItemButton m_OptionsBtn;

	[SerializeField]
	private MenuItemButton m_QuitBtn;

	[Header("[BEGIN MENU]")]
	[SerializeField]
	private GameObject m_BeginMenu;

	[SerializeField]
	private MenuItemButton m_ContinueBtn;

	[SerializeField]
	private MenuItemButton m_ChaptersBtn;

	[SerializeField]
	private MenuItemButton m_LoadGameBtn;

	[Header("[SLOT MENU]")]
	[SerializeField]
	private GameObject m_SelectSlotMenu;

	[SerializeField]
	private MenuItemButton m_Slot01Btn;

	[SerializeField]
	private MenuItemButton m_Slot02Btn;

	[SerializeField]
	private MenuItemButton m_Slot03Btn;

	[SerializeField]
	private TextMeshProUGUI m_Slot01Label;

	[SerializeField]
	private TextMeshProUGUI m_Slot02Label;

	[SerializeField]
	private TextMeshProUGUI m_Slot03Label;

	[Header("[ARE YOU SURE]")]
	[SerializeField]
	private GameObject m_WarningMenu;

	[SerializeField]
	private TextMeshProUGUI m_AreYouSureWarningLbl;

	[SerializeField]
	private GameObject m_WarningControllerNav;

	[SerializeField]
	private GameObject m_WarningKBNav;

	[SerializeField]
	private MenuItemButton m_WarningYesBtn;

	[SerializeField]
	private MenuItemButton m_WarningNoBtn;

	[Header("CHAPTER SELECT MENU")]
	[SerializeField]
	private GameObject m_ChapterSelectMenu;

	[SerializeField]
	private Image m_CircleImage;

	[SerializeField]
	private Image m_CircleImageColor;

	[SerializeField]
	private BaseUIButton m_CircleBtn;

	[SerializeField]
	private Localize m_ChapterLbl;

	[SerializeField]
	private Localize m_ChapterNumberLbl;

	[SerializeField]
	private Localize m_ChapterTitleLbl;

	[SerializeField]
	private MenuItemArrows m_ChapterArrows;

	[SerializeField]
	private MenuItemChapterImage m_CH1Image;

	[SerializeField]
	private MenuItemChapterImage m_CH2Image;

	[SerializeField]
	private MenuItemChapterImage m_CH3Image;

	[SerializeField]
	private MenuItemChapterImage m_CH4Image;

	[SerializeField]
	private MenuItemChapterImage m_CH5Image;

	[SerializeField]
	private MenuItemChapterImage m_ArchivesImage;

	[Header("Store")]
	[SerializeField]
	private BaseUIButton m_StoreBtn;

	private SaveFileData m_SelectedSlot;

	private OptionsMenuController m_OptionsMenuController;

	private MenuItem m_CurrentMenuItem;

	private MenuItem m_PreviousMenuItem;

	private List<MenuItemButton> m_ActiveMenuItemButtons = new List<MenuItemButton>();

	private List<MenuItemButton> m_MainMenuItemButtons = new List<MenuItemButton>();

	private List<MenuItemButton> m_BeginMenuItemButtons = new List<MenuItemButton>();

	private List<MenuItemButton> m_SelectSlotMenuItemButtons = new List<MenuItemButton>();

	private List<MenuItemButton> m_WarningMenuItemButtons = new List<MenuItemButton>();

	private List<Image> m_BackgroundImages;

	private List<Image> m_ForegroundImages;

	private AudioClip m_WhooshClip;

	private AudioClip m_SketchesMusicClip;

	private int m_SelectedChapter;

	private int m_MaxChapters = 6;

	private int m_SelectedIndex;

	private int m_IndexMax = 3;

	private float m_SelectYTimeNext;

	private float m_SelectYTimeRate = 0.2f;

	private float m_SelectXTimeNext;

	private float m_SelectXTimeRate = 0.2f;

	private float m_ActiveAxisX;

	private float m_ActiveAxisY;

	private float m_AxisThreshold = 0.8f;

	public AudioObject TitleMusicAudioObject { get; private set; }

	public override void InitController(object _data)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		((Transform)m_Visuals).localScale = new Vector3(1f, 1f, 1f);
		GameManager.Instance.LockPause();
		GameManager.Instance.AudioManager.ListenerSetActive(active: true);
		m_WhooshClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Whoosh");
		m_SketchesMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH1/MUS_Title_Music_Sketches");
		GameManager.Instance.GameData.NoSaveFile = null;
		SetupMenus();
		InitializeChapterImages();
		ShowMainMenu();
		CheckForController();
		CheckInitialInput();
		m_SelectNav.Init(NavInputDataVO.Create("A", "MENU/CONTROL_SELECT"));
		m_BackNav.Init(NavInputDataVO.Create("B", "MENU/CONTROL_BACK"));
		m_ActiveMenuItemButtons = m_MainMenuItemButtons;
		((Graphic)m_BlackOutImage).color = new Color(0f, 0f, 0f, 1f);
		((Graphic)m_FullFlickerImage).color = new Color(0f, 0f, 0f, 0f);
		((Graphic)m_LightFlickerImage).color = new Color(1f, 1f, 1f, 0f);
		((Graphic)m_CircleImage).color = new Color(0f, 0f, 0f, 1f);
		((Behaviour)m_CircleImageColor).enabled = false;
		string Translation = "MENU/CHAPTER_COPY";
		if (LocalizationManager.TryGetTranslation("MENU/CHAPTER_COPY", out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			((Component)m_ChapterLbl).GetComponent<TextMeshProUGUI>().text = Translation.ToUpper();
		}
		m_VersionLabel.text = GameManager.Instance.GetProjectVersion();
		InputManager.OnDeviceAttached += HandleInputManagerOnDeviceAttached;
		InputManager.OnDeviceDetached += HandleInputManagerOnDeviceDetached;
		m_ChapterArrows.OnLeft += HandleChapterArrowOnLeft;
		m_ChapterArrows.OnRight += HandleChapterArrowOnRight;
		m_CircleBtn.OnEnter += HandleCircleBtnOnEnter;
		m_CircleBtn.OnExit += HandleCircleBtnOnExit;
		m_CircleBtn.OnClick += HandleCircleBtnOnClick;
		m_KBBackBtn.OnClick += HandleKBBackBtnOnClick;
		m_StoreBtn.gameObject.SetActive(false);
	}

	private void SetupMenus()
	{
		GetButtonData(ref m_BeginBtn, ref m_MainMenuItemButtons, "MENU/MENU_BEGIN", CheckMainMenuItem);
		GetButtonData(ref m_OptionsBtn, ref m_MainMenuItemButtons, "MENU/MENU_OPTIONS", CheckMainMenuItem);
		GetButtonData(ref m_QuitBtn, ref m_MainMenuItemButtons, "MENU/MENU_QUIT", CheckMainMenuItem);
		GetButtonData(ref m_Slot01Btn, ref m_SelectSlotMenuItemButtons, "SAVE 1", CheckSelectedSlotItem);
		GetButtonData(ref m_Slot02Btn, ref m_SelectSlotMenuItemButtons, "SAVE 2", CheckSelectedSlotItem);
		GetButtonData(ref m_Slot03Btn, ref m_SelectSlotMenuItemButtons, "SAVE 3", CheckSelectedSlotItem);
		m_Slot01Label.text = GetCurrentChapter(0);
		m_Slot02Label.text = GetCurrentChapter(1);
		m_Slot03Label.text = GetCurrentChapter(2);
		GetButtonData(ref m_ContinueBtn, ref m_BeginMenuItemButtons, "MENU/BEGIN_CONTINUE", CheckSelectedBeginMenu);
		GetButtonData(ref m_LoadGameBtn, ref m_BeginMenuItemButtons, "MENU/BEGIN_NEW_GAME", CheckSelectedBeginMenu);
		GetButtonData(ref m_ChaptersBtn, ref m_BeginMenuItemButtons, "MENU/BEGIN_SELECT_CHAPTER", CheckSelectedBeginMenu);
		GetButtonData(ref m_WarningYesBtn, ref m_WarningMenuItemButtons, "YES", WarningYes);
		GetButtonData(ref m_WarningNoBtn, ref m_WarningMenuItemButtons, "NO", WarningNo);
	}

	private void GetButtonData(ref MenuItemButton button, ref List<MenuItemButton> buttonList, string title, Action callback)
	{
		button.Init(MenuItemButtonDataVO.Create(title, callback));
		button.OnSelected += HandleMenuItemOnSelected;
		buttonList.Add(button);
	}

	private string GetCurrentChapter(int index)
	{
		SaveFileData saveFileData = GameManager.Instance.GameData.SaveFiles[index];
		string result = "- EMPTY -";
		if (saveFileData != null && saveFileData.PlayTime > 0f)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(saveFileData.PlayTime);
			result = "CHAPTER " + saveFileData.CurrentChapter + "  -  " + $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		}
		return result;
	}

	private void HandleKBBackBtnOnClick(object sender, EventArgs e)
	{
		GetBackFromCurrentMenuItem();
	}

	private void WarningYes()
	{
		HideWarningMenu();
		NewGame();
	}

	private void WarningNo()
	{
		HideWarningMenu();
	}

	private void HandleCircleBtnOnEnter(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)m_CircleImage).color = ((Graphic)m_CircleImageColor).color;
	}

	private void HandleCircleBtnOnExit(object sender, EventArgs e)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)m_CircleImage).color = new Color(0f, 0f, 0f, 1f);
	}

	private void HandleCircleBtnOnClick(object sender, EventArgs e)
	{
		CheckSelectedChapter();
	}

	private void HandleChapterArrowOnLeft(object sender, EventArgs e)
	{
		m_SelectedChapter--;
		if (m_SelectedChapter < 0)
		{
			m_SelectedChapter = m_MaxChapters - 1;
		}
		CheckChapterSelected();
	}

	private void HandleChapterArrowOnRight(object sender, EventArgs e)
	{
		m_SelectedChapter++;
		if (m_SelectedChapter >= m_MaxChapters)
		{
			m_SelectedChapter = 0;
		}
		CheckChapterSelected();
	}

	private void InitializeChapterImages()
	{
		m_ChapterLbl.SetTerm("MENU/CHAPTER_COPY");
		m_ChapterNumberLbl.SetTerm("MENU/CH1");
		m_ChapterTitleLbl.SetTerm("MENU/CH1_TITLE");
		DisableAllChapterImages();
		m_CH1Image.gameObject.SetActive(true);
	}

	private void DisableAllChapterImages()
	{
		m_CH1Image.gameObject.SetActive(false);
		m_CH2Image.gameObject.SetActive(false);
		m_CH3Image.gameObject.SetActive(false);
		m_CH4Image.gameObject.SetActive(false);
		m_CH5Image.gameObject.SetActive(false);
		m_ArchivesImage.gameObject.SetActive(false);
	}

	private void HandleInputManagerOnDeviceAttached(InputDevice obj)
	{
		CheckForController();
	}

	private void HandleInputManagerOnDeviceDetached(InputDevice obj)
	{
		CheckForController();
		for (int i = 0; i < m_ActiveMenuItemButtons.Count; i++)
		{
			m_ActiveMenuItemButtons[i].Button.OnPointerExit(null);
		}
	}

	private void CheckForController()
	{
		m_NavigationInput.SetActive(GameManager.Instance.HasController);
		m_NavicationKB.SetActive(!GameManager.Instance.HasController);
		m_WarningControllerNav.SetActive(GameManager.Instance.HasController);
		m_WarningKBNav.SetActive(!GameManager.Instance.HasController);
	}

	public void SetNavigationInput(bool hasSelect, bool hasBack)
	{
		m_SelectNav.gameObject.SetActive(hasSelect);
		m_BackNav.gameObject.SetActive(hasBack);
		m_KBBackBtn.gameObject.SetActive(hasBack);
	}

	public override void PlayIn()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		GameManager.Instance.AudioManager.Play(m_WhooshClip);
		TitleMusicAudioObject = GameManager.Instance.AudioManager.Play(m_SketchesMusicClip, AudioObjectType.MUSIC, -1);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_LightFlickerImage.DOFade(0.075f, 0.2f), (Ease)1), -1, (LoopType)1);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_FullFlickerImage.DOFade(0.05f, 0.1f), (Ease)7), -1, (LoopType)1);
		GameManager.Instance.HideScreenBlocker(0f);
		TweenSettingsExtensions.OnComplete<Tweener>(m_BlackOutImage.DOFade(0f, 0.5f), new TweenCallback(PlayInComplete));
	}

	public override void PlayInComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_game_at_main_menu");
		base.PlayInComplete();
	}

	private void CheckInitialInput()
	{
		if (GameManager.Instance.HasController)
		{
			Cursor.lockState = (CursorLockMode)1;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = (CursorLockMode)0;
			Cursor.visible = true;
		}
	}

	private void Update()
	{
		if (!GameManager.Instance.HasController || m_CurrentMenuItem == MenuItem.OPTIONS)
		{
			return;
		}
		if (InputUtil.GetInputY(out m_ActiveAxisY) && Time.unscaledTime > m_SelectYTimeNext)
		{
			m_SelectYTimeNext = Time.unscaledTime + m_SelectYTimeRate;
			if (m_ActiveAxisY > m_AxisThreshold)
			{
				m_SelectedIndex--;
			}
			else if (m_ActiveAxisY < 0f - m_AxisThreshold)
			{
				m_SelectedIndex++;
			}
			if (m_SelectedIndex >= m_IndexMax)
			{
				m_SelectedIndex = 0;
			}
			else if (m_SelectedIndex < 0)
			{
				m_SelectedIndex = m_IndexMax - 1;
			}
			CheckActiveMenuItemButtons();
		}
		if (m_CurrentMenuItem == MenuItem.CHAPTERS)
		{
			if (InputUtil.GetInputX(out m_ActiveAxisX) && Time.unscaledTime > m_SelectXTimeNext)
			{
				m_SelectXTimeNext = Time.unscaledTime + m_SelectXTimeRate;
				if (m_ActiveAxisX > m_AxisThreshold)
				{
					m_ChapterArrows.TriggerRight();
				}
				else if (m_ActiveAxisX < 0f - m_AxisThreshold)
				{
					m_ChapterArrows.TriggerLeft();
				}
			}
			if (m_ActiveAxisX > 0f - m_AxisThreshold && m_ActiveAxisX < m_AxisThreshold)
			{
				m_SelectXTimeNext = 0f;
			}
		}
		if (Input.GetAxis("Mouse Y") != 0f)
		{
			m_NavigationInput.SetActive(false);
			m_NavicationKB.SetActive(true);
			m_WarningControllerNav.SetActive(false);
			m_WarningKBNav.SetActive(true);
		}
		else if (GameManager.Instance.HasController && (InputManager.ActiveDevice.LeftStickY.Value != 0f || InputManager.ActiveDevice.DPadY.Value != 0f))
		{
			m_NavigationInput.SetActive(true);
			m_NavicationKB.SetActive(false);
			m_WarningControllerNav.SetActive(true);
			m_WarningKBNav.SetActive(false);
		}
		if (m_ActiveAxisY > 0f - m_AxisThreshold && m_ActiveAxisY < m_AxisThreshold)
		{
			m_SelectYTimeNext = 0f;
		}
		if (PlayerInput.Jump())
		{
			if (m_CurrentMenuItem == MenuItem.WARNING)
			{
				HideWarningMenu();
				NewGame();
			}
			else
			{
				CheckSelectedItem();
			}
		}
		else if (PlayerInput.BackOnPressed())
		{
			if (m_CurrentMenuItem == MenuItem.WARNING)
			{
				HideWarningMenu();
			}
			else
			{
				GetBackFromCurrentMenuItem();
			}
		}
	}

	private void GetBackFromCurrentMenuItem()
	{
		switch (m_CurrentMenuItem)
		{
		case MenuItem.SLOT:
			ShowMainMenu();
			break;
		case MenuItem.BEGIN:
			ShowSelectSlotMenu();
			break;
		case MenuItem.CHAPTERS:
			ShowBeginMenu();
			break;
		}
	}

	private void CheckSelectedItem()
	{
		switch (m_CurrentMenuItem)
		{
		case MenuItem.MAIN:
			CheckMainMenuItem();
			break;
		case MenuItem.SLOT:
			CheckSelectedSlotItem();
			break;
		case MenuItem.BEGIN:
			CheckSelectedBeginMenu();
			break;
		case MenuItem.CHAPTERS:
			CheckSelectedChapter();
			break;
		}
	}

	private void CheckActiveMenuItemButtons()
	{
		if (m_ActiveMenuItemButtons == null)
		{
			return;
		}
		for (int i = 0; i < m_ActiveMenuItemButtons.Count; i++)
		{
			if (i == m_SelectedIndex)
			{
				m_ActiveMenuItemButtons[i].Button.OnPointerEnter(null);
			}
			else
			{
				m_ActiveMenuItemButtons[i].Button.OnPointerExit(null);
			}
		}
	}

	private void HideMenus()
	{
		HideMainMenu();
		HideSelectSlotMenu();
		HideBeginMenu();
		HideChapterSelectMenu();
		HideWarningMenu();
	}

	private void ShowMainMenu()
	{
		HideMenus();
		SetNavigationInput(hasSelect: true, hasBack: false);
		m_CurrentMenuItem = MenuItem.MAIN;
		m_MainMenu.SetActive(true);
		m_ActiveMenuItemButtons = m_MainMenuItemButtons;
		m_SelectedIndex = 0;
		CheckActiveMenuItemButtons();
	}

	private void HideMainMenu()
	{
		m_MainMenu.SetActive(false);
	}

	private void CheckMainMenuItem()
	{
		switch (m_SelectedIndex)
		{
		case 0:
			ShowSelectSlotMenu();
			SetNavigationInput(hasSelect: true, hasBack: true);
			break;
		case 1:
			ShowOptionsMenu();
			break;
		case 2:
			GameManager.Instance.Quit();
			break;
		}
	}

	private void ShowOptionsMenu()
	{
		m_CurrentMenuItem = MenuItem.OPTIONS;
		m_OptionsMenuController = GameManager.Instance.UIManager.Show<OptionsMenuController>("UI/Menus/OptionsMenuController", "PAUSE");
		m_OptionsMenuController.OnPlayOutComplete += HandleOptionsMenuControllerOnPlayOutComplete;
		((Component)m_Visuals).gameObject.SetActive(false);
	}

	private void HandleOptionsMenuControllerOnPlayOutComplete(object sender, EventArgs e)
	{
		m_OptionsMenuController.OnPlayOutComplete -= HandleOptionsMenuControllerOnPlayOutComplete;
		m_OptionsMenuController = null;
		((Component)m_Visuals).gameObject.SetActive(true);
		m_CurrentMenuItem = MenuItem.MAIN;
	}

	private void ShowSelectSlotMenu()
	{
		HideMenus();
		SetNavigationInput(hasSelect: true, hasBack: true);
		m_CurrentMenuItem = MenuItem.SLOT;
		m_SelectSlotMenu.SetActive(true);
		m_ActiveMenuItemButtons = m_SelectSlotMenuItemButtons;
		m_SelectedIndex = 0;
		CheckActiveMenuItemButtons();
	}

	private void HideSelectSlotMenu()
	{
		m_SelectSlotMenu.SetActive(false);
	}

	private void CheckSelectedSlotItem()
	{
		SelectSlot(m_SelectedIndex);
	}

	private void SelectSlot(int index)
	{
		if (GameManager.Instance.GameData.SaveFiles[index] == null)
		{
			GameManager.Instance.GameData.SaveFiles[index] = new SaveFileData(index);
		}
		GameManager.Instance.GameData.CurrentSaveFile = GameManager.Instance.GameData.SaveFiles[index];
		m_SelectedSlot = GameManager.Instance.GameData.CurrentSaveFile;
		m_CH1Image.Init(isUnlocked: true);
		m_CH2Image.Init(m_SelectedSlot.CH1Data != null && m_SelectedSlot.CH1Data.IsChapterComplete);
		m_CH3Image.Init(m_SelectedSlot.CH2Data != null && m_SelectedSlot.CH2Data.IsChapterComplete);
		m_CH4Image.Init(m_SelectedSlot.CH3Data != null && m_SelectedSlot.CH3Data.IsChapterComplete);
		m_CH5Image.Init(m_SelectedSlot.CH4Data != null && m_SelectedSlot.CH4Data.IsChapterComplete);
		m_ArchivesImage.Init(m_SelectedSlot.IsNewGamePlus);
		if (m_SelectedSlot.PlayTime <= 0.001f)
		{
			m_ContinueBtn.gameObject.SetActive(false);
			if (m_BeginMenuItemButtons.Contains(m_ContinueBtn))
			{
				m_BeginMenuItemButtons.Remove(m_ContinueBtn);
			}
		}
		else
		{
			m_ContinueBtn.gameObject.SetActive(true);
			if (!m_BeginMenuItemButtons.Contains(m_ContinueBtn))
			{
				m_BeginMenuItemButtons.Insert(0, m_ContinueBtn);
			}
		}
		ShowBeginMenu();
	}

	private void ShowBeginMenu()
	{
		HideMenus();
		m_CurrentMenuItem = MenuItem.BEGIN;
		m_BeginMenu.SetActive(true);
		m_ActiveMenuItemButtons = m_BeginMenuItemButtons;
		m_SelectedIndex = 0;
		CheckActiveMenuItemButtons();
	}

	private void HideBeginMenu()
	{
		m_BeginMenu.SetActive(false);
	}

	private void CheckSelectedBeginMenu()
	{
		bool flag = m_SelectedSlot.PlayTime > 0.001f;
		switch (m_SelectedIndex)
		{
		case 0:
			if (flag)
			{
				Continue();
			}
			else
			{
				NewGame();
			}
			break;
		case 1:
			if (flag)
			{
				ShowWarningMenu();
			}
			else
			{
				ShowChapterSelectMenu();
			}
			break;
		case 2:
			ShowChapterSelectMenu();
			break;
		}
	}

	private void Continue()
	{
		switch (m_SelectedSlot.CurrentChapter)
		{
		case 1:
			LaunchChapter("CH1");
			break;
		case 2:
			LaunchChapter("CH2");
			break;
		case 3:
			LaunchChapter("CH3");
			break;
		case 4:
			LaunchChapter("CH4");
			break;
		case 5:
			LaunchChapter("CH5");
			break;
		}
	}

	private void NewGame()
	{
		int iD = m_SelectedSlot.ID;
		m_SelectedSlot = new SaveFileData(iD);
		m_SelectedSlot.CH1Data = new CH1DataVO();
		GameManager.Instance.GameData.CurrentSaveFile = m_SelectedSlot;
		GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
		StartNewGame();
	}

	private void StartNewGame()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		m_CurrentMenuItem = MenuItem.LAUNCHING;
		TweenSettingsExtensions.OnComplete<Tweener>(m_BlackOutImage.DOFade(1f, 0.5f), (TweenCallback)delegate
		{
			CH1IntroModalController cH1IntroModalController = GameManager.Instance.UIManager.Show<CH1IntroModalController>("UI/Modals/CH1IntroModalController", "MODAL", this);
			cH1IntroModalController.OnPlayOutComplete += HandleStartNewGameOnPlayOutComplete;
		});
	}

	private void HandleStartNewGameOnPlayOutComplete(object sender, EventArgs e)
	{
		(sender as CH1IntroModalController).OnPlayOutComplete += HandleStartNewGameOnPlayOutComplete;
		LaunchChapter("CH1");
	}

	private void ShowWarningMenu()
	{
		m_PreviousMenuItem = m_CurrentMenuItem;
		m_CurrentMenuItem = MenuItem.WARNING;
		m_WarningMenu.SetActive(true);
		m_ActiveMenuItemButtons = m_WarningMenuItemButtons;
	}

	private void HideWarningMenu()
	{
		m_CurrentMenuItem = m_PreviousMenuItem;
		m_WarningMenu.SetActive(false);
		m_WarningNoBtn.Deselect();
		m_WarningYesBtn.Deselect();
		m_ActiveMenuItemButtons = m_BeginMenuItemButtons;
		CheckActiveMenuItemButtons();
	}

	private void ShowChapterSelectMenu()
	{
		HideMenus();
		string Translation = "MENU/CHAPTER_COPY";
		if (LocalizationManager.TryGetTranslation("MENU/CHAPTER_COPY", out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			((Component)m_ChapterLbl).GetComponent<TextMeshProUGUI>().text = Translation.ToUpper();
		}
		UpdateChapterDetails("MENU/CH1", "MENU/CH1_TITLE");
		m_CurrentMenuItem = MenuItem.CHAPTERS;
		m_ChapterSelectMenu.SetActive(true);
		m_SelectedChapter = 0;
		DisableAllChapterImages();
		m_CH1Image.gameObject.SetActive(true);
		((Selectable)m_CircleBtn.Button).interactable = true;
	}

	private void HideChapterSelectMenu()
	{
		m_ChapterSelectMenu.SetActive(false);
	}

	private void CheckChapterSelected()
	{
		DisableAllChapterImages();
		switch (m_SelectedChapter)
		{
		case 0:
			m_CH1Image.gameObject.SetActive(true);
			UpdateChapterDetails("MENU/CH1", "MENU/CH1_TITLE");
			break;
		case 1:
			m_CH2Image.gameObject.SetActive(true);
			UpdateChapterDetails("MENU/CH2", "MENU/CH2_TITLE");
			break;
		case 2:
			m_CH3Image.gameObject.SetActive(true);
			UpdateChapterDetails("MENU/CH3", "MENU/CH3_TITLE");
			break;
		case 3:
			m_CH4Image.gameObject.SetActive(true);
			UpdateChapterDetails("MENU/CH4", "MENU/CH4_TITLE");
			break;
		case 4:
			m_CH5Image.gameObject.SetActive(true);
			UpdateChapterDetails("MENU/CH5", "MENU/CH5_TITLE");
			break;
		case 5:
			m_ArchivesImage.gameObject.SetActive(true);
			((Component)m_ChapterNumberLbl).GetComponent<TextMeshProUGUI>().text = "?";
			((Component)m_ChapterTitleLbl).GetComponent<TextMeshProUGUI>().text = "ARCHIVES";
			UpdateChapterDetails("?", "MENU/MENU_ARCHIVES");
			break;
		}
	}

	private void UpdateChapterDetails(string number, string title)
	{
		string Translation = number;
		if (LocalizationManager.TryGetTranslation(number, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			((Component)m_ChapterNumberLbl).GetComponent<TextMeshProUGUI>().text = Translation.ToUpper();
		}
		string Translation2 = title;
		if (LocalizationManager.TryGetTranslation(title, out Translation2, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			((Component)m_ChapterTitleLbl).GetComponent<TextMeshProUGUI>().text = Translation2.ToUpper();
		}
	}

	private void CheckSelectedChapter()
	{
		switch (m_SelectedChapter)
		{
		case 0:
			ResetChapterSelectDate();
			StartNewGame();
			break;
		case 1:
			if ((m_SelectedSlot.CH1Data != null && m_SelectedSlot.CH1Data.IsChapterComplete) || m_SelectedSlot.IsNewGamePlus)
			{
				ResetChapterSelectDate();
				LaunchChapter("CH2");
			}
			break;
		case 2:
			if ((m_SelectedSlot.CH2Data != null && m_SelectedSlot.CH2Data.IsChapterComplete) || m_SelectedSlot.IsNewGamePlus)
			{
				ResetChapterSelectDate();
				LaunchChapter("CH3");
			}
			break;
		case 3:
			if ((m_SelectedSlot.CH3Data != null && m_SelectedSlot.CH3Data.IsChapterComplete) || m_SelectedSlot.IsNewGamePlus)
			{
				ResetChapterSelectDate();
				LaunchChapter("CH4");
			}
			break;
		case 4:
			if ((m_SelectedSlot.CH4Data != null && m_SelectedSlot.CH4Data.IsChapterComplete) || m_SelectedSlot.IsNewGamePlus)
			{
				ResetChapterSelectDate();
				LaunchChapter("CH5");
			}
			break;
		case 5:
			if (m_SelectedSlot.IsNewGamePlus)
			{
				ResetChapterSelectDate();
				LaunchChapter("Archives");
			}
			break;
		}
	}

	private void ResetChapterSelectDate()
	{
		bool isNewGamePlus = m_SelectedSlot.IsNewGamePlus;
		GameManager.Instance.GameData.NoSaveFile = new SaveFileData(-1);
		m_SelectedSlot = GameManager.Instance.GameData.NoSaveFile;
		m_SelectedSlot.IsNewGamePlus = isNewGamePlus;
		m_SelectedSlot.HasDied = false;
		m_SelectedSlot.PlayTime = 0f;
		m_SelectedSlot.CH1Data = new CH1DataVO();
		m_SelectedSlot.CH2Data = new CH2DataVO();
		m_SelectedSlot.CH3Data = new CH3DataVO();
		m_SelectedSlot.CH4Data = new CH4DataVO();
		m_SelectedSlot.CH5Data = new CH5DataVO();
		GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
		GameManager.Instance.GameData.CurrentSaveFile = m_SelectedSlot;
	}

	private void LaunchChapter(string chapterName)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		m_CurrentMenuItem = MenuItem.LAUNCHING;
		m_BlackOutImage.DOFade(1f, 0.49f);
		TweenSettingsExtensions.OnComplete<Tweener>(TitleMusicAudioObject.AudioSource.DOFade(0f, 0.5f), (TweenCallback)delegate
		{
			TitleMusicAudioObject.Clear();
			TitleMusicAudioObject = null;
			GameManager.Instance.AudioManager.ListenerSetActive(active: false);
			GameManager.Instance.LoadScene(new GenericLoaderDataVO(chapterName));
			Kill();
		});
	}

	private void ShowURL(string url)
	{
		if ((Object)(object)GameManager.Instance.SteamManager != (Object)null && SteamUser.BLoggedOn())
		{
			SteamFriends.ActivateGameOverlayToWebPage(url);
		}
		else
		{
			Application.OpenURL(url);
		}
	}

	private void HandleMenuItemOnSelected(object sender, EventArgs e)
	{
		for (int i = 0; i < m_ActiveMenuItemButtons.Count; i++)
		{
			MenuItemButton menuItemButton = m_ActiveMenuItemButtons[i];
			if (menuItemButton.gameObject.activeSelf)
			{
				if (((object)menuItemButton).Equals(sender))
				{
					m_SelectedIndex = i;
				}
				else
				{
					menuItemButton.Deselect();
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_LightFlickerImage, false);
		m_WhooshClip = null;
		m_SketchesMusicClip = null;
		if ((Object)(object)TitleMusicAudioObject != (Object)null)
		{
			TitleMusicAudioObject.Clear();
			TitleMusicAudioObject = null;
		}
		if (m_ActiveMenuItemButtons != null)
		{
			for (int i = 0; i < m_ActiveMenuItemButtons.Count; i++)
			{
				m_ActiveMenuItemButtons[i].OnSelected -= HandleMenuItemOnSelected;
			}
			m_ActiveMenuItemButtons.Clear();
			m_ActiveMenuItemButtons = null;
		}
		base.OnDisposed();
	}
}
