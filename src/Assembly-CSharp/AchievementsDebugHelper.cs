using System.Collections.Generic;
using UnityEngine;

public class AchievementsDebugHelper : MonoBehaviour
{
	[SerializeField]
	private bool _ShowDebugInfo = true;

	private bool _IsShowingAchievements;

	private bool _IsShowingIndividualAchievement;

	private bool _IsShowingStats;

	private bool _IsShowingIndividualStat;

	private string _AchievementStatus = string.Empty;

	private int? _CurrentStatValue;

	private int _DefaultXOffset = 150;

	private int _ButtonHeight = 30;

	private KeyValuePair<AchievementName, string> _CurrentAchievementShowing;

	private StatName _CurrentStatName;

	private void OnGUI()
	{
		if (_ShowDebugInfo)
		{
		}
	}

	private void DrawAchievements()
	{
		if (_IsShowingAchievements)
		{
			if (!_IsShowingIndividualAchievement)
			{
				DrawAchievementButtons();
			}
			else
			{
				DrawIndividualAchievement();
			}
		}
	}

	private void DrawAchievementButtons()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		int num = _DefaultXOffset;
		int num2 = 10;
		foreach (KeyValuePair<AchievementName, string> achievementName in AchievementsController.AchievementNames)
		{
			if (GUI.Button(new Rect((float)num, (float)num2, 130f, (float)_ButtonHeight), achievementName.Value))
			{
				_IsShowingIndividualAchievement = true;
				_CurrentAchievementShowing = achievementName;
			}
			num += _DefaultXOffset;
			if (num > 600)
			{
				num = _DefaultXOffset;
				num2 += 40;
			}
		}
	}

	private void DrawIndividualAchievement()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(new Rect((float)_DefaultXOffset, 10f, 130f, (float)_ButtonHeight), _CurrentAchievementShowing.Value))
		{
			_IsShowingIndividualAchievement = false;
			_AchievementStatus = string.Empty;
		}
		if (GUI.Button(new Rect((float)_DefaultXOffset, 60f, 200f, (float)_ButtonHeight), "Unlock Achievement"))
		{
			AchievementsController.SetAchievement(_CurrentAchievementShowing.Key);
			_AchievementStatus = "Achievement Status: Successfully got an achievement!";
		}
		if (GUI.Button(new Rect((float)_DefaultXOffset, 100f, 200f, (float)_ButtonHeight), "Clear Achievement"))
		{
			AchievementsController.ClearAchievement(_CurrentAchievementShowing.Key);
			_AchievementStatus = "Achievement Status: Successfully cleared an achievement!";
		}
		if (GUI.Button(new Rect((float)_DefaultXOffset, 140f, 200f, (float)_ButtonHeight), "Check Achievement"))
		{
			if (AchievementsController.GetAchievement(_CurrentAchievementShowing.Key))
			{
				_AchievementStatus = "Achievement Status: achievement is unlocked!";
			}
			else
			{
				_AchievementStatus = "Achievement Status: achievement is locked!";
			}
		}
		GUI.Label(new Rect((float)_DefaultXOffset, 170f, 500f, 20f), _AchievementStatus);
	}

	private void DrawStats()
	{
		if (_IsShowingStats)
		{
			if (_IsShowingIndividualStat)
			{
				DrawIndividualStat();
			}
			else
			{
				DrawStatsButtons();
			}
		}
	}

	private void DrawStatsButtons()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		int num = _DefaultXOffset;
		int num2 = 10;
		foreach (StatAchievement stat in AchievementsController.Stats)
		{
			if (GUI.Button(new Rect((float)num, (float)num2, 130f, (float)_ButtonHeight), stat.StatKey.ToString()))
			{
				_IsShowingIndividualStat = true;
				_CurrentStatName = stat.StatKey;
				AchievementsController.GetStat(_CurrentStatName, out _CurrentStatValue);
			}
			num += _DefaultXOffset;
			if (num > 600)
			{
				num = _DefaultXOffset;
				num2 += 40;
			}
		}
	}

	private void DrawIndividualStat()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(new Rect((float)_DefaultXOffset, 10f, 130f, (float)_ButtonHeight), _CurrentStatName.ToString()))
		{
			_IsShowingIndividualStat = false;
		}
		if (_CurrentStatValue.HasValue)
		{
			GUI.Label(new Rect((float)(_DefaultXOffset * 2), 10f, 250f, (float)(_ButtonHeight * 2)), _CurrentStatValue.ToString());
		}
		if (GUI.Button(new Rect((float)_DefaultXOffset, 60f, 60f, (float)_ButtonHeight), "+1"))
		{
			_CurrentStatValue++;
			AchievementsController.IncrementStatByValue(_CurrentStatName, 1);
		}
		if (GUI.Button(new Rect((float)_DefaultXOffset, 110f, 60f, (float)_ButtonHeight), "+10"))
		{
			_CurrentStatValue += 10;
			AchievementsController.IncrementStatByValue(_CurrentStatName, 10);
		}
		if (GUI.Button(new Rect((float)_DefaultXOffset, 160f, 60f, (float)_ButtonHeight), "+100"))
		{
			_CurrentStatValue += 100;
			AchievementsController.IncrementStatByValue(_CurrentStatName, 100);
		}
		if (GUI.Button(new Rect((float)_DefaultXOffset, 210f, 60f, (float)_ButtonHeight), "+1,000"))
		{
			_CurrentStatValue += 1000;
			AchievementsController.IncrementStatByValue(_CurrentStatName, 1000);
		}
	}

	private void DrawHideShowAchievementButton()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(new Rect(10f, 10f, 130f, (float)_ButtonHeight), (!_IsShowingAchievements) ? "Show Achievements" : "Hide Achievements"))
		{
			_IsShowingAchievements = !_IsShowingAchievements;
			if (!_IsShowingAchievements)
			{
				_IsShowingIndividualAchievement = false;
			}
			_IsShowingIndividualStat = false;
			_IsShowingStats = false;
		}
	}

	private void DrawShowHideStatButton()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(new Rect(10f, 50f, 130f, (float)_ButtonHeight), (!_IsShowingStats) ? "Show Stats" : "Hide Stats"))
		{
			_IsShowingStats = !_IsShowingStats;
			if (!_IsShowingStats)
			{
				_IsShowingIndividualAchievement = false;
			}
			_IsShowingAchievements = false;
			_IsShowingIndividualStat = false;
		}
	}

	private void DrawResetButtons()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(new Rect(10f, 90f, 130f, (float)_ButtonHeight), "Reset Stats"))
		{
			AchievementsController.ResetStats();
		}
		if (GUI.Button(new Rect(10f, 130f, 130f, (float)_ButtonHeight), "Reset Achievements"))
		{
			foreach (KeyValuePair<AchievementName, string> achievementName in AchievementsController.AchievementNames)
			{
				AchievementsController.ClearAchievement(achievementName.Key);
			}
			AchievementsController.StoreStats();
		}
		if (GUI.Button(new Rect(10f, 170f, 130f, (float)_ButtonHeight), "Clear All Level Times"))
		{
		}
		if (GUI.Button(new Rect(10f, 200f, 130f, (float)_ButtonHeight), "Clear All PlayerPrefs"))
		{
			PlayerPrefs.DeleteAll();
		}
	}
}
