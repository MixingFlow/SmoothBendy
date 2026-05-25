using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("Layout/Text Container")]
public class TextContainer : UIBehaviour
{
	private bool m_hasChanged;

	[SerializeField]
	private Vector2 m_pivot;

	[SerializeField]
	private TextContainerAnchors m_anchorPosition = TextContainerAnchors.Middle;

	[SerializeField]
	private Rect m_rect;

	private bool m_isDefaultWidth;

	private bool m_isDefaultHeight;

	private bool m_isAutoFitting;

	private Vector3[] m_corners = (Vector3[])(object)new Vector3[4];

	private Vector3[] m_worldCorners = (Vector3[])(object)new Vector3[4];

	[SerializeField]
	private Vector4 m_margins;

	private RectTransform m_rectTransform;

	private static Vector2 k_defaultSize = new Vector2(100f, 100f);

	private TextMeshPro m_textMeshPro;

	public bool hasChanged
	{
		get
		{
			return m_hasChanged;
		}
		set
		{
			m_hasChanged = value;
		}
	}

	public Vector2 pivot
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_pivot;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (m_pivot != value)
			{
				m_pivot = value;
				m_anchorPosition = GetAnchorPosition(m_pivot);
				m_hasChanged = true;
				OnContainerChanged();
			}
		}
	}

	public TextContainerAnchors anchorPosition
	{
		get
		{
			return m_anchorPosition;
		}
		set
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (m_anchorPosition != value)
			{
				m_anchorPosition = value;
				m_pivot = GetPivot(m_anchorPosition);
				m_hasChanged = true;
				OnContainerChanged();
			}
		}
	}

	public Rect rect
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_rect;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (m_rect != value)
			{
				m_rect = value;
				m_hasChanged = true;
				OnContainerChanged();
			}
		}
	}

	public Vector2 size
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(((Rect)(ref m_rect)).width, ((Rect)(ref m_rect)).height);
		}
		set
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (new Vector2(((Rect)(ref m_rect)).width, ((Rect)(ref m_rect)).height) != value)
			{
				SetRect(value);
				m_hasChanged = true;
				m_isDefaultWidth = false;
				m_isDefaultHeight = false;
				OnContainerChanged();
			}
		}
	}

	public float width
	{
		get
		{
			return ((Rect)(ref m_rect)).width;
		}
		set
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			SetRect(new Vector2(value, ((Rect)(ref m_rect)).height));
			m_hasChanged = true;
			m_isDefaultWidth = false;
			OnContainerChanged();
		}
	}

	public float height
	{
		get
		{
			return ((Rect)(ref m_rect)).height;
		}
		set
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			SetRect(new Vector2(((Rect)(ref m_rect)).width, value));
			m_hasChanged = true;
			m_isDefaultHeight = false;
			OnContainerChanged();
		}
	}

	public bool isDefaultWidth => m_isDefaultWidth;

	public bool isDefaultHeight => m_isDefaultHeight;

	public bool isAutoFitting
	{
		get
		{
			return m_isAutoFitting;
		}
		set
		{
			m_isAutoFitting = value;
		}
	}

	public Vector3[] corners => m_corners;

	public Vector3[] worldCorners => m_worldCorners;

	public Vector4 margins
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_margins;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (m_margins != value)
			{
				m_margins = value;
				m_hasChanged = true;
				OnContainerChanged();
			}
		}
	}

	public RectTransform rectTransform
	{
		get
		{
			if ((Object)(object)m_rectTransform == (Object)null)
			{
				m_rectTransform = ((Component)this).GetComponent<RectTransform>();
			}
			return m_rectTransform;
		}
	}

	public TextMeshPro textMeshPro
	{
		get
		{
			if ((Object)(object)m_textMeshPro == (Object)null)
			{
				m_textMeshPro = ((Component)this).GetComponent<TextMeshPro>();
			}
			return m_textMeshPro;
		}
	}

	protected override void Awake()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		m_rectTransform = rectTransform;
		if ((Object)(object)m_rectTransform == (Object)null)
		{
			Vector2 val = m_pivot;
			m_rectTransform = ((Component)this).gameObject.AddComponent<RectTransform>();
			m_pivot = val;
		}
		m_textMeshPro = ((Component)this).GetComponent(typeof(TextMeshPro)) as TextMeshPro;
		if (((Rect)(ref m_rect)).width != 0f && ((Rect)(ref m_rect)).height != 0f)
		{
			return;
		}
		if ((Object)(object)m_textMeshPro != (Object)null && m_textMeshPro.anchor != TMP_Compatibility.AnchorPositions.None)
		{
			Debug.LogWarning((object)"Converting from using anchor and lineLength properties to Text Container.", (Object)(object)this);
			m_isDefaultHeight = true;
			int num = (int)m_textMeshPro.anchor;
			m_textMeshPro.anchor = TMP_Compatibility.AnchorPositions.None;
			if (num == 9)
			{
				switch (m_textMeshPro.alignment)
				{
				case TextAlignmentOptions.TopLeft:
					m_textMeshPro.alignment = TextAlignmentOptions.BaselineLeft;
					break;
				case TextAlignmentOptions.Top:
					m_textMeshPro.alignment = TextAlignmentOptions.Baseline;
					break;
				case TextAlignmentOptions.TopRight:
					m_textMeshPro.alignment = TextAlignmentOptions.BaselineRight;
					break;
				case TextAlignmentOptions.TopJustified:
					m_textMeshPro.alignment = TextAlignmentOptions.BaselineJustified;
					break;
				}
				num = 3;
			}
			m_anchorPosition = (TextContainerAnchors)num;
			m_pivot = GetPivot(m_anchorPosition);
			if (m_textMeshPro.lineLength == 72f)
			{
				((Rect)(ref m_rect)).size = m_textMeshPro.GetPreferredValues(m_textMeshPro.text);
			}
			else
			{
				((Rect)(ref m_rect)).width = m_textMeshPro.lineLength;
				((Rect)(ref m_rect)).height = m_textMeshPro.GetPreferredValues(((Rect)(ref m_rect)).width, float.PositiveInfinity).y;
			}
		}
		else
		{
			m_isDefaultWidth = true;
			m_isDefaultHeight = true;
			m_pivot = GetPivot(m_anchorPosition);
			((Rect)(ref m_rect)).width = 20f;
			((Rect)(ref m_rect)).height = 5f;
			m_rectTransform.sizeDelta = size;
		}
		m_margins = new Vector4(0f, 0f, 0f, 0f);
		UpdateCorners();
	}

	protected override void OnEnable()
	{
		OnContainerChanged();
	}

	protected override void OnDisable()
	{
	}

	private void OnContainerChanged()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		UpdateCorners();
		if ((Object)(object)m_rectTransform != (Object)null)
		{
			m_rectTransform.sizeDelta = size;
			((Transform)m_rectTransform).hasChanged = true;
		}
		if ((Object)(object)textMeshPro != (Object)null)
		{
			((Graphic)m_textMeshPro).SetVerticesDirty();
			m_textMeshPro.margin = m_margins;
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rectTransform == (Object)null)
		{
			m_rectTransform = ((Component)this).gameObject.AddComponent<RectTransform>();
		}
		if (m_rectTransform.sizeDelta != k_defaultSize)
		{
			size = m_rectTransform.sizeDelta;
		}
		pivot = m_rectTransform.pivot;
		m_hasChanged = true;
		OnContainerChanged();
	}

	private void SetRect(Vector2 size)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_rect = new Rect(((Rect)(ref m_rect)).x, ((Rect)(ref m_rect)).y, size.x, size.y);
	}

	private void UpdateCorners()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		ref Vector3 reference = ref m_corners[0];
		reference = new Vector3((0f - m_pivot.x) * ((Rect)(ref m_rect)).width, (0f - m_pivot.y) * ((Rect)(ref m_rect)).height);
		ref Vector3 reference2 = ref m_corners[1];
		reference2 = new Vector3((0f - m_pivot.x) * ((Rect)(ref m_rect)).width, (1f - m_pivot.y) * ((Rect)(ref m_rect)).height);
		ref Vector3 reference3 = ref m_corners[2];
		reference3 = new Vector3((1f - m_pivot.x) * ((Rect)(ref m_rect)).width, (1f - m_pivot.y) * ((Rect)(ref m_rect)).height);
		ref Vector3 reference4 = ref m_corners[3];
		reference4 = new Vector3((1f - m_pivot.x) * ((Rect)(ref m_rect)).width, (0f - m_pivot.y) * ((Rect)(ref m_rect)).height);
		if ((Object)(object)m_rectTransform != (Object)null)
		{
			m_rectTransform.pivot = m_pivot;
		}
	}

	private Vector2 GetPivot(TextContainerAnchors anchor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		Vector2 zero = Vector2.zero;
		switch (anchor)
		{
		case TextContainerAnchors.TopLeft:
			((Vector2)(ref zero))._002Ector(0f, 1f);
			break;
		case TextContainerAnchors.Top:
			((Vector2)(ref zero))._002Ector(0.5f, 1f);
			break;
		case TextContainerAnchors.TopRight:
			((Vector2)(ref zero))._002Ector(1f, 1f);
			break;
		case TextContainerAnchors.Left:
			((Vector2)(ref zero))._002Ector(0f, 0.5f);
			break;
		case TextContainerAnchors.Middle:
			((Vector2)(ref zero))._002Ector(0.5f, 0.5f);
			break;
		case TextContainerAnchors.Right:
			((Vector2)(ref zero))._002Ector(1f, 0.5f);
			break;
		case TextContainerAnchors.BottomLeft:
			((Vector2)(ref zero))._002Ector(0f, 0f);
			break;
		case TextContainerAnchors.Bottom:
			((Vector2)(ref zero))._002Ector(0.5f, 0f);
			break;
		case TextContainerAnchors.BottomRight:
			((Vector2)(ref zero))._002Ector(1f, 0f);
			break;
		}
		return zero;
	}

	private TextContainerAnchors GetAnchorPosition(Vector2 pivot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (pivot == new Vector2(0f, 1f))
		{
			return TextContainerAnchors.TopLeft;
		}
		if (pivot == new Vector2(0.5f, 1f))
		{
			return TextContainerAnchors.Top;
		}
		if (pivot == new Vector2(1f, 1f))
		{
			return TextContainerAnchors.TopRight;
		}
		if (pivot == new Vector2(0f, 0.5f))
		{
			return TextContainerAnchors.Left;
		}
		if (pivot == new Vector2(0.5f, 0.5f))
		{
			return TextContainerAnchors.Middle;
		}
		if (pivot == new Vector2(1f, 0.5f))
		{
			return TextContainerAnchors.Right;
		}
		if (pivot == new Vector2(0f, 0f))
		{
			return TextContainerAnchors.BottomLeft;
		}
		if (pivot == new Vector2(0.5f, 0f))
		{
			return TextContainerAnchors.Bottom;
		}
		if (pivot == new Vector2(1f, 0f))
		{
			return TextContainerAnchors.BottomRight;
		}
		return TextContainerAnchors.Custom;
	}
}
