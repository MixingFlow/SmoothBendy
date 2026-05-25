using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH3PowerCoreTaskController : CH3BaseTaskController
{
	private const int POWER_CORE_MAX = 3;

	private const int POWER_CORE_ACTIVE = 2;

	[Header("<Controllers>")]
	[SerializeField]
	private CH3LiftController m_LiftController;

	[Header("Spawner")]
	[SerializeField]
	private SpecialSpawnerNode m_StrikerSpawner;

	[SerializeField]
	private WaypointList m_RoamingList;

	[Header("Puzzles")]
	[SerializeField]
	private List<CH3InkPressurePuzzle> m_InkPressurePuzzles;

	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_FinalLiftTrigger;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private StrikerAi m_Striker;

	private AudioClip[] m_MissionEndClips;

	private AudioClip[] m_BeginTaskClips;

	private AudioClip m_PowerCorePickupClip;

	private AudioClip m_AliceMissionEndClip;

	private Sprite m_ObjectiveSprite;

	private int m_PowerCoreCount;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ObjectiveSprite = GameManager.Instance.AssetManager.GetAsset<Sprite>("UI/ObjectiveIcons/power_core_icon");
		m_BeginTaskClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskValveStart/");
		m_MissionEndClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskValve/");
		m_PowerCorePickupClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_valvepanelcorepickup");
		m_AliceMissionEndClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_34_valvemissionend");
		m_FinalLiftTrigger.SetActive(active: false);
		SetWeapon(m_WeaponStationController.WeaponStation.m_Plunger);
		m_Striker = GameManager.Instance.AssetManager.CreateAsset<StrikerAi>("GamePlay/Characters/Ai_Striker");
		m_Striker.gameObject.SetActive(false);
		for (int i = 0; i < m_InkPressurePuzzles.Count; i++)
		{
			CH3InkPressurePuzzle cH3InkPressurePuzzle = m_InkPressurePuzzles[i];
			cH3InkPressurePuzzle.ID = i;
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Object[i].ID == i && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Object[i].IsComplete)
			{
				cH3InkPressurePuzzle.ForceComplete();
			}
			else
			{
				cH3InkPressurePuzzle.Activate();
			}
		}
	}

	public override void Activate()
	{
		base.Activate();
		m_BendyController.SetActive(active: false);
		m_SearcherController.SetActive(active: true);
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Status.IsComplete)
		{
			for (int i = 0; i < m_InkPressurePuzzles.Count; i++)
			{
				CH3InkPressurePuzzle cH3InkPressurePuzzle = m_InkPressurePuzzles[i];
				cH3InkPressurePuzzle.ActivateInteraction();
				cH3InkPressurePuzzle.OnComplete += HandleValveOnComplete;
			}
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Status.IsStarted)
		{
			GoToNextController();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Status.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Status.IsStarted)
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
		for (int i = 0; i < m_InkPressurePuzzles.Count; i++)
		{
			CH3InkPressurePuzzle cH3InkPressurePuzzle = m_InkPressurePuzzles[i];
			if (Object.op_Implicit((Object)(object)cH3InkPressurePuzzle))
			{
				cH3InkPressurePuzzle.ForceComplete();
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
		CheckForStriker();
		m_WeaponStationController.Block();
		for (int i = 0; i < m_BeginTaskClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_BeginTaskClips[i], SubtitleConstants.DIA_CH3_ALICE_VALVES_START[i], isTrimmed: true));
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
		CheckForStriker();
		GetWeapon();
	}

	private void ForceComplete()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_VALVE_CORES_COMPLETE_TIP"));
		m_LiftController.GoToFloor(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_LiftController.ForceCloseLift();
		});
		if (Object.op_Implicit((Object)(object)m_Striker))
		{
			m_Striker.Dispose();
		}
		m_BendyController.SetActive(active: true);
		GetWeapon();
		for (int num = 0; num < m_InkPressurePuzzles.Count; num++)
		{
			CH3InkPressurePuzzle cH3InkPressurePuzzle = m_InkPressurePuzzles[num];
			if (Object.op_Implicit((Object)(object)cH3InkPressurePuzzle))
			{
				cH3InkPressurePuzzle.ForceComplete();
			}
		}
		m_FinalLiftTrigger.OnEnter += HandleFinalLiftTriggerOnEnter;
		m_FinalLiftTrigger.SetActive(active: true);
		ActivateDropbox();
	}

	private void UpdateObjective()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_VALVE_CORES_START", "OBJECTIVES/CH3_OBJECTIVE_TASK_VALVE_CORES_TIP");
		for (int i = 0; i < GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Object.Length; i++)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Object[i].IsComplete)
			{
				m_PowerCoreCount++;
			}
		}
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, m_PowerCoreCount);
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

	private void CheckForStriker()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Object[2].IsComplete)
		{
			m_Striker.gameObject.SetActive(true);
			m_Striker.OnDeath += HandleStrikerOnDeath;
			m_Striker.PowerCoreSetActive(active: true);
			m_Striker.transform.position = m_StrikerSpawner.transform.position;
			m_Striker.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			m_Striker.UpdateWaypointList(m_RoamingList.Waypoints);
		}
	}

	protected override void BeginTask()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_VALVE_CORES", "OBJECTIVES/CH3_OBJECTIVE_TASK_VALVE_CORES_TIP", 4f);
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, 0);
		GameManager.Instance.ShowObjective(objectiveDataVO);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Status.IsStarted = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleBeginDialogueOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_VALVE_CORES_START", string.Empty, 4f));
		EnableWeapon();
	}

	private void HandleStrikerOnDeath(object sender, EventArgs e)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		m_Striker.OnDeath -= HandleStrikerOnDeath;
		m_Striker.PowerCoreSetActive(active: false);
		Vector3 position = m_Striker.transform.position;
		Vector3 position2 = position + Vector3.up * 5f;
		Interactable powerCore = GameManager.Instance.AssetManager.CreateAsset<Interactable>("GamePlay/CH3/CH3PowerCore");
		powerCore.transform.position = position2;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(powerCore.transform, position, 0.5f, false), (Ease)30), (TweenCallback)delegate
		{
			GearDropOnComplete(powerCore);
		});
		m_BendyController.SetActive(active: true);
	}

	private void GearDropOnComplete(Interactable powerCore)
	{
		powerCore.SetActive(active: true);
		powerCore.OnInteracted += HandleStrikerPowerCoreOnInteracted;
	}

	private void HandleStrikerPowerCoreOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = (Interactable)sender;
		interactable.OnInteracted -= HandleStrikerPowerCoreOnInteracted;
		GameManager.Instance.AudioManager.Play(m_PowerCorePickupClip);
		interactable.Dispose();
		m_PowerCoreCount++;
		GameManager.Instance.CurrentObjective.ItemCounter++;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Object[2].IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		if (m_PowerCoreCount < 3)
		{
			GameManager.Instance.GameDataManager.Save();
		}
		CheckStatus();
	}

	private void HandleValveOnComplete(object sender, EventArgs e)
	{
		CH3InkPressurePuzzle cH3InkPressurePuzzle = (CH3InkPressurePuzzle)sender;
		cH3InkPressurePuzzle.OnComplete -= HandleValveOnComplete;
		if (m_InkPressurePuzzles.Contains(cH3InkPressurePuzzle))
		{
			m_InkPressurePuzzles.Remove(cH3InkPressurePuzzle);
			m_PowerCoreCount++;
			GameManager.Instance.CurrentObjective.ItemCounter++;
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Object[cH3InkPressurePuzzle.ID].IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		if (m_PowerCoreCount < 3)
		{
			GameManager.Instance.GameDataManager.Save();
		}
		CheckStatus();
	}

	private void CheckStatus()
	{
		if (m_PowerCoreCount == 2)
		{
			m_LiftController.GoToRandomFloor();
		}
		else if (m_PowerCoreCount >= 3)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceMissionEndClip, "DIACH3/DIA_CH3_ALICE_13")).OnComplete += HandleMissionEndClipOnComplete;
			CollectAllPowerCores();
		}
	}

	private void CollectAllPowerCores()
	{
		m_FinalLiftTrigger.SetActive(active: true);
		m_FinalLiftTrigger.OnEnter += HandleFinalLiftTriggerOnEnter;
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.FEELING_THE_PRESSURE);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Status.IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
		ActivateDropbox();
	}

	private void HandleMissionEndClipOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleMissionEndClipOnComplete;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_VALVE_CORES_COMPLETE_TIP", 4f));
	}

	private void HandleFinalLiftTriggerOnEnter(object sender, EventArgs e)
	{
		m_FinalLiftTrigger.OnEnter -= HandleFinalLiftTriggerOnEnter;
		for (int i = 0; i < m_MissionEndClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MissionEndClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_TASK_VALVES_LIFT[i], isTrimmed: true));
		}
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_FinalLiftTrigger))
		{
			m_FinalLiftTrigger.OnEnter -= HandleFinalLiftTriggerOnEnter;
		}
		m_Striker = null;
		m_MissionEndClips = null;
		m_AliceMissionEndClip = null;
		m_PowerCorePickupClip = null;
		m_ObjectiveSprite = null;
		base.OnDisposed();
	}
}
