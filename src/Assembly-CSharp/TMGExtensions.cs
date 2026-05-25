using System;
using System.Collections.Generic;
using UnityEngine;

public static class TMGExtensions
{
	public static int ParseInt(this string value)
	{
		int num = 0;
		foreach (char c in value)
		{
			num = 10 * num + (int)char.GetNumericValue(c);
		}
		return num;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		Random random = new Random();
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = random.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static T RandomItem<T>(this IList<T> list)
	{
		if (list.Count == 0)
		{
			throw new IndexOutOfRangeException("Cannot select a random item from an empty list");
		}
		return list[Random.Range(0, list.Count)];
	}

	public static T RemoveRandom<T>(this IList<T> list)
	{
		if (list.Count == 0)
		{
			throw new IndexOutOfRangeException("Cannot remove a random item from an empty list");
		}
		int index = Random.Range(0, list.Count);
		T result = list[index];
		list.RemoveAt(index);
		return result;
	}

	public static void Send(this EventHandler handler, object sender)
	{
		handler?.Invoke(sender, EventArgs.Empty);
	}
}
