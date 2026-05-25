using System;
using System.Collections.Generic;
using UnityEngine;

public class MeatlyController : BaseController
{
	[SerializeField]
	private Chapters m_Chapter;

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
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip09, "DIACH1/DIA_CH1_HENRY_09"));
		audioObject.OnComplete += delegate
		{
			switch (m_Chapter)
			{
			case Chapters.ONE:
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_CREATOR);
				break;
			case Chapters.TWO:
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.MAN_BEHIND_THE_CURTAIN);
				break;
			case Chapters.THREE:
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.TEA_TIME);
				break;
			case Chapters.FOUR:
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.BARBECUED);
				break;
			case Chapters.FIVE:
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.A_SWEET_DISCOVERY);
				break;
			}
			CheckAllMeatlyAchievements();
		};
	}

	private void CheckAllMeatlyAchievements()
	{
		if (GameManager.Instance.AchievementManager.GetAchievement(AchievementName.THE_CREATOR) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.MAN_BEHIND_THE_CURTAIN) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.TEA_TIME) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.BARBECUED) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.A_SWEET_DISCOVERY))
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.GRAND_PUPPETEER);
		}
	}

	protected override void OnDisposed()
	{
		m_HenryClip09 = null;
		base.OnDisposed();
	}
}
