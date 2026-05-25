public static class AudioLogAllAchievements
{
	public static void Check()
	{
		if (GameManager.Instance.AchievementManager.GetAchievement(AchievementName.THE_PAST_SPEAKS) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.OLD_PROBLEMS) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.HEARING_VOICES) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.STILL_LISTENING) && GameManager.Instance.AchievementManager.GetAchievement(AchievementName.NOW_HEAR_THIS))
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_VOICE_COLLECTOR);
		}
	}
}
