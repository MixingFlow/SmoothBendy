public class Achievement
{
	public int ID;

	public AchievementName AchievementName { get; private set; }

	public string DisplayName { get; private set; }

	public Achievement(int id, AchievementName name, string displayName)
	{
		ID = id;
		AchievementName = name;
		DisplayName = displayName;
	}

	public Achievement()
	{
	}
}
