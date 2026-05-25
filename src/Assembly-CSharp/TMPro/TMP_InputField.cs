using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TMPro;

[AddComponentMenu("UI/TextMeshPro - Input Field", 11)]
public class TMP_InputField : Selectable, IUpdateSelectedHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement, IEventSystemHandler
{
	public enum ContentType
	{
		Standard,
		Autocorrected,
		IntegerNumber,
		DecimalNumber,
		Alphanumeric,
		Name,
		EmailAddress,
		Password,
		Pin,
		Custom
	}

	public enum InputType
	{
		Standard,
		AutoCorrect,
		Password
	}

	public enum CharacterValidation
	{
		None,
		Integer,
		Decimal,
		Alphanumeric,
		Name,
		EmailAddress
	}

	public enum LineType
	{
		SingleLine,
		MultiLineSubmit,
		MultiLineNewline
	}

	public delegate char OnValidateInput(string text, int charIndex, char addedChar);

	[Serializable]
	public class SubmitEvent : UnityEvent<string>
	{
	}

	[Serializable]
	public class OnChangeEvent : UnityEvent<string>
	{
	}

	protected enum EditState
	{
		Continue,
		Finish
	}

	protected TouchScreenKeyboard m_Keyboard;

	private static readonly char[] kSeparators = new char[6] { ' ', '.', ',', '\t', '\r', '\n' };

	[SerializeField]
	protected RectTransform m_TextViewport;

	[SerializeField]
	protected TMP_Text m_TextComponent;

	protected RectTransform m_TextComponentRectTransform;

	[SerializeField]
	protected Graphic m_Placeholder;

	[SerializeField]
	private ContentType m_ContentType;

	[SerializeField]
	private InputType m_InputType;

	[SerializeField]
	private char m_AsteriskChar = '*';

	[SerializeField]
	private TouchScreenKeyboardType m_KeyboardType;

	[SerializeField]
	private LineType m_LineType;

	[SerializeField]
	private bool m_HideMobileInput;

	[SerializeField]
	private CharacterValidation m_CharacterValidation;

	[SerializeField]
	private int m_CharacterLimit;

	[SerializeField]
	private SubmitEvent m_OnEndEdit = new SubmitEvent();

	[SerializeField]
	private SubmitEvent m_OnSubmit = new SubmitEvent();

	[SerializeField]
	private SubmitEvent m_OnFocusLost = new SubmitEvent();

	[SerializeField]
	private OnChangeEvent m_OnValueChanged = new OnChangeEvent();

	[SerializeField]
	private OnValidateInput m_OnValidateInput;

	[SerializeField]
	private Color m_CaretColor = new Color(10f / 51f, 10f / 51f, 10f / 51f, 1f);

	[SerializeField]
	private bool m_CustomCaretColor;

	[SerializeField]
	private Color m_SelectionColor = new Color(56f / 85f, 0.80784315f, 1f, 64f / 85f);

	[SerializeField]
	protected string m_Text = string.Empty;

	[SerializeField]
	[Range(0f, 4f)]
	private float m_CaretBlinkRate = 0.85f;

	[SerializeField]
	[Range(1f, 5f)]
	private int m_CaretWidth = 1;

	[SerializeField]
	private bool m_ReadOnly;

	[SerializeField]
	private bool m_RichText = true;

	protected int m_StringPosition;

	protected int m_StringSelectPosition;

	protected int m_CaretPosition;

	protected int m_CaretSelectPosition;

	private RectTransform caretRectTrans;

	protected UIVertex[] m_CursorVerts;

	private CanvasRenderer m_CachedInputRenderer;

	[NonSerialized]
	protected Mesh m_Mesh;

	private bool m_AllowInput;

	private bool m_HasLostFocus;

	private bool m_ShouldActivateNextUpdate;

	private bool m_UpdateDrag;

	private bool m_DragPositionOutOfBounds;

	private const float kHScrollSpeed = 0.05f;

	private const float kVScrollSpeed = 0.1f;

	protected bool m_CaretVisible;

	private Coroutine m_BlinkCoroutine;

	private float m_BlinkStartTime;

	protected int m_DrawStart;

	protected int m_DrawEnd;

	private Coroutine m_DragCoroutine;

	private string m_OriginalText = string.Empty;

	private bool m_WasCanceled;

	private bool m_HasDoneFocusTransition;

	private bool m_isLastKeyBackspace;

	private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

	private bool isCaretInsideTag;

	private Event m_ProcessingEvent = new Event();

	protected Mesh mesh
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if ((Object)(object)m_Mesh == (Object)null)
			{
				m_Mesh = new Mesh();
			}
			return m_Mesh;
		}
	}

	public bool shouldHideMobileInput
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			RuntimePlatform platform = Application.platform;
			if ((int)platform == 11 || (int)platform == 8)
			{
				return m_HideMobileInput;
			}
			return true;
		}
		set
		{
			SetPropertyUtility.SetStruct(ref m_HideMobileInput, value);
		}
	}

	public string text
	{
		get
		{
			return m_Text;
		}
		set
		{
			if (!(text == value))
			{
				m_Text = value;
				if (m_Keyboard != null)
				{
					m_Keyboard.text = m_Text;
				}
				if (m_StringPosition > m_Text.Length)
				{
					m_StringPosition = (m_StringSelectPosition = m_Text.Length);
				}
				SendOnValueChangedAndUpdateLabel();
			}
		}
	}

	public bool isFocused => m_AllowInput;

	public float caretBlinkRate
	{
		get
		{
			return m_CaretBlinkRate;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_CaretBlinkRate, value) && m_AllowInput)
			{
				SetCaretActive();
			}
		}
	}

	public int caretWidth
	{
		get
		{
			return m_CaretWidth;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_CaretWidth, value))
			{
				MarkGeometryAsDirty();
			}
		}
	}

	public RectTransform textViewport
	{
		get
		{
			return m_TextViewport;
		}
		set
		{
			SetPropertyUtility.SetClass(ref m_TextViewport, value);
		}
	}

	public TMP_Text textComponent
	{
		get
		{
			return m_TextComponent;
		}
		set
		{
			SetPropertyUtility.SetClass(ref m_TextComponent, value);
		}
	}

	public Graphic placeholder
	{
		get
		{
			return m_Placeholder;
		}
		set
		{
			SetPropertyUtility.SetClass(ref m_Placeholder, value);
		}
	}

	public Color caretColor
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return (!customCaretColor) ? ((Graphic)textComponent).color : m_CaretColor;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (SetPropertyUtility.SetColor(ref m_CaretColor, value))
			{
				MarkGeometryAsDirty();
			}
		}
	}

	public bool customCaretColor
	{
		get
		{
			return m_CustomCaretColor;
		}
		set
		{
			if (m_CustomCaretColor != value)
			{
				m_CustomCaretColor = value;
				MarkGeometryAsDirty();
			}
		}
	}

	public Color selectionColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_SelectionColor;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (SetPropertyUtility.SetColor(ref m_SelectionColor, value))
			{
				MarkGeometryAsDirty();
			}
		}
	}

	public SubmitEvent onEndEdit
	{
		get
		{
			return m_OnEndEdit;
		}
		set
		{
			SetPropertyUtility.SetClass(ref m_OnEndEdit, value);
		}
	}

	public SubmitEvent onSubmit
	{
		get
		{
			return m_OnSubmit;
		}
		set
		{
			SetPropertyUtility.SetClass(ref m_OnSubmit, value);
		}
	}

	public SubmitEvent onFocusLost
	{
		get
		{
			return m_OnFocusLost;
		}
		set
		{
			SetPropertyUtility.SetClass(ref m_OnFocusLost, value);
		}
	}

	public OnChangeEvent onValueChanged
	{
		get
		{
			return m_OnValueChanged;
		}
		set
		{
			SetPropertyUtility.SetClass(ref m_OnValueChanged, value);
		}
	}

	public OnValidateInput onValidateInput
	{
		get
		{
			return m_OnValidateInput;
		}
		set
		{
			SetPropertyUtility.SetClass(ref m_OnValidateInput, value);
		}
	}

	public int characterLimit
	{
		get
		{
			return m_CharacterLimit;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_CharacterLimit, Math.Max(0, value)))
			{
				UpdateLabel();
			}
		}
	}

	public ContentType contentType
	{
		get
		{
			return m_ContentType;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_ContentType, value))
			{
				EnforceContentType();
			}
		}
	}

	public LineType lineType
	{
		get
		{
			return m_LineType;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_LineType, value))
			{
				SetTextComponentWrapMode();
			}
			SetToCustomIfContentTypeIsNot(ContentType.Standard, ContentType.Autocorrected);
		}
	}

	public InputType inputType
	{
		get
		{
			return m_InputType;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_InputType, value))
			{
				SetToCustom();
			}
		}
	}

	public TouchScreenKeyboardType keyboardType
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_KeyboardType;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref m_KeyboardType, value))
			{
				SetToCustom();
			}
		}
	}

	public CharacterValidation characterValidation
	{
		get
		{
			return m_CharacterValidation;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_CharacterValidation, value))
			{
				SetToCustom();
			}
		}
	}

	public bool readOnly
	{
		get
		{
			return m_ReadOnly;
		}
		set
		{
			m_ReadOnly = value;
		}
	}

	public bool richText
	{
		get
		{
			return m_RichText;
		}
		set
		{
			m_RichText = value;
			SetTextComponentRichTextMode();
		}
	}

	public bool multiLine => m_LineType == LineType.MultiLineNewline || lineType == LineType.MultiLineSubmit;

	public char asteriskChar
	{
		get
		{
			return m_AsteriskChar;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_AsteriskChar, value))
			{
				UpdateLabel();
			}
		}
	}

	public bool wasCanceled => m_WasCanceled;

	protected int caretPositionInternal
	{
		get
		{
			return m_CaretPosition + Input.compositionString.Length;
		}
		set
		{
			m_CaretPosition = value;
			ClampPos(ref m_CaretPosition);
		}
	}

	protected int stringPositionInternal
	{
		get
		{
			return m_StringPosition + Input.compositionString.Length;
		}
		set
		{
			m_StringPosition = value;
			ClampPos(ref m_StringPosition);
		}
	}

	protected int caretSelectPositionInternal
	{
		get
		{
			return m_CaretSelectPosition + Input.compositionString.Length;
		}
		set
		{
			m_CaretSelectPosition = value;
			ClampPos(ref m_CaretSelectPosition);
		}
	}

	protected int stringSelectPositionInternal
	{
		get
		{
			return m_StringSelectPosition + Input.compositionString.Length;
		}
		set
		{
			m_StringSelectPosition = value;
			ClampPos(ref m_StringSelectPosition);
		}
	}

	private bool hasSelection => stringPositionInternal != stringSelectPositionInternal;

	public int caretPosition
	{
		get
		{
			return m_StringSelectPosition + Input.compositionString.Length;
		}
		set
		{
			selectionAnchorPosition = value;
			selectionFocusPosition = value;
		}
	}

	public int selectionAnchorPosition
	{
		get
		{
			m_StringPosition = GetStringIndexFromCaretPosition(m_CaretPosition);
			return m_StringPosition + Input.compositionString.Length;
		}
		set
		{
			if (Input.compositionString.Length == 0)
			{
				m_CaretPosition = value;
				ClampPos(ref m_CaretPosition);
			}
		}
	}

	public int selectionFocusPosition
	{
		get
		{
			m_StringSelectPosition = GetStringIndexFromCaretPosition(m_CaretSelectPosition);
			return m_StringSelectPosition + Input.compositionString.Length;
		}
		set
		{
			if (Input.compositionString.Length == 0)
			{
				m_CaretSelectPosition = value;
				ClampPos(ref m_CaretSelectPosition);
			}
		}
	}

	private static string clipboard
	{
		get
		{
			return GUIUtility.systemCopyBuffer;
		}
		set
		{
			GUIUtility.systemCopyBuffer = value;
		}
	}

	protected TMP_InputField()
	{
	}//IL_0049: Unknown result type (might be due to invalid IL or missing references)
	//IL_004e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0068: Unknown result type (might be due to invalid IL or missing references)
	//IL_006d: Unknown result type (might be due to invalid IL or missing references)
	//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
	//IL_00ac: Expected O, but got Unknown


	protected void ClampPos(ref int pos)
	{
		if (pos < 0)
		{
			pos = 0;
		}
		else if (pos > text.Length)
		{
			pos = text.Length;
		}
	}

	protected override void OnEnable()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		((Selectable)this).OnEnable();
		if (m_Text == null)
		{
			m_Text = string.Empty;
		}
		m_DrawStart = 0;
		m_DrawEnd = m_Text.Length;
		if ((Object)(object)m_CachedInputRenderer != (Object)null)
		{
			m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, (Texture)(object)Texture2D.whiteTexture);
		}
		if ((Object)(object)m_TextComponent != (Object)null)
		{
			((Graphic)m_TextComponent).RegisterDirtyVerticesCallback(new UnityAction(MarkGeometryAsDirty));
			((Graphic)m_TextComponent).RegisterDirtyVerticesCallback(new UnityAction(UpdateLabel));
			UpdateLabel();
		}
	}

	protected override void OnDisable()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		m_BlinkCoroutine = null;
		DeactivateInputField();
		if ((Object)(object)m_TextComponent != (Object)null)
		{
			((Graphic)m_TextComponent).UnregisterDirtyVerticesCallback(new UnityAction(MarkGeometryAsDirty));
			((Graphic)m_TextComponent).UnregisterDirtyVerticesCallback(new UnityAction(UpdateLabel));
		}
		CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild((ICanvasElement)(object)this);
		if ((Object)(object)m_CachedInputRenderer != (Object)null)
		{
			m_CachedInputRenderer.Clear();
		}
		if ((Object)(object)m_Mesh != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)m_Mesh);
		}
		m_Mesh = null;
		((Selectable)this).OnDisable();
	}

	private IEnumerator CaretBlink()
	{
		m_CaretVisible = true;
		yield return null;
		while (isFocused && m_CaretBlinkRate > 0f)
		{
			float blinkPeriod = 1f / m_CaretBlinkRate;
			bool blinkState = (Time.unscaledTime - m_BlinkStartTime) % blinkPeriod < blinkPeriod / 2f;
			if (m_CaretVisible != blinkState)
			{
				m_CaretVisible = blinkState;
				if (!hasSelection)
				{
					MarkGeometryAsDirty();
				}
			}
			yield return null;
		}
		m_BlinkCoroutine = null;
	}

	private void SetCaretVisible()
	{
		if (m_AllowInput)
		{
			m_CaretVisible = true;
			m_BlinkStartTime = Time.unscaledTime;
			SetCaretActive();
		}
	}

	private void SetCaretActive()
	{
		if (!m_AllowInput)
		{
			return;
		}
		if (m_CaretBlinkRate > 0f)
		{
			if (m_BlinkCoroutine == null)
			{
				m_BlinkCoroutine = ((MonoBehaviour)this).StartCoroutine(CaretBlink());
			}
		}
		else
		{
			m_CaretVisible = true;
		}
	}

	protected void OnFocus()
	{
		SelectAll();
	}

	protected void SelectAll()
	{
		stringPositionInternal = text.Length;
		stringSelectPositionInternal = 0;
	}

	public void MoveTextEnd(bool shift)
	{
		int length = text.Length;
		if (shift)
		{
			stringSelectPositionInternal = length;
		}
		else
		{
			stringPositionInternal = length;
			stringSelectPositionInternal = stringPositionInternal;
		}
		UpdateLabel();
	}

	public void MoveTextStart(bool shift)
	{
		int num = 0;
		if (shift)
		{
			stringSelectPositionInternal = num;
		}
		else
		{
			stringPositionInternal = num;
			stringSelectPositionInternal = stringPositionInternal;
		}
		UpdateLabel();
	}

	private bool InPlaceEditing()
	{
		return !TouchScreenKeyboard.isSupported;
	}

	protected virtual void LateUpdate()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Invalid comparison between Unknown and I4
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Invalid comparison between Unknown and I4
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Invalid comparison between Unknown and I4
		if (m_ShouldActivateNextUpdate)
		{
			if (!isFocused)
			{
				ActivateInputFieldInternal();
				m_ShouldActivateNextUpdate = false;
				return;
			}
			m_ShouldActivateNextUpdate = false;
		}
		if (InPlaceEditing() || !isFocused)
		{
			return;
		}
		AssignPositioningIfNeeded();
		if (m_Keyboard == null || !m_Keyboard.active)
		{
			if (m_Keyboard != null)
			{
				if (!m_ReadOnly)
				{
					this.text = m_Keyboard.text;
				}
				if ((int)m_Keyboard.status == 2)
				{
					m_WasCanceled = true;
				}
			}
			((Selectable)this).OnDeselect((BaseEventData)null);
			return;
		}
		string text = m_Keyboard.text;
		if (m_Text != text)
		{
			if (m_ReadOnly)
			{
				m_Keyboard.text = m_Text;
			}
			else
			{
				m_Text = string.Empty;
				for (int i = 0; i < text.Length; i++)
				{
					char c = text[i];
					if (c == '\r' || c == '\u0003')
					{
						c = '\n';
					}
					if (onValidateInput != null)
					{
						c = onValidateInput(m_Text, m_Text.Length, c);
					}
					else if (characterValidation != CharacterValidation.None)
					{
						c = Validate(m_Text, m_Text.Length, c);
					}
					if (lineType == LineType.MultiLineSubmit && c == '\n')
					{
						m_Keyboard.text = m_Text;
						((Selectable)this).OnDeselect((BaseEventData)null);
						return;
					}
					if (c != 0)
					{
						m_Text += c;
					}
				}
				if (characterLimit > 0 && m_Text.Length > characterLimit)
				{
					m_Text = m_Text.Substring(0, characterLimit);
				}
				int num = (stringSelectPositionInternal = m_Text.Length);
				stringPositionInternal = num;
				if (m_Text != text)
				{
					m_Keyboard.text = m_Text;
				}
				SendOnValueChangedAndUpdateLabel();
			}
		}
		if ((int)m_Keyboard.status == 1)
		{
			if ((int)m_Keyboard.status == 2)
			{
				m_WasCanceled = true;
			}
			((Selectable)this).OnDeselect((BaseEventData)null);
		}
	}

	protected int GetCharacterIndexFromPosition(Vector2 pos)
	{
		return 0;
	}

	private bool MayDrag(PointerEventData eventData)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return ((UIBehaviour)this).IsActive() && ((Selectable)this).IsInteractable() && (int)eventData.button == 0 && (Object)(object)m_TextComponent != (Object)null && m_Keyboard == null;
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (MayDrag(eventData))
		{
			m_UpdateDrag = true;
		}
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (MayDrag(eventData))
		{
			CaretPosition cursor;
			int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(m_TextComponent, Vector2.op_Implicit(eventData.position), eventData.pressEventCamera, out cursor);
			switch (cursor)
			{
			case CaretPosition.Left:
				stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition);
				break;
			case CaretPosition.Right:
				stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1;
				break;
			}
			caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
			MarkGeometryAsDirty();
			m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(textViewport, eventData.position, eventData.pressEventCamera);
			if (m_DragPositionOutOfBounds && m_DragCoroutine == null)
			{
				m_DragCoroutine = ((MonoBehaviour)this).StartCoroutine(MouseDragOutsideRect(eventData));
			}
			((AbstractEventData)eventData).Use();
		}
	}

	private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
	{
		Vector2 localMousePos = default(Vector2);
		while (m_UpdateDrag && m_DragPositionOutOfBounds)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(textViewport, eventData.position, eventData.pressEventCamera, ref localMousePos);
			Rect rect = textViewport.rect;
			if (multiLine)
			{
				if (localMousePos.y > ((Rect)(ref rect)).yMax)
				{
					MoveUp(shift: true, goToFirstChar: true);
				}
				else if (localMousePos.y < ((Rect)(ref rect)).yMin)
				{
					MoveDown(shift: true, goToLastChar: true);
				}
			}
			else if (localMousePos.x < ((Rect)(ref rect)).xMin)
			{
				MoveLeft(shift: true, ctrl: false);
			}
			else if (localMousePos.x > ((Rect)(ref rect)).xMax)
			{
				MoveRight(shift: true, ctrl: false);
			}
			UpdateLabel();
			float delay = ((!multiLine) ? 0.05f : 0.1f);
			yield return (object)new WaitForSeconds(delay);
		}
		m_DragCoroutine = null;
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (MayDrag(eventData))
		{
			m_UpdateDrag = false;
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (!MayDrag(eventData))
		{
			return;
		}
		EventSystem.current.SetSelectedGameObject(((Component)this).gameObject, (BaseEventData)(object)eventData);
		bool allowInput = m_AllowInput;
		((Selectable)this).OnPointerDown(eventData);
		if (!InPlaceEditing() && (m_Keyboard == null || !m_Keyboard.active))
		{
			((Selectable)this).OnSelect((BaseEventData)(object)eventData);
			return;
		}
		if (allowInput)
		{
			CaretPosition cursor;
			int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(m_TextComponent, Vector2.op_Implicit(eventData.position), eventData.pressEventCamera, out cursor);
			int num;
			switch (cursor)
			{
			case CaretPosition.Left:
				num = (stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition));
				stringPositionInternal = num;
				break;
			case CaretPosition.Right:
				num = (stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1);
				stringPositionInternal = num;
				break;
			}
			num = (caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringPositionInternal));
			caretPositionInternal = num;
		}
		UpdateLabel();
		((AbstractEventData)eventData).Use();
	}

	protected EditState KeyPressed(Event evt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected I4, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected I4, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected I4, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Invalid comparison between Unknown and I4
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Invalid comparison between Unknown and I4
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Invalid comparison between Unknown and I4
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Invalid comparison between Unknown and I4
		EventModifiers modifiers = evt.modifiers;
		RuntimePlatform platform = Application.platform;
		bool flag = (((int)platform != 0 && (int)platform != 1) ? ((modifiers & 2) != 0) : ((modifiers & 8) != 0));
		bool flag2 = (modifiers & 1) != 0;
		bool flag3 = (modifiers & 4) != 0;
		bool flag4 = flag && !flag3 && !flag2;
		KeyCode keyCode = evt.keyCode;
		switch (keyCode - 271)
		{
		default:
			switch (keyCode - 97)
			{
			case 0:
				goto IL_0117;
			case 2:
				goto IL_012b;
			}
			switch (keyCode - 118)
			{
			case 0:
				goto IL_015f;
			case 2:
				goto IL_0178;
			}
			if ((int)keyCode != 8)
			{
				if ((int)keyCode != 13)
				{
					if ((int)keyCode != 27)
					{
						if ((int)keyCode != 127)
						{
							break;
						}
						ForwardSpace();
						return EditState.Continue;
					}
					m_WasCanceled = true;
					return EditState.Finish;
				}
				goto case 0;
			}
			Backspace();
			return EditState.Continue;
		case 7:
			MoveTextStart(flag2);
			return EditState.Continue;
		case 8:
			MoveTextEnd(flag2);
			return EditState.Continue;
		case 5:
			MoveLeft(flag2, flag);
			return EditState.Continue;
		case 4:
			MoveRight(flag2, flag);
			return EditState.Continue;
		case 2:
			MoveUp(flag2);
			return EditState.Continue;
		case 3:
			MoveDown(flag2);
			return EditState.Continue;
		case 0:
			{
				if (lineType != LineType.MultiLineNewline)
				{
					return EditState.Finish;
				}
				break;
			}
			IL_012b:
			if (flag4)
			{
				if (inputType != InputType.Password)
				{
					clipboard = GetSelectedString();
				}
				else
				{
					clipboard = string.Empty;
				}
				return EditState.Continue;
			}
			break;
			IL_0178:
			if (flag4)
			{
				if (inputType != InputType.Password)
				{
					clipboard = GetSelectedString();
				}
				else
				{
					clipboard = string.Empty;
				}
				Delete();
				SendOnValueChangedAndUpdateLabel();
				return EditState.Continue;
			}
			break;
			IL_0117:
			if (flag4)
			{
				SelectAll();
				return EditState.Continue;
			}
			break;
			IL_015f:
			if (flag4)
			{
				Append(clipboard);
				return EditState.Continue;
			}
			break;
		}
		char c = evt.character;
		if (!multiLine && (c == '\t' || c == '\r' || c == '\n'))
		{
			return EditState.Continue;
		}
		if (c == '\r' || c == '\u0003')
		{
			c = '\n';
		}
		if (IsValidChar(c))
		{
			Append(c);
		}
		if (c == '\0' && Input.compositionString.Length > 0)
		{
			UpdateLabel();
		}
		return EditState.Continue;
	}

	private bool IsValidChar(char c)
	{
		switch (c)
		{
		case '\u007f':
			return false;
		case '\t':
		case '\n':
			return true;
		default:
			return m_TextComponent.font.HasCharacter(c, searchFallbacks: true);
		}
	}

	public void ProcessEvent(Event e)
	{
		KeyPressed(e);
	}

	public virtual void OnUpdateSelected(BaseEventData eventData)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		if (!isFocused)
		{
			return;
		}
		bool flag = false;
		while (Event.PopEvent(m_ProcessingEvent))
		{
			if ((int)m_ProcessingEvent.rawType == 4)
			{
				flag = true;
				EditState editState = KeyPressed(m_ProcessingEvent);
				if (editState == EditState.Finish)
				{
					DeactivateInputField();
					break;
				}
			}
			EventType type = m_ProcessingEvent.type;
			if ((int)type == 13 || (int)type == 14)
			{
				string commandName = m_ProcessingEvent.commandName;
				if (commandName != null && commandName == "SelectAll")
				{
					SelectAll();
					flag = true;
				}
			}
		}
		if (flag)
		{
			UpdateLabel();
		}
		((AbstractEventData)eventData).Use();
	}

	private string GetSelectedString()
	{
		if (!hasSelection)
		{
			return string.Empty;
		}
		int num = stringPositionInternal;
		int num2 = stringSelectPositionInternal;
		if (num > num2)
		{
			int num3 = num;
			num = num2;
			num2 = num3;
		}
		return text.Substring(num, num2 - num);
	}

	private int FindtNextWordBegin()
	{
		if (stringSelectPositionInternal + 1 >= text.Length)
		{
			return text.Length;
		}
		int num = text.IndexOfAny(kSeparators, stringSelectPositionInternal + 1);
		if (num == -1)
		{
			return text.Length;
		}
		return num + 1;
	}

	private void MoveRight(bool shift, bool ctrl)
	{
		if (hasSelection && !shift)
		{
			int num = (stringSelectPositionInternal = Mathf.Max(stringPositionInternal, stringSelectPositionInternal));
			stringPositionInternal = num;
			num = (caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal));
			caretPositionInternal = num;
			return;
		}
		int num3 = caretSelectPositionInternal;
		int num4 = ((!ctrl) ? (stringSelectPositionInternal + 1) : FindtNextWordBegin());
		if (shift)
		{
			stringSelectPositionInternal = num4;
			caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
		}
		else
		{
			int num = (stringPositionInternal = num4);
			stringSelectPositionInternal = num;
			num = (caretPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal));
			caretSelectPositionInternal = num;
		}
		isCaretInsideTag = num3 == caretSelectPositionInternal;
		Debug.Log((object)("Caret is " + ((!isCaretInsideTag) ? " [Not Inside Tag]" : " [Inside Tag]")));
	}

	private int FindtPrevWordBegin()
	{
		if (stringSelectPositionInternal - 2 < 0)
		{
			return 0;
		}
		int num = text.LastIndexOfAny(kSeparators, stringSelectPositionInternal - 2);
		if (num == -1)
		{
			return 0;
		}
		return num + 1;
	}

	private void MoveLeft(bool shift, bool ctrl)
	{
		if (hasSelection && !shift)
		{
			int num = (stringSelectPositionInternal = Mathf.Min(stringPositionInternal, stringSelectPositionInternal));
			stringPositionInternal = num;
			num = (caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal));
			caretPositionInternal = num;
			return;
		}
		int num3 = caretSelectPositionInternal;
		int num4 = ((!ctrl) ? (stringSelectPositionInternal - 1) : FindtPrevWordBegin());
		if (shift)
		{
			stringSelectPositionInternal = num4;
			caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
		}
		else
		{
			int num = (stringPositionInternal = num4);
			stringSelectPositionInternal = num;
			num = (caretPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal));
			caretSelectPositionInternal = num;
		}
		isCaretInsideTag = num3 == caretSelectPositionInternal;
		Debug.Log((object)("Caret is " + ((!isCaretInsideTag) ? " [Not Inside Tag]" : " [Inside Tag]")));
	}

	private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
	{
		if (originalPos >= m_TextComponent.textInfo.characterCount)
		{
			originalPos--;
		}
		TMP_CharacterInfo tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[originalPos];
		int lineNumber = tMP_CharacterInfo.lineNumber;
		if (lineNumber - 1 < 0)
		{
			return (!goToFirstChar) ? originalPos : 0;
		}
		int num = m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex - 1;
		for (int i = m_TextComponent.textInfo.lineInfo[lineNumber - 1].firstCharacterIndex; i < num; i++)
		{
			TMP_CharacterInfo tMP_CharacterInfo2 = m_TextComponent.textInfo.characterInfo[i];
			float num2 = (tMP_CharacterInfo.origin - tMP_CharacterInfo2.origin) / (tMP_CharacterInfo2.xAdvance - tMP_CharacterInfo2.origin);
			if (num2 >= 0f && num2 <= 1f)
			{
				if (num2 < 0.5f)
				{
					return i;
				}
				return i + 1;
			}
		}
		return num;
	}

	private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
	{
		if (originalPos >= m_TextComponent.textInfo.characterCount)
		{
			return text.Length;
		}
		TMP_CharacterInfo tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[originalPos];
		int lineNumber = tMP_CharacterInfo.lineNumber;
		if (lineNumber + 1 >= m_TextComponent.textInfo.lineCount)
		{
			return (!goToLastChar) ? originalPos : (m_TextComponent.textInfo.characterCount - 1);
		}
		int lastCharacterIndex = m_TextComponent.textInfo.lineInfo[lineNumber + 1].lastCharacterIndex;
		for (int i = m_TextComponent.textInfo.lineInfo[lineNumber + 1].firstCharacterIndex; i < lastCharacterIndex; i++)
		{
			TMP_CharacterInfo tMP_CharacterInfo2 = m_TextComponent.textInfo.characterInfo[i];
			float num = (tMP_CharacterInfo.origin - tMP_CharacterInfo2.origin) / (tMP_CharacterInfo2.xAdvance - tMP_CharacterInfo2.origin);
			if (num >= 0f && num <= 1f)
			{
				if (num < 0.5f)
				{
					return i;
				}
				return i + 1;
			}
		}
		return lastCharacterIndex;
	}

	private void MoveDown(bool shift)
	{
		MoveDown(shift, goToLastChar: true);
	}

	private void MoveDown(bool shift, bool goToLastChar)
	{
		int num;
		if (hasSelection && !shift)
		{
			num = (caretSelectPositionInternal = Mathf.Max(caretPositionInternal, caretSelectPositionInternal));
			caretPositionInternal = num;
		}
		int num3 = ((!multiLine) ? text.Length : LineDownCharacterPosition(caretSelectPositionInternal, goToLastChar));
		if (shift)
		{
			caretSelectPositionInternal = num3;
			stringSelectPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal);
			return;
		}
		num = (caretPositionInternal = num3);
		caretSelectPositionInternal = num;
		num = (stringPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal));
		stringSelectPositionInternal = num;
	}

	private void MoveUp(bool shift)
	{
		MoveUp(shift, goToFirstChar: true);
	}

	private void MoveUp(bool shift, bool goToFirstChar)
	{
		int num;
		if (hasSelection && !shift)
		{
			num = (caretSelectPositionInternal = Mathf.Min(caretPositionInternal, caretSelectPositionInternal));
			caretPositionInternal = num;
		}
		int num3 = (multiLine ? LineUpCharacterPosition(caretSelectPositionInternal, goToFirstChar) : 0);
		if (shift)
		{
			caretSelectPositionInternal = num3;
			stringSelectPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal);
			return;
		}
		num = (caretPositionInternal = num3);
		caretSelectPositionInternal = num;
		num = (stringPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal));
		stringSelectPositionInternal = num;
	}

	private void Delete()
	{
		if (!m_ReadOnly && stringPositionInternal != stringSelectPositionInternal)
		{
			if (stringPositionInternal < stringSelectPositionInternal)
			{
				m_Text = text.Substring(0, stringPositionInternal) + text.Substring(stringSelectPositionInternal, text.Length - stringSelectPositionInternal);
				stringSelectPositionInternal = stringPositionInternal;
			}
			else
			{
				m_Text = text.Substring(0, stringSelectPositionInternal) + text.Substring(stringPositionInternal, text.Length - stringPositionInternal);
				stringPositionInternal = stringSelectPositionInternal;
			}
		}
	}

	private void ForwardSpace()
	{
		if (!m_ReadOnly)
		{
			if (hasSelection)
			{
				Delete();
				SendOnValueChangedAndUpdateLabel();
			}
			else if (stringPositionInternal < text.Length)
			{
				m_Text = text.Remove(stringPositionInternal, 1);
				SendOnValueChangedAndUpdateLabel();
			}
		}
	}

	private void Backspace()
	{
		if (!m_ReadOnly)
		{
			if (hasSelection)
			{
				Delete();
				SendOnValueChangedAndUpdateLabel();
			}
			else if (stringPositionInternal > 0)
			{
				m_Text = text.Remove(stringPositionInternal - 1, 1);
				stringSelectPositionInternal = --stringPositionInternal;
				m_isLastKeyBackspace = true;
				SendOnValueChangedAndUpdateLabel();
			}
		}
	}

	private void Insert(char c)
	{
		if (!m_ReadOnly)
		{
			string text = c.ToString();
			Delete();
			if (characterLimit <= 0 || this.text.Length < characterLimit)
			{
				m_Text = this.text.Insert(m_StringPosition, text);
				stringSelectPositionInternal = (stringPositionInternal += text.Length);
				SendOnValueChanged();
			}
		}
	}

	private void SendOnValueChangedAndUpdateLabel()
	{
		SendOnValueChanged();
		UpdateLabel();
	}

	private void SendOnValueChanged()
	{
		if (onValueChanged != null)
		{
			((UnityEvent<string>)onValueChanged).Invoke(text);
		}
	}

	protected void SendOnSubmit()
	{
		if (onEndEdit != null)
		{
			((UnityEvent<string>)onEndEdit).Invoke(m_Text);
		}
	}

	protected void SendOnFocusLost()
	{
		if (onFocusLost != null)
		{
			((UnityEvent<string>)onFocusLost).Invoke(m_Text);
		}
	}

	protected virtual void Append(string input)
	{
		if (m_ReadOnly || !InPlaceEditing())
		{
			return;
		}
		int i = 0;
		for (int length = input.Length; i < length; i++)
		{
			char c = input[i];
			if (c >= ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\n')
			{
				Append(c);
			}
		}
	}

	protected virtual void Append(char input)
	{
		if (!m_ReadOnly && InPlaceEditing())
		{
			if (onValidateInput != null)
			{
				input = onValidateInput(text, stringPositionInternal, input);
			}
			else if (characterValidation != CharacterValidation.None)
			{
				input = Validate(text, stringPositionInternal, input);
			}
			if (input != 0)
			{
				Insert(input);
			}
		}
	}

	protected void UpdateLabel()
	{
		if ((Object)(object)m_TextComponent != (Object)null && (Object)(object)m_TextComponent.font != (Object)null)
		{
			string text = ((Input.compositionString.Length <= 0) ? this.text : (this.text.Substring(0, m_StringPosition) + Input.compositionString + this.text.Substring(m_StringPosition)));
			string text2 = ((inputType != InputType.Password) ? text : new string(asteriskChar, text.Length));
			bool flag = string.IsNullOrEmpty(text);
			if ((Object)(object)m_Placeholder != (Object)null)
			{
				((Behaviour)m_Placeholder).enabled = flag;
			}
			if (!m_AllowInput)
			{
				m_DrawStart = 0;
				m_DrawEnd = m_Text.Length;
			}
			if (!flag)
			{
				SetCaretVisible();
			}
			m_TextComponent.text = text2 + "\u200b";
			MarkGeometryAsDirty();
		}
	}

	private int GetCaretPositionFromStringIndex(int stringIndex)
	{
		int characterCount = m_TextComponent.textInfo.characterCount;
		for (int i = 0; i < characterCount; i++)
		{
			if (m_TextComponent.textInfo.characterInfo[i].index >= stringIndex)
			{
				return i;
			}
		}
		return characterCount;
	}

	private int GetStringIndexFromCaretPosition(int caretPosition)
	{
		return m_TextComponent.textInfo.characterInfo[caretPosition].index;
	}

	public void ForceLabelUpdate()
	{
		UpdateLabel();
	}

	private void MarkGeometryAsDirty()
	{
		CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild((ICanvasElement)(object)this);
	}

	public virtual void Rebuild(CanvasUpdate update)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)update == 4)
		{
			UpdateGeometry();
		}
	}

	public virtual void LayoutComplete()
	{
	}

	public virtual void GraphicUpdateComplete()
	{
	}

	private void UpdateGeometry()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		if (shouldHideMobileInput)
		{
			if ((Object)(object)m_CachedInputRenderer == (Object)null && (Object)(object)m_TextComponent != (Object)null)
			{
				GameObject val = new GameObject(((Object)((Component)this).transform).name + " Input Caret");
				((Object)val).hideFlags = (HideFlags)52;
				val.transform.SetParent(m_TextComponent.transform.parent);
				val.transform.SetAsFirstSibling();
				val.layer = ((Component)this).gameObject.layer;
				caretRectTrans = val.AddComponent<RectTransform>();
				m_CachedInputRenderer = val.AddComponent<CanvasRenderer>();
				m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, (Texture)(object)Texture2D.whiteTexture);
				val.AddComponent<LayoutElement>().ignoreLayout = true;
				AssignPositioningIfNeeded();
			}
			if (!((Object)(object)m_CachedInputRenderer == (Object)null))
			{
				OnFillVBO(mesh);
				m_CachedInputRenderer.SetMesh(mesh);
			}
		}
	}

	private void AssignPositioningIfNeeded()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_TextComponent != (Object)null && (Object)(object)caretRectTrans != (Object)null && (((Transform)caretRectTrans).localPosition != ((Transform)m_TextComponent.rectTransform).localPosition || ((Transform)caretRectTrans).localRotation != ((Transform)m_TextComponent.rectTransform).localRotation || ((Transform)caretRectTrans).localScale != ((Transform)m_TextComponent.rectTransform).localScale || caretRectTrans.anchorMin != m_TextComponent.rectTransform.anchorMin || caretRectTrans.anchorMax != m_TextComponent.rectTransform.anchorMax || caretRectTrans.anchoredPosition != m_TextComponent.rectTransform.anchoredPosition || caretRectTrans.sizeDelta != m_TextComponent.rectTransform.sizeDelta || caretRectTrans.pivot != m_TextComponent.rectTransform.pivot))
		{
			((Transform)caretRectTrans).localPosition = ((Transform)m_TextComponent.rectTransform).localPosition;
			((Transform)caretRectTrans).localRotation = ((Transform)m_TextComponent.rectTransform).localRotation;
			((Transform)caretRectTrans).localScale = ((Transform)m_TextComponent.rectTransform).localScale;
			caretRectTrans.anchorMin = m_TextComponent.rectTransform.anchorMin;
			caretRectTrans.anchorMax = m_TextComponent.rectTransform.anchorMax;
			caretRectTrans.anchoredPosition = m_TextComponent.rectTransform.anchoredPosition;
			caretRectTrans.sizeDelta = m_TextComponent.rectTransform.sizeDelta;
			caretRectTrans.pivot = m_TextComponent.rectTransform.pivot;
		}
	}

	private void OnFillVBO(Mesh vbo)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		VertexHelper val = new VertexHelper();
		try
		{
			if (!isFocused)
			{
				val.FillMesh(vbo);
				return;
			}
			if (!hasSelection)
			{
				GenerateCaret(val, Vector2.zero);
			}
			else
			{
				GenerateHightlight(val, Vector2.zero);
			}
			val.FillMesh(vbo);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		if (m_CaretVisible)
		{
			if (m_CursorVerts == null)
			{
				CreateCursorVerts();
			}
			float num = m_CaretWidth;
			int characterCount = m_TextComponent.textInfo.characterCount;
			Vector2 zero = Vector2.zero;
			float num2 = 0f;
			caretPositionInternal = GetCaretPositionFromStringIndex(stringPositionInternal);
			TMP_CharacterInfo tMP_CharacterInfo;
			if (caretPositionInternal == 0)
			{
				tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[0];
				((Vector2)(ref zero))._002Ector(tMP_CharacterInfo.origin, tMP_CharacterInfo.descender);
				num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
			}
			else if (caretPositionInternal < characterCount)
			{
				tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[caretPositionInternal];
				((Vector2)(ref zero))._002Ector(tMP_CharacterInfo.origin, tMP_CharacterInfo.descender);
				num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
			}
			else
			{
				tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[characterCount - 1];
				((Vector2)(ref zero))._002Ector(tMP_CharacterInfo.xAdvance, tMP_CharacterInfo.descender);
				num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
			}
			AdjustRectTransformRelativeToViewport(zero, num2, tMP_CharacterInfo.isVisible);
			float num3 = zero.y + num2;
			float num4 = num2;
			Rect rect = m_TextComponent.rectTransform.rect;
			float num5 = num3 - Mathf.Min(num4, ((Rect)(ref rect)).height);
			m_CursorVerts[0].position = new Vector3(zero.x, num5, 0f);
			m_CursorVerts[1].position = new Vector3(zero.x, num3, 0f);
			m_CursorVerts[2].position = new Vector3(zero.x + num, num3, 0f);
			m_CursorVerts[3].position = new Vector3(zero.x + num, num5, 0f);
			m_CursorVerts[0].color = Color32.op_Implicit(caretColor);
			m_CursorVerts[1].color = Color32.op_Implicit(caretColor);
			m_CursorVerts[2].color = Color32.op_Implicit(caretColor);
			m_CursorVerts[3].color = Color32.op_Implicit(caretColor);
			vbo.AddUIVertexQuad(m_CursorVerts);
			int height = Screen.height;
			zero.y = (float)height - zero.y;
			Input.compositionCursorPos = zero;
		}
	}

	private void CreateCursorVerts()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		m_CursorVerts = (UIVertex[])(object)new UIVertex[4];
		for (int i = 0; i < m_CursorVerts.Length; i++)
		{
			ref UIVertex reference = ref m_CursorVerts[i];
			reference = UIVertex.simpleVert;
			m_CursorVerts[i].uv0 = Vector2.zero;
		}
	}

	private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		TMP_TextInfo textInfo = m_TextComponent.textInfo;
		caretPositionInternal = GetCaretPositionFromStringIndex(stringPositionInternal);
		caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
		Debug.Log((object)("StringPosition:" + stringPositionInternal + "  StringSelectPosition:" + stringSelectPositionInternal));
		float num = 0f;
		Vector2 startPosition = default(Vector2);
		if (caretSelectPositionInternal < textInfo.characterCount)
		{
			((Vector2)(ref startPosition))._002Ector(textInfo.characterInfo[caretSelectPositionInternal].origin, textInfo.characterInfo[caretSelectPositionInternal].descender);
			num = textInfo.characterInfo[caretSelectPositionInternal].ascender - textInfo.characterInfo[caretSelectPositionInternal].descender;
		}
		else
		{
			((Vector2)(ref startPosition))._002Ector(textInfo.characterInfo[caretSelectPositionInternal - 1].xAdvance, textInfo.characterInfo[caretSelectPositionInternal - 1].descender);
			num = textInfo.characterInfo[caretSelectPositionInternal - 1].ascender - textInfo.characterInfo[caretSelectPositionInternal - 1].descender;
		}
		AdjustRectTransformRelativeToViewport(startPosition, num, isCharVisible: true);
		int num2 = Mathf.Max(0, caretPositionInternal);
		int num3 = Mathf.Max(0, caretSelectPositionInternal);
		if (num2 > num3)
		{
			int num4 = num2;
			num2 = num3;
			num3 = num4;
		}
		num3--;
		int num5 = textInfo.characterInfo[num2].lineNumber;
		int lastCharacterIndex = textInfo.lineInfo[num5].lastCharacterIndex;
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.uv0 = Vector2.zero;
		simpleVert.color = Color32.op_Implicit(selectionColor);
		Vector2 val = default(Vector2);
		Vector2 val2 = default(Vector2);
		for (int i = num2; i <= num3 && i < textInfo.characterCount; i++)
		{
			if (i == lastCharacterIndex || i == num3)
			{
				TMP_CharacterInfo tMP_CharacterInfo = textInfo.characterInfo[num2];
				TMP_CharacterInfo tMP_CharacterInfo2 = textInfo.characterInfo[i];
				((Vector2)(ref val))._002Ector(tMP_CharacterInfo.origin, tMP_CharacterInfo.ascender);
				((Vector2)(ref val2))._002Ector(tMP_CharacterInfo2.xAdvance, tMP_CharacterInfo2.descender);
				Rect rect = m_TextViewport.rect;
				Vector2 min = ((Rect)(ref rect)).min;
				Rect rect2 = m_TextViewport.rect;
				Vector2 max = ((Rect)(ref rect2)).max;
				float num6 = m_TextComponent.rectTransform.anchoredPosition.x + val.x - min.x;
				if (num6 < 0f)
				{
					val.x -= num6;
				}
				float num7 = m_TextComponent.rectTransform.anchoredPosition.y + val2.y - min.y;
				if (num7 < 0f)
				{
					val2.y -= num7;
				}
				float num8 = max.x - (m_TextComponent.rectTransform.anchoredPosition.x + val2.x);
				if (num8 < 0f)
				{
					val2.x += num8;
				}
				float num9 = max.y - (m_TextComponent.rectTransform.anchoredPosition.y + val.y);
				if (num9 < 0f)
				{
					val.y += num9;
				}
				if (!(m_TextComponent.rectTransform.anchoredPosition.y + val.y < min.y) && !(m_TextComponent.rectTransform.anchoredPosition.y + val2.y > max.y))
				{
					int currentVertCount = vbo.currentVertCount;
					simpleVert.position = new Vector3(val.x, val2.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(val2.x, val2.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(val2.x, val.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(val.x, val.y, 0f);
					vbo.AddVert(simpleVert);
					vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
					vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
				}
				num2 = i + 1;
				num5++;
				if (num5 < textInfo.lineCount)
				{
					lastCharacterIndex = textInfo.lineInfo[num5].lastCharacterIndex;
				}
			}
		}
	}

	private void AdjustRectTransformRelativeToViewport(Vector2 startPosition, float height, bool isCharVisible)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = m_TextViewport.rect;
		float xMin = ((Rect)(ref rect)).xMin;
		Rect rect2 = m_TextViewport.rect;
		float xMax = ((Rect)(ref rect2)).xMax;
		float num = xMax - (m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x + m_TextComponent.margin.z);
		if (num < 0f && (!multiLine || (multiLine && isCharVisible)))
		{
			RectTransform rectTransform = m_TextComponent.rectTransform;
			rectTransform.anchoredPosition += new Vector2(num, 0f);
			AssignPositioningIfNeeded();
		}
		float num2 = m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x - m_TextComponent.margin.x - xMin;
		if (num2 < 0f)
		{
			RectTransform rectTransform2 = m_TextComponent.rectTransform;
			rectTransform2.anchoredPosition += new Vector2(0f - num2, 0f);
			AssignPositioningIfNeeded();
		}
		if (m_LineType != LineType.SingleLine)
		{
			Rect rect3 = m_TextViewport.rect;
			float num3 = ((Rect)(ref rect3)).yMax - (m_TextComponent.rectTransform.anchoredPosition.y + startPosition.y + height);
			if (num3 < -0.0001f)
			{
				RectTransform rectTransform3 = m_TextComponent.rectTransform;
				rectTransform3.anchoredPosition += new Vector2(0f, num3);
				AssignPositioningIfNeeded();
			}
			float num4 = m_TextComponent.rectTransform.anchoredPosition.y + startPosition.y;
			Rect rect4 = m_TextViewport.rect;
			float num5 = num4 - ((Rect)(ref rect4)).yMin;
			if (num5 < 0f)
			{
				RectTransform rectTransform4 = m_TextComponent.rectTransform;
				rectTransform4.anchoredPosition -= new Vector2(0f, num5);
				AssignPositioningIfNeeded();
			}
		}
		if (!m_isLastKeyBackspace)
		{
			return;
		}
		float num6 = m_TextComponent.rectTransform.anchoredPosition.x + m_TextComponent.textInfo.characterInfo[0].origin - m_TextComponent.margin.x;
		float num7 = m_TextComponent.rectTransform.anchoredPosition.x + m_TextComponent.textInfo.characterInfo[m_TextComponent.textInfo.characterCount - 1].origin + m_TextComponent.margin.z;
		if (m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x <= xMin + 0.0001f)
		{
			if (num6 < xMin)
			{
				float num8 = Mathf.Min((xMax - xMin) / 2f, xMin - num6);
				RectTransform rectTransform5 = m_TextComponent.rectTransform;
				rectTransform5.anchoredPosition += new Vector2(num8, 0f);
				AssignPositioningIfNeeded();
			}
		}
		else if (num7 < xMax && num6 < xMin)
		{
			float num9 = Mathf.Min(xMax - num7, xMin - num6);
			RectTransform rectTransform6 = m_TextComponent.rectTransform;
			rectTransform6.anchoredPosition += new Vector2(num9, 0f);
			AssignPositioningIfNeeded();
		}
		m_isLastKeyBackspace = false;
	}

	protected char Validate(string text, int pos, char ch)
	{
		if (characterValidation == CharacterValidation.None || !((Behaviour)this).enabled)
		{
			return ch;
		}
		if (characterValidation == CharacterValidation.Integer || characterValidation == CharacterValidation.Decimal)
		{
			bool flag = pos == 0 && text.Length > 0 && text[0] == '-';
			bool flag2 = stringPositionInternal == 0 || stringSelectPositionInternal == 0;
			if (!flag)
			{
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '-' && (pos == 0 || flag2))
				{
					return ch;
				}
				if (ch == '.' && characterValidation == CharacterValidation.Decimal && !text.Contains("."))
				{
					return ch;
				}
			}
		}
		else if (characterValidation == CharacterValidation.Alphanumeric)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch;
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (characterValidation == CharacterValidation.Name)
		{
			char c = ((text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)]);
			char c2 = ((text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)]);
			if (char.IsLetter(ch))
			{
				if (char.IsLower(ch) && c == ' ')
				{
					return char.ToUpper(ch);
				}
				if (char.IsUpper(ch) && c != ' ' && c != '\'')
				{
					return char.ToLower(ch);
				}
				return ch;
			}
			switch (ch)
			{
			case '\'':
				if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
				{
					return ch;
				}
				break;
			case ' ':
				if (c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
				{
					return ch;
				}
				break;
			}
		}
		else if (characterValidation == CharacterValidation.EmailAddress)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch;
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '@' && text.IndexOf('@') == -1)
			{
				return ch;
			}
			if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
			{
				return ch;
			}
			if (ch == '.')
			{
				char c3 = ((text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)]);
				char c4 = ((text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)]);
				if (c3 != '.' && c4 != '.')
				{
					return ch;
				}
			}
		}
		return '\0';
	}

	public void ActivateInputField()
	{
		if (!((Object)(object)m_TextComponent == (Object)null) && !((Object)(object)m_TextComponent.font == (Object)null) && ((UIBehaviour)this).IsActive() && ((Selectable)this).IsInteractable())
		{
			if (isFocused && m_Keyboard != null && !m_Keyboard.active)
			{
				m_Keyboard.active = true;
				m_Keyboard.text = m_Text;
			}
			m_HasLostFocus = false;
			m_ShouldActivateNextUpdate = true;
		}
	}

	private void ActivateInputFieldInternal()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)EventSystem.current == (Object)null)
		{
			return;
		}
		if ((Object)(object)EventSystem.current.currentSelectedGameObject != (Object)(object)((Component)this).gameObject)
		{
			EventSystem.current.SetSelectedGameObject(((Component)this).gameObject);
		}
		if (TouchScreenKeyboard.isSupported)
		{
			if (Input.touchSupported)
			{
				TouchScreenKeyboard.hideInput = shouldHideMobileInput;
			}
			m_Keyboard = ((inputType != InputType.Password) ? TouchScreenKeyboard.Open(m_Text, keyboardType, inputType == InputType.AutoCorrect, multiLine) : TouchScreenKeyboard.Open(m_Text, keyboardType, false, multiLine, true));
			MoveTextEnd(shift: false);
		}
		else
		{
			Input.imeCompositionMode = (IMECompositionMode)1;
			OnFocus();
		}
		m_AllowInput = true;
		m_OriginalText = text;
		m_WasCanceled = false;
		SetCaretVisible();
		UpdateLabel();
	}

	public override void OnSelect(BaseEventData eventData)
	{
		Debug.Log((object)"OnSelect()");
		((Selectable)this).OnSelect(eventData);
		ActivateInputField();
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)eventData.button == 0)
		{
			ActivateInputField();
		}
	}

	public void DeactivateInputField()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (!m_AllowInput)
		{
			return;
		}
		m_HasDoneFocusTransition = false;
		m_AllowInput = false;
		if ((Object)(object)m_Placeholder != (Object)null)
		{
			((Behaviour)m_Placeholder).enabled = string.IsNullOrEmpty(m_Text);
		}
		if ((Object)(object)m_TextComponent != (Object)null && ((Selectable)this).IsInteractable())
		{
			if (m_WasCanceled)
			{
				text = m_OriginalText;
			}
			if (m_Keyboard != null)
			{
				m_Keyboard.active = false;
				m_Keyboard = null;
			}
			m_StringPosition = (m_StringSelectPosition = 0);
			((Transform)m_TextComponent.rectTransform).localPosition = Vector3.zero;
			if ((Object)(object)caretRectTrans != (Object)null)
			{
				((Transform)caretRectTrans).localPosition = Vector3.zero;
			}
			SendOnSubmit();
			if (m_HasLostFocus)
			{
				SendOnFocusLost();
			}
			Input.imeCompositionMode = (IMECompositionMode)0;
		}
		MarkGeometryAsDirty();
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		Debug.Log((object)"OnDeselect()");
		m_HasLostFocus = true;
		DeactivateInputField();
		((Selectable)this).OnDeselect(eventData);
	}

	public virtual void OnSubmit(BaseEventData eventData)
	{
		Debug.Log((object)"OnSubmit()");
		if (((UIBehaviour)this).IsActive() && ((Selectable)this).IsInteractable() && !isFocused)
		{
			m_ShouldActivateNextUpdate = true;
		}
	}

	private void EnforceContentType()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		switch (contentType)
		{
		case ContentType.Standard:
			m_InputType = InputType.Standard;
			m_KeyboardType = (TouchScreenKeyboardType)0;
			m_CharacterValidation = CharacterValidation.None;
			break;
		case ContentType.Autocorrected:
			m_InputType = InputType.AutoCorrect;
			m_KeyboardType = (TouchScreenKeyboardType)0;
			m_CharacterValidation = CharacterValidation.None;
			break;
		case ContentType.IntegerNumber:
			m_LineType = LineType.SingleLine;
			m_TextComponent.enableWordWrapping = false;
			m_InputType = InputType.Standard;
			m_KeyboardType = (TouchScreenKeyboardType)4;
			m_CharacterValidation = CharacterValidation.Integer;
			break;
		case ContentType.DecimalNumber:
			m_LineType = LineType.SingleLine;
			m_TextComponent.enableWordWrapping = false;
			m_InputType = InputType.Standard;
			m_KeyboardType = (TouchScreenKeyboardType)2;
			m_CharacterValidation = CharacterValidation.Decimal;
			break;
		case ContentType.Alphanumeric:
			m_LineType = LineType.SingleLine;
			m_TextComponent.enableWordWrapping = false;
			m_InputType = InputType.Standard;
			m_KeyboardType = (TouchScreenKeyboardType)1;
			m_CharacterValidation = CharacterValidation.Alphanumeric;
			break;
		case ContentType.Name:
			m_LineType = LineType.SingleLine;
			m_TextComponent.enableWordWrapping = false;
			m_InputType = InputType.Standard;
			m_KeyboardType = (TouchScreenKeyboardType)0;
			m_CharacterValidation = CharacterValidation.Name;
			break;
		case ContentType.EmailAddress:
			m_LineType = LineType.SingleLine;
			m_TextComponent.enableWordWrapping = false;
			m_InputType = InputType.Standard;
			m_KeyboardType = (TouchScreenKeyboardType)7;
			m_CharacterValidation = CharacterValidation.EmailAddress;
			break;
		case ContentType.Password:
			m_LineType = LineType.SingleLine;
			m_TextComponent.enableWordWrapping = false;
			m_InputType = InputType.Password;
			m_KeyboardType = (TouchScreenKeyboardType)0;
			m_CharacterValidation = CharacterValidation.None;
			break;
		case ContentType.Pin:
			m_LineType = LineType.SingleLine;
			m_TextComponent.enableWordWrapping = false;
			m_InputType = InputType.Password;
			m_KeyboardType = (TouchScreenKeyboardType)4;
			m_CharacterValidation = CharacterValidation.Integer;
			break;
		}
	}

	private void SetTextComponentWrapMode()
	{
		if (!((Object)(object)m_TextComponent == (Object)null))
		{
			if (m_LineType == LineType.SingleLine)
			{
				m_TextComponent.enableWordWrapping = false;
			}
			else
			{
				m_TextComponent.enableWordWrapping = true;
			}
		}
	}

	private void SetTextComponentRichTextMode()
	{
		if (!((Object)(object)m_TextComponent == (Object)null))
		{
			m_TextComponent.richText = m_RichText;
		}
	}

	private void SetToCustomIfContentTypeIsNot(params ContentType[] allowedContentTypes)
	{
		if (contentType == ContentType.Custom)
		{
			return;
		}
		for (int i = 0; i < allowedContentTypes.Length; i++)
		{
			if (contentType == allowedContentTypes[i])
			{
				return;
			}
		}
		contentType = ContentType.Custom;
	}

	private void SetToCustom()
	{
		if (contentType != ContentType.Custom)
		{
			contentType = ContentType.Custom;
		}
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (m_HasDoneFocusTransition)
		{
			state = (SelectionState)1;
		}
		else if ((int)state == 2)
		{
			m_HasDoneFocusTransition = true;
		}
		((Selectable)this).DoStateTransition(state, instant);
	}

	bool ICanvasElement.IsDestroyed()
	{
		return ((UIBehaviour)this).IsDestroyed();
	}
}
