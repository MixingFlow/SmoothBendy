public class StatAchievement
{
	private StatName _StatName;

	private int? _CurrentValue;

	public int? CurrentValue
	{
		get
		{
			return _CurrentValue;
		}
		set
		{
			_CurrentValue = value;
		}
	}

	public StatName StatKey => _StatName;

	public StatAchievement(StatName statName)
	{
		_StatName = statName;
		AchievementsController.GetStat(_StatName, out _CurrentValue);
	}

	public void IncrementStat()
	{
		_CurrentValue++;
		SaveStat();
	}

	public void IncrementStatByValue(int incrementAmount)
	{
		_CurrentValue += incrementAmount;
		SaveStat();
	}

	public void SaveStat()
	{
		int? currentValue = _CurrentValue;
		if (!currentValue.HasValue)
		{
			_CurrentValue = 0;
		}
		AchievementsController.SetStat(_StatName, _CurrentValue.Value);
	}
}
