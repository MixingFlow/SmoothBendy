using System;
using System.Collections;
using UnityEngine;

namespace S13Audio;

public static class S13AudioUtil
{
	public delegate void FadeInOutDelegate();

	public delegate void WaitForDurationDelegate();

	private static double PITCH_RATIO = 1.0594630943592953;

	private static double PITCH_RATIO_LOG10 = 0.025085832971998432;

	public static IEnumerator FadeIn(AudioSource audioSource, float fadeInTime, float targetVolume, bool ignoreTimeScale, FadeInOutDelegate completionHandler = null)
	{
		float timeElapsed = 0f;
		float lastTime = Time.realtimeSinceStartup;
		while (timeElapsed < fadeInTime)
		{
			float t = timeElapsed / fadeInTime;
			audioSource.volume = Mathf.Lerp(0f, targetVolume, t);
			if (ignoreTimeScale)
			{
				timeElapsed += Time.realtimeSinceStartup - lastTime;
				lastTime = Time.realtimeSinceStartup;
			}
			else
			{
				timeElapsed += Time.deltaTime;
			}
			yield return null;
		}
		audioSource.volume = targetVolume;
		completionHandler?.Invoke();
	}

	public static IEnumerator FadeOut(AudioSource audioSource, float fadeOutTime, float currentVolume, bool ignoreTimeScale, FadeInOutDelegate completionHandler = null)
	{
		float timeElapsed = 0f;
		float lastTime = Time.realtimeSinceStartup;
		while (timeElapsed < fadeOutTime)
		{
			float t = timeElapsed / fadeOutTime;
			audioSource.volume = Mathf.Lerp(currentVolume, 0f, t);
			if (ignoreTimeScale)
			{
				timeElapsed += Time.realtimeSinceStartup - lastTime;
				lastTime = Time.realtimeSinceStartup;
			}
			else
			{
				timeElapsed += Time.deltaTime;
			}
			yield return null;
		}
		audioSource.volume = 0f;
		completionHandler?.Invoke();
	}

	public static IEnumerator WaitForDuration(float timeDuration, bool ignoreTimeScale, WaitForDurationDelegate completionHandler)
	{
		if (completionHandler != null)
		{
			if (ignoreTimeScale)
			{
				IEnumerator waitTime = RealTimeWaitForSeconds(timeDuration);
				while (waitTime.MoveNext())
				{
					yield return waitTime.Current;
				}
			}
			else
			{
				yield return (object)new WaitForSeconds(timeDuration);
			}
			completionHandler();
		}
		else
		{
			Debug.LogError((object)"WaitForDuration: Delegate cannot be null.");
		}
	}

	public static IEnumerator RealTimeWaitForSeconds(float duration)
	{
		float finishTime = Time.realtimeSinceStartup + duration;
		while (Time.realtimeSinceStartup < finishTime)
		{
			yield return null;
		}
	}

	public static float SemitoneToPitch(float semitone)
	{
		return (float)Math.Pow(PITCH_RATIO, semitone);
	}

	public static float PitchToSemitone(float pitch)
	{
		return (float)(Math.Log10(pitch) / PITCH_RATIO_LOG10);
	}

	public static float Lin2dB(float lin)
	{
		if (lin > 0f)
		{
			return 20f * Mathf.Log10(lin);
		}
		return -60f;
	}

	public static float dB2Lin(float dB)
	{
		return Mathf.Pow(10f, dB / 20f);
	}

	public static void DeinterleaveBuffer(float[] source, out float[][] output, int sourceChannels)
	{
		int num = source.Length / sourceChannels;
		output = new float[sourceChannels][];
		for (int i = 0; i < sourceChannels; i++)
		{
			output[i] = new float[num];
			for (int j = 0; j < num; j++)
			{
				output[i][j] = source[j * sourceChannels + i];
			}
		}
	}
}
