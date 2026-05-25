using System;
using System.Collections;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH3BendyController : BaseController
{
	private const float DEFAULT_TIMER_MIN = 180f;

	private const float DEFAULT_TIMER_MAX = 240f;

	private const float BENDY_GRACE_PERIOD = 5f;

	[Header("Bendy")]
	[SerializeField]
	private BendyAi m_BendyPrefab;

	[Header("Cutout Waypoints")]
	[SerializeField]
	private List<WaypointNode> m_CutoutWaypoints;

	[Header("Spawners")]
	[SerializeField]
	private List<BendySpawnerList> m_BendySpawnerList;

	[Header("Events")]
	[SerializeField]
	private EventTrigger m_BendyDespawner;

	private BendySpawnerList m_ActiveList;

	private AudioObject m_BendyMusic;

	private AudioClip m_BendyMusicClip;

	private bool m_PlayChaseMusic;

	private bool m_CanSpawn;

	private float m_TimerMin = 120f;

	private float m_TimerMax = 240f;

	private float m_Timer;

	private float m_TimerLimit = 300f;

	private bool m_IsPlayingMusic;

	private float m_TimerMinAdjusted;

	private float m_TimerMaxAdjusted;

	private BorisAi m_Boris => GameManager.Instance.CharacterManager.Boris;

	public BendyAi Bendy { get; private set; }

	public bool IsActive { get; private set; }

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_BendyMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/MUS_Little_Devil_Darling_Remastered");
		GameManager.Instance.CurrentChapter.DeathController.OnDeath += HandlePlayerOnDeath;
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		KillBendy();
	}

	private void Update()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		m_TimerMinAdjusted = m_TimerMin / GameManager.Instance.PlayerSettings.BendyAggressionScale;
		m_TimerMaxAdjusted = m_TimerMax / GameManager.Instance.PlayerSettings.BendyAggressionScale;
		if (m_TimerLimit > m_TimerMax)
		{
			m_TimerLimit = Random.Range(m_TimerMinAdjusted, m_TimerMaxAdjusted);
		}
		if (GameManager.Instance.isPaused)
		{
			return;
		}
		if (m_PlayChaseMusic && GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding)
		{
			m_PlayChaseMusic = false;
		}
		if (!m_PlayChaseMusic && m_IsPlayingMusic && (Object)(object)m_BendyMusic != (Object)null)
		{
			m_IsPlayingMusic = false;
			ShortcutExtensions.DOKill((Component)(object)m_BendyMusic.AudioSource, false);
			TweenSettingsExtensions.OnComplete<Tweener>(m_BendyMusic.AudioSource.DOFade(0f, 2f), (TweenCallback)delegate
			{
				if (!m_PlayChaseMusic)
				{
					m_BendyMusic.Clear();
					m_BendyMusic = null;
				}
			});
		}
		if (!IsActive || !m_CanSpawn)
		{
			return;
		}
		if (!Object.op_Implicit((Object)(object)Bendy))
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_TimerLimit)
			{
				m_Timer = 0f;
				m_TimerLimit = Random.Range(m_TimerMinAdjusted, m_TimerMaxAdjusted);
				m_CanSpawn = false;
				SpawnBendy();
			}
		}
		else
		{
			Vector3 val = Bendy.transform.position - GameManager.Instance.Player.transform.position;
			if (((Vector3)(ref val)).magnitude > 250f || val.y > 250f)
			{
				Bendy.ForceKill();
			}
		}
	}

	public void SetSpawnTimer(float min, float max)
	{
		m_TimerMin = min;
		m_TimerMax = max;
	}

	public void ResetSpawnTimer()
	{
		m_TimerMin = 180f;
		m_TimerMax = 240f;
	}

	public void SetActive(bool active)
	{
		IsActive = active;
		if (IsActive)
		{
			((MonoBehaviour)this).StopAllCoroutines();
			KillBendy();
			ClearAllSpawners();
			m_CanSpawn = true;
			DebugLog("[BENDY] - Bendy will spawn in " + m_TimerLimit + " seconds.");
		}
		else
		{
			((MonoBehaviour)this).StopAllCoroutines();
			KillBendy();
		}
	}

	public void ForceSpawn()
	{
		m_Timer = 0f;
		m_TimerLimit = Random.Range(m_TimerMinAdjusted, m_TimerMaxAdjusted);
		m_CanSpawn = false;
		((MonoBehaviour)this).StopAllCoroutines();
		KillBendy();
		ClearAllSpawners();
		SpawnBendy();
	}

	private void SpawnBendy()
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		if (m_BendySpawnerList == null || m_BendySpawnerList.Count <= 0)
		{
			return;
		}
		m_ActiveList = GetClosestSpawner();
		if ((Object)(object)m_ActiveList == (Object)null)
		{
			DebugLog("No Spawner Found - Not Spawning Bendy...");
			return;
		}
		BendySpawner bendySpawner = null;
		BendySpawner bendySpawner2 = null;
		int num = Random.Range(0, m_ActiveList.BendySpawners.Count);
		for (int i = 0; i < m_ActiveList.BendySpawners.Count; i++)
		{
			if (i == num)
			{
				bendySpawner = m_ActiveList.BendySpawners[i];
			}
			else
			{
				bendySpawner2 = m_ActiveList.BendySpawners[i];
			}
		}
		m_Boris.SetCower(active: true);
		Bendy = Object.Instantiate<BendyAi>(m_BendyPrefab);
		DebugLog("[BENDY] - Spawned at (" + ((Object)m_ActiveList.gameObject).name + ")");
		m_ActiveList.Use();
		Bendy.transform.position = bendySpawner.Watpoints[0].transform.position;
		Bendy.transform.eulerAngles = bendySpawner.Watpoints[0].transform.eulerAngles;
		Bendy.OnWaypointComplete += HandleBendyOnWaypointComplete;
		Bendy.OnSpotted += HandleBendyOnSpotted;
		Bendy.OnTrackingLost += HandleBendyOnTrackingLost;
		Bendy.UpdateWaypointList(bendySpawner2.Watpoints);
		Bendy.SetPassive(IsPassive: true);
		((MonoBehaviour)this).StartCoroutine(SetBendyNotPassive());
		m_BendyDespawner.OnEnter -= HandleBendyDespawnerOnEnter;
		m_BendyDespawner.OnEnter += HandleBendyDespawnerOnEnter;
	}

	private void HandleBendyOnTrackingLost(object sender, EventArgs e)
	{
		StopChaseMusic();
	}

	private IEnumerator SetBendyNotPassive()
	{
		yield return (object)new WaitForSeconds(5f);
		yield return (object)new WaitForEndOfFrame();
		if (!base.IsDisposed && Object.op_Implicit((Object)(object)Bendy))
		{
			Bendy.SetPassive(IsPassive: false);
		}
	}

	private void HandleBendyDespawnerOnEnter(object sender, EventArgs e)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		m_BendyDespawner.OnEnter -= HandleBendyDespawnerOnEnter;
		DebugLog("[BENDY] - Bendy has been despawned.");
		m_CanSpawn = true;
		m_ActiveList.Reset();
		if (Object.op_Implicit((Object)(object)Bendy))
		{
			m_Boris.SetCower(active: false);
			Bendy.ForceKill();
			Bendy = null;
		}
		if (!((Object)(object)m_BendyMusic != (Object)null))
		{
			return;
		}
		m_IsPlayingMusic = false;
		ShortcutExtensions.DOKill((Component)(object)m_BendyMusic.AudioSource, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_BendyMusic.AudioSource.DOFade(0f, 2f), (TweenCallback)delegate
		{
			if (Object.op_Implicit((Object)(object)m_BendyMusic))
			{
				m_BendyMusic.Clear();
				m_BendyMusic = null;
			}
		});
	}

	private void HandleBendyOnSpotted(object sender, EventArgs e)
	{
		m_PlayChaseMusic = true;
		if ((Object)(object)m_BendyMusic != (Object)null)
		{
			m_BendyMusic.Clear();
			m_BendyMusic = null;
		}
		m_IsPlayingMusic = true;
		m_BendyMusic = GameManager.Instance.AudioManager.Play(m_BendyMusicClip, AudioObjectType.MUSIC, -1);
	}

	public void StopChaseMusic()
	{
		m_PlayChaseMusic = false;
	}

	private void HandleBendyOnWaypointComplete(object sender, EventArgs e)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		m_CanSpawn = true;
		if (Object.op_Implicit((Object)(object)Bendy))
		{
			Bendy.OnWaypointComplete -= HandleBendyOnWaypointComplete;
			Bendy.OnSpotted -= HandleBendyOnSpotted;
		}
		if ((Object)(object)m_BendyMusic != (Object)null)
		{
			m_IsPlayingMusic = false;
			ShortcutExtensions.DOKill((Component)(object)m_BendyMusic.AudioSource, false);
			TweenSettingsExtensions.OnComplete<Tweener>(m_BendyMusic.AudioSource.DOFade(0f, 2f), (TweenCallback)delegate
			{
				m_BendyMusic.Clear();
				m_BendyMusic = null;
			});
		}
		m_ActiveList.Reset();
		KillBendy();
	}

	public void GoToCutout()
	{
		if (Object.op_Implicit((Object)(object)Bendy))
		{
			List<WaypointNode> list = new List<WaypointNode>();
			list.Add(GetClosestCutout());
			if (list.Count > 0)
			{
				Bendy.OnWaypointComplete -= HandleBendyOnWaypointComplete;
				Bendy.OnWaypointComplete -= HandleBendyCutoutWaypointComplete;
				Bendy.OnWaypointComplete += HandleBendyCutoutWaypointComplete;
				Bendy.UpdateWaypointList(list, _isActive: true);
				m_BendyDespawner.OnEnter -= HandleBendyDespawnerOnEnter;
				m_BendyDespawner.OnEnter += HandleBendyDespawnerOnEnter;
			}
		}
		else
		{
			SpawnBendy();
		}
	}

	private void HandleBendyCutoutWaypointComplete(object sender, EventArgs e)
	{
		Bendy.OnWaypointComplete -= HandleBendyCutoutWaypointComplete;
		m_ActiveList = GetClosestSpawner();
		BendySpawner bendySpawner = null;
		int num = Random.Range(0, m_ActiveList.BendySpawners.Count);
		for (int i = 0; i < m_ActiveList.BendySpawners.Count; i++)
		{
			if (i == num)
			{
				bendySpawner = m_ActiveList.BendySpawners[i];
			}
		}
		m_ActiveList.Use();
		Bendy.OnWaypointComplete += HandleBendyOnWaypointComplete;
		Bendy.UpdateWaypointList(bendySpawner.Watpoints);
	}

	private BendySpawnerList GetClosestSpawner()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		BendySpawnerList result = null;
		float num = float.PositiveInfinity;
		Vector3 position = GameManager.Instance.Player.transform.position;
		foreach (BendySpawnerList bendySpawner in m_BendySpawnerList)
		{
			foreach (BendySpawner bendySpawner2 in bendySpawner.BendySpawners)
			{
				Vector3 val = bendySpawner2.transform.position - position;
				float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = bendySpawner2.BendySpawnerList;
				}
			}
		}
		return result;
	}

	private WaypointNode GetClosestCutout()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		WaypointNode result = null;
		float num = float.PositiveInfinity;
		Vector3 position = GameManager.Instance.Player.transform.position;
		foreach (WaypointNode cutoutWaypoint in m_CutoutWaypoints)
		{
			Vector3 val = cutoutWaypoint.transform.position - position;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = cutoutWaypoint;
			}
		}
		return result;
	}

	private void KillBendy()
	{
		m_Boris.SetCower(active: false);
		if (Object.op_Implicit((Object)(object)Bendy))
		{
			Bendy.Dispose();
			Bendy = null;
		}
		if (Object.op_Implicit((Object)(object)m_BendyMusic))
		{
			ShortcutExtensions.DOKill((Component)(object)m_BendyMusic.AudioSource, false);
			m_BendyMusic.Clear();
			m_BendyMusic = null;
		}
	}

	private void ClearAllSpawners()
	{
		for (int i = 0; i < m_BendySpawnerList.Count; i++)
		{
			m_BendySpawnerList[i].Reset();
		}
	}

	protected override void OnDisposed()
	{
		m_ActiveList = null;
		m_BendyMusic = null;
		m_BendyMusicClip = null;
		base.OnDisposed();
	}
}
