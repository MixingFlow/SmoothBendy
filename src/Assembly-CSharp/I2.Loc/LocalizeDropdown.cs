using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc;

[AddComponentMenu("I2/Localization/Localize Dropdown")]
public class LocalizeDropdown : MonoBehaviour
{
	public List<string> _Terms = new List<string>();

	public void Start()
	{
		LocalizationManager.OnLocalizeEvent += OnLocalize;
		OnLocalize();
	}

	public void OnDestroy()
	{
		LocalizationManager.OnLocalizeEvent -= OnLocalize;
	}

	private void OnEnable()
	{
		if (_Terms.Count == 0)
		{
			FillValues();
		}
		OnLocalize();
	}

	public void OnLocalize()
	{
		if (((Behaviour)this).enabled && !((Object)(object)((Component)this).gameObject == (Object)null) && ((Component)this).gameObject.activeInHierarchy && !string.IsNullOrEmpty(LocalizationManager.CurrentLanguage))
		{
			UpdateLocalization();
		}
	}

	private void FillValues()
	{
		Dropdown component = ((Component)this).GetComponent<Dropdown>();
		if ((Object)(object)component == (Object)null && I2Utils.IsPlaying())
		{
			FillValuesTMPro();
			return;
		}
		foreach (OptionData option in component.options)
		{
			_Terms.Add(option.text);
		}
	}

	public void UpdateLocalization()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		Dropdown component = ((Component)this).GetComponent<Dropdown>();
		if ((Object)(object)component == (Object)null)
		{
			UpdateLocalizationTMPro();
			return;
		}
		component.options.Clear();
		foreach (string term in _Terms)
		{
			string translation = LocalizationManager.GetTranslation(term);
			component.options.Add(new OptionData(translation));
		}
		component.RefreshShownValue();
	}

	public void UpdateLocalizationTMPro()
	{
		TMP_Dropdown component = ((Component)this).GetComponent<TMP_Dropdown>();
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		component.options.Clear();
		foreach (string term in _Terms)
		{
			string translation = LocalizationManager.GetTranslation(term);
			component.options.Add(new TMP_Dropdown.OptionData(translation));
		}
		component.RefreshShownValue();
	}

	private void FillValuesTMPro()
	{
		TMP_Dropdown component = ((Component)this).GetComponent<TMP_Dropdown>();
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		foreach (TMP_Dropdown.OptionData option in component.options)
		{
			_Terms.Add(option.text);
		}
	}
}
