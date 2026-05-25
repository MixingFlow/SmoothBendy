using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH2MusicDepartmentController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private CH2MusicController m_MusicController;

	[Header("Searcher Spawn Trigger")]
	[SerializeField]
	private EventTrigger m_SearcherSpawnTrigger;

	[SerializeField]
	private InkExplosionEffect m_InkExplosion;

	[Header("Searcher Spawners")]
	[SerializeField]
	private Transform m_SearcherInkBlob;

	[SerializeField]
	private SearcherAi m_InitialSearcher;

	[SerializeField]
	private List<SearcherAi> m_MusicDeptSearchers;

	[Header("Randoms")]
	[SerializeField]
	private GameObject m_RandomSearchers;

	[Header("Doors")]
	[SerializeField]
	private GenericDoorController m_GateDoor;

	[SerializeField]
	private BaseDoorController m_RecordingStudioDoor;

	[SerializeField]
	private BaseDoorController m_PoolRoomDoor;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioObject m_SearchersMusicObject;

	private AudioClip m_SearchersMusicClip;

	private AudioClip m_SearcherScareClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_SearchersMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_The_Searchers");
		m_SearcherScareClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_SearcherStartCue01");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicDepartmentObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_SearcherSpawnTrigger.OnEnter += HandleSearcherEventTriggerOnEnter;
		m_SearcherSpawnTrigger.SetActive(active: true);
	}

	private void HandleSearcherEventTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		m_SearcherSpawnTrigger.OnEnter -= HandleSearcherEventTriggerOnEnter;
		if (!Object.op_Implicit((Object)(object)m_InitialSearcher))
		{
			return;
		}
		m_InitialSearcher.OnActivate += HandleInitialSearcherOnActivate;
		m_InitialSearcher.OnRespawn += HandleInitialSearcherOnDeath;
		Sequence val = DOTween.Sequence();
		S13AudioManager.Instance.InvokeEvent("evt_ch2_dept_ink_blob_fall");
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_SearcherInkBlob, -15f, 1f, false), (Ease)5), (TweenCallback)delegate
		{
			if (Object.op_Implicit((Object)(object)m_InitialSearcher))
			{
				m_InkExplosion.ExplodeOnly();
				m_InitialSearcher.gameObject.SetActive(true);
				S13AudioManager.Instance.InvokeEvent("evt_ch2_dept_ink_blob_land");
			}
		}));
	}

	private void HandleInitialSearcherOnActivate(object sender, EventArgs e)
	{
		m_InitialSearcher.OnActivate -= HandleInitialSearcherOnActivate;
		m_InkExplosion.ExplodeOnly();
		GameManager.Instance.AudioManager.Play(m_SearcherScareClip, AudioObjectType.MUSIC);
		m_SearchersMusicObject = GameManager.Instance.AudioManager.Play(m_SearchersMusicClip, AudioObjectType.MUSIC, -1);
	}

	private void HandleInitialSearcherOnDeath(object sender, EventArgs e)
	{
		m_InitialSearcher.OnDeath -= HandleInitialSearcherOnDeath;
		m_RandomSearchers.SetActive(true);
		for (int i = 0; i < m_MusicDeptSearchers.Count; i++)
		{
			SearcherAi searcherAi = m_MusicDeptSearchers[i];
			searcherAi.OnRespawn += HandleSearcherOnDeath;
			searcherAi.gameObject.SetActive(true);
		}
	}

	private void HandleSearcherOnDeath(object sender, EventArgs e)
	{
		SearcherAi searcherAi = (SearcherAi)sender;
		searcherAi.OnDeath -= HandleSearcherOnDeath;
		if (m_MusicDeptSearchers.Contains(searcherAi))
		{
			m_MusicDeptSearchers.Remove(searcherAi);
		}
		if (m_MusicDeptSearchers.Count <= 0)
		{
			OpenDepartment();
		}
	}

	private void OpenDepartment()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		if ((Object)(object)m_SearchersMusicObject != (Object)null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_SearchersMusicObject.AudioSource.DOFade(0f, 1.5f), new TweenCallback(ClearSearcherMusic));
		}
		m_RecordingStudioDoor.Unlock();
		m_PoolRoomDoor.Unlock();
		m_GateDoor.Open();
		m_MusicController.Activate();
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicDepartmentObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void ClearSearcherMusic()
	{
		if ((Object)(object)m_SearchersMusicObject != (Object)null)
		{
			m_SearchersMusicObject.Clear();
			m_SearchersMusicObject = null;
		}
	}

	private void ForceComplete()
	{
		m_GateDoor.ForceOpen();
		m_RandomSearchers.SetActive(true);
		m_RecordingStudioDoor.Unlock();
		m_PoolRoomDoor.Unlock();
		if (Object.op_Implicit((Object)(object)m_InitialSearcher))
		{
			m_InitialSearcher.Dispose();
		}
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		ClearSearcherMusic();
		m_SearchersMusicObject = null;
		m_SearchersMusicClip = null;
		m_SearcherScareClip = null;
		base.OnDisposed();
	}
}
