using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public static class DOTweenUtil
{
	public static Tweener DOAudioListenerVolume(float endValue, float duration, Action onComplete = null)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		float volume = AudioListener.volume;
		return (Tweener)(object)TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => volume), (DOSetter<float>)delegate(float value)
		{
			volume = value;
		}, endValue, duration), (TweenCallback)delegate
		{
			AudioListener.volume = volume;
		}), (TweenCallback)delegate
		{
			if (onComplete != null)
			{
				onComplete();
			}
		});
	}

	public static Tweener DOAmbientLightColor(float endValue, float duration, Action onComplete = null)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		float newColor = RenderSettings.ambientIntensity;
		return (Tweener)(object)TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetUpdate<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => newColor), (DOSetter<float>)delegate(float value)
		{
			newColor = value;
		}, endValue, duration), (UpdateType)0), (TweenCallback)delegate
		{
			RenderSettings.ambientIntensity = newColor;
		}), (TweenCallback)delegate
		{
			if (onComplete != null)
			{
				onComplete();
			}
		});
	}

	public static void KillAll()
	{
		DOTween.KillAll(false);
		DOTween.Clear(false);
	}
}
