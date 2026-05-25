using UnityEngine;

namespace S13Audio;

public class S13Delay
{
	private float _delayFeedback;

	private float _dryMix;

	private float _wetMix;

	private int _delaySampleLength;

	private int _numChannels;

	private S13RingBuffer<float> _delayBuffer;

	public float Feedback
	{
		get
		{
			return _delayFeedback;
		}
		set
		{
			_delayFeedback = value;
		}
	}

	public float DryMix
	{
		get
		{
			return _dryMix;
		}
		set
		{
			_dryMix = value;
		}
	}

	public float WetMix
	{
		get
		{
			return _wetMix;
		}
		set
		{
			_wetMix = value;
		}
	}

	public S13Delay(float delayTime, float delayFeedback, float sampleRate, int numChannels)
	{
		_delayFeedback = delayFeedback;
		_numChannels = numChannels;
		_delaySampleLength = Mathf.RoundToInt(delayTime * sampleRate);
		_delayBuffer = new S13RingBuffer<float>(_delaySampleLength * _numChannels);
		DryMix = (WetMix = 1f);
		Reset();
	}

	public S13Delay(int delaySamples, float delayFeedback, int numChannels)
	{
		_delayFeedback = delayFeedback;
		_numChannels = numChannels;
		_delaySampleLength = delaySamples;
		_delayBuffer = new S13RingBuffer<float>(_delaySampleLength * _numChannels);
		DryMix = (WetMix = 1f);
		Reset();
	}

	public void SetDelayTime(float delayTime, float sampleRate)
	{
		_delaySampleLength = Mathf.RoundToInt(delayTime * sampleRate);
		_delayBuffer.Length = _delaySampleLength * _numChannels;
	}

	public void Process(float[] audioData, int numChannels)
	{
		if (_delayBuffer.Length == 0)
		{
			return;
		}
		for (int i = 0; i < audioData.Length; i += numChannels)
		{
			for (int j = 0; j < numChannels; j++)
			{
				float num = _delayBuffer.Read();
				_delayBuffer.Write(audioData[i + j] + num * _delayFeedback);
				audioData[i + j] = audioData[i + j] * DryMix + num * WetMix;
			}
		}
	}

	public void Reset()
	{
		_delayBuffer.Clear(0f);
	}
}
