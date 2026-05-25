using DG.Tweening.Core;
using DG.Tweening.Plugins;
using UnityEngine;

namespace DG.Tweening;

public static class DOTweenProShortcuts
{
	static DOTweenProShortcuts()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		SpiralPlugin val = new SpiralPlugin();
	}

	public static Tweener DOSpiral(this Transform target, float duration, Vector3? axis = null, SpiralMode mode = (SpiralMode)0, float speed = 1f, float frequency = 10f, float depth = 0f, bool snapping = false)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (Mathf.Approximately(speed, 0f))
		{
			speed = 1f;
		}
		if (!axis.HasValue || (axis.HasValue && axis.GetValueOrDefault() == Vector3.zero))
		{
			axis = Vector3.forward;
		}
		TweenerCore<Vector3, Vector3, SpiralOptions> val = TweenSettingsExtensions.SetTarget<TweenerCore<Vector3, Vector3, SpiralOptions>>(DOTween.To<Vector3, Vector3, SpiralOptions>(SpiralPlugin.Get(), (DOGetter<Vector3>)(() => target.localPosition), (DOSetter<Vector3>)delegate(Vector3 x)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			target.localPosition = x;
		}, axis.Value, duration), (object)target);
		val.plugOptions.mode = mode;
		val.plugOptions.speed = speed;
		val.plugOptions.frequency = frequency;
		val.plugOptions.depth = depth;
		val.plugOptions.snapping = snapping;
		return (Tweener)(object)val;
	}

	public static Tweener DOSpiral(this Rigidbody target, float duration, Vector3? axis = null, SpiralMode mode = (SpiralMode)0, float speed = 1f, float frequency = 10f, float depth = 0f, bool snapping = false)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (Mathf.Approximately(speed, 0f))
		{
			speed = 1f;
		}
		if (!axis.HasValue || (axis.HasValue && axis.GetValueOrDefault() == Vector3.zero))
		{
			axis = Vector3.forward;
		}
		TweenerCore<Vector3, Vector3, SpiralOptions> val = TweenSettingsExtensions.SetTarget<TweenerCore<Vector3, Vector3, SpiralOptions>>(DOTween.To<Vector3, Vector3, SpiralOptions>(SpiralPlugin.Get(), (DOGetter<Vector3>)(() => target.position), (DOSetter<Vector3>)target.MovePosition, axis.Value, duration), (object)target);
		val.plugOptions.mode = mode;
		val.plugOptions.speed = speed;
		val.plugOptions.frequency = frequency;
		val.plugOptions.depth = depth;
		val.plugOptions.snapping = snapping;
		return (Tweener)(object)val;
	}
}
