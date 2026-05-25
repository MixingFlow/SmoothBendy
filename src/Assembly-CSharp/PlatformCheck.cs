using System.IO;
using UnityEngine;

public class PlatformCheck : MonoBehaviour
{
	private void Start()
	{
		if (IsRunningOnSteam())
		{
			Debug.Log((object)"Running on Steam");
		}
		else if (IsRunningOnGOG())
		{
			Debug.Log((object)"Running on GOG");
		}
		else
		{
			Debug.Log((object)"Platform not detected (Steam or GOG)");
		}
	}

	public bool IsRunningOnSteam()
	{
		return Directory.GetFiles(Path.Combine(Application.dataPath, ".."), "*steam*", SearchOption.TopDirectoryOnly).Length != 0;
	}

	public bool IsRunningOnGOG()
	{
		return Directory.GetFiles(Path.Combine(Application.dataPath, ".."), "*gog*", SearchOption.TopDirectoryOnly).Length != 0;
	}
}
