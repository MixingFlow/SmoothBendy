using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class AchievementsController : MonoBehaviour
{
	private static IAchievements _CurrentAchievementsController;

	private static List<Achievement> Achievements;

	private static Dictionary<AchievementName, string> _AchievementNames;

	private static List<StatAchievement> _StatAchievements;

	private static AchievementMapper[] achievementMappers;

	public static Dictionary<AchievementName, string> AchievementNames => _AchievementNames;

	public static ReadOnlyCollection<StatAchievement> Stats => _StatAchievements.AsReadOnly();

	public static void Initialize()
	{
		AchievementIDMapping[] mapping = new AchievementIDMapping[0];
		_CurrentAchievementsController = new SteamAchievements();
		if (_CurrentAchievementsController != null)
		{
			_CurrentAchievementsController.Initialize(mapping);
		}
		InitializeStatAchievements();
		InitializeAchievementNames();
	}

	private void Awake()
	{
		achievementMappers = ((Component)this).gameObject.GetComponentsInChildren<AchievementMapper>();
	}

	private void Start()
	{
	}

	private static void InitializeAchievementNames()
	{
		_AchievementNames = new Dictionary<AchievementName, string>();
		Achievements = new List<Achievement>();
	}

	public static void CheckAchievementsAgainstStatRequirements(StatName statUpdated)
	{
		StatAchievement statAchievementByStatName = GetStatAchievementByStatName(statUpdated);
		foreach (ProgressAchievement item in Achievements.OfType<ProgressAchievement>())
		{
			if (item.StatRequirement.Stat == statAchievementByStatName)
			{
				int? currentValue = statAchievementByStatName.CurrentValue;
				if (currentValue.HasValue && currentValue.GetValueOrDefault() >= item.StatRequirement.MinimumRequirement)
				{
					Debug.Log((object)("*AchievementController* Awarding achievement " + item.DisplayName));
					_CurrentAchievementsController.SetAchievement(item.AchievementName);
				}
			}
		}
		DebugOutAllStatProgress();
	}

	private static void DebugOutAllStatProgress()
	{
		foreach (ProgressAchievement item in Achievements.OfType<ProgressAchievement>())
		{
			StatAchievement statAchievementByStatName = GetStatAchievementByStatName(item.StatRequirement.Stat.StatKey);
			Debug.Log((object)("*AchievementController* Checking achievement " + item.DisplayName + " with stat requirement " + item.StatRequirement.Stat.StatKey.ToString() + " and minimum req " + item.StatRequirement.MinimumRequirement.ToString() + " current amount is " + statAchievementByStatName.CurrentValue));
		}
	}

	private static StatAchievement GetStatAchievementByStatName(StatName statName)
	{
		return _StatAchievements.Find((StatAchievement statAchievement) => statAchievement.StatKey == statName);
	}

	private static void InitializeStatAchievements()
	{
		Debug.Log((object)"Initializing stat achievements.");
		_StatAchievements = new List<StatAchievement>();
		foreach (StatAchievement statAchievement in _StatAchievements)
		{
		}
	}

	public static bool GetAchievement(AchievementName key)
	{
		if (!CanUseAchievements())
		{
			return false;
		}
		return _CurrentAchievementsController.GetAchievement(key);
	}

	public static void SetAchievement(AchievementName key)
	{
		if (CanUseAchievements())
		{
			_CurrentAchievementsController.SetAchievement(key);
			_CurrentAchievementsController.StoreStats();
		}
	}

	public static void ClearAchievement(AchievementName key)
	{
		if (CanUseAchievements())
		{
			_CurrentAchievementsController.ClearAchievement(key);
		}
	}

	public static void GetStat(StatName name, out int? data)
	{
		if (!CanUseAchievements())
		{
			data = null;
		}
		else
		{
			_CurrentAchievementsController.GetStat(name, out data);
		}
	}

	public static void GetStat(StatName name, out float? data)
	{
		if (!CanUseAchievements())
		{
			data = null;
		}
		else
		{
			_CurrentAchievementsController.GetStat(name, out data);
		}
	}

	public static void SetStat(StatName name, int data)
	{
		if (!CanUseAchievements())
		{
			Debug.Log((object)"AchievementController - cannot set achievement");
			return;
		}
		StatAchievement statAchievementByStatName = GetStatAchievementByStatName(name);
		if (statAchievementByStatName != null)
		{
			Debug.Log((object)string.Concat("Got stat ", name, ", now setting it to ", data));
			statAchievementByStatName.CurrentValue = data;
		}
		_CurrentAchievementsController.SetStat(name, data);
	}

	public static void SetStat(StatName name, float data)
	{
		if (CanUseAchievements())
		{
			_CurrentAchievementsController.SetStat(name, data);
		}
	}

	public static void IncrementStatAchievement(StatName name)
	{
		if (CanUseAchievements())
		{
			Debug.Log((object)string.Concat("*Can save stat ", name, ". Now trying to find it in collection and increment.*"));
			StatAchievement statAchievement = _StatAchievements.Find((StatAchievement achievement) => achievement.StatKey == name);
			statAchievement.IncrementStat();
		}
	}

	public static void IncrementStatByValue(StatName name, int incrementAmount)
	{
		if (CanUseAchievements())
		{
			StatAchievement statAchievement = _StatAchievements.Find((StatAchievement achievement) => achievement.StatKey == name);
			statAchievement.IncrementStatByValue(incrementAmount);
		}
	}

	public static void SaveStatAchievement(StatName name)
	{
		if (CanUseAchievements())
		{
			StatAchievement statAchievement = _StatAchievements.Find((StatAchievement achievement) => achievement.StatKey == name);
			statAchievement.SaveStat();
		}
	}

	public static void IndicateProgress(AchievementName name, uint currentProgress, uint maxProgress)
	{
		if (CanUseAchievements())
		{
			_CurrentAchievementsController.IndicateProgress(name, currentProgress, maxProgress);
		}
	}

	public static void StoreStats()
	{
		if (CanUseAchievements())
		{
			_CurrentAchievementsController.StoreStats();
		}
	}

	public static void ResetStats()
	{
		if (CanUseAchievements())
		{
			_CurrentAchievementsController.ResetStats();
			_CurrentAchievementsController.StoreStats();
		}
	}

	public static void ResetStatsAndAchievements()
	{
		if (CanUseAchievements())
		{
			_CurrentAchievementsController.ResetStatsAndAchievements();
			_CurrentAchievementsController.StoreStats();
		}
	}

	public static void SetAchievementUser(int id)
	{
		_CurrentAchievementsController.SetAchievementUser(id);
	}

	private static bool CanUseAchievements()
	{
		return false;
	}
}
