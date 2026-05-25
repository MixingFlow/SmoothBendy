using UnityEngine;

namespace I2.Loc;

public abstract class LocalizeTarget<T> : ILocalizeTarget where T : Object
{
	public T mTarget;

	public override bool IsValid(Localize cmp)
	{
		if ((Object)(object)mTarget != (Object)null)
		{
			object obj = mTarget;
			Component val = (Component)((obj is Component) ? obj : null);
			if ((Object)(object)val != (Object)null && (Object)(object)val.gameObject != (Object)(object)((Component)cmp).gameObject)
			{
				mTarget = (T)(object)null;
			}
		}
		if ((Object)(object)mTarget == (Object)null)
		{
			mTarget = ((Component)cmp).GetComponent<T>();
		}
		return (Object)(object)mTarget != (Object)null;
	}
}
