using System;
using System.Collections.Generic;
using UnityEngine;

public class CH3InkWaterController : BaseController
{
	[SerializeField]
	private List<EventTrigger> m_InkWaters;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_InkWaters.Count; i++)
		{
			m_InkWaters[i].OnEnter += HandleWaterTriggerOnEnter;
			m_InkWaters[i].OnExit += HandleWaterTriggerOnExit;
		}
	}

	private void HandleWaterTriggerOnEnter(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetRun(active: false);
	}

	private void HandleWaterTriggerOnExit(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetRun(active: true);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
