using System;
using TMG.Core;
using UnityEngine;

public class DoorSideController : TMGMonoBehaviour
{
	[SerializeField]
	private BaseDoorController m_Door;

	[SerializeField]
	private EventTrigger m_ActiveSpace;

	private bool m_CanOpen;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Door.Lock();
		m_CanOpen = false;
		m_ActiveSpace.OnEnter += HandleActiveSpaceOnEnter;
		m_ActiveSpace.OnExit += HandleActiveSpaceOnExit;
	}

	private void HandleActiveSpaceOnEnter(object sender, EventArgs e)
	{
		if (!m_CanOpen)
		{
			m_CanOpen = true;
			m_Door.Unlock();
		}
	}

	private void HandleActiveSpaceOnExit(object sender, EventArgs e)
	{
		if (m_CanOpen)
		{
			m_CanOpen = false;
			m_Door.Lock();
		}
		m_ActiveSpace.ResetTrigger();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
