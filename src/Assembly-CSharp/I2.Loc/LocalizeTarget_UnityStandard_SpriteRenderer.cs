using UnityEngine;

namespace I2.Loc;

public class LocalizeTarget_UnityStandard_SpriteRenderer : LocalizeTarget<SpriteRenderer>
{
	static LocalizeTarget_UnityStandard_SpriteRenderer()
	{
		AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void AutoRegister()
	{
		LocalizeTargetDesc_Type<SpriteRenderer, LocalizeTarget_UnityStandard_SpriteRenderer> localizeTargetDesc_Type = new LocalizeTargetDesc_Type<SpriteRenderer, LocalizeTarget_UnityStandard_SpriteRenderer>();
		localizeTargetDesc_Type.Name = "SpriteRenderer";
		localizeTargetDesc_Type.Priority = 100;
		LocalizationManager.RegisterTarget(localizeTargetDesc_Type);
	}

	public override eTermType GetPrimaryTermType(Localize cmp)
	{
		return eTermType.Sprite;
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
		primaryTerm = ((!((Object)(object)mTarget.sprite != (Object)null)) ? string.Empty : ((Object)mTarget.sprite).name);
		secondaryTerm = null;
	}

	public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
	{
		Sprite sprite = mTarget.sprite;
		if ((Object)(object)sprite == (Object)null || ((Object)sprite).name != mainTranslation)
		{
			mTarget.sprite = cmp.FindTranslatedObject<Sprite>(mainTranslation);
		}
	}
}
