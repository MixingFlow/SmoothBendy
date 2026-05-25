using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH3ThickInkTaskController : CH3BaseTaskController
{
	private class SpawnPoints
	{
		public Transform SpawnPoint;

		public SwollenSearcherAi ActiveAi;

		public bool isUsed;
	}

	private const int THICK_INK_MAX = 3;

	private const int THICK_INK_ACTIVE = 2;

	[Header("<Controllers>")]
	[SerializeField]
	private CH3LiftController m_LiftController;

	[Header("Spawner")]
	[SerializeField]
	private SpecialSpawnerNode m_FisherSpawner;

	[SerializeField]
	private WaypointList m_RoamingList;

	[Header("Swollen Searchers")]
	[SerializeField]
	private List<Transform> m_SpawnPositions;

	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_FinalLiftTrigger;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private FisherAi m_Fisher;

	private SwollenSearcherAi m_SwollenSearcherPrefab;

	private AudioClip[] m_MissionEndClips;

	private AudioClip[] m_BeginTaskClips;

	private AudioClip[] m_MissionCompleteClips;

	private AudioClip m_ThickInkPickup;

	private List<SpawnPoints> m_SpawnPoints = new List<SpawnPoints>();

	private List<SwollenSearcherAi> m_SwollenSearchers = new List<SwollenSearcherAi>();

	private Sprite m_ObjectiveSprite;

	private int m_ThickInkCount;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ObjectiveSprite = GameManager.Instance.AssetManager.GetAsset<Sprite>("UI/ObjectiveIcons/thick_ink_icon");
		m_BeginTaskClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskThickInkStart/");
		m_MissionEndClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskThickInk/");
		m_ThickInkPickup = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_thickinkpickup");
		m_MissionCompleteClips = (AudioClip[])(object)new AudioClip[2]
		{
			GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_30_searchermissionendA"),
			GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_31_searchermissionendB")
		};
		m_FinalLiftTrigger.SetActive(active: false);
		SetWeapon(m_WeaponStationController.WeaponStation.m_InkTool);
		m_Fisher = GameManager.Instance.AssetManager.CreateAsset<FisherAi>("GamePlay/Characters/Ai_Fisher");
		m_Fisher.gameObject.SetActive(false);
		m_SwollenSearcherPrefab = GameManager.Instance.AssetManager.CreateAsset<SwollenSearcherAi>("GamePlay/Characters/Ai_SwollenSearcher");
		m_SwollenSearcherPrefab.gameObject.SetActive(false);
	}

	public override void Activate()
	{
		base.Activate();
		m_BendyController.SetActive(active: false);
		m_SearcherController.SetActive(active: true);
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Status.IsComplete)
		{
			for (int i = 0; i < m_SpawnPositions.Count; i++)
			{
				SpawnPoints spawnPoints = new SpawnPoints();
				spawnPoints.SpawnPoint = m_SpawnPositions[i];
				m_SpawnPoints.Add(spawnPoints);
			}
			for (int j = 0; j < 2; j++)
			{
				SpawnSwollenSearcher();
			}
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Status.IsStarted)
		{
			GoToNextController();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Status.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Status.IsStarted)
		{
			ForceStart();
		}
		else
		{
			InternalActivate();
		}
	}

	private void GoToNextController()
	{
		if (m_SwollenSearchers != null)
		{
			for (int i = 0; i < m_SwollenSearchers.Count; i++)
			{
				m_SwollenSearchers[i].Dispose();
			}
		}
		if (Object.op_Implicit((Object)(object)m_Weapon))
		{
			m_Weapon.Dispose();
		}
		SendOnComplete();
	}

	private void InternalActivate()
	{
		CheckForPiper();
		m_WeaponStationController.Block();
		for (int i = 0; i < m_BeginTaskClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_BeginTaskClips[i], SubtitleConstants.DIA_CH3_ALICE_THICK_INK_START[i], isTrimmed: true));
			if (i >= m_BeginTaskClips.Length - 1)
			{
				audioObject.OnComplete += HandleBeginDialogueOnComplete;
			}
		}
		m_Weapon.Interaction.SetActive(active: false);
	}

	private void ForceStart()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		m_LiftController.GoToFloor(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_LiftController.ForceCloseLift();
		});
		UpdateObjective();
		CheckForPiper();
		GetWeapon();
	}

	private void ForceComplete()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_THICK_INK_COMPLETE_TIP"));
		m_LiftController.GoToFloor(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_LiftController.ForceCloseLift();
		});
		if (Object.op_Implicit((Object)(object)m_Fisher))
		{
			m_Fisher.Dispose();
		}
		m_BendyController.SetActive(active: true);
		GetWeapon();
		if (m_SwollenSearchers != null)
		{
			for (int num = 0; num < m_SwollenSearchers.Count; num++)
			{
				m_SwollenSearchers[num].Dispose();
			}
		}
		m_FinalLiftTrigger.OnEnter += HandleFinalLiftTriggerOnEnter;
		m_FinalLiftTrigger.SetActive(active: true);
		ActivateDropbox();
	}

	private void UpdateObjective()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_THICK_INK_START", "OBJECTIVES/CH3_OBJECTIVE_TASK_THICK_INK_TIP");
		for (int i = 0; i < GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Object.Length; i++)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Object[i].IsComplete)
			{
				m_ThickInkCount++;
			}
		}
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, m_ThickInkCount);
		GameManager.Instance.UpdateObjective(objectiveDataVO);
	}

	private void GetWeapon()
	{
		if (Object.op_Implicit((Object)(object)m_Weapon))
		{
			PlayerController player = GameManager.Instance.Player;
			player.InactiveWeapon = player.WeaponGameObject;
			player.InactiveWeapon.gameObject.SetActive(false);
			player.WeaponGameObject = m_Weapon.gameObject;
			m_Weapon.SetParentAndAlign(player.WeaponParent);
			m_Weapon.Equip();
			m_Weapon.Interaction.SetActive(active: false);
			m_Weapon.Interaction.Dispose();
		}
	}

	private void CheckForPiper()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Object[2].IsComplete)
		{
			m_Fisher.gameObject.SetActive(true);
			m_Fisher.AnimationController.animatePhysics = false;
			m_Fisher.OnDeath += HandleFisherOnDeath;
			m_Fisher.ThickInkSetActive(active: true);
			m_Fisher.transform.position = m_FisherSpawner.transform.position;
			m_Fisher.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			m_Fisher.UpdateWaypointList(m_RoamingList.Waypoints);
		}
	}

	protected override void BeginTask()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_THICK_INK", "OBJECTIVES/CH3_OBJECTIVE_TASK_THICK_INK_TIP", 4f);
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, 0);
		GameManager.Instance.ShowObjective(objectiveDataVO);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Status.IsStarted = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleBeginDialogueOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_THICK_INK_START", string.Empty, 4f));
		EnableWeapon();
	}

	private void HandleFisherOnDeath(object sender, EventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		m_Fisher.OnDeath -= HandleFisherOnDeath;
		Vector3 position = m_Fisher.transform.position;
		Interactable interactable = Object.Instantiate<Interactable>(GameManager.Instance.AssetManager.GetAsset<Interactable>("GamePlay/CH3/CH3ThickInk"));
		interactable.OnInteracted += HandleFisherThickInkOnInteracted;
		interactable.transform.position = position;
		interactable.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
		m_BendyController.SetActive(active: true);
	}

	private void SpawnSwollenSearcher()
	{
		SwollenSearcherAi swollenSearcherAi = Object.Instantiate<SwollenSearcherAi>(m_SwollenSearcherPrefab);
		swollenSearcherAi.gameObject.SetActive(true);
		swollenSearcherAi.OnDeath += HandleSwollenSearcherOnDeath;
		swollenSearcherAi.OnHide += HandleSwollenSearcherOnHide;
		m_SwollenSearchers.Add(swollenSearcherAi);
		GetNewLocation(swollenSearcherAi);
	}

	private void GetNewLocation(SwollenSearcherAi swollenSearcher)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		SpawnPoints obj = null;
		m_SpawnPoints.Shuffle();
		for (int i = 0; i < m_SpawnPoints.Count; i++)
		{
			SpawnPoints spawnPoints = m_SpawnPoints[i];
			if (!spawnPoints.isUsed)
			{
				obj = spawnPoints;
				spawnPoints.isUsed = true;
				spawnPoints.ActiveAi = swollenSearcher;
				swollenSearcher.transform.position = spawnPoints.SpawnPoint.position;
				swollenSearcher.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
				swollenSearcher.Rise(Random.Range(7f, 18f));
			}
		}
		for (int j = 0; j < m_SpawnPoints.Count; j++)
		{
			SpawnPoints spawnPoints2 = m_SpawnPoints[j];
			if (!spawnPoints2.Equals(obj) && ((object)spawnPoints2.ActiveAi).Equals((object)swollenSearcher))
			{
				spawnPoints2.ActiveAi = null;
				spawnPoints2.isUsed = false;
			}
		}
	}

	private void HandleSwollenSearcherOnHide(object sender, EventArgs e)
	{
		SwollenSearcherAi swollenSearcher = (SwollenSearcherAi)sender;
		GetNewLocation(swollenSearcher);
	}

	private void HandleSwollenSearcherOnDeath(object sender, EventArgs e)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		SwollenSearcherAi swollenSearcherAi = (SwollenSearcherAi)sender;
		swollenSearcherAi.OnDeath -= HandleSwollenSearcherOnDeath;
		m_SwollenSearchers.Remove(swollenSearcherAi);
		Vector3 position = swollenSearcherAi.transform.position;
		Interactable interactable = Object.Instantiate<Interactable>(GameManager.Instance.AssetManager.GetAsset<Interactable>("GamePlay/CH3/CH3ThickInk"));
		interactable.OnInteracted += HandleThickInkOnInteracted;
		interactable.transform.position = position;
		interactable.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
	}

	private void HandleThickInkOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = (Interactable)sender;
		interactable.OnInteracted -= HandleThickInkOnInteracted;
		interactable.Dispose();
		GameManager.Instance.AudioManager.Play(m_ThickInkPickup);
		int num = -1;
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Object[0].IsComplete)
		{
			num = 0;
		}
		else if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Object[1].IsComplete)
		{
			num = 1;
		}
		m_ThickInkCount++;
		GameManager.Instance.CurrentObjective.ItemCounter++;
		if (num > -1)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Object[num].IsComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
			if (m_ThickInkCount < 3)
			{
				GameManager.Instance.GameDataManager.Save();
			}
		}
		CheckStatus();
	}

	private void HandleFisherThickInkOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = (Interactable)sender;
		interactable.OnInteracted -= HandleThickInkOnInteracted;
		interactable.Dispose();
		GameManager.Instance.AudioManager.Play(m_ThickInkPickup);
		m_ThickInkCount++;
		GameManager.Instance.CurrentObjective.ItemCounter++;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Object[2].IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		if (m_ThickInkCount < 3)
		{
			GameManager.Instance.GameDataManager.Save();
		}
		CheckStatus();
	}

	private void CheckStatus()
	{
		if (m_ThickInkCount == 2)
		{
			m_LiftController.GoToRandomFloor();
		}
		else
		{
			if (m_ThickInkCount < 3)
			{
				return;
			}
			for (int i = 0; i < m_MissionCompleteClips.Length; i++)
			{
				AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MissionCompleteClips[i], SubtitleConstants.DIA_CH3_ALICE_THICK_INK_COMPLETE[i], isTrimmed: true));
				if (i >= m_MissionCompleteClips.Length - 1)
				{
					audioObject.OnComplete += HandleMissionCompleteDialogueOnComplete;
				}
			}
			for (int j = 0; j < m_SwollenSearchers.Count; j++)
			{
				m_SwollenSearchers[j].FinalHide();
			}
			m_SwollenSearchers.Clear();
			CollectAllThickInk();
		}
	}

	private void CollectAllThickInk()
	{
		m_FinalLiftTrigger.SetActive(active: true);
		m_FinalLiftTrigger.OnEnter += HandleFinalLiftTriggerOnEnter;
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.ULTIMATE_STOMACHACHE);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Status.IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
		ActivateDropbox();
	}

	private void HandleMissionCompleteDialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleMissionCompleteDialogueOnComplete;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_THICK_INK_COMPLETE_TIP", 4f));
	}

	private void HandleFinalLiftTriggerOnEnter(object sender, EventArgs e)
	{
		m_FinalLiftTrigger.OnEnter -= HandleFinalLiftTriggerOnEnter;
		for (int i = 0; i < m_MissionEndClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MissionEndClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_TASK_THICK_INK_LIFT[i], isTrimmed: true));
		}
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_FinalLiftTrigger != (Object)null)
		{
			m_FinalLiftTrigger.Dispose();
		}
		if (m_SwollenSearchers != null)
		{
			m_SwollenSearchers.Clear();
			m_SwollenSearchers = null;
		}
		m_ThickInkPickup = null;
		m_Fisher = null;
		m_SwollenSearcherPrefab = null;
		m_MissionEndClips = null;
		m_BeginTaskClips = null;
		m_MissionCompleteClips = null;
		m_ObjectiveSprite = null;
		base.OnDisposed();
	}
}
