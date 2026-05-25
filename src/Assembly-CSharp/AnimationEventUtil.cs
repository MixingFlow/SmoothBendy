using UnityEngine;

public static class AnimationEventUtil
{
	public static void AddAnimationEvent(ref Animator animator, string clipName, string functionName, int frame)
	{
		AnimationClip val = null;
		for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
		{
			if (((Object)animator.runtimeAnimatorController.animationClips[i]).name == clipName)
			{
				val = animator.runtimeAnimatorController.animationClips[i];
				break;
			}
		}
		if (!((Object)(object)val != (Object)null))
		{
			return;
		}
		float num = (float)frame / 30f;
		AnimationEvent val2 = AddEvent(functionName, num);
		AnimationEvent[] events = val.events;
		bool flag = true;
		foreach (AnimationEvent val3 in events)
		{
			if (val3.functionName == functionName && val3.time == num)
			{
				flag = false;
				return;
			}
		}
		if (flag)
		{
			val.AddEvent(val2);
		}
	}

	private static AnimationEvent AddEvent(string functionName, float time)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		AnimationEvent val = new AnimationEvent();
		val.functionName = functionName;
		val.time = time;
		return val;
	}

	public static AnimationEventController AddEventController(ref Animator animator)
	{
		AnimationEventController animationEventController = ((Component)animator).gameObject.GetComponent<AnimationEventController>();
		if ((Object)(object)animationEventController == (Object)null)
		{
			animationEventController = ((Component)animator).gameObject.AddComponent<AnimationEventController>();
		}
		return animationEventController;
	}
}
