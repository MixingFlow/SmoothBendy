using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StringLookupSettingsSO : ScriptableObject
{
	[SerializeField]
	public string Namespace = "UnityEngine";

	[SerializeField]
	public string StringLookupClass = "StringLookup";

	[SerializeField]
	public string ResourcesFilePath = "Assets/StringLookup/Resources/";

	[SerializeField]
	public string ScriptFilePath = "Assets/StringLookup/Scripts/";

	[SerializeField]
	public string LookupFilePath = "Lookup/";

	[SerializeField]
	public bool HasWhiteList;

	[SerializeField]
	public List<string> WhiteList = new List<string>();
}
