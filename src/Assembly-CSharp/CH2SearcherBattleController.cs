using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH2SearcherBattleController : BaseController
{
	[Header("Objective: Defeat The Searchers")]
	[SerializeField]
	private GameObject m_SammyBalcony;

	[SerializeField]
	private GameObject m_PostFightSearchers;

	[SerializeField]
	private List<SearcherAi> m_Searchers;

	[SerializeField]
	private EventTrigger m_FinaleTrigger;

	[SerializeField]
	private EventTrigger m_ExitTrigger;

	[SerializeField]
	private BaseDoorController m_Door;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioObject m_SearcherMusic;

	private AudioClip m_SearcherMusicClip;

	private AudioClip m_SearcherMusicStartClip;

	public override void InitOnComplete()
	{
		m_SammyBalcony.SetActive(false);
		m_FinaleTrigger.SetActive(active: false);
		m_ExitTrigger.SetActive(active: false);
		for (int i = 0; i < m_Searchers.Count; i++)
		{
			SearcherAi searcherAi = m_Searchers[i];
			searcherAi.gameObject.SetActive(false);
		}
		m_SearcherMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_The_Searchers");
		m_SearcherMusicStartClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_SearcherStartCue01");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InfirmaryObjective.IsStarted)
		{
			ForceComplete();
			return;
		}
		m_Door.Close(0f, (Ease)1);
		m_Door.Lock();
		m_SammyBalcony.SetActive(true);
		m_FinaleTrigger.OnEnter += HandleFinaleTriggerOnEnter;
		m_FinaleTrigger.SetActive(active: true);
	}

	private void HandleFinaleTriggerOnEnter(object sender, EventArgs e)
	{
		m_FinaleTrigger.OnEnter -= HandleFinaleTriggerOnEnter;
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned += HandlePlayerOnSpawned;
		for (int i = 0; i < m_Searchers.Count; i++)
		{
			SearcherAi searcherAi = m_Searchers[i];
			searcherAi.OnRespawn += HandleSearcherOnRespawn;
			searcherAi.gameObject.SetActive(true);
		}
		AudioObject audioObject = GameManager.Instance.AudioManager.Play(m_SearcherMusicStartClip, AudioObjectType.MUSIC);
		audioObject.OnComplete += delegate
		{
			m_SearcherMusic = GameManager.Instance.AudioManager.Play(m_SearcherMusicClip, AudioObjectType.MUSIC, -1);
		};
	}

	private void HandlePlayerOnSpawned(object sender, EventArgs e)
	{
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		ClearSearcherMusic();
		m_Door.ForceOpen(145f);
		m_Door.Lock();
		m_SammyBalcony.SetActive(false);
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SanctuaryObjective.IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InfirmaryObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void HandleSearcherOnRespawn(object sender, EventArgs e)
	{
		SearcherAi item = (SearcherAi)sender;
		if (m_Searchers.Contains(item))
		{
			m_Searchers.Remove(item);
		}
		if (m_Searchers.Count <= 0)
		{
			m_PostFightSearchers.SetActive(true);
			OpenDoor();
		}
	}

	public void OpenDoor()
	{
		ClearSearcherMusic();
		m_Door.Unlock();
		m_ExitTrigger.OnEnter += HandleExitTriggerOnEnter;
		m_ExitTrigger.SetActive(active: true);
	}

	private void HandleExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_ExitTrigger.OnEnter -= HandleExitTriggerOnEnter;
		m_SammyBalcony.SetActive(false);
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SanctuaryObjective.IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InfirmaryObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void ClearSearcherMusic()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		if (!((Object)(object)m_SearcherMusic != (Object)null))
		{
			return;
		}
		TweenSettingsExtensions.OnComplete<Tweener>(m_SearcherMusic.AudioSource.DOFade(0f, 5f), (TweenCallback)delegate
		{
			if ((Object)(object)m_SearcherMusic != (Object)null)
			{
				m_SearcherMusic.Clear();
				m_SearcherMusic = null;
			}
		});
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH2_save_point_08");
		m_SammyBalcony.SetActive(false);
		m_Door.ForceOpen(-145f);
		m_Door.Lock();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		}
		ClearSearcherMusic();
		m_SearcherMusicClip = null;
		m_SearcherMusicStartClip = null;
		base.OnDisposed();
	}
}
