using System;
using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class BlockedDoorController : TMGMonoBehaviour
{
	[SerializeField]
	private BaseDoorController m_Door;

	[SerializeField]
	private List<Breakable> m_Planks;

	public BaseDoorController Door => m_Door;

	public event EventHandler OnUnlocked;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_Planks.Count; i++)
		{
			m_Planks[i].OnBroken += HandlePlankOnBroken;
		}
	}

	public void ForceOpen()
	{
		for (int i = 0; i < m_Planks.Count; i++)
		{
			m_Planks[i].Dispose();
			m_Door.Unlock();
		}
	}

	private void HandlePlankOnBroken(object sender, EventArgs e)
	{
		Breakable breakable = (Breakable)sender;
		breakable.OnBroken -= HandlePlankOnBroken;
		if (m_Planks.Contains(breakable))
		{
			m_Planks.Remove(breakable);
		}
		if (m_Planks.Count <= 0)
		{
			m_Door.Unlock();
			this.OnUnlocked.Send(this);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
