using UnityEngine;

public class EpicAchievements : IAchievements
{
	private bool _IsInitialized;

	public void Initialize(AchievementIDMapping[] mapping = null)
	{
		_IsInitialized = true;
	}

	public bool GetAchievement(AchievementName achievement)
	{
		if (!Object.op_Implicit((Object)(object)EOSController.instance))
		{
			return false;
		}
		string achievementName = achievement.ToString();
		return EOSController.instance.QueryAchievement(achievementName) >= 1.0;
	}

	public void SetAchievement(AchievementName achievement)
	{
		if (Object.op_Implicit((Object)(object)EOSController.instance))
		{
			string achievement2 = achievement.ToString();
			EOSController.instance.UnlockAchievementAsync(achievement2);
		}
	}

	public void ClearAchievement(AchievementName name)
	{
	}

	public bool IsConnected()
	{
		if (Object.op_Implicit((Object)(object)EOSController.instance))
		{
			return EOSController.instance.IsInitialized;
		}
		return false;
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

	public void SetAchievementUser(int userID)
	{
	}

	public void SetStat(StatName name, int data)
	{
	}

	public void SetStat(StatName name, float data)
	{
	}

	public void ResetStats()
	{
	}

	public void ResetStatsAndAchievements()
	{
	}

	public void StoreStats()
	{
	}
}
