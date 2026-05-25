using System.Collections;
using UnityEngine;

namespace S13Audio;

public class S13FadeControl : MonoBehaviour
{
	public delegate void FadeDelegate();

	private AnimationCurve curve;

	private bool _isFadingIn;

	private bool _isFadingOut;

	private IEnumerator fadeInEnumerator;

	private IEnumerator fadeOutEnumerator;

	public bool IsFadingIn => _isFadingIn;

	public bool IsFadingOut => _isFadingOut;

	public bool IsFading => _isFadingIn && _isFadingOut;

	public AnimationCurve Curve
	{
		set
		{
			curve = value;
		}
	}

	public void FadeIn(AudioSource audioSource, float startVolume, float endVolume, float fadeTime = 1f, bool ignoreTimeScale = true, FadeDelegate completionHandler = null)
	{
		if (_isFadingIn)
		{
			return;
		}
		if (_isFadingOut)
		{
			if (fadeOutEnumerator == null)
			{
				Debug.LogWarning((object)"Attempting to stop unassigned fade OUT coroutine, operation cancelled.", (Object)(object)((Component)this).gameObject);
				return;
			}
			((MonoBehaviour)this).StopCoroutine(fadeOutEnumerator);
			_isFadingOut = false;
		}
		_isFadingIn = true;
		fadeInEnumerator = Fade(audioSource, startVolume, endVolume, fadeTime, ignoreTimeScale, completionHandler, fadeOut: false);
		((MonoBehaviour)this).StartCoroutine(fadeInEnumerator);
	}

	public void FadeOut(AudioSource audioSource, float startVolume, float fadeTime = 1f, bool ignoreTimeScale = true, FadeDelegate completionHandler = null)
	{
		if (_isFadingOut)
		{
			return;
		}
		if (_isFadingIn)
		{
			if (fadeInEnumerator == null)
			{
				Debug.LogWarning((object)"Attempting to stop unassigned fade IN coroutine, operation cancelled.", (Object)(object)((Component)this).gameObject);
				return;
			}
			((MonoBehaviour)this).StopCoroutine(fadeInEnumerator);
			_isFadingIn = false;
		}
		_isFadingOut = true;
		fadeOutEnumerator = Fade(audioSource, startVolume, 0f, fadeTime, ignoreTimeScale, completionHandler);
		((MonoBehaviour)this).StartCoroutine(fadeOutEnumerator);
	}

	private IEnumerator Fade(AudioSource audioSource, float startVolume, float endVolume, float fadeTime, bool ignoreTimeScale, FadeDelegate completionHandler = null, bool fadeOut = true)
	{
		float timeElapsed = 0f;
		float lastTime = Time.realtimeSinceStartup;
		while (timeElapsed < fadeTime)
		{
			float t = timeElapsed / fadeTime;
			if (curve != null)
			{
				audioSource.volume = Mathf.Lerp(startVolume, endVolume, curve.Evaluate(t));
			}
			audioSource.volume = Mathf.Lerp(startVolume, endVolume, t);
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
		if (fadeOut)
		{
			audioSource.volume = 0f;
			_isFadingOut = false;
		}
		else
		{
			audioSource.volume = endVolume;
			_isFadingIn = false;
		}
		completionHandler?.Invoke();
	}
}
