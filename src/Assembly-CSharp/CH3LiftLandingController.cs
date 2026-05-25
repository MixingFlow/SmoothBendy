using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH3LiftLandingController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private CH3LiftController m_LiftController;

	[Header("Objective: Get To The Lift!")]
	[SerializeField]
	private EventTrigger m_ActivateTrigger;

	[SerializeField]
	private EventTrigger m_LiftLowerTrigger;

	[SerializeField]
	private EventTrigger m_BorisToLiftTrigger;

	[SerializeField]
	private WaypointList m_LiftEntrancePath;

	[SerializeField]
	private WaypointList m_LiftPath;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private BorisAi m_Boris => GameManager.Instance.CharacterManager.Boris;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ActivateTrigger.SetActive(active: false);
		m_LiftLowerTrigger.SetActive(active: false);
		m_BorisToLiftTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceLairObjective.IsStarted)
		{
			ForceGoToAlice();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.EnterLiftObjective.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.5f, (TweenCallback)delegate
		{
			m_Boris.AddToWaypointList(m_LiftEntrancePath.Waypoints);
		});
		m_ActivateTrigger.OnEnter += HandleActivateTriggerOnEnter;
		m_ActivateTrigger.SetActive(active: true);
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_08");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		m_Boris.StopWaypointPathing();
		m_LiftController.EnableBoris();
		m_LiftController.GoToFloor(0, isInitialArrival: true, wasCalled: true);
		m_LiftLowerTrigger.OnEnter += HandleLiftLowerTriggerOnEnter;
		m_LiftLowerTrigger.SetActive(active: true);
	}

	private void ForceGoToAlice()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_16", "OBJECTIVES/CH3_OBJECTIVE_16_TIP"));
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_08");
		SendOnComplete();
	}

	private void HandleActivateTriggerOnEnter(object sender, EventArgs e)
	{
		m_ActivateTrigger.OnEnter -= HandleActivateTriggerOnEnter;
		m_LiftController.InitialArrival();
		m_BorisToLiftTrigger.OnEnter += HandleBorisToLiftTriggerOnEnter;
		m_BorisToLiftTrigger.SetActive(active: true);
	}

	private void HandleBorisToLiftTriggerOnEnter(object sender, EventArgs e)
	{
		m_BorisToLiftTrigger.OnEnter -= HandleBorisToLiftTriggerOnEnter;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.EnterLiftObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		m_Boris.OnWaypointComplete += HandleBorisLiftNodeOnComplete;
		m_Boris.AddToWaypointList(m_LiftPath.Waypoints);
	}

	private void HandleBorisLiftNodeOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleBorisLiftNodeOnComplete;
		m_Boris.StopWaypointPathing();
		if ((Object)(object)m_LiftEntrancePath != (Object)null)
		{
			m_LiftEntrancePath.Dispose();
		}
		m_LiftPath.Dispose();
		m_Boris.transform.SetParent(m_LiftController.Lift);
		m_LiftController.EnableBoris();
		m_LiftLowerTrigger.OnEnter += HandleLiftLowerTriggerOnEnter;
		m_LiftLowerTrigger.SetActive(active: true);
	}

	private void HandleLiftLowerTriggerOnEnter(object sender, EventArgs e)
	{
		m_LiftLowerTrigger.OnEnter -= HandleLiftLowerTriggerOnEnter;
		m_LiftController.GoToAlice();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_ActivateTrigger))
		{
			m_ActivateTrigger.OnEnter -= HandleActivateTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_LiftLowerTrigger))
		{
			m_LiftLowerTrigger.OnEnter -= HandleLiftLowerTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_BorisToLiftTrigger))
		{
			m_BorisToLiftTrigger.OnEnter -= HandleBorisToLiftTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_Boris))
		{
			m_Boris.OnWaypointComplete -= HandleBorisLiftNodeOnComplete;
		}
		base.OnDisposed();
	}
}
