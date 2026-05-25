using System;
using UnityEngine;

public class CH1AudioLogsController : BaseController
{
	[Header("Interactables")]
	[SerializeField]
	private AudioLog m_AudioLogWally01;

	[SerializeField]
	private AudioLog m_AudioLogThomas01;

	private AudioLogModalController m_AudioLogController;

	private AudioClip m_ActiveAudioClip;

	private AudioClip m_Wally_01;

	private AudioClip m_Thomas_01;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Wally_01 = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/DIA/CH1/AudioLogs/ch1_audiolog_wally");
		m_Thomas_01 = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/DIA/CH1/AudioLogs/ch1_audiolog_thomas");
		m_AudioLogWally01.OnInteracted += HandleAudioLogWally01OnInteracted;
		m_AudioLogThomas01.OnInteracted += HandleAudioLogThomas01OnInteracted;
	}

	private void HandleAudioLogWally01OnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogWally01.SetActive(active: false);
		m_AudioLogWally01.OnInteracted -= HandleAudioLogWally01OnInteracted;
		TryAudioLogAchievement(m_AudioLogWally01.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_WALLY",
			Log = "AudioLog/WALLY_OFFERING_TO_THE_DOGS",
			LogWorldPosition = m_AudioLogWally01.transform.position
		}, m_AudioLogWally01, m_Wally_01, HandleAudioWally01OnComplete);
	}

	private void HandleAudioWally01OnComplete()
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_Wally_01)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogWally01.SetActive(active: true);
		m_AudioLogWally01.OnInteracted += HandleAudioLogWally01OnInteracted;
	}

	private void HandleAudioLogThomas01OnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogThomas01.SetActive(active: false);
		m_AudioLogThomas01.OnInteracted -= HandleAudioLogThomas01OnInteracted;
		TryAudioLogAchievement(m_AudioLogThomas01.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_THOMAS",
			Log = "AudioLog/THOMAS_DIRTY_JOB",
			LogWorldPosition = m_AudioLogThomas01.transform.position
		}, m_AudioLogThomas01, m_Thomas_01, HandleAudioThomas01OnComplete);
	}

	private void HandleAudioThomas01OnComplete()
	{
		if ((Object)(object)m_ActiveAudioClip == (Object)(object)m_Thomas_01)
		{
			m_AudioLogController.PlayOut();
		}
		m_AudioLogThomas01.SetActive(active: true);
		m_AudioLogThomas01.OnInteracted += HandleAudioLogThomas01OnInteracted;
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
		if (!GameManager.Instance.GameData.CH1AchievementData.AudioLogs.Contains(id))
		{
			GameManager.Instance.GameData.CH1AchievementData.AudioLogs.Add(id);
		}
		if (GameManager.Instance.GameData.CH1AchievementData.AudioLogs.Count >= 2)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_PAST_SPEAKS);
		}
		AudioLogAllAchievements.Check();
	}

	protected override void OnDisposed()
	{
		AudioControllerReset();
		m_ActiveAudioClip = null;
		m_Wally_01 = null;
		m_Thomas_01 = null;
		base.OnDisposed();
	}
}
