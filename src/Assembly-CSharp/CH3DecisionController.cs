using System;
using S13Audio;
using UnityEngine;

public class CH3DecisionController : BaseController
{
	[Header("Choose Bendy!")]
	[SerializeField]
	private EventTrigger m_BendyTrigger;

	[SerializeField]
	private EventTrigger m_SlowTrigger;

	[SerializeField]
	private DisposableObject m_AliceRoom;

	[SerializeField]
	private BaseDoorController m_AliceDoor;

	[SerializeField]
	private GameObject m_AliceGate;

	[SerializeField]
	private GameObject m_AliceWall;

	[Header("Choose Alice!")]
	[SerializeField]
	private EventTrigger m_AliceTrigger;

	[SerializeField]
	private DisposableObject m_BendyRoom;

	[SerializeField]
	private BaseDoorController m_BendyDoor;

	[SerializeField]
	private GameObject m_BendyGate;

	[SerializeField]
	private GameObject m_BendyWall;

	[Header("Chosen!")]
	[SerializeField]
	private EventTrigger m_DecisionCompleteTrigger;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_GateCloseClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_GateCloseClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Gate_Close_01");
		m_SlowTrigger.SetActive(active: false);
		m_BendyTrigger.SetActive(active: false);
		m_AliceTrigger.SetActive(active: false);
		m_DecisionCompleteTrigger.SetActive(active: false);
		m_BendyGate.SetActive(false);
		m_AliceGate.SetActive(false);
		m_BendyWall.SetActive(false);
		m_AliceWall.SetActive(false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.DecisionObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_BendyTrigger.SetActive(active: true);
		m_BendyTrigger.OnEnter += HandleBendyTriggerOnEnter;
		m_AliceTrigger.SetActive(active: true);
		m_AliceTrigger.OnEnter += HandleAliceTriggerOnEnter;
		m_DecisionCompleteTrigger.SetActive(active: true);
		m_DecisionCompleteTrigger.OnEnter += HandleDecisionTriggerOnComplete;
		m_SlowTrigger.SetActive(active: true);
		m_SlowTrigger.OnEnter += HandleSlowTriggerOnEnter;
		m_SlowTrigger.OnExit += HandleSlowTriggerOnExit;
	}

	private void HandleSlowTriggerOnEnter(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetSlowed(active: true);
		GameManager.Instance.Player.SetJump(active: false);
	}

	private void HandleSlowTriggerOnExit(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetSlowed(active: false);
		GameManager.Instance.Player.SetJump(active: true);
	}

	private void HandleBendyTriggerOnEnter(object sender, EventArgs e)
	{
		m_BendyTrigger.OnEnter -= HandleBendyTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_GateCloseClip);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ChoseDevilsPath = true;
		m_AliceRoom.Dispose();
		m_AliceGate.SetActive(true);
		m_AliceWall.SetActive(true);
	}

	private void HandleAliceTriggerOnEnter(object sender, EventArgs e)
	{
		m_AliceTrigger.OnEnter -= HandleAliceTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_GateCloseClip);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ChoseDevilsPath = false;
		m_SlowTrigger.OnEnter -= HandleSlowTriggerOnEnter;
		m_SlowTrigger.OnExit -= HandleSlowTriggerOnExit;
		m_BendyRoom.Dispose();
		m_BendyGate.SetActive(true);
		m_BendyWall.SetActive(true);
	}

	private void HandleDecisionTriggerOnComplete(object sender, EventArgs e)
	{
		m_DecisionCompleteTrigger.OnEnter -= HandleDecisionTriggerOnComplete;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.DecisionObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_05");
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ChoseDevilsPath)
		{
			m_AliceRoom.Dispose();
			m_AliceGate.SetActive(true);
			m_AliceDoor.ForceOpen(145f);
			m_AliceWall.SetActive(true);
			m_SlowTrigger.SetActive(active: true);
			m_SlowTrigger.OnEnter += HandleSlowTriggerOnEnter;
			m_SlowTrigger.OnExit += HandleSlowTriggerOnExit;
		}
		else
		{
			m_BendyRoom.Dispose();
			m_BendyGate.SetActive(true);
			m_BendyDoor.ForceOpen(-145f);
			m_BendyWall.SetActive(true);
		}
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_BendyTrigger))
		{
			m_BendyTrigger.OnEnter -= HandleBendyTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_AliceTrigger))
		{
			m_AliceTrigger.OnEnter -= HandleAliceTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_DecisionCompleteTrigger))
		{
			m_DecisionCompleteTrigger.OnEnter -= HandleDecisionTriggerOnComplete;
		}
		if (Object.op_Implicit((Object)(object)m_SlowTrigger))
		{
			m_SlowTrigger.OnEnter -= HandleSlowTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_SlowTrigger))
		{
			m_SlowTrigger.OnExit -= HandleSlowTriggerOnExit;
		}
		m_GateCloseClip = null;
		base.OnDisposed();
	}
}
