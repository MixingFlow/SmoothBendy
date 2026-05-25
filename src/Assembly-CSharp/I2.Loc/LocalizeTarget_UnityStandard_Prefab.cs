using UnityEngine;

namespace I2.Loc;

public class LocalizeTarget_UnityStandard_Prefab : LocalizeTarget<GameObject>
{
	static LocalizeTarget_UnityStandard_Prefab()
	{
		AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void AutoRegister()
	{
		LocalizeTargetDesc_Prefab localizeTargetDesc_Prefab = new LocalizeTargetDesc_Prefab();
		localizeTargetDesc_Prefab.Name = "Prefab";
		localizeTargetDesc_Prefab.Priority = 250;
		LocalizationManager.RegisterTarget(localizeTargetDesc_Prefab);
	}

	public override bool IsValid(Localize cmp)
	{
		return true;
	}

	public override eTermType GetPrimaryTermType(Localize cmp)
	{
		return eTermType.GameObject;
	}

	public override eTermType GetSecondaryTermType(Localize cmp)
	{
		return eTermType.Text;
	}

	public override bool CanUseSecondaryTerm()
	{
		return false;
	}

	public override bool AllowMainTermToBeRTL()
	{
		return false;
	}

	public override bool AllowSecondTermToBeRTL()
	{
		return false;
	}

	public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
	{
		primaryTerm = ((Object)cmp).name;
		secondaryTerm = null;
	}

	public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
	{
		if (string.IsNullOrEmpty(mainTranslation) || (Object.op_Implicit((Object)(object)mTarget) && ((Object)mTarget).name == mainTranslation))
		{
			return;
		}
		Transform transform = ((Component)cmp).transform;
		string text = mainTranslation;
		int num = mainTranslation.LastIndexOfAny(LanguageSourceData.CategorySeparators);
		if (num >= 0)
		{
			text = text.Substring(num + 1);
		}
		Transform val = InstantiateNewPrefab(cmp, mainTranslation);
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		((Object)val).name = text;
		for (int num2 = transform.childCount - 1; num2 >= 0; num2--)
		{
			Transform child = transform.GetChild(num2);
			if ((Object)(object)child != (Object)(object)val)
			{
				Object.Destroy((Object)(object)((Component)child).gameObject);
			}
		}
	}

	private Transform InstantiateNewPrefab(Localize cmp, string mainTranslation)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = cmp.FindTranslatedObject<GameObject>(mainTranslation);
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		GameObject val2 = mTarget;
		mTarget = Object.Instantiate<GameObject>(val);
		if ((Object)(object)mTarget == (Object)null)
		{
			return null;
		}
		Transform transform = ((Component)cmp).transform;
		Transform transform2 = mTarget.transform;
		transform2.SetParent(transform);
		Transform val3 = ((!Object.op_Implicit((Object)(object)val2)) ? transform : val2.transform);
		transform2.rotation = val3.rotation;
		transform2.position = val3.position;
		return transform2;
	}
}
