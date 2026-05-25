using UnityEngine;

public class SwitchAchievements : IAchievements
{
	public void SetAchievementUser(int userID)
	{
	}

	public void Initialize(AchievementIDMapping[] mapping = null)
	{
	}

	public bool GetAchievement(AchievementName name)
	{
		Debug.Log((object)"*** SwitchAchievements = in GetAchievement()");
		return false;
	}

	public void SetAchievement(AchievementName name)
	{
	}

	public void ClearAchievement(AchievementName name)
	{
	}

	public void SetStat(StatName name, int data)
	{
	}

	public void SetStat(StatName name, float data)
	{
		SetStat(name, (int)data);
	}

	public void GetStat(StatName name, out int? data)
	{
		data = 0;
	}

	public void GetStat(StatName name, out float? data)
	{
		data = 0f;
	}

	public void IndicateProgress(AchievementName name, uint currentProgress, uint maxProgress)
	{
	}

	public void StoreStats()
	{
	}

	public void ResetStats()
	{
	}

	public void ResetStatsAndAchievements()
	{
	}

	public bool IsConnected()
	{
		return false;
	}

	private void SendUpdateStats()
	{
	}
}
