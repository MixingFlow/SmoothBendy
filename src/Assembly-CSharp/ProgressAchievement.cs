public class ProgressAchievement : Achievement
{
	public StatRequirement StatRequirement { get; private set; }

	public ProgressAchievement(int id, AchievementName name, string displayName, StatRequirement statRequirement)
		: base(id, name, displayName)
	{
		StatRequirement = statRequirement;
	}

	public ProgressAchievement()
	{
	}
}
