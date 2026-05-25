using System;
using System.IO;
using UnityEngine;

namespace S13Audio;

public class S13Compressor : MonoBehaviour
{
	private delegate float SlopeCalculation(float ratio);

	[AudioSlider("Threshold (dB)", -60f, 0f)]
	public float threshold;

	[AudioSlider("Ratio (x:1)", 1f, 20f)]
	public float ratio = 1f;

	[AudioSlider("Knee", 0f, 1f)]
	public float knee = 0.2f;

	[AudioSlider("Pre-gain (dB)", -12f, 24f)]
	public float preGain;

	[AudioSlider("Post-gain (dB)", -12f, 24f)]
	public float postGain;

	[AudioSlider("Attack time (ms)", 0f, 200f)]
	public float attackTime = 10f;

	[AudioSlider("Release time (ms)", 10f, 3000f)]
	public float releaseTime = 50f;

	[AudioSlider("Lookahead time (ms)", 0f, 200f)]
	public float lookaheadTime;

	public ProcessType processType;

	public EnvDetectMode detectMode;

	private S13EnvelopeDetector[] _envelopeDetector;

	private S13Delay _lookaheadDelay;

	private SlopeCalculation _slopeFunc;

	private float _sampleRate;

	private void Awake()
	{
		if (processType == ProcessType.Compressor)
		{
			_slopeFunc = CompressorSlope;
		}
		else if (processType == ProcessType.Limiter)
		{
			_slopeFunc = LimiterSlope;
		}
		attackTime /= 1000f;
		releaseTime /= 1000f;
		_sampleRate = AudioSettings.outputSampleRate;
		_envelopeDetector = new S13EnvelopeDetector[2];
		_envelopeDetector[0] = new S13EnvelopeDetector(attackTime, releaseTime, detectMode, _sampleRate);
		_envelopeDetector[1] = new S13EnvelopeDetector(attackTime, releaseTime, detectMode, _sampleRate);
		_lookaheadDelay = new S13Delay(0.2f, 0f, _sampleRate, 2);
		_lookaheadDelay.SetDelayTime(lookaheadTime, _sampleRate);
		_lookaheadDelay.DryMix = 0f;
	}

	private void Start()
	{
	}

	private void OnAudioFilterRead(float[] data, int numChannels)
	{
		float num = S13AudioUtil.dB2Lin(postGain);
		float num2 = threshold * knee * -1f;
		float num3 = threshold - num2 / 2f;
		float num4 = threshold + num2 / 2f;
		if (preGain != 0f)
		{
			float num5 = S13AudioUtil.dB2Lin(preGain);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] *= num5;
			}
		}
		float[][] array = new float[numChannels][];
		switch (numChannels)
		{
		case 2:
		{
			S13AudioUtil.DeinterleaveBuffer(data, out var output, numChannels);
			_envelopeDetector[0].GetEnvelope(output[0], out array[0]);
			_envelopeDetector[1].GetEnvelope(output[1], out array[1]);
			for (int j = 0; j < array[0].Length; j++)
			{
				array[0][j] = Mathf.Max(array[0][j], array[1][j]);
			}
			break;
		}
		case 1:
			_envelopeDetector[0].GetEnvelope(data, out array[0]);
			break;
		default:
			Debug.LogError((object)(((Object)this).name + ": Only mono or stereo audio source supported."));
			Debug.Break();
			break;
		}
		if (lookaheadTime > 0f)
		{
			_lookaheadDelay.SetDelayTime(lookaheadTime, _sampleRate);
			_lookaheadDelay.Process(data, numChannels);
		}
		int num6 = 0;
		int num7 = 0;
		while (num6 < data.Length)
		{
			float num8 = S13AudioUtil.Lin2dB(array[0][num7]);
			float num9 = _slopeFunc(ratio);
			float dB;
			if (num2 > 0f && num8 > num3 && num8 < num4)
			{
				num9 *= (num8 - num3) / num2 * 0.5f;
				dB = num9 * (num3 - num8);
			}
			else
			{
				dB = num9 * (threshold - num8);
				dB = Mathf.Min(0f, dB);
			}
			dB = S13AudioUtil.dB2Lin(dB);
			for (int k = 0; k < numChannels; k++)
			{
				data[num6 + k] *= dB * num;
			}
			num6 += numChannels;
			num7++;
		}
	}

	private float CompressorSlope(float ratio)
	{
		return 1f - 1f / ratio;
	}

	private float LimiterSlope(float ratio)
	{
		return 1f;
	}

	private void Plot()
	{
		string path = Environment.CurrentDirectory + "/Data/compressor_plot.txt";
		StreamWriter streamWriter = File.CreateText(path);
		float num = threshold * knee * -1f;
		float num2 = threshold - num / 2f;
		float num3 = threshold + num / 2f;
		float num4 = 0.17578125f;
		float num5 = -90f;
		for (int i = 0; i < 512; i++)
		{
			float num6 = _slopeFunc(ratio);
			float dB;
			if (num > 0f && num5 > num2 && num5 < num3)
			{
				num6 = num6 * ((num5 - num2) / num) * 0.5f;
				dB = num6 * (num2 - num5);
			}
			else
			{
				dB = num6 * (threshold - num5);
				dB = Mathf.Min(0f, dB);
			}
			dB = S13AudioUtil.dB2Lin(dB);
			float lin = S13AudioUtil.dB2Lin(num5) * dB;
			streamWriter.WriteLine("{0}\t{1}", num5, S13AudioUtil.Lin2dB(lin));
			num5 += num4;
		}
		streamWriter.Close();
		Debug.Log((object)"compressor plotted");
	}
}
