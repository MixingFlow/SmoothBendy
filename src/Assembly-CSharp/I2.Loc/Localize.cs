using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace I2.Loc;

[AddComponentMenu("I2/Localization/I2 Localize")]
public class Localize : MonoBehaviour
{
	public enum TermModification
	{
		DontModify,
		ToUpper,
		ToLower,
		ToUpperFirst,
		ToTitle
	}

	public string mTerm = string.Empty;

	public string mTermSecondary = string.Empty;

	[NonSerialized]
	public string FinalTerm;

	[NonSerialized]
	public string FinalSecondaryTerm;

	public TermModification PrimaryTermModifier;

	public TermModification SecondaryTermModifier;

	public string TermPrefix;

	public string TermSuffix;

	public bool LocalizeOnAwake = true;

	private string LastLocalizedLanguage;

	public bool IgnoreRTL;

	public int MaxCharactersInRTL;

	public bool IgnoreNumbersInRTL = true;

	public bool CorrectAlignmentForRTL = true;

	public bool AddSpacesToJoinedLanguages;

	public bool AllowLocalizedParameters = true;

	public List<Object> TranslatedObjects = new List<Object>();

	[NonSerialized]
	public Dictionary<string, Object> mAssetDictionary = new Dictionary<string, Object>(StringComparer.Ordinal);

	public UnityEvent LocalizeEvent = new UnityEvent();

	public static string MainTranslation;

	public static string SecondaryTranslation;

	public static string CallBackTerm;

	public static string CallBackSecondaryTerm;

	public static Localize CurrentLocalizeComponent;

	public bool AlwaysForceLocalize;

	[SerializeField]
	public EventCallback LocalizeCallBack = new EventCallback();

	public bool mGUI_ShowReferences;

	public bool mGUI_ShowTems = true;

	public bool mGUI_ShowCallback;

	public ILocalizeTarget mLocalizeTarget;

	public string mLocalizeTargetName;

	public string Term
	{
		get
		{
			return mTerm;
		}
		set
		{
			SetTerm(value);
		}
	}

	public string SecondaryTerm
	{
		get
		{
			return mTermSecondary;
		}
		set
		{
			SetTerm(null, value);
		}
	}

	private void Awake()
	{
		UpdateAssetDictionary();
		FindTarget();
		if (LocalizeOnAwake)
		{
			OnLocalize();
		}
	}

	private void OnEnable()
	{
		OnLocalize();
	}

	public bool HasCallback()
	{
		if (LocalizeCallBack.HasCallback())
		{
			return true;
		}
		return ((UnityEventBase)LocalizeEvent).GetPersistentEventCount() > 0;
	}

	public void OnLocalize(bool Force = false)
	{
		if ((!Force && (!((Behaviour)this).enabled || (Object)(object)((Component)this).gameObject == (Object)null || !((Component)this).gameObject.activeInHierarchy)) || string.IsNullOrEmpty(LocalizationManager.CurrentLanguage) || (!AlwaysForceLocalize && !Force && !HasCallback() && LastLocalizedLanguage == LocalizationManager.CurrentLanguage))
		{
			return;
		}
		LastLocalizedLanguage = LocalizationManager.CurrentLanguage;
		if (string.IsNullOrEmpty(FinalTerm) || string.IsNullOrEmpty(FinalSecondaryTerm))
		{
			GetFinalTerms(out FinalTerm, out FinalSecondaryTerm);
		}
		bool flag = I2Utils.IsPlaying() && HasCallback();
		if (!flag && string.IsNullOrEmpty(FinalTerm) && string.IsNullOrEmpty(FinalSecondaryTerm))
		{
			return;
		}
		CallBackTerm = FinalTerm;
		CallBackSecondaryTerm = FinalSecondaryTerm;
		MainTranslation = ((!string.IsNullOrEmpty(FinalTerm) && !(FinalTerm == "-")) ? LocalizationManager.GetTranslation(FinalTerm, FixForRTL: false) : null);
		SecondaryTranslation = ((!string.IsNullOrEmpty(FinalSecondaryTerm) && !(FinalSecondaryTerm == "-")) ? LocalizationManager.GetTranslation(FinalSecondaryTerm, FixForRTL: false) : null);
		if (!flag && string.IsNullOrEmpty(FinalTerm) && string.IsNullOrEmpty(SecondaryTranslation))
		{
			return;
		}
		CurrentLocalizeComponent = this;
		LocalizeCallBack.Execute((Object)(object)this);
		LocalizeEvent.Invoke();
		LocalizationManager.ApplyLocalizationParams(ref MainTranslation, ((Component)this).gameObject, AllowLocalizedParameters);
		if (!FindTarget())
		{
			return;
		}
		bool flag2 = LocalizationManager.IsRight2Left && !IgnoreRTL;
		if (MainTranslation != null)
		{
			switch (PrimaryTermModifier)
			{
			case TermModification.ToUpper:
				MainTranslation = MainTranslation.ToUpper();
				break;
			case TermModification.ToLower:
				MainTranslation = MainTranslation.ToLower();
				break;
			case TermModification.ToUpperFirst:
				MainTranslation = GoogleTranslation.UppercaseFirst(MainTranslation);
				break;
			case TermModification.ToTitle:
				MainTranslation = GoogleTranslation.TitleCase(MainTranslation);
				break;
			}
			if (!string.IsNullOrEmpty(TermPrefix))
			{
				MainTranslation = ((!flag2) ? (TermPrefix + MainTranslation) : (MainTranslation + TermPrefix));
			}
			if (!string.IsNullOrEmpty(TermSuffix))
			{
				MainTranslation = ((!flag2) ? (MainTranslation + TermSuffix) : (TermSuffix + MainTranslation));
			}
			if (AddSpacesToJoinedLanguages && LocalizationManager.HasJoinedWords && !string.IsNullOrEmpty(MainTranslation))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(MainTranslation[0]);
				int i = 1;
				for (int length = MainTranslation.Length; i < length; i++)
				{
					stringBuilder.Append(' ');
					stringBuilder.Append(MainTranslation[i]);
				}
				MainTranslation = stringBuilder.ToString();
			}
			if (flag2 && mLocalizeTarget.AllowMainTermToBeRTL() && !string.IsNullOrEmpty(MainTranslation))
			{
				MainTranslation = LocalizationManager.ApplyRTLfix(MainTranslation, MaxCharactersInRTL, IgnoreNumbersInRTL);
			}
		}
		if (SecondaryTranslation != null)
		{
			switch (SecondaryTermModifier)
			{
			case TermModification.ToUpper:
				SecondaryTranslation = SecondaryTranslation.ToUpper();
				break;
			case TermModification.ToLower:
				SecondaryTranslation = SecondaryTranslation.ToLower();
				break;
			case TermModification.ToUpperFirst:
				SecondaryTranslation = GoogleTranslation.UppercaseFirst(SecondaryTranslation);
				break;
			case TermModification.ToTitle:
				SecondaryTranslation = GoogleTranslation.TitleCase(SecondaryTranslation);
				break;
			}
			if (flag2 && mLocalizeTarget.AllowSecondTermToBeRTL() && !string.IsNullOrEmpty(SecondaryTranslation))
			{
				SecondaryTranslation = LocalizationManager.ApplyRTLfix(SecondaryTranslation);
			}
		}
		if (LocalizationManager.HighlightLocalizedTargets)
		{
			MainTranslation = "LOC:" + FinalTerm;
		}
		mLocalizeTarget.DoLocalize(this, MainTranslation, SecondaryTranslation);
		CurrentLocalizeComponent = null;
	}

	public bool FindTarget()
	{
		if ((Object)(object)mLocalizeTarget != (Object)null && mLocalizeTarget.IsValid(this))
		{
			return true;
		}
		if ((Object)(object)mLocalizeTarget != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)mLocalizeTarget);
			mLocalizeTarget = null;
			mLocalizeTargetName = null;
		}
		if (!string.IsNullOrEmpty(mLocalizeTargetName))
		{
			foreach (ILocalizeTargetDescriptor mLocalizeTarget in LocalizationManager.mLocalizeTargets)
			{
				if (mLocalizeTargetName == mLocalizeTarget.GetTargetType().ToString())
				{
					if (mLocalizeTarget.CanLocalize(this))
					{
						this.mLocalizeTarget = mLocalizeTarget.CreateTarget(this);
					}
					if ((Object)(object)this.mLocalizeTarget != (Object)null)
					{
						return true;
					}
				}
			}
		}
		foreach (ILocalizeTargetDescriptor mLocalizeTarget2 in LocalizationManager.mLocalizeTargets)
		{
			if (mLocalizeTarget2.CanLocalize(this))
			{
				this.mLocalizeTarget = mLocalizeTarget2.CreateTarget(this);
				mLocalizeTargetName = mLocalizeTarget2.GetTargetType().ToString();
				if ((Object)(object)this.mLocalizeTarget != (Object)null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void GetFinalTerms(out string primaryTerm, out string secondaryTerm)
	{
		primaryTerm = string.Empty;
		secondaryTerm = string.Empty;
		if (FindTarget())
		{
			if ((Object)(object)mLocalizeTarget != (Object)null)
			{
				mLocalizeTarget.GetFinalTerms(this, mTerm, mTermSecondary, out primaryTerm, out secondaryTerm);
				primaryTerm = I2Utils.RemoveNonASCII(primaryTerm);
			}
			if (!string.IsNullOrEmpty(mTerm))
			{
				primaryTerm = mTerm;
			}
			if (!string.IsNullOrEmpty(mTermSecondary))
			{
				secondaryTerm = mTermSecondary;
			}
			if (primaryTerm != null)
			{
				primaryTerm = primaryTerm.Trim();
			}
			if (secondaryTerm != null)
			{
				secondaryTerm = secondaryTerm.Trim();
			}
		}
	}

	public string GetMainTargetsText()
	{
		string primaryTerm = null;
		string secondaryTerm = null;
		if ((Object)(object)mLocalizeTarget != (Object)null)
		{
			mLocalizeTarget.GetFinalTerms(this, null, null, out primaryTerm, out secondaryTerm);
		}
		return (!string.IsNullOrEmpty(primaryTerm)) ? primaryTerm : mTerm;
	}

	public void SetFinalTerms(string Main, string Secondary, out string primaryTerm, out string secondaryTerm, bool RemoveNonASCII)
	{
		primaryTerm = ((!RemoveNonASCII) ? Main : I2Utils.RemoveNonASCII(Main));
		secondaryTerm = Secondary;
	}

	public void SetTerm(string primary)
	{
		if (!string.IsNullOrEmpty(primary))
		{
			FinalTerm = (mTerm = primary);
		}
		OnLocalize(Force: true);
	}

	public void SetTerm(string primary, string secondary)
	{
		if (!string.IsNullOrEmpty(primary))
		{
			FinalTerm = (mTerm = primary);
		}
		FinalSecondaryTerm = (mTermSecondary = secondary);
		OnLocalize(Force: true);
	}

	internal T GetSecondaryTranslatedObj<T>(ref string mainTranslation, ref string secondaryTranslation) where T : Object
	{
		DeserializeTranslation(mainTranslation, out var value, out var secondary);
		T val = (T)(object)null;
		if (!string.IsNullOrEmpty(secondary))
		{
			val = GetObject<T>(secondary);
			if ((Object)(object)val != (Object)null)
			{
				mainTranslation = value;
				secondaryTranslation = secondary;
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			val = GetObject<T>(secondaryTranslation);
		}
		return val;
	}

	public void UpdateAssetDictionary()
	{
		TranslatedObjects.RemoveAll((Object x) => x == (Object)null);
		mAssetDictionary = TranslatedObjects.Distinct().ToDictionary((Object o) => o.name);
		mAssetDictionary = (from o in TranslatedObjects.Distinct()
			group o by o.name).ToDictionary((IGrouping<string, Object> g) => g.Key, Enumerable.First);
	}

	internal T GetObject<T>(string Translation) where T : Object
	{
		if (string.IsNullOrEmpty(Translation))
		{
			return (T)(object)null;
		}
		return GetTranslatedObject<T>(Translation);
	}

	private T GetTranslatedObject<T>(string Translation) where T : Object
	{
		return FindTranslatedObject<T>(Translation);
	}

	private void DeserializeTranslation(string translation, out string value, out string secondary)
	{
		if (!string.IsNullOrEmpty(translation) && translation.Length > 1 && translation[0] == '[')
		{
			int num = translation.IndexOf(']');
			if (num > 0)
			{
				secondary = translation.Substring(1, num - 1);
				value = translation.Substring(num + 1);
				return;
			}
		}
		value = translation;
		secondary = string.Empty;
	}

	public T FindTranslatedObject<T>(string value) where T : Object
	{
		if (string.IsNullOrEmpty(value))
		{
			return (T)(object)null;
		}
		if (mAssetDictionary == null || mAssetDictionary.Count != TranslatedObjects.Count)
		{
			UpdateAssetDictionary();
		}
		foreach (KeyValuePair<string, Object> item in mAssetDictionary)
		{
			if (item.Value is T && value.EndsWith(item.Key, StringComparison.OrdinalIgnoreCase) && string.Compare(value, item.Key, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return (T)(object)item.Value;
			}
		}
		Object obj = LocalizationManager.FindAsset(value);
		T val = (T)(object)((obj is T) ? obj : null);
		if (Object.op_Implicit((Object)(object)val))
		{
			return val;
		}
		return ResourceManager.pInstance.GetAsset<T>(value);
	}

	public bool HasTranslatedObject(Object Obj)
	{
		if (TranslatedObjects.Contains(Obj))
		{
			return true;
		}
		return ResourceManager.pInstance.HasAsset(Obj);
	}

	public void AddTranslatedObject(Object Obj)
	{
		if (!TranslatedObjects.Contains(Obj))
		{
			TranslatedObjects.Add(Obj);
			UpdateAssetDictionary();
		}
	}

	public void SetGlobalLanguage(string Language)
	{
		LocalizationManager.CurrentLanguage = Language;
	}
}
