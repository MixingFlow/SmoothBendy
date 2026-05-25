using Steamworks;
using UnityEngine;

public class SteamAchievements : IAchievements
{
	private bool _IsInitialized;

	public void SetAchievementUser(int userID)
	{
	}

	public void Initialize(AchievementIDMapping[] mapping = null)
	{
		_IsInitialized = true;
	}

	public void SetStat(StatName name, int data)
	{
	}

	public void SetStat(StatName name, float data)
	{
	}

	public bool GetAchievement(AchievementName name)
	{
		SteamUserStats.RequestCurrentStats();
		SteamUserStats.GetAchievement(name.ToString(), out var pbAchieved);
		object[] obj = new object[10] { "Game ID: ", 622650, "\nSteam Game ID: ", null, null, null, null, null, null, null };
		AppId_t invalid = AppId_t.Invalid;
		obj[3] = invalid.m_AppId;
		obj[4] = "\nAchievement: ";
		obj[5] = name.ToString();
		obj[6] = "\nSteam Has Achievement: ";
		obj[7] = SteamUserStats.GetAchievement(name.ToString(), out pbAchieved);
		obj[8] = "\nIs Achieved: ";
		obj[9] = pbAchieved;
		Debug.Log((object)string.Concat(obj));
		return pbAchieved;
	}

	public void SetAchievement(AchievementName name)
	{
		if (!GetAchievement(name))
		{
			Debug.Log((object)("Achievement Found: " + name));
			SteamUserStats.SetAchievement(name.ToString());
			SteamUserStats.StoreStats();
		}
	}

	public void ClearAchievement(AchievementName name)
	{
		SteamUserStats.ClearAchievement(name.ToString());
	}

	public void GetStat(StatName name, out int? data)
	{
		data = 0;
	}

	public void GetStat(StatName name, out float? data)
	{
		data = 0f;
	}

	public void IndicateProgress(AchievementName name, uint currentProgress, uint maxProgress)
	{
	}

	public void ResetStats()
	{
	}

	public void ResetStatsAndAchievements()
	{
	}

	public void StoreStats()
	{
	}

	public bool IsConnected()
	{
		return Object.op_Implicit((Object)(object)GameManager.Instance.SteamManager) && GameManager.Instance.SteamManager.IsInitialized && _IsInitialized && SteamUser.BLoggedOn();
	}
}
