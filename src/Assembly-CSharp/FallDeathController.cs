using System;
using TMG.Core;
using UnityEngine;

public class FallDeathController : TMGMonoBehaviour
{
	private EventTrigger m_EventTrigger;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_EventTrigger = ((Component)this).GetComponent<EventTrigger>();
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		for (int i = 0; i < 7; i++)
		{
			GameManager.Instance.ShowHurtBorder();
		}
		m_EventTrigger.ResetTrigger();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_EventTrigger))
		{
			m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
