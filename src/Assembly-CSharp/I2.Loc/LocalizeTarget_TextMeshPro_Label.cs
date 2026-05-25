using System;
using TMPro;
using UnityEngine;

namespace I2.Loc;

public class LocalizeTarget_TextMeshPro_Label : LocalizeTarget<TextMeshPro>
{
	private TextAlignmentOptions mAlignment_RTL = TextAlignmentOptions.Right;

	private TextAlignmentOptions mAlignment_LTR = TextAlignmentOptions.Left;

	private bool mAlignmentWasRTL;

	private bool mInitializeAlignment = true;

	static LocalizeTarget_TextMeshPro_Label()
	{
		AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void AutoRegister()
	{
		LocalizeTargetDesc_Type<TextMeshPro, LocalizeTarget_TextMeshPro_Label> localizeTargetDesc_Type = new LocalizeTargetDesc_Type<TextMeshPro, LocalizeTarget_TextMeshPro_Label>();
		localizeTargetDesc_Type.Name = "TextMeshPro Label";
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
		TMP_FontAsset secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<TMP_FontAsset>(ref mainTranslation, ref secondaryTranslation);
		if ((Object)(object)secondaryTranslatedObj != (Object)null)
		{
			SetFont(mTarget, secondaryTranslatedObj);
		}
		else
		{
			Material secondaryTranslatedObj2 = cmp.GetSecondaryTranslatedObj<Material>(ref mainTranslation, ref secondaryTranslation);
			if ((Object)(object)secondaryTranslatedObj2 != (Object)null && (Object)(object)mTarget.fontMaterial != (Object)(object)secondaryTranslatedObj2)
			{
				if (!((Object)secondaryTranslatedObj2).name.StartsWith(((Object)mTarget.font).name, StringComparison.Ordinal))
				{
					secondaryTranslatedObj = GetTMPFontFromMaterial(cmp, (!secondaryTranslation.EndsWith(((Object)secondaryTranslatedObj2).name, StringComparison.Ordinal)) ? ((Object)secondaryTranslatedObj2).name : secondaryTranslation);
					if ((Object)(object)secondaryTranslatedObj != (Object)null)
					{
						SetFont(mTarget, secondaryTranslatedObj);
					}
				}
				SetMaterial(mTarget, secondaryTranslatedObj2);
			}
		}
		if (mInitializeAlignment)
		{
			mInitializeAlignment = false;
			mAlignmentWasRTL = LocalizationManager.IsRight2Left;
			InitAlignment_TMPro(mAlignmentWasRTL, mTarget.alignment, out mAlignment_LTR, out mAlignment_RTL);
		}
		else
		{
			InitAlignment_TMPro(mAlignmentWasRTL, mTarget.alignment, out var alignLTR, out var alignRTL);
			if ((mAlignmentWasRTL && mAlignment_RTL != alignRTL) || (!mAlignmentWasRTL && mAlignment_LTR != alignLTR))
			{
				mAlignment_LTR = alignLTR;
				mAlignment_RTL = alignRTL;
			}
			mAlignmentWasRTL = LocalizationManager.IsRight2Left;
		}
		if (mainTranslation == null || !(mTarget.text != mainTranslation))
		{
			return;
		}
		if (mainTranslation != null && cmp.CorrectAlignmentForRTL)
		{
			mTarget.alignment = ((!LocalizationManager.IsRight2Left) ? mAlignment_LTR : mAlignment_RTL);
			mTarget.isRightToLeftText = LocalizationManager.IsRight2Left;
			if (LocalizationManager.IsRight2Left)
			{
				mainTranslation = I2Utils.ReverseText(mainTranslation);
			}
		}
		mTarget.text = mainTranslation;
	}

	internal static TMP_FontAsset GetTMPFontFromMaterial(Localize cmp, string matName)
	{
		string text = " .\\/-[]()";
		int num = matName.Length - 1;
		while (num > 0)
		{
			while (num > 0 && text.IndexOf(matName[num]) >= 0)
			{
				num--;
			}
			if (num <= 0)
			{
				break;
			}
			string translation = matName.Substring(0, num + 1);
			TMP_FontAsset tMP_FontAsset = cmp.GetObject<TMP_FontAsset>(translation);
			if ((Object)(object)tMP_FontAsset != (Object)null)
			{
				return tMP_FontAsset;
			}
			while (num > 0 && text.IndexOf(matName[num]) < 0)
			{
				num--;
			}
		}
		return null;
	}

	internal static void InitAlignment_TMPro(bool isRTL, TextAlignmentOptions alignment, out TextAlignmentOptions alignLTR, out TextAlignmentOptions alignRTL)
	{
		alignLTR = (alignRTL = alignment);
		if (isRTL)
		{
			switch (alignment)
			{
			case TextAlignmentOptions.TopRight:
				alignLTR = TextAlignmentOptions.TopLeft;
				break;
			case TextAlignmentOptions.Right:
				alignLTR = TextAlignmentOptions.Left;
				break;
			case TextAlignmentOptions.BottomRight:
				alignLTR = TextAlignmentOptions.BottomLeft;
				break;
			case TextAlignmentOptions.BaselineRight:
				alignLTR = TextAlignmentOptions.BaselineLeft;
				break;
			case TextAlignmentOptions.MidlineRight:
				alignLTR = TextAlignmentOptions.MidlineLeft;
				break;
			case TextAlignmentOptions.CaplineRight:
				alignLTR = TextAlignmentOptions.CaplineLeft;
				break;
			case TextAlignmentOptions.TopLeft:
				alignLTR = TextAlignmentOptions.TopRight;
				break;
			case TextAlignmentOptions.Left:
				alignLTR = TextAlignmentOptions.Right;
				break;
			case TextAlignmentOptions.BottomLeft:
				alignLTR = TextAlignmentOptions.BottomRight;
				break;
			case TextAlignmentOptions.BaselineLeft:
				alignLTR = TextAlignmentOptions.BaselineRight;
				break;
			case TextAlignmentOptions.MidlineLeft:
				alignLTR = TextAlignmentOptions.MidlineRight;
				break;
			case TextAlignmentOptions.CaplineLeft:
				alignLTR = TextAlignmentOptions.CaplineRight;
				break;
			case TextAlignmentOptions.Top:
			case TextAlignmentOptions.TopJustified:
			case TextAlignmentOptions.Center:
			case TextAlignmentOptions.Justified:
			case TextAlignmentOptions.Bottom:
			case TextAlignmentOptions.BottomJustified:
			case TextAlignmentOptions.Baseline:
			case TextAlignmentOptions.BaselineJustified:
			case TextAlignmentOptions.Midline:
			case TextAlignmentOptions.MidlineJustified:
			case TextAlignmentOptions.Capline:
				break;
			}
		}
		else
		{
			switch (alignment)
			{
			case TextAlignmentOptions.TopRight:
				alignRTL = TextAlignmentOptions.TopLeft;
				break;
			case TextAlignmentOptions.Right:
				alignRTL = TextAlignmentOptions.Left;
				break;
			case TextAlignmentOptions.BottomRight:
				alignRTL = TextAlignmentOptions.BottomLeft;
				break;
			case TextAlignmentOptions.BaselineRight:
				alignRTL = TextAlignmentOptions.BaselineLeft;
				break;
			case TextAlignmentOptions.MidlineRight:
				alignRTL = TextAlignmentOptions.MidlineLeft;
				break;
			case TextAlignmentOptions.CaplineRight:
				alignRTL = TextAlignmentOptions.CaplineLeft;
				break;
			case TextAlignmentOptions.TopLeft:
				alignRTL = TextAlignmentOptions.TopRight;
				break;
			case TextAlignmentOptions.Left:
				alignRTL = TextAlignmentOptions.Right;
				break;
			case TextAlignmentOptions.BottomLeft:
				alignRTL = TextAlignmentOptions.BottomRight;
				break;
			case TextAlignmentOptions.BaselineLeft:
				alignRTL = TextAlignmentOptions.BaselineRight;
				break;
			case TextAlignmentOptions.MidlineLeft:
				alignRTL = TextAlignmentOptions.MidlineRight;
				break;
			case TextAlignmentOptions.CaplineLeft:
				alignRTL = TextAlignmentOptions.CaplineRight;
				break;
			case TextAlignmentOptions.Top:
			case TextAlignmentOptions.TopJustified:
			case TextAlignmentOptions.Center:
			case TextAlignmentOptions.Justified:
			case TextAlignmentOptions.Bottom:
			case TextAlignmentOptions.BottomJustified:
			case TextAlignmentOptions.Baseline:
			case TextAlignmentOptions.BaselineJustified:
			case TextAlignmentOptions.Midline:
			case TextAlignmentOptions.MidlineJustified:
			case TextAlignmentOptions.Capline:
				break;
			}
		}
	}

	internal static void SetFont(TMP_Text label, TMP_FontAsset newFont)
	{
		if ((Object)(object)label.font != (Object)(object)newFont)
		{
			label.font = newFont;
		}
		if ((Object)(object)label != (Object)null)
		{
			SetFont(label, newFont);
		}
	}

	internal static void SetMaterial(TMP_Text label, Material newMat)
	{
		if ((Object)(object)label.fontSharedMaterial != (Object)(object)newMat)
		{
			label.fontSharedMaterial = newMat;
		}
		if ((Object)(object)label != (Object)null)
		{
			SetMaterial(label, newMat);
		}
	}
}
