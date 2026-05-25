using System;
using UnityEngine;

public class CH3AudioLogController : BaseController
{
	[Header("Interactables")]
	[SerializeField]
	private AudioLog m_AudioLogGrantGenius;

	[SerializeField]
	private AudioLog m_AudioLogShawnCrooked;

	[SerializeField]
	private AudioLog m_AudioLogNormanTrouble;

	[SerializeField]
	private AudioLog m_AudioLogWallySmile;

	[SerializeField]
	private AudioLog m_AudioLogSusieApart;

	[SerializeField]
	private AudioLog m_AudioLogSusieLunch;

	[SerializeField]
	private AudioLog m_AudioLogJoeyDrewBelief;

	[SerializeField]
	private AudioLog m_AudioLogHenry;

	[SerializeField]
	private AudioLog m_AudioLogThomas;

	[SerializeField]
	private AudioLog m_AudioLogWallyThomas;

	private AudioLogModalController m_AudioLogController;

	private AudioClip m_ActiveAudioClip;

	private AudioClip m_AudioClipGrantGenius;

	private AudioClip m_AudioClipShawnCrooked;

	private AudioClip m_AudioClipNormanTrouble;

	private AudioClip m_AudioClipWallySmile;

	private AudioClip m_AudioClipSusieApart;

	private AudioClip m_AudioClipSusieLunch;

	private AudioClip m_AudioClipJoeyDrewBelief;

	private AudioClip m_AudioClipHenry;

	private AudioClip m_AudioClipThomams;

	private AudioClip m_AudioClipWallyThomas;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AudioClipGrantGenius = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_grant_thegeniusupstairs");
		m_AudioClipShawnCrooked = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_shawn_crookedsmiles");
		m_AudioClipNormanTrouble = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_norman_lookingfortrouble");
		m_AudioClipWallySmile = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_wally_crackasmile");
		m_AudioClipSusieApart = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_susie_everythingiscomingapart");
		m_AudioClipSusieLunch = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_susie_lunchwithjoey");
		m_AudioClipJoeyDrewBelief = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_joeydrew_timetobelieve");
		m_AudioClipHenry = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_henry");
		m_AudioClipThomams = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_Thomas_CuttingCorners");
		m_AudioClipWallyThomas = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/AudioLogs/CH3_AudioLog_Wally_Thomas");
		m_AudioLogGrantGenius.OnInteracted += HandleAudioLogGrantGeniusOnInteracted;
		m_AudioLogShawnCrooked.OnInteracted += HandleAudioLogShawnCrookedOnInteracted;
		m_AudioLogNormanTrouble.OnInteracted += HandleAudioLogNormanTroubleOnInteracted;
		m_AudioLogWallySmile.OnInteracted += HandleAudioLogWallySmileOnInteracted;
		m_AudioLogSusieApart.OnInteracted += HandleAudioLogSusieApartOnInteracted;
		m_AudioLogSusieLunch.OnInteracted += HandleAudioLogSusieLunchOnInteracted;
		m_AudioLogJoeyDrewBelief.OnInteracted += HandleAudioLogJoeyDrewBeliefOnInteracted;
		m_AudioLogHenry.OnInteracted += HandleAudioLogHenryOnInteracted;
		m_AudioLogThomas.OnInteracted += HandleAudioLogThomasOnInteracted;
		m_AudioLogWallyThomas.OnInteracted += HandleAudioLogWallyThomasOnInteracted;
	}

	private void HandleAudioLogGrantGeniusOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogGrantGenius.SetActive(active: false);
		m_AudioLogGrantGenius.OnInteracted -= HandleAudioLogGrantGeniusOnInteracted;
		TryAudioLogAchievement(m_AudioLogGrantGenius.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_GRANT",
			Log = "AudioLog/GRANT_THE_GENIUS_UPSTAIRS",
			LogWorldPosition = m_AudioLogGrantGenius.transform.position
		}, m_AudioLogGrantGenius, m_AudioClipGrantGenius, HandleAudioLogGrantGeniusOnComplete);
	}

	private void HandleAudioLogGrantGeniusOnComplete()
	{
		CheckClip(m_AudioClipGrantGenius);
		m_AudioLogGrantGenius.SetActive(active: true);
		m_AudioLogGrantGenius.OnInteracted += HandleAudioLogGrantGeniusOnInteracted;
	}

	private void HandleAudioLogShawnCrookedOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogShawnCrooked.SetActive(active: false);
		m_AudioLogShawnCrooked.OnInteracted -= HandleAudioLogShawnCrookedOnInteracted;
		TryAudioLogAchievement(m_AudioLogShawnCrooked.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_SHAWN",
			Log = "AudioLog/SHAWN_CROOKED_SMILES",
			LogWorldPosition = m_AudioLogShawnCrooked.transform.position
		}, m_AudioLogShawnCrooked, m_AudioClipShawnCrooked, HandleAudioLogShawnCrookedOnComplete);
	}

	private void HandleAudioLogShawnCrookedOnComplete()
	{
		CheckClip(m_AudioClipShawnCrooked);
		m_AudioLogShawnCrooked.SetActive(active: true);
		m_AudioLogShawnCrooked.OnInteracted += HandleAudioLogShawnCrookedOnInteracted;
	}

	private void HandleAudioLogNormanTroubleOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogNormanTrouble.SetActive(active: false);
		m_AudioLogNormanTrouble.OnInteracted -= HandleAudioLogNormanTroubleOnInteracted;
		TryAudioLogAchievement(m_AudioLogNormanTrouble.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_NORMAN",
			Log = "AudioLog/NORMAN_LOOKING_FOR_TROUBLE",
			LogWorldPosition = m_AudioLogNormanTrouble.transform.position
		}, m_AudioLogNormanTrouble, m_AudioClipNormanTrouble, HandleAudioLogNormanTroubleOnComplete);
	}

	private void HandleAudioLogNormanTroubleOnComplete()
	{
		CheckClip(m_AudioClipNormanTrouble);
		m_AudioLogNormanTrouble.SetActive(active: true);
		m_AudioLogNormanTrouble.OnInteracted += HandleAudioLogNormanTroubleOnInteracted;
	}

	private void HandleAudioLogWallySmileOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogWallySmile.SetActive(active: false);
		m_AudioLogWallySmile.OnInteracted -= HandleAudioLogWallySmileOnInteracted;
		TryAudioLogAchievement(m_AudioLogWallySmile.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_WALLY",
			Log = "AudioLog/WALLY_CRACK_A_SMILE",
			LogWorldPosition = m_AudioLogWallySmile.transform.position
		}, m_AudioLogWallySmile, m_AudioClipWallySmile, HandleAudioLogWallySmileOnComplete);
	}

	private void HandleAudioLogWallySmileOnComplete()
	{
		CheckClip(m_AudioClipWallySmile);
		m_AudioLogWallySmile.SetActive(active: true);
		m_AudioLogWallySmile.OnInteracted += HandleAudioLogWallySmileOnInteracted;
	}

	private void HandleAudioLogSusieApartOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogSusieApart.SetActive(active: false);
		m_AudioLogSusieApart.OnInteracted -= HandleAudioLogSusieApartOnInteracted;
		TryAudioLogAchievement(m_AudioLogSusieApart.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_SUSIE",
			Log = "AudioLog/SUSIE_EVERYTHING_IS_COMING_APART",
			LogWorldPosition = m_AudioLogSusieApart.transform.position
		}, m_AudioLogSusieApart, m_AudioClipSusieApart, HandleAudioLogSusieApartOnComplete);
	}

	private void HandleAudioLogSusieApartOnComplete()
	{
		CheckClip(m_AudioClipSusieApart);
		m_AudioLogSusieApart.SetActive(active: true);
		m_AudioLogSusieApart.OnInteracted += HandleAudioLogSusieApartOnInteracted;
	}

	private void HandleAudioLogSusieLunchOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogSusieLunch.SetActive(active: false);
		m_AudioLogSusieLunch.OnInteracted -= HandleAudioLogSusieLunchOnInteracted;
		TryAudioLogAchievement(m_AudioLogSusieLunch.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_SUSIE",
			Log = "AudioLog/SUSIE_LUNCH_WITH_JOEY",
			LogWorldPosition = m_AudioLogSusieLunch.transform.position
		}, m_AudioLogSusieLunch, m_AudioClipSusieLunch, HandleAudioLogSusieLunchOnComplete);
	}

	private void HandleAudioLogSusieLunchOnComplete()
	{
		CheckClip(m_AudioClipSusieLunch);
		m_AudioLogSusieLunch.SetActive(active: true);
		m_AudioLogSusieLunch.OnInteracted += HandleAudioLogSusieLunchOnInteracted;
	}

	private void HandleAudioLogJoeyDrewBeliefOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogJoeyDrewBelief.SetActive(active: false);
		m_AudioLogJoeyDrewBelief.OnInteracted -= HandleAudioLogJoeyDrewBeliefOnInteracted;
		TryAudioLogAchievement(m_AudioLogJoeyDrewBelief.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_JOEY",
			Log = "AudioLog/JOEY_BELIEF",
			LogWorldPosition = m_AudioLogJoeyDrewBelief.transform.position
		}, m_AudioLogJoeyDrewBelief, m_AudioClipJoeyDrewBelief, HandleAudioLogJoeyDrewBeliefOnComplete);
	}

	private void HandleAudioLogJoeyDrewBeliefOnComplete()
	{
		CheckClip(m_AudioClipJoeyDrewBelief);
		m_AudioLogJoeyDrewBelief.SetActive(active: true);
		m_AudioLogJoeyDrewBelief.OnInteracted += HandleAudioLogJoeyDrewBeliefOnInteracted;
	}

	private void HandleAudioLogHenryOnInteracted(object sender, EventArgs e)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogHenry.SetActive(active: false);
		m_AudioLogHenry.OnInteracted -= HandleAudioLogHenryOnInteracted;
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.LONG_FORGOTTEN_SELF);
		TryAudioLogAchievement(m_AudioLogHenry.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_HENRY",
			Log = "AudioLog/HENRY_NEW_CHARACTER",
			LogWorldPosition = m_AudioLogHenry.transform.position
		}, m_AudioLogHenry, m_AudioClipHenry, HandleAudioLogHenryOnComplete);
	}

	private void HandleAudioLogHenryOnComplete()
	{
		CheckClip(m_AudioClipHenry);
		m_AudioLogHenry.SetActive(active: true);
		m_AudioLogHenry.OnInteracted += HandleAudioLogHenryOnInteracted;
	}

	private void HandleAudioLogThomasOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogThomas.SetActive(active: false);
		m_AudioLogThomas.OnInteracted -= HandleAudioLogThomasOnInteracted;
		TryAudioLogAchievement(m_AudioLogThomas.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/NAME_THOMAS",
			Log = "AudioLog/THOMAS_CUTTING_CORNERS",
			LogWorldPosition = m_AudioLogThomas.transform.position
		}, m_AudioLogThomas, m_AudioClipThomams, HandleAudioLogThomasOnComplete);
	}

	private void HandleAudioLogThomasOnComplete()
	{
		CheckClip(m_AudioClipThomams);
		m_AudioLogThomas.SetActive(active: true);
		m_AudioLogThomas.OnInteracted += HandleAudioLogThomasOnInteracted;
	}

	private void HandleAudioLogWallyThomasOnInteracted(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLogWallyThomas.SetActive(active: false);
		m_AudioLogWallyThomas.OnInteracted -= HandleAudioLogWallyThomasOnInteracted;
		TryAudioLogAchievement(m_AudioLogWallyThomas.GetID());
		PlayAudioLog(new AudioLogDataVO
		{
			Name = "AudioLog/WALLY_AND_THOMAS_NAME",
			Log = "AudioLog/WALLY_AND_THOMAS_THE_CREATORS",
			LogWorldPosition = m_AudioLogWallyThomas.transform.position
		}, m_AudioLogWallyThomas, m_AudioClipWallyThomas, HandleAudioLogWallyThomasOnComplete);
	}

	private void HandleAudioLogWallyThomasOnComplete()
	{
		CheckClip(m_AudioClipWallyThomas);
		m_AudioLogWallyThomas.SetActive(active: true);
		m_AudioLogWallyThomas.OnInteracted += HandleAudioLogWallyThomasOnInteracted;
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
		if (!GameManager.Instance.GameData.CH3AchievementData.AudioLogs.Contains(id))
		{
			GameManager.Instance.GameData.CH3AchievementData.AudioLogs.Add(id);
		}
		if (GameManager.Instance.GameData.CH3AchievementData.AudioLogs.Count >= 10)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.HEARING_VOICES);
		}
		AudioLogAllAchievements.Check();
	}

	protected override void OnDisposed()
	{
		AudioControllerReset();
		m_ActiveAudioClip = null;
		m_AudioClipGrantGenius = null;
		m_AudioClipShawnCrooked = null;
		m_AudioClipNormanTrouble = null;
		m_AudioClipWallySmile = null;
		m_AudioClipSusieApart = null;
		m_AudioClipSusieLunch = null;
		m_AudioClipJoeyDrewBelief = null;
		m_AudioClipHenry = null;
		m_AudioClipWallyThomas = null;
		m_AudioClipThomams = null;
		base.OnDisposed();
	}
}
