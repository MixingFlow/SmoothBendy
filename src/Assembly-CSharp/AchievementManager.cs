using TMG.Core;

public class AchievementManager : TMGAbstractDisposable
{
	private IAchievements m_AchievementController;

	private AchievementIDMapping[] m_AchievementIDs;

	private void InitializeStatAchievements()
	{
	}

	public void CheckAchievementsAgainstStatRequirements(StatName statUpdated)
	{
	}

	private void DebugOutAllStatProgress()
	{
	}

	private StatAchievement GetStatAchievementByStatName(StatName statName)
	{
		return null;
	}

	public bool GetAchievement(AchievementName key)
	{
		if (CanUseAchievements())
		{
			return m_AchievementController.GetAchievement(key);
		}
		return false;
	}

	public void SetAchievement(AchievementName key)
	{
		if (CanUseAchievements())
		{
			m_AchievementController.SetAchievement(key);
			m_AchievementController.StoreStats();
		}
	}

	public void ClearAchievement(AchievementName key)
	{
		if (CanUseAchievements())
		{
			m_AchievementController.ClearAchievement(key);
		}
	}

	public void GetStat(StatName name, out int? data)
	{
		if (!CanUseAchievements())
		{
			data = null;
		}
		else
		{
			m_AchievementController.GetStat(name, out data);
		}
	}

	public void GetStat(StatName name, out float? data)
	{
		if (!CanUseAchievements())
		{
			data = null;
		}
		else
		{
			m_AchievementController.GetStat(name, out data);
		}
	}

	public void SetStat(StatName name, int data)
	{
		if (CanUseAchievements())
		{
			StatAchievement statAchievementByStatName = GetStatAchievementByStatName(name);
			if (statAchievementByStatName != null)
			{
				statAchievementByStatName.CurrentValue = data;
			}
			m_AchievementController.SetStat(name, data);
		}
	}

	public void SetStat(StatName name, float data)
	{
		if (CanUseAchievements())
		{
			m_AchievementController.SetStat(name, data);
		}
	}

	public void IncrementStatAchievement(StatName name)
	{
		CanUseAchievements();
	}

	public void IncrementStatByValue(StatName name, int incrementAmount)
	{
		CanUseAchievements();
	}

	public void SaveStatAchievement(StatName name)
	{
		CanUseAchievements();
	}

	public void IndicateProgress(AchievementName name, uint currentProgress, uint maxProgress)
	{
		if (CanUseAchievements())
		{
			m_AchievementController.IndicateProgress(name, currentProgress, maxProgress);
		}
	}

	public void StoreStats()
	{
		if (CanUseAchievements())
		{
			m_AchievementController.StoreStats();
		}
	}

	public void ResetStats()
	{
		if (CanUseAchievements())
		{
			m_AchievementController.ResetStats();
			m_AchievementController.StoreStats();
		}
	}

	public void ResetStatsAndAchievements()
	{
		if (CanUseAchievements())
		{
			m_AchievementController.ResetStatsAndAchievements();
			m_AchievementController.StoreStats();
		}
	}

	public void SetAchievementUser(int id)
	{
		m_AchievementController.SetAchievementUser(id);
	}

	private bool CanUseAchievements()
	{
		if (m_AchievementController != null)
		{
			return m_AchievementController.IsConnected();
		}
		return false;
	}

	public void InitSteam()
	{
		m_AchievementIDs = new AchievementIDMapping[0];
		m_AchievementController = new SteamAchievements();
		if (m_AchievementController != null)
		{
			m_AchievementController.Initialize(m_AchievementIDs);
		}
	}

	public void InitGOG()
	{
		m_AchievementIDs = new AchievementIDMapping[0];
		if (m_AchievementController != null)
		{
			m_AchievementController.Initialize(m_AchievementIDs);
		}
	}

	public void InitEpic()
	{
		m_AchievementIDs = new AchievementIDMapping[0];
		m_AchievementController = new EpicAchievements();
		if (m_AchievementController != null)
		{
			m_AchievementController.Initialize(m_AchievementIDs);
		}
	}
}
