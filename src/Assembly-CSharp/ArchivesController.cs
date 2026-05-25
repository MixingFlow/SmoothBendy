using System;
using DG.Tweening;
using I2.Loc;
using UnityEngine;

public class ArchivesController : BaseController
{
	protected class LocalizationSupport : ILocalizationParamsManager
	{
		public string GetParameterValue(string Param)
		{
			if (Param != null && Param == "N")
			{
				return "\n";
			}
			return null;
		}
	}

	[SerializeField]
	private PlayerController m_Player;

	[SerializeField]
	private EventTrigger m_HenryTrigger;

	private AudioObject m_Music;

	protected static ILocalizationParamsManager localizationParamsManager;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		SetupGameSettings();
		GameManager.Instance.HideScreenBlocker(1f);
		m_Music = GameManager.Instance.AudioManager.Play("Audio/MUS/CH5/MUS_LonelyAngelViolinEdition", AudioObjectType.MUSIC, -1);
		m_HenryTrigger.OnEnter += HandleHenryOnEnter;
		m_HenryTrigger.SetActive(active: true);
		SetupLocalization();
	}

	private void SetupGameSettings()
	{
		AudioListener.volume = 0f;
		GameManager.Instance.ParticleManager.Initialize();
		if (Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera))
		{
			GameManager.Instance.GameCamera.Brightness.SetBrightness(GameManager.Instance.PlayerSettings.Brightness);
		}
		m_Player.SetSensitivity(GameManager.Instance.PlayerSettings.Sensitivity);
		if (GameManager.Instance.AiGlobalNetwork != null)
		{
			GameManager.Instance.AiGlobalNetwork.Dispose();
		}
		GameManager.Instance.UnlockPause();
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(DOTweenUtil.DOAudioListenerVolume(1f, 1f), 0.5f), (Ease)5);
	}

	private void HandleHenryOnEnter(object sender, EventArgs e)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		m_HenryTrigger.OnEnter -= HandleHenryOnEnter;
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.STANDING_PROUD);
		});
	}

	private void SetupLocalization()
	{
		if (localizationParamsManager == null)
		{
			localizationParamsManager = new LocalizationSupport();
			if (!LocalizationManager.ParamManagers.Contains(localizationParamsManager))
			{
				Debug.Log((object)"<color=green>-- Adding Localization Support object for Globals replacement --</color>", (Object)(object)this);
				LocalizationManager.ParamManagers.Add(localizationParamsManager);
				LocalizationManager.LocalizeAll(Force: true);
			}
		}
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_Music != (Object)null)
		{
			m_Music.Clear();
			m_Music = null;
		}
		base.OnDisposed();
	}
}
