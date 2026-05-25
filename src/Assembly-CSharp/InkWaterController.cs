using System;
using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class InkWaterController : TMGMonoBehaviour
{
	[Header("Event Triggers")]
	[SerializeField]
	private List<EventTrigger> m_EventTriggers;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_EventTriggers.Count; i++)
		{
			m_EventTriggers[i].OnEnter += HandleEventTriggerOnEnter;
			m_EventTriggers[i].OnExit += HandleEventTriggerOnExit;
		}
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		Enter((EventTrigger)sender);
	}

	private void HandleEventTriggerOnExit(object sender, EventArgs e)
	{
		Exit((EventTrigger)sender);
	}

	private void Enter(EventTrigger trigger)
	{
		GameManager.Instance.Player.SetSlowed(active: true);
		GameManager.Instance.Player.SetJump(active: false);
	}

	private void Exit(EventTrigger trigger)
	{
		GameManager.Instance.Player.SetSlowed(active: false);
		GameManager.Instance.Player.SetJump(active: true);
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < m_EventTriggers.Count; i++)
		{
			m_EventTriggers[i].OnEnter -= HandleEventTriggerOnEnter;
			m_EventTriggers[i].OnExit -= HandleEventTriggerOnExit;
		}
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		base.OnDisposed();
	}
}
