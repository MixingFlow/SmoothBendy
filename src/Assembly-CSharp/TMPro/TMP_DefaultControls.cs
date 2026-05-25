using UnityEngine;
using UnityEngine.UI;

namespace TMPro;

public static class TMP_DefaultControls
{
	public struct Resources
	{
		public Sprite standard;

		public Sprite background;

		public Sprite inputField;

		public Sprite knob;

		public Sprite checkmark;

		public Sprite dropdown;

		public Sprite mask;
	}

	private const float kWidth = 160f;

	private const float kThickHeight = 30f;

	private const float kThinHeight = 20f;

	private static Vector2 s_ThickElementSize = new Vector2(160f, 30f);

	private static Vector2 s_ThinElementSize = new Vector2(160f, 20f);

	private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);

	private static Color s_TextColor = new Color(10f / 51f, 10f / 51f, 10f / 51f, 1f);

	private static GameObject CreateUIElementRoot(string name, Vector2 size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		RectTransform val2 = val.AddComponent<RectTransform>();
		val2.sizeDelta = size;
		return val;
	}

	private static GameObject CreateUIObject(string name, GameObject parent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		GameObject val = new GameObject(name);
		val.AddComponent<RectTransform>();
		SetParentAndAlign(val, parent);
		return val;
	}

	private static void SetDefaultTextValues(TMP_Text lbl)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)lbl).color = s_TextColor;
		lbl.fontSize = 14f;
	}

	private static void SetDefaultColorTransitionValues(Selectable slider)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ColorBlock colors = slider.colors;
		((ColorBlock)(ref colors)).highlightedColor = new Color(0.882f, 0.882f, 0.882f);
		((ColorBlock)(ref colors)).pressedColor = new Color(0.698f, 0.698f, 0.698f);
		((ColorBlock)(ref colors)).disabledColor = new Color(0.521f, 0.521f, 0.521f);
	}

	private static void SetParentAndAlign(GameObject child, GameObject parent)
	{
		if (!((Object)(object)parent == (Object)null))
		{
			child.transform.SetParent(parent.transform, false);
			SetLayerRecursively(child, parent.layer);
		}
	}

	private static void SetLayerRecursively(GameObject go, int layer)
	{
		go.layer = layer;
		Transform transform = go.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			SetLayerRecursively(((Component)transform.GetChild(i)).gameObject, layer);
		}
	}

	public static GameObject CreateScrollbar(Resources resources)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateUIElementRoot("Scrollbar", s_ThinElementSize);
		GameObject val2 = CreateUIObject("Sliding Area", val);
		GameObject val3 = CreateUIObject("Handle", val2);
		Image val4 = val.AddComponent<Image>();
		val4.sprite = resources.background;
		val4.type = (Type)1;
		((Graphic)val4).color = s_DefaultSelectableColor;
		Image val5 = val3.AddComponent<Image>();
		val5.sprite = resources.standard;
		val5.type = (Type)1;
		((Graphic)val5).color = s_DefaultSelectableColor;
		RectTransform component = val2.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(-20f, -20f);
		component.anchorMin = Vector2.zero;
		component.anchorMax = Vector2.one;
		RectTransform component2 = val3.GetComponent<RectTransform>();
		component2.sizeDelta = new Vector2(20f, 20f);
		Scrollbar val6 = val.AddComponent<Scrollbar>();
		val6.handleRect = component2;
		((Selectable)val6).targetGraphic = (Graphic)(object)val5;
		SetDefaultColorTransitionValues((Selectable)(object)val6);
		return val;
	}

	public static GameObject CreateInputField(Resources resources)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateUIElementRoot("TextMeshPro - InputField", s_ThickElementSize);
		GameObject val2 = CreateUIObject("Text Area", val);
		GameObject val3 = CreateUIObject("Placeholder", val2);
		GameObject val4 = CreateUIObject("Text", val2);
		Image val5 = val.AddComponent<Image>();
		val5.sprite = resources.inputField;
		val5.type = (Type)1;
		((Graphic)val5).color = s_DefaultSelectableColor;
		TMP_InputField tMP_InputField = val.AddComponent<TMP_InputField>();
		SetDefaultColorTransitionValues((Selectable)(object)tMP_InputField);
		val2.AddComponent<RectMask2D>();
		RectTransform component = val2.GetComponent<RectTransform>();
		component.anchorMin = Vector2.zero;
		component.anchorMax = Vector2.one;
		component.sizeDelta = Vector2.zero;
		component.offsetMin = new Vector2(10f, 6f);
		component.offsetMax = new Vector2(-10f, -7f);
		TextMeshProUGUI textMeshProUGUI = val4.AddComponent<TextMeshProUGUI>();
		textMeshProUGUI.text = string.Empty;
		textMeshProUGUI.enableWordWrapping = false;
		textMeshProUGUI.extraPadding = true;
		textMeshProUGUI.richText = true;
		SetDefaultTextValues(textMeshProUGUI);
		TextMeshProUGUI textMeshProUGUI2 = val3.AddComponent<TextMeshProUGUI>();
		textMeshProUGUI2.text = "Enter text...";
		textMeshProUGUI2.fontSize = 14f;
		textMeshProUGUI2.fontStyle = FontStyles.Italic;
		textMeshProUGUI2.enableWordWrapping = false;
		textMeshProUGUI2.extraPadding = true;
		Color color = ((Graphic)textMeshProUGUI).color;
		color.a *= 0.5f;
		((Graphic)textMeshProUGUI2).color = color;
		RectTransform component2 = val4.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		component2.sizeDelta = Vector2.zero;
		component2.offsetMin = new Vector2(0f, 0f);
		component2.offsetMax = new Vector2(0f, 0f);
		RectTransform component3 = val3.GetComponent<RectTransform>();
		component3.anchorMin = Vector2.zero;
		component3.anchorMax = Vector2.one;
		component3.sizeDelta = Vector2.zero;
		component3.offsetMin = new Vector2(0f, 0f);
		component3.offsetMax = new Vector2(0f, 0f);
		tMP_InputField.textViewport = component;
		tMP_InputField.textComponent = textMeshProUGUI;
		tMP_InputField.placeholder = (Graphic)(object)textMeshProUGUI2;
		return val;
	}

	public static GameObject CreateDropdown(Resources resources)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateUIElementRoot("Dropdown", s_ThickElementSize);
		GameObject val2 = CreateUIObject("Label", val);
		GameObject val3 = CreateUIObject("Arrow", val);
		GameObject val4 = CreateUIObject("Template", val);
		GameObject val5 = CreateUIObject("Viewport", val4);
		GameObject val6 = CreateUIObject("Content", val5);
		GameObject val7 = CreateUIObject("Item", val6);
		GameObject val8 = CreateUIObject("Item Background", val7);
		GameObject val9 = CreateUIObject("Item Checkmark", val7);
		GameObject val10 = CreateUIObject("Item Label", val7);
		GameObject val11 = CreateScrollbar(resources);
		((Object)val11).name = "Scrollbar";
		SetParentAndAlign(val11, val4);
		Scrollbar component = val11.GetComponent<Scrollbar>();
		component.SetDirection((Direction)2, true);
		RectTransform component2 = val11.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.right;
		component2.anchorMax = Vector2.one;
		component2.pivot = Vector2.one;
		component2.sizeDelta = new Vector2(component2.sizeDelta.x, 0f);
		TextMeshProUGUI textMeshProUGUI = val10.AddComponent<TextMeshProUGUI>();
		SetDefaultTextValues(textMeshProUGUI);
		textMeshProUGUI.alignment = TextAlignmentOptions.Left;
		Image val12 = val8.AddComponent<Image>();
		((Graphic)val12).color = Color32.op_Implicit(new Color32((byte)245, (byte)245, (byte)245, byte.MaxValue));
		Image val13 = val9.AddComponent<Image>();
		val13.sprite = resources.checkmark;
		Toggle val14 = val7.AddComponent<Toggle>();
		((Selectable)val14).targetGraphic = (Graphic)(object)val12;
		val14.graphic = (Graphic)(object)val13;
		val14.isOn = true;
		Image val15 = val4.AddComponent<Image>();
		val15.sprite = resources.standard;
		val15.type = (Type)1;
		ScrollRect val16 = val4.AddComponent<ScrollRect>();
		val16.content = (RectTransform)val6.transform;
		val16.viewport = (RectTransform)val5.transform;
		val16.horizontal = false;
		val16.movementType = (MovementType)2;
		val16.verticalScrollbar = component;
		val16.verticalScrollbarVisibility = (ScrollbarVisibility)2;
		val16.verticalScrollbarSpacing = -3f;
		Mask val17 = val5.AddComponent<Mask>();
		val17.showMaskGraphic = false;
		Image val18 = val5.AddComponent<Image>();
		val18.sprite = resources.mask;
		val18.type = (Type)1;
		TextMeshProUGUI textMeshProUGUI2 = val2.AddComponent<TextMeshProUGUI>();
		SetDefaultTextValues(textMeshProUGUI2);
		textMeshProUGUI2.alignment = TextAlignmentOptions.Left;
		Image val19 = val3.AddComponent<Image>();
		val19.sprite = resources.dropdown;
		Image val20 = val.AddComponent<Image>();
		val20.sprite = resources.standard;
		((Graphic)val20).color = s_DefaultSelectableColor;
		val20.type = (Type)1;
		TMP_Dropdown tMP_Dropdown = val.AddComponent<TMP_Dropdown>();
		((Selectable)tMP_Dropdown).targetGraphic = (Graphic)(object)val20;
		SetDefaultColorTransitionValues((Selectable)(object)tMP_Dropdown);
		tMP_Dropdown.template = val4.GetComponent<RectTransform>();
		tMP_Dropdown.captionText = textMeshProUGUI2;
		tMP_Dropdown.itemText = textMeshProUGUI;
		textMeshProUGUI.text = "Option A";
		tMP_Dropdown.options.Add(new TMP_Dropdown.OptionData
		{
			text = "Option A"
		});
		tMP_Dropdown.options.Add(new TMP_Dropdown.OptionData
		{
			text = "Option B"
		});
		tMP_Dropdown.options.Add(new TMP_Dropdown.OptionData
		{
			text = "Option C"
		});
		tMP_Dropdown.RefreshShownValue();
		RectTransform component3 = val2.GetComponent<RectTransform>();
		component3.anchorMin = Vector2.zero;
		component3.anchorMax = Vector2.one;
		component3.offsetMin = new Vector2(10f, 6f);
		component3.offsetMax = new Vector2(-25f, -7f);
		RectTransform component4 = val3.GetComponent<RectTransform>();
		component4.anchorMin = new Vector2(1f, 0.5f);
		component4.anchorMax = new Vector2(1f, 0.5f);
		component4.sizeDelta = new Vector2(20f, 20f);
		component4.anchoredPosition = new Vector2(-15f, 0f);
		RectTransform component5 = val4.GetComponent<RectTransform>();
		component5.anchorMin = new Vector2(0f, 0f);
		component5.anchorMax = new Vector2(1f, 0f);
		component5.pivot = new Vector2(0.5f, 1f);
		component5.anchoredPosition = new Vector2(0f, 2f);
		component5.sizeDelta = new Vector2(0f, 150f);
		RectTransform component6 = val5.GetComponent<RectTransform>();
		component6.anchorMin = new Vector2(0f, 0f);
		component6.anchorMax = new Vector2(1f, 1f);
		component6.sizeDelta = new Vector2(-18f, 0f);
		component6.pivot = new Vector2(0f, 1f);
		RectTransform component7 = val6.GetComponent<RectTransform>();
		component7.anchorMin = new Vector2(0f, 1f);
		component7.anchorMax = new Vector2(1f, 1f);
		component7.pivot = new Vector2(0.5f, 1f);
		component7.anchoredPosition = new Vector2(0f, 0f);
		component7.sizeDelta = new Vector2(0f, 28f);
		RectTransform component8 = val7.GetComponent<RectTransform>();
		component8.anchorMin = new Vector2(0f, 0.5f);
		component8.anchorMax = new Vector2(1f, 0.5f);
		component8.sizeDelta = new Vector2(0f, 20f);
		RectTransform component9 = val8.GetComponent<RectTransform>();
		component9.anchorMin = Vector2.zero;
		component9.anchorMax = Vector2.one;
		component9.sizeDelta = Vector2.zero;
		RectTransform component10 = val9.GetComponent<RectTransform>();
		component10.anchorMin = new Vector2(0f, 0.5f);
		component10.anchorMax = new Vector2(0f, 0.5f);
		component10.sizeDelta = new Vector2(20f, 20f);
		component10.anchoredPosition = new Vector2(10f, 0f);
		RectTransform component11 = val10.GetComponent<RectTransform>();
		component11.anchorMin = Vector2.zero;
		component11.anchorMax = Vector2.one;
		component11.offsetMin = new Vector2(20f, 1f);
		component11.offsetMax = new Vector2(-10f, -2f);
		val4.SetActive(false);
		return val;
	}
}
