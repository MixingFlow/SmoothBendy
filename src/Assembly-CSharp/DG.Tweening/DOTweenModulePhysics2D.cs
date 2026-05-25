using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening;

public static class DOTweenModulePhysics2D
{
	public static Tweener DOMove(this Rigidbody2D target, Vector2 endValue, float duration, bool snapping = false)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector2>)(() => target.position), (DOSetter<Vector2>)target.MovePosition, endValue, duration), snapping), (object)target);
	}

	public static Tweener DOMoveX(this Rigidbody2D target, float endValue, float duration, bool snapping = false)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector2>)(() => target.position), (DOSetter<Vector2>)target.MovePosition, new Vector2(endValue, 0f), duration), (AxisConstraint)2, snapping), (object)target);
	}

	public static Tweener DOMoveY(this Rigidbody2D target, float endValue, float duration, bool snapping = false)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector2>)(() => target.position), (DOSetter<Vector2>)target.MovePosition, new Vector2(0f, endValue), duration), (AxisConstraint)4, snapping), (object)target);
	}

	public static Tweener DORotate(this Rigidbody2D target, float endValue, float duration)
	{
		return (Tweener)(object)TweenSettingsExtensions.SetTarget<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => target.rotation), (DOSetter<float>)target.MoveRotation, endValue, duration), (object)target);
	}

	public static Sequence DOJump(this Rigidbody2D target, Vector2 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		if (numJumps < 1)
		{
			numJumps = 1;
		}
		float startPosY = 0f;
		float offsetY = -1f;
		bool offsetYSet = false;
		Sequence s = DOTween.Sequence();
		Tween yTween = (Tween)(object)TweenSettingsExtensions.OnStart<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetRelative<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector2>)(() => target.position), (DOSetter<Vector2>)delegate(Vector2 x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			target.position = x;
		}, new Vector2(0f, jumpPower), duration / (float)(numJumps * 2)), (AxisConstraint)4, snapping), (Ease)6)), numJumps * 2, (LoopType)1), (TweenCallback)delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			startPosY = target.position.y;
		});
		TweenSettingsExtensions.SetEase<Sequence>(TweenSettingsExtensions.SetTarget<Sequence>(TweenSettingsExtensions.Join(TweenSettingsExtensions.Append(s, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector2>)(() => target.position), (DOSetter<Vector2>)delegate(Vector2 x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			target.position = x;
		}, new Vector2(endValue.x, 0f), duration), (AxisConstraint)2, snapping), (Ease)1)), yTween), (object)target), DOTween.defaultEaseType);
		TweenSettingsExtensions.OnUpdate<Tween>(yTween, (TweenCallback)delegate
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			if (!offsetYSet)
			{
				offsetYSet = true;
				offsetY = ((!((Tween)s).isRelative) ? (endValue.y - startPosY) : endValue.y);
			}
			Vector3 val = Vector2.op_Implicit(target.position);
			val.y += DOVirtual.EasedValue(0f, offsetY, TweenExtensions.ElapsedPercentage(yTween, true), (Ease)6);
			target.MovePosition(Vector2.op_Implicit(val));
		});
		return s;
	}
}
