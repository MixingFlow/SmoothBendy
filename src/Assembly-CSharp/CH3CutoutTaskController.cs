using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CH3CutoutTaskController : CH3BaseTaskController
{
	[Header("<Controllers>")]
	[SerializeField]
	private CH3LiftController m_LiftController;

	[Header("Task Objective")]
	[SerializeField]
	private List<Breakable> m_Cutouts;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private int m_CutoutMax;

	private int m_CutoutCount;

	private Sprite m_ObjectiveSprite;

	private AudioClip[] m_MissionEndClips;

	private AudioClip[] m_BeginTaskClips;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ObjectiveSprite = GameManager.Instance.AssetManager.GetAsset<Sprite>("UI/ObjectiveIcons/cutout_icon");
		m_BeginTaskClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskCutoutStart/");
		m_MissionEndClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskCutoutEnd/");
		m_CutoutMax = m_Cutouts.Count;
		SetWeapon(m_WeaponStationController.WeaponStation.m_Axe);
	}

	public override void Activate()
	{
		base.Activate();
		m_SearcherController.SetActive(active: true);
		m_GangsterController.SetActive(active: true);
		for (int i = 0; i < m_Cutouts.Count; i++)
		{
			Breakable breakable = m_Cutouts[i];
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Object[i].ID == i && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Object[i].IsComplete)
			{
				breakable.Dispose();
			}
		}
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Status.IsComplete)
		{
			for (int j = 0; j < m_Cutouts.Count; j++)
			{
				Breakable breakable2 = m_Cutouts[j];
				if (Object.op_Implicit((Object)(object)breakable2))
				{
					breakable2.OnBroken += HandleCutoutOnBroken;
				}
			}
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ButcherGangTask.IsStarted)
		{
			GoToNextController();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Status.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Status.IsStarted)
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
		for (int i = 0; i < m_Cutouts.Count; i++)
		{
			Breakable breakable = m_Cutouts[i];
			if (Object.op_Implicit((Object)(object)breakable))
			{
				breakable.Dispose();
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
		m_WeaponStationController.Block();
		for (int i = 0; i < m_BeginTaskClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_BeginTaskClips[i], SubtitleConstants.DIA_CH3_ALICE_CUTOUTS_START[i], isTrimmed: true));
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
		GetWeapon();
	}

	private void ForceComplete()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_CUTOUT_COMPLETE_TIP"));
		m_LiftController.GoToFloor(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_LiftController.ForceCloseLift();
		});
		m_BendyController.SetActive(active: true);
		GetWeapon();
		for (int num = 0; num < m_Cutouts.Count; num++)
		{
			Breakable breakable = m_Cutouts[num];
			if (Object.op_Implicit((Object)(object)breakable))
			{
				breakable.Dispose();
			}
		}
		ActivateDropbox();
	}

	private void UpdateObjective()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_CUTOUT_START", "OBJECTIVES/CH3_OBJECTIVE_TASK_CUTOUT_TIP");
		for (int i = 0; i < GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Object.Length; i++)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Object[i].IsComplete)
			{
				m_CutoutCount++;
			}
		}
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, m_CutoutMax - m_CutoutCount);
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

	protected override void BeginTask()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_CUTOUT", "OBJECTIVES/CH3_OBJECTIVE_TASK_CUTOUT_TIP", 4f);
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, m_CutoutMax);
		GameManager.Instance.ShowObjective(objectiveDataVO);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Status.IsStarted = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleBeginDialogueOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_CUTOUT_START", string.Empty, 4f));
		EnableWeapon();
	}

	private void HandleCutoutOnBroken(object sender, EventArgs e)
	{
		Breakable breakable = (Breakable)sender;
		breakable.OnBroken -= HandleCutoutOnBroken;
		int num = -1;
		if (m_Cutouts.Contains(breakable))
		{
			num = m_Cutouts.IndexOf(breakable);
			m_CutoutCount++;
			GameManager.Instance.CurrentObjective.ItemCounter--;
		}
		if (num > -1)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Object[num].IsComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
			if (m_CutoutCount == 6 || m_CutoutCount == 12)
			{
				GameManager.Instance.GameDataManager.Save();
			}
		}
		CheckStatus();
	}

	private void CheckStatus()
	{
		if (m_CutoutCount == 2)
		{
			m_LiftController.GoToRandomFloor();
		}
		else
		{
			if (m_CutoutCount < m_CutoutMax)
			{
				return;
			}
			m_LiftController.GoToRandomFloor();
			for (int i = 0; i < m_MissionEndClips.Length; i++)
			{
				AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MissionEndClips[i], SubtitleConstants.DIA_CH3_ALICE_CUTOUTS_END[i], isTrimmed: true));
				if (i >= m_MissionEndClips.Length - 1)
				{
					audioObject.OnComplete += HandleMissionEndClipsOnComplete;
				}
			}
			BreakAllCutouts();
		}
	}

	private void BreakAllCutouts()
	{
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.ANGER_MANAGEMENT);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.CutoutTask.Status.IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleMissionEndClipsOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_CUTOUT_COMPLETE_TIP", 4f));
		m_BendyController.GoToCutout();
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.ANGER_MANAGEMENT);
		ActivateDropbox();
	}

	protected override void OnDisposed()
	{
		m_ObjectiveSprite = null;
		m_MissionEndClips = null;
		m_BeginTaskClips = null;
		base.OnDisposed();
	}
}
