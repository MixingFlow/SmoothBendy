using System;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using InControl;
using TMG.Controls;
using TMG.UI;
using TMG.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : BaseUIController
{
	private enum MenuItem
	{
		OPTIONS,
		SETTINGS,
		ADVANCED,
		EXTRAS
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

	[Header("< Images >")]
	[SerializeField]
	private Image m_BlackOutImage;

	[SerializeField]
	private Image m_LightFlickerImage;

	[SerializeField]
	private Image m_FullFlickerImage;

	[Header("[BEGIN MENU]")]
	[SerializeField]
	private GameObject m_OptionsMenu;

	[SerializeField]
	private MenuItemButton m_SettingsBtn;

	[SerializeField]
	private MenuItemButton m_AdvancedBtn;

	[Header("[SETTINGS MNEU]")]
	[SerializeField]
	private GameObject m_SettingsMenu;

	[SerializeField]
	private Transform m_SettingsMenuParent;

	[Header("[ADVANCED MENU]")]
	[SerializeField]
	private GameObject m_AdvancedMenu;

	[SerializeField]
	private Transform m_AdvancedMenuParent;

	[Header("<PREFABS>")]
	[SerializeField]
	private MenuItemOption m_MenuItemOptionPrefab;

	[SerializeField]
	private MenuItemCategory m_MenuItemCategoryPrefab;

	[SerializeField]
	private MenuItemEmpty m_MenuItemEmptyPrefab;

	private OptionsControls m_OptionControls;

	private MenuItem m_CurrentMenuItem;

	private List<MenuItemButton> m_ActiveMenuItemButtons = new List<MenuItemButton>();

	private List<MenuItemButton> m_OptionsMenuItemButtons = new List<MenuItemButton>();

	private List<MenuItemOption> m_SettingsMenuOptions = new List<MenuItemOption>();

	private List<MenuItemOption> m_AdvancedMenuOptions = new List<MenuItemOption>();

	private int m_SelectedIndex;

	private int m_IndexMax = 2;

	private float m_SelectYTimeNext;

	private float m_SelectYTimeRate = 0.15f;

	private float m_SelectXTimeNext;

	private float m_SelectXTimeRate = 0.15f;

	private float m_ActiveAxisX;

	private float m_ActiveAxisY;

	private float m_AxisThreshold = 0.8f;

	public override void InitController(object _data)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		m_OptionControls = new OptionsControls();
		m_OptionControls.Initialize();
		((Transform)m_Visuals).localScale = new Vector3(1f, 1f, 1f);
		((Graphic)m_BlackOutImage).color = new Color(0f, 0f, 0f, 1f);
		((Graphic)m_FullFlickerImage).color = new Color(0f, 0f, 0f, 0f);
		((Graphic)m_LightFlickerImage).color = new Color(1f, 1f, 1f, 0f);
		GenerateOptionsMenu();
		GenerateSettingsMenu();
		GenerateAdvancedMenu();
		m_ActiveMenuItemButtons = m_OptionsMenuItemButtons;
		m_MenuItemOptionPrefab.gameObject.SetActive(false);
		m_MenuItemCategoryPrefab.gameObject.SetActive(false);
		m_MenuItemEmptyPrefab.gameObject.SetActive(false);
		ShowOptionsMenu();
		CheckForController();
		CheckInitialInput();
		m_SelectNav.Init(NavInputDataVO.Create("A", "MENU/CONTROL_SELECT"));
		m_BackNav.Init(NavInputDataVO.Create("B", "MENU/CONTROL_BACK"));
		if (GameManager.Instance.HasController)
		{
			m_SettingsBtn.Button.OnPointerEnter(null);
			m_AdvancedBtn.Button.OnPointerExit(null);
		}
		InputManager.OnDeviceAttached += HandleInputManagerOnDeviceAttached;
		InputManager.OnDeviceDetached += HandleInputManagerOnDeviceDetached;
		m_KBBackBtn.OnClick += HandleKBBackBtnOnClick;
	}

	public override void PlayIn()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_LightFlickerImage.DOFade(0.075f, 0.2f), (Ease)1), -1, (LoopType)1), (UpdateType)0, true);
		TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_FullFlickerImage.DOFade(0.05f, 0.1f), (Ease)7), -1, (LoopType)1), (UpdateType)0, true);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(m_BlackOutImage.DOFade(0f, 0.25f), (UpdateType)0, true), new TweenCallback(PlayInComplete));
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

	private void GenerateOptionsMenu()
	{
		GetButtonData(ref m_SettingsBtn, ref m_OptionsMenuItemButtons, "MENU/MENU_SETTINGS", CheckSelectedItem);
		GetButtonData(ref m_AdvancedBtn, ref m_OptionsMenuItemButtons, "MENU/MENU_ADVANCED", CheckSelectedItem);
	}

	private void GenerateAdvancedMenu()
	{
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_FULLSCREEN", (!GameManager.Instance.PlayerSettings.Fullscreen) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_RESOLUTION", m_OptionControls.HandleResolutionValeChange(GameManager.Instance.PlayerSettings.CurrentResolution), hasTranslation: false);
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "FIELD OF VIEW", GetFoVValue(GameManager.Instance.PlayerSettings.FoV), hasTranslation: false);
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVNACED_QUALITY", GetQuality());
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_AA", (!GameManager.Instance.PlayerSettings.AA) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_VSYNC", (!GameManager.Instance.PlayerSettings.VSync) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_DOF", (!GameManager.Instance.PlayerSettings.DoF) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_BLOOM", (!GameManager.Instance.PlayerSettings.Bloom) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_AO", (!GameManager.Instance.PlayerSettings.AmbientOcclusion) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_MOTION_BLUR", (!GameManager.Instance.PlayerSettings.MotionBlur) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "MENU/ADVANCED_FILM_GRAIN", (!GameManager.Instance.PlayerSettings.Grain) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "FLICKERING LIGHTS", (!GameManager.Instance.PlayerSettings.flickerLights) ? "OFF" : "ON", hasTranslation: false);
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "CROSSHAIR SCALE", GetCrosshairScaleValue(GameManager.Instance.PlayerSettings.CrosshairScale), hasTranslation: false);
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "CROSSHAIR OPACITY", GetCrosshairOpacityValue(GameManager.Instance.PlayerSettings.CrosshairOpacity), hasTranslation: false);
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "WEAPON SCALE", GetWeaponScaleValue(GameManager.Instance.PlayerSettings.weaponScale), hasTranslation: false);
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "BENDY AGGRESSION", GetWeaponScaleValue(GameManager.Instance.PlayerSettings.BendyAggressionScale), hasTranslation: false);
		CreateOption(ref m_AdvancedMenuOptions, m_AdvancedMenuParent, "FORCE HALLOWEEN", (!GameManager.Instance.PlayerSettings.forceHalloween) ? "OFF" : "ON", hasTranslation: false);
		Transform val = base.transform.Find("Visuals");
		if (!((Object)(object)val != (Object)null))
		{
			return;
		}
		Transform val2 = val.Find("AdvancedMenu");
		if (!((Object)(object)val2 != (Object)null))
		{
			return;
		}
		Transform val3 = val2.Find("Navigation");
		if ((Object)(object)val3 != (Object)null)
		{
			RectTransform component = ((Component)val3).GetComponent<RectTransform>();
			if ((Object)(object)component != (Object)null)
			{
				component.anchoredPosition = new Vector2(0f, 70f);
			}
		}
	}

	private string GetQuality()
	{
		if (GameManager.Instance.PlayerSettings.currentQuality == 0)
		{
			return "MENU/QUALITY_VERY_LOW";
		}
		if (GameManager.Instance.PlayerSettings.currentQuality == 1)
		{
			return "MENU/QUALITY_LOW";
		}
		if (GameManager.Instance.PlayerSettings.currentQuality == 2)
		{
			return "MENU/QUALITY_MEDIUM";
		}
		_ = GameManager.Instance.PlayerSettings.currentQuality;
		_ = 3;
		return "MENU/QUALITY_HIGH";
	}

	private void GenerateSettingsMenu()
	{
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		GenerateCategory(m_SettingsMenuParent, "Audio");
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MODE", GetAudioType());
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_MASTER", GetNumberValue(GameManager.Instance.PlayerSettings.Volume), hasTranslation: false);
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_MUSIC", GetNumberValue(GameManager.Instance.PlayerSettings.MusicVolume), hasTranslation: false);
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_SFX", GetNumberValue(GameManager.Instance.PlayerSettings.SFXVolume), hasTranslation: false);
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_DIA", GetNumberValue(GameManager.Instance.PlayerSettings.DialogueVolume), hasTranslation: false);
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_SUBTITLES", (!GameManager.Instance.PlayerSettings.Subtitles) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_LANGUAGE", GetLanguageValue(GameManager.Instance.PlayerSettings.Language).ToUpper());
		CreateEmpty(m_SettingsMenuParent);
		GenerateCategory(m_SettingsMenuParent, "Controls");
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_SENSITIVITY", GetNumberValue(GameManager.Instance.PlayerSettings.Sensitivity), hasTranslation: false);
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_INVERTED", (!GameManager.Instance.PlayerSettings.Inverted) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "Toggle Run", (!GameManager.Instance.PlayerSettings.ToggleRun) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_TIPS", (!GameManager.Instance.PlayerSettings.Tips) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateEmpty(m_SettingsMenuParent);
		GenerateCategory(m_SettingsMenuParent, "GRAPHICS");
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_BRIGHTNESS", GetNumberValue(GameManager.Instance.PlayerSettings.Brightness), hasTranslation: false);
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_CAMERA", (!GameManager.Instance.PlayerSettings.ViewBobbing) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "VIEW SWAYING", (!GameManager.Instance.PlayerSettings.ViewSwaying) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		CreateOption(ref m_SettingsMenuOptions, m_SettingsMenuParent, "MENU/SETTINGS_CROSSHAIR", (!GameManager.Instance.PlayerSettings.Crosshair) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
		Transform val = base.transform.Find("Visuals");
		if (!((Object)(object)val != (Object)null))
		{
			return;
		}
		Transform val2 = val.Find("SettingsMenu");
		if ((Object)(object)val2 != (Object)null)
		{
			Transform val3 = val2.Find("Navigation");
			if ((Object)(object)val3 != (Object)null)
			{
				val3.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			}
		}
	}

	private string GetNumberValue(float value)
	{
		return (value * 100f).ToString("N0");
	}

	private string GetLanguageValue(string language)
	{
		return LocalizationManager.GetLanguageFromCode(language);
	}

	private void CreateEmpty(Transform parent)
	{
		Object.Instantiate<MenuItemEmpty>(m_MenuItemEmptyPrefab).SetParentAndAlignWithScale(parent);
	}

	private void GenerateCategory(Transform parent, string label)
	{
		Object.Instantiate<MenuItemCategory>(m_MenuItemCategoryPrefab).Init(parent, label);
	}

	private void CreateOption(ref List<MenuItemOption> optionsList, Transform parent, string label, string status, bool hasTranslation = true)
	{
		MenuItemOption menuItemOption = Object.Instantiate<MenuItemOption>(m_MenuItemOptionPrefab);
		menuItemOption.OnLeft += HandleOptionOnLeft;
		menuItemOption.OnRight += HandleOptionOnRight;
		menuItemOption.Init(parent, label, status, hasTranslation);
		optionsList.Add(menuItemOption);
	}

	private void HandleOptionOnLeft(object sender, EventArgs e)
	{
		MenuItemOption menuItemOption = sender as MenuItemOption;
		if (Object.op_Implicit((Object)(object)menuItemOption))
		{
			CheckOptionsDirection(menuItemOption, isRight: false);
		}
	}

	private void HandleOptionOnRight(object sender, EventArgs e)
	{
		MenuItemOption menuItemOption = sender as MenuItemOption;
		if (Object.op_Implicit((Object)(object)menuItemOption))
		{
			CheckOptionsDirection(menuItemOption, isRight: true);
		}
	}

	private void CheckOptionsDirection(MenuItemOption option, bool isRight)
	{
		if (m_SettingsMenuOptions.Contains(option))
		{
			m_SelectedIndex = m_SettingsMenuOptions.IndexOf(option);
			CheckSettingsMenuItem(isRight);
		}
		else if (m_AdvancedMenuOptions.Contains(option))
		{
			m_SelectedIndex = m_AdvancedMenuOptions.IndexOf(option);
			CheckAdvancedMenuItem(isRight);
		}
	}

	private void Update()
	{
		if (!GameManager.Instance.HasController)
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
		if (m_CurrentMenuItem == MenuItem.SETTINGS || m_CurrentMenuItem == MenuItem.ADVANCED)
		{
			if (InputUtil.GetInputX(out m_ActiveAxisX) && Time.unscaledTime > m_SelectXTimeNext)
			{
				m_SelectXTimeNext = Time.unscaledTime + m_SelectXTimeRate;
				if (m_ActiveAxisX > m_AxisThreshold)
				{
					if (m_CurrentMenuItem == MenuItem.SETTINGS)
					{
						CheckSettingsMenuItem(isRight: true);
					}
					else if (m_CurrentMenuItem == MenuItem.ADVANCED)
					{
						CheckAdvancedMenuItem(isRight: true);
					}
				}
				else if (m_ActiveAxisX < 0f - m_AxisThreshold)
				{
					if (m_CurrentMenuItem == MenuItem.SETTINGS)
					{
						CheckSettingsMenuItem();
					}
					else if (m_CurrentMenuItem == MenuItem.ADVANCED)
					{
						CheckAdvancedMenuItem(isRight: false);
					}
				}
				if (m_ActiveAxisX > 0f - m_AxisThreshold && m_ActiveAxisX < m_AxisThreshold)
				{
					m_SelectXTimeNext = 0f;
				}
			}
		}
		else if (PlayerInput.Jump())
		{
			CheckSelectedItem();
		}
		if (Input.GetAxis("Mouse Y") != 0f)
		{
			m_NavigationInput.SetActive(false);
			m_NavicationKB.SetActive(true);
		}
		else if (GameManager.Instance.HasController && (InputManager.ActiveDevice.LeftStickY.Value != 0f || InputManager.ActiveDevice.DPadY.Value != 0f))
		{
			m_NavigationInput.SetActive(true);
			m_NavicationKB.SetActive(false);
		}
		if (m_ActiveAxisY > 0f - m_AxisThreshold && m_ActiveAxisY < m_AxisThreshold)
		{
			m_SelectYTimeNext = 0f;
		}
		if (PlayerInput.BackOnPressed())
		{
			GetBackFromCurrentMenuItem();
		}
	}

	private void DisableAllMenus()
	{
		m_OptionsMenu.SetActive(false);
		m_SettingsMenu.SetActive(false);
		m_AdvancedMenu.SetActive(false);
	}

	private void ShowOptionsMenu()
	{
		DisableAllMenus();
		m_OptionsMenu.SetActive(true);
		SetNavigationInput(hasSelect: true, hasBack: true);
		m_CurrentMenuItem = MenuItem.OPTIONS;
		m_SelectedIndex = 0;
		m_IndexMax = 2;
	}

	private void ShowSettingsMenu()
	{
		DisableAllMenus();
		m_SettingsMenu.SetActive(true);
		SetNavigationInput(hasSelect: true, hasBack: true);
		m_CurrentMenuItem = MenuItem.SETTINGS;
		m_SelectedIndex = 0;
		m_IndexMax = m_SettingsMenuOptions.Count;
	}

	private void ShowAdvancedMenu()
	{
		DisableAllMenus();
		m_AdvancedMenu.SetActive(true);
		SetNavigationInput(hasSelect: true, hasBack: true);
		m_CurrentMenuItem = MenuItem.ADVANCED;
		m_SelectedIndex = 0;
		m_IndexMax = m_AdvancedMenuOptions.Count;
	}

	private void HandleKBBackBtnOnClick(object sender, EventArgs e)
	{
		GetBackFromCurrentMenuItem();
	}

	private void GetBackFromCurrentMenuItem()
	{
		MenuItem currentMenuItem = m_CurrentMenuItem;
		switch (currentMenuItem)
		{
		case MenuItem.OPTIONS:
			Kill();
			return;
		case MenuItem.SETTINGS:
			m_SelectedIndex = 0;
			break;
		}
		if (currentMenuItem == MenuItem.ADVANCED)
		{
			m_SelectedIndex = 1;
		}
		ShowOptionsMenu();
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
	}

	public void SetNavigationInput(bool hasSelect, bool hasBack)
	{
		m_SelectNav.gameObject.SetActive(hasSelect);
		m_BackNav.gameObject.SetActive(hasBack);
		m_KBBackBtn.gameObject.SetActive(hasBack);
	}

	private void CheckSelectedItem()
	{
		if (m_CurrentMenuItem == MenuItem.OPTIONS)
		{
			CheckOptionsMenuItem();
		}
	}

	private void CheckOptionsMenuItem()
	{
		switch (m_SelectedIndex)
		{
		case 1:
			ShowAdvancedMenu();
			break;
		case 0:
			ShowSettingsMenu();
			break;
		}
	}

	private void CheckSettingsMenuItem(bool isRight = false)
	{
		float value = ((!isRight) ? (-0.05f) : 0.05f);
		int num = (isRight ? 1 : (-1));
		switch (m_SelectedIndex)
		{
		case 0:
		{
			int num2 = GameManager.Instance.PlayerSettings.currentAudioType + num;
			if (num2 < 0)
			{
				num2 = 5;
			}
			else if (num2 > 5)
			{
				num2 = 0;
			}
			m_SettingsMenuOptions[0].UpdateValue(m_OptionControls.UpdateAudioType(num2));
			break;
		}
		case 1:
			m_SettingsMenuOptions[1].UpdateValue(m_OptionControls.HandleMasterVolumeValueChange(value));
			break;
		case 2:
			m_SettingsMenuOptions[2].UpdateValue(m_OptionControls.HandleMusicVolumeValueChange(value));
			break;
		case 3:
			m_SettingsMenuOptions[3].UpdateValue(m_OptionControls.HandleSFXVolumeValueChange(value));
			break;
		case 4:
			m_SettingsMenuOptions[4].UpdateValue(m_OptionControls.HandleDialogueVolumeValueChange(value));
			break;
		case 5:
			m_OptionControls.HandleSubtitleValueChange();
			m_SettingsMenuOptions[5].UpdateValue((!GameManager.Instance.PlayerSettings.Subtitles) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 6:
			m_SettingsMenuOptions[6].UpdateValue(m_OptionControls.HandleLanguageValueChange(isRight), isLanguage: true);
			break;
		case 7:
			m_SettingsMenuOptions[7].UpdateValue(m_OptionControls.HandleSensitivityValueChange(value));
			break;
		case 8:
			m_OptionControls.HandleInvertedValueChange();
			m_SettingsMenuOptions[8].UpdateValue((!GameManager.Instance.PlayerSettings.Inverted) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 9:
			m_OptionControls.HandleToggleRunValueChange();
			m_SettingsMenuOptions[9].UpdateValue((!GameManager.Instance.PlayerSettings.ToggleRun) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 10:
			m_OptionControls.HandleTipsValueChange();
			m_SettingsMenuOptions[10].UpdateValue((!GameManager.Instance.PlayerSettings.Tips) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 11:
			m_SettingsMenuOptions[11].UpdateValue(m_OptionControls.HandleBrightnessValueChange(value));
			break;
		case 12:
			m_OptionControls.HandleViewBobbingValueChange();
			m_SettingsMenuOptions[12].UpdateValue((!GameManager.Instance.PlayerSettings.ViewBobbing) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 13:
			m_OptionControls.HandleViewSwayingValueChange();
			m_SettingsMenuOptions[13].UpdateValue((!GameManager.Instance.PlayerSettings.ViewSwaying) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 14:
			m_OptionControls.HandleCrosshairValueChange();
			m_SettingsMenuOptions[14].UpdateValue((!GameManager.Instance.PlayerSettings.Crosshair) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		}
	}

	private void CheckAdvancedMenuItem(bool isRight)
	{
		int num = (isRight ? 1 : (-1));
		switch (m_SelectedIndex)
		{
		case 0:
			m_OptionControls.HandleFullscreenValueChange();
			m_AdvancedMenuOptions[0].UpdateValue((!GameManager.Instance.PlayerSettings.Fullscreen) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 1:
		{
			int num3 = GameManager.Instance.PlayerSettings.CurrentResolution + num;
			if (num3 < 0)
			{
				num3 = GameManager.Instance.PlayerSettings.Resolutions.Count - 1;
			}
			else if (num3 >= GameManager.Instance.PlayerSettings.Resolutions.Count)
			{
				num3 = 0;
			}
			m_AdvancedMenuOptions[1].UpdateValue(m_OptionControls.HandleResolutionValeChange(num3));
			break;
		}
		case 2:
			m_AdvancedMenuOptions[2].UpdateValue(m_OptionControls.HandleFoVValueChange(num));
			break;
		case 3:
		{
			int num2 = GameManager.Instance.PlayerSettings.currentQuality + num;
			if (num2 < 0)
			{
				num2 = 3;
			}
			else if (num2 > 3)
			{
				num2 = 0;
			}
			m_AdvancedMenuOptions[3].UpdateValue(m_OptionControls.UpdateQuality(num2));
			break;
		}
		case 4:
			m_OptionControls.HandleAAValueChange();
			m_AdvancedMenuOptions[4].UpdateValue((!GameManager.Instance.PlayerSettings.AA) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 5:
			m_OptionControls.HandleVSyncValueChange();
			m_AdvancedMenuOptions[5].UpdateValue((!GameManager.Instance.PlayerSettings.VSync) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			QualitySettings.vSyncCount = (GameManager.Instance.PlayerSettings.VSync ? 1 : 0);
			if (QualitySettings.vSyncCount == 0)
			{
				Application.targetFrameRate = -1;
			}
			else
			{
				Application.targetFrameRate = 60;
			}
			break;
		case 6:
			m_OptionControls.HandleDOFValueChange();
			m_AdvancedMenuOptions[6].UpdateValue((!GameManager.Instance.PlayerSettings.DoF) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 7:
			m_OptionControls.HandleBloomValueChange();
			m_AdvancedMenuOptions[7].UpdateValue((!GameManager.Instance.PlayerSettings.Bloom) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 8:
			m_OptionControls.HandleAmbientOcclusionValueChange();
			m_AdvancedMenuOptions[8].UpdateValue((!GameManager.Instance.PlayerSettings.AmbientOcclusion) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 9:
			m_OptionControls.HandleMotionBlurValueChange();
			m_AdvancedMenuOptions[9].UpdateValue((!GameManager.Instance.PlayerSettings.MotionBlur) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 10:
			m_OptionControls.HandleGrainValueChange();
			m_AdvancedMenuOptions[10].UpdateValue((!GameManager.Instance.PlayerSettings.Grain) ? "MENU/SETTINGS_OFF" : "MENU/SETTINGS_ON");
			break;
		case 11:
			m_OptionControls.HandleFlickerDisableValueChange();
			m_AdvancedMenuOptions[11].UpdateValue((!GameManager.Instance.PlayerSettings.flickerLights) ? "OFF" : "ON");
			break;
		case 12:
			m_AdvancedMenuOptions[12].UpdateValue(m_OptionControls.HandleCrosshairScaleValueChange(num));
			break;
		case 13:
			m_AdvancedMenuOptions[13].UpdateValue(m_OptionControls.HandleCrosshairOpacityValueChange(num));
			break;
		case 14:
			m_AdvancedMenuOptions[14].UpdateValue(m_OptionControls.HandleWeaponScaleValueChange(num));
			break;
		case 15:
			m_AdvancedMenuOptions[15].UpdateValue(m_OptionControls.HandleBendyAggressionValueChange(num));
			break;
		case 16:
			m_OptionControls.HandleForceHalloweenValueChange();
			m_AdvancedMenuOptions[16].UpdateValue((!GameManager.Instance.PlayerSettings.forceHalloween) ? "OFF" : "ON");
			break;
		}
	}

	private void CheckActiveMenuItemButtons()
	{
		if (m_CurrentMenuItem == MenuItem.SETTINGS)
		{
			for (int i = 0; i < m_SettingsMenuOptions.Count; i++)
			{
				if (i == m_SelectedIndex)
				{
					m_SettingsMenuOptions[i].HandleButtonOnEnter(null, null);
				}
				else
				{
					m_SettingsMenuOptions[i].HandleButtonOnExit(null, null);
				}
			}
		}
		else if (m_CurrentMenuItem == MenuItem.ADVANCED)
		{
			for (int j = 0; j < m_AdvancedMenuOptions.Count; j++)
			{
				if (j == m_SelectedIndex)
				{
					m_AdvancedMenuOptions[j].HandleButtonOnEnter(null, null);
				}
				else
				{
					m_AdvancedMenuOptions[j].HandleButtonOnExit(null, null);
				}
			}
		}
		else
		{
			if (m_ActiveMenuItemButtons == null)
			{
				return;
			}
			for (int k = 0; k < m_ActiveMenuItemButtons.Count; k++)
			{
				if (k == m_SelectedIndex)
				{
					m_ActiveMenuItemButtons[k].Button.OnPointerEnter(null);
				}
				else
				{
					m_ActiveMenuItemButtons[k].Button.OnPointerExit(null);
				}
			}
		}
	}

	private void GetButtonData(ref MenuItemButton button, ref List<MenuItemButton> buttonList, string title, Action callback)
	{
		button.Init(MenuItemButtonDataVO.Create(title, callback));
		button.OnSelected += HandleMenuItemOnSelected;
		buttonList.Add(button);
	}

	private void HandleMenuItemOnSelected(object sender, EventArgs e)
	{
		for (int i = 0; i < m_ActiveMenuItemButtons.Count; i++)
		{
			MenuItemButton menuItemButton = m_ActiveMenuItemButtons[i];
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

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_LightFlickerImage, false);
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

	private string GetFoVValue(float value)
	{
		return value.ToString("N0");
	}

	private string GetCrosshairScaleValue(float value)
	{
		return value.ToString("N1");
	}

	private string GetCrosshairOpacityValue(float value)
	{
		return value.ToString("N1");
	}

	private string GetAudioType()
	{
		if (GameManager.Instance.PlayerSettings.currentAudioType == 0)
		{
			return "MONO";
		}
		if (GameManager.Instance.PlayerSettings.currentAudioType == 1)
		{
			return "STER";
		}
		if (GameManager.Instance.PlayerSettings.currentAudioType == 2)
		{
			return "QUAD";
		}
		if (GameManager.Instance.PlayerSettings.currentAudioType == 3)
		{
			return "SURR";
		}
		if (GameManager.Instance.PlayerSettings.currentAudioType == 4)
		{
			return "5.1";
		}
		if (GameManager.Instance.PlayerSettings.currentAudioType == 5)
		{
			return "7.1";
		}
		_ = GameManager.Instance.PlayerSettings.currentAudioType;
		return "STER";
	}

	private string GetWeaponScaleValue(float value)
	{
		return value.ToString("N1");
	}
}
