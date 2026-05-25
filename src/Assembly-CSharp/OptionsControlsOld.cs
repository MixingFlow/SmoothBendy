using System;
using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using TMG.UI.Controls;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsControlsOld : TMGMonoBehaviour
{
	private enum QualityLevel
	{
		LOW,
		MEDIUM,
		HIGH
	}

	[Header("Canvas")]
	[SerializeField]
	private CanvasGroup m_Visuals;

	[Header("Controls")]
	[SerializeField]
	private Slider m_SensitivitySlider;

	[SerializeField]
	private Toggle m_InvertedToggle;

	[SerializeField]
	private Toggle m_CrosshairToggle;

	[SerializeField]
	private BaseUIButton m_KeyBindingsButton;

	[SerializeField]
	private MenuItemButton m_OptionsButton;

	[SerializeField]
	private GameObject m_OptionsPrompt;

	[SerializeField]
	private GameObject m_KeyBindingsPrompt;

	[Header("Audio")]
	[SerializeField]
	private Slider m_MasterVolumeSlider;

	[SerializeField]
	private Slider m_MusicVolumeSlider;

	[SerializeField]
	private Slider m_SFXVolumeSlider;

	[SerializeField]
	private Slider m_DialogueVolumeSlider;

	[SerializeField]
	private Toggle m_SubtitlesToggle;

	[Header("Graphics")]
	[SerializeField]
	private Toggle m_QualityLowToggle;

	[SerializeField]
	private Toggle m_QualityMediumToggle;

	[SerializeField]
	private Toggle m_QualityHighToggle;

	[SerializeField]
	private BaseUGUIDropdown m_ResolutionDropdown;

	[SerializeField]
	private Slider m_BrightnessSlider;

	[SerializeField]
	private Toggle m_FullscreenToggle;

	[SerializeField]
	private Toggle m_AAToggle;

	[SerializeField]
	private Toggle m_VSyncToggle;

	[SerializeField]
	private Toggle m_DepthBlurToggle;

	[SerializeField]
	private Toggle m_BloomToggle;

	[SerializeField]
	private Toggle m_VolumetricLightToggle;

	[SerializeField]
	private Toggle m_AmbientOcclusionToggle;

	[SerializeField]
	private Toggle m_GrainToggle;

	[SerializeField]
	private Toggle m_MotionBlurToggle;

	[SerializeField]
	private Toggle m_FogToggle;

	[SerializeField]
	private Toggle m_ShadowsToggle;

	[SerializeField]
	private Toggle m_ViewBobbingToggle;

	[SerializeField]
	private Toggle m_ViewSwayingToggle;

	[SerializeField]
	private Toggle m_DustParticlesToggle;

	[Header("Custom Options")]
	[SerializeField]
	private GameObject m_PreviousButton;

	private QualityLevel m_CurrentQuality;

	private const string MASTER_VOLUME = "Master";

	private const string SFX_VOLUME = "Effects";

	private const string MUSIC_VOLUME = "Music";

	private const string DIALOGUE_VOLUME = "Dialogue";

	public void Initialize()
	{
		m_Visuals.alpha = 0f;
		m_OptionsPrompt.SetActive(false);
		m_KeyBindingsPrompt.SetActive(false);
		((Component)m_Visuals).gameObject.SetActive(false);
		m_SensitivitySlider.value = GameManager.Instance.PlayerSettings.Sensitivity;
		m_InvertedToggle.isOn = GameManager.Instance.PlayerSettings.Inverted;
		m_CrosshairToggle.isOn = GameManager.Instance.PlayerSettings.Crosshair;
		m_MasterVolumeSlider.value = GameManager.Instance.PlayerSettings.Volume;
		m_MusicVolumeSlider.value = GameManager.Instance.PlayerSettings.MusicVolume;
		m_SFXVolumeSlider.value = GameManager.Instance.PlayerSettings.SFXVolume;
		m_DialogueVolumeSlider.value = GameManager.Instance.PlayerSettings.DialogueVolume;
		m_SubtitlesToggle.isOn = GameManager.Instance.PlayerSettings.Subtitles;
		m_BrightnessSlider.value = GameManager.Instance.PlayerSettings.Brightness;
		m_FullscreenToggle.isOn = GameManager.Instance.PlayerSettings.Fullscreen;
		m_AAToggle.isOn = GameManager.Instance.PlayerSettings.AA;
		m_VSyncToggle.isOn = GameManager.Instance.PlayerSettings.VSync;
		m_DepthBlurToggle.isOn = GameManager.Instance.PlayerSettings.DoF;
		m_BloomToggle.isOn = GameManager.Instance.PlayerSettings.Bloom;
		m_VolumetricLightToggle.isOn = GameManager.Instance.PlayerSettings.VolumetricLighting;
		m_AmbientOcclusionToggle.isOn = GameManager.Instance.PlayerSettings.AmbientOcclusion;
		m_GrainToggle.isOn = GameManager.Instance.PlayerSettings.Grain;
		m_MotionBlurToggle.isOn = GameManager.Instance.PlayerSettings.MotionBlur;
		m_FogToggle.isOn = GameManager.Instance.PlayerSettings.Fog;
		m_ShadowsToggle.isOn = GameManager.Instance.PlayerSettings.Shadows;
		m_ViewBobbingToggle.isOn = GameManager.Instance.PlayerSettings.ViewBobbing;
		m_ViewSwayingToggle.isOn = GameManager.Instance.PlayerSettings.ViewSwaying;
		m_DustParticlesToggle.isOn = GameManager.Instance.PlayerSettings.DustParticles;
		AudioListener.volume = GameManager.Instance.PlayerSettings.Volume;
		InitQualityToggles();
		InitResolutionDropdown();
		AddListeners();
		if (!((Object)(object)m_PreviousButton != (Object)null))
		{
			m_OptionsButton.InitBasic();
		}
	}

	public void Show()
	{
		m_OptionsPrompt.SetActive(true);
		((Component)m_Visuals).gameObject.SetActive(true);
		m_Visuals.alpha = 0f;
		ShortcutExtensions.DOKill((Component)(object)m_Visuals, false);
		TweenSettingsExtensions.SetUpdate<Tweener>(m_Visuals.DOFade(1f, 0.25f), (UpdateType)0);
	}

	public void Hide()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)m_Visuals, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(m_Visuals.DOFade(0f, 0.25f), (UpdateType)0), new TweenCallback(HideOnComplete));
	}

	private void HideOnComplete()
	{
		((Component)m_Visuals).gameObject.SetActive(false);
		m_OptionsPrompt.SetActive(false);
		m_Visuals.alpha = 0f;
	}

	private void HandleKeyBindingBtnOnSelected(object sender, EventArgs e)
	{
		if ((Object)(object)m_PreviousButton != (Object)null)
		{
			m_PreviousButton.SetActive(false);
		}
		m_KeyBindingsPrompt.SetActive(true);
		m_OptionsPrompt.SetActive(false);
	}

	private void HandleOptionsBtnOnSelected(object sender, EventArgs e)
	{
		if ((Object)(object)m_PreviousButton != (Object)null)
		{
			m_PreviousButton.SetActive(true);
			m_OptionsButton.Deselect();
		}
		m_KeyBindingsPrompt.SetActive(false);
		m_OptionsPrompt.SetActive(true);
	}

	private void InitQualityToggles()
	{
		m_CurrentQuality = (QualityLevel)GameManager.Instance.PlayerSettings.currentQuality;
		m_QualityLowToggle.isOn = m_CurrentQuality == QualityLevel.LOW;
		m_QualityMediumToggle.isOn = m_CurrentQuality == QualityLevel.MEDIUM;
		m_QualityHighToggle.isOn = m_CurrentQuality == QualityLevel.HIGH;
		((UnityEvent<bool>)(object)m_QualityLowToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityLowValueChange);
		((UnityEvent<bool>)(object)m_QualityMediumToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityMediumValueChange);
		((UnityEvent<bool>)(object)m_QualityHighToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityHighValueChange);
	}

	private void HandleQualityLowValueChange(bool active)
	{
		RemoveQualityToggleListeners();
		UpdateQuality(0);
		((Selectable)m_QualityLowToggle).interactable = false;
		((Selectable)m_QualityMediumToggle).interactable = true;
		m_QualityMediumToggle.isOn = false;
		((Selectable)m_QualityHighToggle).interactable = true;
		m_QualityHighToggle.isOn = false;
		((UnityEvent<bool>)(object)m_QualityMediumToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityMediumValueChange);
		((UnityEvent<bool>)(object)m_QualityHighToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityHighValueChange);
	}

	private void HandleQualityMediumValueChange(bool active)
	{
		RemoveQualityToggleListeners();
		UpdateQuality(1);
		((Selectable)m_QualityMediumToggle).interactable = false;
		m_QualityLowToggle.isOn = false;
		m_QualityHighToggle.isOn = false;
		((UnityEvent<bool>)(object)m_QualityLowToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityLowValueChange);
		((UnityEvent<bool>)(object)m_QualityHighToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityHighValueChange);
	}

	private void HandleQualityHighValueChange(bool active)
	{
		RemoveQualityToggleListeners();
		UpdateQuality(2);
		((Selectable)m_QualityHighToggle).interactable = false;
		((Selectable)m_QualityLowToggle).interactable = true;
		m_QualityLowToggle.isOn = false;
		((Selectable)m_QualityMediumToggle).interactable = true;
		m_QualityMediumToggle.isOn = false;
		((UnityEvent<bool>)(object)m_QualityLowToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityLowValueChange);
		((UnityEvent<bool>)(object)m_QualityMediumToggle.onValueChanged).AddListener((UnityAction<bool>)HandleQualityMediumValueChange);
	}

	private void RemoveQualityToggleListeners()
	{
		((UnityEvent<bool>)(object)m_QualityLowToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleQualityLowValueChange);
		((UnityEvent<bool>)(object)m_QualityMediumToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleQualityMediumValueChange);
		((UnityEvent<bool>)(object)m_QualityHighToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleQualityHighValueChange);
	}

	private void UpdateQuality(int level)
	{
		GameManager.Instance.PlayerSettings.currentQuality = level;
		QualitySettings.SetQualityLevel(level, true);
		QualitySettings.shadows = (ShadowQuality)(GameManager.Instance.PlayerSettings.Shadows ? 2 : 0);
		QualitySettings.vSyncCount = (GameManager.Instance.PlayerSettings.VSync ? 1 : 0);
		if (QualitySettings.vSyncCount == 0)
		{
			Application.targetFrameRate = -1;
		}
		else
		{
			Application.targetFrameRate = 60;
		}
		UpdateEffects(level);
	}

	private void UpdateEffects(int level)
	{
		bool flag = level > 0;
		GameManager.Instance.PlayerSettings.Bloom = flag;
		m_BloomToggle.isOn = GameManager.Instance.PlayerSettings.Bloom;
		GameManager.Instance.PlayerSettings.AmbientOcclusion = flag;
		m_AmbientOcclusionToggle.isOn = GameManager.Instance.PlayerSettings.AmbientOcclusion;
		GameManager.Instance.PlayerSettings.Shadows = flag;
		m_ShadowsToggle.isOn = GameManager.Instance.PlayerSettings.Shadows;
		GameManager.Instance.PlayerSettings.Fog = flag;
		m_FogToggle.isOn = GameManager.Instance.PlayerSettings.Fog;
		GameManager.Instance.PlayerSettings.VSync = flag;
		m_VSyncToggle.isOn = GameManager.Instance.PlayerSettings.VSync;
		bool flag2 = level > 1;
		GameManager.Instance.PlayerSettings.AA = flag2;
		m_AAToggle.isOn = GameManager.Instance.PlayerSettings.AA;
		GameManager.Instance.PlayerSettings.DoF = flag2;
		m_DepthBlurToggle.isOn = GameManager.Instance.PlayerSettings.DoF;
		GameManager.Instance.PlayerSettings.VolumetricLighting = flag2;
		m_VolumetricLightToggle.isOn = GameManager.Instance.PlayerSettings.VolumetricLighting;
		GameManager.Instance.PlayerSettings.Grain = flag2;
		m_GrainToggle.isOn = GameManager.Instance.PlayerSettings.Grain;
		GameManager.Instance.PlayerSettings.MotionBlur = flag2;
		m_MotionBlurToggle.isOn = GameManager.Instance.PlayerSettings.MotionBlur;
		GameManager.Instance.PlayerSettings.DustParticles = flag2;
		m_DustParticlesToggle.isOn = GameManager.Instance.PlayerSettings.DustParticles;
		UpdateScionPostProcess();
	}

	private void UpdateScionPostProcess()
	{
		if (!GameManager.Instance.PlayerSettings.Grain && !GameManager.Instance.PlayerSettings.DoF)
		{
			((Behaviour)GameManager.Instance.GameCamera.UnityDOF).enabled = false;
			GameManager.Instance.GameCamera.DoF = false;
		}
	}

	private void InitResolutionDropdown()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		for (int i = 0; i < GameManager.Instance.PlayerSettings.Resolutions.Count; i++)
		{
			Resolution val = GameManager.Instance.PlayerSettings.Resolutions[i];
			list.Add(((Resolution)(ref val)).width + "x" + ((Resolution)(ref val)).height);
			int height = ((Resolution)(ref val)).height;
			Resolution currentResolution = Screen.currentResolution;
			if (height == ((Resolution)(ref currentResolution)).height)
			{
				int width = ((Resolution)(ref val)).width;
				Resolution currentResolution2 = Screen.currentResolution;
				if (width == ((Resolution)(ref currentResolution2)).width)
				{
					m_ResolutionDropdown.CaptionText.text = ((Resolution)(ref val)).width + "x" + ((Resolution)(ref val)).height;
				}
			}
		}
		m_ResolutionDropdown.Dropdown.AddOptions(list);
		m_ResolutionDropdown.Dropdown.value = GameManager.Instance.PlayerSettings.CurrentResolution;
		((UnityEvent<int>)(object)m_ResolutionDropdown.Dropdown.onValueChanged).AddListener((UnityAction<int>)HandleResolutionValeChange);
	}

	private void HandleResolutionValeChange(int index)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Resolution val = GameManager.Instance.PlayerSettings.Resolutions[index];
		m_ResolutionDropdown.Dropdown.value = index;
		m_ResolutionDropdown.CaptionText.text = ((Resolution)(ref val)).width + "x" + ((Resolution)(ref val)).height;
		Screen.SetResolution(((Resolution)(ref val)).width, ((Resolution)(ref val)).height, GameManager.Instance.PlayerSettings.Fullscreen);
		GameManager.Instance.PlayerSettings.CurrentResolution = index;
		GameManager.Instance.PlayerSettings.ResolutionWidth = ((Resolution)(ref val)).width;
		GameManager.Instance.PlayerSettings.ResolutionHeight = ((Resolution)(ref val)).height;
	}

	private void HandleInvertedValueChange(bool isClaptrap)
	{
		GameManager.Instance.PlayerSettings.Inverted = isClaptrap;
	}

	private void HandleSubtitleValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.Subtitles = active;
	}

	private void HandleFullscreenValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.Fullscreen = active;
	}

	private void HandleSensitivityValueChange(float value)
	{
		GameManager.Instance.PlayerSettings.Sensitivity = value;
		if (!((Object)(object)GameManager.Instance.Player == (Object)null))
		{
			GameManager.Instance.Player.SetSensitivity(value);
		}
	}

	private void HandleMasterVolumeValueChange(float value)
	{
		GameManager.Instance.PlayerSettings.Volume = value;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Master", value);
		AudioListener.volume = value;
	}

	private void HandleMusicVolumeValueChange(float value)
	{
		GameManager.Instance.PlayerSettings.MusicVolume = value;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Music", value);
	}

	private void HandleSFXVolumeValueChange(float value)
	{
		GameManager.Instance.PlayerSettings.SFXVolume = value;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Effects", value);
	}

	private void HandleDialogueVolumeValueChange(float value)
	{
		GameManager.Instance.PlayerSettings.DialogueVolume = value;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Dialogue", value);
	}

	private void HandleDOFValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.DoF = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.DoF = active;
			UpdateScionPostProcess();
		}
	}

	private void HandleBloomValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.Bloom = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.Bloom = active;
		}
	}

	private void HandleVolumetricLightingValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.VolumetricLighting = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.VolumetricLighting = active;
		}
	}

	private void HandleAmbientOcclusionValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.AmbientOcclusion = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.AmbientOcclusion = active;
		}
	}

	private void HandleGrainValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.Grain = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.Grain = active;
			UpdateScionPostProcess();
		}
	}

	private void HandleMotionBlurValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.MotionBlur = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.MotionBlur = active;
		}
	}

	private void HandleViewBobbingValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.ViewBobbing = active;
	}

	private void HandleViewSwayingValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.ViewSwaying = active;
	}

	private void HandleAAValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.AA = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.AA = active;
		}
	}

	private void HandleVSyncValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.VSync = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			QualitySettings.vSyncCount = (active ? 1 : 0);
			if (QualitySettings.vSyncCount == 0)
			{
				Application.targetFrameRate = -1;
			}
			else
			{
				Application.targetFrameRate = 60;
			}
		}
	}

	private void HandleFogValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.Fog = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.Fog = active;
		}
	}

	private void HandleShadowValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.Shadows = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			QualitySettings.shadows = (ShadowQuality)(active ? 2 : 0);
		}
	}

	private void HandleDustParticlesValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.DustParticles = active;
		if (!((Object)(object)GameManager.Instance.GameCamera == (Object)null))
		{
			GameManager.Instance.GameCamera.Dust = active;
		}
	}

	private void HandleCrosshairValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.Crosshair = active;
	}

	private void HandleBrightnessValueChange(float value)
	{
		GameManager.Instance.PlayerSettings.Brightness = value;
	}

	private void AddListeners()
	{
		m_KeyBindingsButton.OnSelected += HandleKeyBindingBtnOnSelected;
		m_OptionsButton.OnSelected += HandleOptionsBtnOnSelected;
		((UnityEvent<float>)(object)m_BrightnessSlider.onValueChanged).AddListener((UnityAction<float>)HandleBrightnessValueChange);
		((UnityEvent<float>)(object)m_SensitivitySlider.onValueChanged).AddListener((UnityAction<float>)HandleSensitivityValueChange);
		((UnityEvent<bool>)(object)m_InvertedToggle.onValueChanged).AddListener((UnityAction<bool>)HandleInvertedValueChange);
		((UnityEvent<bool>)(object)m_CrosshairToggle.onValueChanged).AddListener((UnityAction<bool>)HandleCrosshairValueChange);
		((UnityEvent<bool>)(object)m_SubtitlesToggle.onValueChanged).AddListener((UnityAction<bool>)HandleSubtitleValueChange);
		((UnityEvent<float>)(object)m_MasterVolumeSlider.onValueChanged).AddListener((UnityAction<float>)HandleMasterVolumeValueChange);
		((UnityEvent<float>)(object)m_MusicVolumeSlider.onValueChanged).AddListener((UnityAction<float>)HandleMusicVolumeValueChange);
		((UnityEvent<float>)(object)m_SFXVolumeSlider.onValueChanged).AddListener((UnityAction<float>)HandleSFXVolumeValueChange);
		((UnityEvent<float>)(object)m_DialogueVolumeSlider.onValueChanged).AddListener((UnityAction<float>)HandleDialogueVolumeValueChange);
		((UnityEvent<bool>)(object)m_FullscreenToggle.onValueChanged).AddListener((UnityAction<bool>)HandleFullscreenValueChange);
		((UnityEvent<bool>)(object)m_AAToggle.onValueChanged).AddListener((UnityAction<bool>)HandleAAValueChange);
		((UnityEvent<bool>)(object)m_VSyncToggle.onValueChanged).AddListener((UnityAction<bool>)HandleVSyncValueChange);
		((UnityEvent<bool>)(object)m_DepthBlurToggle.onValueChanged).AddListener((UnityAction<bool>)HandleDOFValueChange);
		((UnityEvent<bool>)(object)m_BloomToggle.onValueChanged).AddListener((UnityAction<bool>)HandleBloomValueChange);
		((UnityEvent<bool>)(object)m_VolumetricLightToggle.onValueChanged).AddListener((UnityAction<bool>)HandleVolumetricLightingValueChange);
		((UnityEvent<bool>)(object)m_AmbientOcclusionToggle.onValueChanged).AddListener((UnityAction<bool>)HandleAmbientOcclusionValueChange);
		((UnityEvent<bool>)(object)m_GrainToggle.onValueChanged).AddListener((UnityAction<bool>)HandleGrainValueChange);
		((UnityEvent<bool>)(object)m_MotionBlurToggle.onValueChanged).AddListener((UnityAction<bool>)HandleMotionBlurValueChange);
		((UnityEvent<bool>)(object)m_ShadowsToggle.onValueChanged).AddListener((UnityAction<bool>)HandleShadowValueChange);
		((UnityEvent<bool>)(object)m_FogToggle.onValueChanged).AddListener((UnityAction<bool>)HandleFogValueChange);
		((UnityEvent<bool>)(object)m_ViewBobbingToggle.onValueChanged).AddListener((UnityAction<bool>)HandleViewBobbingValueChange);
		((UnityEvent<bool>)(object)m_ViewSwayingToggle.onValueChanged).AddListener((UnityAction<bool>)HandleViewSwayingValueChange);
		((UnityEvent<bool>)(object)m_DustParticlesToggle.onValueChanged).AddListener((UnityAction<bool>)HandleDustParticlesValueChange);
	}

	private void RemoveListeners()
	{
		m_KeyBindingsButton.OnSelected -= HandleKeyBindingBtnOnSelected;
		m_OptionsButton.OnSelected -= HandleOptionsBtnOnSelected;
		((UnityEvent<float>)(object)m_BrightnessSlider.onValueChanged).RemoveListener((UnityAction<float>)HandleBrightnessValueChange);
		((UnityEvent<float>)(object)m_SensitivitySlider.onValueChanged).RemoveListener((UnityAction<float>)HandleSensitivityValueChange);
		((UnityEvent<bool>)(object)m_InvertedToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleInvertedValueChange);
		((UnityEvent<bool>)(object)m_CrosshairToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleCrosshairValueChange);
		((UnityEvent<bool>)(object)m_SubtitlesToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleSubtitleValueChange);
		((UnityEvent<float>)(object)m_MasterVolumeSlider.onValueChanged).RemoveListener((UnityAction<float>)HandleMasterVolumeValueChange);
		((UnityEvent<float>)(object)m_MusicVolumeSlider.onValueChanged).RemoveListener((UnityAction<float>)HandleMusicVolumeValueChange);
		((UnityEvent<float>)(object)m_SFXVolumeSlider.onValueChanged).RemoveListener((UnityAction<float>)HandleSFXVolumeValueChange);
		((UnityEvent<float>)(object)m_DialogueVolumeSlider.onValueChanged).RemoveListener((UnityAction<float>)HandleDialogueVolumeValueChange);
		((UnityEvent<bool>)(object)m_FullscreenToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleFullscreenValueChange);
		((UnityEvent<bool>)(object)m_AAToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleAAValueChange);
		((UnityEvent<bool>)(object)m_VSyncToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleVSyncValueChange);
		((UnityEvent<bool>)(object)m_DepthBlurToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleDOFValueChange);
		((UnityEvent<bool>)(object)m_BloomToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleBloomValueChange);
		((UnityEvent<bool>)(object)m_VolumetricLightToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleVolumetricLightingValueChange);
		((UnityEvent<bool>)(object)m_AmbientOcclusionToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleAmbientOcclusionValueChange);
		((UnityEvent<bool>)(object)m_GrainToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleGrainValueChange);
		((UnityEvent<bool>)(object)m_MotionBlurToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleMotionBlurValueChange);
		((UnityEvent<bool>)(object)m_ViewBobbingToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleViewBobbingValueChange);
		((UnityEvent<bool>)(object)m_ViewSwayingToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleViewSwayingValueChange);
		((UnityEvent<bool>)(object)m_ShadowsToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleShadowValueChange);
		((UnityEvent<bool>)(object)m_DustParticlesToggle.onValueChanged).RemoveListener((UnityAction<bool>)HandleDustParticlesValueChange);
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
