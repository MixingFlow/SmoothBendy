using System;
using UnityEngine;

public class CH4AudioLogController : BaseController
{
	[Header("Interactables")]
	[SerializeField]
	private AudioLog m_AudioLogGrantTransform;

	[SerializeField]
	private AudioLog m_AudioLogWallyTransform;

	[SerializeField]
	private AudioLog m_AudioLogBertTransform;

	[SerializeField]
	private AudioLog m_AudioLogLacieTransform;

	[SerializeField]
	private AudioLog m_AudioLogJoeyTransform;

	[SerializeField]
	private AudioLog m_AudioLogSusieTransform;

	private AudioLogModalController m_AudioLogController;

	private AudioClip m_ActiveAudioClip;

	private AudioClip m_AudioClipGrantTransform;

	private AudioClip m_AudioClipWallyTransform;

	private AudioClip m_AudioClipBertTransform;

	private AudioClip m_AudioClipLacieTransform;

	private AudioClip m_AudioClipJoeyTransform;

	private AudioClip m_AudioClipSusieTransform;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AudioClipGrantTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/AudioLogs/ch4_audiolog_grant");
		m_AudioClipWallyTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/AudioLogs/ch4_audiolog_wally");
		m_AudioClipBertTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/AudioLogs/ch4_audiolog_bert");
		m_AudioClipLacieTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/AudioLogs/ch4_audiolog_lacie");
		m_AudioClipJoeyTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/AudioLogs/ch4_audiolog_joey");
		m_AudioClipSusieTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/AudioLogs/ch4_audiolog_susie");
		m_AudioLogGrantTransform.OnInteracted += HandleAudioLogGrantTransformOnInteracted;
		m_AudioLogWallyTransform.OnInteracted += HandleAudioLogWallyTransformOnInteracted;
		m_AudioLogBertTransform.OnInteracted += HandleAudioLogBertTransformOnInteracted;
		m_AudioLogLacieTransform.OnInteracted += HandleAudioLogLacieTransformOnInteracted;
		m_AudioLogJoeyTransform.OnInteracted += HandleAudioLogJoeyTransformOnInteracted;
		m_AudioLogSusieTransform.OnInteracted += HandleAudioLogSusieTransformOnInteracted;
	}

	private void HandleAudioLogGrantTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogGrantTransform.SetActive(active: false);
		m_AudioLogGrantTransform.OnInteracted -= HandleAudioLogGrantTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogGrantTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/UNKNOWN_NAME",
			Log = "AudioLog/UNKNOWN_INDISCERNIBLE",
			LogWorldPosition = m_AudioLogGrantTransform.transform.position
		}, m_AudioLogGrantTransform, m_AudioClipGrantTransform, HandleAudioLogGrantTransformOnComplete);
	}

	private void HandleAudioLogGrantTransformOnComplete()
	{
		CheckClip(m_AudioClipGrantTransform);
		m_AudioLogGrantTransform.SetActive(active: true);
		m_AudioLogGrantTransform.OnInteracted += HandleAudioLogGrantTransformOnInteracted;
	}

	private void HandleAudioLogWallyTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogWallyTransform.SetActive(active: false);
		m_AudioLogWallyTransform.OnInteracted -= HandleAudioLogWallyTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogWallyTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_WALLY",
			Log = "AudioLog/WALLY_WAREHOUSE_GAMES",
			LogWorldPosition = m_AudioLogWallyTransform.transform.position
		}, m_AudioLogWallyTransform, m_AudioClipWallyTransform, HandleAudioLogWallyTransformOnComplete);
	}

	private void HandleAudioLogWallyTransformOnComplete()
	{
		CheckClip(m_AudioClipWallyTransform);
		m_AudioLogWallyTransform.SetActive(active: true);
		m_AudioLogWallyTransform.OnInteracted += HandleAudioLogWallyTransformOnInteracted;
	}

	private void HandleAudioLogBertTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogBertTransform.SetActive(active: false);
		m_AudioLogBertTransform.OnInteracted -= HandleAudioLogBertTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogBertTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_BERT",
			Log = "AudioLog/BERT_COLOSSAL_WONDERS",
			LogWorldPosition = m_AudioLogBertTransform.transform.position
		}, m_AudioLogBertTransform, m_AudioClipBertTransform, HandleAudioLogBertTransformOnComplete);
	}

	private void HandleAudioLogBertTransformOnComplete()
	{
		CheckClip(m_AudioClipBertTransform);
		m_AudioLogBertTransform.SetActive(active: true);
		m_AudioLogBertTransform.OnInteracted += HandleAudioLogBertTransformOnInteracted;
	}

	private void HandleAudioLogLacieTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogLacieTransform.SetActive(active: false);
		m_AudioLogLacieTransform.OnInteracted -= HandleAudioLogLacieTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogLacieTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_LACIE",
			Log = "AudioLog/LACIE_SILLY_GAMES",
			LogWorldPosition = m_AudioLogLacieTransform.transform.position
		}, m_AudioLogLacieTransform, m_AudioClipLacieTransform, HandleAudioLogLacieTransformOnComplete);
	}

	private void HandleAudioLogLacieTransformOnComplete()
	{
		CheckClip(m_AudioClipLacieTransform);
		m_AudioLogLacieTransform.SetActive(active: true);
		m_AudioLogLacieTransform.OnInteracted += HandleAudioLogLacieTransformOnInteracted;
	}

	private void HandleAudioLogJoeyTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogJoeyTransform.SetActive(active: false);
		m_AudioLogJoeyTransform.OnInteracted -= HandleAudioLogJoeyTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogJoeyTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_JOEY",
			Log = "AudioLog/JOEY_DREAMING",
			LogWorldPosition = m_AudioLogJoeyTransform.transform.position
		}, m_AudioLogJoeyTransform, m_AudioClipJoeyTransform, HandleAudioLogJoeyTransformOnComplete);
	}

	private void HandleAudioLogJoeyTransformOnComplete()
	{
		CheckClip(m_AudioClipJoeyTransform);
		m_AudioLogJoeyTransform.SetActive(active: true);
		m_AudioLogJoeyTransform.OnInteracted += HandleAudioLogJoeyTransformOnInteracted;
	}

	private void HandleAudioLogSusieTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogSusieTransform.SetActive(active: false);
		m_AudioLogSusieTransform.OnInteracted -= HandleAudioLogSusieTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogSusieTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_SUSIE",
			Log = "AudioLog/SUSIE_PERFECT",
			LogWorldPosition = m_AudioLogSusieTransform.transform.position
		}, m_AudioLogSusieTransform, m_AudioClipSusieTransform, HandleAudioLogSusieTransformOnComplete);
	}

	private void HandleAudioLogSusieTransformOnComplete()
	{
		CheckClip(m_AudioClipSusieTransform);
		m_AudioLogSusieTransform.SetActive(active: true);
		m_AudioLogSusieTransform.OnInteracted += HandleAudioLogSusieTransformOnInteracted;
	}

	private void CheckClip(AudioClip clip)
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)clip)
		{
			m_AudioLogController.PlayOut();
		}
	}

	private void PlayAudioLog(AudioLogDataVO vo, AudioLog audioLog, AudioClip audioClip, Action onComplete)
	{
		AudioControllerReset();
		m_ActiveAudioClip = audioClip;
		m_AudioLogController = GameManager.Instance.UIManager.Show<AudioLogModalController>("UI/Modals/AudioLogModalController", "MODAL", vo);
		audioLog.Play(m_ActiveAudioClip, onComplete);
	}

	private void AudioControllerReset()
	{
		if ((Object)(object)m_AudioLogController != (Object)null)
		{
			m_AudioLogController.Dispose();
			m_AudioLogController = null;
		}
	}

	private void TryAudioLogAchievement(int id)
	{
		if (!GameManager.Instance.GameData.CH4AchievementData.AudioLogs.Contains(id))
		{
			GameManager.Instance.GameData.CH4AchievementData.AudioLogs.Add(id);
		}
		if (GameManager.Instance.GameData.CH4AchievementData.AudioLogs.Count >= 6)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.STILL_LISTENING);
		}
		AudioLogAllAchievements.Check();
	}

	protected override void OnDisposed()
	{
		AudioControllerReset();
		m_ActiveAudioClip = null;
		m_AudioClipGrantTransform = null;
		m_AudioClipGrantTransform = null;
		m_AudioClipWallyTransform = null;
		m_AudioClipBertTransform = null;
		m_AudioClipLacieTransform = null;
		m_AudioClipJoeyTransform = null;
		m_AudioClipSusieTransform = null;
		base.OnDisposed();
	}
}
