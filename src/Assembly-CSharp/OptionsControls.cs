using System.Collections.Generic;
using I2.Loc;
using TMG.Core;
using UnityEngine;

public class OptionsControls : TMGAbstractDisposable
{
	private enum QualityLevel
	{
		LOW,
		MEDIUM,
		HIGH
	}

	private const string MASTER_VOLUME = "Master";

	private const string SFX_VOLUME = "Effects";

	private const string MUSIC_VOLUME = "Music";

	private const string DIALOGUE_VOLUME = "Dialogue";

	public List<string> Resolutions = new List<string>();

	public void Initialize()
	{
		InitResolutionDropdown();
	}

	private void InitResolutionDropdown()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		for (int i = 0; i < GameManager.Instance.PlayerSettings.Resolutions.Count; i++)
		{
			Resolution val = GameManager.Instance.PlayerSettings.Resolutions[i];
			list.Add(((Resolution)(ref val)).width + "x" + ((Resolution)(ref val)).height);
		}
		Resolutions.Clear();
		Resolutions = list;
	}

	public string UpdateQuality(int level)
	{
		GameManager.Instance.PlayerSettings.currentQuality = level;
		QualitySettings.SetQualityLevel(level, true);
		UpdateEffects(level);
		switch (level)
		{
		case 0:
			QualitySettings.shadows = (ShadowQuality)0;
			return "MENU/QUALITY_VERY_LOW";
		case 1:
			QualitySettings.shadows = (ShadowQuality)1;
			return "MENU/QUALITY_LOW";
		case 2:
			QualitySettings.shadows = (ShadowQuality)1;
			return "MENU/QUALITY_MEDIUM";
		case 3:
			QualitySettings.shadows = (ShadowQuality)2;
			return "MENU/QUALITY_HIGH";
		default:
			return "MENU/QUALITY_HIGH";
		}
	}

	private void UpdateEffects(int level)
	{
		bool shadows = level > 0;
		GameManager.Instance.PlayerSettings.Shadows = shadows;
		bool flag = level > 1;
		GameManager.Instance.PlayerSettings.Fog = flag;
		GameManager.Instance.PlayerSettings.VolumetricLighting = flag;
		bool dustParticles = level > 2;
		GameManager.Instance.PlayerSettings.DustParticles = dustParticles;
		GameManager.Instance.ParticleManager.UpdateQuality(level);
	}

	public string HandleResolutionValeChange(int index)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Resolution val = GameManager.Instance.PlayerSettings.Resolutions[index];
		Screen.SetResolution(((Resolution)(ref val)).width, ((Resolution)(ref val)).height, GameManager.Instance.PlayerSettings.Fullscreen);
		GameManager.Instance.PlayerSettings.CurrentResolution = index;
		GameManager.Instance.PlayerSettings.ResolutionWidth = ((Resolution)(ref val)).width;
		GameManager.Instance.PlayerSettings.ResolutionHeight = ((Resolution)(ref val)).height;
		return ((Resolution)(ref val)).width + "x" + ((Resolution)(ref val)).height;
	}

	public void HandleFullscreenValueChange()
	{
		GameManager.Instance.PlayerSettings.Fullscreen = !GameManager.Instance.PlayerSettings.Fullscreen;
	}

	public string HandleMasterVolumeValueChange(float value)
	{
		float num = ClampValue(GameManager.Instance.PlayerSettings.Volume + value);
		GameManager.Instance.PlayerSettings.Volume = num;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Master", LinearToDecibel(num));
		return (num * 100f).ToString("N0");
	}

	public string HandleMusicVolumeValueChange(float value)
	{
		float num = ClampValue(GameManager.Instance.PlayerSettings.MusicVolume + value);
		GameManager.Instance.PlayerSettings.MusicVolume = num;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Music", LinearToDecibel(num));
		return (num * 100f).ToString("N0");
	}

	public string HandleSFXVolumeValueChange(float value)
	{
		float num = ClampValue(GameManager.Instance.PlayerSettings.SFXVolume + value);
		GameManager.Instance.PlayerSettings.SFXVolume = num;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Effects", LinearToDecibel(num));
		return (num * 100f).ToString("N0");
	}

	public string HandleDialogueVolumeValueChange(float value)
	{
		float num = ClampValue(GameManager.Instance.PlayerSettings.DialogueVolume + value);
		GameManager.Instance.PlayerSettings.DialogueVolume = num;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Dialogue", LinearToDecibel(num));
		return (num * 100f).ToString("N0");
	}

	public void HandleSubtitleValueChange()
	{
		GameManager.Instance.PlayerSettings.Subtitles = !GameManager.Instance.PlayerSettings.Subtitles;
	}

	public string HandleLanguageValueChange(bool isRight)
	{
		List<string> allLanguagesCode = LocalizationManager.GetAllLanguagesCode();
		string text = LocalizationManager.CurrentLanguageCode;
		for (int i = 0; i < allLanguagesCode.Count; i++)
		{
			if (text == allLanguagesCode[i])
			{
				if (i <= 0)
				{
					text = allLanguagesCode[(!isRight) ? (allLanguagesCode.Count - 1) : (i + 1)];
					break;
				}
				if (i != allLanguagesCode.Count - 1)
				{
					text = allLanguagesCode[(!isRight) ? (i - 1) : (i + 1)];
					break;
				}
				text = ((!isRight) ? allLanguagesCode[allLanguagesCode.Count - 2] : allLanguagesCode[0]);
			}
		}
		return text;
	}

	public string HandleSensitivityValueChange(float value)
	{
		float num = ClampValue(GameManager.Instance.PlayerSettings.Sensitivity + value);
		GameManager.Instance.PlayerSettings.Sensitivity = num;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			GameManager.Instance.Player.SetSensitivity(num);
		}
		return (num * 100f).ToString("N0");
	}

	public void HandleInvertedValueChange()
	{
		GameManager.Instance.PlayerSettings.Inverted = !GameManager.Instance.PlayerSettings.Inverted;
	}

	public void HandleToggleRunValueChange()
	{
		GameManager.Instance.PlayerSettings.ToggleRun = !GameManager.Instance.PlayerSettings.ToggleRun;
	}

	public void HandleTipsValueChange()
	{
		GameManager.Instance.PlayerSettings.Tips = !GameManager.Instance.PlayerSettings.Tips;
	}

	public string HandleBrightnessValueChange(float value)
	{
		float num = ClampValue(GameManager.Instance.PlayerSettings.Brightness + value);
		GameManager.Instance.PlayerSettings.Brightness = num;
		float brightness = Mathf.Clamp(num, 0.35f, 0.65f);
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.Brightness.SetBrightness(brightness);
		}
		return (num * 100f).ToString("N0");
	}

	public void HandleMotionBlurValueChange()
	{
		GameManager.Instance.PlayerSettings.MotionBlur = !GameManager.Instance.PlayerSettings.MotionBlur;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.MotionBlur = GameManager.Instance.PlayerSettings.MotionBlur;
		}
	}

	public void HandleCrosshairValueChange()
	{
		GameManager.Instance.PlayerSettings.Crosshair = !GameManager.Instance.PlayerSettings.Crosshair;
	}

	public void HandleDOFValueChange()
	{
		GameManager.Instance.PlayerSettings.DoF = !GameManager.Instance.PlayerSettings.DoF;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.DoF = GameManager.Instance.PlayerSettings.DoF;
			((Behaviour)GameManager.Instance.GameCamera.UnityDOF).enabled = GameManager.Instance.PlayerSettings.DoF;
		}
	}

	public void HandleBloomValueChange()
	{
		GameManager.Instance.PlayerSettings.Bloom = !GameManager.Instance.PlayerSettings.Bloom;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.Bloom = GameManager.Instance.PlayerSettings.Bloom;
		}
	}

	public void HandleVolumetricLightingValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.VolumetricLighting = active;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.VolumetricLighting = active;
		}
	}

	public void HandleAmbientOcclusionValueChange()
	{
		GameManager.Instance.PlayerSettings.AmbientOcclusion = !GameManager.Instance.PlayerSettings.AmbientOcclusion;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.AmbientOcclusion = GameManager.Instance.PlayerSettings.AmbientOcclusion;
		}
	}

	public void HandleGrainValueChange()
	{
		GameManager.Instance.PlayerSettings.Grain = !GameManager.Instance.PlayerSettings.Grain;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.Grain = GameManager.Instance.PlayerSettings.Grain;
		}
	}

	public void HandleViewBobbingValueChange()
	{
		GameManager.Instance.PlayerSettings.ViewBobbing = !GameManager.Instance.PlayerSettings.ViewBobbing;
	}

	public void HandleViewSwayingValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.ViewSwaying = active;
	}

	public void HandleAAValueChange()
	{
		GameManager.Instance.PlayerSettings.AA = !GameManager.Instance.PlayerSettings.AA;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.AA = GameManager.Instance.PlayerSettings.AA;
		}
	}

	public void HandleVSyncValueChange()
	{
		GameManager.Instance.PlayerSettings.VSync = !GameManager.Instance.PlayerSettings.VSync;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			QualitySettings.vSyncCount = (GameManager.Instance.PlayerSettings.VSync ? 1 : 0);
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

	public void HandleFogValueChange(bool active)
	{
		GameManager.Instance.PlayerSettings.Fog = active;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.Fog = active;
		}
	}

	private float ClampValue(float value)
	{
		if (value > 1f)
		{
			value = 1f;
		}
		else if (value < 0f)
		{
			value = 0f;
		}
		return value;
	}

	private float LinearToDecibel(float linearVal)
	{
		if (linearVal != 0f)
		{
			return 20f * Mathf.Log10(linearVal);
		}
		return -80f;
	}

	public string HandleFoVValueChange(float value)
	{
		float num = Mathf.Clamp(GameManager.Instance.PlayerSettings.FoV + value * 5f, 55f, 120f);
		GameManager.Instance.PlayerSettings.FoV = num;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.FOV = num;
		}
		return num.ToString("N0");
	}

	public void HandleViewSwayingValueChange()
	{
		GameManager.Instance.PlayerSettings.ViewSwaying = !GameManager.Instance.PlayerSettings.ViewSwaying;
	}

	public string HandleCrosshairScaleValueChange(float value)
	{
		float crosshairScale = Mathf.Clamp(GameManager.Instance.PlayerSettings.CrosshairScale + value * 0.1f, 0.1f, 1f);
		GameManager.Instance.PlayerSettings.CrosshairScale = crosshairScale;
		return crosshairScale.ToString("N1");
	}

	public string HandleCrosshairOpacityValueChange(float value)
	{
		float crosshairOpacity = Mathf.Clamp(GameManager.Instance.PlayerSettings.CrosshairOpacity + value * 0.1f, 0.1f, 1f);
		GameManager.Instance.PlayerSettings.CrosshairOpacity = crosshairOpacity;
		return crosshairOpacity.ToString("N1");
	}

	public string UpdateAudioType(int level)
	{
		GameManager.Instance.PlayerSettings.currentAudioType = level;
		switch (level)
		{
		case 0:
			AudioSettings.speakerMode = (AudioSpeakerMode)1;
			GameManager.Instance.AudioTypeChanged = true;
			return "MONO";
		case 1:
			AudioSettings.speakerMode = (AudioSpeakerMode)2;
			GameManager.Instance.AudioTypeChanged = true;
			return "STER";
		case 2:
			AudioSettings.speakerMode = (AudioSpeakerMode)3;
			GameManager.Instance.AudioTypeChanged = true;
			return "QUAD";
		case 3:
			AudioSettings.speakerMode = (AudioSpeakerMode)4;
			GameManager.Instance.AudioTypeChanged = true;
			return "SURR";
		case 4:
			AudioSettings.speakerMode = (AudioSpeakerMode)5;
			GameManager.Instance.AudioTypeChanged = true;
			return "5.1";
		case 5:
			AudioSettings.speakerMode = (AudioSpeakerMode)6;
			GameManager.Instance.AudioTypeChanged = true;
			return "7.1";
		default:
			return "STER";
		}
	}

	public void HandleFlickerDisableValueChange()
	{
		GameManager.Instance.PlayerSettings.flickerLights = !GameManager.Instance.PlayerSettings.flickerLights;
	}

	public string HandleWeaponScaleValueChange(float value)
	{
		float weaponScale = Mathf.Clamp(GameManager.Instance.PlayerSettings.weaponScale + value * 0.1f, 0.5f, 1f);
		GameManager.Instance.PlayerSettings.weaponScale = weaponScale;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			GameManager.Instance.Player.ScaleWeapon();
		}
		return weaponScale.ToString("N1");
	}

	public string HandleBendyAggressionValueChange(float value)
	{
		float bendyAggressionScale = Mathf.Clamp(GameManager.Instance.PlayerSettings.BendyAggressionScale + value * 0.1f, 0.5f, 2f);
		GameManager.Instance.PlayerSettings.BendyAggressionScale = bendyAggressionScale;
		return bendyAggressionScale.ToString("N1");
	}

	public void HandleForceHalloweenValueChange()
	{
		GameManager.Instance.PlayerSettings.forceHalloween = !GameManager.Instance.PlayerSettings.forceHalloween;
	}
}
