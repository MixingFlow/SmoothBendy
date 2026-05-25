using System;

public static class SeasonalCheck
{
	public static bool IsSeasonal()
	{
		bool result = false;
		if (GetCurrentSeason() != SeasonalType.None)
		{
			result = true;
		}
		return result;
	}

	public static bool IsSeasonal(out SeasonalType seasonalType)
	{
		bool result = false;
		seasonalType = GetCurrentSeason();
		if (seasonalType != SeasonalType.None)
		{
			result = true;
		}
		return result;
	}

	public static SeasonalType GetCurrentSeason()
	{
		SeasonalType result = SeasonalType.None;
		DateTime now = DateTime.Now;
		if (GameManager.Instance.PlayerSettings.forceHalloween)
		{
			result = SeasonalType.InkDemonsEve;
		}
		else if (now.Month == 2 && now.Day > 10 && now.Day < 16)
		{
			result = (SeasonalType)1;
		}
		else if (now.Month == 10 && now.Day >= 23)
		{
			result = SeasonalType.InkDemonsEve;
		}
		else if (now.Month == 11 && now.Day > 19 && now.Day < 30)
		{
			result = (SeasonalType)3;
		}
		else if (now.Month == 12 && now.Day > 23 && now.Day < 27)
		{
			result = (SeasonalType)4;
		}
		return result;
	}
}
