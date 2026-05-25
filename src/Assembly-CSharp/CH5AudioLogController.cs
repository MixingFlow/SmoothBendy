using System;
using UnityEngine;

public class CH5AudioLogController : BaseController
{
	[Header("Interactables")]
	[SerializeField]
	private AudioLog m_AudioLogThomasTransform;

	[SerializeField]
	private AudioLog m_AudioLogJoeyMemoTransform;

	[SerializeField]
	private AudioLog m_AudioLogJoeyTommyTransform;

	[SerializeField]
	private AudioLog m_AudioLogWallyTransform;

	[SerializeField]
	private AudioLog m_AudioLogJoeySusieTransform;

	private AudioLogModalController m_AudioLogController;

	private AudioClip m_ActiveAudioClip;

	private AudioClip m_AudioClipThomasTransform;

	private AudioClip m_AudioClipWallyTransform;

	private AudioClip m_AudioClipJoeyMemoTransform;

	private AudioClip m_AudioClipJoeyTommyTransform;

	private AudioClip m_AudioClipJoeySusieTransform;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AudioClipThomasTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/AudioLogs/ch5_audiolog_thomasconnor");
		m_AudioClipWallyTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/AudioLogs/ch5_audiolog_wallyfranks");
		m_AudioClipJoeyMemoTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/AudioLogs/ch5_audiolog_joeydrew01");
		m_AudioClipJoeyTommyTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/AudioLogs/ch5_audiolog_joeydrew02");
		m_AudioClipJoeySusieTransform = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/AudioLogs/ch5_audiolog_joeydrew03");
		m_AudioLogThomasTransform.OnInteracted += HandleAudioLogThomasTransformOnInteracted;
		m_AudioLogWallyTransform.OnInteracted += HandleAudioLogWallyTransformOnInteracted;
		m_AudioLogJoeyMemoTransform.OnInteracted += HandleAudioLogJoeyMemoTransformOnInteracted;
		m_AudioLogJoeyTommyTransform.OnInteracted += HandleAudioLogJoeyTommyTransformOnInteracted;
		m_AudioLogJoeySusieTransform.OnInteracted += HandleAudioLogJoeySusieTransformOnInteracted;
	}

	private void HandleAudioLogThomasTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogThomasTransform.SetActive(active: false);
		m_AudioLogThomasTransform.OnInteracted -= HandleAudioLogThomasTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogThomasTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_THOMAS",
			Log = "AudioLog/THOMAS_PROGRESS_REPORT",
			LogWorldPosition = m_AudioLogThomasTransform.transform.position
		}, m_AudioLogThomasTransform, m_AudioClipThomasTransform, HandleAudioLogThomasTransformOnComplete);
	}

	private void HandleAudioLogThomasTransformOnComplete()
	{
		CheckClip(m_AudioClipThomasTransform);
		m_AudioLogThomasTransform.SetActive(active: true);
		m_AudioLogThomasTransform.OnInteracted += HandleAudioLogThomasTransformOnInteracted;
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
			Log = "AudioLog/WALLY_CAKE",
			LogWorldPosition = m_AudioLogWallyTransform.transform.position
		}, m_AudioLogWallyTransform, m_AudioClipWallyTransform, HandleAudioLogWallyTransformOnComplete);
	}

	private void HandleAudioLogWallyTransformOnComplete()
	{
		CheckClip(m_AudioClipWallyTransform);
		m_AudioLogWallyTransform.SetActive(active: true);
		m_AudioLogWallyTransform.OnInteracted += HandleAudioLogWallyTransformOnInteracted;
	}

	private void HandleAudioLogJoeyMemoTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogJoeyMemoTransform.SetActive(active: false);
		m_AudioLogJoeyMemoTransform.OnInteracted -= HandleAudioLogJoeyMemoTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogJoeyMemoTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_JOEY",
			Log = "AudioLog/JOEY_MEMO",
			LogWorldPosition = m_AudioLogJoeyMemoTransform.transform.position
		}, m_AudioLogJoeyMemoTransform, m_AudioClipJoeyMemoTransform, HandleAudioLogJoeyMemoTransformOnComplete);
	}

	private void HandleAudioLogJoeyMemoTransformOnComplete()
	{
		CheckClip(m_AudioClipJoeyMemoTransform);
		m_AudioLogJoeyMemoTransform.SetActive(active: true);
		m_AudioLogJoeyMemoTransform.OnInteracted += HandleAudioLogJoeyMemoTransformOnInteracted;
	}

	private void HandleAudioLogJoeyTommyTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogJoeyTommyTransform.SetActive(active: false);
		m_AudioLogJoeyTommyTransform.OnInteracted -= HandleAudioLogJoeyTommyTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogJoeyTommyTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_JOEY",
			Log = "AudioLog/JOEY_LISTENTOMMY",
			LogWorldPosition = m_AudioLogJoeyTommyTransform.transform.position
		}, m_AudioLogJoeyTommyTransform, m_AudioClipJoeyTommyTransform, HandleAudioLogJoeyTommyTransformOnComplete);
	}

	private void HandleAudioLogJoeyTommyTransformOnComplete()
	{
		CheckClip(m_AudioClipJoeyTommyTransform);
		m_AudioLogJoeyTommyTransform.SetActive(active: true);
		m_AudioLogJoeyTommyTransform.OnInteracted += HandleAudioLogJoeyTommyTransformOnInteracted;
	}

	private void HandleAudioLogJoeySusieTransformOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogJoeySusieTransform.SetActive(active: false);
		m_AudioLogJoeySusieTransform.OnInteracted -= HandleAudioLogJoeySusieTransformOnInteracted;
		TryAudioLogAchievement(m_AudioLogJoeySusieTransform.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_JOEY",
			Log = "AudioLog/JOEY_SUSIEMEETING",
			LogWorldPosition = m_AudioLogJoeySusieTransform.transform.position
		}, m_AudioLogJoeySusieTransform, m_AudioClipJoeySusieTransform, HandleAudioLogJoeySusieTransformOnComplete);
	}

	private void HandleAudioLogJoeySusieTransformOnComplete()
	{
		CheckClip(m_AudioClipJoeySusieTransform);
		m_AudioLogJoeySusieTransform.SetActive(active: true);
		m_AudioLogJoeySusieTransform.OnInteracted += HandleAudioLogJoeySusieTransformOnInteracted;
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
		if (!GameManager.Instance.GameData.CH5AchievementData.AudioLogs.Contains(id))
		{
			GameManager.Instance.GameData.CH5AchievementData.AudioLogs.Add(id);
		}
		if (GameManager.Instance.GameData.CH5AchievementData.AudioLogs.Count >= 5)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.NOW_HEAR_THIS);
		}
		AudioLogAllAchievements.Check();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
