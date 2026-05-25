using System;
using System.Collections.Generic;
using UnityEngine;

public class CH3MeatlyController : BaseController
{
	[SerializeField]
	private List<Collider> m_FakeWalls;

	[SerializeField]
	private EventTrigger m_AchievementTrigger;

	private AudioClip m_HenryClip09;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HenryClip09 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_09");
		m_AchievementTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		for (int i = 0; i < m_FakeWalls.Count; i++)
		{
			m_FakeWalls[i].enabled = false;
		}
		m_AchievementTrigger.SetActive(active: true);
		m_AchievementTrigger.OnEnter += HandleMeatlyAchievementTrigger;
	}

	private void HandleMeatlyAchievementTrigger(object sender, EventArgs e)
	{
		m_AchievementTrigger.OnEnter -= HandleMeatlyAchievementTrigger;
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip09, "DIACH1/DIA_CH1_HENRY_07"));
		audioObject.OnComplete += delegate
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.TEA_TIME);
		};
	}

	protected override void OnDisposed()
	{
		m_HenryClip09 = null;
		base.OnDisposed();
	}
}
