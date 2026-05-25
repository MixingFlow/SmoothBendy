public class StatRequirement
{
	private StatAchievement stat;

	private int minimumRequirement;

	public StatAchievement Stat
	{
		get
		{
			return stat;
		}
		set
		{
			stat = value;
		}
	}

	public int MinimumRequirement
	{
		get
		{
			return minimumRequirement;
		}
		set
		{
			minimumRequirement = value;
		}
	}

	public StatRequirement(StatAchievement stat, int minimumRequirement)
	{
		this.stat = stat;
		this.minimumRequirement = minimumRequirement;
	}

	public StatRequirement()
	{
	}
}
