using UnityEngine;

namespace S13Audio;

public class S13ResumeImpl
{
	private int[] _positions;

	public S13ResumeImpl(int count)
	{
		_positions = new int[count];
		for (int i = 0; i < _positions.Length; i++)
		{
			_positions[i] = 0;
		}
	}

	public void ResumePosition(AudioSource audioSource, int soundIndex)
	{
		audioSource.timeSamples = _positions[soundIndex];
	}

	public void SavePosition(AudioSource audioSource, int soundIndex)
	{
		_positions[soundIndex] = audioSource.timeSamples;
	}
}
