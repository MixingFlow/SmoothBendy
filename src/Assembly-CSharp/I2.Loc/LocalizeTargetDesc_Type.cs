using UnityEngine;

namespace I2.Loc;

public class LocalizeTargetDesc_Type<T, G> : LocalizeTargetDesc<G> where T : Object where G : LocalizeTarget<T>
{
	public override bool CanLocalize(Localize cmp)
	{
		return (Object)(object)((Component)cmp).GetComponent<T>() != (Object)null;
	}

	public override ILocalizeTarget CreateTarget(Localize cmp)
	{
		T component = ((Component)cmp).GetComponent<T>();
		if ((Object)(object)component == (Object)null)
		{
			return null;
		}
		G val = ScriptableObject.CreateInstance<G>();
		val.mTarget = component;
		return val;
	}
}
