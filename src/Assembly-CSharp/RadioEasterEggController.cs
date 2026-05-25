using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class RadioEasterEggController : TMGMonoBehaviour
{
	[Header("Chapter")]
	[SerializeField]
	private Chapters m_Chapter;

	[Header("Interactable")]
	[SerializeField]
	private Interactable m_Radio;

	[SerializeField]
	private Transform m_Container;

	private AudioObject m_RadioAudioObject;

	private AudioClip m_RadioAudioClip;

	private string m_SongAssetKey;

	private AchievementName m_AchievementAssetKey;

	public override void Init()
	{
		base.Init();
		if (m_Chapter == Chapters.ONE)
		{
			m_SongAssetKey = "Audio/EasterEggs/EasterEggKyleBendySong";
			m_AchievementAssetKey = AchievementName.CROONER_TUNER;
		}
		else if (m_Chapter == Chapters.TWO)
		{
			m_SongAssetKey = "Audio/EasterEggs/EasterEggDAGamesBuildOurMachine";
			m_AchievementAssetKey = AchievementName.COAST_TO_COAST;
		}
		else if (m_Chapter == Chapters.THREE)
		{
			m_SongAssetKey = "Audio/EasterEggs/EasterEggInkMusical";
			m_AchievementAssetKey = AchievementName.TURN_IT_UP;
		}
		else if (m_Chapter == Chapters.FOUR)
		{
			m_SongAssetKey = "Audio/EasterEggs/EasterEggJTMusicCantBeErased";
			m_AchievementAssetKey = AchievementName.FINGER_WAGGIN;
		}
		else if (m_Chapter == Chapters.FIVE)
		{
			m_SongAssetKey = "Audio/EasterEggs/EasterEggLonelyAngel";
			m_AchievementAssetKey = AchievementName.TOE_TAPPIN;
		}
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_RadioAudioClip = GameManager.Instance.AssetManager.GetAsset<AudioClip>(m_SongAssetKey);
		m_Radio.OnInteracted += HandleRadioOnInteracted;
	}

	private void HandleRadioOnInteracted(object sender, EventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		m_Radio.OnInteracted -= HandleRadioOnInteracted;
		m_RadioAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_RadioAudioClip, m_Radio.transform.position);
		m_RadioAudioObject.OnComplete += HandleRadioAudioOnComplete;
		if (Object.op_Implicit((Object)(object)m_Container))
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_Container, 1.01f, 0.25f), (Ease)7), -1, (LoopType)1);
		}
		else
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_Radio.transform, 1.01f, 0.25f), (Ease)7), -1, (LoopType)1);
		}
		GameManager.Instance.AchievementManager.SetAchievement(m_AchievementAssetKey);
		CheckAllAchievments();
	}

	private void CheckAllAchievments()
	{
		if (GameManager.Instance.AchievementManager.GetAchievement(AchievementName.CROONER_TUNER) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.COAST_TO_COAST) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.TURN_IT_UP) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.FINGER_WAGGIN) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.TOE_TAPPIN))
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.GOLD_RECORD);
		}
	}

	private void HandleRadioAudioOnComplete(object sender, EventArgs e)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		m_RadioAudioObject.OnComplete -= HandleRadioAudioOnComplete;
		if (Object.op_Implicit((Object)(object)m_Container))
		{
			ShortcutExtensions.DOKill((Component)(object)m_Container, false);
			m_Container.localScale = Vector3.one;
		}
		else
		{
			ShortcutExtensions.DOKill((Component)(object)m_Radio.transform, false);
			m_Radio.transform.localScale = Vector3.one;
		}
		m_RadioAudioClip = null;
		if ((Object)(object)m_RadioAudioObject != (Object)null)
		{
			m_RadioAudioObject.Clear();
			m_RadioAudioObject = null;
		}
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_Radio != (Object)null)
		{
			ShortcutExtensions.DOKill((Component)(object)m_Radio.transform, false);
		}
		if (Object.op_Implicit((Object)(object)m_Container))
		{
			ShortcutExtensions.DOKill((Component)(object)m_Container, false);
		}
		m_RadioAudioClip = null;
		if ((Object)(object)m_RadioAudioObject != (Object)null)
		{
			m_RadioAudioObject.Clear();
			m_RadioAudioObject = null;
		}
		base.OnDisposed();
	}
}
