using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace I2.Loc;

public static class I2Utils
{
	public static string ReverseText(string source)
	{
		int length = source.Length;
		char[] array = new char[length];
		for (int i = 0; i < length; i++)
		{
			array[length - 1 - i] = source[i];
		}
		return new string(array);
	}

	public static string RemoveNonASCII(string text, bool allowCategory = false)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		return new string((from c in text.ToCharArray()
			select (!char.IsControl(c) && (c != '\\' || allowCategory)) ? c : ' ').ToArray());
	}

	public static string SplitLine(string line, int maxCharacters)
	{
		if (maxCharacters <= 0 || line.Length < maxCharacters)
		{
			return line;
		}
		char[] array = line.ToCharArray();
		bool flag = true;
		bool flag2 = false;
		int i = 0;
		int num = 0;
		for (; i < array.Length; i++)
		{
			if (flag)
			{
				num++;
				if (array[i] == '\n')
				{
					num = 0;
				}
				if (num >= maxCharacters && char.IsWhiteSpace(array[i]))
				{
					array[i] = '\n';
					flag = false;
					flag2 = false;
				}
			}
			else if (!char.IsWhiteSpace(array[i]))
			{
				flag = true;
				num = 0;
			}
			else if (array[i] != '\n')
			{
				array[i] = '\0';
			}
			else
			{
				if (!flag2)
				{
					array[i] = '\0';
				}
				flag2 = true;
			}
		}
		return new string(array.Where((char c) => c != '\0').ToArray());
	}

	public static bool FindNextTag(string line, int iStart, out int tagStart, out int tagEnd)
	{
		tagStart = -1;
		tagEnd = -1;
		int length = line.Length;
		tagStart = iStart;
		while (tagStart < length && line[tagStart] != '[' && line[tagStart] != '(' && line[tagStart] != '{')
		{
			tagStart++;
		}
		if (tagStart == length)
		{
			return false;
		}
		bool flag = false;
		for (tagEnd = tagStart + 1; tagEnd < length; tagEnd++)
		{
			char c = line[tagEnd];
			if (c == ']' || c == ')' || c == '}')
			{
				if (flag)
				{
					return FindNextTag(line, tagEnd + 1, out tagStart, out tagEnd);
				}
				return true;
			}
			if (c > 'ÿ')
			{
				flag = true;
			}
		}
		return false;
	}

	public static bool IsPlaying()
	{
		if (Application.isPlaying)
		{
			return true;
		}
		return false;
	}

	public static string GetPath(this Transform tr)
	{
		Transform parent = tr.parent;
		if ((Object)(object)tr == (Object)null)
		{
			return ((Object)tr).name;
		}
		return parent.GetPath() + "/" + ((Object)tr).name;
	}

	public static Transform FindObject(string objectPath)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return FindObject(SceneManager.GetActiveScene(), objectPath);
	}

	public static Transform FindObject(Scene scene, string objectPath)
	{
		GameObject[] rootGameObjects = ((Scene)(ref scene)).GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			Transform transform = rootGameObjects[i].transform;
			if (((Object)transform).name == objectPath)
			{
				return transform;
			}
			if (objectPath.StartsWith(((Object)transform).name + "/"))
			{
				return FindObject(transform, objectPath.Substring(((Object)transform).name.Length + 1));
			}
		}
		return null;
	}

	public static Transform FindObject(Transform root, string objectPath)
	{
		for (int i = 0; i < root.childCount; i++)
		{
			Transform child = root.GetChild(i);
			if (((Object)child).name == objectPath)
			{
				return child;
			}
			if (objectPath.StartsWith(((Object)child).name + "/"))
			{
				return FindObject(child, objectPath.Substring(((Object)child).name.Length + 1));
			}
		}
		return null;
	}

	public static H FindInParents<H>(Transform tr) where H : Component
	{
		if (!Object.op_Implicit((Object)(object)tr))
		{
			return (H)(object)null;
		}
		H component = ((Component)tr).GetComponent<H>();
		while (!Object.op_Implicit((Object)(object)component) && Object.op_Implicit((Object)(object)tr))
		{
			component = ((Component)tr).GetComponent<H>();
			tr = tr.parent;
		}
		return component;
	}

	public static string GetCaptureMatch(Match match)
	{
		for (int num = match.Groups.Count - 1; num >= 0; num--)
		{
			if (match.Groups[num].Success)
			{
				return match.Groups[num].ToString();
			}
		}
		return match.ToString();
	}

	public static void SendWebRequest(UnityWebRequest www)
	{
		www.SendWebRequest();
	}
}
