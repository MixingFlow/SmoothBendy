using System;
using System.Collections.Generic;
using UnityEngine;

public class CH1CollectableController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private CH1JumpScareController m_JumpscareController;

	[SerializeField]
	private CH1SammysRoomController m_SammysRoomController;

	[Header("Objectives")]
	[SerializeField]
	private EventTrigger m_MainPowerTrigger;

	[Header("Collectables")]
	[SerializeField]
	private List<CH1Pedestal> m_Pedestals;

	[SerializeField]
	private List<Transform> m_Locations;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_HenryClip04;

	private AudioClip m_HenryClip05;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HenryClip04 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_04");
		m_HenryClip05 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_05");
		m_MainPowerTrigger.SetActive(active: false);
		for (int i = 0; i < m_Pedestals.Count; i++)
		{
			m_Pedestals[i].Initialize(m_Locations[i]);
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsComplete)
		{
			return;
		}
		for (int j = 0; j < m_Pedestals.Count; j++)
		{
			if (!m_Pedestals[j].isComplete)
			{
				return;
			}
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Record.IsStarted)
		{
			m_SammysRoomController.ForceOpen();
		}
		else
		{
			m_SammysRoomController.Activate();
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsStarted)
		{
			ForceStart();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		m_JumpscareController.Activate();
		m_MainPowerTrigger.OnEnter += HandleMainPowerTriggerOnEnter;
		m_MainPowerTrigger.SetActive(active: true);
	}

	private void ForceStart()
	{
		UpdateObjective();
		for (int i = 0; i < m_Pedestals.Count; i++)
		{
			CH1Pedestal cH1Pedestal = m_Pedestals[i];
			if (!cH1Pedestal.isCollected && !cH1Pedestal.isComplete)
			{
				cH1Pedestal.OnCollect += HandleCollectableOnCollect;
				cH1Pedestal.OnComplete += HandlePedestalOnComplete;
				cH1Pedestal.Activate();
			}
			else if (cH1Pedestal.isCollected && !cH1Pedestal.isComplete)
			{
				cH1Pedestal.OnComplete += HandlePedestalOnComplete;
				cH1Pedestal.Activate();
			}
		}
	}

	private void ForceComplete()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_04", "OBJECTIVES/CH1_OBJ_04_TIP", 4f));
		SendOnComplete();
	}

	private void HandleMainPowerTriggerOnEnter(object sender, EventArgs e)
	{
		m_MainPowerTrigger.OnEnter -= HandleMainPowerTriggerOnEnter;
		m_MainPowerTrigger.Dispose();
		GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsStarted = true;
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip04, "DIACH1/DIA_CH1_HENRY_04"));
		audioObject.OnComplete += delegate
		{
			UpdateObjective(isShow: true);
		};
		for (int num = 0; num < m_Pedestals.Count; num++)
		{
			CH1Pedestal cH1Pedestal = m_Pedestals[num];
			cH1Pedestal.Activate();
			cH1Pedestal.OnCollect += HandleCollectableOnCollect;
			cH1Pedestal.OnComplete += HandlePedestalOnComplete;
		}
	}

	private void UpdateObjective(bool isShow = false)
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_03", "OBJECTIVES/CH1_OBJ_03_TIP", 4f);
		List<Sprite> list = new List<Sprite>();
		for (int i = 0; i < m_Pedestals.Count; i++)
		{
			list.Add(m_Pedestals[i].MenuSprite);
		}
		objectiveDataVO.AddItems(list);
		if (isShow)
		{
			GameManager.Instance.ShowObjective(objectiveDataVO);
			return;
		}
		GameManager.Instance.UpdateObjective(objectiveDataVO);
		for (int j = 0; j < m_Pedestals.Count; j++)
		{
			m_Pedestals[j].UpdateObjective();
		}
	}

	private void HandleCollectableOnCollect(object sender, EventArgs e)
	{
		CH1Pedestal cH1Pedestal = (CH1Pedestal)sender;
		cH1Pedestal.OnCollect -= HandleCollectableOnCollect;
		for (int i = 0; i < m_Pedestals.Count; i++)
		{
			if (!m_Pedestals[i].isCollected)
			{
				return;
			}
		}
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip05, "DIACH1/DIA_CH1_HENRY_05"));
	}

	private void HandlePedestalOnComplete(object sender, EventArgs e)
	{
		CH1Pedestal cH1Pedestal = sender as CH1Pedestal;
		if ((Object)(object)cH1Pedestal == (Object)null)
		{
			return;
		}
		cH1Pedestal.OnComplete -= HandlePedestalOnComplete;
		for (int i = 0; i < m_Pedestals.Count; i++)
		{
			if (!m_Pedestals[i].isComplete)
			{
				return;
			}
		}
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.PICKING_UP_THE_PIECES);
		SendOnComplete();
	}

	public void TurnOffLights()
	{
		for (int i = 0; i < m_Pedestals.Count; i++)
		{
			m_Pedestals[i].TurnLightOff();
		}
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_MainPowerTrigger))
		{
			m_MainPowerTrigger.OnEnter -= HandleMainPowerTriggerOnEnter;
		}
		if (m_Pedestals != null)
		{
			m_Pedestals.Clear();
			m_Pedestals = null;
		}
		m_HenryClip04 = null;
		m_HenryClip05 = null;
		base.OnDisposed();
	}
}
