using System;
using UnityEngine;

[Serializable]
public class VersionSettingsSO : ScriptableObject
{
	[SerializeField]
	public int Major = 1;

	[SerializeField]
	public int Minor;

	[SerializeField]
	public int Patch;

	[SerializeField]
	public int Hotfix;
}
