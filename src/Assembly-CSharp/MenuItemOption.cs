using System;
using I2.Loc;
using TMG.Core;
using TMG.UI.Controls;
using TMPro;
using UnityEngine;

public class MenuItemOption : TMGMonoBehaviour
{
	private const float PADDING = 10f;

	private const float LABEL_WIDTH_MAX = 281f;

	private const float STATUS_WIDTH_MAX = 147.9f;

	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private TextMeshProUGUI m_Status;

	[SerializeField]
	private RectTransform m_LeftArrow;

	[SerializeField]
	private RectTransform m_RightArrow;

	[SerializeField]
	private BaseUIButton m_LeftBtn;

	[SerializeField]
	private BaseUIButton m_RightBtn;

	private BaseUIButton m_Button;

	private float m_ArrowWidth;

	private bool m_HasTranslationStatus;

	public event EventHandler OnLeft;

	public event EventHandler OnRight;

	public void Init(Transform parent, string label, string status, bool hasTranslationStatus)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		SetParentAndAlignWithScale(parent);
		m_ArrowWidth = m_LeftArrow.sizeDelta.x;
		m_LeftArrow.anchoredPosition = Vector2.zero;
		m_RightArrow.anchoredPosition = Vector2.zero;
		m_Label.rectTransform.anchoredPosition = Vector2.zero;
		m_Status.rectTransform.anchoredPosition = Vector2.zero;
		string Translation = label;
		if (LocalizationManager.TryGetTranslation(label, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			label = Translation;
		}
		m_Label.text = label.ToUpper() + ((!label.Contains(":")) ? ":" : string.Empty);
		m_HasTranslationStatus = hasTranslationStatus;
		if (m_HasTranslationStatus)
		{
			string Translation2 = status;
			if (LocalizationManager.TryGetTranslation(status, out Translation2, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
			{
				status = Translation2;
			}
		}
		m_Status.text = status.ToUpper();
		SetOptionAnchor(ref m_Label, ref m_Status);
		m_LeftBtn.OnClick += HandleLeftBtnOnClick;
		m_RightBtn.OnClick += HandleRightBtnOnClick;
		((Component)m_LeftArrow).gameObject.SetActive(false);
		((Component)m_RightArrow).gameObject.SetActive(false);
		m_Button = base.gameObject.GetComponent<BaseUIButton>();
		m_Button.OnEnter += HandleButtonOnEnter;
		m_Button.OnExit += HandleButtonOnExit;
	}

	public void UpdateValue(string value, bool isLanguage = false)
	{
		string text = value;
		if (m_HasTranslationStatus)
		{
			if (isLanguage)
			{
				LocalizationManager.CurrentLanguageCode = value;
				GameManager.Instance.PlayerSettings.Language = value;
				text = LocalizationManager.GetLanguageFromCode(value);
			}
			string Translation = text;
			if (LocalizationManager.TryGetTranslation(text, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
			{
				text = Translation;
			}
			text = text.ToUpper();
			if (isLanguage)
			{
				LocalizationManager.UpdateSources();
				LocalizationManager.LocalizeAll(Force: true);
				LocalizationManager.InitializeIfNeeded();
			}
		}
		m_Status.text = text;
	}

	public void HandleButtonOnEnter(object sender, EventArgs e)
	{
		((Component)m_LeftArrow).gameObject.SetActive(true);
		((Component)m_RightArrow).gameObject.SetActive(true);
	}

	public void HandleButtonOnExit(object sender, EventArgs e)
	{
		((Component)m_LeftArrow).gameObject.SetActive(false);
		((Component)m_RightArrow).gameObject.SetActive(false);
	}

	private void HandleLeftBtnOnClick(object sender, EventArgs e)
	{
		PressLeftArrow();
	}

	public void PressLeftArrow()
	{
		this.OnLeft.Send(this);
	}

	private void HandleRightBtnOnClick(object sender, EventArgs e)
	{
		PressRightArrow();
	}

	public void PressRightArrow()
	{
		this.OnRight.Send(this);
	}

	public void SetOptionAnchor(ref TextMeshProUGUI label, ref TextMeshProUGUI status)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		label.ForceMeshUpdate();
		Vector2 sizeDelta = label.rectTransform.sizeDelta;
		sizeDelta.x = label.renderedWidth;
		label.rectTransform.sizeDelta = sizeDelta;
		label.ForceMeshUpdate();
		status.ForceMeshUpdate();
		Vector2 sizeDelta2 = status.rectTransform.sizeDelta;
		float num = (sizeDelta2.x = Mathf.Clamp(status.renderedWidth + 20f, status.renderedWidth + 20f, 147.9f));
		status.rectTransform.sizeDelta = sizeDelta2;
		status.ForceMeshUpdate();
		float num2 = label.renderedWidth / 2f;
		float num3 = num / 2f;
		label.rectTransform.anchoredPosition = new Vector2(0f - num3 - 10f, 0f);
		status.rectTransform.anchoredPosition = Vector2.op_Implicit(new Vector3(num2, 0f));
		float num4 = num2 + num3 + m_ArrowWidth + 10f;
		m_LeftArrow.anchoredPosition = new Vector2(0f - num4, 0f);
		m_RightArrow.anchoredPosition = new Vector2(num4 - 10f, 0f);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
