using UnityEngine;

namespace S13Audio;

public class S13EnvelopeDetector
{
	protected float _attackTime;

	protected float _releaseTime;

	protected float _attackGain;

	protected float _releaseGain;

	protected float _sampleRate;

	protected float _envelopeSample;

	protected EnvDetectMode _detectMode;

	protected IEnvelopeDetection _detector;

	public float AttackTime
	{
		get
		{
			return _attackTime;
		}
		set
		{
			_attackTime = value;
			_attackGain = CalculateTimeConstant(_attackTime);
		}
	}

	public float ReleaseTime
	{
		get
		{
			return _releaseTime;
		}
		set
		{
			_releaseTime = value;
			_releaseGain = CalculateTimeConstant(_releaseTime);
		}
	}

	public EnvDetectMode DetectMode
	{
		get
		{
			return _detectMode;
		}
		set
		{
			switch (_detectMode)
			{
			case EnvDetectMode.Peak:
				_detector = new EnvDetectPeak();
				break;
			case EnvDetectMode.Rms:
				_detector = new EnvDetectRms();
				break;
			}
		}
	}

	public S13EnvelopeDetector(float attackTime, float releaseTime, EnvDetectMode detectMode, float sampleRate = 44100f)
	{
		_sampleRate = sampleRate;
		AttackTime = attackTime;
		ReleaseTime = releaseTime;
		DetectMode = detectMode;
		Reset();
	}

	public void GetEnvelope(float[] audioData, out float[] envelope)
	{
		envelope = new float[audioData.Length];
		_detector.Buffer = audioData;
		for (int i = 0; i < audioData.Length; i++)
		{
			float num = _detector[i];
			if (_envelopeSample < num)
			{
				_envelopeSample = num + _attackGain * (_envelopeSample - num);
			}
			else
			{
				_envelopeSample = num + _releaseGain * (_envelopeSample - num);
			}
			envelope[i] = _envelopeSample;
		}
	}

	public void Reset()
	{
		_envelopeSample = 0f;
		_detector.Reset();
	}

	private float CalculateTimeConstant(float time)
	{
		return Mathf.Exp(-1f / (time * _sampleRate));
	}
}
