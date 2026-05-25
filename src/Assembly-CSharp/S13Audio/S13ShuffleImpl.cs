using System;
using UnityEngine;

namespace S13Audio;

public class S13ShuffleImpl
{
	private int[] _randomOrder;

	private int _index;

	private bool _avoidLastPlayed = true;

	public S13ShuffleImpl(S13Range range, bool shuffleOnInit = true, bool avoidLastPlayed = true)
	{
		_avoidLastPlayed = avoidLastPlayed;
		if (range.Length == 0)
		{
			Debug.LogWarning((object)"ShuffleImpl: Invalid range length.");
		}
		_randomOrder = new int[range.Length];
		for (int i = 0; i < range.Length; i++)
		{
			_randomOrder[i] = range.Start + i;
		}
		_index = 0;
		if (shuffleOnInit)
		{
			Shuffle();
		}
	}

	public void Shuffle()
	{
		float[] array = new float[_randomOrder.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Random.value;
		}
		int num = _randomOrder[_randomOrder.Length - 1];
		Array.Sort(array, _randomOrder);
		if (_avoidLastPlayed && num.Equals(_randomOrder[0]))
		{
			Array.Reverse(_randomOrder);
		}
	}

	public int Next(bool reshuffle = true)
	{
		if (_randomOrder.Length == 0)
		{
			return 0;
		}
		int result = _randomOrder[_index];
		if (++_index >= _randomOrder.Length)
		{
			_index = 0;
			if (reshuffle)
			{
				Shuffle();
			}
		}
		return result;
	}
}
