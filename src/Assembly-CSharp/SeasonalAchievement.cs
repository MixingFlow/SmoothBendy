using System;
using TMG.Core;
using UnityEngine;

public class SeasonalAchievement : TMGMonoBehaviour
{
	[SerializeField]
	private AchievementName m_Achievement;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	public override void InitOnComplete()
	{
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		GameManager.Instance.AchievementManager.SetAchievement(m_Achievement);
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_EventTrigger != (Object)null)
		{
			m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
