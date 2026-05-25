using DG.Tweening.Core;
using UnityEngine;

namespace DG.Tweening;

public static class DOTweenModuleUnityVersion
{
	public static Sequence DOGradientColor(this Material target, Gradient gradient, float duration)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		GradientColorKey[] colorKeys = gradient.colorKeys;
		int num = colorKeys.Length;
		for (int i = 0; i < num; i++)
		{
			GradientColorKey val2 = colorKeys[i];
			if (i == 0 && val2.time <= 0f)
			{
				target.color = val2.color;
				continue;
			}
			float num2 = ((i != num - 1) ? (duration * ((i != 0) ? (val2.time - colorKeys[i - 1].time) : val2.time)) : (duration - TweenExtensions.Duration((Tween)(object)val, false)));
			TweenSettingsExtensions.Append(val, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOColor(target, val2.color, num2), (Ease)1));
		}
		return val;
	}

	public static Sequence DOGradientColor(this Material target, Gradient gradient, string property, float duration)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		GradientColorKey[] colorKeys = gradient.colorKeys;
		int num = colorKeys.Length;
		for (int i = 0; i < num; i++)
		{
			GradientColorKey val2 = colorKeys[i];
			if (i == 0 && val2.time <= 0f)
			{
				target.SetColor(property, val2.color);
				continue;
			}
			float num2 = ((i != num - 1) ? (duration * ((i != 0) ? (val2.time - colorKeys[i - 1].time) : val2.time)) : (duration - TweenExtensions.Duration((Tween)(object)val, false)));
			TweenSettingsExtensions.Append(val, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOColor(target, val2.color, property, num2), (Ease)1));
		}
		return val;
	}

	public static CustomYieldInstruction WaitForCompletion(this Tween t, bool returnCustomYieldInstruction)
	{
		if (!t.active)
		{
			if (Debugger.logPriority > 0)
			{
				Debugger.LogInvalidTween(t);
			}
			return null;
		}
		return (CustomYieldInstruction)(object)new DOTweenCYInstruction.WaitForCompletion(t);
	}

	public static CustomYieldInstruction WaitForRewind(this Tween t, bool returnCustomYieldInstruction)
	{
		if (!t.active)
		{
			if (Debugger.logPriority > 0)
			{
				Debugger.LogInvalidTween(t);
			}
			return null;
		}
		return (CustomYieldInstruction)(object)new DOTweenCYInstruction.WaitForRewind(t);
	}

	public static CustomYieldInstruction WaitForKill(this Tween t, bool returnCustomYieldInstruction)
	{
		if (!t.active)
		{
			if (Debugger.logPriority > 0)
			{
				Debugger.LogInvalidTween(t);
			}
			return null;
		}
		return (CustomYieldInstruction)(object)new DOTweenCYInstruction.WaitForKill(t);
	}

	public static CustomYieldInstruction WaitForElapsedLoops(this Tween t, int elapsedLoops, bool returnCustomYieldInstruction)
	{
		if (!t.active)
		{
			if (Debugger.logPriority > 0)
			{
				Debugger.LogInvalidTween(t);
			}
			return null;
		}
		return (CustomYieldInstruction)(object)new DOTweenCYInstruction.WaitForElapsedLoops(t, elapsedLoops);
	}

	public static CustomYieldInstruction WaitForPosition(this Tween t, float position, bool returnCustomYieldInstruction)
	{
		if (!t.active)
		{
			if (Debugger.logPriority > 0)
			{
				Debugger.LogInvalidTween(t);
			}
			return null;
		}
		return (CustomYieldInstruction)(object)new DOTweenCYInstruction.WaitForPosition(t, position);
	}

	public static CustomYieldInstruction WaitForStart(this Tween t, bool returnCustomYieldInstruction)
	{
		if (!t.active)
		{
			if (Debugger.logPriority > 0)
			{
				Debugger.LogInvalidTween(t);
			}
			return null;
		}
		return (CustomYieldInstruction)(object)new DOTweenCYInstruction.WaitForStart(t);
	}
}
