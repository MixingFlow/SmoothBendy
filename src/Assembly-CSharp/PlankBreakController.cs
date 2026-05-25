using System;
using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class PlankBreakController : TMGMonoBehaviour
{
	[Serializable]
	public class Plank
	{
		public EventTrigger BreakTrigger;

		public Transform AudioPosition;

		public Transform BreakPosition;

		public Breakable BreakablePlank;

		public int HashID => ((object)BreakTrigger).GetHashCode();
	}

	[Header("Breakable Planks")]
	[SerializeField]
	private List<Plank> m_Planks;

	private int m_PlankCount;

	private int m_BrokenPlankCount;

	public override void Init()
	{
		base.Init();
		m_PlankCount = m_Planks.Count;
		for (int i = 0; i < m_PlankCount; i++)
		{
			m_Planks[i].BreakTrigger.OnEnter += HandleBreakTriggerOnEnter;
		}
	}

	private void HandleBreakTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		int hashCode = sender.GetHashCode();
		for (int i = 0; i < m_PlankCount; i++)
		{
			Plank plank = m_Planks[i];
			if (plank.HashID == hashCode)
			{
				plank.BreakTrigger.OnEnter -= HandleBreakTriggerOnEnter;
				GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/Weapons/Axe/SFX_Axe_Wood_Hit_Crack_06", plank.AudioPosition.position);
				plank.BreakablePlank.Destroy(plank.BreakPosition.position);
				m_BrokenPlankCount++;
				break;
			}
		}
		if (m_BrokenPlankCount >= m_Planks.Count)
		{
			Dispose();
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < m_PlankCount; i++)
		{
			m_Planks[i].BreakTrigger.OnEnter -= HandleBreakTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
