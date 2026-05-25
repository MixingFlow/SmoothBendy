using UnityEngine;

namespace I2.Loc;

public class Example_LocalizedString : MonoBehaviour
{
	public LocalizedString _MyLocalizedString;

	public string _NormalString;

	[TermsPopup("")]
	public string _StringWithTermPopup;

	public void Start()
	{
		Debug.Log((object)_MyLocalizedString);
		Debug.Log((object)LocalizationManager.GetTranslation(_NormalString));
		Debug.Log((object)LocalizationManager.GetTranslation(_StringWithTermPopup));
		LocalizedString localizedString = "Term2";
		string text = localizedString;
		Debug.Log((object)text);
		LocalizedString myLocalizedString = _MyLocalizedString;
		Debug.Log((object)myLocalizedString);
		LocalizedString localizedString2 = "Term3";
		Debug.Log((object)localizedString2);
		LocalizedString localizedString3 = "Term3";
		localizedString3.mRTL_IgnoreArabicFix = true;
		Debug.Log((object)localizedString3);
		LocalizedString localizedString4 = "Term3";
		localizedString4.mRTL_ConvertNumbers = true;
		localizedString4.mRTL_MaxLineLength = 20;
		Debug.Log((object)localizedString4);
		LocalizedString localizedString5 = localizedString4;
		Debug.Log((object)localizedString5);
	}
}
