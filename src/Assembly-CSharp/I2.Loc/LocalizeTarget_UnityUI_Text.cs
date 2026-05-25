using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc;

public class LocalizeTarget_UnityUI_Text : LocalizeTarget<Text>
{
	private TextAnchor mAlignment_RTL = (TextAnchor)2;

	private TextAnchor mAlignment_LTR;

	private bool mAlignmentWasRTL;

	private bool mInitializeAlignment = true;

	static LocalizeTarget_UnityUI_Text()
	{
		AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void AutoRegister()
	{
		LocalizeTargetDesc_Type<Text, LocalizeTarget_UnityUI_Text> localizeTargetDesc_Type = new LocalizeTargetDesc_Type<Text, LocalizeTarget_UnityUI_Text>();
		localizeTargetDesc_Type.Name = "Text";
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
		secondaryTerm = ((!((Object)(object)mTarget.font != (Object)null)) ? string.Empty : ((Object)mTarget.font).name);
	}

	public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		Font secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<Font>(ref mainTranslation, ref secondaryTranslation);
		if ((Object)(object)secondaryTranslatedObj != (Object)null && (Object)(object)secondaryTranslatedObj != (Object)(object)mTarget.font)
		{
			mTarget.font = secondaryTranslatedObj;
		}
		if (mInitializeAlignment)
		{
			mInitializeAlignment = false;
			mAlignmentWasRTL = LocalizationManager.IsRight2Left;
			InitAlignment(mAlignmentWasRTL, mTarget.alignment, out mAlignment_LTR, out mAlignment_RTL);
		}
		else
		{
			InitAlignment(mAlignmentWasRTL, mTarget.alignment, out var alignLTR, out var alignRTL);
			if ((mAlignmentWasRTL && mAlignment_RTL != alignRTL) || (!mAlignmentWasRTL && mAlignment_LTR != alignLTR))
			{
				mAlignment_LTR = alignLTR;
				mAlignment_RTL = alignRTL;
			}
			mAlignmentWasRTL = LocalizationManager.IsRight2Left;
		}
		if (mainTranslation != null && mTarget.text != mainTranslation)
		{
			if (cmp.CorrectAlignmentForRTL)
			{
				mTarget.alignment = ((!LocalizationManager.IsRight2Left) ? mAlignment_LTR : mAlignment_RTL);
			}
			mTarget.text = mainTranslation;
			((Graphic)mTarget).SetVerticesDirty();
		}
	}

	private void InitAlignment(bool isRTL, TextAnchor alignment, out TextAnchor alignLTR, out TextAnchor alignRTL)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected I4, but got Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected I4, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected I4, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected I4, but got Unknown
		TextAnchor val;
		alignRTL = (TextAnchor)(int)(val = alignment);
		alignLTR = (TextAnchor)(int)val;
		if (isRTL)
		{
			switch ((int)alignment)
			{
			case 2:
				alignLTR = (TextAnchor)0;
				break;
			case 5:
				alignLTR = (TextAnchor)3;
				break;
			case 8:
				alignLTR = (TextAnchor)6;
				break;
			case 0:
				alignLTR = (TextAnchor)2;
				break;
			case 3:
				alignLTR = (TextAnchor)5;
				break;
			case 6:
				alignLTR = (TextAnchor)8;
				break;
			case 1:
			case 4:
			case 7:
				break;
			}
		}
		else
		{
			switch ((int)alignment)
			{
			case 2:
				alignRTL = (TextAnchor)0;
				break;
			case 5:
				alignRTL = (TextAnchor)3;
				break;
			case 8:
				alignRTL = (TextAnchor)6;
				break;
			case 0:
				alignRTL = (TextAnchor)2;
				break;
			case 3:
				alignRTL = (TextAnchor)5;
				break;
			case 6:
				alignRTL = (TextAnchor)8;
				break;
			case 1:
			case 4:
			case 7:
				break;
			}
		}
	}
}
