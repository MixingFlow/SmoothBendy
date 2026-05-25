using System;
using System.Collections.Generic;
using TMG.Data;
using UnityEngine;

public class BaconSoupController : BaseController
{
	[Header("Chapter")]
	[SerializeField]
	private Chapters m_Chapter;

	[Header("Bacon Soups")]
	[SerializeField]
	private List<CannedSoupEdible> m_BaconSoups;

	private AchievementName m_AchievementAssetKey;

	private AchievementSaveData m_AchievementDataVO;

	public override void Init()
	{
		base.Init();
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (m_Chapter == Chapters.ONE)
		{
			LoadBaconSoupCollected(GameManager.Instance.GameData.CH1AchievementData.BaconSoup);
			m_AchievementAssetKey = AchievementName.A_TASTE_OF_HOME;
			m_AchievementDataVO = GameManager.Instance.GameData.CH1AchievementData;
		}
		else if (m_Chapter == Chapters.TWO)
		{
			LoadBaconSoupCollected(GameManager.Instance.GameData.CH2AchievementData.BaconSoup);
			m_AchievementAssetKey = AchievementName.CANADIAN_BACON;
			m_AchievementDataVO = GameManager.Instance.GameData.CH2AchievementData;
		}
		else if (m_Chapter == Chapters.THREE)
		{
			LoadBaconSoupCollected(GameManager.Instance.GameData.CH3AchievementData.BaconSoup);
			m_AchievementAssetKey = AchievementName.BRING_HOME_THE_BACON;
			m_AchievementDataVO = GameManager.Instance.GameData.CH3AchievementData;
		}
		else if (m_Chapter == Chapters.FOUR)
		{
			LoadBaconSoupCollected(GameManager.Instance.GameData.CH4AchievementData.BaconSoup);
			m_AchievementAssetKey = AchievementName.JUST_LIKE_MOM_USED_TO_MAKE;
			m_AchievementDataVO = GameManager.Instance.GameData.CH4AchievementData;
		}
		else if (m_Chapter == Chapters.FIVE)
		{
			LoadBaconSoupCollected(GameManager.Instance.GameData.CH5AchievementData.BaconSoup);
			m_AchievementAssetKey = AchievementName.NO_NEED_FOR_A_SPOON;
			m_AchievementDataVO = GameManager.Instance.GameData.CH5AchievementData;
		}
		for (int i = 0; i < m_BaconSoups.Count; i++)
		{
			m_BaconSoups[i].OnInteracted += HandleCannedSoupOnInteracted;
		}
	}

	private void HandleCannedSoupOnInteracted(object sender, EventArgs e)
	{
		CannedSoupEdible cannedSoupEdible = (CannedSoupEdible)sender;
		cannedSoupEdible.OnInteracted -= HandleCannedSoupOnInteracted;
		int iD = cannedSoupEdible.GetID();
		if (m_BaconSoups.Contains(cannedSoupEdible))
		{
			m_BaconSoups.Remove(cannedSoupEdible);
			m_AchievementDataVO.BaconSoup.Add(iD);
		}
		if (m_BaconSoups.Count <= 0)
		{
			GameManager.Instance.AchievementManager.SetAchievement(m_AchievementAssetKey);
			CheckAllBaconSoupAchievements();
			Dispose();
		}
	}

	private void CheckAllBaconSoupAchievements()
	{
		if (GameManager.Instance.AchievementManager.GetAchievement(AchievementName.A_TASTE_OF_HOME) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.CANADIAN_BACON) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.BRING_HOME_THE_BACON) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.JUST_LIKE_MOM_USED_TO_MAKE) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.NO_NEED_FOR_A_SPOON))
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.MASTER_OF_BACON);
		}
	}

	public void LoadBaconSoupCollected(List<int> _BaconSoupIDsCollected)
	{
		if (GameManager.Instance.AchievementManager.GetAchievement(m_AchievementAssetKey))
		{
			return;
		}
		for (int num = m_BaconSoups.Count - 1; num > -1; num--)
		{
			CannedSoupEdible cannedSoupEdible = m_BaconSoups[num];
			if (_BaconSoupIDsCollected.Contains(cannedSoupEdible.GetID()))
			{
				cannedSoupEdible.Dispose();
				m_BaconSoups.RemoveAt(num);
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
