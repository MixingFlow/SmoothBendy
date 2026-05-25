using UnityEngine;

public class PS4Trophies : IAchievements
{
	public void SetAchievementUser(int userID)
	{
	}

	public bool GetAchievement(AchievementName achievementName)
	{
		return false;
	}

	private int GetTrophyIDFromAchievement(AchievementName achievementName)
	{
		return 0;
	}

	public void SetAchievement(AchievementName achievementName)
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
	}

	public void GetStat(StatName name, out int? data)
	{
		data = 0;
	}

	public void GetStat(StatName name, out float? data)
	{
		data = 0f;
	}

	public void Initialize(AchievementIDMapping[] mapping = null)
	{
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

	private void DumpGameInfo()
	{
	}

	private void TrophiesDebugLog(string message)
	{
		Debug.Log((object)("*PS4 TROPHIES* " + message));
	}
}
