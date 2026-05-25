using UnityEngine;

namespace I2.Loc;

public class LocalizeTarget_UnityStandard_TextMesh : LocalizeTarget<TextMesh>
{
	private TextAlignment mAlignment_RTL = (TextAlignment)2;

	private TextAlignment mAlignment_LTR;

	private bool mAlignmentWasRTL;

	private bool mInitializeAlignment = true;

	static LocalizeTarget_UnityStandard_TextMesh()
	{
		AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void AutoRegister()
	{
		LocalizeTargetDesc_Type<TextMesh, LocalizeTarget_UnityStandard_TextMesh> localizeTargetDesc_Type = new LocalizeTargetDesc_Type<TextMesh, LocalizeTarget_UnityStandard_TextMesh>();
		localizeTargetDesc_Type.Name = "TextMesh";
		localizeTargetDesc_Type.Priority = 100;
		LocalizationManager.RegisterTarget(localizeTargetDesc_Type);
	}

	public override eTermType GetPrimaryTermType(Localize cmp)
	{
		return eTermType.Text;
	}

	public override eTermType GetSecondaryTermType(Localize cmp)
	{
		return eTermType.Font;
	}

	public override bool CanUseSecondaryTerm()
	{
		return true;
	}

	public override bool AllowMainTermToBeRTL()
	{
		return true;
	}

	public override bool AllowSecondTermToBeRTL()
	{
		return false;
	}

	public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
	{
		primaryTerm = ((!Object.op_Implicit((Object)(object)mTarget)) ? null : mTarget.text);
		secondaryTerm = ((!string.IsNullOrEmpty(Secondary) || !((Object)(object)mTarget.font != (Object)null)) ? null : ((Object)mTarget.font).name);
	}

	public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Invalid comparison between Unknown and I4
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Invalid comparison between Unknown and I4
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		Font secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<Font>(ref mainTranslation, ref secondaryTranslation);
		if ((Object)(object)secondaryTranslatedObj != (Object)null && (Object)(object)mTarget.font != (Object)(object)secondaryTranslatedObj)
		{
			mTarget.font = secondaryTranslatedObj;
		}
		if (mInitializeAlignment)
		{
			mInitializeAlignment = false;
			mAlignment_LTR = (mAlignment_RTL = mTarget.alignment);
			if (LocalizationManager.IsRight2Left && (int)mAlignment_RTL == 2)
			{
				mAlignment_LTR = (TextAlignment)0;
			}
			if (!LocalizationManager.IsRight2Left && (int)mAlignment_LTR == 0)
			{
				mAlignment_RTL = (TextAlignment)2;
			}
		}
		if (mainTranslation != null && mTarget.text != mainTranslation)
		{
			if (cmp.CorrectAlignmentForRTL && (int)mTarget.alignment != 1)
			{
				mTarget.alignment = ((!LocalizationManager.IsRight2Left) ? mAlignment_LTR : mAlignment_RTL);
			}
			mTarget.font.RequestCharactersInTexture(mainTranslation);
			mTarget.text = mainTranslation;
		}
	}
}
