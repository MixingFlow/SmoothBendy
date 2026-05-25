using System;
using System.Collections.Generic;
using System.Reflection;

namespace S13Audio;

public static class S13EventList
{
	private static List<string> eventStrings = new List<string>();

	private static List<string> PopulateList()
	{
		Type typeFromHandle = typeof(S13AudioEvents);
		MethodInfo[] methods = typeFromHandle.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
		eventStrings = new List<string>(methods.Length);
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.Name != "Start" && methodInfo.IsPrivate)
			{
				eventStrings.Add(methodInfo.Name);
			}
		}
		return eventStrings;
	}

	public static List<string> GetList()
	{
		if (eventStrings == null || eventStrings.Count < 1)
		{
			PopulateList();
		}
		return eventStrings;
	}

	public static void Refresh()
	{
		eventStrings.Clear();
	}
}
