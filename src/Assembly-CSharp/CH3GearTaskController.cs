using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH3GearTaskController : CH3BaseTaskController
{
	private const int GEAR_MAX = 3;

	private const int GEAR_ACTIVE = 2;

	[Header("<Controllers>")]
	[SerializeField]
	private CH3LiftController m_LiftController;

	[SerializeField]
	private CH3ProjectionistScareController m_ProjectionistScareController;

	[Header("Spawner")]
	[SerializeField]
	private SpecialSpawnerNode m_PiperSpawner;

	[SerializeField]
	private WaypointList m_RoamingList;

	[Header("Puzzles")]
	[SerializeField]
	private List<CH3GearPanel> m_GearPanels;

	[SerializeField]
	private List<CH3GearPanel> m_EmptyGearPanels;

	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_FinalLiftTrigger;

	[Header("Alice Angel Dialogue")]
	[SerializeField]
	private EventTrigger m_InkDemonWarningTrigger;

	[SerializeField]
	private EventTrigger m_BendyDontRunTrigger;

	private PiperAi m_Piper;

	private AudioClip[] m_MissionEndClips;

	private AudioClip[] m_BeginTaskClips;

	private AudioClip[] m_DontRunClips;

	private AudioClip m_TakeGearClip;

	private AudioClip m_AliceMissionIntroClip;

	private AudioClip m_HenryTrueClip;

	private AudioClip m_HenryFalseClip_01;

	private AudioClip m_HenryFalseClip_02;

	private AudioClip[] m_MissionCompleteClips;

	private int m_FalseCount;

	private Sprite m_ObjectiveSprite;

	private int m_GearCount;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ObjectiveSprite = GameManager.Instance.AssetManager.GetAsset<Sprite>("UI/ObjectiveIcons/gear_icon");
		m_BeginTaskClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TasksBegin/");
		m_MissionCompleteClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskGearEnd/");
		m_MissionEndClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskGear/");
		m_TakeGearClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_gearmission_takegear");
		m_AliceMissionIntroClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_22_gearmissionintro");
		m_HenryTrueClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_27_valvepuzzle_trueA");
		m_HenryFalseClip_01 = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_25_valvepuzzle_falseA");
		m_HenryFalseClip_02 = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_26_valvepuzzle_falseB");
		m_FinalLiftTrigger.SetActive(active: false);
		m_Piper = GameManager.Instance.AssetManager.CreateAsset<PiperAi>("GamePlay/Characters/Ai_Piper");
		m_Piper.gameObject.SetActive(false);
		SetWeapon(m_WeaponStationController.WeaponStation.m_Wrench);
		for (int i = 0; i < m_GearPanels.Count; i++)
		{
			CH3GearPanel cH3GearPanel = m_GearPanels[i];
			cH3GearPanel.ID = i;
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Object[i].ID == i && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Object[i].IsComplete)
			{
				cH3GearPanel.ForceComplete();
			}
			else
			{
				cH3GearPanel.Activate();
			}
		}
		for (int j = 0; j < m_EmptyGearPanels.Count; j++)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Status.IsComplete)
			{
				m_EmptyGearPanels[j].ForceComplete();
			}
			else
			{
				m_EmptyGearPanels[j].Activate();
			}
		}
		m_InkDemonWarningTrigger.SetActive(active: false);
		m_BendyDontRunTrigger.SetActive(active: false);
		m_DontRunClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/InkDemon/");
	}

	public override void Activate()
	{
		base.Activate();
		m_BendyController.SetActive(active: false);
		m_SearcherController.SetActive(active: true);
		m_ProjectionistScareController.Activate();
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Status.IsComplete)
		{
			for (int i = 0; i < m_GearPanels.Count; i++)
			{
				CH3GearPanel cH3GearPanel = m_GearPanels[i];
				cH3GearPanel.RotateGears();
				cH3GearPanel.ActivateInteraction();
				cH3GearPanel.OnComplete += HandleGearPanelOnComplete;
			}
			for (int j = 0; j < m_EmptyGearPanels.Count; j++)
			{
				CH3GearPanel cH3GearPanel2 = m_EmptyGearPanels[j];
				cH3GearPanel2.RotateGears();
				cH3GearPanel2.ActivateInteraction();
				cH3GearPanel2.OnComplete += HandleEmptyGearPanelOnComplete;
			}
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ThickInkTask.Status.IsStarted)
		{
			GoToNextController();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Status.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Status.IsStarted)
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
		for (int i = 0; i < m_GearPanels.Count; i++)
		{
			CH3GearPanel cH3GearPanel = m_GearPanels[i];
			if (Object.op_Implicit((Object)(object)cH3GearPanel))
			{
				cH3GearPanel.ForceComplete();
			}
		}
		for (int j = 0; j < m_EmptyGearPanels.Count; j++)
		{
			CH3GearPanel cH3GearPanel2 = m_EmptyGearPanels[j];
			if (Object.op_Implicit((Object)(object)cH3GearPanel2))
			{
				cH3GearPanel2.ForceComplete();
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
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceMissionIntroClip, "DIACH3/DIA_CH3_ALICE_01")).OnComplete += HandleBeginDialogueOnComplete;
		m_InkDemonWarningTrigger.OnEnter -= HandleInkDemonWarningTriggerOnEnter;
		m_InkDemonWarningTrigger.OnEnter += HandleInkDemonWarningTriggerOnEnter;
		m_InkDemonWarningTrigger.SetActive(active: true);
		m_BendyDontRunTrigger.OnEnter -= HandleBendyDontRunTriggerOnEnter;
		m_BendyDontRunTrigger.OnEnter += HandleBendyDontRunTriggerOnEnter;
		m_BendyDontRunTrigger.SetActive(active: true);
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
		if (m_GearCount < 1)
		{
			m_InkDemonWarningTrigger.OnEnter -= HandleInkDemonWarningTriggerOnEnter;
			m_InkDemonWarningTrigger.OnEnter += HandleInkDemonWarningTriggerOnEnter;
			m_InkDemonWarningTrigger.SetActive(active: true);
			m_BendyDontRunTrigger.OnEnter -= HandleBendyDontRunTriggerOnEnter;
			m_BendyDontRunTrigger.OnEnter += HandleBendyDontRunTriggerOnEnter;
			m_BendyDontRunTrigger.SetActive(active: true);
		}
	}

	private void ForceComplete()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_GEARS_COMPLETE_TIP"));
		m_LiftController.GoToFloor(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_LiftController.ForceCloseLift();
		});
		if (Object.op_Implicit((Object)(object)m_Piper))
		{
			m_Piper.Dispose();
		}
		m_BendyController.SetActive(active: true);
		GetWeapon();
		for (int num = 0; num < m_GearPanels.Count; num++)
		{
			CH3GearPanel cH3GearPanel = m_GearPanels[num];
			if (Object.op_Implicit((Object)(object)cH3GearPanel))
			{
				cH3GearPanel.ForceComplete();
			}
		}
		for (int num2 = 0; num2 < m_EmptyGearPanels.Count; num2++)
		{
			CH3GearPanel cH3GearPanel2 = m_EmptyGearPanels[num2];
			if (Object.op_Implicit((Object)(object)cH3GearPanel2))
			{
				cH3GearPanel2.ForceComplete();
			}
		}
		m_FinalLiftTrigger.OnEnter += HandleFinalLiftTriggerOnEnter;
		m_FinalLiftTrigger.SetActive(active: true);
		ActivateDropbox();
	}

	private void UpdateObjective()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_GEARS_START", "OBJECTIVES/CH3_OBJECTIVE_TASK_GEARS_TIP");
		for (int i = 0; i < GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Object.Length; i++)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Object[i].IsComplete)
			{
				m_GearCount++;
			}
		}
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, m_GearCount);
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
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Object[2].IsComplete)
		{
			m_Piper.gameObject.SetActive(true);
			m_Piper.OnDeath += HandlePiperOnDeath;
			m_Piper.GearSetActive(active: true);
			m_Piper.transform.position = m_PiperSpawner.transform.position;
			m_Piper.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			m_Piper.UpdateWaypointList(m_RoamingList.Waypoints);
		}
	}

	private void HandleBeginDialogueOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_GEARS_START", string.Empty, 4f));
		EnableWeapon();
	}

	protected override void BeginTask()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_GEARS", "OBJECTIVES/CH3_OBJECTIVE_TASK_GEARS_TIP", 4f);
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, 0);
		GameManager.Instance.ShowObjective(objectiveDataVO);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Status.IsStarted = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleInkDemonWarningTriggerOnEnter(object sender, EventArgs e)
	{
		m_InkDemonWarningTrigger.OnEnter -= HandleInkDemonWarningTriggerOnEnter;
		for (int i = 0; i < m_BeginTaskClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_BeginTaskClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_BEGIN_TASKS[i], isTrimmed: true));
		}
	}

	private void HandleBendyDontRunTriggerOnEnter(object sender, EventArgs e)
	{
		m_BendyDontRunTrigger.OnEnter -= HandleBendyDontRunTriggerOnEnter;
		for (int i = 0; i < m_DontRunClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_DontRunClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_INK_DEMON[i], isTrimmed: true));
		}
	}

	private void HandlePiperOnDeath(object sender, EventArgs e)
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
		m_Piper.OnDeath -= HandlePiperOnDeath;
		m_Piper.GearSetActive(active: false);
		Vector3 position = m_Piper.transform.position;
		Vector3 position2 = position + Vector3.up * 5f;
		Interactable gear = GameManager.Instance.AssetManager.CreateAsset<Interactable>("GamePlay/CH3/CH3Gear");
		gear.transform.position = position2;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(gear.transform, position, 0.5f, false), (Ease)30), (TweenCallback)delegate
		{
			GearDropOnComplete(gear);
		});
		m_BendyController.SetActive(active: true);
	}

	private void GearDropOnComplete(Interactable gear)
	{
		gear.SetActive(active: true);
		gear.OnInteracted += HandlePiperGearOnInteracted;
	}

	private void HandlePiperGearOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = (Interactable)sender;
		interactable.OnInteracted -= HandlePiperGearOnInteracted;
		GameManager.Instance.AudioManager.Play(m_TakeGearClip);
		interactable.Dispose();
		m_GearCount++;
		GameManager.Instance.CurrentObjective.ItemCounter++;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Object[2].IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		if (m_GearCount < 3)
		{
			GameManager.Instance.GameDataManager.Save();
		}
		CheckStatus();
	}

	private void HandleGearPanelOnComplete(object sender, EventArgs e)
	{
		CH3GearPanel cH3GearPanel = (CH3GearPanel)sender;
		cH3GearPanel.OnComplete -= HandleGearPanelOnComplete;
		if (m_GearPanels.Contains(cH3GearPanel))
		{
			m_GearPanels.Remove(cH3GearPanel);
			m_GearCount++;
			GameManager.Instance.CurrentObjective.ItemCounter++;
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Object[cH3GearPanel.ID].IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		if (m_GearCount < 3)
		{
			GameManager.Instance.GameDataManager.Save();
		}
		CheckStatus();
	}

	private void CheckStatus()
	{
		if (m_GearCount == 1)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryTrueClip, "DIACH3/DIA_CH3_HENRY_28"));
		}
		else if (m_GearCount == 2)
		{
			m_LiftController.GoToRandomFloor();
		}
		else
		{
			if (m_GearCount < 3)
			{
				return;
			}
			for (int i = 0; i < m_MissionCompleteClips.Length; i++)
			{
				AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MissionCompleteClips[i], SubtitleConstants.DIA_CH3_ALICE_GEAR_END[i], isTrimmed: true));
				if (i >= m_MissionCompleteClips.Length - 1)
				{
					audioObject.OnComplete += HandleMissionCompleteDialogueOnComplete;
				}
			}
			CollectAllGears();
		}
	}

	private void CollectAllGears()
	{
		for (int i = 0; i < m_GearPanels.Count; i++)
		{
			m_GearPanels[i].DisableInteraction();
		}
		m_FinalLiftTrigger.SetActive(active: true);
		m_FinalLiftTrigger.OnEnter += HandleFinalLiftTriggerOnEnter;
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.SPARE_PARTS);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Status.IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
		ActivateDropbox();
	}

	private void HandleEmptyGearPanelOnComplete(object sender, EventArgs e)
	{
		(sender as CH3GearPanel).OnComplete -= HandleEmptyGearPanelOnComplete;
		if (m_FalseCount == 0)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryFalseClip_01, "DIACH3/DIA_CH3_HENRY_26"));
		}
		else if (m_FalseCount == 1)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryFalseClip_02, "DIACH3/DIA_CH3_HENRY_27"));
		}
		m_FalseCount++;
	}

	private void HandleMissionCompleteDialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleMissionCompleteDialogueOnComplete;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_GEARS_COMPLETE_TIP", 4f));
	}

	private void HandleFinalLiftTriggerOnEnter(object sender, EventArgs e)
	{
		m_FinalLiftTrigger.OnEnter -= HandleFinalLiftTriggerOnEnter;
		for (int i = 0; i < m_MissionEndClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MissionEndClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_TASK_GEARS_LIFT[i], isTrimmed: true));
		}
	}

	private void ClearWarningDialogues()
	{
		if (Object.op_Implicit((Object)(object)m_InkDemonWarningTrigger))
		{
			m_InkDemonWarningTrigger.SetActive(active: false);
			m_InkDemonWarningTrigger.OnEnter -= HandleInkDemonWarningTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_BendyDontRunTrigger))
		{
			m_BendyDontRunTrigger.SetActive(active: false);
			m_BendyDontRunTrigger.OnEnter -= HandleBendyDontRunTriggerOnEnter;
		}
	}

	protected override void OnDisposed()
	{
		ClearWarningDialogues();
		if ((Object)(object)m_FinalLiftTrigger != (Object)null)
		{
			m_FinalLiftTrigger.Dispose();
		}
		m_Piper = null;
		m_MissionEndClips = null;
		m_BeginTaskClips = null;
		m_DontRunClips = null;
		m_TakeGearClip = null;
		m_AliceMissionIntroClip = null;
		m_HenryTrueClip = null;
		m_HenryFalseClip_01 = null;
		m_HenryFalseClip_02 = null;
		m_MissionCompleteClips = null;
		m_ObjectiveSprite = null;
		base.OnDisposed();
	}
}
