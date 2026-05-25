public class InGameAchievementTracker
{
	private int _CurrentValue;

	private int _UnlockValue;

	private AchievementName _Achievement;

	public InGameAchievementTracker(AchievementName achievement, int unlockValue)
	{
		_CurrentValue = 0;
		_UnlockValue = unlockValue;
		_Achievement = achievement;
	}

	public void IncrementValue()
	{
		_CurrentValue++;
		if (_CurrentValue >= _UnlockValue)
		{
			AchievementsController.SetAchievement(_Achievement);
		}
	}

	public void Reset()
	{
		_CurrentValue = 0;
	}
}
