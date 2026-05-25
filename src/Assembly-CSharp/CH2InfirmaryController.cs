using System;
using UnityEngine;

public class CH2InfirmaryController : BaseController
{
	[Header("Objective: Go To The Sewers")]
	[SerializeField]
	private CH1PipeValve m_Valve;

	[SerializeField]
	private EventTrigger m_ValveEventTrigger;

	[SerializeField]
	private CH3LeverLight m_Lever;

	[SerializeField]
	private GenericDoorController m_GateDoor;

	[SerializeField]
	private GameObject m_Blocker;

	[SerializeField]
	private GameObject m_SammyDoorBlockage;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_HenryClip11;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HenryClip11 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_11");
		m_ValveEventTrigger.SetActive(active: false);
		m_Lever.Disable();
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InfirmaryObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_Blocker.SetActive(false);
		m_ValveEventTrigger.OnEnter += HandleValveTriggerOnEnter;
		m_ValveEventTrigger.SetActive(active: true);
	}

	private void HandleValveTriggerOnEnter(object sender, EventArgs e)
	{
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip11, "DIACH2/DIA_CH2_HENRY_11"));
		m_Valve.SetEmptyCollision(active: false);
		m_Valve.ActivateEmpty();
		m_Lever.OnComplete += HandleLeverOnComplete;
		m_Lever.Activate();
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		m_Lever.OnComplete -= HandleLeverOnComplete;
		m_GateDoor.Open();
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InfirmaryObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void ForceComplete()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SewersObjective.IsComplete)
		{
			m_Valve.ForceComplete();
		}
		else
		{
			m_Valve.SetEmptyCollision(active: false);
			m_Valve.ActivateEmpty();
		}
		m_Blocker.SetActive(false);
		m_GateDoor.ForceOpen();
		m_Lever.ForceComplete();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_Lever))
		{
			m_Lever.OnComplete -= HandleLeverOnComplete;
		}
		if (Object.op_Implicit((Object)(object)m_ValveEventTrigger))
		{
			m_ValveEventTrigger.OnEnter -= HandleValveTriggerOnEnter;
		}
		m_HenryClip11 = null;
		base.OnDisposed();
	}
}
