using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc;

public class LocalizeTarget_UnityUI_Image : LocalizeTarget<Image>
{
	static LocalizeTarget_UnityUI_Image()
	{
		AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void AutoRegister()
	{
		LocalizeTargetDesc_Type<Image, LocalizeTarget_UnityUI_Image> localizeTargetDesc_Type = new LocalizeTargetDesc_Type<Image, LocalizeTarget_UnityUI_Image>();
		localizeTargetDesc_Type.Name = "Image";
		localizeTargetDesc_Type.Priority = 100;
		LocalizationManager.RegisterTarget(localizeTargetDesc_Type);
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

	public override eTermType GetPrimaryTermType(Localize cmp)
	{
		return (!((Object)(object)mTarget.sprite == (Object)null)) ? eTermType.Sprite : eTermType.Texture;
	}

	public override eTermType GetSecondaryTermType(Localize cmp)
	{
		return eTermType.Text;
	}

	public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
	{
		primaryTerm = ((!Object.op_Implicit((Object)(object)((Graphic)mTarget).mainTexture)) ? string.Empty : ((Object)((Graphic)mTarget).mainTexture).name);
		if ((Object)(object)mTarget.sprite != (Object)null && ((Object)mTarget.sprite).name != primaryTerm)
		{
			primaryTerm = primaryTerm + "." + ((Object)mTarget.sprite).name;
		}
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
