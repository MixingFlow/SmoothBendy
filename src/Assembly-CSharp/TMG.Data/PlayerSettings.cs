using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using TMG.Core;
using UnityEngine;

namespace TMG.Data;

public class PlayerSettings : TMGAbstractDisposable
{
	private class ResolutionEqualityComparer : IEqualityComparer<Resolution>
	{
		public bool Equals(Resolution x, Resolution y)
		{
			if (((Resolution)(ref x)).width == ((Resolution)(ref y)).width && ((Resolution)(ref x)).height == ((Resolution)(ref y)).height)
			{
				return ((Resolution)(ref x)).refreshRate == ((Resolution)(ref y)).refreshRate;
			}
			return false;
		}

		public int GetHashCode(Resolution obj)
		{
			return ((Resolution)(ref obj)).width ^ ((Resolution)(ref obj)).height ^ ((Resolution)(ref obj)).refreshRate;
		}
	}

	public const float SENSITIVITY_MULTIPLIER = 5f;

	private bool m_Shadows;

	private List<Resolution> m_Resolutions;

	private Dictionary<int, string> m_Quality;

	public bool isVolumetricLightSupported
	{
		get
		{
			if (Shader.Find("Sandbox/VolumetricLight").isSupported && Shader.Find("Hidden/BilateralBlur").isSupported)
			{
				return Shader.Find("Hidden/BlitAdd").isSupported;
			}
			return false;
		}
	}

	public bool isPostProcessSupported
	{
		get
		{
			if (SystemInfo.supportsImageEffects && Shader.Find("Hidden/ScionBloom").isSupported)
			{
				return Shader.Find("Hidden/ScionDepthOfField").isSupported;
			}
			return false;
		}
	}

	public bool isSSAOSupported
	{
		get
		{
			if (Shader.Find("Hidden/Amplify Occlusion/Occlusion").isSupported && Shader.Find("Hidden/Amplify Occlusion/Blur").isSupported)
			{
				return Shader.Find("Hidden/Amplify Occlusion/Copy").isSupported;
			}
			return false;
		}
	}

	public float Sensitivity
	{
		get
		{
			return PlayerPrefsManager.GetFloat("SENSITIVITY");
		}
		set
		{
			PlayerPrefsManager.Save("SENSITIVITY", value);
		}
	}

	public bool Inverted
	{
		get
		{
			return PlayerPrefsManager.GetBool("INVERTED");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("INVERTED", value2);
		}
	}

	public bool ToggleRun
	{
		get
		{
			return PlayerPrefsManager.GetBool("TOGGLE_RUN");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("TOGGLE_RUN", value2);
		}
	}

	public bool Tips
	{
		get
		{
			return PlayerPrefsManager.GetBool("TIPS");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("TIPS", value2);
		}
	}

	public bool Crosshair
	{
		get
		{
			return PlayerPrefsManager.GetBool("CROSSHAIR");
		}
		set
		{
			int num = (value ? 1 : 0);
			PlayerPrefsManager.Save("CROSSHAIR", num);
			if ((Object)(object)GameManager.Instance.GameCamera != (Object)null)
			{
				if (num == 1)
				{
					GameManager.Instance.ShowCrosshair();
				}
				else
				{
					GameManager.Instance.HideCrosshair();
				}
			}
		}
	}

	public float Volume
	{
		get
		{
			return PlayerPrefsManager.GetFloat("VOLUME");
		}
		set
		{
			PlayerPrefsManager.Save("VOLUME", value);
		}
	}

	public float MusicVolume
	{
		get
		{
			return PlayerPrefsManager.GetFloat("MUSIC_VOLUME");
		}
		set
		{
			PlayerPrefsManager.Save("MUSIC_VOLUME", value);
		}
	}

	public float SFXVolume
	{
		get
		{
			return PlayerPrefsManager.GetFloat("SFX_VOLUME");
		}
		set
		{
			PlayerPrefsManager.Save("SFX_VOLUME", value);
		}
	}

	public float DialogueVolume
	{
		get
		{
			return PlayerPrefsManager.GetFloat("DIALOGUE_VOLUME");
		}
		set
		{
			PlayerPrefsManager.Save("DIALOGUE_VOLUME", value);
		}
	}

	public bool Subtitles
	{
		get
		{
			return PlayerPrefsManager.GetBool("SUBTITLES");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("SUBTITLES", value2);
		}
	}

	public string Language
	{
		get
		{
			return PlayerPrefsManager.GetString("LANGUAGE");
		}
		set
		{
			PlayerPrefsManager.Save("LANGUAGE", value);
		}
	}

	public float Brightness
	{
		get
		{
			return PlayerPrefsManager.GetFloat("BRIGHTNESS");
		}
		set
		{
			PlayerPrefsManager.Save("BRIGHTNESS", value);
			if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
			{
				GameManager.Instance.GameCamera.Brightness.SetBrightness(value);
			}
		}
	}

	public bool Fullscreen
	{
		get
		{
			return PlayerPrefsManager.GetBool("FULLSCREEN");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("FULLSCREEN", value2);
			Screen.fullScreen = value;
		}
	}

	public bool DoF
	{
		get
		{
			return PlayerPrefsManager.GetBool("DOF");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("DOF", value2);
		}
	}

	public bool Bloom
	{
		get
		{
			return PlayerPrefsManager.GetBool("BLOOM");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("BLOOM", value2);
		}
	}

	public bool VolumetricLighting
	{
		get
		{
			return PlayerPrefsManager.GetBool("VOLUMETRIC_LIGHT");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("VOLUMETRIC_LIGHT", value2);
		}
	}

	public bool AmbientOcclusion
	{
		get
		{
			return PlayerPrefsManager.GetBool("AMBIENT_OCCLUSION");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("AMBIENT_OCCLUSION", value2);
		}
	}

	public bool Grain
	{
		get
		{
			return PlayerPrefsManager.GetBool("GRAIN");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("GRAIN", value2);
		}
	}

	public bool MotionBlur
	{
		get
		{
			return PlayerPrefsManager.GetBool("MOTION_BLUR");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("MOTION_BLUR", value2);
		}
	}

	public bool ViewBobbing
	{
		get
		{
			return PlayerPrefsManager.GetBool("VIEW_BOBBING");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("VIEW_BOBBING", value2);
		}
	}

	public bool ViewSwaying
	{
		get
		{
			return PlayerPrefsManager.GetBool("VIEW_SWAYING");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("VIEW_SWAYING", value2);
		}
	}

	public bool AA
	{
		get
		{
			return PlayerPrefsManager.GetBool("ANTI_ALIASING");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("ANTI_ALIASING", value2);
		}
	}

	public bool VSync
	{
		get
		{
			return PlayerPrefsManager.GetBool("V_SYNC");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("V_SYNC", value2);
		}
	}

	public bool Fog
	{
		get
		{
			return PlayerPrefsManager.GetBool("FOG");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("FOG", value2);
		}
	}

	public bool Shadows
	{
		get
		{
			return PlayerPrefsManager.GetBool("SHADOWS");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("SHADOWS", value2);
		}
	}

	public bool DustParticles
	{
		get
		{
			return PlayerPrefsManager.GetBool("DUST_PARTICLES");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("DUST_PARTICLES", value2);
			if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
			{
				GameManager.Instance.GameCamera.Dust = value;
			}
		}
	}

	public int currentQuality
	{
		get
		{
			return PlayerPrefsManager.GetInt("QUALITY");
		}
		set
		{
			PlayerPrefsManager.Save("QUALITY", value);
		}
	}

	public int CurrentResolution
	{
		get
		{
			return PlayerPrefsManager.GetInt("RESOLUTION_CURRENT");
		}
		set
		{
			PlayerPrefsManager.Save("RESOLUTION_CURRENT", value);
		}
	}

	public int ResolutionWidth
	{
		get
		{
			return PlayerPrefsManager.GetInt("RESOLUTION_WIDTH");
		}
		set
		{
			PlayerPrefsManager.Save("RESOLUTION_WIDTH", value);
		}
	}

	public int ResolutionHeight
	{
		get
		{
			return PlayerPrefsManager.GetInt("RESOLUTION_HEIGHT");
		}
		set
		{
			PlayerPrefsManager.Save("RESOLUTION_HEIGHT", value);
		}
	}

	public List<Resolution> Resolutions
	{
		get
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if (m_Resolutions == null || m_Resolutions.Count <= 0)
			{
				m_Resolutions = new List<Resolution>(Screen.resolutions);
				Resolution currentResolution = Screen.currentResolution;
				if (!m_Resolutions.Contains(currentResolution, new ResolutionEqualityComparer()))
				{
					m_Resolutions.Add(currentResolution);
				}
				m_Resolutions = m_Resolutions.OrderBy(delegate(Resolution r)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					Resolution val = r;
					return ((Resolution)(ref val)).width;
				}).ThenBy(delegate(Resolution r)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					Resolution val = r;
					return ((Resolution)(ref val)).height;
				}).ToList();
			}
			return m_Resolutions;
		}
	}

	public Dictionary<int, string> Quality
	{
		get
		{
			if (m_Quality == null || m_Quality.Count <= 0)
			{
				m_Quality = new Dictionary<int, string>();
				string[] array = new string[4] { "Very Low", "Low", "Medium", "High" };
				for (int i = 0; i < 4; i++)
				{
					m_Quality.Add(i, array[i]);
				}
			}
			return m_Quality;
		}
	}

	public float FoV
	{
		get
		{
			return PlayerPrefsManager.GetFloat("FoV");
		}
		set
		{
			PlayerPrefsManager.Save("FoV", value);
		}
	}

	public float CrosshairScale
	{
		get
		{
			return PlayerPrefsManager.GetFloat("CROSSHAIR_SCALE");
		}
		set
		{
			PlayerPrefsManager.Save("CROSSHAIR_SCALE", value);
		}
	}

	public float CrosshairOpacity
	{
		get
		{
			return PlayerPrefsManager.GetFloat("CROSSHAIR_OPACITY");
		}
		set
		{
			PlayerPrefsManager.Save("CROSSHAIR_OPACITY", value);
		}
	}

	public int currentAudioType
	{
		get
		{
			return PlayerPrefsManager.GetInt("AUDIO_TYPE");
		}
		set
		{
			PlayerPrefsManager.Save("AUDIO_TYPE", value);
		}
	}

	public bool flickerLights
	{
		get
		{
			return PlayerPrefsManager.GetBool("FLICKER_LIGHTS");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("FLICKER_LIGHTS", value2);
		}
	}

	public float weaponScale
	{
		get
		{
			return PlayerPrefsManager.GetFloat("WEAPON_SCALE");
		}
		set
		{
			PlayerPrefsManager.Save("WEAPON_SCALE", value);
		}
	}

	public float BendyAggressionScale
	{
		get
		{
			return PlayerPrefsManager.GetFloat("BENDY_AGGRESSION_SCALE");
		}
		set
		{
			PlayerPrefsManager.Save("BENDY_AGGRESSION_SCALE", value);
		}
	}

	public bool forceHalloween
	{
		get
		{
			return PlayerPrefsManager.GetBool("FORCE_HALLOWEEN");
		}
		set
		{
			int value2 = (value ? 1 : 0);
			PlayerPrefsManager.Save("FORCE_HALLOWEEN", value2);
		}
	}

	public void Initialize()
	{
		if (!PlayerPrefs.HasKey("v1.5.1.0"))
		{
			Debug.Log((object)"Resetting Player Prefs");
			PlayerPrefs.DeleteAll();
			PlayerPrefsManager.Save("v1.5.1.0", 1);
		}
		if (!PlayerPrefs.HasKey("BRIGHTNESS"))
		{
			Debug.Log((object)"Initializing Brightness");
			PlayerPrefsManager.Save("BRIGHTNESS", 0.5f);
		}
		if (!PlayerPrefs.HasKey("QUALITY"))
		{
			Debug.Log((object)"Initializing Quality");
			PlayerPrefsManager.Save("QUALITY", 3);
		}
		if (!PlayerPrefs.HasKey("VOLUME"))
		{
			Debug.Log((object)"Initializing Volume");
			PlayerPrefsManager.Save("VOLUME", 1f);
		}
		if (!PlayerPrefs.HasKey("FoV"))
		{
			Debug.Log((object)"Initializing FoV");
			PlayerPrefsManager.Save("FoV", 55f);
		}
		if (!PlayerPrefs.HasKey("CROSSHAIR_SCALE"))
		{
			Debug.Log((object)"Initializing Crosshair Scale");
			PlayerPrefsManager.Save("CROSSHAIR_SCALE", 1f);
		}
		if (!PlayerPrefs.HasKey("CROSSHAIR_OPACITY"))
		{
			Debug.Log((object)"Initializing Crosshair Opacity");
			PlayerPrefsManager.Save("CROSSHAIR_OPACITY", 0.5f);
		}
		if (!PlayerPrefs.HasKey("MUSIC_VOLUME"))
		{
			Debug.Log((object)"Initializing Music Volume");
			PlayerPrefsManager.Save("MUSIC_VOLUME", 0.95f);
		}
		if (!PlayerPrefs.HasKey("SFX_VOLUME"))
		{
			Debug.Log((object)"Initializing SFX Volume");
			PlayerPrefsManager.Save("SFX_VOLUME", 0.95f);
		}
		if (!PlayerPrefs.HasKey("DIALOGUE_VOLUME"))
		{
			Debug.Log((object)"Initializing Dialogue Volume");
			PlayerPrefsManager.Save("DIALOGUE_VOLUME", 0.95f);
		}
		if (!PlayerPrefs.HasKey("SUBTITLES"))
		{
			Debug.Log((object)"Initializing Subtitles");
			PlayerPrefsManager.Save("SUBTITLES", 1);
		}
		if (!PlayerPrefs.HasKey("LANGUAGE"))
		{
			Debug.Log((object)"Initializing Language");
			string value = (LocalizationManager.CurrentLanguageCode = LocalizationManager.GetLanguageCode(LocalizationManager.GetCurrentDeviceLanguage()));
			PlayerPrefsManager.Save("LANGUAGE", value);
		}
		if (!PlayerPrefs.HasKey("INVERTED"))
		{
			Debug.Log((object)"Initializing Inverted");
			PlayerPrefsManager.Save("INVERTED", 0);
		}
		if (!PlayerPrefs.HasKey("TOGGLE_RUN"))
		{
			Debug.Log((object)"Initializing Toggle Run");
			PlayerPrefsManager.Save("TOGGLE_RUN", 0);
		}
		if (!PlayerPrefs.HasKey("CROSSHAIR"))
		{
			Debug.Log((object)"Initializing Crosshair");
			PlayerPrefsManager.Save("CROSSHAIR", 1);
		}
		if (!PlayerPrefs.HasKey("SENSITIVITY"))
		{
			Debug.Log((object)"Initializing Look Sensitivity");
			PlayerPrefsManager.Save("SENSITIVITY", 0.35f);
		}
		if (!PlayerPrefs.HasKey("BLOOM"))
		{
			Debug.Log((object)"Initializing Bloom Lighting");
			PlayerPrefsManager.Save("BLOOM", 1);
		}
		if (!PlayerPrefs.HasKey("DOF"))
		{
			Debug.Log((object)"Initializing Depth of Field");
			PlayerPrefsManager.Save("DOF", 1);
		}
		if (!PlayerPrefs.HasKey("VOLUMETRIC_LIGHT"))
		{
			Debug.Log((object)"Initializing Volumetric Lighting");
			PlayerPrefsManager.Save("VOLUMETRIC_LIGHT", 1);
		}
		if (!PlayerPrefs.HasKey("AMBIENT_OCCLUSION"))
		{
			Debug.Log((object)"Initializing Ambient Occlusion");
			PlayerPrefsManager.Save("AMBIENT_OCCLUSION", 1);
		}
		if (!PlayerPrefs.HasKey("GRAIN"))
		{
			Debug.Log((object)"Initializing Grain");
			PlayerPrefsManager.Save("GRAIN", 1);
		}
		if (!PlayerPrefs.HasKey("MOTION_BLUR"))
		{
			Debug.Log((object)"Initializing Motion Blur");
			PlayerPrefsManager.Save("MOTION_BLUR", 1);
		}
		if (!PlayerPrefs.HasKey("ANTI_ALIASING"))
		{
			Debug.Log((object)"Initializing Anti Aliasing");
			PlayerPrefsManager.Save("ANTI_ALIASING", 1);
		}
		if (!PlayerPrefs.HasKey("V_SYNC"))
		{
			Debug.Log((object)"Initializing V-Sync");
			PlayerPrefsManager.Save("V_SYNC", 1);
		}
		if (!PlayerPrefs.HasKey("VIEW_BOBBING"))
		{
			Debug.Log((object)"Initializing View Bobbing");
			PlayerPrefsManager.Save("VIEW_BOBBING", 1);
		}
		if (!PlayerPrefs.HasKey("VIEW_SWAYING"))
		{
			Debug.Log((object)"Initializing View Swaying");
			PlayerPrefsManager.Save("VIEW_SWAYING", 1);
		}
		if (!PlayerPrefs.HasKey("SHADOWS"))
		{
			Debug.Log((object)"Initializing Shadow");
			PlayerPrefsManager.Save("SHADOWS", 1);
		}
		if (!PlayerPrefs.HasKey("FOG"))
		{
			Debug.Log((object)"Initializing Fog");
			PlayerPrefsManager.Save("FOG", 1);
		}
		if (!PlayerPrefs.HasKey("DUST_PARTICLES"))
		{
			Debug.Log((object)"Initializing Dust Particles");
			PlayerPrefsManager.Save("DUST_PARTICLES", 1);
		}
		if (!PlayerPrefs.HasKey("TIPS"))
		{
			Debug.Log((object)"Initializing TIPS");
			PlayerPrefsManager.Save("TIPS", 1);
		}
		if (!PlayerPrefs.HasKey("AUDIO_TYPE"))
		{
			Debug.Log((object)"Initializing Audio Type");
			PlayerPrefsManager.Save("AUDIO_TYPE", 1);
		}
		if (!PlayerPrefs.HasKey("FLICKER_LIGHTS"))
		{
			Debug.Log((object)"Initializing Flickering Lights");
			PlayerPrefsManager.Save("FLICKER_LIGHTS", 1);
		}
		if (!PlayerPrefs.HasKey("WEAPON_SCALE"))
		{
			Debug.Log((object)"Initializing Weapon Scale");
			PlayerPrefsManager.Save("WEAPON_SCALE", 1f);
		}
		if (!PlayerPrefs.HasKey("BENDY_AGGRESSION_SCALE"))
		{
			Debug.Log((object)"Initializing Bendy Aggression Scale");
			PlayerPrefsManager.Save("BENDY_AGGRESSION_SCALE", 1f);
		}
		if (!PlayerPrefs.HasKey("FORCE_HALLOWEEN"))
		{
			Debug.Log((object)"Initializing Force Halloween");
			PlayerPrefsManager.Save("FORCE_HALLOWEEN", 0);
		}
		LocalizationManager.CurrentLanguageCode = GameManager.Instance.PlayerSettings.Language;
		QualitySettings.vSyncCount = (GameManager.Instance.PlayerSettings.VSync ? 1 : 0);
		if (QualitySettings.vSyncCount == 0)
		{
			Application.targetFrameRate = -1;
		}
		else
		{
			Application.targetFrameRate = 60;
		}
		InitializeQuality();
		InitializeResolution();
	}

	private void InitializeQuality()
	{
		QualitySettings.SetQualityLevel(currentQuality, true);
	}

	private void InitializeResolution()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (!PlayerPrefs.HasKey("FULLSCREEN"))
		{
			Debug.Log((object)"Initializing Fullscreen");
			PlayerPrefsManager.Save("FULLSCREEN", 1);
		}
		Resolution val;
		if (!PlayerPrefs.HasKey("RESOLUTION_WIDTH"))
		{
			Debug.Log((object)"Initializing Resolution Width");
			val = GameManager.Instance.PlayerSettings.Resolutions[GameManager.Instance.PlayerSettings.Resolutions.Count - 1];
			PlayerPrefsManager.Save("RESOLUTION_WIDTH", ((Resolution)(ref val)).width);
		}
		if (!PlayerPrefs.HasKey("RESOLUTION_HEIGHT"))
		{
			Debug.Log((object)"Initializing Resolution Height");
			val = GameManager.Instance.PlayerSettings.Resolutions[GameManager.Instance.PlayerSettings.Resolutions.Count - 1];
			PlayerPrefsManager.Save("RESOLUTION_HEIGHT", ((Resolution)(ref val)).height);
		}
		if (!PlayerPrefs.HasKey("RESOLUTION_CURRENT"))
		{
			Debug.Log((object)"Initializing Resolution Current");
			PlayerPrefsManager.Save("RESOLUTION_CURRENT", GameManager.Instance.PlayerSettings.Resolutions.Count - 1);
		}
		Fullscreen = PlayerPrefsManager.GetBool("FULLSCREEN");
		int num = PlayerPrefsManager.GetInt("RESOLUTION_WIDTH");
		int num2 = PlayerPrefsManager.GetInt("RESOLUTION_HEIGHT");
		Screen.SetResolution(num, num2, Fullscreen);
		Debug.Log((object)("Fullscreen: " + Fullscreen + "\nResolution :" + num + " x " + num2 + "\nResolution Index: " + CurrentResolution));
	}

	protected override void OnDisposed()
	{
		if (m_Resolutions != null)
		{
			m_Resolutions.Clear();
			m_Resolutions = null;
		}
		base.OnDisposed();
	}
}
