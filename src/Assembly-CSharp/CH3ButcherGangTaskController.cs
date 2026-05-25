using System;
using System.Collections;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH3ButcherGangTaskController : CH3BaseTaskController
{
	private class SpawnPoint
	{
		public Transform Spawner;

		public BaseAiController Ai;

		public bool isInUse;
	}

	[Header("<Controllers>")]
	[SerializeField]
	private CH3ServiceController m_ServiceController;

	[SerializeField]
	private CH3LiftController m_LiftController;

	[SerializeField]
	private CH3StairwellController m_StairwellController;

	[SerializeField]
	private CH3AccountingController m_AccountingDoor;

	[Header("Spawners")]
	[SerializeField]
	private List<Transform> m_GangsterSpawners;

	[SerializeField]
	private Transform m_SearcherSpawnerParent;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_EndMissionEventTrigger;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private List<ButcherGangAi> m_Gangsters = new List<ButcherGangAi>();

	private List<BaseAiController> m_Searchers = new List<BaseAiController>();

	private List<SpawnPoint> m_SearcherSpawners = new List<SpawnPoint>();

	private AudioClip[] m_BeginTaskClips;

	private AudioObject m_MusicAudio;

	private AudioObject m_MusicEndAudio;

	private AudioObject m_ButchGangAudio;

	private AudioClip m_MusicSlow;

	private AudioClip m_MusicFast;

	private AudioClip m_MusicEnd;

	private AudioClip m_ButcherGangBreakInClip;

	private AudioClip m_ReturnClip;

	private AudioClip m_AliceMissionEndClip;

	private int m_ButcherGangCount = 3;

	private int m_ButcherGangDeaths;

	private bool m_IsCompleted;

	public override void InitOnComplete()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		base.InitOnComplete();
		m_BeginTaskClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskButcherGangStart/");
		m_MusicSlow = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_theoldgangslow");
		m_MusicFast = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_theoldgangfaster");
		m_MusicEnd = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_theoldgangending");
		m_ButcherGangBreakInClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_theyrebreakingin");
		m_ReturnClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_spawnreturn");
		m_AliceMissionEndClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_68_enemymissionendA");
		foreach (Transform item in m_SearcherSpawnerParent)
		{
			Transform spawner = item;
			SpawnPoint spawnPoint = new SpawnPoint();
			spawnPoint.Spawner = spawner;
			m_SearcherSpawners.Add(spawnPoint);
		}
		m_EndMissionEventTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		base.Activate();
		m_AccountingDoor.Activate();
		m_AccountingDoor.ActivateAxe();
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Status.IsStarted)
		{
			GoToNextController();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ButcherGangTask.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			InternalActivate();
		}
	}

	private void GoToNextController()
	{
		SendOnComplete();
	}

	private void InternalActivate()
	{
		m_StairwellController.CloseAllFloors();
		m_LiftController.DisableLift();
		m_BendyController.SetActive(active: false);
		m_SearcherController.SetActive(active: false);
		m_GangsterController.SetActive(active: false);
		ActivateSearchers();
		for (int i = 0; i < m_BeginTaskClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_BeginTaskClips[i], SubtitleConstants.DIA_CH3_ALICE_TASK_BUTCHER_GANG_START[i], isTrimmed: true));
			if (i == 1)
			{
				audioObject.OnComplete += HandleBeginDialogueMiddleOnComplete;
			}
			else if (i >= m_BeginTaskClips.Length - 1)
			{
				audioObject.OnComplete += HandleBeginDialogueOnComplete;
			}
		}
		m_MusicAudio = GameManager.Instance.AudioManager.Play(m_MusicSlow, AudioObjectType.MUSIC, -1);
		m_MusicAudio.AudioSource.volume = 0f;
		m_MusicAudio.AudioSource.DOFade(1f, 30f);
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned += HandlePlayerOnSpawned;
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ButcherGangTask.IsStarted)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ButcherGangTask.IsStarted = true;
			GameManager.Instance.GameDataManager.Save();
		}
	}

	private void ForceComplete()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", string.Empty));
		m_BendyController.SetActive(active: true);
		m_SearcherController.SetActive(active: true);
		m_GangsterController.SetActive(active: true);
		m_StairwellController.OpenAllFloors();
		m_LiftController.EnableLift();
		m_EndMissionEventTrigger.OnEnter += HandleEndMissionEventTriggerOnEnter;
		m_EndMissionEventTrigger.SetActive(active: true);
	}

	private void HandleEndMissionEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EndMissionEventTrigger.OnEnter -= HandleEndMissionEventTriggerOnEnter;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.InactiveWeapon);
			GameManager.Instance.Player.InactiveWeapon = null;
		}
		SendOnComplete();
	}

	private void HandleBeginDialogueMiddleOnComplete(object sender, EventArgs e)
	{
		m_ButchGangAudio = GameManager.Instance.AudioManager.Play(m_ButcherGangBreakInClip, AudioObjectType.SOUND_EFFECT, -1);
	}

	private void HandleBeginDialogueOnComplete(object sender, EventArgs e)
	{
		ActivateButcherGang();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_KILL_START", "OBJECTIVES/CH3_OBJECTIVE_TASK_KILL_START_TIP", 4f));
	}

	private void ActivateSearchers()
	{
		for (int i = 0; i < m_SearcherSpawners.Count; i++)
		{
			SpawnPoint spawnPoint = m_SearcherSpawners[i];
			spawnPoint.Ai = null;
			spawnPoint.isInUse = false;
		}
		((MonoBehaviour)this).StartCoroutine(SpawnSearchers(7));
	}

	private IEnumerator SpawnSearchers(int count, SpawnPoint ignoreSpawner = null)
	{
		if (base.IsDisposed)
		{
			yield break;
		}
		for (int i = 0; i < count; i++)
		{
			yield return (object)new WaitForSeconds(1f);
			yield return (object)new WaitForEndOfFrame();
			if (base.IsDisposed || m_Searchers == null)
			{
				yield return null;
			}
			BaseAiController baseAiController = GameManager.Instance.AssetManager.CreateAsset<BaseAiController>("GamePlay/Characters/Ai_Searcher");
			m_SearcherSpawners.Shuffle();
			for (int j = 0; j < m_SearcherSpawners.Count; j++)
			{
				SpawnPoint spawnPoint = m_SearcherSpawners[j];
				if (!spawnPoint.isInUse && !spawnPoint.Equals(ignoreSpawner))
				{
					spawnPoint.Ai = baseAiController;
					spawnPoint.isInUse = true;
					baseAiController.transform.position = spawnPoint.Spawner.position;
					baseAiController.OnRespawn += HandleSearcherOnRespawn;
					break;
				}
			}
			m_Searchers.Add(baseAiController);
		}
	}

	private void HandleSearcherOnRespawn(object sender, EventArgs e)
	{
		BaseAiController baseAiController = (BaseAiController)sender;
		baseAiController.OnRespawn -= HandleSearcherOnRespawn;
		if (m_IsCompleted)
		{
			return;
		}
		if (m_Searchers.Contains(baseAiController))
		{
			m_Searchers.Remove(baseAiController);
		}
		m_SearcherSpawners.Shuffle();
		foreach (SpawnPoint searcherSpawner in m_SearcherSpawners)
		{
			if ((Object)(object)searcherSpawner.Ai != (Object)null && ((object)searcherSpawner.Ai).Equals((object)baseAiController))
			{
				searcherSpawner.Ai = null;
				searcherSpawner.isInUse = false;
				((MonoBehaviour)this).StartCoroutine(SpawnSearchers(1, searcherSpawner));
				break;
			}
		}
	}

	private void ActivateButcherGang()
	{
		for (int i = 0; i < m_Gangsters.Count; i++)
		{
			ButcherGangAi butcherGangAi = m_Gangsters[i];
			if (Object.op_Implicit((Object)(object)butcherGangAi))
			{
				butcherGangAi.Dispose();
			}
		}
		m_Gangsters.Clear();
		((MonoBehaviour)this).StartCoroutine(SpawnButcherGang("GamePlay/Characters/Ai_Striker", m_GangsterSpawners[0]));
		((MonoBehaviour)this).StartCoroutine(SpawnButcherGang("GamePlay/Characters/Ai_Piper", m_GangsterSpawners[1], 4f));
		((MonoBehaviour)this).StartCoroutine(SpawnButcherGang("GamePlay/Characters/Ai_Fisher", m_GangsterSpawners[2], 8f));
	}

	private IEnumerator SpawnButcherGang(string prefab, Transform spawnPosition, float delay = 0f)
	{
		if (base.IsDisposed)
		{
			yield return null;
		}
		yield return (object)new WaitForSeconds(0.1f + delay);
		yield return (object)new WaitForEndOfFrame();
		if (base.IsDisposed || m_Gangsters == null)
		{
			yield return null;
		}
		ButcherGangAi butcherGangAi = GameManager.Instance.AssetManager.CreateAsset<ButcherGangAi>(prefab);
		butcherGangAi.OnDeath += HandleButcherGangOnDeath;
		butcherGangAi.transform.position = spawnPosition.position;
		butcherGangAi.transform.eulerAngles = new Vector3(0f, 90f, 0f);
		butcherGangAi.SetTarget(GameManager.Instance.Player.transform);
		butcherGangAi.SetThought(AiThought.Follow);
		m_Gangsters.Add(butcherGangAi);
	}

	private void HandleButcherGangOnDeath(object sender, EventArgs e)
	{
		ButcherGangAi butcherGangAi = (ButcherGangAi)sender;
		butcherGangAi.OnDeath -= HandleButcherGangOnDeath;
		m_ButcherGangDeaths++;
		if (m_Gangsters.Contains(butcherGangAi))
		{
			m_Gangsters.Remove(butcherGangAi);
		}
		if (m_ButcherGangDeaths >= m_ButcherGangCount)
		{
			StartComplete();
		}
		else if (m_ButcherGangDeaths == 2)
		{
			QuickenMusic();
		}
	}

	private void QuickenMusic()
	{
		float num = m_MusicAudio.AudioSource.time - m_MusicSlow.length / m_MusicFast.length;
		m_MusicAudio.Clear();
		m_MusicAudio = GameManager.Instance.AudioManager.Play(m_MusicFast, AudioObjectType.MUSIC, -1);
		if (num <= 0f || num > m_MusicAudio.AudioSource.time)
		{
			num = 0f;
		}
		m_MusicAudio.AudioSource.time = num;
	}

	private void StartComplete()
	{
		GameManager.Instance.Player.OnDeath -= HandlePlayerOnSpawned;
		m_IsCompleted = true;
		for (int i = 0; i < m_Searchers.Count; i++)
		{
			m_Searchers[i].SetThought(AiThought.Die);
		}
		ClearEnemies();
		m_ButchGangAudio.Clear();
		m_MusicAudio.Clear();
		m_MusicEndAudio = GameManager.Instance.AudioManager.Play(m_MusicEnd, AudioObjectType.MUSIC);
		m_MusicEndAudio.OnComplete += HandleMusicEndOnComplete;
	}

	private void HandleMusicEndOnComplete(object sender, EventArgs e)
	{
		m_MusicEndAudio.OnComplete -= HandleMusicEndOnComplete;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceMissionEndClip, "DIACH3/DIA_CH3_ALICE_29")).OnComplete += HandleAliceEndDialogueOnComplete;
	}

	private void HandleAliceEndDialogueOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", string.Empty, 4f));
		m_BendyController.SetActive(active: true);
		m_SearcherController.SetActive(active: true);
		m_GangsterController.SetActive(active: true);
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.FRONT_LINES);
		m_StairwellController.UnlockAllFloors();
		m_LiftController.EnableLift();
		m_ServiceController.Activate();
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ButcherGangTask.IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
		m_EndMissionEventTrigger.OnEnter += HandleEndMissionEventTriggerOnEnter;
		m_EndMissionEventTrigger.SetActive(active: true);
	}

	private void HandlePlayerOnSpawned(object sender, EventArgs e)
	{
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		m_ButcherGangDeaths = 0;
		m_BendyController.SetActive(active: true);
		((MonoBehaviour)this).StartCoroutine(RestartTask());
	}

	private IEnumerator RestartTask()
	{
		KillAllEnemies();
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
		}
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.InactiveWeapon);
		}
		MeleeWeapon meleeWeapon = GameManager.Instance.AssetManager.CreateAsset<MeleeWeapon>("GamePlay/Weapons/Weapon_Axe");
		MeleeWeapon meleeWeapon2 = GameManager.Instance.AssetManager.CreateAsset<MeleeWeapon>("GamePlay/Weapons/Weapon_Gent");
		m_AccountingDoor.Reset();
		m_AccountingDoor.DisableAxe();
		meleeWeapon2.KillInteraction();
		meleeWeapon2.Equip();
		GameManager.Instance.Player.InactiveWeapon = meleeWeapon2.gameObject;
		meleeWeapon2.SetParentAndAlign(GameManager.Instance.Player.WeaponParent);
		meleeWeapon2.gameObject.SetActive(false);
		meleeWeapon.KillInteraction();
		meleeWeapon.Equip();
		GameManager.Instance.Player.WeaponGameObject = meleeWeapon.gameObject;
		GameManager.Instance.Player.EquipWeapon();
		meleeWeapon.SetParentAndAlign(GameManager.Instance.Player.WeaponParent);
		ClearAudio();
		yield return (object)new WaitForSeconds(1.5f);
		yield return (object)new WaitForEndOfFrame();
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_ReturnClip, "DIACH3/DIA_CH3_ALICE_SPAWN"));
		m_WeaponStationController.OnTaskComplete += HandleDropboxOnInteracted;
		m_WeaponStationController.ActivateDropbox();
	}

	private void HandleDropboxOnInteracted(object sender, EventArgs e)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		m_WeaponStationController.OnTaskComplete -= HandleDropboxOnInteracted;
		PlayerController player = GameManager.Instance.Player;
		if ((Object)(object)player.InactiveWeapon != (Object)null)
		{
			Object.Destroy((Object)(object)player.WeaponGameObject);
			player.WeaponGameObject = player.InactiveWeapon;
			player.WeaponGameObject.SetActive(true);
			player.WeaponGameObject.transform.localPosition = Vector3.zero;
			player.WeaponGameObject.transform.localEulerAngles = Vector3.zero;
			player.InactiveWeapon = null;
		}
		Activate();
	}

	public void KillAllEnemies()
	{
		for (int i = 0; i < m_Gangsters.Count; i++)
		{
			m_Gangsters[i].Dispose();
		}
		m_Gangsters.Clear();
		for (int j = 0; j < m_Searchers.Count; j++)
		{
			m_Searchers[j].Dispose();
		}
		m_Searchers.Clear();
	}

	private void ClearEnemies()
	{
		if (m_Gangsters != null)
		{
			m_Gangsters.Clear();
			m_Gangsters = null;
		}
		if (m_Searchers != null)
		{
			m_Searchers.Clear();
			m_Searchers = null;
		}
	}

	private void ClearAudio()
	{
		if ((Object)(object)m_MusicAudio != (Object)null)
		{
			m_MusicAudio.Clear();
			m_MusicAudio = null;
		}
		if ((Object)(object)m_MusicEndAudio != (Object)null)
		{
			m_MusicEndAudio.Clear();
			m_MusicEndAudio = null;
		}
		if ((Object)(object)m_ButchGangAudio != (Object)null)
		{
			m_ButchGangAudio.Clear();
			m_ButchGangAudio = null;
		}
	}

	protected override void OnDisposed()
	{
		ClearEnemies();
		ClearAudio();
		if (Object.op_Implicit((Object)(object)m_EndMissionEventTrigger))
		{
			m_EndMissionEventTrigger.OnEnter += HandleEndMissionEventTriggerOnEnter;
		}
		m_BeginTaskClips = null;
		m_MusicSlow = null;
		m_MusicFast = null;
		m_MusicEnd = null;
		m_ButcherGangBreakInClip = null;
		m_ReturnClip = null;
		m_AliceMissionEndClip = null;
		base.OnDisposed();
	}
}
