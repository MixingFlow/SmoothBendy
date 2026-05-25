using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening;

public static class DOTweenModulePhysics
{
	public static Tweener DOMove(this Rigidbody target, Vector3 endValue, float duration, bool snapping = false)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, endValue, duration), snapping), (object)target);
	}

	public static Tweener DOMoveX(this Rigidbody target, float endValue, float duration, bool snapping = false)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, new Vector3(endValue, 0f, 0f), duration), (AxisConstraint)2, snapping), (object)target);
	}

	public static Tweener DOMoveY(this Rigidbody target, float endValue, float duration, bool snapping = false)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, new Vector3(0f, endValue, 0f), duration), (AxisConstraint)4, snapping), (object)target);
	}

	public static Tweener DOMoveZ(this Rigidbody target, float endValue, float duration, bool snapping = false)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetTarget<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, new Vector3(0f, 0f, endValue), duration), (AxisConstraint)8, snapping), (object)target);
	}

	public static Tweener DORotate(this Rigidbody target, Vector3 endValue, float duration, RotateMode mode = (RotateMode)0)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		TweenerCore<Quaternion, Vector3, QuaternionOptions> val = DOTween.To((DOGetter<Quaternion>)(() => target.rotation), (DOSetter<Quaternion>)target.MoveRotation, endValue, duration);
		TweenSettingsExtensions.SetTarget<TweenerCore<Quaternion, Vector3, QuaternionOptions>>(val, (object)target);
		val.plugOptions.rotateMode = mode;
		return (Tweener)(object)val;
	}

	public static Tweener DOLookAt(this Rigidbody target, Vector3 towards, float duration, AxisConstraint axisConstraint = (AxisConstraint)0, Vector3? up = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		TweenerCore<Quaternion, Vector3, QuaternionOptions> val = Extensions.SetSpecialStartupMode<TweenerCore<Quaternion, Vector3, QuaternionOptions>>(TweenSettingsExtensions.SetTarget<TweenerCore<Quaternion, Vector3, QuaternionOptions>>(DOTween.To((DOGetter<Quaternion>)(() => target.rotation), (DOSetter<Quaternion>)target.MoveRotation, towards, duration), (object)target), (SpecialStartupMode)1);
		val.plugOptions.axisConstraint = axisConstraint;
		val.plugOptions.up = (up.HasValue ? up.Value : Vector3.up);
		return (Tweener)(object)val;
	}

	public static Sequence DOJump(this Rigidbody target, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		if (numJumps < 1)
		{
			numJumps = 1;
		}
		float startPosY = 0f;
		float offsetY = -1f;
		bool offsetYSet = false;
		Sequence s = DOTween.Sequence();
		Tween yTween = (Tween)(object)TweenSettingsExtensions.OnStart<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetRelative<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, new Vector3(0f, jumpPower, 0f), duration / (float)(numJumps * 2)), (AxisConstraint)4, snapping), (Ease)6)), numJumps * 2, (LoopType)1), (TweenCallback)delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			startPosY = target.position.y;
		});
		TweenSettingsExtensions.SetEase<Sequence>(TweenSettingsExtensions.SetTarget<Sequence>(TweenSettingsExtensions.Join(TweenSettingsExtensions.Join(TweenSettingsExtensions.Append(s, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, new Vector3(endValue.x, 0f, 0f), duration), (AxisConstraint)2, snapping), (Ease)1)), (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetOptions(DOTween.To((DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, new Vector3(0f, 0f, endValue.z), duration), (AxisConstraint)8, snapping), (Ease)1)), yTween), (object)target), DOTween.defaultEaseType);
		TweenSettingsExtensions.OnUpdate<Tween>(yTween, (TweenCallback)delegate
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			if (!offsetYSet)
			{
				offsetYSet = true;
				offsetY = ((!((Tween)s).isRelative) ? (endValue.y - startPosY) : endValue.y);
			}
			Vector3 position = target.position;
			position.y += DOVirtual.EasedValue(0f, offsetY, TweenExtensions.ElapsedPercentage(yTween, true), (Ease)6);
			target.MovePosition(position);
		});
		return s;
	}

	public static TweenerCore<Vector3, Path, PathOptions> DOPath(this Rigidbody target, Vector3[] path, float duration, PathType pathType = (PathType)0, PathMode pathMode = (PathMode)1, int resolution = 10, Color? gizmoColor = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (resolution < 1)
		{
			resolution = 1;
		}
		TweenerCore<Vector3, Path, PathOptions> val = TweenSettingsExtensions.SetUpdate<TweenerCore<Vector3, Path, PathOptions>>(TweenSettingsExtensions.SetTarget<TweenerCore<Vector3, Path, PathOptions>>(DOTween.To<Vector3, Path, PathOptions>(PathPlugin.Get(), (DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, new Path(pathType, path, resolution, gizmoColor), duration), (object)target), (UpdateType)2);
		val.plugOptions.isRigidbody = true;
		val.plugOptions.mode = pathMode;
		return val;
	}

	public static TweenerCore<Vector3, Path, PathOptions> DOLocalPath(this Rigidbody target, Vector3[] path, float duration, PathType pathType = (PathType)0, PathMode pathMode = (PathMode)1, int resolution = 10, Color? gizmoColor = null)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (resolution < 1)
		{
			resolution = 1;
		}
		Transform trans = ((Component)target).transform;
		TweenerCore<Vector3, Path, PathOptions> val = TweenSettingsExtensions.SetUpdate<TweenerCore<Vector3, Path, PathOptions>>(TweenSettingsExtensions.SetTarget<TweenerCore<Vector3, Path, PathOptions>>(DOTween.To<Vector3, Path, PathOptions>(PathPlugin.Get(), (DOGetter<Vector3>)(() => trans.localPosition), (DOSetter<Vector3>)delegate(Vector3 x)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			target.MovePosition((!((Object)(object)trans.parent == (Object)null)) ? trans.parent.TransformPoint(x) : x);
		}, new Path(pathType, path, resolution, gizmoColor), duration), (object)target), (UpdateType)2);
		val.plugOptions.isRigidbody = true;
		val.plugOptions.mode = pathMode;
		val.plugOptions.useLocalPosition = true;
		return val;
	}

	internal static TweenerCore<Vector3, Path, PathOptions> DOPath(this Rigidbody target, Path path, float duration, PathMode pathMode = (PathMode)1)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		TweenerCore<Vector3, Path, PathOptions> val = TweenSettingsExtensions.SetTarget<TweenerCore<Vector3, Path, PathOptions>>(DOTween.To<Vector3, Path, PathOptions>(PathPlugin.Get(), (DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, path, duration), (object)target);
		val.plugOptions.isRigidbody = true;
		val.plugOptions.mode = pathMode;
		return val;
	}

	internal static TweenerCore<Vector3, Path, PathOptions> DOLocalPath(this Rigidbody target, Path path, float duration, PathMode pathMode = (PathMode)1)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Transform trans = ((Component)target).transform;
		TweenerCore<Vector3, Path, PathOptions> val = TweenSettingsExtensions.SetTarget<TweenerCore<Vector3, Path, PathOptions>>(DOTween.To<Vector3, Path, PathOptions>(PathPlugin.Get(), (DOGetter<Vector3>)(() => trans.localPosition), (DOSetter<Vector3>)delegate(Vector3 x)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			target.MovePosition((!((Object)(object)trans.parent == (Object)null)) ? trans.parent.TransformPoint(x) : x);
		}, path, duration), (object)target);
		val.plugOptions.isRigidbody = true;
		val.plugOptions.mode = pathMode;
		val.plugOptions.useLocalPosition = true;
		return val;
	}
}
