using System;
using System.Collections.Generic;

public static class ListsExtensionMethods
{
	public static void RemoveRange<T>(this List<T> list, IEnumerable<T> collection)
	{
		foreach (T item in collection)
		{
			list.RemoveAll((T x) => x.Equals(item));
		}
	}

	public static void RemoveRange<T>(this HashSet<T> hash, IEnumerable<T> collection)
	{
		foreach (T item in collection)
		{
			hash.Remove(item);
		}
	}

	public static void AddRange<T>(this HashSet<T> hash, IEnumerable<T> collection)
	{
		foreach (T item in collection)
		{
			hash.Add(item);
		}
	}

	public static void ForEach<T>(this T[] array, Action<T> action)
	{
		foreach (T obj in array)
		{
			action(obj);
		}
	}

	public static void ForEach<T>(this HashSet<T> array, Action<T> action)
	{
		foreach (T item in array)
		{
			action(item);
		}
	}
}
