using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TMPro;

[AddComponentMenu("UI/TMP Dropdown", 35)]
[RequireComponent(typeof(RectTransform))]
public class TMP_Dropdown : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
{
	protected internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, ICancelHandler, IEventSystemHandler
	{
		[SerializeField]
		private TMP_Text m_Text;

		[SerializeField]
		private Image m_Image;

		[SerializeField]
		private RectTransform m_RectTransform;

		[SerializeField]
		private Toggle m_Toggle;

		public TMP_Text text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}

		public Image image
		{
			get
			{
				return m_Image;
			}
			set
			{
				m_Image = value;
			}
		}

		public RectTransform rectTransform
		{
			get
			{
				return m_RectTransform;
			}
			set
			{
				m_RectTransform = value;
			}
		}

		public Toggle toggle
		{
			get
			{
				return m_Toggle;
			}
			set
			{
				m_Toggle = value;
			}
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			EventSystem.current.SetSelectedGameObject(((Component)this).gameObject);
		}

		public virtual void OnCancel(BaseEventData eventData)
		{
			TMP_Dropdown componentInParent = ((Component)this).GetComponentInParent<TMP_Dropdown>();
			if (Object.op_Implicit((Object)(object)componentInParent))
			{
				componentInParent.Hide();
			}
		}
	}

	[Serializable]
	public class OptionData
	{
		[SerializeField]
		private string m_Text;

		[SerializeField]
		private Sprite m_Image;

		public string text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}

		public Sprite image
		{
			get
			{
				return m_Image;
			}
			set
			{
				m_Image = value;
			}
		}

		public OptionData()
		{
		}

		public OptionData(string text)
		{
			this.text = text;
		}

		public OptionData(Sprite image)
		{
			this.image = image;
		}

		public OptionData(string text, Sprite image)
		{
			this.text = text;
			this.image = image;
		}
	}

	[Serializable]
	public class OptionDataList
	{
		[SerializeField]
		private List<OptionData> m_Options;

		public List<OptionData> options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		public OptionDataList()
		{
			options = new List<OptionData>();
		}
	}

	[Serializable]
	public class DropdownEvent : UnityEvent<int>
	{
	}

	[SerializeField]
	private RectTransform m_Template;

	[SerializeField]
	private TMP_Text m_CaptionText;

	[SerializeField]
	private Image m_CaptionImage;

	[Space]
	[SerializeField]
	private TMP_Text m_ItemText;

	[SerializeField]
	private Image m_ItemImage;

	[Space]
	[SerializeField]
	private int m_Value;

	[Space]
	[SerializeField]
	private OptionDataList m_Options = new OptionDataList();

	[Space]
	[SerializeField]
	private DropdownEvent m_OnValueChanged = new DropdownEvent();

	private GameObject m_Dropdown;

	private GameObject m_Blocker;

	private List<DropdownItem> m_Items = new List<DropdownItem>();

	private TweenRunner<FloatTween> m_AlphaTweenRunner;

	private bool validTemplate;

	private static OptionData s_NoOptionData = new OptionData();

	public RectTransform template
	{
		get
		{
			return m_Template;
		}
		set
		{
			m_Template = value;
			RefreshShownValue();
		}
	}

	public TMP_Text captionText
	{
		get
		{
			return m_CaptionText;
		}
		set
		{
			m_CaptionText = value;
			RefreshShownValue();
		}
	}

	public Image captionImage
	{
		get
		{
			return m_CaptionImage;
		}
		set
		{
			m_CaptionImage = value;
			RefreshShownValue();
		}
	}

	public TMP_Text itemText
	{
		get
		{
			return m_ItemText;
		}
		set
		{
			m_ItemText = value;
			RefreshShownValue();
		}
	}

	public Image itemImage
	{
		get
		{
			return m_ItemImage;
		}
		set
		{
			m_ItemImage = value;
			RefreshShownValue();
		}
	}

	public List<OptionData> options
	{
		get
		{
			return m_Options.options;
		}
		set
		{
			m_Options.options = value;
			RefreshShownValue();
		}
	}

	public DropdownEvent onValueChanged
	{
		get
		{
			return m_OnValueChanged;
		}
		set
		{
			m_OnValueChanged = value;
		}
	}

	public int value
	{
		get
		{
			return m_Value;
		}
		set
		{
			if (!Application.isPlaying || (value != m_Value && options.Count != 0))
			{
				m_Value = Mathf.Clamp(value, 0, options.Count - 1);
				RefreshShownValue();
				((UnityEvent<int>)m_OnValueChanged).Invoke(m_Value);
			}
		}
	}

	protected TMP_Dropdown()
	{
	}

	protected override void Awake()
	{
		m_AlphaTweenRunner = new TweenRunner<FloatTween>();
		m_AlphaTweenRunner.Init((MonoBehaviour)(object)this);
		if (Object.op_Implicit((Object)(object)m_CaptionImage))
		{
			((Behaviour)m_CaptionImage).enabled = (Object)(object)m_CaptionImage.sprite != (Object)null;
		}
		if (Object.op_Implicit((Object)(object)m_Template))
		{
			((Component)m_Template).gameObject.SetActive(false);
		}
	}

	public void RefreshShownValue()
	{
		OptionData optionData = s_NoOptionData;
		if (options.Count > 0)
		{
			optionData = options[Mathf.Clamp(m_Value, 0, options.Count - 1)];
		}
		if (Object.op_Implicit((Object)(object)m_CaptionText))
		{
			if (optionData != null && optionData.text != null)
			{
				m_CaptionText.text = optionData.text;
			}
			else
			{
				m_CaptionText.text = string.Empty;
			}
		}
		if (Object.op_Implicit((Object)(object)m_CaptionImage))
		{
			if (optionData != null)
			{
				m_CaptionImage.sprite = optionData.image;
			}
			else
			{
				m_CaptionImage.sprite = null;
			}
			((Behaviour)m_CaptionImage).enabled = (Object)(object)m_CaptionImage.sprite != (Object)null;
		}
	}

	public void AddOptions(List<OptionData> options)
	{
		this.options.AddRange(options);
		RefreshShownValue();
	}

	public void AddOptions(List<string> options)
	{
		for (int i = 0; i < options.Count; i++)
		{
			this.options.Add(new OptionData(options[i]));
		}
		RefreshShownValue();
	}

	public void AddOptions(List<Sprite> options)
	{
		for (int i = 0; i < options.Count; i++)
		{
			this.options.Add(new OptionData(options[i]));
		}
		RefreshShownValue();
	}

	public void ClearOptions()
	{
		options.Clear();
		RefreshShownValue();
	}

	private void SetupTemplate()
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		validTemplate = false;
		if (!Object.op_Implicit((Object)(object)m_Template))
		{
			Debug.LogError((object)"The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", (Object)(object)this);
			return;
		}
		GameObject gameObject = ((Component)m_Template).gameObject;
		gameObject.SetActive(true);
		Toggle componentInChildren = ((Component)m_Template).GetComponentInChildren<Toggle>();
		validTemplate = true;
		if (!Object.op_Implicit((Object)(object)componentInChildren) || (Object)(object)((Component)componentInChildren).transform == (Object)(object)template)
		{
			validTemplate = false;
			Debug.LogError((object)"The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", (Object)(object)template);
		}
		else if (!(((Component)componentInChildren).transform.parent is RectTransform))
		{
			validTemplate = false;
			Debug.LogError((object)"The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", (Object)(object)template);
		}
		else if ((Object)(object)itemText != (Object)null && !itemText.transform.IsChildOf(((Component)componentInChildren).transform))
		{
			validTemplate = false;
			Debug.LogError((object)"The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", (Object)(object)template);
		}
		else if ((Object)(object)itemImage != (Object)null && !((Component)itemImage).transform.IsChildOf(((Component)componentInChildren).transform))
		{
			validTemplate = false;
			Debug.LogError((object)"The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", (Object)(object)template);
		}
		if (!validTemplate)
		{
			gameObject.SetActive(false);
			return;
		}
		DropdownItem dropdownItem = ((Component)componentInChildren).gameObject.AddComponent<DropdownItem>();
		dropdownItem.text = m_ItemText;
		dropdownItem.image = m_ItemImage;
		dropdownItem.toggle = componentInChildren;
		dropdownItem.rectTransform = (RectTransform)((Component)componentInChildren).transform;
		Canvas orAddComponent = GetOrAddComponent<Canvas>(gameObject);
		orAddComponent.overrideSorting = true;
		orAddComponent.sortingOrder = 30000;
		GetOrAddComponent<GraphicRaycaster>(gameObject);
		GetOrAddComponent<CanvasGroup>(gameObject);
		gameObject.SetActive(false);
		validTemplate = true;
	}

	private static T GetOrAddComponent<T>(GameObject go) where T : Component
	{
		T val = go.GetComponent<T>();
		if (!Object.op_Implicit((Object)(object)val))
		{
			val = go.AddComponent<T>();
		}
		return val;
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		Show();
	}

	public virtual void OnSubmit(BaseEventData eventData)
	{
		Show();
	}

	public virtual void OnCancel(BaseEventData eventData)
	{
		Hide();
	}

	public void Show()
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		if (!((UIBehaviour)this).IsActive() || !((Selectable)this).IsInteractable() || (Object)(object)m_Dropdown != (Object)null)
		{
			return;
		}
		if (!validTemplate)
		{
			SetupTemplate();
			if (!validTemplate)
			{
				return;
			}
		}
		List<Canvas> list = TMP_ListPool<Canvas>.Get();
		((Component)this).gameObject.GetComponentsInParent<Canvas>(false, list);
		if (list.Count == 0)
		{
			return;
		}
		Canvas val = list[0];
		TMP_ListPool<Canvas>.Release(list);
		((Component)m_Template).gameObject.SetActive(true);
		m_Dropdown = CreateDropdownList(((Component)m_Template).gameObject);
		((Object)m_Dropdown).name = "Dropdown List";
		m_Dropdown.SetActive(true);
		Transform transform = m_Dropdown.transform;
		RectTransform val2 = (RectTransform)(object)((transform is RectTransform) ? transform : null);
		((Transform)val2).SetParent(((Component)m_Template).transform.parent, false);
		DropdownItem componentInChildren = m_Dropdown.GetComponentInChildren<DropdownItem>();
		GameObject gameObject = ((Component)((Transform)componentInChildren.rectTransform).parent).gameObject;
		Transform transform2 = gameObject.transform;
		RectTransform val3 = (RectTransform)(object)((transform2 is RectTransform) ? transform2 : null);
		((Component)componentInChildren.rectTransform).gameObject.SetActive(true);
		Rect rect = val3.rect;
		Rect rect2 = componentInChildren.rectTransform.rect;
		Vector2 val4 = ((Rect)(ref rect2)).min - ((Rect)(ref rect)).min + Vector2.op_Implicit(((Transform)componentInChildren.rectTransform).localPosition);
		Vector2 val5 = ((Rect)(ref rect2)).max - ((Rect)(ref rect)).max + Vector2.op_Implicit(((Transform)componentInChildren.rectTransform).localPosition);
		Vector2 size = ((Rect)(ref rect2)).size;
		m_Items.Clear();
		Toggle val6 = null;
		for (int i = 0; i < options.Count; i++)
		{
			OptionData data = options[i];
			DropdownItem item = AddItem(data, value == i, componentInChildren, m_Items);
			if (!((Object)(object)item == (Object)null))
			{
				item.toggle.isOn = value == i;
				((UnityEvent<bool>)(object)item.toggle.onValueChanged).AddListener((UnityAction<bool>)delegate
				{
					OnSelectItem(item.toggle);
				});
				if (item.toggle.isOn)
				{
					((Selectable)item.toggle).Select();
				}
				if ((Object)(object)val6 != (Object)null)
				{
					Navigation navigation = ((Selectable)val6).navigation;
					Navigation navigation2 = ((Selectable)item.toggle).navigation;
					((Navigation)(ref navigation)).mode = (Mode)4;
					((Navigation)(ref navigation2)).mode = (Mode)4;
					((Navigation)(ref navigation)).selectOnDown = (Selectable)(object)item.toggle;
					((Navigation)(ref navigation)).selectOnRight = (Selectable)(object)item.toggle;
					((Navigation)(ref navigation2)).selectOnLeft = (Selectable)(object)val6;
					((Navigation)(ref navigation2)).selectOnUp = (Selectable)(object)val6;
					((Selectable)val6).navigation = navigation;
					((Selectable)item.toggle).navigation = navigation2;
				}
				val6 = item.toggle;
			}
		}
		Vector2 sizeDelta = val3.sizeDelta;
		sizeDelta.y = size.y * (float)m_Items.Count + val4.y - val5.y;
		val3.sizeDelta = sizeDelta;
		Rect rect3 = val2.rect;
		float height = ((Rect)(ref rect3)).height;
		Rect rect4 = val3.rect;
		float num = height - ((Rect)(ref rect4)).height;
		if (num > 0f)
		{
			val2.sizeDelta = new Vector2(val2.sizeDelta.x, val2.sizeDelta.y - num);
		}
		Vector3[] array = (Vector3[])(object)new Vector3[4];
		val2.GetWorldCorners(array);
		Transform transform3 = ((Component)val).transform;
		RectTransform val7 = (RectTransform)(object)((transform3 is RectTransform) ? transform3 : null);
		Rect rect5 = val7.rect;
		for (int num2 = 0; num2 < 2; num2++)
		{
			bool flag = false;
			int num3 = 0;
			while (num3 < 4)
			{
				Vector3 val8 = ((Transform)val7).InverseTransformPoint(array[num3]);
				float num4 = ((Vector3)(ref val8))[num2];
				Vector2 min = ((Rect)(ref rect5)).min;
				if (!(num4 < ((Vector2)(ref min))[num2]))
				{
					float num5 = ((Vector3)(ref val8))[num2];
					Vector2 max = ((Rect)(ref rect5)).max;
					if (!(num5 > ((Vector2)(ref max))[num2]))
					{
						num3++;
						continue;
					}
				}
				flag = true;
				break;
			}
			if (flag)
			{
				RectTransformUtility.FlipLayoutOnAxis(val2, num2, false, false);
			}
		}
		for (int num6 = 0; num6 < m_Items.Count; num6++)
		{
			RectTransform rectTransform = m_Items[num6].rectTransform;
			rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0f);
			rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0f);
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, val4.y + size.y * (float)(m_Items.Count - 1 - num6) + size.y * rectTransform.pivot.y);
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size.y);
		}
		AlphaFadeList(0.15f, 0f, 1f);
		((Component)m_Template).gameObject.SetActive(false);
		((Component)componentInChildren).gameObject.SetActive(false);
		m_Blocker = CreateBlocker(val);
	}

	protected virtual GameObject CreateBlocker(Canvas rootCanvas)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		GameObject val = new GameObject("Blocker");
		RectTransform val2 = val.AddComponent<RectTransform>();
		((Transform)val2).SetParent(((Component)rootCanvas).transform, false);
		val2.anchorMin = Vector2.op_Implicit(Vector3.zero);
		val2.anchorMax = Vector2.op_Implicit(Vector3.one);
		val2.sizeDelta = Vector2.zero;
		Canvas val3 = val.AddComponent<Canvas>();
		val3.overrideSorting = true;
		Canvas component = m_Dropdown.GetComponent<Canvas>();
		val3.sortingLayerID = component.sortingLayerID;
		val3.sortingOrder = component.sortingOrder - 1;
		val.AddComponent<GraphicRaycaster>();
		Image val4 = val.AddComponent<Image>();
		((Graphic)val4).color = Color.clear;
		Button val5 = val.AddComponent<Button>();
		((UnityEvent)val5.onClick).AddListener(new UnityAction(Hide));
		return val;
	}

	protected virtual void DestroyBlocker(GameObject blocker)
	{
		Object.Destroy((Object)(object)blocker);
	}

	protected virtual GameObject CreateDropdownList(GameObject template)
	{
		return Object.Instantiate<GameObject>(template);
	}

	protected virtual void DestroyDropdownList(GameObject dropdownList)
	{
		Object.Destroy((Object)(object)dropdownList);
	}

	protected virtual DropdownItem CreateItem(DropdownItem itemTemplate)
	{
		return Object.Instantiate<DropdownItem>(itemTemplate);
	}

	protected virtual void DestroyItem(DropdownItem item)
	{
	}

	private DropdownItem AddItem(OptionData data, bool selected, DropdownItem itemTemplate, List<DropdownItem> items)
	{
		DropdownItem dropdownItem = CreateItem(itemTemplate);
		((Transform)dropdownItem.rectTransform).SetParent(((Transform)itemTemplate.rectTransform).parent, false);
		((Component)dropdownItem).gameObject.SetActive(true);
		((Object)((Component)dropdownItem).gameObject).name = "Item " + items.Count + ((data.text == null) ? string.Empty : (": " + data.text));
		if ((Object)(object)dropdownItem.toggle != (Object)null)
		{
			dropdownItem.toggle.isOn = false;
		}
		if (Object.op_Implicit((Object)(object)dropdownItem.text))
		{
			dropdownItem.text.text = data.text;
		}
		if (Object.op_Implicit((Object)(object)dropdownItem.image))
		{
			dropdownItem.image.sprite = data.image;
			((Behaviour)dropdownItem.image).enabled = (Object)(object)dropdownItem.image.sprite != (Object)null;
		}
		items.Add(dropdownItem);
		return dropdownItem;
	}

	private void AlphaFadeList(float duration, float alpha)
	{
		CanvasGroup component = m_Dropdown.GetComponent<CanvasGroup>();
		AlphaFadeList(duration, component.alpha, alpha);
	}

	private void AlphaFadeList(float duration, float start, float end)
	{
		if (!end.Equals(start))
		{
			FloatTween info = new FloatTween
			{
				duration = duration,
				startValue = start,
				targetValue = end
			};
			info.AddOnChangedCallback(SetAlpha);
			info.ignoreTimeScale = true;
			m_AlphaTweenRunner.StartTween(info);
		}
	}

	private void SetAlpha(float alpha)
	{
		if (Object.op_Implicit((Object)(object)m_Dropdown))
		{
			CanvasGroup component = m_Dropdown.GetComponent<CanvasGroup>();
			component.alpha = alpha;
		}
	}

	public void Hide()
	{
		if ((Object)(object)m_Dropdown != (Object)null)
		{
			AlphaFadeList(0.15f, 0f);
			if (((UIBehaviour)this).IsActive())
			{
				((MonoBehaviour)this).StartCoroutine(DelayedDestroyDropdownList(0.15f));
			}
		}
		if ((Object)(object)m_Blocker != (Object)null)
		{
			DestroyBlocker(m_Blocker);
		}
		m_Blocker = null;
		((Selectable)this).Select();
	}

	private IEnumerator DelayedDestroyDropdownList(float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		for (int i = 0; i < m_Items.Count; i++)
		{
			if ((Object)(object)m_Items[i] != (Object)null)
			{
				DestroyItem(m_Items[i]);
			}
			m_Items.Clear();
		}
		if ((Object)(object)m_Dropdown != (Object)null)
		{
			DestroyDropdownList(m_Dropdown);
		}
		m_Dropdown = null;
	}

	private void OnSelectItem(Toggle toggle)
	{
		if (!toggle.isOn)
		{
			toggle.isOn = true;
		}
		int num = -1;
		Transform transform = ((Component)toggle).transform;
		Transform parent = transform.parent;
		for (int i = 0; i < parent.childCount; i++)
		{
			if ((Object)(object)parent.GetChild(i) == (Object)(object)transform)
			{
				num = i - 1;
				break;
			}
		}
		if (num >= 0)
		{
			value = num;
			Hide();
		}
	}
}
