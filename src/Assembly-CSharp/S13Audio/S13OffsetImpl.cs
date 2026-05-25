using UnityEngine;

namespace S13Audio;

public class S13OffsetImpl
{
	private int _offsetSamples;

	private float _lastRandom = -1f;

	public S13OffsetImpl(int sampleOffset)
	{
		sampleOffset = ((sampleOffset >= 0) ? sampleOffset : 0);
		_offsetSamples = sampleOffset;
	}

	public void SetPlayPosition(AudioSource audioSource, bool randomOffset = false)
	{
		int i = _offsetSamples;
		if (randomOffset)
		{
			float num = Random.value;
			if (Mathf.Abs(num - _lastRandom) < 0.1f)
			{
				num += _lastRandom + Random.Range(-0.5f, 0.5f);
			}
			i += (int)((float)audioSource.clip.samples * num);
			_lastRandom = num;
		}
		for (; i < 0; i += audioSource.clip.samples)
		{
		}
		while (i > audioSource.clip.samples)
		{
			i -= audioSource.clip.samples;
		}
		audioSource.timeSamples = i;
	}
}
