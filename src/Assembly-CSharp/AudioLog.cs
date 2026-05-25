using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AudioLog : Interactable
{
	[Header("ID")]
	[SerializeField]
	private int ID = -1;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_CassettePlayer;

	private AudioObject m_RunningAudioObject;

	private AudioObject m_AudioObject;

	private Action m_OnComplete;

	private AudioClip m_OnClip;

	private AudioClip m_OffClip;

	private AudioClip m_RunningClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_OnClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Cassette_Player_Turn_On_01");
		m_OffClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Cassette_Player_Turn_Off_01");
		m_RunningClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Cassette_Player_Run_01");
	}

	public void Play(string audioKey, Action onComplete = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		m_OnComplete = onComplete;
		m_CassettePlayer.localScale = Vector3.one;
		ShortcutExtensions.DOKill((Component)(object)m_CassettePlayer, false);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_CassettePlayer, 1.01f, 0.25f), (Ease)7), -1, (LoopType)1);
		GameManager.Instance.AudioManager.PlayAtPosition(m_OnClip, m_CassettePlayer.position);
		m_RunningAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_RunningClip, m_CassettePlayer.position, AudioObjectType.SOUND_EFFECT, -1);
		m_AudioObject = GameManager.Instance.AudioManager.PlayAtPosition(audioKey, m_CassettePlayer.position, AudioObjectType.DIALOGUE);
		m_AudioObject.OnComplete += HandleAudioObjectOnComplete;
	}

	public void Play(AudioClip audioClip, Action onComplete = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		m_OnComplete = onComplete;
		SetActive(active: false);
		m_CassettePlayer.localScale = Vector3.one;
		ShortcutExtensions.DOKill((Component)(object)m_CassettePlayer, false);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_CassettePlayer, 1.01f, 0.25f), (Ease)7), -1, (LoopType)1);
		GameManager.Instance.AudioManager.PlayAtPosition(m_OnClip, m_CassettePlayer.position);
		m_RunningAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_RunningClip, m_CassettePlayer.position, AudioObjectType.SOUND_EFFECT, -1);
		m_AudioObject = GameManager.Instance.AudioManager.PlayAtPosition(audioClip, m_CassettePlayer.position, AudioObjectType.DIALOGUE);
		m_AudioObject.OnComplete += HandleAudioObjectOnComplete;
	}

	private void HandleAudioObjectOnComplete(object sender, EventArgs e)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		m_AudioObject.OnComplete -= HandleAudioObjectOnComplete;
		m_AudioObject = null;
		m_RunningAudioObject.Clear();
		m_RunningAudioObject = null;
		ShortcutExtensions.DOKill((Component)(object)m_CassettePlayer, false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_CassettePlayer, 1f, 0.25f), (Ease)7);
		if (!base.isSingleInteraction)
		{
			SetActive(active: true);
		}
		GameManager.Instance.AudioManager.PlayAtPosition(m_OffClip, m_CassettePlayer.position);
		((MonoBehaviour)this).StartCoroutine(DelayOnComplete());
	}

	private IEnumerator DelayOnComplete()
	{
		yield return (object)new WaitForSeconds(0.5f);
		if (m_OnComplete != null)
		{
			m_OnComplete();
		}
	}

	public int SetID(int CurrentIDCount)
	{
		if (ID != -1)
		{
			return ID;
		}
		return ID = ++CurrentIDCount;
	}

	public int GetID()
	{
		if (ID != -1)
		{
			return ID;
		}
		return -1;
	}

	public void ResetID()
	{
		ID = -1;
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_CassettePlayer, false);
		if ((Object)(object)m_AudioObject != (Object)null)
		{
			m_AudioObject.OnComplete -= HandleAudioObjectOnComplete;
			m_AudioObject.Clear();
			m_AudioObject = null;
		}
		if ((Object)(object)m_RunningAudioObject != (Object)null)
		{
			m_RunningAudioObject.Clear();
			m_RunningAudioObject = null;
		}
		m_OnComplete = null;
		m_OnClip = null;
		m_OffClip = null;
		m_RunningClip = null;
		base.OnDisposed();
	}
}
