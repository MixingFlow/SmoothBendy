using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening;

public static class ShortcutExtensionsTMPText
{
	public static Tweener DOColor(this TMP_Text target, Color endValue, float duration)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		return (Tweener)(object)TweenSettingsExtensions.SetTarget<TweenerCore<Color, Color, ColorOptions>>(DOTween.To((DOGetter<Color>)(() => ((Graphic)target).color), (DOSetter<Color>)delegate(Color x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Graphic)target).color = x;
		}, endValue, duration), (object)target);
	}

	public static Tweener DOFaceColor(this TMP_Text target, Color32 endValue, float duration)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return (Tweener)(object)TweenSettingsExtensions.SetTarget<TweenerCore<Color, Color, ColorOptions>>(DOTween.To((DOGetter<Color>)(() => Color32.op_Implicit(target.faceColor)), (DOSetter<Color>)delegate(Color x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			target.faceColor = Color32.op_Implicit(x);
		}, Color32.op_Implicit(endValue), duration), (object)target);
	}

	public static Tweener DOOutlineColor(this TMP_Text target, Color32 endValue, float duration)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return (Tweener)(object)TweenSettingsExtensions.SetTarget<TweenerCore<Color, Color, ColorOptions>>(DOTween.To((DOGetter<Color>)(() => Color32.op_Implicit(target.outlineColor)), (DOSetter<Color>)delegate(Color x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			target.outlineColor = Color32.op_Implicit(x);
		}, Color32.op_Implicit(endValue), duration), (object)target);
	}

	public static Tweener DOGlowColor(this TMP_Text target, Color endValue, float duration, bool useSharedMaterial = false)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return (!useSharedMaterial) ? TweenSettingsExtensions.SetTarget<Tweener>(ShortcutExtensions.DOColor(target.fontMaterial, endValue, "_GlowColor", duration), (object)target) : TweenSettingsExtensions.SetTarget<Tweener>(ShortcutExtensions.DOColor(target.fontSharedMaterial, endValue, "_GlowColor", duration), (object)target);
	}

	public static Tweener DOFade(this TMP_Text target, float endValue, float duration)
	{
		return TweenSettingsExtensions.SetTarget<Tweener>(DOTween.ToAlpha((DOGetter<Color>)(() => ((Graphic)target).color), (DOSetter<Color>)delegate(Color x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Graphic)target).color = x;
		}, endValue, duration), (object)target);
	}

	public static Tweener DOFaceFade(this TMP_Text target, float endValue, float duration)
	{
		return TweenSettingsExtensions.SetTarget<Tweener>(DOTween.ToAlpha((DOGetter<Color>)(() => Color32.op_Implicit(target.faceColor)), (DOSetter<Color>)delegate(Color x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			target.faceColor = Color32.op_Implicit(x);
		}, endValue, duration), (object)target);
	}

	public static Tweener DOScale(this TMP_Text target, float endValue, float duration)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Transform t = target.transform;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(endValue, endValue, endValue);
		return (Tweener)(object)TweenSettingsExtensions.SetTarget<TweenerCore<Vector3, Vector3, VectorOptions>>(DOTween.To((DOGetter<Vector3>)(() => t.localScale), (DOSetter<Vector3>)delegate(Vector3 x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			t.localScale = x;
		}, val, duration), (object)target);
	}

	public static Tweener DOFontSize(this TMP_Text target, float endValue, float duration)
	{
		return (Tweener)(object)TweenSettingsExtensions.SetTarget<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => target.fontSize), (DOSetter<float>)delegate(float x)
		{
			target.fontSize = x;
		}, endValue, duration), (object)target);
	}

	public static Tweener DOMaxVisibleCharacters(this TMP_Text target, int endValue, float duration)
	{
		return TweenSettingsExtensions.SetTarget<Tweener>(DOTween.To((DOGetter<int>)(() => target.maxVisibleCharacters), (DOSetter<int>)delegate(int x)
		{
			target.maxVisibleCharacters = x;
		}, endValue, duration), (object)target);
	}

	public static Tweener DOText(this TMP_Text target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = (ScrambleMode)0, string scrambleChars = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<string>)(() => target.text), (DOSetter<string>)delegate(string x)
		{
			target.text = x;
		}, endValue, duration), richTextEnabled, scrambleMode, scrambleChars), (object)target);
	}
}
