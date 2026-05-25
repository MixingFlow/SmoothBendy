using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TMPro;

public class TMP_Text : MaskableGraphic
{
	protected enum TextInputSources
	{
		Text,
		SetText,
		SetCharArray,
		String
	}

	[SerializeField]
	protected string m_text;

	[SerializeField]
	protected bool m_isRightToLeft;

	[SerializeField]
	protected TMP_FontAsset m_fontAsset;

	protected TMP_FontAsset m_currentFontAsset;

	protected bool m_isSDFShader;

	[SerializeField]
	protected Material m_sharedMaterial;

	protected Material m_currentMaterial;

	protected MaterialReference[] m_materialReferences = new MaterialReference[32];

	protected Dictionary<int, int> m_materialReferenceIndexLookup = new Dictionary<int, int>();

	protected TMP_XmlTagStack<MaterialReference> m_materialReferenceStack = new TMP_XmlTagStack<MaterialReference>(new MaterialReference[16]);

	protected int m_currentMaterialIndex;

	[SerializeField]
	protected Material[] m_fontSharedMaterials;

	[SerializeField]
	protected Material m_fontMaterial;

	[SerializeField]
	protected Material[] m_fontMaterials;

	protected bool m_isMaterialDirty;

	[FormerlySerializedAs("m_fontColor")]
	[SerializeField]
	protected Color32 m_fontColor32 = Color32.op_Implicit(Color.white);

	[SerializeField]
	protected Color m_fontColor = Color.white;

	protected static Color32 s_colorWhite = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	[SerializeField]
	protected bool m_enableVertexGradient;

	[SerializeField]
	protected VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

	[SerializeField]
	protected TMP_ColorGradient m_fontColorGradientPreset;

	protected TMP_SpriteAsset m_spriteAsset;

	[SerializeField]
	protected bool m_tintAllSprites;

	protected bool m_tintSprite;

	protected Color32 m_spriteColor;

	[SerializeField]
	protected bool m_overrideHtmlColors;

	[SerializeField]
	protected Color32 m_faceColor = Color32.op_Implicit(Color.white);

	[SerializeField]
	protected Color32 m_outlineColor = Color32.op_Implicit(Color.black);

	protected float m_outlineWidth;

	[SerializeField]
	protected float m_fontSize = 36f;

	protected float m_currentFontSize;

	[SerializeField]
	protected float m_fontSizeBase = 36f;

	protected TMP_XmlTagStack<float> m_sizeStack = new TMP_XmlTagStack<float>(new float[16]);

	[SerializeField]
	protected int m_fontWeight = 400;

	protected int m_fontWeightInternal;

	protected TMP_XmlTagStack<int> m_fontWeightStack = new TMP_XmlTagStack<int>(new int[16]);

	[SerializeField]
	protected bool m_enableAutoSizing;

	protected float m_maxFontSize;

	protected float m_minFontSize;

	[SerializeField]
	protected float m_fontSizeMin;

	[SerializeField]
	protected float m_fontSizeMax;

	[SerializeField]
	protected FontStyles m_fontStyle;

	protected FontStyles m_style;

	protected bool m_isUsingBold;

	[SerializeField]
	[FormerlySerializedAs("m_lineJustification")]
	protected TextAlignmentOptions m_textAlignment;

	protected TextAlignmentOptions m_lineJustification;

	protected Vector3[] m_textContainerLocalCorners = (Vector3[])(object)new Vector3[4];

	[SerializeField]
	protected float m_characterSpacing;

	protected float m_cSpacing;

	protected float m_monoSpacing;

	[SerializeField]
	protected float m_lineSpacing;

	protected float m_lineSpacingDelta;

	protected float m_lineHeight;

	[SerializeField]
	protected float m_lineSpacingMax;

	[SerializeField]
	protected float m_paragraphSpacing;

	[SerializeField]
	protected float m_charWidthMaxAdj;

	protected float m_charWidthAdjDelta;

	[SerializeField]
	protected bool m_enableWordWrapping;

	protected bool m_isCharacterWrappingEnabled;

	protected bool m_isNonBreakingSpace;

	protected bool m_isIgnoringAlignment;

	[SerializeField]
	protected float m_wordWrappingRatios = 0.4f;

	[SerializeField]
	protected bool m_enableAdaptiveJustification;

	protected float m_adaptiveJustificationThreshold = 10f;

	[SerializeField]
	protected TextOverflowModes m_overflowMode;

	protected bool m_isTextTruncated;

	[SerializeField]
	protected bool m_enableKerning;

	[SerializeField]
	protected bool m_enableExtraPadding;

	[SerializeField]
	protected bool checkPaddingRequired;

	[SerializeField]
	protected bool m_isRichText = true;

	[SerializeField]
	protected bool m_parseCtrlCharacters = true;

	protected bool m_isOverlay;

	[SerializeField]
	protected bool m_isOrthographic;

	[SerializeField]
	protected bool m_isCullingEnabled;

	[SerializeField]
	protected bool m_ignoreCulling = true;

	[SerializeField]
	protected TextureMappingOptions m_horizontalMapping;

	[SerializeField]
	protected TextureMappingOptions m_verticalMapping;

	protected TextRenderFlags m_renderMode = TextRenderFlags.Render;

	protected int m_maxVisibleCharacters = 99999;

	protected int m_maxVisibleWords = 99999;

	protected int m_maxVisibleLines = 99999;

	[SerializeField]
	protected bool m_useMaxVisibleDescender = true;

	[SerializeField]
	protected int m_pageToDisplay = 1;

	protected bool m_isNewPage;

	[SerializeField]
	protected Vector4 m_margin = new Vector4(0f, 0f, 0f, 0f);

	protected float m_marginLeft;

	protected float m_marginRight;

	protected float m_marginWidth;

	protected float m_marginHeight;

	protected float m_width = -1f;

	[SerializeField]
	protected TMP_TextInfo m_textInfo;

	[SerializeField]
	protected bool m_havePropertiesChanged;

	[SerializeField]
	protected bool m_isUsingLegacyAnimationComponent;

	protected Transform m_transform;

	protected RectTransform m_rectTransform;

	protected Mesh m_mesh;

	[SerializeField]
	protected bool m_isVolumetricText;

	protected float m_flexibleHeight = -1f;

	protected float m_flexibleWidth = -1f;

	protected float m_minHeight;

	protected float m_minWidth;

	protected float m_preferredWidth;

	protected float m_renderedWidth;

	protected bool m_isPreferredWidthDirty;

	protected float m_preferredHeight;

	protected float m_renderedHeight;

	protected bool m_isPreferredHeightDirty;

	protected bool m_isCalculatingPreferredValues;

	protected int m_layoutPriority;

	protected bool m_isCalculateSizeRequired;

	protected bool m_isLayoutDirty;

	protected bool m_verticesAlreadyDirty;

	protected bool m_layoutAlreadyDirty;

	protected bool m_isAwake;

	[SerializeField]
	protected bool m_isInputParsingRequired;

	[SerializeField]
	protected TextInputSources m_inputSource;

	protected string old_text;

	protected float old_arg0;

	protected float old_arg1;

	protected float old_arg2;

	protected float m_fontScale;

	protected float m_fontScaleMultiplier;

	protected char[] m_htmlTag = new char[128];

	protected XML_TagAttribute[] m_xmlAttribute = new XML_TagAttribute[8];

	protected float tag_LineIndent;

	protected float tag_Indent;

	protected TMP_XmlTagStack<float> m_indentStack = new TMP_XmlTagStack<float>(new float[16]);

	protected bool tag_NoParsing;

	protected bool m_isParsingText;

	protected int[] m_char_buffer;

	private TMP_CharacterInfo[] m_internalCharacterInfo;

	protected char[] m_input_CharArray = new char[256];

	private int m_charArray_Length;

	protected int m_totalCharacterCount;

	protected int m_characterCount;

	protected int m_firstCharacterOfLine;

	protected int m_firstVisibleCharacterOfLine;

	protected int m_lastCharacterOfLine;

	protected int m_lastVisibleCharacterOfLine;

	protected int m_lineNumber;

	protected int m_lineVisibleCharacterCount;

	protected int m_pageNumber;

	protected float m_maxAscender;

	protected float m_maxCapHeight;

	protected float m_maxDescender;

	protected float m_maxLineAscender;

	protected float m_maxLineDescender;

	protected float m_startOfLineAscender;

	protected float m_lineOffset;

	protected Extents m_meshExtents;

	protected Color32 m_htmlColor = Color32.op_Implicit(new Color(255f, 255f, 255f, 128f));

	protected TMP_XmlTagStack<Color32> m_colorStack = new TMP_XmlTagStack<Color32>((Color32[])(object)new Color32[16]);

	protected float m_tabSpacing;

	protected float m_spacing;

	protected TMP_XmlTagStack<int> m_styleStack = new TMP_XmlTagStack<int>(new int[16]);

	protected TMP_XmlTagStack<int> m_actionStack = new TMP_XmlTagStack<int>(new int[16]);

	protected float m_padding;

	protected float m_baselineOffset;

	protected float m_xAdvance;

	protected TMP_TextElementType m_textElementType;

	protected TMP_TextElement m_cached_TextElement;

	protected TMP_Glyph m_cached_Underline_GlyphInfo;

	protected TMP_Glyph m_cached_Ellipsis_GlyphInfo;

	protected TMP_SpriteAsset m_defaultSpriteAsset;

	protected TMP_SpriteAsset m_currentSpriteAsset;

	protected int m_spriteCount;

	protected int m_spriteIndex;

	protected InlineGraphicManager m_inlineGraphics;

	protected bool m_ignoreActiveState;

	private readonly float[] k_Power = new float[10] { 0.5f, 0.05f, 0.005f, 0.0005f, 5E-05f, 5E-06f, 5E-07f, 5E-08f, 5E-09f, 5E-10f };

	protected static Vector2 k_LargePositiveVector2 = new Vector2(2.1474836E+09f, 2.1474836E+09f);

	protected static Vector2 k_LargeNegativeVector2 = new Vector2(-2.1474836E+09f, -2.1474836E+09f);

	protected static float k_LargePositiveFloat = 32768f;

	protected static float k_LargeNegativeFloat = -32768f;

	protected static int k_LargePositiveInt = int.MaxValue;

	protected static int k_LargeNegativeInt = -2147483647;

	public string text
	{
		get
		{
			return m_text;
		}
		set
		{
			if (!(m_text == value))
			{
				m_text = value;
				m_inputSource = TextInputSources.String;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				m_isInputParsingRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public bool isRightToLeftText
	{
		get
		{
			return m_isRightToLeft;
		}
		set
		{
			if (m_isRightToLeft != value)
			{
				m_isRightToLeft = value;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				m_isInputParsingRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public TMP_FontAsset font
	{
		get
		{
			return m_fontAsset;
		}
		set
		{
			if (!((Object)(object)m_fontAsset == (Object)(object)value))
			{
				m_fontAsset = value;
				LoadFontAsset();
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				m_isInputParsingRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public virtual Material fontSharedMaterial
	{
		get
		{
			return m_sharedMaterial;
		}
		set
		{
			if (!((Object)(object)m_sharedMaterial == (Object)(object)value))
			{
				SetSharedMaterial(value);
				m_havePropertiesChanged = true;
				m_isInputParsingRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetMaterialDirty();
			}
		}
	}

	public virtual Material[] fontSharedMaterials
	{
		get
		{
			return GetSharedMaterials();
		}
		set
		{
			SetSharedMaterials(value);
			m_havePropertiesChanged = true;
			m_isInputParsingRequired = true;
			((Graphic)this).SetVerticesDirty();
			((Graphic)this).SetMaterialDirty();
		}
	}

	public Material fontMaterial
	{
		get
		{
			return GetMaterial(m_sharedMaterial);
		}
		set
		{
			if (!((Object)(object)m_sharedMaterial != (Object)null) || ((Object)m_sharedMaterial).GetInstanceID() != ((Object)value).GetInstanceID())
			{
				m_sharedMaterial = value;
				m_padding = GetPaddingForMaterial();
				m_havePropertiesChanged = true;
				m_isInputParsingRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetMaterialDirty();
			}
		}
	}

	public virtual Material[] fontMaterials
	{
		get
		{
			return GetMaterials(m_fontSharedMaterials);
		}
		set
		{
			SetSharedMaterials(value);
			m_havePropertiesChanged = true;
			m_isInputParsingRequired = true;
			((Graphic)this).SetVerticesDirty();
			((Graphic)this).SetMaterialDirty();
		}
	}

	public override Color color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_fontColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!(m_fontColor == value))
			{
				m_havePropertiesChanged = true;
				m_fontColor = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public float alpha
	{
		get
		{
			return m_fontColor.a;
		}
		set
		{
			if (m_fontColor.a != value)
			{
				m_fontColor.a = value;
				m_havePropertiesChanged = true;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public bool enableVertexGradient
	{
		get
		{
			return m_enableVertexGradient;
		}
		set
		{
			if (m_enableVertexGradient != value)
			{
				m_havePropertiesChanged = true;
				m_enableVertexGradient = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public VertexGradient colorGradient
	{
		get
		{
			return m_fontColorGradient;
		}
		set
		{
			m_havePropertiesChanged = true;
			m_fontColorGradient = value;
			((Graphic)this).SetVerticesDirty();
		}
	}

	public TMP_ColorGradient colorGradientPreset
	{
		get
		{
			return m_fontColorGradientPreset;
		}
		set
		{
			m_havePropertiesChanged = true;
			m_fontColorGradientPreset = value;
			((Graphic)this).SetVerticesDirty();
		}
	}

	public TMP_SpriteAsset spriteAsset
	{
		get
		{
			return m_spriteAsset;
		}
		set
		{
			m_spriteAsset = value;
		}
	}

	public bool tintAllSprites
	{
		get
		{
			return m_tintAllSprites;
		}
		set
		{
			if (m_tintAllSprites != value)
			{
				m_tintAllSprites = value;
				m_havePropertiesChanged = true;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public bool overrideColorTags
	{
		get
		{
			return m_overrideHtmlColors;
		}
		set
		{
			if (m_overrideHtmlColors != value)
			{
				m_havePropertiesChanged = true;
				m_overrideHtmlColors = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public Color32 faceColor
	{
		get
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_sharedMaterial == (Object)null)
			{
				return m_faceColor;
			}
			m_faceColor = Color32.op_Implicit(m_sharedMaterial.GetColor(ShaderUtilities.ID_FaceColor));
			return m_faceColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (!m_faceColor.Compare(value))
			{
				SetFaceColor(value);
				m_havePropertiesChanged = true;
				m_faceColor = value;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetMaterialDirty();
			}
		}
	}

	public Color32 outlineColor
	{
		get
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_sharedMaterial == (Object)null)
			{
				return m_outlineColor;
			}
			m_outlineColor = Color32.op_Implicit(m_sharedMaterial.GetColor(ShaderUtilities.ID_OutlineColor));
			return m_outlineColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (!m_outlineColor.Compare(value))
			{
				SetOutlineColor(value);
				m_havePropertiesChanged = true;
				m_outlineColor = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public float outlineWidth
	{
		get
		{
			if ((Object)(object)m_sharedMaterial == (Object)null)
			{
				return m_outlineWidth;
			}
			m_outlineWidth = m_sharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth);
			return m_outlineWidth;
		}
		set
		{
			if (m_outlineWidth != value)
			{
				SetOutlineThickness(value);
				m_havePropertiesChanged = true;
				m_outlineWidth = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public float fontSize
	{
		get
		{
			return m_fontSize;
		}
		set
		{
			if (m_fontSize != value)
			{
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
				m_fontSize = value;
				if (!m_enableAutoSizing)
				{
					m_fontSizeBase = m_fontSize;
				}
			}
		}
	}

	public float fontScale => m_fontScale;

	public int fontWeight
	{
		get
		{
			return m_fontWeight;
		}
		set
		{
			if (m_fontWeight != value)
			{
				m_fontWeight = value;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public float pixelsPerUnit
	{
		get
		{
			Canvas canvas = ((Graphic)this).canvas;
			if (!Object.op_Implicit((Object)(object)canvas))
			{
				return 1f;
			}
			if (!Object.op_Implicit((Object)(object)font))
			{
				return canvas.scaleFactor;
			}
			if ((Object)(object)m_currentFontAsset == (Object)null || m_currentFontAsset.fontInfo.PointSize <= 0f || m_fontSize <= 0f)
			{
				return 1f;
			}
			return m_fontSize / m_currentFontAsset.fontInfo.PointSize;
		}
	}

	public bool enableAutoSizing
	{
		get
		{
			return m_enableAutoSizing;
		}
		set
		{
			if (m_enableAutoSizing != value)
			{
				m_enableAutoSizing = value;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public float fontSizeMin
	{
		get
		{
			return m_fontSizeMin;
		}
		set
		{
			if (m_fontSizeMin != value)
			{
				m_fontSizeMin = value;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public float fontSizeMax
	{
		get
		{
			return m_fontSizeMax;
		}
		set
		{
			if (m_fontSizeMax != value)
			{
				m_fontSizeMax = value;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public FontStyles fontStyle
	{
		get
		{
			return m_fontStyle;
		}
		set
		{
			if (m_fontStyle != value)
			{
				m_fontStyle = value;
				m_havePropertiesChanged = true;
				checkPaddingRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public bool isUsingBold => m_isUsingBold;

	public TextAlignmentOptions alignment
	{
		get
		{
			return m_textAlignment;
		}
		set
		{
			if (m_textAlignment != value)
			{
				m_havePropertiesChanged = true;
				m_textAlignment = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public float characterSpacing
	{
		get
		{
			return m_characterSpacing;
		}
		set
		{
			if (m_characterSpacing != value)
			{
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
				m_characterSpacing = value;
			}
		}
	}

	public float lineSpacing
	{
		get
		{
			return m_lineSpacing;
		}
		set
		{
			if (m_lineSpacing != value)
			{
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
				m_lineSpacing = value;
			}
		}
	}

	public float paragraphSpacing
	{
		get
		{
			return m_paragraphSpacing;
		}
		set
		{
			if (m_paragraphSpacing != value)
			{
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
				m_paragraphSpacing = value;
			}
		}
	}

	public float characterWidthAdjustment
	{
		get
		{
			return m_charWidthMaxAdj;
		}
		set
		{
			if (m_charWidthMaxAdj != value)
			{
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
				m_charWidthMaxAdj = value;
			}
		}
	}

	public bool enableWordWrapping
	{
		get
		{
			return m_enableWordWrapping;
		}
		set
		{
			if (m_enableWordWrapping != value)
			{
				m_havePropertiesChanged = true;
				m_isInputParsingRequired = true;
				m_isCalculateSizeRequired = true;
				m_enableWordWrapping = value;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public float wordWrappingRatios
	{
		get
		{
			return m_wordWrappingRatios;
		}
		set
		{
			if (m_wordWrappingRatios != value)
			{
				m_wordWrappingRatios = value;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public bool enableAdaptiveJustification
	{
		get
		{
			return m_enableAdaptiveJustification;
		}
		set
		{
			if (m_enableAdaptiveJustification != value)
			{
				m_enableAdaptiveJustification = value;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public TextOverflowModes OverflowMode
	{
		get
		{
			return m_overflowMode;
		}
		set
		{
			if (m_overflowMode != value)
			{
				m_overflowMode = value;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public bool enableKerning
	{
		get
		{
			return m_enableKerning;
		}
		set
		{
			if (m_enableKerning != value)
			{
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
				m_enableKerning = value;
			}
		}
	}

	public bool extraPadding
	{
		get
		{
			return m_enableExtraPadding;
		}
		set
		{
			if (m_enableExtraPadding != value)
			{
				m_havePropertiesChanged = true;
				m_enableExtraPadding = value;
				UpdateMeshPadding();
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public bool richText
	{
		get
		{
			return m_isRichText;
		}
		set
		{
			if (m_isRichText != value)
			{
				m_isRichText = value;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
				m_isInputParsingRequired = true;
			}
		}
	}

	public bool parseCtrlCharacters
	{
		get
		{
			return m_parseCtrlCharacters;
		}
		set
		{
			if (m_parseCtrlCharacters != value)
			{
				m_parseCtrlCharacters = value;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
				m_isInputParsingRequired = true;
			}
		}
	}

	public bool isOverlay
	{
		get
		{
			return m_isOverlay;
		}
		set
		{
			if (m_isOverlay != value)
			{
				m_isOverlay = value;
				SetShaderDepth();
				m_havePropertiesChanged = true;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public bool isOrthographic
	{
		get
		{
			return m_isOrthographic;
		}
		set
		{
			if (m_isOrthographic != value)
			{
				m_havePropertiesChanged = true;
				m_isOrthographic = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public bool enableCulling
	{
		get
		{
			return m_isCullingEnabled;
		}
		set
		{
			if (m_isCullingEnabled != value)
			{
				m_isCullingEnabled = value;
				SetCulling();
				m_havePropertiesChanged = true;
			}
		}
	}

	public bool ignoreVisibility
	{
		get
		{
			return m_ignoreCulling;
		}
		set
		{
			if (m_ignoreCulling != value)
			{
				m_havePropertiesChanged = true;
				m_ignoreCulling = value;
			}
		}
	}

	public TextureMappingOptions horizontalMapping
	{
		get
		{
			return m_horizontalMapping;
		}
		set
		{
			if (m_horizontalMapping != value)
			{
				m_havePropertiesChanged = true;
				m_horizontalMapping = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public TextureMappingOptions verticalMapping
	{
		get
		{
			return m_verticalMapping;
		}
		set
		{
			if (m_verticalMapping != value)
			{
				m_havePropertiesChanged = true;
				m_verticalMapping = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public TextRenderFlags renderMode
	{
		get
		{
			return m_renderMode;
		}
		set
		{
			if (m_renderMode != value)
			{
				m_renderMode = value;
				m_havePropertiesChanged = true;
			}
		}
	}

	public int maxVisibleCharacters
	{
		get
		{
			return m_maxVisibleCharacters;
		}
		set
		{
			if (m_maxVisibleCharacters != value)
			{
				m_havePropertiesChanged = true;
				m_maxVisibleCharacters = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public int maxVisibleWords
	{
		get
		{
			return m_maxVisibleWords;
		}
		set
		{
			if (m_maxVisibleWords != value)
			{
				m_havePropertiesChanged = true;
				m_maxVisibleWords = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public int maxVisibleLines
	{
		get
		{
			return m_maxVisibleLines;
		}
		set
		{
			if (m_maxVisibleLines != value)
			{
				m_havePropertiesChanged = true;
				m_isInputParsingRequired = true;
				m_maxVisibleLines = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public bool useMaxVisibleDescender
	{
		get
		{
			return m_useMaxVisibleDescender;
		}
		set
		{
			if (m_useMaxVisibleDescender != value)
			{
				m_havePropertiesChanged = true;
				m_isInputParsingRequired = true;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public int pageToDisplay
	{
		get
		{
			return m_pageToDisplay;
		}
		set
		{
			if (m_pageToDisplay != value)
			{
				m_havePropertiesChanged = true;
				m_pageToDisplay = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public virtual Vector4 margin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_margin;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (!(m_margin == value))
			{
				m_margin = value;
				ComputeMarginSize();
				m_havePropertiesChanged = true;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public TMP_TextInfo textInfo => m_textInfo;

	public bool havePropertiesChanged
	{
		get
		{
			return m_havePropertiesChanged;
		}
		set
		{
			if (m_havePropertiesChanged != value)
			{
				m_havePropertiesChanged = value;
				m_isInputParsingRequired = true;
				((Graphic)this).SetAllDirty();
			}
		}
	}

	public bool isUsingLegacyAnimationComponent
	{
		get
		{
			return m_isUsingLegacyAnimationComponent;
		}
		set
		{
			m_isUsingLegacyAnimationComponent = value;
		}
	}

	public Transform transform
	{
		get
		{
			if ((Object)(object)m_transform == (Object)null)
			{
				m_transform = ((Component)this).GetComponent<Transform>();
			}
			return m_transform;
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

	public virtual bool autoSizeTextContainer { get; set; }

	public virtual Mesh mesh => m_mesh;

	public bool isVolumetricText
	{
		get
		{
			return m_isVolumetricText;
		}
		set
		{
			if (m_isVolumetricText != value)
			{
				m_havePropertiesChanged = value;
				m_textInfo.ResetVertexLayout(value);
				m_isInputParsingRequired = true;
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetLayoutDirty();
			}
		}
	}

	public Bounds bounds
	{
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_mesh == (Object)null)
			{
				return default(Bounds);
			}
			return GetCompoundBounds();
		}
	}

	public Bounds textBounds
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (m_textInfo == null)
			{
				return default(Bounds);
			}
			return GetTextBounds();
		}
	}

	public float flexibleHeight => m_flexibleHeight;

	public float flexibleWidth => m_flexibleWidth;

	public float minHeight => m_minHeight;

	public float minWidth => m_minWidth;

	public virtual float preferredWidth
	{
		get
		{
			if (!m_isPreferredWidthDirty)
			{
				return m_preferredWidth;
			}
			m_preferredWidth = GetPreferredWidth();
			return m_preferredWidth;
		}
	}

	public virtual float preferredHeight
	{
		get
		{
			if (!m_isPreferredHeightDirty)
			{
				return m_preferredHeight;
			}
			m_preferredHeight = GetPreferredHeight();
			return m_preferredHeight;
		}
	}

	public virtual float renderedWidth => GetRenderedWidth();

	public virtual float renderedHeight => GetRenderedHeight();

	public int layoutPriority => m_layoutPriority;

	protected virtual void LoadFontAsset()
	{
	}

	protected virtual void SetSharedMaterial(Material mat)
	{
	}

	protected virtual Material GetMaterial(Material mat)
	{
		return null;
	}

	protected virtual void SetFontBaseMaterial(Material mat)
	{
	}

	protected virtual Material[] GetSharedMaterials()
	{
		return null;
	}

	protected virtual void SetSharedMaterials(Material[] materials)
	{
	}

	protected virtual Material[] GetMaterials(Material[] mats)
	{
		return null;
	}

	protected virtual Material CreateMaterialInstance(Material source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Material val = new Material(source);
		val.shaderKeywords = source.shaderKeywords;
		((Object)val).name = ((Object)val).name + " (Instance)";
		return val;
	}

	protected void SetVertexColorGradient(TMP_ColorGradient gradient)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)gradient == (Object)null))
		{
			m_fontColorGradient.bottomLeft = gradient.bottomLeft;
			m_fontColorGradient.bottomRight = gradient.bottomRight;
			m_fontColorGradient.topLeft = gradient.topLeft;
			m_fontColorGradient.topRight = gradient.topRight;
			((Graphic)this).SetVerticesDirty();
		}
	}

	protected virtual void SetFaceColor(Color32 color)
	{
	}

	protected virtual void SetOutlineColor(Color32 color)
	{
	}

	protected virtual void SetOutlineThickness(float thickness)
	{
	}

	protected virtual void SetShaderDepth()
	{
	}

	protected virtual void SetCulling()
	{
	}

	protected virtual float GetPaddingForMaterial()
	{
		return 0f;
	}

	protected virtual float GetPaddingForMaterial(Material mat)
	{
		return 0f;
	}

	protected virtual Vector3[] GetTextContainerLocalCorners()
	{
		return null;
	}

	public virtual void ForceMeshUpdate()
	{
	}

	public virtual void ForceMeshUpdate(bool ignoreActiveState)
	{
	}

	internal void SetTextInternal(string text)
	{
		m_text = text;
		m_renderMode = TextRenderFlags.DontRender;
		m_isInputParsingRequired = true;
		ForceMeshUpdate();
		m_renderMode = TextRenderFlags.Render;
	}

	public virtual void UpdateGeometry(Mesh mesh, int index)
	{
	}

	public virtual void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
	{
	}

	public virtual void UpdateVertexData()
	{
	}

	public virtual void SetVertices(Vector3[] vertices)
	{
	}

	public virtual void UpdateMeshPadding()
	{
	}

	public void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)this).CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
		InternalCrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
	}

	public void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
	{
		((Graphic)this).CrossFadeAlpha(alpha, duration, ignoreTimeScale);
		InternalCrossFadeAlpha(alpha, duration, ignoreTimeScale);
	}

	protected virtual void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
	{
	}

	protected virtual void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
	{
	}

	protected void ParseInputText()
	{
		m_isInputParsingRequired = false;
		switch (m_inputSource)
		{
		case TextInputSources.Text:
		case TextInputSources.String:
			StringToCharArray(m_text, ref m_char_buffer);
			break;
		case TextInputSources.SetText:
			SetTextArrayToCharArray(m_input_CharArray, ref m_char_buffer);
			break;
		}
		SetArraySizes(m_char_buffer);
	}

	public void SetText(string text)
	{
		m_inputSource = TextInputSources.SetCharArray;
		StringToCharArray(text, ref m_char_buffer);
		m_isInputParsingRequired = true;
		m_havePropertiesChanged = true;
		m_isCalculateSizeRequired = true;
		((Graphic)this).SetVerticesDirty();
		((Graphic)this).SetLayoutDirty();
	}

	public void SetText(string text, float arg0)
	{
		SetText(text, arg0, 255f, 255f);
	}

	public void SetText(string text, float arg0, float arg1)
	{
		SetText(text, arg0, arg1, 255f);
	}

	public void SetText(string text, float arg0, float arg1, float arg2)
	{
		if (text == old_text && arg0 == old_arg0 && arg1 == old_arg1 && arg2 == old_arg2)
		{
			return;
		}
		old_text = text;
		old_arg1 = 255f;
		old_arg2 = 255f;
		int precision = 0;
		int index = 0;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '{')
			{
				if (text[i + 2] == ':')
				{
					precision = text[i + 3] - 48;
				}
				switch (text[i + 1] - 48)
				{
				case 0:
					old_arg0 = arg0;
					AddFloatToCharArray(arg0, ref index, precision);
					break;
				case 1:
					old_arg1 = arg1;
					AddFloatToCharArray(arg1, ref index, precision);
					break;
				case 2:
					old_arg2 = arg2;
					AddFloatToCharArray(arg2, ref index, precision);
					break;
				}
				i = ((text[i + 2] != ':') ? (i + 2) : (i + 4));
			}
			else
			{
				m_input_CharArray[index] = c;
				index++;
			}
		}
		m_input_CharArray[index] = '\0';
		m_charArray_Length = index;
		m_inputSource = TextInputSources.SetText;
		m_isInputParsingRequired = true;
		m_havePropertiesChanged = true;
		m_isCalculateSizeRequired = true;
		((Graphic)this).SetVerticesDirty();
		((Graphic)this).SetLayoutDirty();
	}

	public void SetText(StringBuilder text)
	{
		m_inputSource = TextInputSources.SetCharArray;
		StringBuilderToIntArray(text, ref m_char_buffer);
		m_isInputParsingRequired = true;
		m_havePropertiesChanged = true;
		m_isCalculateSizeRequired = true;
		((Graphic)this).SetVerticesDirty();
		((Graphic)this).SetLayoutDirty();
	}

	public void SetCharArray(char[] charArray)
	{
		if (charArray == null || charArray.Length == 0)
		{
			return;
		}
		if (m_char_buffer.Length <= charArray.Length)
		{
			int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
			m_char_buffer = new int[num];
		}
		int num2 = 0;
		for (int i = 0; i < charArray.Length; i++)
		{
			if (charArray[i] == '\\' && i < charArray.Length - 1)
			{
				switch ((int)charArray[i + 1])
				{
				case 110:
					m_char_buffer[num2] = 10;
					i++;
					num2++;
					continue;
				case 114:
					m_char_buffer[num2] = 13;
					i++;
					num2++;
					continue;
				case 116:
					m_char_buffer[num2] = 9;
					i++;
					num2++;
					continue;
				}
			}
			m_char_buffer[num2] = charArray[i];
			num2++;
		}
		m_char_buffer[num2] = 0;
		m_inputSource = TextInputSources.SetCharArray;
		m_havePropertiesChanged = true;
		m_isInputParsingRequired = true;
	}

	protected void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
	{
		if (charArray == null || m_charArray_Length == 0)
		{
			return;
		}
		if (charBuffer.Length <= m_charArray_Length)
		{
			int num = ((m_charArray_Length <= 1024) ? Mathf.NextPowerOfTwo(m_charArray_Length + 1) : (m_charArray_Length + 256));
			charBuffer = new int[num];
		}
		int num2 = 0;
		for (int i = 0; i < m_charArray_Length; i++)
		{
			if (char.IsHighSurrogate(charArray[i]) && char.IsLowSurrogate(charArray[i + 1]))
			{
				charBuffer[num2] = char.ConvertToUtf32(charArray[i], charArray[i + 1]);
				i++;
				num2++;
			}
			else
			{
				charBuffer[num2] = charArray[i];
				num2++;
			}
		}
		charBuffer[num2] = 0;
	}

	protected void StringToCharArray(string text, ref int[] chars)
	{
		if (text == null)
		{
			chars[0] = 0;
			return;
		}
		if (chars == null || chars.Length <= text.Length)
		{
			int num = ((text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256));
			chars = new int[num];
		}
		int num2 = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (m_inputSource == TextInputSources.Text && text[i] == '\\' && text.Length > i + 1)
			{
				switch ((int)text[i + 1])
				{
				case 85:
					if (text.Length > i + 9)
					{
						chars[num2] = GetUTF32(i + 2);
						i += 9;
						num2++;
						continue;
					}
					break;
				case 92:
					if (!m_parseCtrlCharacters || text.Length <= i + 2)
					{
						break;
					}
					chars[num2] = text[i + 1];
					chars[num2 + 1] = text[i + 2];
					i += 2;
					num2 += 2;
					continue;
				case 110:
					if (!m_parseCtrlCharacters)
					{
						break;
					}
					chars[num2] = 10;
					i++;
					num2++;
					continue;
				case 114:
					if (!m_parseCtrlCharacters)
					{
						break;
					}
					chars[num2] = 13;
					i++;
					num2++;
					continue;
				case 116:
					if (!m_parseCtrlCharacters)
					{
						break;
					}
					chars[num2] = 9;
					i++;
					num2++;
					continue;
				case 117:
					if (text.Length > i + 5)
					{
						chars[num2] = (ushort)GetUTF16(i + 2);
						i += 5;
						num2++;
						continue;
					}
					break;
				}
			}
			if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
			{
				chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
				i++;
				num2++;
			}
			else
			{
				chars[num2] = text[i];
				num2++;
			}
		}
		chars[num2] = 0;
	}

	protected void StringBuilderToIntArray(StringBuilder text, ref int[] chars)
	{
		if (text == null)
		{
			chars[0] = 0;
			return;
		}
		if (chars == null || chars.Length <= text.Length)
		{
			int num = ((text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256));
			chars = new int[num];
		}
		int num2 = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (m_parseCtrlCharacters && text[i] == '\\' && text.Length > i + 1)
			{
				switch ((int)text[i + 1])
				{
				case 85:
					if (text.Length > i + 9)
					{
						chars[num2] = GetUTF32(i + 2);
						i += 9;
						num2++;
						continue;
					}
					break;
				case 92:
					if (text.Length <= i + 2)
					{
						break;
					}
					chars[num2] = text[i + 1];
					chars[num2 + 1] = text[i + 2];
					i += 2;
					num2 += 2;
					continue;
				case 110:
					chars[num2] = 10;
					i++;
					num2++;
					continue;
				case 114:
					chars[num2] = 13;
					i++;
					num2++;
					continue;
				case 116:
					chars[num2] = 9;
					i++;
					num2++;
					continue;
				case 117:
					if (text.Length > i + 5)
					{
						chars[num2] = (ushort)GetUTF16(i + 2);
						i += 5;
						num2++;
						continue;
					}
					break;
				}
			}
			if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
			{
				chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
				i++;
				num2++;
			}
			else
			{
				chars[num2] = text[i];
				num2++;
			}
		}
		chars[num2] = 0;
	}

	protected void AddFloatToCharArray(float number, ref int index, int precision)
	{
		if (number < 0f)
		{
			m_input_CharArray[index++] = '-';
			number = 0f - number;
		}
		number += k_Power[Mathf.Min(9, precision)];
		int num = (int)number;
		AddIntToCharArray(num, ref index, precision);
		if (precision > 0)
		{
			m_input_CharArray[index++] = '.';
			number -= (float)num;
			for (int i = 0; i < precision; i++)
			{
				number *= 10f;
				int num2 = (int)number;
				m_input_CharArray[index++] = (char)(num2 + 48);
				number -= (float)num2;
			}
		}
	}

	protected void AddIntToCharArray(int number, ref int index, int precision)
	{
		if (number < 0)
		{
			m_input_CharArray[index++] = '-';
			number = -number;
		}
		int num = index;
		do
		{
			m_input_CharArray[num++] = (char)(number % 10 + 48);
			number /= 10;
		}
		while (number > 0);
		int num2 = num;
		while (index + 1 < num)
		{
			num--;
			char c = m_input_CharArray[index];
			m_input_CharArray[index] = m_input_CharArray[num];
			m_input_CharArray[num] = c;
			index++;
		}
		index = num2;
	}

	protected virtual int SetArraySizes(int[] chars)
	{
		return 0;
	}

	protected virtual void GenerateTextMesh()
	{
	}

	public Vector2 GetPreferredValues()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (m_isInputParsingRequired || m_isTextTruncated)
		{
			m_isCalculatingPreferredValues = true;
			ParseInputText();
		}
		float num = GetPreferredWidth();
		float num2 = GetPreferredHeight();
		return new Vector2(num, num2);
	}

	public Vector2 GetPreferredValues(float width, float height)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (m_isInputParsingRequired || m_isTextTruncated)
		{
			m_isCalculatingPreferredValues = true;
			ParseInputText();
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(width, height);
		float num = GetPreferredWidth(val);
		float num2 = GetPreferredHeight(val);
		return new Vector2(num, num2);
	}

	public Vector2 GetPreferredValues(string text)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		m_isCalculatingPreferredValues = true;
		StringToCharArray(text, ref m_char_buffer);
		SetArraySizes(m_char_buffer);
		Vector2 val = k_LargePositiveVector2;
		float num = GetPreferredWidth(val);
		float num2 = GetPreferredHeight(val);
		return new Vector2(num, num2);
	}

	public Vector2 GetPreferredValues(string text, float width, float height)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		m_isCalculatingPreferredValues = true;
		StringToCharArray(text, ref m_char_buffer);
		SetArraySizes(m_char_buffer);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(width, height);
		float num = GetPreferredWidth(val);
		float num2 = GetPreferredHeight(val);
		return new Vector2(num, num2);
	}

	protected float GetPreferredWidth()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		float defaultFontSize = ((!m_enableAutoSizing) ? m_fontSize : m_fontSizeMax);
		Vector2 marginSize = k_LargePositiveVector2;
		if (m_isInputParsingRequired || m_isTextTruncated)
		{
			m_isCalculatingPreferredValues = true;
			ParseInputText();
		}
		float x = CalculatePreferredValues(defaultFontSize, marginSize).x;
		m_isPreferredWidthDirty = false;
		return x;
	}

	protected float GetPreferredWidth(Vector2 margin)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		float defaultFontSize = ((!m_enableAutoSizing) ? m_fontSize : m_fontSizeMax);
		return CalculatePreferredValues(defaultFontSize, margin).x;
	}

	protected float GetPreferredHeight()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		float defaultFontSize = ((!m_enableAutoSizing) ? m_fontSize : m_fontSizeMax);
		Vector2 marginSize = default(Vector2);
		((Vector2)(ref marginSize))._002Ector((m_marginWidth == 0f) ? k_LargePositiveFloat : m_marginWidth, k_LargePositiveFloat);
		if (m_isInputParsingRequired || m_isTextTruncated)
		{
			m_isCalculatingPreferredValues = true;
			ParseInputText();
		}
		float y = CalculatePreferredValues(defaultFontSize, marginSize).y;
		m_isPreferredHeightDirty = false;
		return y;
	}

	protected float GetPreferredHeight(Vector2 margin)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		float defaultFontSize = ((!m_enableAutoSizing) ? m_fontSize : m_fontSizeMax);
		return CalculatePreferredValues(defaultFontSize, margin).y;
	}

	public Vector2 GetRenderedValues()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Bounds val = GetTextBounds();
		return Vector2.op_Implicit(((Bounds)(ref val)).size);
	}

	protected float GetRenderedWidth()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetRenderedValues().x;
	}

	protected float GetRenderedHeight()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetRenderedValues().y;
	}

	protected virtual Vector2 CalculatePreferredValues(float defaultFontSize, Vector2 marginSize)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_fontAsset == (Object)null || m_fontAsset.characterDictionary == null)
		{
			Debug.LogWarning((object)("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + ((Object)this).GetInstanceID()));
			return Vector2.zero;
		}
		if (m_char_buffer == null || m_char_buffer.Length == 0 || m_char_buffer[0] == 0)
		{
			return Vector2.zero;
		}
		m_currentFontAsset = m_fontAsset;
		m_currentMaterial = m_sharedMaterial;
		m_currentMaterialIndex = 0;
		m_materialReferenceStack.SetDefault(new MaterialReference(0, m_currentFontAsset, null, m_currentMaterial, m_padding));
		int totalCharacterCount = m_totalCharacterCount;
		if (m_internalCharacterInfo == null || totalCharacterCount > m_internalCharacterInfo.Length)
		{
			m_internalCharacterInfo = new TMP_CharacterInfo[(totalCharacterCount <= 1024) ? Mathf.NextPowerOfTwo(totalCharacterCount) : (totalCharacterCount + 256)];
		}
		m_fontScale = defaultFontSize / m_currentFontAsset.fontInfo.PointSize * ((!m_isOrthographic) ? 0.1f : 1f);
		m_fontScaleMultiplier = 1f;
		float num = defaultFontSize / m_fontAsset.fontInfo.PointSize * m_fontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
		float num2 = m_fontScale;
		m_currentFontSize = defaultFontSize;
		m_sizeStack.SetDefault(m_currentFontSize);
		int num3 = 0;
		m_style = m_fontStyle;
		float num4 = 1f;
		m_baselineOffset = 0f;
		m_styleStack.Clear();
		m_lineOffset = 0f;
		m_lineHeight = 0f;
		float num5 = m_currentFontAsset.fontInfo.LineHeight - (m_currentFontAsset.fontInfo.Ascender - m_currentFontAsset.fontInfo.Descender);
		m_cSpacing = 0f;
		m_monoSpacing = 0f;
		float num6 = 0f;
		m_xAdvance = 0f;
		float num7 = 0f;
		tag_LineIndent = 0f;
		tag_Indent = 0f;
		m_indentStack.SetDefault(0f);
		tag_NoParsing = false;
		m_characterCount = 0;
		m_firstCharacterOfLine = 0;
		m_maxLineAscender = k_LargeNegativeFloat;
		m_maxLineDescender = k_LargePositiveFloat;
		m_lineNumber = 0;
		float x = marginSize.x;
		m_marginLeft = 0f;
		m_marginRight = 0f;
		m_width = -1f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		m_maxAscender = 0f;
		m_maxDescender = 0f;
		bool flag = true;
		bool flag2 = false;
		WordWrapState state = default(WordWrapState);
		SaveWordWrappingState(ref state, 0, 0);
		WordWrapState state2 = default(WordWrapState);
		int num11 = 0;
		int endIndex = 0;
		for (int i = 0; m_char_buffer[i] != 0; i++)
		{
			num3 = m_char_buffer[i];
			m_textElementType = TMP_TextElementType.Character;
			m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
			m_currentFontAsset = m_materialReferences[m_currentMaterialIndex].fontAsset;
			int currentMaterialIndex = m_currentMaterialIndex;
			if (m_isRichText && num3 == 60)
			{
				m_isParsingText = true;
				if (ValidateHtmlTag(m_char_buffer, i + 1, out endIndex))
				{
					i = endIndex;
					if (m_textElementType == TMP_TextElementType.Character)
					{
						continue;
					}
				}
			}
			m_isParsingText = false;
			bool isUsingAlternateTypeface = m_textInfo.characterInfo[m_characterCount].isUsingAlternateTypeface;
			float num12 = 1f;
			if (m_textElementType == TMP_TextElementType.Character)
			{
				if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)num3))
					{
						num3 = char.ToUpper((char)num3);
					}
				}
				else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)num3))
					{
						num3 = char.ToLower((char)num3);
					}
				}
				else if (((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num3))
				{
					num12 = 0.8f;
					num3 = char.ToUpper((char)num3);
				}
			}
			if (m_textElementType == TMP_TextElementType.Sprite)
			{
				TMP_Sprite tMP_Sprite = m_currentSpriteAsset.spriteInfoList[m_spriteIndex];
				if (tMP_Sprite == null)
				{
					continue;
				}
				num3 = 57344 + m_spriteIndex;
				m_currentFontAsset = m_fontAsset;
				float num13 = m_currentFontSize / m_fontAsset.fontInfo.PointSize * m_fontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				num2 = m_fontAsset.fontInfo.Ascender / tMP_Sprite.height * tMP_Sprite.scale * num13;
				m_cached_TextElement = tMP_Sprite;
				m_internalCharacterInfo[m_characterCount].elementType = TMP_TextElementType.Sprite;
				m_currentMaterialIndex = currentMaterialIndex;
			}
			else if (m_textElementType == TMP_TextElementType.Character)
			{
				m_cached_TextElement = m_textInfo.characterInfo[m_characterCount].textElement;
				if (m_cached_TextElement == null)
				{
					continue;
				}
				m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
				m_fontScale = m_currentFontSize * num12 / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				num2 = m_fontScale * m_fontScaleMultiplier * m_cached_TextElement.scale;
				m_internalCharacterInfo[m_characterCount].elementType = TMP_TextElementType.Character;
			}
			float num14 = num2;
			if (num3 == 173)
			{
				num2 = 0f;
			}
			m_internalCharacterInfo[m_characterCount].character = (char)num3;
			if (m_enableKerning && m_characterCount >= 1)
			{
				int character = m_internalCharacterInfo[m_characterCount - 1].character;
				KerningPairKey kerningPairKey = new KerningPairKey(character, num3);
				m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out var value);
				if (value != null)
				{
					m_xAdvance += value.XadvanceOffset * num2;
				}
			}
			float num15 = 0f;
			if (m_monoSpacing != 0f)
			{
				num15 = m_monoSpacing / 2f - (m_cached_TextElement.width / 2f + m_cached_TextElement.xOffset) * num2;
				m_xAdvance += num15;
			}
			num4 = ((m_textElementType != TMP_TextElementType.Character || isUsingAlternateTypeface || ((m_style & FontStyles.Bold) != FontStyles.Bold && (m_fontStyle & FontStyles.Bold) != FontStyles.Bold)) ? 1f : (1f + m_currentFontAsset.boldSpacing * 0.01f));
			m_internalCharacterInfo[m_characterCount].baseLine = 0f - m_lineOffset + m_baselineOffset;
			float num16 = m_currentFontAsset.fontInfo.Ascender * ((m_textElementType != TMP_TextElementType.Character) ? m_internalCharacterInfo[m_characterCount].scale : num2) + m_baselineOffset;
			m_internalCharacterInfo[m_characterCount].ascender = num16 - m_lineOffset;
			m_maxLineAscender = ((!(num16 > m_maxLineAscender)) ? m_maxLineAscender : num16);
			float num17 = m_currentFontAsset.fontInfo.Descender * ((m_textElementType != TMP_TextElementType.Character) ? m_internalCharacterInfo[m_characterCount].scale : num2) + m_baselineOffset;
			float num18 = (m_internalCharacterInfo[m_characterCount].descender = num17 - m_lineOffset);
			m_maxLineDescender = ((!(num17 < m_maxLineDescender)) ? m_maxLineDescender : num17);
			if ((m_style & FontStyles.Subscript) == FontStyles.Subscript || (m_style & FontStyles.Superscript) == FontStyles.Superscript)
			{
				float num19 = (num16 - m_baselineOffset) / m_currentFontAsset.fontInfo.SubSize;
				num16 = m_maxLineAscender;
				m_maxLineAscender = ((!(num19 > m_maxLineAscender)) ? m_maxLineAscender : num19);
				float num20 = (num17 - m_baselineOffset) / m_currentFontAsset.fontInfo.SubSize;
				num17 = m_maxLineDescender;
				m_maxLineDescender = ((!(num20 < m_maxLineDescender)) ? m_maxLineDescender : num20);
			}
			if (m_lineNumber == 0)
			{
				m_maxAscender = ((!(m_maxAscender > num16)) ? num16 : m_maxAscender);
			}
			if (num3 == 9 || !char.IsWhiteSpace((char)num3) || m_textElementType == TMP_TextElementType.Sprite)
			{
				float num21 = ((m_width == -1f) ? (x + 0.0001f - m_marginLeft - m_marginRight) : Mathf.Min(x + 0.0001f - m_marginLeft - m_marginRight, m_width));
				num10 = m_xAdvance + m_cached_TextElement.xAdvance * ((num3 == 173) ? num14 : num2);
				if (num10 > num21 && enableWordWrapping && m_characterCount != m_firstCharacterOfLine)
				{
					if (num11 == state2.previous_WordBreak || flag)
					{
						if (!m_isCharacterWrappingEnabled)
						{
							m_isCharacterWrappingEnabled = true;
						}
						else
						{
							flag2 = true;
						}
					}
					i = RestoreWordWrappingState(ref state2);
					num11 = i;
					if (m_char_buffer[i] == 173)
					{
						m_isTextTruncated = true;
						m_char_buffer[i] = 45;
						CalculatePreferredValues(defaultFontSize, marginSize);
						return Vector2.zero;
					}
					if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f)
					{
						float num22 = m_maxLineAscender - m_startOfLineAscender;
						m_lineOffset += num22;
						state2.lineOffset = m_lineOffset;
						state2.previousLineAscender = m_maxLineAscender;
					}
					float num23 = m_maxLineAscender - m_lineOffset;
					float num24 = m_maxLineDescender - m_lineOffset;
					m_maxDescender = ((!(m_maxDescender < num24)) ? num24 : m_maxDescender);
					m_firstCharacterOfLine = m_characterCount;
					num8 += m_xAdvance;
					num9 = ((!m_enableWordWrapping) ? Mathf.Max(num9, num23 - num24) : (m_maxAscender - m_maxDescender));
					SaveWordWrappingState(ref state, i, m_characterCount - 1);
					m_lineNumber++;
					if (m_lineHeight == 0f)
					{
						float num25 = m_internalCharacterInfo[m_characterCount].ascender - m_internalCharacterInfo[m_characterCount].baseLine;
						num6 = 0f - m_maxLineDescender + num25 + (num5 + m_lineSpacing + m_lineSpacingDelta) * num;
						m_lineOffset += num6;
						m_startOfLineAscender = num25;
					}
					else
					{
						m_lineOffset += m_lineHeight + m_lineSpacing * num;
					}
					m_maxLineAscender = k_LargeNegativeFloat;
					m_maxLineDescender = k_LargePositiveFloat;
					m_xAdvance = tag_Indent;
					continue;
				}
			}
			if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f && !m_isNewPage)
			{
				float num26 = m_maxLineAscender - m_startOfLineAscender;
				num18 -= num26;
				m_lineOffset += num26;
				m_startOfLineAscender += num26;
				state2.lineOffset = m_lineOffset;
				state2.previousLineAscender = m_startOfLineAscender;
			}
			if (num3 == 9)
			{
				float num27 = m_currentFontAsset.fontInfo.TabWidth * num2;
				float num28 = Mathf.Ceil(m_xAdvance / num27) * num27;
				m_xAdvance = ((!(num28 > m_xAdvance)) ? (m_xAdvance + num27) : num28);
			}
			else if (m_monoSpacing != 0f)
			{
				m_xAdvance += m_monoSpacing - num15 + (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing;
			}
			else
			{
				m_xAdvance += (m_cached_TextElement.xAdvance * num4 + m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing;
			}
			if (num3 == 13)
			{
				num7 = Mathf.Max(num7, num8 + m_xAdvance);
				num8 = 0f;
				m_xAdvance = tag_Indent;
			}
			if (num3 == 10 || m_characterCount == totalCharacterCount - 1)
			{
				if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f)
				{
					float num29 = m_maxLineAscender - m_startOfLineAscender;
					num18 -= num29;
					m_lineOffset += num29;
				}
				float num30 = m_maxLineDescender - m_lineOffset;
				m_maxDescender = ((!(m_maxDescender < num30)) ? num30 : m_maxDescender);
				m_firstCharacterOfLine = m_characterCount + 1;
				if (num3 == 10 && m_characterCount != totalCharacterCount - 1)
				{
					num7 = Mathf.Max(num7, num8 + num10);
					num8 = 0f;
				}
				else
				{
					num8 = Mathf.Max(num7, num8 + num10);
				}
				num9 = m_maxAscender - m_maxDescender;
				if (num3 == 10)
				{
					SaveWordWrappingState(ref state, i, m_characterCount);
					SaveWordWrappingState(ref state2, i, m_characterCount);
					m_lineNumber++;
					if (m_lineHeight == 0f)
					{
						num6 = 0f - m_maxLineDescender + num16 + (num5 + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * num;
						m_lineOffset += num6;
					}
					else
					{
						m_lineOffset += m_lineHeight + (m_lineSpacing + m_paragraphSpacing) * num;
					}
					m_maxLineAscender = k_LargeNegativeFloat;
					m_maxLineDescender = k_LargePositiveFloat;
					m_startOfLineAscender = num16;
					m_xAdvance = tag_LineIndent + tag_Indent;
				}
			}
			if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
			{
				if ((char.IsWhiteSpace((char)num3) || num3 == 45 || num3 == 173) && !m_isNonBreakingSpace && num3 != 160 && num3 != 8209 && num3 != 8239 && num3 != 8288)
				{
					SaveWordWrappingState(ref state2, i, m_characterCount);
					m_isCharacterWrappingEnabled = false;
					flag = false;
				}
				else if (((num3 > 4352 && num3 < 4607) || (num3 > 11904 && num3 < 40959) || (num3 > 43360 && num3 < 43391) || (num3 > 44032 && num3 < 55295) || (num3 > 63744 && num3 < 64255) || (num3 > 65072 && num3 < 65103) || (num3 > 65280 && num3 < 65519)) && !m_isNonBreakingSpace)
				{
					if (flag || flag2 || (!TMP_Settings.linebreakingRules.leadingCharacters.ContainsKey(num3) && m_characterCount < totalCharacterCount - 1 && !TMP_Settings.linebreakingRules.followingCharacters.ContainsKey(m_internalCharacterInfo[m_characterCount + 1].character)))
					{
						SaveWordWrappingState(ref state2, i, m_characterCount);
						m_isCharacterWrappingEnabled = false;
						flag = false;
					}
				}
				else if (flag || m_isCharacterWrappingEnabled || flag2)
				{
					SaveWordWrappingState(ref state2, i, m_characterCount);
				}
			}
			m_characterCount++;
		}
		m_isCharacterWrappingEnabled = false;
		num8 += ((!(m_margin.x > 0f)) ? 0f : m_margin.x);
		num8 += ((!(m_margin.z > 0f)) ? 0f : m_margin.z);
		num9 += ((!(m_margin.y > 0f)) ? 0f : m_margin.y);
		num9 += ((!(m_margin.w > 0f)) ? 0f : m_margin.w);
		num8 = (float)(int)(num8 * 100f + 1f) / 100f;
		num9 = (float)(int)(num9 * 100f + 1f) / 100f;
		return new Vector2(num8, num9);
	}

	protected virtual Bounds GetCompoundBounds()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return default(Bounds);
	}

	protected Bounds GetTextBounds()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		if (m_textInfo == null)
		{
			return default(Bounds);
		}
		Extents extents = new Extents(k_LargePositiveVector2, k_LargeNegativeVector2);
		for (int i = 0; i < m_textInfo.characterCount; i++)
		{
			if (m_textInfo.characterInfo[i].isVisible)
			{
				extents.min.x = Mathf.Min(extents.min.x, m_textInfo.characterInfo[i].bottomLeft.x);
				extents.min.y = Mathf.Min(extents.min.y, m_textInfo.characterInfo[i].descender);
				extents.max.x = Mathf.Max(extents.max.x, m_textInfo.characterInfo[i].xAdvance);
				extents.max.y = Mathf.Max(extents.max.y, m_textInfo.characterInfo[i].ascender);
			}
		}
		Vector2 val = default(Vector2);
		val.x = extents.max.x - extents.min.x;
		val.y = extents.max.y - extents.min.y;
		Vector2 val2 = (extents.min + extents.max) / 2f;
		return new Bounds(Vector2.op_Implicit(val2), Vector2.op_Implicit(val));
	}

	protected virtual void AdjustLineOffset(int startIndex, int endIndex, float offset)
	{
	}

	protected void ResizeLineExtents(int size)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		size = ((size <= 1024) ? Mathf.NextPowerOfTwo(size + 1) : (size + 256));
		TMP_LineInfo[] array = new TMP_LineInfo[size];
		for (int i = 0; i < size; i++)
		{
			if (i < m_textInfo.lineInfo.Length)
			{
				ref TMP_LineInfo reference = ref array[i];
				reference = m_textInfo.lineInfo[i];
				continue;
			}
			array[i].lineExtents.min = k_LargePositiveVector2;
			array[i].lineExtents.max = k_LargeNegativeVector2;
			array[i].ascender = k_LargeNegativeFloat;
			array[i].descender = k_LargePositiveFloat;
		}
		m_textInfo.lineInfo = array;
	}

	public virtual TMP_TextInfo GetTextInfo(string text)
	{
		return null;
	}

	protected virtual void ComputeMarginSize()
	{
	}

	protected int GetArraySizes(int[] chars)
	{
		int endIndex = 0;
		m_totalCharacterCount = 0;
		m_isUsingBold = false;
		m_isParsingText = false;
		for (int i = 0; chars[i] != 0; i++)
		{
			int num = chars[i];
			if (m_isRichText && num == 60 && ValidateHtmlTag(chars, i + 1, out endIndex))
			{
				i = endIndex;
				if ((m_style & FontStyles.Bold) == FontStyles.Bold)
				{
					m_isUsingBold = true;
				}
			}
			else
			{
				if (!char.IsWhiteSpace((char)num))
				{
				}
				m_totalCharacterCount++;
			}
		}
		return m_totalCharacterCount;
	}

	protected void SaveWordWrappingState(ref WordWrapState state, int index, int count)
	{
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		state.currentFontAsset = m_currentFontAsset;
		state.currentSpriteAsset = m_currentSpriteAsset;
		state.currentMaterial = m_currentMaterial;
		state.currentMaterialIndex = m_currentMaterialIndex;
		state.previous_WordBreak = index;
		state.total_CharacterCount = count;
		state.visible_CharacterCount = m_lineVisibleCharacterCount;
		state.visible_LinkCount = m_textInfo.linkCount;
		state.firstCharacterIndex = m_firstCharacterOfLine;
		state.firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
		state.lastVisibleCharIndex = m_lastVisibleCharacterOfLine;
		state.fontStyle = m_style;
		state.fontScale = m_fontScale;
		state.fontScaleMultiplier = m_fontScaleMultiplier;
		state.currentFontSize = m_currentFontSize;
		state.xAdvance = m_xAdvance;
		state.maxCapHeight = m_maxCapHeight;
		state.maxAscender = m_maxAscender;
		state.maxDescender = m_maxDescender;
		state.maxLineAscender = m_maxLineAscender;
		state.maxLineDescender = m_maxLineDescender;
		state.previousLineAscender = m_startOfLineAscender;
		state.preferredWidth = m_preferredWidth;
		state.preferredHeight = m_preferredHeight;
		state.meshExtents = m_meshExtents;
		state.lineNumber = m_lineNumber;
		state.lineOffset = m_lineOffset;
		state.baselineOffset = m_baselineOffset;
		state.vertexColor = m_htmlColor;
		state.tagNoParsing = tag_NoParsing;
		state.colorStack = m_colorStack;
		state.sizeStack = m_sizeStack;
		state.fontWeightStack = m_fontWeightStack;
		state.styleStack = m_styleStack;
		state.actionStack = m_actionStack;
		state.materialReferenceStack = m_materialReferenceStack;
		if (m_lineNumber < m_textInfo.lineInfo.Length)
		{
			state.lineInfo = m_textInfo.lineInfo[m_lineNumber];
		}
	}

	protected int RestoreWordWrappingState(ref WordWrapState state)
	{
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		int previous_WordBreak = state.previous_WordBreak;
		m_currentFontAsset = state.currentFontAsset;
		m_currentSpriteAsset = state.currentSpriteAsset;
		m_currentMaterial = state.currentMaterial;
		m_currentMaterialIndex = state.currentMaterialIndex;
		m_characterCount = state.total_CharacterCount + 1;
		m_lineVisibleCharacterCount = state.visible_CharacterCount;
		m_textInfo.linkCount = state.visible_LinkCount;
		m_firstCharacterOfLine = state.firstCharacterIndex;
		m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
		m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
		m_style = state.fontStyle;
		m_fontScale = state.fontScale;
		m_fontScaleMultiplier = state.fontScaleMultiplier;
		m_currentFontSize = state.currentFontSize;
		m_xAdvance = state.xAdvance;
		m_maxCapHeight = state.maxCapHeight;
		m_maxAscender = state.maxAscender;
		m_maxDescender = state.maxDescender;
		m_maxLineAscender = state.maxLineAscender;
		m_maxLineDescender = state.maxLineDescender;
		m_startOfLineAscender = state.previousLineAscender;
		m_preferredWidth = state.preferredWidth;
		m_preferredHeight = state.preferredHeight;
		m_meshExtents = state.meshExtents;
		m_lineNumber = state.lineNumber;
		m_lineOffset = state.lineOffset;
		m_baselineOffset = state.baselineOffset;
		m_htmlColor = state.vertexColor;
		tag_NoParsing = state.tagNoParsing;
		m_colorStack = state.colorStack;
		m_sizeStack = state.sizeStack;
		m_fontWeightStack = state.fontWeightStack;
		m_styleStack = state.styleStack;
		m_actionStack = state.actionStack;
		m_materialReferenceStack = state.materialReferenceStack;
		if (m_lineNumber < m_textInfo.lineInfo.Length)
		{
			ref TMP_LineInfo reference = ref m_textInfo.lineInfo[m_lineNumber];
			reference = state.lineInfo;
		}
		return previous_WordBreak;
	}

	protected virtual void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
		m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
		m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
		m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
		vertexColor.a = ((m_fontColor32.a >= vertexColor.a) ? vertexColor.a : m_fontColor32.a);
		if (!m_enableVertexGradient)
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
		}
		else if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
		}
		else if ((Object)(object)m_fontColorGradientPreset != (Object)null)
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.color = Color32.op_Implicit(m_fontColorGradientPreset.bottomLeft * Color32.op_Implicit(vertexColor));
			m_textInfo.characterInfo[m_characterCount].vertex_TL.color = Color32.op_Implicit(m_fontColorGradientPreset.topLeft * Color32.op_Implicit(vertexColor));
			m_textInfo.characterInfo[m_characterCount].vertex_TR.color = Color32.op_Implicit(m_fontColorGradientPreset.topRight * Color32.op_Implicit(vertexColor));
			m_textInfo.characterInfo[m_characterCount].vertex_BR.color = Color32.op_Implicit(m_fontColorGradientPreset.bottomRight * Color32.op_Implicit(vertexColor));
		}
		else
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.color = Color32.op_Implicit(m_fontColorGradient.bottomLeft * Color32.op_Implicit(vertexColor));
			m_textInfo.characterInfo[m_characterCount].vertex_TL.color = Color32.op_Implicit(m_fontColorGradient.topLeft * Color32.op_Implicit(vertexColor));
			m_textInfo.characterInfo[m_characterCount].vertex_TR.color = Color32.op_Implicit(m_fontColorGradient.topRight * Color32.op_Implicit(vertexColor));
			m_textInfo.characterInfo[m_characterCount].vertex_BR.color = Color32.op_Implicit(m_fontColorGradient.bottomRight * Color32.op_Implicit(vertexColor));
		}
		if (!m_isSDFShader)
		{
			style_padding = 0f;
		}
		FaceInfo fontInfo = m_currentFontAsset.fontInfo;
		Vector2 uv = default(Vector2);
		uv.x = (m_cached_TextElement.x - padding - style_padding) / fontInfo.AtlasWidth;
		uv.y = 1f - (m_cached_TextElement.y + padding + style_padding + m_cached_TextElement.height) / fontInfo.AtlasHeight;
		Vector2 uv2 = default(Vector2);
		uv2.x = uv.x;
		uv2.y = 1f - (m_cached_TextElement.y - padding - style_padding) / fontInfo.AtlasHeight;
		Vector2 uv3 = default(Vector2);
		uv3.x = (m_cached_TextElement.x + padding + style_padding + m_cached_TextElement.width) / fontInfo.AtlasWidth;
		uv3.y = uv2.y;
		Vector2 uv4 = default(Vector2);
		uv4.x = uv3.x;
		uv4.y = uv.y;
		m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv;
		m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv2;
		m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv3;
		m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv4;
	}

	protected virtual void SaveSpriteVertexInfo(Color32 vertexColor)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
		m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
		m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
		m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
		if (m_tintAllSprites)
		{
			m_tintSprite = true;
		}
		Color32 val = ((!m_tintSprite) ? m_spriteColor : m_spriteColor.Multiply(vertexColor));
		val.a = ((val.a >= m_fontColor32.a) ? m_fontColor32.a : (val.a = ((val.a >= vertexColor.a) ? vertexColor.a : val.a)));
		if (!m_enableVertexGradient)
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.color = val;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.color = val;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.color = val;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.color = val;
		}
		else if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.color = val;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.color = val;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.color = val;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.color = val;
		}
		else if ((Object)(object)m_fontColorGradientPreset != (Object)null)
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.color = ((!m_tintSprite) ? val : val.Multiply(Color32.op_Implicit(m_fontColorGradientPreset.bottomLeft)));
			m_textInfo.characterInfo[m_characterCount].vertex_TL.color = ((!m_tintSprite) ? val : val.Multiply(Color32.op_Implicit(m_fontColorGradientPreset.topLeft)));
			m_textInfo.characterInfo[m_characterCount].vertex_TR.color = ((!m_tintSprite) ? val : val.Multiply(Color32.op_Implicit(m_fontColorGradientPreset.topRight)));
			m_textInfo.characterInfo[m_characterCount].vertex_BR.color = ((!m_tintSprite) ? val : val.Multiply(Color32.op_Implicit(m_fontColorGradientPreset.bottomRight)));
		}
		else
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.color = ((!m_tintSprite) ? val : val.Multiply(Color32.op_Implicit(m_fontColorGradient.bottomLeft)));
			m_textInfo.characterInfo[m_characterCount].vertex_TL.color = ((!m_tintSprite) ? val : val.Multiply(Color32.op_Implicit(m_fontColorGradient.topLeft)));
			m_textInfo.characterInfo[m_characterCount].vertex_TR.color = ((!m_tintSprite) ? val : val.Multiply(Color32.op_Implicit(m_fontColorGradient.topRight)));
			m_textInfo.characterInfo[m_characterCount].vertex_BR.color = ((!m_tintSprite) ? val : val.Multiply(Color32.op_Implicit(m_fontColorGradient.bottomRight)));
		}
		Vector2 uv = default(Vector2);
		((Vector2)(ref uv))._002Ector(m_cached_TextElement.x / (float)m_currentSpriteAsset.spriteSheet.width, m_cached_TextElement.y / (float)m_currentSpriteAsset.spriteSheet.height);
		Vector2 uv2 = default(Vector2);
		((Vector2)(ref uv2))._002Ector(uv.x, (m_cached_TextElement.y + m_cached_TextElement.height) / (float)m_currentSpriteAsset.spriteSheet.height);
		Vector2 uv3 = default(Vector2);
		((Vector2)(ref uv3))._002Ector((m_cached_TextElement.x + m_cached_TextElement.width) / (float)m_currentSpriteAsset.spriteSheet.width, uv2.y);
		Vector2 uv4 = default(Vector2);
		((Vector2)(ref uv4))._002Ector(uv3.x, uv.y);
		m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv;
		m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv2;
		m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv3;
		m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv4;
	}

	protected virtual void FillCharacterVertexBuffers(int i, int index_X4)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		int materialReferenceIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
		index_X4 = m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
		TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
		m_textInfo.characterInfo[i].vertexIndex = index_X4;
		ref Vector3 reference = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4];
		reference = characterInfo[i].vertex_BL.position;
		ref Vector3 reference2 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4];
		reference2 = characterInfo[i].vertex_TL.position;
		ref Vector3 reference3 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4];
		reference3 = characterInfo[i].vertex_TR.position;
		ref Vector3 reference4 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4];
		reference4 = characterInfo[i].vertex_BR.position;
		ref Vector2 reference5 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4];
		reference5 = characterInfo[i].vertex_BL.uv;
		ref Vector2 reference6 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4];
		reference6 = characterInfo[i].vertex_TL.uv;
		ref Vector2 reference7 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4];
		reference7 = characterInfo[i].vertex_TR.uv;
		ref Vector2 reference8 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4];
		reference8 = characterInfo[i].vertex_BR.uv;
		ref Vector2 reference9 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4];
		reference9 = characterInfo[i].vertex_BL.uv2;
		ref Vector2 reference10 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4];
		reference10 = characterInfo[i].vertex_TL.uv2;
		ref Vector2 reference11 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4];
		reference11 = characterInfo[i].vertex_TR.uv2;
		ref Vector2 reference12 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4];
		reference12 = characterInfo[i].vertex_BR.uv2;
		ref Color32 reference13 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4];
		reference13 = characterInfo[i].vertex_BL.color;
		ref Color32 reference14 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4];
		reference14 = characterInfo[i].vertex_TL.color;
		ref Color32 reference15 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4];
		reference15 = characterInfo[i].vertex_TR.color;
		ref Color32 reference16 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4];
		reference16 = characterInfo[i].vertex_BR.color;
		m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
	}

	protected virtual void FillCharacterVertexBuffers(int i, int index_X4, bool isVolumetric)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		int materialReferenceIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
		index_X4 = m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
		TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
		m_textInfo.characterInfo[i].vertexIndex = index_X4;
		ref Vector3 reference = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4];
		reference = characterInfo[i].vertex_BL.position;
		ref Vector3 reference2 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4];
		reference2 = characterInfo[i].vertex_TL.position;
		ref Vector3 reference3 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4];
		reference3 = characterInfo[i].vertex_TR.position;
		ref Vector3 reference4 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4];
		reference4 = characterInfo[i].vertex_BR.position;
		if (isVolumetric)
		{
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(0f, 0f, m_fontSize * m_fontScale);
			ref Vector3 reference5 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[4 + index_X4];
			reference5 = characterInfo[i].vertex_BL.position + val;
			ref Vector3 reference6 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[5 + index_X4];
			reference6 = characterInfo[i].vertex_TL.position + val;
			ref Vector3 reference7 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[6 + index_X4];
			reference7 = characterInfo[i].vertex_TR.position + val;
			ref Vector3 reference8 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[7 + index_X4];
			reference8 = characterInfo[i].vertex_BR.position + val;
		}
		ref Vector2 reference9 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4];
		reference9 = characterInfo[i].vertex_BL.uv;
		ref Vector2 reference10 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4];
		reference10 = characterInfo[i].vertex_TL.uv;
		ref Vector2 reference11 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4];
		reference11 = characterInfo[i].vertex_TR.uv;
		ref Vector2 reference12 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4];
		reference12 = characterInfo[i].vertex_BR.uv;
		if (isVolumetric)
		{
			ref Vector2 reference13 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[4 + index_X4];
			reference13 = characterInfo[i].vertex_BL.uv;
			ref Vector2 reference14 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[5 + index_X4];
			reference14 = characterInfo[i].vertex_TL.uv;
			ref Vector2 reference15 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[6 + index_X4];
			reference15 = characterInfo[i].vertex_TR.uv;
			ref Vector2 reference16 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[7 + index_X4];
			reference16 = characterInfo[i].vertex_BR.uv;
		}
		ref Vector2 reference17 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4];
		reference17 = characterInfo[i].vertex_BL.uv2;
		ref Vector2 reference18 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4];
		reference18 = characterInfo[i].vertex_TL.uv2;
		ref Vector2 reference19 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4];
		reference19 = characterInfo[i].vertex_TR.uv2;
		ref Vector2 reference20 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4];
		reference20 = characterInfo[i].vertex_BR.uv2;
		if (isVolumetric)
		{
			ref Vector2 reference21 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[4 + index_X4];
			reference21 = characterInfo[i].vertex_BL.uv2;
			ref Vector2 reference22 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[5 + index_X4];
			reference22 = characterInfo[i].vertex_TL.uv2;
			ref Vector2 reference23 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[6 + index_X4];
			reference23 = characterInfo[i].vertex_TR.uv2;
			ref Vector2 reference24 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[7 + index_X4];
			reference24 = characterInfo[i].vertex_BR.uv2;
		}
		ref Color32 reference25 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4];
		reference25 = characterInfo[i].vertex_BL.color;
		ref Color32 reference26 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4];
		reference26 = characterInfo[i].vertex_TL.color;
		ref Color32 reference27 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4];
		reference27 = characterInfo[i].vertex_TR.color;
		ref Color32 reference28 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4];
		reference28 = characterInfo[i].vertex_BR.color;
		if (isVolumetric)
		{
			Color32 val2 = default(Color32);
			((Color32)(ref val2))._002Ector(byte.MaxValue, byte.MaxValue, (byte)128, byte.MaxValue);
			m_textInfo.meshInfo[materialReferenceIndex].colors32[4 + index_X4] = val2;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[5 + index_X4] = val2;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[6 + index_X4] = val2;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[7 + index_X4] = val2;
		}
		m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + (isVolumetric ? 8 : 4);
	}

	protected virtual void FillSpriteVertexBuffers(int i, int index_X4)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		int materialReferenceIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
		index_X4 = m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
		TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
		m_textInfo.characterInfo[i].vertexIndex = index_X4;
		ref Vector3 reference = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4];
		reference = characterInfo[i].vertex_BL.position;
		ref Vector3 reference2 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4];
		reference2 = characterInfo[i].vertex_TL.position;
		ref Vector3 reference3 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4];
		reference3 = characterInfo[i].vertex_TR.position;
		ref Vector3 reference4 = ref m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4];
		reference4 = characterInfo[i].vertex_BR.position;
		ref Vector2 reference5 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4];
		reference5 = characterInfo[i].vertex_BL.uv;
		ref Vector2 reference6 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4];
		reference6 = characterInfo[i].vertex_TL.uv;
		ref Vector2 reference7 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4];
		reference7 = characterInfo[i].vertex_TR.uv;
		ref Vector2 reference8 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4];
		reference8 = characterInfo[i].vertex_BR.uv;
		ref Vector2 reference9 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4];
		reference9 = characterInfo[i].vertex_BL.uv2;
		ref Vector2 reference10 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4];
		reference10 = characterInfo[i].vertex_TL.uv2;
		ref Vector2 reference11 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4];
		reference11 = characterInfo[i].vertex_TR.uv2;
		ref Vector2 reference12 = ref m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4];
		reference12 = characterInfo[i].vertex_BR.uv2;
		ref Color32 reference13 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4];
		reference13 = characterInfo[i].vertex_BL.color;
		ref Color32 reference14 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4];
		reference14 = characterInfo[i].vertex_TL.color;
		ref Color32 reference15 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4];
		reference15 = characterInfo[i].vertex_TR.color;
		ref Color32 reference16 = ref m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4];
		reference16 = characterInfo[i].vertex_BR.color;
		m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
	}

	protected virtual void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, float sdfScale, Color32 underlineColor)
	{
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_090d: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_091f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0943: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096b: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		if (m_cached_Underline_GlyphInfo == null)
		{
			if (!TMP_Settings.warningsDisabled)
			{
				Debug.LogWarning((object)"Unable to add underline since the Font Asset doesn't contain the underline character.", (Object)(object)this);
			}
			return;
		}
		int num = index + 12;
		if (num > m_textInfo.meshInfo[0].vertices.Length)
		{
			m_textInfo.meshInfo[0].ResizeMeshInfo(num / 4);
		}
		start.y = Mathf.Min(start.y, end.y);
		end.y = Mathf.Min(start.y, end.y);
		float num2 = m_cached_Underline_GlyphInfo.width / 2f * maxScale;
		if (end.x - start.x < m_cached_Underline_GlyphInfo.width * maxScale)
		{
			num2 = (end.x - start.x) / 2f;
		}
		float num3 = m_padding * startScale / maxScale;
		float num4 = m_padding * endScale / maxScale;
		float height = m_cached_Underline_GlyphInfo.height;
		Vector3[] vertices = m_textInfo.meshInfo[0].vertices;
		ref Vector3 reference = ref vertices[index];
		reference = start + new Vector3(0f, 0f - (height + m_padding) * maxScale, 0f);
		ref Vector3 reference2 = ref vertices[index + 1];
		reference2 = start + new Vector3(0f, m_padding * maxScale, 0f);
		ref Vector3 reference3 = ref vertices[index + 2];
		reference3 = vertices[index + 1] + new Vector3(num2, 0f, 0f);
		ref Vector3 reference4 = ref vertices[index + 3];
		reference4 = vertices[index] + new Vector3(num2, 0f, 0f);
		ref Vector3 reference5 = ref vertices[index + 4];
		reference5 = vertices[index + 3];
		ref Vector3 reference6 = ref vertices[index + 5];
		reference6 = vertices[index + 2];
		ref Vector3 reference7 = ref vertices[index + 6];
		reference7 = end + new Vector3(0f - num2, m_padding * maxScale, 0f);
		ref Vector3 reference8 = ref vertices[index + 7];
		reference8 = end + new Vector3(0f - num2, (0f - (height + m_padding)) * maxScale, 0f);
		ref Vector3 reference9 = ref vertices[index + 8];
		reference9 = vertices[index + 7];
		ref Vector3 reference10 = ref vertices[index + 9];
		reference10 = vertices[index + 6];
		ref Vector3 reference11 = ref vertices[index + 10];
		reference11 = end + new Vector3(0f, m_padding * maxScale, 0f);
		ref Vector3 reference12 = ref vertices[index + 11];
		reference12 = end + new Vector3(0f, (0f - (height + m_padding)) * maxScale, 0f);
		Vector2[] uvs = m_textInfo.meshInfo[0].uvs0;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((m_cached_Underline_GlyphInfo.x - num3) / m_fontAsset.fontInfo.AtlasWidth, 1f - (m_cached_Underline_GlyphInfo.y + m_padding + m_cached_Underline_GlyphInfo.height) / m_fontAsset.fontInfo.AtlasHeight);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(val.x, 1f - (m_cached_Underline_GlyphInfo.y - m_padding) / m_fontAsset.fontInfo.AtlasHeight);
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector((m_cached_Underline_GlyphInfo.x - num3 + m_cached_Underline_GlyphInfo.width / 2f) / m_fontAsset.fontInfo.AtlasWidth, val2.y);
		Vector2 val4 = default(Vector2);
		((Vector2)(ref val4))._002Ector(val3.x, val.y);
		Vector2 val5 = default(Vector2);
		((Vector2)(ref val5))._002Ector((m_cached_Underline_GlyphInfo.x + num4 + m_cached_Underline_GlyphInfo.width / 2f) / m_fontAsset.fontInfo.AtlasWidth, val2.y);
		Vector2 val6 = default(Vector2);
		((Vector2)(ref val6))._002Ector(val5.x, val.y);
		Vector2 val7 = default(Vector2);
		((Vector2)(ref val7))._002Ector((m_cached_Underline_GlyphInfo.x + num4 + m_cached_Underline_GlyphInfo.width) / m_fontAsset.fontInfo.AtlasWidth, val2.y);
		Vector2 val8 = default(Vector2);
		((Vector2)(ref val8))._002Ector(val7.x, val.y);
		uvs[index] = val;
		uvs[1 + index] = val2;
		uvs[2 + index] = val3;
		uvs[3 + index] = val4;
		ref Vector2 reference13 = ref uvs[4 + index];
		reference13 = new Vector2(val3.x - val3.x * 0.001f, val.y);
		ref Vector2 reference14 = ref uvs[5 + index];
		reference14 = new Vector2(val3.x - val3.x * 0.001f, val2.y);
		ref Vector2 reference15 = ref uvs[6 + index];
		reference15 = new Vector2(val3.x + val3.x * 0.001f, val2.y);
		ref Vector2 reference16 = ref uvs[7 + index];
		reference16 = new Vector2(val3.x + val3.x * 0.001f, val.y);
		uvs[8 + index] = val6;
		uvs[9 + index] = val5;
		uvs[10 + index] = val7;
		uvs[11 + index] = val8;
		float num5 = 0f;
		float x = (vertices[index + 2].x - start.x) / (end.x - start.x);
		float scale = Mathf.Abs(sdfScale);
		Vector2[] uvs2 = m_textInfo.meshInfo[0].uvs2;
		ref Vector2 reference17 = ref uvs2[index];
		reference17 = PackUV(0f, 0f, scale);
		ref Vector2 reference18 = ref uvs2[1 + index];
		reference18 = PackUV(0f, 1f, scale);
		ref Vector2 reference19 = ref uvs2[2 + index];
		reference19 = PackUV(x, 1f, scale);
		ref Vector2 reference20 = ref uvs2[3 + index];
		reference20 = PackUV(x, 0f, scale);
		num5 = (vertices[index + 4].x - start.x) / (end.x - start.x);
		x = (vertices[index + 6].x - start.x) / (end.x - start.x);
		ref Vector2 reference21 = ref uvs2[4 + index];
		reference21 = PackUV(num5, 0f, scale);
		ref Vector2 reference22 = ref uvs2[5 + index];
		reference22 = PackUV(num5, 1f, scale);
		ref Vector2 reference23 = ref uvs2[6 + index];
		reference23 = PackUV(x, 1f, scale);
		ref Vector2 reference24 = ref uvs2[7 + index];
		reference24 = PackUV(x, 0f, scale);
		num5 = (vertices[index + 8].x - start.x) / (end.x - start.x);
		x = (vertices[index + 6].x - start.x) / (end.x - start.x);
		ref Vector2 reference25 = ref uvs2[8 + index];
		reference25 = PackUV(num5, 0f, scale);
		ref Vector2 reference26 = ref uvs2[9 + index];
		reference26 = PackUV(num5, 1f, scale);
		ref Vector2 reference27 = ref uvs2[10 + index];
		reference27 = PackUV(1f, 1f, scale);
		ref Vector2 reference28 = ref uvs2[11 + index];
		reference28 = PackUV(1f, 0f, scale);
		Color32[] colors = m_textInfo.meshInfo[0].colors32;
		colors[index] = underlineColor;
		colors[1 + index] = underlineColor;
		colors[2 + index] = underlineColor;
		colors[3 + index] = underlineColor;
		colors[4 + index] = underlineColor;
		colors[5 + index] = underlineColor;
		colors[6 + index] = underlineColor;
		colors[7 + index] = underlineColor;
		colors[8 + index] = underlineColor;
		colors[9 + index] = underlineColor;
		colors[10 + index] = underlineColor;
		colors[11 + index] = underlineColor;
		index += 12;
	}

	protected void GetSpecialCharacters(TMP_FontAsset fontAsset)
	{
		if (!fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo))
		{
		}
		if (fontAsset.characterDictionary.TryGetValue(8230, out m_cached_Ellipsis_GlyphInfo))
		{
		}
	}

	protected TMP_FontAsset GetFontAssetForWeight(int fontWeight)
	{
		bool flag = (m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic;
		TMP_FontAsset tMP_FontAsset = null;
		int num = fontWeight / 100;
		if (flag)
		{
			return m_currentFontAsset.fontWeights[num].italicTypeface;
		}
		return m_currentFontAsset.fontWeights[num].regularTypeface;
	}

	protected virtual void SetActiveSubMeshes(bool state)
	{
	}

	protected Vector2 PackUV(float x, float y, float scale)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 result = default(Vector2);
		result.x = Mathf.Floor(x * 511f);
		result.y = Mathf.Floor(y * 511f);
		result.x = result.x * 4096f + result.y;
		result.y = scale;
		return result;
	}

	protected float PackUV(float x, float y)
	{
		double num = Math.Floor(x * 511f);
		double num2 = Math.Floor(y * 511f);
		return (float)(num * 4096.0 + num2);
	}

	protected int HexToInt(char hex)
	{
		return hex switch
		{
			'0' => 0, 
			'1' => 1, 
			'2' => 2, 
			'3' => 3, 
			'4' => 4, 
			'5' => 5, 
			'6' => 6, 
			'7' => 7, 
			'8' => 8, 
			'9' => 9, 
			'A' => 10, 
			'B' => 11, 
			'C' => 12, 
			'D' => 13, 
			'E' => 14, 
			'F' => 15, 
			'a' => 10, 
			'b' => 11, 
			'c' => 12, 
			'd' => 13, 
			'e' => 14, 
			'f' => 15, 
			_ => 15, 
		};
	}

	protected int GetUTF16(int i)
	{
		int num = HexToInt(m_text[i]) * 4096;
		num += HexToInt(m_text[i + 1]) * 256;
		num += HexToInt(m_text[i + 2]) * 16;
		return num + HexToInt(m_text[i + 3]);
	}

	protected int GetUTF32(int i)
	{
		int num = 0;
		num += HexToInt(m_text[i]) * 268435456;
		num += HexToInt(m_text[i + 1]) * 16777216;
		num += HexToInt(m_text[i + 2]) * 1048576;
		num += HexToInt(m_text[i + 3]) * 65536;
		num += HexToInt(m_text[i + 4]) * 4096;
		num += HexToInt(m_text[i + 5]) * 256;
		num += HexToInt(m_text[i + 6]) * 16;
		return num + HexToInt(m_text[i + 7]);
	}

	protected Color32 HexCharsToColor(char[] hexChars, int tagCount)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		switch (tagCount)
		{
		case 7:
		{
			byte b12 = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
			byte b13 = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
			byte b14 = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
			return new Color32(b12, b13, b14, byte.MaxValue);
		}
		case 9:
		{
			byte b8 = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
			byte b9 = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
			byte b10 = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
			byte b11 = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
			return new Color32(b8, b9, b10, b11);
		}
		case 13:
		{
			byte b5 = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
			byte b6 = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
			byte b7 = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
			return new Color32(b5, b6, b7, byte.MaxValue);
		}
		case 15:
		{
			byte b = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
			byte b2 = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
			byte b3 = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
			byte b4 = (byte)(HexToInt(hexChars[13]) * 16 + HexToInt(hexChars[14]));
			return new Color32(b, b2, b3, b4);
		}
		default:
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}

	protected Color32 HexCharsToColor(char[] hexChars, int startIndex, int length)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		switch (length)
		{
		case 7:
		{
			byte b5 = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 2]));
			byte b6 = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 4]));
			byte b7 = (byte)(HexToInt(hexChars[startIndex + 5]) * 16 + HexToInt(hexChars[startIndex + 6]));
			return new Color32(b5, b6, b7, byte.MaxValue);
		}
		case 9:
		{
			byte b = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 2]));
			byte b2 = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 4]));
			byte b3 = (byte)(HexToInt(hexChars[startIndex + 5]) * 16 + HexToInt(hexChars[startIndex + 6]));
			byte b4 = (byte)(HexToInt(hexChars[startIndex + 7]) * 16 + HexToInt(hexChars[startIndex + 8]));
			return new Color32(b, b2, b3, b4);
		}
		default:
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}

	protected float ConvertToFloat(char[] chars, int startIndex, int length, int decimalPointIndex)
	{
		if (startIndex == 0)
		{
			return -9999f;
		}
		int num = startIndex + length - 1;
		float num2 = 0f;
		float num3 = 1f;
		decimalPointIndex = ((decimalPointIndex <= 0) ? (num + 1) : decimalPointIndex);
		if (chars[startIndex] == '-')
		{
			startIndex++;
			num3 = -1f;
		}
		if (chars[startIndex] == '+' || chars[startIndex] == '%')
		{
			startIndex++;
		}
		for (int i = startIndex; i < num + 1; i++)
		{
			if (!char.IsDigit(chars[i]) && chars[i] != '.')
			{
				return -9999f;
			}
			switch (decimalPointIndex - i)
			{
			case 4:
				num2 += (float)((chars[i] - 48) * 1000);
				break;
			case 3:
				num2 += (float)((chars[i] - 48) * 100);
				break;
			case 2:
				num2 += (float)((chars[i] - 48) * 10);
				break;
			case 1:
				num2 += (float)(chars[i] - 48);
				break;
			case -1:
				num2 += (float)(chars[i] - 48) * 0.1f;
				break;
			case -2:
				num2 += (float)(chars[i] - 48) * 0.01f;
				break;
			case -3:
				num2 += (float)(chars[i] - 48) * 0.001f;
				break;
			}
		}
		return num2 * num3;
	}

	protected bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
	{
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2235: Unknown result type (might be due to invalid IL or missing references)
		//IL_223a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2246: Unknown result type (might be due to invalid IL or missing references)
		//IL_23f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_23fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2407: Unknown result type (might be due to invalid IL or missing references)
		//IL_2272: Unknown result type (might be due to invalid IL or missing references)
		//IL_2277: Unknown result type (might be due to invalid IL or missing references)
		//IL_2283: Unknown result type (might be due to invalid IL or missing references)
		//IL_2308: Unknown result type (might be due to invalid IL or missing references)
		//IL_230d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2312: Unknown result type (might be due to invalid IL or missing references)
		//IL_231e: Unknown result type (might be due to invalid IL or missing references)
		//IL_232b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2330: Unknown result type (might be due to invalid IL or missing references)
		//IL_2335: Unknown result type (might be due to invalid IL or missing references)
		//IL_2341: Unknown result type (might be due to invalid IL or missing references)
		//IL_23c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_23cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_23d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_234e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2353: Unknown result type (might be due to invalid IL or missing references)
		//IL_2358: Unknown result type (might be due to invalid IL or missing references)
		//IL_2364: Unknown result type (might be due to invalid IL or missing references)
		//IL_2371: Unknown result type (might be due to invalid IL or missing references)
		//IL_2376: Unknown result type (might be due to invalid IL or missing references)
		//IL_237b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2387: Unknown result type (might be due to invalid IL or missing references)
		//IL_2394: Unknown result type (might be due to invalid IL or missing references)
		//IL_2399: Unknown result type (might be due to invalid IL or missing references)
		//IL_239e: Unknown result type (might be due to invalid IL or missing references)
		//IL_23aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_2414: Unknown result type (might be due to invalid IL or missing references)
		//IL_2419: Unknown result type (might be due to invalid IL or missing references)
		//IL_241e: Unknown result type (might be due to invalid IL or missing references)
		//IL_242a: Unknown result type (might be due to invalid IL or missing references)
		//IL_25e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_25e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c71: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c76: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ce0: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ce5: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		byte b = 0;
		TagUnits tagUnits = TagUnits.Pixels;
		TagType tagType = TagType.None;
		int num2 = 0;
		m_xmlAttribute[num2].nameHashCode = 0;
		m_xmlAttribute[num2].valueType = TagType.None;
		m_xmlAttribute[num2].valueHashCode = 0;
		m_xmlAttribute[num2].valueStartIndex = 0;
		m_xmlAttribute[num2].valueLength = 0;
		m_xmlAttribute[num2].valueDecimalIndex = 0;
		endIndex = startIndex;
		bool flag = false;
		bool flag2 = false;
		for (int i = startIndex; i < chars.Length && chars[i] != 0; i++)
		{
			if (num >= m_htmlTag.Length)
			{
				break;
			}
			if (chars[i] == 60)
			{
				break;
			}
			if (chars[i] == 62)
			{
				flag2 = true;
				endIndex = i;
				m_htmlTag[num] = '\0';
				break;
			}
			m_htmlTag[num] = (char)chars[i];
			num++;
			if (b == 1)
			{
				switch (tagType)
				{
				case TagType.None:
					if (chars[i] == 43 || chars[i] == 45 || char.IsDigit((char)chars[i]))
					{
						tagType = TagType.NumericalValue;
						m_xmlAttribute[num2].valueType = TagType.NumericalValue;
						m_xmlAttribute[num2].valueStartIndex = num - 1;
						m_xmlAttribute[num2].valueLength++;
					}
					else if (chars[i] == 35)
					{
						tagType = TagType.ColorValue;
						m_xmlAttribute[num2].valueType = TagType.ColorValue;
						m_xmlAttribute[num2].valueStartIndex = num - 1;
						m_xmlAttribute[num2].valueLength++;
					}
					else if (chars[i] == 34)
					{
						tagType = TagType.StringValue;
						m_xmlAttribute[num2].valueType = TagType.StringValue;
						m_xmlAttribute[num2].valueStartIndex = num;
					}
					else
					{
						tagType = TagType.StringValue;
						m_xmlAttribute[num2].valueType = TagType.StringValue;
						m_xmlAttribute[num2].valueStartIndex = num - 1;
						m_xmlAttribute[num2].valueHashCode = ((m_xmlAttribute[num2].valueHashCode << 5) + m_xmlAttribute[num2].valueHashCode) ^ chars[i];
						m_xmlAttribute[num2].valueLength++;
					}
					break;
				case TagType.NumericalValue:
					if (chars[i] == 46)
					{
						m_xmlAttribute[num2].valueDecimalIndex = num - 1;
					}
					if (chars[i] == 112 || chars[i] == 101 || chars[i] == 37 || chars[i] == 32)
					{
						b = 2;
						tagType = TagType.None;
						num2++;
						m_xmlAttribute[num2].nameHashCode = 0;
						m_xmlAttribute[num2].valueType = TagType.None;
						m_xmlAttribute[num2].valueHashCode = 0;
						m_xmlAttribute[num2].valueStartIndex = 0;
						m_xmlAttribute[num2].valueLength = 0;
						m_xmlAttribute[num2].valueDecimalIndex = 0;
						if (chars[i] == 101)
						{
							tagUnits = TagUnits.FontUnits;
						}
						else if (chars[i] == 37)
						{
							tagUnits = TagUnits.Percentage;
						}
					}
					else if (b != 2)
					{
						m_xmlAttribute[num2].valueLength++;
					}
					break;
				case TagType.ColorValue:
					if (chars[i] != 32)
					{
						m_xmlAttribute[num2].valueLength++;
						break;
					}
					b = 2;
					tagType = TagType.None;
					num2++;
					m_xmlAttribute[num2].nameHashCode = 0;
					m_xmlAttribute[num2].valueType = TagType.None;
					m_xmlAttribute[num2].valueHashCode = 0;
					m_xmlAttribute[num2].valueStartIndex = 0;
					m_xmlAttribute[num2].valueLength = 0;
					m_xmlAttribute[num2].valueDecimalIndex = 0;
					break;
				case TagType.StringValue:
					if (chars[i] != 34)
					{
						m_xmlAttribute[num2].valueHashCode = ((m_xmlAttribute[num2].valueHashCode << 5) + m_xmlAttribute[num2].valueHashCode) ^ chars[i];
						m_xmlAttribute[num2].valueLength++;
						break;
					}
					b = 2;
					tagType = TagType.None;
					num2++;
					m_xmlAttribute[num2].nameHashCode = 0;
					m_xmlAttribute[num2].valueType = TagType.None;
					m_xmlAttribute[num2].valueHashCode = 0;
					m_xmlAttribute[num2].valueStartIndex = 0;
					m_xmlAttribute[num2].valueLength = 0;
					m_xmlAttribute[num2].valueDecimalIndex = 0;
					break;
				}
			}
			if (chars[i] == 61)
			{
				b = 1;
			}
			if (b == 0 && chars[i] == 32)
			{
				if (flag)
				{
					return false;
				}
				flag = true;
				b = 2;
				tagType = TagType.None;
				num2++;
				m_xmlAttribute[num2].nameHashCode = 0;
				m_xmlAttribute[num2].valueType = TagType.None;
				m_xmlAttribute[num2].valueHashCode = 0;
				m_xmlAttribute[num2].valueStartIndex = 0;
				m_xmlAttribute[num2].valueLength = 0;
				m_xmlAttribute[num2].valueDecimalIndex = 0;
			}
			if (b == 0)
			{
				m_xmlAttribute[num2].nameHashCode = (m_xmlAttribute[num2].nameHashCode << 3) - m_xmlAttribute[num2].nameHashCode + chars[i];
			}
			if (b == 2 && chars[i] == 32)
			{
				b = 0;
			}
		}
		if (!flag2)
		{
			return false;
		}
		if (tag_NoParsing && m_xmlAttribute[0].nameHashCode != 53822163 && m_xmlAttribute[0].nameHashCode != 49429939)
		{
			return false;
		}
		if (m_xmlAttribute[0].nameHashCode == 53822163 || m_xmlAttribute[0].nameHashCode == 49429939)
		{
			tag_NoParsing = false;
			return true;
		}
		if (m_htmlTag[0] == '#' && num == 7)
		{
			m_htmlColor = HexCharsToColor(m_htmlTag, num);
			m_colorStack.Add(m_htmlColor);
			return true;
		}
		if (m_htmlTag[0] == '#' && num == 9)
		{
			m_htmlColor = HexCharsToColor(m_htmlTag, num);
			m_colorStack.Add(m_htmlColor);
			return true;
		}
		float num3 = 0f;
		Material material;
		switch (m_xmlAttribute[0].nameHashCode)
		{
		case 66:
		case 98:
			m_style |= FontStyles.Bold;
			m_fontWeightInternal = 700;
			m_fontWeightStack.Add(700);
			return true;
		case 395:
		case 427:
			if ((m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
			{
				m_style &= (FontStyles)(-2);
				m_fontWeightInternal = m_fontWeightStack.Remove();
			}
			return true;
		case 73:
		case 105:
			m_style |= FontStyles.Italic;
			return true;
		case 402:
		case 434:
			m_style &= (FontStyles)(-3);
			return true;
		case 83:
		case 115:
			m_style |= FontStyles.Strikethrough;
			return true;
		case 412:
		case 444:
			if ((m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
			{
				m_style &= (FontStyles)(-65);
			}
			return true;
		case 85:
		case 117:
			m_style |= FontStyles.Underline;
			return true;
		case 414:
		case 446:
			if ((m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
			{
				m_style &= (FontStyles)(-5);
			}
			return true;
		case 4728:
		case 6552:
			m_fontScaleMultiplier = ((!(m_currentFontAsset.fontInfo.SubSize > 0f)) ? 1f : m_currentFontAsset.fontInfo.SubSize);
			m_baselineOffset = m_currentFontAsset.fontInfo.SubscriptOffset * m_fontScale * m_fontScaleMultiplier;
			m_style |= FontStyles.Subscript;
			return true;
		case 20849:
		case 22673:
			if ((m_style & FontStyles.Subscript) == FontStyles.Subscript)
			{
				if ((m_style & FontStyles.Superscript) == FontStyles.Superscript)
				{
					m_fontScaleMultiplier = ((!(m_currentFontAsset.fontInfo.SubSize > 0f)) ? 1f : m_currentFontAsset.fontInfo.SubSize);
					m_baselineOffset = m_currentFontAsset.fontInfo.SuperscriptOffset * m_fontScale * m_fontScaleMultiplier;
				}
				else
				{
					m_baselineOffset = 0f;
					m_fontScaleMultiplier = 1f;
				}
				m_style &= (FontStyles)(-257);
			}
			return true;
		case 4742:
		case 6566:
			m_fontScaleMultiplier = ((!(m_currentFontAsset.fontInfo.SubSize > 0f)) ? 1f : m_currentFontAsset.fontInfo.SubSize);
			m_baselineOffset = m_currentFontAsset.fontInfo.SuperscriptOffset * m_fontScale * m_fontScaleMultiplier;
			m_style |= FontStyles.Superscript;
			return true;
		case 20863:
		case 22687:
			if ((m_style & FontStyles.Superscript) == FontStyles.Superscript)
			{
				if ((m_style & FontStyles.Subscript) == FontStyles.Subscript)
				{
					m_fontScaleMultiplier = ((!(m_currentFontAsset.fontInfo.SubSize > 0f)) ? 1f : m_currentFontAsset.fontInfo.SubSize);
					m_baselineOffset = m_currentFontAsset.fontInfo.SubscriptOffset * m_fontScale * m_fontScaleMultiplier;
				}
				else
				{
					m_baselineOffset = 0f;
					m_fontScaleMultiplier = 1f;
				}
				m_style &= (FontStyles)(-129);
			}
			return true;
		case -330774850:
		case 2012149182:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			if ((m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
			{
				return true;
			}
			m_style &= (FontStyles)(-2);
			switch ((int)num3)
			{
			case 100:
				m_fontWeightInternal = 100;
				break;
			case 200:
				m_fontWeightInternal = 200;
				break;
			case 300:
				m_fontWeightInternal = 300;
				break;
			case 400:
				m_fontWeightInternal = 400;
				break;
			case 500:
				m_fontWeightInternal = 500;
				break;
			case 600:
				m_fontWeightInternal = 600;
				break;
			case 700:
				m_fontWeightInternal = 700;
				m_style |= FontStyles.Bold;
				break;
			case 800:
				m_fontWeightInternal = 800;
				break;
			case 900:
				m_fontWeightInternal = 900;
				break;
			}
			m_fontWeightStack.Add(m_fontWeightInternal);
			return true;
		case -1885698441:
		case 457225591:
			m_fontWeightInternal = m_fontWeightStack.Remove();
			if (m_fontWeightInternal == 400)
			{
				m_style &= (FontStyles)(-2);
			}
			return true;
		case 4556:
		case 6380:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				m_xAdvance = num3;
				return true;
			case TagUnits.FontUnits:
				m_xAdvance = num3 * m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				return true;
			case TagUnits.Percentage:
				m_xAdvance = m_marginWidth * num3 / 100f;
				return true;
			default:
				return false;
			}
		case 20677:
		case 22501:
			m_isIgnoringAlignment = false;
			return true;
		case 11642281:
		case 16034505:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				m_baselineOffset = num3;
				return true;
			case TagUnits.FontUnits:
				m_baselineOffset = num3 * m_fontScale * m_fontAsset.fontInfo.Ascender;
				return true;
			case TagUnits.Percentage:
				return false;
			default:
				return false;
			}
		case 50348802:
		case 54741026:
			m_baselineOffset = 0f;
			return true;
		case 31191:
		case 43991:
			if (m_overflowMode == TextOverflowModes.Page)
			{
				m_xAdvance = tag_LineIndent + tag_Indent;
				m_lineOffset = 0f;
				m_pageNumber++;
				m_isNewPage = true;
			}
			return true;
		case 31169:
		case 43969:
			m_isNonBreakingSpace = true;
			return true;
		case 144016:
		case 156816:
			m_isNonBreakingSpace = false;
			return true;
		case 32745:
		case 45545:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				if (m_htmlTag[5] == '+')
				{
					m_currentFontSize = m_fontSize + num3;
					m_sizeStack.Add(m_currentFontSize);
					m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
					return true;
				}
				if (m_htmlTag[5] == '-')
				{
					m_currentFontSize = m_fontSize + num3;
					m_sizeStack.Add(m_currentFontSize);
					m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
					return true;
				}
				m_currentFontSize = num3;
				m_sizeStack.Add(m_currentFontSize);
				m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				return true;
			case TagUnits.FontUnits:
				m_currentFontSize = m_fontSize * num3;
				m_sizeStack.Add(m_currentFontSize);
				m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				return true;
			case TagUnits.Percentage:
				m_currentFontSize = m_fontSize * num3 / 100f;
				m_sizeStack.Add(m_currentFontSize);
				m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				return true;
			default:
				return false;
			}
		case 145592:
		case 158392:
			m_currentFontSize = m_sizeStack.Remove();
			m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
			return true;
		case 28511:
		case 41311:
		{
			int valueHashCode4 = m_xmlAttribute[0].valueHashCode;
			int nameHashCode = m_xmlAttribute[1].nameHashCode;
			int valueHashCode2 = m_xmlAttribute[1].valueHashCode;
			if (valueHashCode4 == 764638571 || valueHashCode4 == 523367755)
			{
				m_currentFontAsset = m_materialReferences[0].fontAsset;
				m_currentMaterial = m_materialReferences[0].material;
				m_currentMaterialIndex = 0;
				m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				m_materialReferenceStack.Add(m_materialReferences[0]);
				return true;
			}
			if (!MaterialReferenceManager.TryGetFontAsset(valueHashCode4, out var fontAsset))
			{
				fontAsset = Resources.Load<TMP_FontAsset>(TMP_Settings.defaultFontAssetPath + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));
				if ((Object)(object)fontAsset == (Object)null)
				{
					return false;
				}
				MaterialReferenceManager.AddFontAsset(fontAsset);
			}
			if (nameHashCode == 0 && valueHashCode2 == 0)
			{
				m_currentMaterial = fontAsset.material;
				m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, fontAsset, m_materialReferences, m_materialReferenceIndexLookup);
				m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
			}
			else
			{
				if (nameHashCode != 103415287 && nameHashCode != 72669687)
				{
					return false;
				}
				if (MaterialReferenceManager.TryGetMaterial(valueHashCode2, out material))
				{
					m_currentMaterial = material;
					m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, fontAsset, m_materialReferences, m_materialReferenceIndexLookup);
					m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
				}
				else
				{
					material = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength));
					if ((Object)(object)material == (Object)null)
					{
						return false;
					}
					MaterialReferenceManager.AddFontMaterial(valueHashCode2, material);
					m_currentMaterial = material;
					m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, fontAsset, m_materialReferences, m_materialReferenceIndexLookup);
					m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
				}
			}
			m_currentFontAsset = fontAsset;
			m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
			return true;
		}
		case 141358:
		case 154158:
		{
			MaterialReference materialReference2 = m_materialReferenceStack.Remove();
			m_currentFontAsset = materialReference2.fontAsset;
			m_currentMaterial = materialReference2.material;
			m_currentMaterialIndex = materialReference2.index;
			m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
			return true;
		}
		case 72669687:
		case 103415287:
		{
			int valueHashCode2 = m_xmlAttribute[0].valueHashCode;
			if (valueHashCode2 == 764638571 || valueHashCode2 == 523367755)
			{
				if (((Object)m_currentFontAsset.atlas).GetInstanceID() != ((Object)m_currentMaterial.GetTexture(ShaderUtilities.ID_MainTex)).GetInstanceID())
				{
					return false;
				}
				m_currentMaterial = m_materialReferences[0].material;
				m_currentMaterialIndex = 0;
				m_materialReferenceStack.Add(m_materialReferences[0]);
				return true;
			}
			if (MaterialReferenceManager.TryGetMaterial(valueHashCode2, out material))
			{
				if (((Object)m_currentFontAsset.atlas).GetInstanceID() != ((Object)material.GetTexture(ShaderUtilities.ID_MainTex)).GetInstanceID())
				{
					return false;
				}
				m_currentMaterial = material;
				m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, m_currentFontAsset, m_materialReferences, m_materialReferenceIndexLookup);
				m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
			}
			else
			{
				material = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));
				if ((Object)(object)material == (Object)null)
				{
					return false;
				}
				if (((Object)m_currentFontAsset.atlas).GetInstanceID() != ((Object)material.GetTexture(ShaderUtilities.ID_MainTex)).GetInstanceID())
				{
					return false;
				}
				MaterialReferenceManager.AddFontMaterial(valueHashCode2, material);
				m_currentMaterial = material;
				m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, m_currentFontAsset, m_materialReferences, m_materialReferenceIndexLookup);
				m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
			}
			return true;
		}
		case 343615334:
		case 374360934:
		{
			if (((Object)m_currentMaterial.GetTexture(ShaderUtilities.ID_MainTex)).GetInstanceID() != ((Object)m_materialReferenceStack.PreviousItem().material.GetTexture(ShaderUtilities.ID_MainTex)).GetInstanceID())
			{
				return false;
			}
			MaterialReference materialReference = m_materialReferenceStack.Remove();
			m_currentMaterial = materialReference.material;
			m_currentMaterialIndex = materialReference.index;
			return true;
		}
		case 230446:
		case 320078:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				m_xAdvance += num3;
				return true;
			case TagUnits.FontUnits:
				m_xAdvance += num3 * m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				return true;
			case TagUnits.Percentage:
				return false;
			default:
				return false;
			}
		case 186622:
		case 276254:
			if (m_xmlAttribute[0].valueLength != 3)
			{
				return false;
			}
			m_htmlColor.a = (byte)(HexToInt(m_htmlTag[7]) * 16 + HexToInt(m_htmlTag[8]));
			return true;
		case 1750458:
			return false;
		case 426:
			return true;
		case 30266:
		case 43066:
			if (m_isParsingText)
			{
				int linkCount = m_textInfo.linkCount;
				if (linkCount + 1 > m_textInfo.linkInfo.Length)
				{
					TMP_TextInfo.Resize(ref m_textInfo.linkInfo, linkCount + 1);
				}
				m_textInfo.linkInfo[linkCount].textComponent = this;
				m_textInfo.linkInfo[linkCount].hashCode = m_xmlAttribute[0].valueHashCode;
				m_textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = m_characterCount;
				m_textInfo.linkInfo[linkCount].linkIdFirstCharacterIndex = startIndex + m_xmlAttribute[0].valueStartIndex;
				m_textInfo.linkInfo[linkCount].linkIdLength = m_xmlAttribute[0].valueLength;
				m_textInfo.linkInfo[linkCount].SetLinkID(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
			}
			return true;
		case 143113:
		case 155913:
			if (m_isParsingText)
			{
				m_textInfo.linkInfo[m_textInfo.linkCount].linkTextLength = m_characterCount - m_textInfo.linkInfo[m_textInfo.linkCount].linkTextfirstCharacterIndex;
				m_textInfo.linkCount++;
			}
			return true;
		case 186285:
		case 275917:
			switch (m_xmlAttribute[0].valueHashCode)
			{
			case 3774683:
				m_lineJustification = TextAlignmentOptions.Left;
				return true;
			case 136703040:
				m_lineJustification = TextAlignmentOptions.Right;
				return true;
			case -458210101:
				m_lineJustification = TextAlignmentOptions.Center;
				return true;
			case -523808257:
				m_lineJustification = TextAlignmentOptions.Justified;
				return true;
			default:
				return false;
			}
		case 976214:
		case 1065846:
			m_lineJustification = m_textAlignment;
			return true;
		case 237918:
		case 327550:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				m_width = num3;
				break;
			case TagUnits.FontUnits:
				return false;
			case TagUnits.Percentage:
				m_width = m_marginWidth * num3 / 100f;
				break;
			}
			return true;
		case 1027847:
		case 1117479:
			m_width = -1f;
			return true;
		case 233057:
		case 322689:
		{
			TMP_Style style = TMP_StyleSheet.GetStyle(m_xmlAttribute[0].valueHashCode);
			if (style == null)
			{
				return false;
			}
			m_styleStack.Add(style.hashCode);
			for (int k = 0; k < style.styleOpeningTagArray.Length; k++)
			{
				if (style.styleOpeningTagArray[k] == 60 && !ValidateHtmlTag(style.styleOpeningTagArray, k + 1, out k))
				{
					return false;
				}
			}
			return true;
		}
		case 1022986:
		case 1112618:
		{
			TMP_Style style = TMP_StyleSheet.GetStyle(m_xmlAttribute[0].valueHashCode);
			if (style == null)
			{
				int hashCode = m_styleStack.CurrentItem();
				style = TMP_StyleSheet.GetStyle(hashCode);
				m_styleStack.Remove();
			}
			if (style == null)
			{
				return false;
			}
			for (int j = 0; j < style.styleClosingTagArray.Length; j++)
			{
				if (style.styleClosingTagArray[j] == 60)
				{
					ValidateHtmlTag(style.styleClosingTagArray, j + 1, out j);
				}
			}
			return true;
		}
		case 192323:
		case 281955:
			if (m_htmlTag[6] == '#' && num == 13)
			{
				m_htmlColor = HexCharsToColor(m_htmlTag, num);
				m_colorStack.Add(m_htmlColor);
				return true;
			}
			if (m_htmlTag[6] == '#' && num == 15)
			{
				m_htmlColor = HexCharsToColor(m_htmlTag, num);
				m_colorStack.Add(m_htmlColor);
				return true;
			}
			switch (m_xmlAttribute[0].valueHashCode)
			{
			case 125395:
				m_htmlColor = Color32.op_Implicit(Color.red);
				m_colorStack.Add(m_htmlColor);
				return true;
			case 3573310:
				m_htmlColor = Color32.op_Implicit(Color.blue);
				m_colorStack.Add(m_htmlColor);
				return true;
			case 117905991:
				m_htmlColor = Color32.op_Implicit(Color.black);
				m_colorStack.Add(m_htmlColor);
				return true;
			case 121463835:
				m_htmlColor = Color32.op_Implicit(Color.green);
				m_colorStack.Add(m_htmlColor);
				return true;
			case 140357351:
				m_htmlColor = Color32.op_Implicit(Color.white);
				m_colorStack.Add(m_htmlColor);
				return true;
			case 26556144:
				m_htmlColor = new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue);
				m_colorStack.Add(m_htmlColor);
				return true;
			case -36881330:
				m_htmlColor = new Color32((byte)160, (byte)32, (byte)240, byte.MaxValue);
				m_colorStack.Add(m_htmlColor);
				return true;
			case 554054276:
				m_htmlColor = Color32.op_Implicit(Color.yellow);
				m_colorStack.Add(m_htmlColor);
				return true;
			default:
				return false;
			}
		case 1356515:
		case 1983971:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				m_cSpacing = num3;
				break;
			case TagUnits.FontUnits:
				m_cSpacing = num3;
				m_cSpacing *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				return false;
			}
			return true;
		case 6886018:
		case 7513474:
			m_cSpacing = 0f;
			return true;
		case 1524585:
		case 2152041:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				m_monoSpacing = num3;
				break;
			case TagUnits.FontUnits:
				m_monoSpacing = num3;
				m_monoSpacing *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				return false;
			}
			return true;
		case 7054088:
		case 7681544:
			m_monoSpacing = 0f;
			return true;
		case 280416:
			return false;
		case 982252:
		case 1071884:
			m_htmlColor = m_colorStack.Remove();
			return true;
		case 1441524:
		case 2068980:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				tag_Indent = num3;
				break;
			case TagUnits.FontUnits:
				tag_Indent = num3;
				tag_Indent *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				tag_Indent = m_marginWidth * num3 / 100f;
				break;
			}
			m_indentStack.Add(tag_Indent);
			m_xAdvance = tag_Indent;
			return true;
		case 6971027:
		case 7598483:
			tag_Indent = m_indentStack.Remove();
			return true;
		case -842656867:
		case 1109386397:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				tag_LineIndent = num3;
				break;
			case TagUnits.FontUnits:
				tag_LineIndent = num3;
				tag_LineIndent *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				tag_LineIndent = m_marginWidth * num3 / 100f;
				break;
			}
			m_xAdvance += tag_LineIndent;
			return true;
		case -445537194:
		case 1897386838:
			tag_LineIndent = 0f;
			return true;
		case 1619421:
		case 2246877:
		{
			int valueHashCode3 = m_xmlAttribute[0].valueHashCode;
			TMP_SpriteAsset tMP_SpriteAsset;
			if (m_xmlAttribute[0].valueType == TagType.None || m_xmlAttribute[0].valueType == TagType.NumericalValue)
			{
				if ((Object)(object)m_defaultSpriteAsset == (Object)null)
				{
					if ((Object)(object)TMP_Settings.defaultSpriteAsset != (Object)null)
					{
						m_defaultSpriteAsset = TMP_Settings.defaultSpriteAsset;
					}
					else
					{
						m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
					}
				}
				m_currentSpriteAsset = m_defaultSpriteAsset;
				if ((Object)(object)m_currentSpriteAsset == (Object)null)
				{
					return false;
				}
			}
			else if (MaterialReferenceManager.TryGetSpriteAsset(valueHashCode3, out tMP_SpriteAsset))
			{
				m_currentSpriteAsset = tMP_SpriteAsset;
			}
			else
			{
				if ((Object)(object)tMP_SpriteAsset == (Object)null)
				{
					tMP_SpriteAsset = Resources.Load<TMP_SpriteAsset>(TMP_Settings.defaultSpriteAssetPath + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));
				}
				if ((Object)(object)tMP_SpriteAsset == (Object)null)
				{
					return false;
				}
				MaterialReferenceManager.AddSpriteAsset(valueHashCode3, tMP_SpriteAsset);
				m_currentSpriteAsset = tMP_SpriteAsset;
			}
			if (m_xmlAttribute[0].valueType == TagType.NumericalValue)
			{
				int num4 = (int)ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num4 == -9999)
				{
					return false;
				}
				if (num4 > m_currentSpriteAsset.spriteInfoList.Count - 1)
				{
					return false;
				}
				m_spriteIndex = num4;
			}
			else if (m_xmlAttribute[1].nameHashCode == 43347 || m_xmlAttribute[1].nameHashCode == 30547)
			{
				int spriteIndex = m_currentSpriteAsset.GetSpriteIndex(m_xmlAttribute[1].valueHashCode);
				if (spriteIndex == -1)
				{
					return false;
				}
				m_spriteIndex = spriteIndex;
			}
			else
			{
				if (m_xmlAttribute[1].nameHashCode != 295562 && m_xmlAttribute[1].nameHashCode != 205930)
				{
					return false;
				}
				int num5 = (int)ConvertToFloat(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength, m_xmlAttribute[1].valueDecimalIndex);
				if (num5 == -9999)
				{
					return false;
				}
				if (num5 > m_currentSpriteAsset.spriteInfoList.Count - 1)
				{
					return false;
				}
				m_spriteIndex = num5;
			}
			m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentSpriteAsset.material, m_currentSpriteAsset, m_materialReferences, m_materialReferenceIndexLookup);
			m_spriteColor = s_colorWhite;
			m_tintSprite = false;
			if (m_xmlAttribute[1].nameHashCode == 45819 || m_xmlAttribute[1].nameHashCode == 33019)
			{
				m_tintSprite = ConvertToFloat(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength, m_xmlAttribute[1].valueDecimalIndex) != 0f;
			}
			else if (m_xmlAttribute[2].nameHashCode == 45819 || m_xmlAttribute[2].nameHashCode == 33019)
			{
				m_tintSprite = ConvertToFloat(m_htmlTag, m_xmlAttribute[2].valueStartIndex, m_xmlAttribute[2].valueLength, m_xmlAttribute[2].valueDecimalIndex) != 0f;
			}
			if (m_xmlAttribute[1].nameHashCode == 281955 || m_xmlAttribute[1].nameHashCode == 192323)
			{
				m_spriteColor = HexCharsToColor(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength);
			}
			else if (m_xmlAttribute[2].nameHashCode == 281955 || m_xmlAttribute[2].nameHashCode == 192323)
			{
				m_spriteColor = HexCharsToColor(m_htmlTag, m_xmlAttribute[2].valueStartIndex, m_xmlAttribute[2].valueLength);
			}
			m_xmlAttribute[1].nameHashCode = 0;
			m_xmlAttribute[2].nameHashCode = 0;
			m_textElementType = TMP_TextElementType.Sprite;
			return true;
		}
		case 514803617:
		case 730022849:
			m_style |= FontStyles.LowerCase;
			return true;
		case -1883544150:
		case -1668324918:
			m_style &= (FontStyles)(-9);
			return true;
		case 9133802:
		case 13526026:
		case 566686826:
		case 781906058:
			m_style |= FontStyles.UpperCase;
			return true;
		case -1831660941:
		case -1616441709:
		case 47840323:
		case 52232547:
			m_style &= (FontStyles)(-17);
			return true;
		case 551025096:
		case 766244328:
			m_style |= FontStyles.SmallCaps;
			return true;
		case -1847322671:
		case -1632103439:
			m_style &= (FontStyles)(-33);
			return true;
		case 1482398:
		case 2109854:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			m_marginLeft = num3;
			switch (tagUnits)
			{
			case TagUnits.FontUnits:
				m_marginLeft *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				m_marginLeft = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginLeft / 100f;
				break;
			}
			m_marginLeft = ((!(m_marginLeft >= 0f)) ? 0f : m_marginLeft);
			m_marginRight = m_marginLeft;
			return true;
		case 7011901:
		case 7639357:
			m_marginLeft = 0f;
			m_marginRight = 0f;
			return true;
		case -855002522:
		case 1100728678:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			m_marginLeft = num3;
			switch (tagUnits)
			{
			case TagUnits.FontUnits:
				m_marginLeft *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				m_marginLeft = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginLeft / 100f;
				break;
			}
			m_marginLeft = ((!(m_marginLeft >= 0f)) ? 0f : m_marginLeft);
			return true;
		case -1690034531:
		case -884817987:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			m_marginRight = num3;
			switch (tagUnits)
			{
			case TagUnits.FontUnits:
				m_marginRight *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				m_marginRight = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginRight / 100f;
				break;
			}
			m_marginRight = ((!(m_marginRight >= 0f)) ? 0f : m_marginRight);
			return true;
		case -842693512:
		case 1109349752:
			num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
			if (num3 == -9999f || num3 == 0f)
			{
				return false;
			}
			m_lineHeight = num3;
			switch (tagUnits)
			{
			case TagUnits.FontUnits:
				m_lineHeight *= m_fontAsset.fontInfo.LineHeight * m_fontScale;
				break;
			case TagUnits.Percentage:
				m_lineHeight = m_fontAsset.fontInfo.LineHeight * m_lineHeight / 100f * m_fontScale;
				break;
			}
			return true;
		case -445573839:
		case 1897350193:
			m_lineHeight = 0f;
			return true;
		case 10723418:
		case 15115642:
			tag_NoParsing = true;
			return true;
		case 1286342:
		case 1913798:
		{
			int valueHashCode = m_xmlAttribute[0].valueHashCode;
			if (m_isParsingText)
			{
				m_actionStack.Add(valueHashCode);
				Debug.Log((object)("Action ID: [" + valueHashCode + "] First character index: " + m_characterCount));
			}
			return true;
		}
		case 6815845:
		case 7443301:
			if (m_isParsingText)
			{
				Debug.Log((object)("Action ID: [" + m_actionStack.CurrentItem() + "] Last character index: " + (m_characterCount - 1)));
			}
			m_actionStack.Remove();
			return true;
		default:
			return false;
		}
	}
}
