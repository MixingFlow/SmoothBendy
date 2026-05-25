public interface IAchievements
{
	void Initialize(AchievementIDMapping[] mapping = null);

	void SetAchievementUser(int userID);

	bool GetAchievement(AchievementName name);

	void SetAchievement(AchievementName name);

	void ClearAchievement(AchievementName name);

	void SetStat(StatName name, int data);

	void SetStat(StatName name, float data);

	void GetStat(StatName name, out int? data);

	void GetStat(StatName name, out float? data);

	void IndicateProgress(AchievementName name, uint currentProgress, uint maxProgress);

	void StoreStats();

	void ResetStats();

	void ResetStatsAndAchievements();

	bool IsConnected();
}
