using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
[AddComponentMenu("UI/TextMeshPro - Text (UI)", 11)]
[SelectionBase]
public class TextMeshProUGUI : TMP_Text, ILayoutElement
{
	private bool m_isRebuildingLayout;

	[SerializeField]
	private Vector2 m_uvOffset = Vector2.zero;

	[SerializeField]
	private float m_uvLineOffset;

	[SerializeField]
	private bool m_hasFontAssetChanged;

	[SerializeField]
	protected TMP_SubMeshUI[] m_subTextObjects = new TMP_SubMeshUI[16];

	private float m_previousLossyScaleY = -1f;

	private Vector3[] m_RectTransformCorners = (Vector3[])(object)new Vector3[4];

	private CanvasRenderer m_canvasRenderer;

	private Canvas m_canvas;

	private bool m_isFirstAllocation;

	private int m_max_characters = 8;

	private WordWrapState m_SavedWordWrapState = default(WordWrapState);

	private WordWrapState m_SavedLineState = default(WordWrapState);

	private bool m_isMaskingEnabled;

	[SerializeField]
	private Material m_baseMaterial;

	private bool m_isScrollRegionSet;

	private int m_stencilID;

	[SerializeField]
	private Vector4 m_maskOffset;

	private Matrix4x4 m_EnvMapMatrix = default(Matrix4x4);

	[NonSerialized]
	private bool m_isRegisteredForEvents;

	private int m_recursiveCount;

	private int m_recursiveCountA;

	private int loopCountA;

	public override Material materialForRendering
	{
		get
		{
			if ((Object)(object)m_sharedMaterial == (Object)null)
			{
				return null;
			}
			return ((MaskableGraphic)this).GetModifiedMaterial(m_sharedMaterial);
		}
	}

	public override Mesh mesh => m_mesh;

	public CanvasRenderer canvasRenderer
	{
		get
		{
			if ((Object)(object)m_canvasRenderer == (Object)null)
			{
				m_canvasRenderer = ((Component)this).GetComponent<CanvasRenderer>();
			}
			return m_canvasRenderer;
		}
	}

	public InlineGraphicManager inlineGraphicManager => m_inlineGraphics;

	public Vector4 maskOffset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_maskOffset;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_maskOffset = value;
			UpdateMask();
			m_havePropertiesChanged = true;
		}
	}

	public void CalculateLayoutInputHorizontal()
	{
		if (((Component)this).gameObject.activeInHierarchy && (m_isCalculateSizeRequired || ((Transform)m_rectTransform).hasChanged))
		{
			m_preferredWidth = GetPreferredWidth();
			ComputeMarginSize();
			m_isLayoutDirty = true;
		}
	}

	public void CalculateLayoutInputVertical()
	{
		if (((Component)this).gameObject.activeInHierarchy)
		{
			if (m_isCalculateSizeRequired || ((Transform)m_rectTransform).hasChanged)
			{
				m_preferredHeight = GetPreferredHeight();
				ComputeMarginSize();
				m_isLayoutDirty = true;
			}
			m_isCalculateSizeRequired = false;
		}
	}

	public override void SetVerticesDirty()
	{
		if (!m_verticesAlreadyDirty && !((Object)(object)this == (Object)null) && ((UIBehaviour)this).IsActive() && !CanvasUpdateRegistry.IsRebuildingGraphics())
		{
			m_verticesAlreadyDirty = true;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild((ICanvasElement)(object)this);
		}
	}

	public override void SetLayoutDirty()
	{
		if (!m_layoutAlreadyDirty && !((Object)(object)this == (Object)null) && ((UIBehaviour)this).IsActive())
		{
			m_layoutAlreadyDirty = true;
			LayoutRebuilder.MarkLayoutForRebuild(base.rectTransform);
			m_isLayoutDirty = true;
		}
	}

	public override void SetMaterialDirty()
	{
		if (!((Object)(object)this == (Object)null) && ((UIBehaviour)this).IsActive() && !CanvasUpdateRegistry.IsRebuildingGraphics())
		{
			m_isMaterialDirty = true;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild((ICanvasElement)(object)this);
		}
	}

	public override void SetAllDirty()
	{
		((Graphic)this).SetLayoutDirty();
		((Graphic)this).SetVerticesDirty();
		((Graphic)this).SetMaterialDirty();
	}

	public override void Rebuild(CanvasUpdate update)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		if (!((Object)(object)this == (Object)null) && (int)update == 3)
		{
			OnPreRenderCanvas();
			m_verticesAlreadyDirty = false;
			m_layoutAlreadyDirty = false;
			if (m_isMaterialDirty)
			{
				((Graphic)this).UpdateMaterial();
				m_isMaterialDirty = false;
			}
		}
	}

	private void UpdateSubObjectPivot()
	{
		if (m_textInfo != null)
		{
			for (int i = 1; i < m_subTextObjects.Length && (Object)(object)m_subTextObjects[i] != (Object)null; i++)
			{
				m_subTextObjects[i].SetPivotDirty();
			}
		}
	}

	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		Material val = baseMaterial;
		if (((MaskableGraphic)this).m_ShouldRecalculateStencil)
		{
			m_stencilID = TMP_MaterialManager.GetStencilID(((Component)this).gameObject);
			((MaskableGraphic)this).m_ShouldRecalculateStencil = false;
		}
		if (m_stencilID > 0)
		{
			val = TMP_MaterialManager.GetStencilMaterial(baseMaterial, m_stencilID);
			if ((Object)(object)((MaskableGraphic)this).m_MaskMaterial != (Object)null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(((MaskableGraphic)this).m_MaskMaterial);
			}
			((MaskableGraphic)this).m_MaskMaterial = val;
		}
		return val;
	}

	protected override void UpdateMaterial()
	{
		if ((Object)(object)m_canvasRenderer == (Object)null)
		{
			m_canvasRenderer = canvasRenderer;
		}
		m_canvasRenderer.materialCount = 1;
		m_canvasRenderer.SetMaterial(((Graphic)this).materialForRendering, m_sharedMaterial.mainTexture);
	}

	public override void RecalculateClipping()
	{
		((MaskableGraphic)this).RecalculateClipping();
	}

	public override void RecalculateMasking()
	{
		((MaskableGraphic)this).m_ShouldRecalculateStencil = true;
		((Graphic)this).SetMaterialDirty();
	}

	public override void UpdateMeshPadding()
	{
		m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
		m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
		m_havePropertiesChanged = true;
		checkPaddingRequired = false;
		for (int i = 1; i < m_textInfo.materialCount; i++)
		{
			m_subTextObjects[i].UpdateMeshPadding(m_enableExtraPadding, m_isUsingBold);
		}
	}

	protected override void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		int materialCount = m_textInfo.materialCount;
		for (int i = 1; i < materialCount; i++)
		{
			((Graphic)m_subTextObjects[i]).CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
		}
	}

	protected override void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
	{
		int materialCount = m_textInfo.materialCount;
		for (int i = 1; i < materialCount; i++)
		{
			((Graphic)m_subTextObjects[i]).CrossFadeAlpha(alpha, duration, ignoreTimeScale);
		}
	}

	public override void ForceMeshUpdate()
	{
		m_havePropertiesChanged = true;
		OnPreRenderCanvas();
	}

	public override void ForceMeshUpdate(bool ignoreInactive)
	{
		m_havePropertiesChanged = true;
		m_ignoreActiveState = true;
		OnPreRenderCanvas();
	}

	public override TMP_TextInfo GetTextInfo(string text)
	{
		StringToCharArray(text, ref m_char_buffer);
		SetArraySizes(m_char_buffer);
		m_renderMode = TextRenderFlags.DontRender;
		ComputeMarginSize();
		if ((Object)(object)m_canvas == (Object)null)
		{
			m_canvas = ((Graphic)this).canvas;
		}
		GenerateTextMesh();
		m_renderMode = TextRenderFlags.Render;
		return base.textInfo;
	}

	public override void UpdateGeometry(Mesh mesh, int index)
	{
		mesh.RecalculateBounds();
		if (index == 0)
		{
			m_canvasRenderer.SetMesh(mesh);
		}
		else
		{
			m_subTextObjects[index].canvasRenderer.SetMesh(mesh);
		}
	}

	public override void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
	{
		int materialCount = m_textInfo.materialCount;
		for (int i = 0; i < materialCount; i++)
		{
			Mesh val = ((i != 0) ? m_subTextObjects[i].mesh : m_mesh);
			if ((flags & TMP_VertexDataUpdateFlags.Vertices) == TMP_VertexDataUpdateFlags.Vertices)
			{
				val.vertices = m_textInfo.meshInfo[i].vertices;
			}
			if ((flags & TMP_VertexDataUpdateFlags.Uv0) == TMP_VertexDataUpdateFlags.Uv0)
			{
				val.uv = m_textInfo.meshInfo[i].uvs0;
			}
			if ((flags & TMP_VertexDataUpdateFlags.Uv2) == TMP_VertexDataUpdateFlags.Uv2)
			{
				val.uv2 = m_textInfo.meshInfo[i].uvs2;
			}
			if ((flags & TMP_VertexDataUpdateFlags.Colors32) == TMP_VertexDataUpdateFlags.Colors32)
			{
				val.colors32 = m_textInfo.meshInfo[i].colors32;
			}
			val.RecalculateBounds();
			if (i == 0)
			{
				m_canvasRenderer.SetMesh(val);
			}
			else
			{
				m_subTextObjects[i].canvasRenderer.SetMesh(val);
			}
		}
	}

	public override void UpdateVertexData()
	{
		int materialCount = m_textInfo.materialCount;
		for (int i = 0; i < materialCount; i++)
		{
			Mesh val = ((i != 0) ? m_subTextObjects[i].mesh : m_mesh);
			val.vertices = m_textInfo.meshInfo[i].vertices;
			val.uv = m_textInfo.meshInfo[i].uvs0;
			val.uv2 = m_textInfo.meshInfo[i].uvs2;
			val.colors32 = m_textInfo.meshInfo[i].colors32;
			val.RecalculateBounds();
			if (i == 0)
			{
				m_canvasRenderer.SetMesh(val);
			}
			else
			{
				m_subTextObjects[i].canvasRenderer.SetMesh(val);
			}
		}
	}

	public void UpdateFontAsset()
	{
		LoadFontAsset();
	}

	protected override void Awake()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		m_canvas = ((Graphic)this).canvas;
		m_isOrthographic = true;
		m_rectTransform = ((Component)this).gameObject.GetComponent<RectTransform>();
		if ((Object)(object)m_rectTransform == (Object)null)
		{
			m_rectTransform = ((Component)this).gameObject.AddComponent<RectTransform>();
		}
		m_canvasRenderer = ((Component)this).GetComponent<CanvasRenderer>();
		if ((Object)(object)m_canvasRenderer == (Object)null)
		{
			m_canvasRenderer = ((Component)this).gameObject.AddComponent<CanvasRenderer>();
		}
		if ((Object)(object)m_mesh == (Object)null)
		{
			m_mesh = new Mesh();
			((Object)m_mesh).hideFlags = (HideFlags)61;
		}
		if (m_text == null)
		{
			m_enableWordWrapping = TMP_Settings.enableWordWrapping;
			m_enableKerning = TMP_Settings.enableKerning;
			m_enableExtraPadding = TMP_Settings.enableExtraPadding;
			m_tintAllSprites = TMP_Settings.enableTintAllSprites;
			m_parseCtrlCharacters = TMP_Settings.enableParseEscapeCharacters;
		}
		LoadFontAsset();
		TMP_StyleSheet.LoadDefaultStyleSheet();
		m_char_buffer = new int[m_max_characters];
		m_cached_TextElement = new TMP_Glyph();
		m_isFirstAllocation = true;
		if (m_textInfo == null)
		{
			m_textInfo = new TMP_TextInfo(this);
		}
		if ((Object)(object)m_fontAsset == (Object)null)
		{
			Debug.LogWarning((object)("Please assign a Font Asset to this " + ((Object)base.transform).name + " gameobject."), (Object)(object)this);
			return;
		}
		if (m_fontSizeMin == 0f)
		{
			m_fontSizeMin = m_fontSize / 2f;
		}
		if (m_fontSizeMax == 0f)
		{
			m_fontSizeMax = m_fontSize * 2f;
		}
		m_isInputParsingRequired = true;
		m_havePropertiesChanged = true;
		m_isCalculateSizeRequired = true;
		m_isAwake = true;
	}

	protected override void OnEnable()
	{
		if (!m_isRegisteredForEvents)
		{
			m_isRegisteredForEvents = true;
		}
		m_canvas = GetCanvas();
		SetActiveSubMeshes(state: true);
		GraphicRegistry.RegisterGraphicForCanvas(m_canvas, (Graphic)(object)this);
		ComputeMarginSize();
		m_verticesAlreadyDirty = false;
		m_layoutAlreadyDirty = false;
		((MaskableGraphic)this).m_ShouldRecalculateStencil = true;
		m_isInputParsingRequired = true;
		((Graphic)this).SetAllDirty();
		((MaskableGraphic)this).RecalculateClipping();
	}

	protected override void OnDisable()
	{
		if ((Object)(object)((MaskableGraphic)this).m_MaskMaterial != (Object)null)
		{
			TMP_MaterialManager.ReleaseStencilMaterial(((MaskableGraphic)this).m_MaskMaterial);
			((MaskableGraphic)this).m_MaskMaterial = null;
		}
		GraphicRegistry.UnregisterGraphicForCanvas(m_canvas, (Graphic)(object)this);
		CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild((ICanvasElement)(object)this);
		if ((Object)(object)m_canvasRenderer != (Object)null)
		{
			m_canvasRenderer.Clear();
		}
		SetActiveSubMeshes(state: false);
		LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
		((MaskableGraphic)this).RecalculateClipping();
	}

	protected override void OnDestroy()
	{
		GraphicRegistry.UnregisterGraphicForCanvas(m_canvas, (Graphic)(object)this);
		if ((Object)(object)m_mesh != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)m_mesh);
		}
		if ((Object)(object)((MaskableGraphic)this).m_MaskMaterial != (Object)null)
		{
			TMP_MaterialManager.ReleaseStencilMaterial(((MaskableGraphic)this).m_MaskMaterial);
			((MaskableGraphic)this).m_MaskMaterial = null;
		}
		m_isRegisteredForEvents = false;
	}

	protected override void LoadFontAsset()
	{
		ShaderUtilities.GetShaderPropertyIDs();
		if ((Object)(object)m_fontAsset == (Object)null)
		{
			if ((Object)(object)TMP_Settings.defaultFontAsset != (Object)null)
			{
				m_fontAsset = TMP_Settings.defaultFontAsset;
			}
			else
			{
				m_fontAsset = Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
			}
			if ((Object)(object)m_fontAsset == (Object)null)
			{
				Debug.LogWarning((object)("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + ((Object)((Component)this).gameObject).name + "."), (Object)(object)this);
				return;
			}
			if (m_fontAsset.characterDictionary == null)
			{
				Debug.Log((object)"Dictionary is Null!");
			}
			m_sharedMaterial = m_fontAsset.material;
		}
		else
		{
			if (m_fontAsset.characterDictionary == null)
			{
				m_fontAsset.ReadFontDefinition();
			}
			if ((Object)(object)m_sharedMaterial == (Object)null && (Object)(object)m_baseMaterial != (Object)null)
			{
				m_sharedMaterial = m_baseMaterial;
				m_baseMaterial = null;
			}
			if ((Object)(object)m_sharedMaterial == (Object)null || (Object)(object)m_sharedMaterial.mainTexture == (Object)null || ((Object)m_fontAsset.atlas).GetInstanceID() != ((Object)m_sharedMaterial.mainTexture).GetInstanceID())
			{
				if ((Object)(object)m_fontAsset.material == (Object)null)
				{
					Debug.LogWarning((object)("The Font Atlas Texture of the Font Asset " + ((Object)m_fontAsset).name + " assigned to " + ((Object)((Component)this).gameObject).name + " is missing."), (Object)(object)this);
				}
				else
				{
					m_sharedMaterial = m_fontAsset.material;
				}
			}
		}
		GetSpecialCharacters(m_fontAsset);
		m_padding = GetPaddingForMaterial();
		((Graphic)this).SetMaterialDirty();
	}

	private Canvas GetCanvas()
	{
		Canvas result = null;
		List<Canvas> list = TMP_ListPool<Canvas>.Get();
		((Component)this).gameObject.GetComponentsInParent<Canvas>(false, list);
		if (list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (((Behaviour)list[i]).isActiveAndEnabled)
				{
					result = list[i];
					break;
				}
			}
		}
		TMP_ListPool<Canvas>.Release(list);
		return result;
	}

	private void UpdateEnvMapMatrix()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) && !((Object)(object)m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == (Object)null))
		{
			Vector3 val = Vector4.op_Implicit(m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation));
			m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(val), Vector3.one);
			m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, m_EnvMapMatrix);
		}
	}

	private void EnableMasking()
	{
		if ((Object)(object)m_fontMaterial == (Object)null)
		{
			m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
			m_canvasRenderer.SetMaterial(m_fontMaterial, m_sharedMaterial.mainTexture);
		}
		m_sharedMaterial = m_fontMaterial;
		if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_ClipRect))
		{
			m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
			m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_HARD);
			m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_TEX);
			UpdateMask();
		}
		m_isMaskingEnabled = true;
	}

	private void DisableMasking()
	{
		if ((Object)(object)m_fontMaterial != (Object)null)
		{
			if (m_stencilID > 0)
			{
				m_sharedMaterial = ((MaskableGraphic)this).m_MaskMaterial;
			}
			m_canvasRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			Object.DestroyImmediate((Object)(object)m_fontMaterial);
		}
		m_isMaskingEnabled = false;
	}

	private void UpdateMask()
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_rectTransform != (Object)null)
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.GetShaderPropertyIDs();
			}
			m_isScrollRegionSet = true;
			float num = Mathf.Min(Mathf.Min(m_margin.x, m_margin.z), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
			float num2 = Mathf.Min(Mathf.Min(m_margin.y, m_margin.w), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
			num = ((!(num > 0f)) ? 0f : num);
			num2 = ((!(num2 > 0f)) ? 0f : num2);
			Rect rect = m_rectTransform.rect;
			float num3 = (((Rect)(ref rect)).width - Mathf.Max(m_margin.x, 0f) - Mathf.Max(m_margin.z, 0f)) / 2f + num;
			Rect rect2 = m_rectTransform.rect;
			float num4 = (((Rect)(ref rect2)).height - Mathf.Max(m_margin.y, 0f) - Mathf.Max(m_margin.w, 0f)) / 2f + num2;
			Vector3 localPosition = ((Transform)m_rectTransform).localPosition;
			float num5 = 0.5f - m_rectTransform.pivot.x;
			Rect rect3 = m_rectTransform.rect;
			float num6 = num5 * ((Rect)(ref rect3)).width + (Mathf.Max(m_margin.x, 0f) - Mathf.Max(m_margin.z, 0f)) / 2f;
			float num7 = 0.5f - m_rectTransform.pivot.y;
			Rect rect4 = m_rectTransform.rect;
			Vector2 val = Vector2.op_Implicit(localPosition + new Vector3(num6, num7 * ((Rect)(ref rect4)).height + (0f - Mathf.Max(m_margin.y, 0f) + Mathf.Max(m_margin.w, 0f)) / 2f));
			Vector4 val2 = default(Vector4);
			((Vector4)(ref val2))._002Ector(val.x, val.y, num3, num4);
			m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, val2);
		}
	}

	protected override Material GetMaterial(Material mat)
	{
		ShaderUtilities.GetShaderPropertyIDs();
		if ((Object)(object)m_fontMaterial == (Object)null || ((Object)m_fontMaterial).GetInstanceID() != ((Object)mat).GetInstanceID())
		{
			m_fontMaterial = CreateMaterialInstance(mat);
		}
		m_sharedMaterial = m_fontMaterial;
		m_padding = GetPaddingForMaterial();
		((MaskableGraphic)this).m_ShouldRecalculateStencil = true;
		((Graphic)this).SetVerticesDirty();
		((Graphic)this).SetMaterialDirty();
		return m_sharedMaterial;
	}

	protected override Material[] GetMaterials(Material[] mats)
	{
		int materialCount = m_textInfo.materialCount;
		if (m_fontMaterials == null)
		{
			m_fontMaterials = (Material[])(object)new Material[materialCount];
		}
		else if (m_fontMaterials.Length != materialCount)
		{
			TMP_TextInfo.Resize(ref m_fontMaterials, materialCount, isBlockAllocated: false);
		}
		for (int i = 0; i < materialCount; i++)
		{
			if (i == 0)
			{
				m_fontMaterials[i] = base.fontMaterial;
			}
			else
			{
				m_fontMaterials[i] = ((Graphic)m_subTextObjects[i]).material;
			}
		}
		m_fontSharedMaterials = m_fontMaterials;
		return m_fontMaterials;
	}

	protected override void SetSharedMaterial(Material mat)
	{
		m_sharedMaterial = mat;
		m_padding = GetPaddingForMaterial();
		((Graphic)this).SetMaterialDirty();
	}

	protected override Material[] GetSharedMaterials()
	{
		int materialCount = m_textInfo.materialCount;
		if (m_fontSharedMaterials == null)
		{
			m_fontSharedMaterials = (Material[])(object)new Material[materialCount];
		}
		else if (m_fontSharedMaterials.Length != materialCount)
		{
			TMP_TextInfo.Resize(ref m_fontSharedMaterials, materialCount, isBlockAllocated: false);
		}
		for (int i = 0; i < materialCount; i++)
		{
			if (i == 0)
			{
				m_fontSharedMaterials[i] = m_sharedMaterial;
			}
			else
			{
				m_fontSharedMaterials[i] = m_subTextObjects[i].sharedMaterial;
			}
		}
		return m_fontSharedMaterials;
	}

	protected override void SetSharedMaterials(Material[] materials)
	{
		int materialCount = m_textInfo.materialCount;
		if (m_fontSharedMaterials == null)
		{
			m_fontSharedMaterials = (Material[])(object)new Material[materialCount];
		}
		else if (m_fontSharedMaterials.Length != materialCount)
		{
			TMP_TextInfo.Resize(ref m_fontSharedMaterials, materialCount, isBlockAllocated: false);
		}
		for (int i = 0; i < materialCount; i++)
		{
			if (i == 0)
			{
				if (!((Object)(object)materials[i].mainTexture == (Object)null) && ((Object)materials[i].mainTexture).GetInstanceID() == ((Object)m_sharedMaterial.mainTexture).GetInstanceID())
				{
					m_sharedMaterial = (m_fontSharedMaterials[i] = materials[i]);
					m_padding = GetPaddingForMaterial(m_sharedMaterial);
				}
			}
			else if (!((Object)(object)materials[i].mainTexture == (Object)null) && ((Object)materials[i].mainTexture).GetInstanceID() == ((Object)m_subTextObjects[i].sharedMaterial.mainTexture).GetInstanceID() && m_subTextObjects[i].isDefaultMaterial)
			{
				m_subTextObjects[i].sharedMaterial = (m_fontSharedMaterials[i] = materials[i]);
			}
		}
	}

	protected override void SetOutlineThickness(float thickness)
	{
		if ((Object)(object)m_fontMaterial != (Object)null && ((Object)m_sharedMaterial).GetInstanceID() != ((Object)m_fontMaterial).GetInstanceID())
		{
			m_sharedMaterial = m_fontMaterial;
			m_canvasRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
		}
		else if ((Object)(object)m_fontMaterial == (Object)null)
		{
			m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
			m_sharedMaterial = m_fontMaterial;
			m_canvasRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
		}
		thickness = Mathf.Clamp01(thickness);
		m_sharedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
		m_padding = GetPaddingForMaterial();
	}

	protected override void SetFaceColor(Color32 color)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_fontMaterial == (Object)null)
		{
			m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
		}
		m_sharedMaterial = m_fontMaterial;
		m_padding = GetPaddingForMaterial();
		m_sharedMaterial.SetColor(ShaderUtilities.ID_FaceColor, Color32.op_Implicit(color));
	}

	protected override void SetOutlineColor(Color32 color)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_fontMaterial == (Object)null)
		{
			m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
		}
		m_sharedMaterial = m_fontMaterial;
		m_padding = GetPaddingForMaterial();
		m_sharedMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color32.op_Implicit(color));
	}

	protected override void SetShaderDepth()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)m_canvas == (Object)null) && !((Object)(object)m_sharedMaterial == (Object)null))
		{
			if ((int)m_canvas.renderMode == 0 || m_isOverlay)
			{
				m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0f);
			}
			else
			{
				m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 4f);
			}
		}
	}

	protected override void SetCulling()
	{
		if (m_isCullingEnabled)
		{
			m_canvasRenderer.GetMaterial().SetFloat("_CullMode", 2f);
		}
		else
		{
			m_canvasRenderer.GetMaterial().SetFloat("_CullMode", 0f);
		}
	}

	private void SetPerspectiveCorrection()
	{
		if (m_isOrthographic)
		{
			m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0f);
		}
		else
		{
			m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
		}
	}

	protected override float GetPaddingForMaterial(Material mat)
	{
		m_padding = ShaderUtilities.GetPadding(mat, m_enableExtraPadding, m_isUsingBold);
		m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
		m_isSDFShader = mat.HasProperty(ShaderUtilities.ID_WeightNormal);
		return m_padding;
	}

	protected override float GetPaddingForMaterial()
	{
		ShaderUtilities.GetShaderPropertyIDs();
		m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
		m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
		m_isSDFShader = m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal);
		return m_padding;
	}

	private void SetMeshArrays(int size)
	{
		m_textInfo.meshInfo[0].ResizeMeshInfo(size);
		m_canvasRenderer.SetMesh(m_textInfo.meshInfo[0].mesh);
	}

	protected override int SetArraySizes(int[] chars)
	{
		//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Expected O, but got Unknown
		int endIndex = 0;
		int num = 0;
		m_totalCharacterCount = 0;
		m_isUsingBold = false;
		m_isParsingText = false;
		tag_NoParsing = false;
		m_style = m_fontStyle;
		m_fontWeightInternal = (((m_style & FontStyles.Bold) != FontStyles.Bold) ? m_fontWeight : 700);
		m_fontWeightStack.SetDefault(m_fontWeightInternal);
		m_currentFontAsset = m_fontAsset;
		m_currentMaterial = m_sharedMaterial;
		m_currentMaterialIndex = 0;
		m_materialReferenceStack.SetDefault(new MaterialReference(0, m_currentFontAsset, null, m_currentMaterial, m_padding));
		m_materialReferenceIndexLookup.Clear();
		MaterialReference.AddMaterialReference(m_currentMaterial, m_currentFontAsset, m_materialReferences, m_materialReferenceIndexLookup);
		if (m_textInfo == null)
		{
			m_textInfo = new TMP_TextInfo();
		}
		m_textElementType = TMP_TextElementType.Character;
		for (int i = 0; chars[i] != 0; i++)
		{
			if (m_textInfo.characterInfo == null || m_totalCharacterCount >= m_textInfo.characterInfo.Length)
			{
				TMP_TextInfo.Resize(ref m_textInfo.characterInfo, m_totalCharacterCount + 1, isBlockAllocated: true);
			}
			int num2 = chars[i];
			if (m_isRichText && num2 == 60)
			{
				int currentMaterialIndex = m_currentMaterialIndex;
				if (ValidateHtmlTag(chars, i + 1, out endIndex))
				{
					i = endIndex;
					if ((m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						m_isUsingBold = true;
					}
					if (m_textElementType == TMP_TextElementType.Sprite)
					{
						m_materialReferences[m_currentMaterialIndex].referenceCount++;
						m_textInfo.characterInfo[m_totalCharacterCount].character = (char)(57344 + m_spriteIndex);
						m_textInfo.characterInfo[m_totalCharacterCount].fontAsset = m_currentFontAsset;
						m_textInfo.characterInfo[m_totalCharacterCount].materialReferenceIndex = m_currentMaterialIndex;
						m_textElementType = TMP_TextElementType.Character;
						m_currentMaterialIndex = currentMaterialIndex;
						num++;
						m_totalCharacterCount++;
					}
					continue;
				}
			}
			bool flag = false;
			bool isUsingAlternateTypeface = false;
			TMP_FontAsset currentFontAsset = m_currentFontAsset;
			Material currentMaterial = m_currentMaterial;
			int currentMaterialIndex2 = m_currentMaterialIndex;
			if (m_textElementType == TMP_TextElementType.Character)
			{
				if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)num2))
					{
						num2 = char.ToUpper((char)num2);
					}
				}
				else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)num2))
					{
						num2 = char.ToLower((char)num2);
					}
				}
				else if (((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num2))
				{
					num2 = char.ToUpper((char)num2);
				}
			}
			TMP_FontAsset fontAssetForWeight = GetFontAssetForWeight(m_fontWeightInternal);
			if ((Object)(object)fontAssetForWeight != (Object)null)
			{
				flag = true;
				isUsingAlternateTypeface = true;
				m_currentFontAsset = fontAssetForWeight;
			}
			if (!m_currentFontAsset.characterDictionary.TryGetValue(num2, out var value))
			{
				if (m_currentFontAsset.fallbackFontAssets != null && m_currentFontAsset.fallbackFontAssets.Count > 0)
				{
					for (int j = 0; j < m_currentFontAsset.fallbackFontAssets.Count; j++)
					{
						fontAssetForWeight = m_currentFontAsset.fallbackFontAssets[j];
						if (!((Object)(object)fontAssetForWeight == (Object)null) && fontAssetForWeight.characterDictionary.TryGetValue(num2, out value))
						{
							flag = true;
							m_currentFontAsset = fontAssetForWeight;
							break;
						}
					}
				}
				if (value == null && TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
				{
					for (int k = 0; k < TMP_Settings.fallbackFontAssets.Count; k++)
					{
						fontAssetForWeight = TMP_Settings.fallbackFontAssets[k];
						if (!((Object)(object)fontAssetForWeight == (Object)null) && fontAssetForWeight.characterDictionary.TryGetValue(num2, out value))
						{
							flag = true;
							m_currentFontAsset = fontAssetForWeight;
							break;
						}
					}
				}
				if (value == null)
				{
					if (char.IsLower((char)num2))
					{
						if (m_currentFontAsset.characterDictionary.TryGetValue(char.ToUpper((char)num2), out value))
						{
							num2 = (chars[i] = char.ToUpper((char)num2));
						}
					}
					else if (char.IsUpper((char)num2) && m_currentFontAsset.characterDictionary.TryGetValue(char.ToLower((char)num2), out value))
					{
						num2 = (chars[i] = char.ToLower((char)num2));
					}
				}
				if (value == null)
				{
					int num3 = ((TMP_Settings.missingGlyphCharacter != 0) ? TMP_Settings.missingGlyphCharacter : 9633);
					if (m_currentFontAsset.characterDictionary.TryGetValue(num3, out value))
					{
						if (!TMP_Settings.warningsDisabled)
						{
							Debug.LogWarning((object)("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table."), (Object)(object)this);
						}
						num2 = (chars[i] = num3);
					}
					else
					{
						if (TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
						{
							for (int l = 0; l < TMP_Settings.fallbackFontAssets.Count; l++)
							{
								fontAssetForWeight = TMP_Settings.fallbackFontAssets[l];
								if (!((Object)(object)fontAssetForWeight == (Object)null) && fontAssetForWeight.characterDictionary.TryGetValue(num3, out value))
								{
									if (!TMP_Settings.warningsDisabled)
									{
										Debug.LogWarning((object)("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table."), (Object)(object)this);
									}
									num2 = (chars[i] = num3);
									flag = true;
									m_currentFontAsset = fontAssetForWeight;
									break;
								}
							}
						}
						if (value == null)
						{
							fontAssetForWeight = TMP_Settings.GetFontAsset();
							if ((Object)(object)fontAssetForWeight != (Object)null && fontAssetForWeight.characterDictionary.TryGetValue(num3, out value))
							{
								if (!TMP_Settings.warningsDisabled)
								{
									Debug.LogWarning((object)("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table."), (Object)(object)this);
								}
								num2 = (chars[i] = num3);
								flag = true;
								m_currentFontAsset = fontAssetForWeight;
							}
							else
							{
								fontAssetForWeight = TMP_FontAsset.defaultFontAsset;
								if ((Object)(object)fontAssetForWeight != (Object)null && fontAssetForWeight.characterDictionary.TryGetValue(num3, out value))
								{
									if (!TMP_Settings.warningsDisabled)
									{
										Debug.LogWarning((object)("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table."), (Object)(object)this);
									}
									num2 = (chars[i] = num3);
									flag = true;
									m_currentFontAsset = fontAssetForWeight;
								}
								else if (m_currentFontAsset.characterDictionary.TryGetValue(32, out value))
								{
									if (!TMP_Settings.warningsDisabled)
									{
										Debug.LogWarning((object)("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table. It was replaced by a space."), (Object)(object)this);
									}
									num2 = (chars[i] = 32);
								}
							}
						}
					}
				}
			}
			m_textInfo.characterInfo[m_totalCharacterCount].textElement = value;
			m_textInfo.characterInfo[m_totalCharacterCount].isUsingAlternateTypeface = isUsingAlternateTypeface;
			m_textInfo.characterInfo[m_totalCharacterCount].character = (char)num2;
			m_textInfo.characterInfo[m_totalCharacterCount].fontAsset = m_currentFontAsset;
			if (flag)
			{
				if (TMP_Settings.matchMaterialPreset)
				{
					m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(m_currentMaterial, m_currentFontAsset.material);
				}
				else
				{
					m_currentMaterial = m_currentFontAsset.material;
				}
				m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, m_currentFontAsset, m_materialReferences, m_materialReferenceIndexLookup);
			}
			if (!char.IsWhiteSpace((char)num2))
			{
				if (m_materialReferences[m_currentMaterialIndex].referenceCount < 16383)
				{
					m_materialReferences[m_currentMaterialIndex].referenceCount++;
				}
				else
				{
					m_currentMaterialIndex = MaterialReference.AddMaterialReference(new Material(m_currentMaterial), m_currentFontAsset, m_materialReferences, m_materialReferenceIndexLookup);
					m_materialReferences[m_currentMaterialIndex].referenceCount++;
				}
			}
			m_textInfo.characterInfo[m_totalCharacterCount].material = m_currentMaterial;
			m_textInfo.characterInfo[m_totalCharacterCount].materialReferenceIndex = m_currentMaterialIndex;
			m_materialReferences[m_currentMaterialIndex].isFallbackMaterial = flag;
			if (flag)
			{
				m_materialReferences[m_currentMaterialIndex].fallbackMaterial = currentMaterial;
				m_currentFontAsset = currentFontAsset;
				m_currentMaterial = currentMaterial;
				m_currentMaterialIndex = currentMaterialIndex2;
			}
			m_totalCharacterCount++;
		}
		if (m_isCalculatingPreferredValues)
		{
			m_isCalculatingPreferredValues = false;
			m_isInputParsingRequired = true;
			return m_totalCharacterCount;
		}
		m_textInfo.spriteCount = num;
		int num4 = (m_textInfo.materialCount = m_materialReferenceIndexLookup.Count);
		if (num4 > m_textInfo.meshInfo.Length)
		{
			TMP_TextInfo.Resize(ref m_textInfo.meshInfo, num4, isBlockAllocated: false);
		}
		if (m_textInfo.characterInfo.Length - m_totalCharacterCount > 256)
		{
			TMP_TextInfo.Resize(ref m_textInfo.characterInfo, Mathf.Max(m_totalCharacterCount + 1, 256), isBlockAllocated: true);
		}
		for (int m = 0; m < num4; m++)
		{
			if (m > 0)
			{
				if ((Object)(object)m_subTextObjects[m] == (Object)null)
				{
					m_subTextObjects[m] = TMP_SubMeshUI.AddSubTextObject(this, m_materialReferences[m]);
					m_textInfo.meshInfo[m].vertices = null;
				}
				if (m_rectTransform.pivot != ((Graphic)m_subTextObjects[m]).rectTransform.pivot)
				{
					((Graphic)m_subTextObjects[m]).rectTransform.pivot = m_rectTransform.pivot;
				}
				if ((Object)(object)m_subTextObjects[m].sharedMaterial == (Object)null || ((Object)m_subTextObjects[m].sharedMaterial).GetInstanceID() != ((Object)m_materialReferences[m].material).GetInstanceID())
				{
					bool isDefaultMaterial = m_materialReferences[m].isDefaultMaterial;
					m_subTextObjects[m].isDefaultMaterial = isDefaultMaterial;
					if (!isDefaultMaterial || (Object)(object)m_subTextObjects[m].sharedMaterial == (Object)null || ((Object)m_subTextObjects[m].sharedMaterial.mainTexture).GetInstanceID() != ((Object)m_materialReferences[m].material.GetTexture(ShaderUtilities.ID_MainTex)).GetInstanceID())
					{
						m_subTextObjects[m].sharedMaterial = m_materialReferences[m].material;
						m_subTextObjects[m].fontAsset = m_materialReferences[m].fontAsset;
						m_subTextObjects[m].spriteAsset = m_materialReferences[m].spriteAsset;
					}
				}
				if (m_materialReferences[m].isFallbackMaterial)
				{
					m_subTextObjects[m].fallbackMaterial = m_materialReferences[m].material;
					m_subTextObjects[m].fallbackSourceMaterial = m_materialReferences[m].fallbackMaterial;
				}
			}
			int referenceCount = m_materialReferences[m].referenceCount;
			if (m_textInfo.meshInfo[m].vertices == null || m_textInfo.meshInfo[m].vertices.Length < referenceCount * 4)
			{
				if (m_textInfo.meshInfo[m].vertices == null)
				{
					if (m == 0)
					{
						ref TMP_MeshInfo reference = ref m_textInfo.meshInfo[m];
						reference = new TMP_MeshInfo(m_mesh, referenceCount + 1);
					}
					else
					{
						ref TMP_MeshInfo reference2 = ref m_textInfo.meshInfo[m];
						reference2 = new TMP_MeshInfo(m_subTextObjects[m].mesh, referenceCount + 1);
					}
				}
				else
				{
					m_textInfo.meshInfo[m].ResizeMeshInfo((referenceCount <= 1024) ? Mathf.NextPowerOfTwo(referenceCount) : (referenceCount + 256));
				}
			}
			else if (m_textInfo.meshInfo[m].vertices.Length - referenceCount * 4 > 1024)
			{
				m_textInfo.meshInfo[m].ResizeMeshInfo((referenceCount <= 1024) ? Mathf.Max(Mathf.NextPowerOfTwo(referenceCount), 256) : (referenceCount + 256));
			}
		}
		for (int n = num4; n < m_subTextObjects.Length && (Object)(object)m_subTextObjects[n] != (Object)null; n++)
		{
			if (n < m_textInfo.meshInfo.Length)
			{
				m_subTextObjects[n].canvasRenderer.SetMesh((Mesh)null);
			}
		}
		return m_totalCharacterCount;
	}

	protected override void ComputeMarginSize()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)base.rectTransform != (Object)null)
		{
			Rect rect = m_rectTransform.rect;
			m_marginWidth = ((Rect)(ref rect)).width - m_margin.x - m_margin.z;
			Rect rect2 = m_rectTransform.rect;
			m_marginHeight = ((Rect)(ref rect2)).height - m_margin.y - m_margin.w;
			m_RectTransformCorners = GetTextContainerLocalCorners();
		}
	}

	protected override void OnDidApplyAnimationProperties()
	{
		m_havePropertiesChanged = true;
		((Graphic)this).SetVerticesDirty();
		((Graphic)this).SetLayoutDirty();
	}

	protected override void OnCanvasHierarchyChanged()
	{
		((MaskableGraphic)this).OnCanvasHierarchyChanged();
		m_canvas = ((Graphic)this).canvas;
	}

	protected override void OnTransformParentChanged()
	{
		((MaskableGraphic)this).OnTransformParentChanged();
		m_canvas = ((Graphic)this).canvas;
		ComputeMarginSize();
		m_havePropertiesChanged = true;
	}

	protected override void OnRectTransformDimensionsChange()
	{
		if (((Component)this).gameObject.activeInHierarchy)
		{
			ComputeMarginSize();
			UpdateSubObjectPivot();
			((Graphic)this).SetVerticesDirty();
			((Graphic)this).SetLayoutDirty();
		}
	}

	private void LateUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (((Transform)m_rectTransform).hasChanged)
		{
			float y = ((Transform)m_rectTransform).lossyScale.y;
			if (!m_havePropertiesChanged && y != m_previousLossyScaleY && m_text != string.Empty && m_text != null)
			{
				UpdateSDFScale(y);
				m_previousLossyScaleY = y;
			}
			((Transform)m_rectTransform).hasChanged = false;
		}
		if (m_isUsingLegacyAnimationComponent)
		{
			m_havePropertiesChanged = true;
			OnPreRenderCanvas();
		}
	}

	private void OnPreRenderCanvas()
	{
		if (!m_isAwake || (!m_ignoreActiveState && !((UIBehaviour)this).IsActive()))
		{
			return;
		}
		if ((Object)(object)m_canvas == (Object)null)
		{
			m_canvas = ((Graphic)this).canvas;
			if ((Object)(object)m_canvas == (Object)null)
			{
				return;
			}
		}
		loopCountA = 0;
		if (m_havePropertiesChanged || m_isLayoutDirty)
		{
			if (checkPaddingRequired)
			{
				UpdateMeshPadding();
			}
			if (m_isInputParsingRequired || m_isTextTruncated)
			{
				ParseInputText();
			}
			if (m_enableAutoSizing)
			{
				m_fontSize = Mathf.Clamp(m_fontSize, m_fontSizeMin, m_fontSizeMax);
			}
			m_maxFontSize = m_fontSizeMax;
			m_minFontSize = m_fontSizeMin;
			m_lineSpacingDelta = 0f;
			m_charWidthAdjDelta = 0f;
			m_recursiveCount = 0;
			m_isCharacterWrappingEnabled = false;
			m_isTextTruncated = false;
			m_havePropertiesChanged = false;
			m_isLayoutDirty = false;
			m_ignoreActiveState = false;
			GenerateTextMesh();
		}
	}

	protected override void GenerateTextMesh()
	{
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_30dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_30ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_30ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_30f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_310f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3114: Unknown result type (might be due to invalid IL or missing references)
		//IL_3119: Unknown result type (might be due to invalid IL or missing references)
		//IL_3128: Unknown result type (might be due to invalid IL or missing references)
		//IL_3135: Unknown result type (might be due to invalid IL or missing references)
		//IL_313a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3144: Unknown result type (might be due to invalid IL or missing references)
		//IL_3192: Unknown result type (might be due to invalid IL or missing references)
		//IL_3197: Unknown result type (might be due to invalid IL or missing references)
		//IL_319c: Unknown result type (might be due to invalid IL or missing references)
		//IL_31ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_31b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_31bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_31c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_31fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_31ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_3204: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ef1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f27: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f31: Unknown result type (might be due to invalid IL or missing references)
		//IL_2eb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ed8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2edd: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ee2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_302f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3034: Unknown result type (might be due to invalid IL or missing references)
		//IL_3039: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f59: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f68: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_308e: Unknown result type (might be due to invalid IL or missing references)
		//IL_30c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_30c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_30ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_3054: Unknown result type (might be due to invalid IL or missing references)
		//IL_3075: Unknown result type (might be due to invalid IL or missing references)
		//IL_307a: Unknown result type (might be due to invalid IL or missing references)
		//IL_307f: Unknown result type (might be due to invalid IL or missing references)
		//IL_320b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3210: Unknown result type (might be due to invalid IL or missing references)
		//IL_3212: Unknown result type (might be due to invalid IL or missing references)
		//IL_3217: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_3257: Unknown result type (might be due to invalid IL or missing references)
		//IL_325c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3277: Unknown result type (might be due to invalid IL or missing references)
		//IL_327c: Unknown result type (might be due to invalid IL or missing references)
		//IL_328b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3290: Unknown result type (might be due to invalid IL or missing references)
		//IL_3295: Unknown result type (might be due to invalid IL or missing references)
		//IL_3297: Unknown result type (might be due to invalid IL or missing references)
		//IL_329c: Unknown result type (might be due to invalid IL or missing references)
		//IL_32a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_36ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_36f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_36f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_36f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a72: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c32: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c96: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d82: Unknown result type (might be due to invalid IL or missing references)
		//IL_4de6: Unknown result type (might be due to invalid IL or missing references)
		//IL_4deb: Unknown result type (might be due to invalid IL or missing references)
		//IL_4974: Unknown result type (might be due to invalid IL or missing references)
		//IL_4979: Unknown result type (might be due to invalid IL or missing references)
		//IL_498c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4991: Unknown result type (might be due to invalid IL or missing references)
		//IL_49a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_49a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_49bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_49c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_4617: Unknown result type (might be due to invalid IL or missing references)
		//IL_4830: Unknown result type (might be due to invalid IL or missing references)
		//IL_4835: Unknown result type (might be due to invalid IL or missing references)
		//IL_4837: Unknown result type (might be due to invalid IL or missing references)
		//IL_483c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4850: Unknown result type (might be due to invalid IL or missing references)
		//IL_4855: Unknown result type (might be due to invalid IL or missing references)
		//IL_4857: Unknown result type (might be due to invalid IL or missing references)
		//IL_485c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4870: Unknown result type (might be due to invalid IL or missing references)
		//IL_4875: Unknown result type (might be due to invalid IL or missing references)
		//IL_4877: Unknown result type (might be due to invalid IL or missing references)
		//IL_487c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4890: Unknown result type (might be due to invalid IL or missing references)
		//IL_4895: Unknown result type (might be due to invalid IL or missing references)
		//IL_4897: Unknown result type (might be due to invalid IL or missing references)
		//IL_489c: Unknown result type (might be due to invalid IL or missing references)
		//IL_461e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4621: Invalid comparison between Unknown and I4
		//IL_4626: Unknown result type (might be due to invalid IL or missing references)
		//IL_4629: Invalid comparison between Unknown and I4
		//IL_48f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_48f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_48f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_48fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_4910: Unknown result type (might be due to invalid IL or missing references)
		//IL_4915: Unknown result type (might be due to invalid IL or missing references)
		//IL_4917: Unknown result type (might be due to invalid IL or missing references)
		//IL_491c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4930: Unknown result type (might be due to invalid IL or missing references)
		//IL_4935: Unknown result type (might be due to invalid IL or missing references)
		//IL_4937: Unknown result type (might be due to invalid IL or missing references)
		//IL_493c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4950: Unknown result type (might be due to invalid IL or missing references)
		//IL_4955: Unknown result type (might be due to invalid IL or missing references)
		//IL_4957: Unknown result type (might be due to invalid IL or missing references)
		//IL_495c: Unknown result type (might be due to invalid IL or missing references)
		//IL_54de: Unknown result type (might be due to invalid IL or missing references)
		//IL_54e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_54ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_5ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_5ad5: Unknown result type (might be due to invalid IL or missing references)
		//IL_5ae1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3675: Unknown result type (might be due to invalid IL or missing references)
		//IL_3691: Unknown result type (might be due to invalid IL or missing references)
		//IL_3696: Unknown result type (might be due to invalid IL or missing references)
		//IL_369b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3648: Unknown result type (might be due to invalid IL or missing references)
		//IL_3664: Unknown result type (might be due to invalid IL or missing references)
		//IL_3669: Unknown result type (might be due to invalid IL or missing references)
		//IL_366e: Unknown result type (might be due to invalid IL or missing references)
		//IL_52f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_52f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_5302: Unknown result type (might be due to invalid IL or missing references)
		//IL_3611: Unknown result type (might be due to invalid IL or missing references)
		//IL_362c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3631: Unknown result type (might be due to invalid IL or missing references)
		//IL_3636: Unknown result type (might be due to invalid IL or missing references)
		//IL_35e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_3600: Unknown result type (might be due to invalid IL or missing references)
		//IL_3605: Unknown result type (might be due to invalid IL or missing references)
		//IL_360a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5717: Unknown result type (might be due to invalid IL or missing references)
		//IL_5719: Unknown result type (might be due to invalid IL or missing references)
		//IL_5725: Unknown result type (might be due to invalid IL or missing references)
		//IL_5464: Unknown result type (might be due to invalid IL or missing references)
		//IL_5466: Unknown result type (might be due to invalid IL or missing references)
		//IL_5472: Unknown result type (might be due to invalid IL or missing references)
		//IL_53e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_53e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_53f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5838: Unknown result type (might be due to invalid IL or missing references)
		//IL_583a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5846: Unknown result type (might be due to invalid IL or missing references)
		//IL_5676: Unknown result type (might be due to invalid IL or missing references)
		//IL_567b: Unknown result type (might be due to invalid IL or missing references)
		//IL_5293: Unknown result type (might be due to invalid IL or missing references)
		//IL_5298: Unknown result type (might be due to invalid IL or missing references)
		//IL_59a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_59a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_59b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a82: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_24ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_24b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_24f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_24f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1510: Unknown result type (might be due to invalid IL or missing references)
		//IL_1515: Unknown result type (might be due to invalid IL or missing references)
		//IL_1557: Unknown result type (might be due to invalid IL or missing references)
		//IL_155c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_fontAsset == (Object)null || m_fontAsset.characterDictionary == null)
		{
			Debug.LogWarning((object)("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + ((Object)this).GetInstanceID()));
			return;
		}
		if (m_textInfo != null)
		{
			m_textInfo.Clear();
		}
		if (m_char_buffer == null || m_char_buffer.Length == 0 || m_char_buffer[0] == 0)
		{
			ClearMesh();
			m_preferredWidth = 0f;
			m_preferredHeight = 0f;
			TMPro_EventManager.ON_TEXT_CHANGED((Object)(object)this);
			return;
		}
		m_currentFontAsset = m_fontAsset;
		m_currentMaterial = m_sharedMaterial;
		m_currentMaterialIndex = 0;
		m_materialReferenceStack.SetDefault(new MaterialReference(0, m_currentFontAsset, null, m_currentMaterial, m_padding));
		m_currentSpriteAsset = m_spriteAsset;
		int totalCharacterCount = m_totalCharacterCount;
		m_fontScale = m_fontSize / m_currentFontAsset.fontInfo.PointSize;
		float num = m_fontSize / m_fontAsset.fontInfo.PointSize * m_fontAsset.fontInfo.Scale;
		float num2 = m_fontScale;
		m_fontScaleMultiplier = 1f;
		m_currentFontSize = m_fontSize;
		m_sizeStack.SetDefault(m_currentFontSize);
		float num3 = 0f;
		int num4 = 0;
		m_style = m_fontStyle;
		m_fontWeightInternal = (((m_style & FontStyles.Bold) != FontStyles.Bold) ? m_fontWeight : 700);
		m_fontWeightStack.SetDefault(m_fontWeightInternal);
		m_lineJustification = m_textAlignment;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 1f;
		m_baselineOffset = 0f;
		bool flag = false;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		bool flag2 = false;
		Vector3 zero3 = Vector3.zero;
		Vector3 zero4 = Vector3.zero;
		m_fontColor32 = Color32.op_Implicit(m_fontColor);
		m_htmlColor = m_fontColor32;
		m_colorStack.SetDefault(m_htmlColor);
		m_styleStack.Clear();
		m_actionStack.Clear();
		m_lineOffset = 0f;
		m_lineHeight = 0f;
		float num8 = m_currentFontAsset.fontInfo.LineHeight - (m_currentFontAsset.fontInfo.Ascender - m_currentFontAsset.fontInfo.Descender);
		m_cSpacing = 0f;
		m_monoSpacing = 0f;
		float num9 = 0f;
		m_xAdvance = 0f;
		tag_LineIndent = 0f;
		tag_Indent = 0f;
		m_indentStack.SetDefault(0f);
		tag_NoParsing = false;
		m_characterCount = 0;
		m_firstCharacterOfLine = 0;
		m_lastCharacterOfLine = 0;
		m_firstVisibleCharacterOfLine = 0;
		m_lastVisibleCharacterOfLine = 0;
		m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
		m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
		m_lineNumber = 0;
		m_lineVisibleCharacterCount = 0;
		bool flag3 = true;
		m_pageNumber = 0;
		int num10 = Mathf.Clamp(m_pageToDisplay - 1, 0, m_textInfo.pageInfo.Length - 1);
		int num11 = 0;
		Vector4 val = m_margin;
		float marginWidth = m_marginWidth;
		float marginHeight = m_marginHeight;
		m_marginLeft = 0f;
		m_marginRight = 0f;
		m_width = -1f;
		float num12 = marginWidth + 0.0001f - m_marginLeft - m_marginRight;
		m_meshExtents.min = TMP_Text.k_LargePositiveVector2;
		m_meshExtents.max = TMP_Text.k_LargeNegativeVector2;
		m_textInfo.ClearLineInfo();
		m_maxCapHeight = 0f;
		m_maxAscender = 0f;
		m_maxDescender = 0f;
		float num13 = 0f;
		float num14 = 0f;
		bool flag4 = false;
		m_isNewPage = false;
		bool flag5 = true;
		bool flag6 = false;
		int num15 = 0;
		loopCountA++;
		int endIndex = 0;
		Vector3 val2 = default(Vector3);
		Vector3 val3 = default(Vector3);
		Vector3 val4 = default(Vector3);
		Vector3 val5 = default(Vector3);
		Vector3 val6 = default(Vector3);
		Vector3 val7 = default(Vector3);
		for (int i = 0; m_char_buffer[i] != 0; i++)
		{
			num4 = m_char_buffer[i];
			m_textElementType = TMP_TextElementType.Character;
			m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
			m_currentFontAsset = m_materialReferences[m_currentMaterialIndex].fontAsset;
			int currentMaterialIndex = m_currentMaterialIndex;
			if (m_isRichText && num4 == 60)
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
			float num16 = 1f;
			if (m_textElementType == TMP_TextElementType.Character)
			{
				if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)num4))
					{
						num4 = char.ToUpper((char)num4);
					}
				}
				else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)num4))
					{
						num4 = char.ToLower((char)num4);
					}
				}
				else if (((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num4))
				{
					num16 = 0.8f;
					num4 = char.ToUpper((char)num4);
				}
			}
			if (m_textElementType == TMP_TextElementType.Sprite)
			{
				TMP_Sprite tMP_Sprite = m_currentSpriteAsset.spriteInfoList[m_spriteIndex];
				if (tMP_Sprite == null)
				{
					continue;
				}
				num4 = 57344 + m_spriteIndex;
				m_currentFontAsset = m_fontAsset;
				float num17 = m_currentFontSize / m_fontAsset.fontInfo.PointSize * m_fontAsset.fontInfo.Scale;
				num2 = m_fontAsset.fontInfo.Ascender / tMP_Sprite.height * tMP_Sprite.scale * num17;
				m_cached_TextElement = tMP_Sprite;
				m_textInfo.characterInfo[m_characterCount].elementType = TMP_TextElementType.Sprite;
				m_textInfo.characterInfo[m_characterCount].scale = num17;
				m_textInfo.characterInfo[m_characterCount].spriteAsset = m_currentSpriteAsset;
				m_textInfo.characterInfo[m_characterCount].fontAsset = m_currentFontAsset;
				m_textInfo.characterInfo[m_characterCount].materialReferenceIndex = m_currentMaterialIndex;
				m_currentMaterialIndex = currentMaterialIndex;
				num5 = 0f;
			}
			else if (m_textElementType == TMP_TextElementType.Character)
			{
				m_cached_TextElement = m_textInfo.characterInfo[m_characterCount].textElement;
				if (m_cached_TextElement == null)
				{
					continue;
				}
				m_currentFontAsset = m_textInfo.characterInfo[m_characterCount].fontAsset;
				m_currentMaterial = m_textInfo.characterInfo[m_characterCount].material;
				m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
				m_fontScale = m_currentFontSize * num16 / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale;
				num2 = m_fontScale * m_fontScaleMultiplier * m_cached_TextElement.scale;
				m_textInfo.characterInfo[m_characterCount].elementType = TMP_TextElementType.Character;
				m_textInfo.characterInfo[m_characterCount].scale = num2;
				num5 = ((m_currentMaterialIndex != 0) ? m_subTextObjects[m_currentMaterialIndex].padding : m_padding);
			}
			float num18 = num2;
			if (num4 == 173)
			{
				num2 = 0f;
			}
			if (m_isRightToLeft)
			{
				m_xAdvance -= ((m_cached_TextElement.xAdvance * num7 + m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing) * (1f - m_charWidthAdjDelta);
			}
			m_textInfo.characterInfo[m_characterCount].character = (char)num4;
			m_textInfo.characterInfo[m_characterCount].pointSize = m_currentFontSize;
			m_textInfo.characterInfo[m_characterCount].color = m_htmlColor;
			m_textInfo.characterInfo[m_characterCount].style = m_style;
			m_textInfo.characterInfo[m_characterCount].index = (short)i;
			if (m_enableKerning && m_characterCount >= 1)
			{
				int character = m_textInfo.characterInfo[m_characterCount - 1].character;
				KerningPairKey kerningPairKey = new KerningPairKey(character, num4);
				m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out var value);
				if (value != null)
				{
					m_xAdvance += value.XadvanceOffset * num2;
				}
			}
			float num19 = 0f;
			if (m_monoSpacing != 0f)
			{
				num19 = (m_monoSpacing / 2f - (m_cached_TextElement.width / 2f + m_cached_TextElement.xOffset) * num2) * (1f - m_charWidthAdjDelta);
				m_xAdvance += num19;
			}
			if (m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold))
			{
				num6 = m_currentFontAsset.boldStyle * 2f;
				num7 = 1f + m_currentFontAsset.boldSpacing * 0.01f;
			}
			else
			{
				num6 = m_currentFontAsset.normalStyle * 2f;
				num7 = 1f;
			}
			float baseline = m_currentFontAsset.fontInfo.Baseline;
			val2.x = m_xAdvance + (m_cached_TextElement.xOffset - num5 - num6) * num2 * (1f - m_charWidthAdjDelta);
			val2.y = (baseline + m_cached_TextElement.yOffset + num5) * num2 - m_lineOffset + m_baselineOffset;
			val2.z = 0f;
			val3.x = val2.x;
			val3.y = val2.y - (m_cached_TextElement.height + num5 * 2f) * num2;
			val3.z = 0f;
			val4.x = val3.x + (m_cached_TextElement.width + num5 * 2f + num6 * 2f) * num2 * (1f - m_charWidthAdjDelta);
			val4.y = val2.y;
			val4.z = 0f;
			val5.x = val4.x;
			val5.y = val3.y;
			val5.z = 0f;
			if (m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic))
			{
				float num20 = (float)(int)m_currentFontAsset.italicStyle * 0.01f;
				((Vector3)(ref val6))._002Ector(num20 * ((m_cached_TextElement.yOffset + num5 + num6) * num2), 0f, 0f);
				((Vector3)(ref val7))._002Ector(num20 * ((m_cached_TextElement.yOffset - m_cached_TextElement.height - num5 - num6) * num2), 0f, 0f);
				val2 += val6;
				val3 += val7;
				val4 += val6;
				val5 += val7;
			}
			m_textInfo.characterInfo[m_characterCount].bottomLeft = val3;
			m_textInfo.characterInfo[m_characterCount].topLeft = val2;
			m_textInfo.characterInfo[m_characterCount].topRight = val4;
			m_textInfo.characterInfo[m_characterCount].bottomRight = val5;
			m_textInfo.characterInfo[m_characterCount].origin = m_xAdvance;
			m_textInfo.characterInfo[m_characterCount].baseLine = 0f - m_lineOffset + m_baselineOffset;
			m_textInfo.characterInfo[m_characterCount].aspectRatio = (val4.x - val3.x) / (val2.y - val3.y);
			float num21 = m_currentFontAsset.fontInfo.Ascender * ((m_textElementType != TMP_TextElementType.Character) ? m_textInfo.characterInfo[m_characterCount].scale : num2) + m_baselineOffset;
			m_textInfo.characterInfo[m_characterCount].ascender = num21 - m_lineOffset;
			m_maxLineAscender = ((!(num21 > m_maxLineAscender)) ? m_maxLineAscender : num21);
			float num22 = m_currentFontAsset.fontInfo.Descender * ((m_textElementType != TMP_TextElementType.Character) ? m_textInfo.characterInfo[m_characterCount].scale : num2) + m_baselineOffset;
			float num23 = (m_textInfo.characterInfo[m_characterCount].descender = num22 - m_lineOffset);
			m_maxLineDescender = ((!(num22 < m_maxLineDescender)) ? m_maxLineDescender : num22);
			if ((m_style & FontStyles.Subscript) == FontStyles.Subscript || (m_style & FontStyles.Superscript) == FontStyles.Superscript)
			{
				float num24 = (num21 - m_baselineOffset) / m_currentFontAsset.fontInfo.SubSize;
				num21 = m_maxLineAscender;
				m_maxLineAscender = ((!(num24 > m_maxLineAscender)) ? m_maxLineAscender : num24);
				float num25 = (num22 - m_baselineOffset) / m_currentFontAsset.fontInfo.SubSize;
				num22 = m_maxLineDescender;
				m_maxLineDescender = ((!(num25 < m_maxLineDescender)) ? m_maxLineDescender : num25);
			}
			if (m_lineNumber == 0)
			{
				m_maxAscender = ((!(m_maxAscender > num21)) ? num21 : m_maxAscender);
				m_maxCapHeight = Mathf.Max(m_maxCapHeight, m_currentFontAsset.fontInfo.CapHeight * num2);
			}
			if (m_lineOffset == 0f)
			{
				num13 = ((!(num13 > num21)) ? num21 : num13);
			}
			m_textInfo.characterInfo[m_characterCount].isVisible = false;
			if (num4 == 9 || !char.IsWhiteSpace((char)num4) || m_textElementType == TMP_TextElementType.Sprite)
			{
				m_textInfo.characterInfo[m_characterCount].isVisible = true;
				num12 = ((m_width == -1f) ? (marginWidth + 0.0001f - m_marginLeft - m_marginRight) : Mathf.Min(marginWidth + 0.0001f - m_marginLeft - m_marginRight, m_width));
				m_textInfo.lineInfo[m_lineNumber].marginLeft = m_marginLeft;
				if (Mathf.Abs(m_xAdvance) + (m_isRightToLeft ? 0f : m_cached_TextElement.xAdvance) * (1f - m_charWidthAdjDelta) * ((num4 == 173) ? num18 : num2) > num12)
				{
					num11 = m_characterCount - 1;
					if (base.enableWordWrapping && m_characterCount != m_firstCharacterOfLine)
					{
						if (num15 == m_SavedWordWrapState.previous_WordBreak || flag5)
						{
							if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
							{
								if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100f)
								{
									loopCountA = 0;
									m_charWidthAdjDelta += 0.01f;
									GenerateTextMesh();
									return;
								}
								m_maxFontSize = m_fontSize;
								m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
								m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
								if (loopCountA <= 20)
								{
									GenerateTextMesh();
								}
								return;
							}
							if (!m_isCharacterWrappingEnabled)
							{
								m_isCharacterWrappingEnabled = true;
							}
							else
							{
								flag6 = true;
							}
							m_recursiveCount++;
							if (m_recursiveCount > 20)
							{
								continue;
							}
						}
						i = RestoreWordWrappingState(ref m_SavedWordWrapState);
						num15 = i;
						if (m_char_buffer[i] == 173)
						{
							m_isTextTruncated = true;
							m_char_buffer[i] = 45;
							GenerateTextMesh();
							return;
						}
						if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f && !m_isNewPage)
						{
							float num26 = m_maxLineAscender - m_startOfLineAscender;
							AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num26);
							m_lineOffset += num26;
							m_SavedWordWrapState.lineOffset = m_lineOffset;
							m_SavedWordWrapState.previousLineAscender = m_maxLineAscender;
						}
						m_isNewPage = false;
						float num27 = m_maxLineAscender - m_lineOffset;
						float num28 = m_maxLineDescender - m_lineOffset;
						m_maxDescender = ((!(m_maxDescender < num28)) ? num28 : m_maxDescender);
						if (!flag4)
						{
							num14 = m_maxDescender;
						}
						if (m_useMaxVisibleDescender && (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines))
						{
							flag4 = true;
						}
						m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstCharacterOfLine;
						m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex = (m_firstVisibleCharacterOfLine = ((m_firstCharacterOfLine <= m_firstVisibleCharacterOfLine) ? m_firstVisibleCharacterOfLine : m_firstCharacterOfLine));
						m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = (m_lastCharacterOfLine = ((m_characterCount - 1 > 0) ? (m_characterCount - 1) : 0));
						m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = (m_lastVisibleCharacterOfLine = ((m_lastVisibleCharacterOfLine >= m_firstVisibleCharacterOfLine) ? m_lastVisibleCharacterOfLine : m_firstVisibleCharacterOfLine));
						m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;
						m_textInfo.lineInfo[m_lineNumber].visibleCharacterCount = m_lineVisibleCharacterCount;
						m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, num28);
						m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, num27);
						m_textInfo.lineInfo[m_lineNumber].length = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x;
						m_textInfo.lineInfo[m_lineNumber].width = num12;
						m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 - m_cSpacing;
						m_textInfo.lineInfo[m_lineNumber].baseline = 0f - m_lineOffset;
						m_textInfo.lineInfo[m_lineNumber].ascender = num27;
						m_textInfo.lineInfo[m_lineNumber].descender = num28;
						m_textInfo.lineInfo[m_lineNumber].lineHeight = num27 - num28 + num8 * num;
						m_firstCharacterOfLine = m_characterCount;
						m_lineVisibleCharacterCount = 0;
						SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount - 1);
						m_lineNumber++;
						flag3 = true;
						if (m_lineNumber >= m_textInfo.lineInfo.Length)
						{
							ResizeLineExtents(m_lineNumber);
						}
						if (m_lineHeight == 0f)
						{
							float num29 = m_textInfo.characterInfo[m_characterCount].ascender - m_textInfo.characterInfo[m_characterCount].baseLine;
							num9 = 0f - m_maxLineDescender + num29 + (num8 + m_lineSpacing + m_lineSpacingDelta) * num;
							m_lineOffset += num9;
							m_startOfLineAscender = num29;
						}
						else
						{
							m_lineOffset += m_lineHeight + m_lineSpacing * num;
						}
						m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
						m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
						m_xAdvance = tag_Indent;
						continue;
					}
					if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
					{
						if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100f)
						{
							loopCountA = 0;
							m_charWidthAdjDelta += 0.01f;
							GenerateTextMesh();
							return;
						}
						m_maxFontSize = m_fontSize;
						m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
						m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
						m_recursiveCount = 0;
						if (loopCountA <= 20)
						{
							GenerateTextMesh();
						}
						return;
					}
					switch (m_overflowMode)
					{
					case TextOverflowModes.Overflow:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						break;
					case TextOverflowModes.Ellipsis:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						m_isTextTruncated = true;
						if (m_characterCount < 1)
						{
							m_textInfo.characterInfo[m_characterCount].isVisible = false;
							break;
						}
						m_char_buffer[i - 1] = 8230;
						m_char_buffer[i] = 0;
						if (m_cached_Ellipsis_GlyphInfo != null)
						{
							m_textInfo.characterInfo[num11].character = '…';
							m_textInfo.characterInfo[num11].textElement = m_cached_Ellipsis_GlyphInfo;
							m_textInfo.characterInfo[num11].fontAsset = m_materialReferences[0].fontAsset;
							m_textInfo.characterInfo[num11].material = m_materialReferences[0].material;
							m_textInfo.characterInfo[num11].materialReferenceIndex = 0;
						}
						else
						{
							Debug.LogWarning((object)("Unable to use Ellipsis character since it wasn't found in the current Font Asset [" + ((Object)m_fontAsset).name + "]. Consider regenerating this font asset to include the Ellipsis character (u+2026).\nNote: Warnings can be disabled in the TMP Settings file."), (Object)(object)this);
						}
						m_totalCharacterCount = num11 + 1;
						GenerateTextMesh();
						return;
					case TextOverflowModes.Masking:
						if (!m_isMaskingEnabled)
						{
							EnableMasking();
						}
						break;
					case TextOverflowModes.ScrollRect:
						if (!m_isMaskingEnabled)
						{
							EnableMasking();
						}
						break;
					case TextOverflowModes.Truncate:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						m_textInfo.characterInfo[m_characterCount].isVisible = false;
						break;
					}
				}
				if (num4 != 9)
				{
					Color32 vertexColor = ((!m_overrideHtmlColors) ? m_htmlColor : m_fontColor32);
					if (m_textElementType == TMP_TextElementType.Character)
					{
						SaveGlyphVertexInfo(num5, num6, vertexColor);
					}
					else if (m_textElementType == TMP_TextElementType.Sprite)
					{
						SaveSpriteVertexInfo(vertexColor);
					}
				}
				else
				{
					m_textInfo.characterInfo[m_characterCount].isVisible = false;
					m_lastVisibleCharacterOfLine = m_characterCount;
					m_textInfo.lineInfo[m_lineNumber].spaceCount++;
					m_textInfo.spaceCount++;
				}
				if (m_textInfo.characterInfo[m_characterCount].isVisible && num4 != 173)
				{
					if (flag3)
					{
						flag3 = false;
						m_firstVisibleCharacterOfLine = m_characterCount;
					}
					m_lineVisibleCharacterCount++;
					m_lastVisibleCharacterOfLine = m_characterCount;
				}
			}
			else if ((num4 == 10 || char.IsSeparator((char)num4)) && num4 != 173 && num4 != 8203 && num4 != 8288)
			{
				m_textInfo.lineInfo[m_lineNumber].spaceCount++;
				m_textInfo.spaceCount++;
			}
			if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f && !m_isNewPage)
			{
				float num30 = m_maxLineAscender - m_startOfLineAscender;
				AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num30);
				num23 -= num30;
				m_lineOffset += num30;
				m_startOfLineAscender += num30;
				m_SavedWordWrapState.lineOffset = m_lineOffset;
				m_SavedWordWrapState.previousLineAscender = m_startOfLineAscender;
			}
			m_textInfo.characterInfo[m_characterCount].lineNumber = (short)m_lineNumber;
			m_textInfo.characterInfo[m_characterCount].pageNumber = (short)m_pageNumber;
			if ((num4 != 10 && num4 != 13 && num4 != 8230) || m_textInfo.lineInfo[m_lineNumber].characterCount == 1)
			{
				m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;
			}
			if (m_maxAscender - num23 > marginHeight + 0.0001f)
			{
				if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax && m_lineNumber > 0)
				{
					loopCountA = 0;
					m_lineSpacingDelta -= 1f;
					GenerateTextMesh();
					return;
				}
				if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
				{
					m_maxFontSize = m_fontSize;
					m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
					m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
					m_recursiveCount = 0;
					if (loopCountA <= 20)
					{
						GenerateTextMesh();
					}
					return;
				}
				switch (m_overflowMode)
				{
				case TextOverflowModes.Overflow:
					if (m_isMaskingEnabled)
					{
						DisableMasking();
					}
					break;
				case TextOverflowModes.Ellipsis:
					if (m_isMaskingEnabled)
					{
						DisableMasking();
					}
					if (m_lineNumber > 0)
					{
						m_char_buffer[m_textInfo.characterInfo[num11].index] = 8230;
						m_char_buffer[m_textInfo.characterInfo[num11].index + 1] = 0;
						if (m_cached_Ellipsis_GlyphInfo != null)
						{
							m_textInfo.characterInfo[num11].character = '…';
							m_textInfo.characterInfo[num11].textElement = m_cached_Ellipsis_GlyphInfo;
							m_textInfo.characterInfo[num11].fontAsset = m_materialReferences[0].fontAsset;
							m_textInfo.characterInfo[num11].material = m_materialReferences[0].material;
							m_textInfo.characterInfo[num11].materialReferenceIndex = 0;
						}
						else
						{
							Debug.LogWarning((object)("Unable to use Ellipsis character since it wasn't found in the current Font Asset [" + ((Object)m_fontAsset).name + "]. Consider regenerating this font asset to include the Ellipsis character (u+2026).\nNote: Warnings can be disabled in the TMP Settings file."), (Object)(object)this);
						}
						m_totalCharacterCount = num11 + 1;
						GenerateTextMesh();
						m_isTextTruncated = true;
					}
					else
					{
						ClearMesh();
					}
					return;
				case TextOverflowModes.Masking:
					if (!m_isMaskingEnabled)
					{
						EnableMasking();
					}
					break;
				case TextOverflowModes.ScrollRect:
					if (!m_isMaskingEnabled)
					{
						EnableMasking();
					}
					break;
				case TextOverflowModes.Truncate:
					if (m_isMaskingEnabled)
					{
						DisableMasking();
					}
					if (m_lineNumber > 0)
					{
						m_char_buffer[m_textInfo.characterInfo[num11].index + 1] = 0;
						m_totalCharacterCount = num11 + 1;
						GenerateTextMesh();
						m_isTextTruncated = true;
					}
					else
					{
						ClearMesh();
					}
					return;
				case TextOverflowModes.Page:
					if (m_isMaskingEnabled)
					{
						DisableMasking();
					}
					if (num4 == 13 || num4 == 10)
					{
						break;
					}
					i = RestoreWordWrappingState(ref m_SavedLineState);
					if (i == 0)
					{
						ClearMesh();
						return;
					}
					m_isNewPage = true;
					m_xAdvance = tag_Indent;
					m_lineOffset = 0f;
					m_lineNumber++;
					m_pageNumber++;
					continue;
				}
			}
			if (num4 == 9)
			{
				float num31 = m_currentFontAsset.fontInfo.TabWidth * num2;
				float num32 = Mathf.Ceil(m_xAdvance / num31) * num31;
				m_xAdvance = ((!(num32 > m_xAdvance)) ? (m_xAdvance + num31) : num32);
			}
			else if (m_monoSpacing != 0f)
			{
				m_xAdvance += (m_monoSpacing - num19 + (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing) * (1f - m_charWidthAdjDelta);
			}
			else if (!m_isRightToLeft)
			{
				m_xAdvance += ((m_cached_TextElement.xAdvance * num7 + m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing) * (1f - m_charWidthAdjDelta);
			}
			m_textInfo.characterInfo[m_characterCount].xAdvance = m_xAdvance;
			if (num4 == 13)
			{
				m_xAdvance = tag_Indent;
			}
			if (num4 == 10 || m_characterCount == totalCharacterCount - 1)
			{
				if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f && !m_isNewPage)
				{
					float num33 = m_maxLineAscender - m_startOfLineAscender;
					AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num33);
					num23 -= num33;
					m_lineOffset += num33;
				}
				m_isNewPage = false;
				float num34 = m_maxLineAscender - m_lineOffset;
				float num35 = m_maxLineDescender - m_lineOffset;
				m_maxDescender = ((!(m_maxDescender < num35)) ? num35 : m_maxDescender);
				if (!flag4)
				{
					num14 = m_maxDescender;
				}
				if (m_useMaxVisibleDescender && (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines))
				{
					flag4 = true;
				}
				m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstCharacterOfLine;
				m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex = (m_firstVisibleCharacterOfLine = ((m_firstCharacterOfLine <= m_firstVisibleCharacterOfLine) ? m_firstVisibleCharacterOfLine : m_firstCharacterOfLine));
				m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = (m_lastCharacterOfLine = m_characterCount);
				m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = (m_lastVisibleCharacterOfLine = ((m_lastVisibleCharacterOfLine >= m_firstVisibleCharacterOfLine) ? m_lastVisibleCharacterOfLine : m_firstVisibleCharacterOfLine));
				m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;
				m_textInfo.lineInfo[m_lineNumber].visibleCharacterCount = m_lineVisibleCharacterCount;
				m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, num35);
				m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, num34);
				m_textInfo.lineInfo[m_lineNumber].length = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - num5 * num2;
				m_textInfo.lineInfo[m_lineNumber].width = num12;
				if (m_textInfo.lineInfo[m_lineNumber].characterCount == 1)
				{
					m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;
				}
				if (m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].isVisible)
				{
					m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 - m_cSpacing;
				}
				else
				{
					m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastCharacterOfLine].xAdvance - (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 - m_cSpacing;
				}
				m_textInfo.lineInfo[m_lineNumber].baseline = 0f - m_lineOffset;
				m_textInfo.lineInfo[m_lineNumber].ascender = num34;
				m_textInfo.lineInfo[m_lineNumber].descender = num35;
				m_textInfo.lineInfo[m_lineNumber].lineHeight = num34 - num35 + num8 * num;
				m_firstCharacterOfLine = m_characterCount + 1;
				m_lineVisibleCharacterCount = 0;
				if (num4 == 10)
				{
					SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount);
					SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
					m_lineNumber++;
					flag3 = true;
					if (m_lineNumber >= m_textInfo.lineInfo.Length)
					{
						ResizeLineExtents(m_lineNumber);
					}
					if (m_lineHeight == 0f)
					{
						num9 = 0f - m_maxLineDescender + num21 + (num8 + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * num;
						m_lineOffset += num9;
					}
					else
					{
						m_lineOffset += m_lineHeight + (m_lineSpacing + m_paragraphSpacing) * num;
					}
					m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
					m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
					m_startOfLineAscender = num21;
					m_xAdvance = tag_LineIndent + tag_Indent;
					num11 = m_characterCount - 1;
					m_characterCount++;
					continue;
				}
			}
			if (m_textInfo.characterInfo[m_characterCount].isVisible)
			{
				m_meshExtents.min.x = Mathf.Min(m_meshExtents.min.x, m_textInfo.characterInfo[m_characterCount].bottomLeft.x);
				m_meshExtents.min.y = Mathf.Min(m_meshExtents.min.y, m_textInfo.characterInfo[m_characterCount].bottomLeft.y);
				m_meshExtents.max.x = Mathf.Max(m_meshExtents.max.x, m_textInfo.characterInfo[m_characterCount].topRight.x);
				m_meshExtents.max.y = Mathf.Max(m_meshExtents.max.y, m_textInfo.characterInfo[m_characterCount].topRight.y);
			}
			if (m_overflowMode == TextOverflowModes.Page && num4 != 13 && num4 != 10 && m_pageNumber < 16)
			{
				m_textInfo.pageInfo[m_pageNumber].ascender = num13;
				m_textInfo.pageInfo[m_pageNumber].descender = ((!(num22 < m_textInfo.pageInfo[m_pageNumber].descender)) ? m_textInfo.pageInfo[m_pageNumber].descender : num22);
				if (m_pageNumber == 0 && m_characterCount == 0)
				{
					m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
				}
				else if (m_characterCount > 0 && m_pageNumber != m_textInfo.characterInfo[m_characterCount - 1].pageNumber)
				{
					m_textInfo.pageInfo[m_pageNumber - 1].lastCharacterIndex = m_characterCount - 1;
					m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
				}
				else if (m_characterCount == totalCharacterCount - 1)
				{
					m_textInfo.pageInfo[m_pageNumber].lastCharacterIndex = m_characterCount;
				}
			}
			if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
			{
				if ((char.IsWhiteSpace((char)num4) || num4 == 45 || num4 == 173) && !m_isNonBreakingSpace && num4 != 160 && num4 != 8209 && num4 != 8239 && num4 != 8288)
				{
					SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
					m_isCharacterWrappingEnabled = false;
					flag5 = false;
				}
				else if (((num4 > 4352 && num4 < 4607) || (num4 > 11904 && num4 < 40959) || (num4 > 43360 && num4 < 43391) || (num4 > 44032 && num4 < 55295) || (num4 > 63744 && num4 < 64255) || (num4 > 65072 && num4 < 65103) || (num4 > 65280 && num4 < 65519)) && !m_isNonBreakingSpace)
				{
					if (flag5 || flag6 || (!TMP_Settings.linebreakingRules.leadingCharacters.ContainsKey(num4) && m_characterCount < totalCharacterCount - 1 && !TMP_Settings.linebreakingRules.followingCharacters.ContainsKey(m_textInfo.characterInfo[m_characterCount + 1].character)))
					{
						SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
						m_isCharacterWrappingEnabled = false;
						flag5 = false;
					}
				}
				else if (flag5 || m_isCharacterWrappingEnabled || flag6)
				{
					SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
				}
			}
			m_characterCount++;
		}
		num3 = m_maxFontSize - m_minFontSize;
		if (!m_isCharacterWrappingEnabled && m_enableAutoSizing && num3 > 0.051f && m_fontSize < m_fontSizeMax)
		{
			m_minFontSize = m_fontSize;
			m_fontSize += Mathf.Max((m_maxFontSize - m_fontSize) / 2f, 0.05f);
			m_fontSize = (float)(int)(Mathf.Min(m_fontSize, m_fontSizeMax) * 20f + 0.5f) / 20f;
			if (loopCountA <= 20)
			{
				GenerateTextMesh();
			}
			return;
		}
		m_isCharacterWrappingEnabled = false;
		if (m_characterCount == 0)
		{
			ClearMesh();
			TMPro_EventManager.ON_TEXT_CHANGED((Object)(object)this);
			return;
		}
		int index = m_materialReferences[0].referenceCount * 4;
		m_textInfo.meshInfo[0].Clear(uploadChanges: false);
		Vector3 val8 = Vector3.zero;
		Vector3[] rectTransformCorners = m_RectTransformCorners;
		switch (m_textAlignment)
		{
		case TextAlignmentOptions.TopLeft:
		case TextAlignmentOptions.Top:
		case TextAlignmentOptions.TopRight:
		case TextAlignmentOptions.TopJustified:
			val8 = ((m_overflowMode == TextOverflowModes.Page) ? (rectTransformCorners[1] + new Vector3(val.x, 0f - m_textInfo.pageInfo[num10].ascender - val.y, 0f)) : (rectTransformCorners[1] + new Vector3(val.x, 0f - m_maxAscender - val.y, 0f)));
			break;
		case TextAlignmentOptions.Left:
		case TextAlignmentOptions.Center:
		case TextAlignmentOptions.Right:
		case TextAlignmentOptions.Justified:
			val8 = ((m_overflowMode == TextOverflowModes.Page) ? ((rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(val.x, 0f - (m_textInfo.pageInfo[num10].ascender + val.y + m_textInfo.pageInfo[num10].descender - val.w) / 2f, 0f)) : ((rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(val.x, 0f - (m_maxAscender + val.y + num14 - val.w) / 2f, 0f)));
			break;
		case TextAlignmentOptions.BottomLeft:
		case TextAlignmentOptions.Bottom:
		case TextAlignmentOptions.BottomRight:
		case TextAlignmentOptions.BottomJustified:
			val8 = ((m_overflowMode == TextOverflowModes.Page) ? (rectTransformCorners[0] + new Vector3(val.x, 0f - m_textInfo.pageInfo[num10].descender + val.w, 0f)) : (rectTransformCorners[0] + new Vector3(val.x, 0f - num14 + val.w, 0f)));
			break;
		case TextAlignmentOptions.BaselineLeft:
		case TextAlignmentOptions.Baseline:
		case TextAlignmentOptions.BaselineRight:
		case TextAlignmentOptions.BaselineJustified:
			val8 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(val.x, 0f, 0f);
			break;
		case TextAlignmentOptions.MidlineLeft:
		case TextAlignmentOptions.Midline:
		case TextAlignmentOptions.MidlineRight:
		case TextAlignmentOptions.MidlineJustified:
			val8 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(val.x, 0f - (m_meshExtents.max.y + val.y + m_meshExtents.min.y - val.w) / 2f, 0f);
			break;
		case TextAlignmentOptions.CaplineLeft:
		case TextAlignmentOptions.Capline:
		case TextAlignmentOptions.CaplineRight:
		case TextAlignmentOptions.CaplineJustified:
			val8 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(val.x, 0f - (m_maxCapHeight - val.y - val.w) / 2f, 0f);
			break;
		}
		Vector3 val9 = Vector3.zero;
		Vector3 zero5 = Vector3.zero;
		int index_X = 0;
		int index_X2 = 0;
		int num36 = 0;
		int num37 = 0;
		int num38 = 0;
		bool flag7 = false;
		int num39 = 0;
		int num40 = 0;
		bool flag8 = !((Object)(object)m_canvas.worldCamera == (Object)null);
		float num41 = (m_previousLossyScaleY = base.transform.lossyScale.y);
		RenderMode val10 = m_canvas.renderMode;
		float scaleFactor = m_canvas.scaleFactor;
		Color32 underlineColor = Color32.op_Implicit(Color.white);
		Color32 underlineColor2 = Color32.op_Implicit(Color.white);
		float num42 = 0f;
		float num43 = 0f;
		float num44 = 0f;
		float num45 = 0f;
		float num46 = TMP_Text.k_LargePositiveFloat;
		int num47 = 0;
		float num48 = 0f;
		float num49 = 0f;
		float b = 0f;
		TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
		for (int j = 0; j < m_characterCount; j++)
		{
			char character2 = characterInfo[j].character;
			int lineNumber = characterInfo[j].lineNumber;
			TMP_LineInfo tMP_LineInfo = m_textInfo.lineInfo[lineNumber];
			num37 = lineNumber + 1;
			switch (tMP_LineInfo.alignment)
			{
			case TextAlignmentOptions.TopLeft:
			case TextAlignmentOptions.Left:
			case TextAlignmentOptions.BottomLeft:
			case TextAlignmentOptions.BaselineLeft:
			case TextAlignmentOptions.MidlineLeft:
			case TextAlignmentOptions.CaplineLeft:
				if (!m_isRightToLeft)
				{
					((Vector3)(ref val9))._002Ector(tMP_LineInfo.marginLeft, 0f, 0f);
				}
				else
				{
					((Vector3)(ref val9))._002Ector(0f - tMP_LineInfo.maxAdvance, 0f, 0f);
				}
				break;
			case TextAlignmentOptions.Top:
			case TextAlignmentOptions.Center:
			case TextAlignmentOptions.Bottom:
			case TextAlignmentOptions.Baseline:
			case TextAlignmentOptions.Midline:
			case TextAlignmentOptions.Capline:
				((Vector3)(ref val9))._002Ector(tMP_LineInfo.marginLeft + tMP_LineInfo.width / 2f - tMP_LineInfo.maxAdvance / 2f, 0f, 0f);
				break;
			case TextAlignmentOptions.TopRight:
			case TextAlignmentOptions.Right:
			case TextAlignmentOptions.BottomRight:
			case TextAlignmentOptions.BaselineRight:
			case TextAlignmentOptions.MidlineRight:
			case TextAlignmentOptions.CaplineRight:
				if (!m_isRightToLeft)
				{
					((Vector3)(ref val9))._002Ector(tMP_LineInfo.marginLeft + tMP_LineInfo.width - tMP_LineInfo.maxAdvance, 0f, 0f);
				}
				else
				{
					((Vector3)(ref val9))._002Ector(tMP_LineInfo.marginLeft + tMP_LineInfo.width, 0f, 0f);
				}
				break;
			case TextAlignmentOptions.TopJustified:
			case TextAlignmentOptions.Justified:
			case TextAlignmentOptions.BottomJustified:
			case TextAlignmentOptions.BaselineJustified:
			case TextAlignmentOptions.MidlineJustified:
			case TextAlignmentOptions.CaplineJustified:
			{
				if (character2 == '\u00ad' || character2 == '\u200b' || character2 == '\u2060')
				{
					break;
				}
				char character3 = characterInfo[tMP_LineInfo.lastCharacterIndex].character;
				if (!char.IsControl(character3) && lineNumber < m_lineNumber)
				{
					float num50 = (m_isRightToLeft ? (tMP_LineInfo.width + tMP_LineInfo.maxAdvance) : (tMP_LineInfo.width - tMP_LineInfo.maxAdvance));
					float num51 = ((tMP_LineInfo.spaceCount <= 2) ? 1f : m_wordWrappingRatios);
					if (lineNumber != num38 || j == 0)
					{
						if (!m_isRightToLeft)
						{
							((Vector3)(ref val9))._002Ector(tMP_LineInfo.marginLeft, 0f, 0f);
						}
						else
						{
							((Vector3)(ref val9))._002Ector(tMP_LineInfo.marginLeft + tMP_LineInfo.width, 0f, 0f);
						}
					}
					else if (character2 == '\t' || char.IsSeparator(character2))
					{
						int num52 = ((!characterInfo[tMP_LineInfo.lastCharacterIndex].isVisible) ? (tMP_LineInfo.spaceCount - 1) : tMP_LineInfo.spaceCount);
						if (num52 < 1)
						{
							num52 = 1;
						}
						val9 = (m_isRightToLeft ? (val9 - new Vector3(num50 * (1f - num51) / (float)num52, 0f, 0f)) : (val9 + new Vector3(num50 * (1f - num51) / (float)num52, 0f, 0f)));
					}
					else
					{
						val9 = (m_isRightToLeft ? (val9 - new Vector3(num50 * num51 / (float)(tMP_LineInfo.visibleCharacterCount - 1), 0f, 0f)) : (val9 + new Vector3(num50 * num51 / (float)(tMP_LineInfo.visibleCharacterCount - 1), 0f, 0f)));
					}
				}
				else if (!m_isRightToLeft)
				{
					((Vector3)(ref val9))._002Ector(tMP_LineInfo.marginLeft, 0f, 0f);
				}
				else
				{
					((Vector3)(ref val9))._002Ector(tMP_LineInfo.marginLeft + tMP_LineInfo.width, 0f, 0f);
				}
				break;
			}
			}
			zero5 = val8 + val9;
			bool isVisible = characterInfo[j].isVisible;
			if (isVisible)
			{
				TMP_TextElementType elementType = characterInfo[j].elementType;
				switch (elementType)
				{
				case TMP_TextElementType.Character:
				{
					Extents lineExtents = tMP_LineInfo.lineExtents;
					float num53 = m_uvLineOffset * (float)lineNumber % 1f + m_uvOffset.x;
					switch (m_horizontalMapping)
					{
					case TextureMappingOptions.Character:
						characterInfo[j].vertex_BL.uv2.x = m_uvOffset.x;
						characterInfo[j].vertex_TL.uv2.x = m_uvOffset.x;
						characterInfo[j].vertex_TR.uv2.x = 1f + m_uvOffset.x;
						characterInfo[j].vertex_BR.uv2.x = 1f + m_uvOffset.x;
						break;
					case TextureMappingOptions.Line:
						if (m_textAlignment != TextAlignmentOptions.Justified)
						{
							characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num53;
							characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num53;
							characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num53;
							characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num53;
						}
						else
						{
							characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x + val9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num53;
							characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x + val9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num53;
							characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x + val9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num53;
							characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x + val9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num53;
						}
						break;
					case TextureMappingOptions.Paragraph:
						characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x + val9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num53;
						characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x + val9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num53;
						characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x + val9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num53;
						characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x + val9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num53;
						break;
					case TextureMappingOptions.MatchAspect:
					{
						switch (m_verticalMapping)
						{
						case TextureMappingOptions.Character:
							characterInfo[j].vertex_BL.uv2.y = m_uvOffset.y;
							characterInfo[j].vertex_TL.uv2.y = 1f + m_uvOffset.y;
							characterInfo[j].vertex_TR.uv2.y = m_uvOffset.y;
							characterInfo[j].vertex_BR.uv2.y = 1f + m_uvOffset.y;
							break;
						case TextureMappingOptions.Line:
							characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num53;
							characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num53;
							characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
							characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
							break;
						case TextureMappingOptions.Paragraph:
							characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + num53;
							characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + num53;
							characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
							characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
							break;
						case TextureMappingOptions.MatchAspect:
							Debug.Log((object)"ERROR: Cannot Match both Vertical & Horizontal.");
							break;
						}
						float num54 = (1f - (characterInfo[j].vertex_BL.uv2.y + characterInfo[j].vertex_TL.uv2.y) * characterInfo[j].aspectRatio) / 2f;
						characterInfo[j].vertex_BL.uv2.x = characterInfo[j].vertex_BL.uv2.y * characterInfo[j].aspectRatio + num54 + num53;
						characterInfo[j].vertex_TL.uv2.x = characterInfo[j].vertex_BL.uv2.x;
						characterInfo[j].vertex_TR.uv2.x = characterInfo[j].vertex_TL.uv2.y * characterInfo[j].aspectRatio + num54 + num53;
						characterInfo[j].vertex_BR.uv2.x = characterInfo[j].vertex_TR.uv2.x;
						break;
					}
					}
					switch (m_verticalMapping)
					{
					case TextureMappingOptions.Character:
						characterInfo[j].vertex_BL.uv2.y = m_uvOffset.y;
						characterInfo[j].vertex_TL.uv2.y = 1f + m_uvOffset.y;
						characterInfo[j].vertex_TR.uv2.y = 1f + m_uvOffset.y;
						characterInfo[j].vertex_BR.uv2.y = m_uvOffset.y;
						break;
					case TextureMappingOptions.Line:
						characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - tMP_LineInfo.descender) / (tMP_LineInfo.ascender - tMP_LineInfo.descender) + m_uvOffset.y;
						characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - tMP_LineInfo.descender) / (tMP_LineInfo.ascender - tMP_LineInfo.descender) + m_uvOffset.y;
						characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
						characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
						break;
					case TextureMappingOptions.Paragraph:
						characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
						characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
						characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
						characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
						break;
					case TextureMappingOptions.MatchAspect:
					{
						float num55 = (1f - (characterInfo[j].vertex_BL.uv2.x + characterInfo[j].vertex_TR.uv2.x) / characterInfo[j].aspectRatio) / 2f;
						characterInfo[j].vertex_BL.uv2.y = num55 + characterInfo[j].vertex_BL.uv2.x / characterInfo[j].aspectRatio + m_uvOffset.y;
						characterInfo[j].vertex_TL.uv2.y = num55 + characterInfo[j].vertex_TR.uv2.x / characterInfo[j].aspectRatio + m_uvOffset.y;
						characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
						characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
						break;
					}
					}
					num42 = characterInfo[j].scale * (1f - m_charWidthAdjDelta);
					if (!characterInfo[j].isUsingAlternateTypeface && (characterInfo[j].style & FontStyles.Bold) == FontStyles.Bold)
					{
						num42 *= -1f;
					}
					if ((int)val10 != 0)
					{
						if ((int)val10 != 1)
						{
							if ((int)val10 == 2)
							{
								num42 *= num41;
							}
						}
						else
						{
							num42 *= ((!flag8) ? 1f : num41);
						}
					}
					else
					{
						num42 *= num41 / scaleFactor;
					}
					float x = characterInfo[j].vertex_BL.uv2.x;
					float y = characterInfo[j].vertex_BL.uv2.y;
					float x2 = characterInfo[j].vertex_TR.uv2.x;
					float y2 = characterInfo[j].vertex_TR.uv2.y;
					float num56 = Mathf.Floor(x);
					float num57 = Mathf.Floor(y);
					x -= num56;
					x2 -= num56;
					y -= num57;
					y2 -= num57;
					characterInfo[j].vertex_BL.uv2.x = PackUV(x, y);
					characterInfo[j].vertex_BL.uv2.y = num42;
					characterInfo[j].vertex_TL.uv2.x = PackUV(x, y2);
					characterInfo[j].vertex_TL.uv2.y = num42;
					characterInfo[j].vertex_TR.uv2.x = PackUV(x2, y2);
					characterInfo[j].vertex_TR.uv2.y = num42;
					characterInfo[j].vertex_BR.uv2.x = PackUV(x2, y);
					characterInfo[j].vertex_BR.uv2.y = num42;
					break;
				}
				}
				if (j < m_maxVisibleCharacters && lineNumber < m_maxVisibleLines && m_overflowMode != TextOverflowModes.Page)
				{
					ref TMP_Vertex vertex_BL = ref characterInfo[j].vertex_BL;
					vertex_BL.position += zero5;
					ref TMP_Vertex vertex_TL = ref characterInfo[j].vertex_TL;
					vertex_TL.position += zero5;
					ref TMP_Vertex vertex_TR = ref characterInfo[j].vertex_TR;
					vertex_TR.position += zero5;
					ref TMP_Vertex vertex_BR = ref characterInfo[j].vertex_BR;
					vertex_BR.position += zero5;
				}
				else if (j < m_maxVisibleCharacters && lineNumber < m_maxVisibleLines && m_overflowMode == TextOverflowModes.Page && characterInfo[j].pageNumber == num10)
				{
					ref TMP_Vertex vertex_BL2 = ref characterInfo[j].vertex_BL;
					vertex_BL2.position += zero5;
					ref TMP_Vertex vertex_TL2 = ref characterInfo[j].vertex_TL;
					vertex_TL2.position += zero5;
					ref TMP_Vertex vertex_TR2 = ref characterInfo[j].vertex_TR;
					vertex_TR2.position += zero5;
					ref TMP_Vertex vertex_BR2 = ref characterInfo[j].vertex_BR;
					vertex_BR2.position += zero5;
				}
				else
				{
					characterInfo[j].vertex_BL.position = Vector3.zero;
					characterInfo[j].vertex_TL.position = Vector3.zero;
					characterInfo[j].vertex_TR.position = Vector3.zero;
					characterInfo[j].vertex_BR.position = Vector3.zero;
				}
				switch (elementType)
				{
				case TMP_TextElementType.Character:
					FillCharacterVertexBuffers(j, index_X);
					break;
				case TMP_TextElementType.Sprite:
					FillSpriteVertexBuffers(j, index_X2);
					break;
				}
			}
			ref TMP_CharacterInfo reference = ref m_textInfo.characterInfo[j];
			reference.bottomLeft += zero5;
			ref TMP_CharacterInfo reference2 = ref m_textInfo.characterInfo[j];
			reference2.topLeft += zero5;
			ref TMP_CharacterInfo reference3 = ref m_textInfo.characterInfo[j];
			reference3.topRight += zero5;
			ref TMP_CharacterInfo reference4 = ref m_textInfo.characterInfo[j];
			reference4.bottomRight += zero5;
			m_textInfo.characterInfo[j].origin += zero5.x;
			m_textInfo.characterInfo[j].xAdvance += zero5.x;
			m_textInfo.characterInfo[j].ascender += zero5.y;
			m_textInfo.characterInfo[j].descender += zero5.y;
			m_textInfo.characterInfo[j].baseLine += zero5.y;
			if (isVisible)
			{
			}
			if (lineNumber != num38 || j == m_characterCount - 1)
			{
				if (lineNumber != num38)
				{
					m_textInfo.lineInfo[num38].baseline += zero5.y;
					m_textInfo.lineInfo[num38].ascender += zero5.y;
					m_textInfo.lineInfo[num38].descender += zero5.y;
					m_textInfo.lineInfo[num38].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[num38].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[num38].descender);
					m_textInfo.lineInfo[num38].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[num38].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[num38].ascender);
				}
				if (j == m_characterCount - 1)
				{
					m_textInfo.lineInfo[lineNumber].baseline += zero5.y;
					m_textInfo.lineInfo[lineNumber].ascender += zero5.y;
					m_textInfo.lineInfo[lineNumber].descender += zero5.y;
					m_textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[lineNumber].descender);
					m_textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[lineNumber].ascender);
				}
			}
			if (char.IsLetterOrDigit(character2) || character2 == '-' || character2 == '\u00ad' || character2 == '‐' || character2 == '‑')
			{
				if (!flag7)
				{
					flag7 = true;
					num39 = j;
				}
				if (flag7 && j == m_characterCount - 1)
				{
					int num58 = m_textInfo.wordInfo.Length;
					int wordCount = m_textInfo.wordCount;
					if (m_textInfo.wordCount + 1 > num58)
					{
						TMP_TextInfo.Resize(ref m_textInfo.wordInfo, num58 + 1);
					}
					num40 = j;
					m_textInfo.wordInfo[wordCount].firstCharacterIndex = num39;
					m_textInfo.wordInfo[wordCount].lastCharacterIndex = num40;
					m_textInfo.wordInfo[wordCount].characterCount = num40 - num39 + 1;
					m_textInfo.wordInfo[wordCount].textComponent = this;
					num36++;
					m_textInfo.wordCount++;
					m_textInfo.lineInfo[lineNumber].wordCount++;
				}
			}
			else if ((flag7 || (j == 0 && (!char.IsPunctuation(character2) || char.IsWhiteSpace(character2) || j == m_characterCount - 1))) && (j <= 0 || j >= characterInfo.Length - 1 || j >= m_characterCount || (character2 != '\'' && character2 != '’') || !char.IsLetterOrDigit(characterInfo[j - 1].character) || !char.IsLetterOrDigit(characterInfo[j + 1].character)))
			{
				num40 = ((j != m_characterCount - 1 || !char.IsLetterOrDigit(character2)) ? (j - 1) : j);
				flag7 = false;
				int num59 = m_textInfo.wordInfo.Length;
				int wordCount2 = m_textInfo.wordCount;
				if (m_textInfo.wordCount + 1 > num59)
				{
					TMP_TextInfo.Resize(ref m_textInfo.wordInfo, num59 + 1);
				}
				m_textInfo.wordInfo[wordCount2].firstCharacterIndex = num39;
				m_textInfo.wordInfo[wordCount2].lastCharacterIndex = num40;
				m_textInfo.wordInfo[wordCount2].characterCount = num40 - num39 + 1;
				m_textInfo.wordInfo[wordCount2].textComponent = this;
				num36++;
				m_textInfo.wordCount++;
				m_textInfo.lineInfo[lineNumber].wordCount++;
			}
			if ((m_textInfo.characterInfo[j].style & FontStyles.Underline) == FontStyles.Underline)
			{
				bool flag9 = true;
				int pageNumber = m_textInfo.characterInfo[j].pageNumber;
				if (j > m_maxVisibleCharacters || lineNumber > m_maxVisibleLines || (m_overflowMode == TextOverflowModes.Page && pageNumber + 1 != m_pageToDisplay))
				{
					flag9 = false;
				}
				if (!char.IsWhiteSpace(character2))
				{
					num45 = Mathf.Max(num45, m_textInfo.characterInfo[j].scale);
					num46 = Mathf.Min((pageNumber != num47) ? TMP_Text.k_LargePositiveFloat : num46, m_textInfo.characterInfo[j].baseLine + base.font.fontInfo.Underline * num45);
					num47 = pageNumber;
				}
				if (!flag && flag9 && j <= tMP_LineInfo.lastVisibleCharacterIndex && character2 != '\n' && character2 != '\r' && (j != tMP_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character2)))
				{
					flag = true;
					num43 = m_textInfo.characterInfo[j].scale;
					if (num45 == 0f)
					{
						num45 = num43;
					}
					((Vector3)(ref zero))._002Ector(m_textInfo.characterInfo[j].bottomLeft.x, num46, 0f);
					underlineColor = m_textInfo.characterInfo[j].color;
				}
				if (flag && m_characterCount == 1)
				{
					flag = false;
					((Vector3)(ref zero2))._002Ector(m_textInfo.characterInfo[j].topRight.x, num46, 0f);
					num44 = m_textInfo.characterInfo[j].scale;
					DrawUnderlineMesh(zero, zero2, ref index, num43, num44, num45, num42, underlineColor);
					num45 = 0f;
					num46 = TMP_Text.k_LargePositiveFloat;
				}
				else if (flag && (j == tMP_LineInfo.lastCharacterIndex || j >= tMP_LineInfo.lastVisibleCharacterIndex))
				{
					if (char.IsWhiteSpace(character2))
					{
						int lastVisibleCharacterIndex = tMP_LineInfo.lastVisibleCharacterIndex;
						((Vector3)(ref zero2))._002Ector(m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, num46, 0f);
						num44 = m_textInfo.characterInfo[lastVisibleCharacterIndex].scale;
					}
					else
					{
						((Vector3)(ref zero2))._002Ector(m_textInfo.characterInfo[j].topRight.x, num46, 0f);
						num44 = m_textInfo.characterInfo[j].scale;
					}
					flag = false;
					DrawUnderlineMesh(zero, zero2, ref index, num43, num44, num45, num42, underlineColor);
					num45 = 0f;
					num46 = TMP_Text.k_LargePositiveFloat;
				}
				else if (flag && !flag9)
				{
					flag = false;
					((Vector3)(ref zero2))._002Ector(m_textInfo.characterInfo[j - 1].topRight.x, num46, 0f);
					num44 = m_textInfo.characterInfo[j - 1].scale;
					DrawUnderlineMesh(zero, zero2, ref index, num43, num44, num45, num42, underlineColor);
					num45 = 0f;
					num46 = TMP_Text.k_LargePositiveFloat;
				}
			}
			else if (flag)
			{
				flag = false;
				((Vector3)(ref zero2))._002Ector(m_textInfo.characterInfo[j - 1].topRight.x, num46, 0f);
				num44 = m_textInfo.characterInfo[j - 1].scale;
				DrawUnderlineMesh(zero, zero2, ref index, num43, num44, num45, num42, underlineColor);
				num45 = 0f;
				num46 = TMP_Text.k_LargePositiveFloat;
			}
			if ((m_textInfo.characterInfo[j].style & FontStyles.Strikethrough) == FontStyles.Strikethrough)
			{
				bool flag10 = true;
				if (j > m_maxVisibleCharacters || lineNumber > m_maxVisibleLines || (m_overflowMode == TextOverflowModes.Page && m_textInfo.characterInfo[j].pageNumber + 1 != m_pageToDisplay))
				{
					flag10 = false;
				}
				if (!flag2 && flag10 && j <= tMP_LineInfo.lastVisibleCharacterIndex && character2 != '\n' && character2 != '\r' && (j != tMP_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character2)))
				{
					flag2 = true;
					num48 = m_textInfo.characterInfo[j].pointSize;
					num49 = m_textInfo.characterInfo[j].scale;
					((Vector3)(ref zero3))._002Ector(m_textInfo.characterInfo[j].bottomLeft.x, m_textInfo.characterInfo[j].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2.75f * num49, 0f);
					underlineColor2 = m_textInfo.characterInfo[j].color;
					b = m_textInfo.characterInfo[j].baseLine;
				}
				if (flag2 && m_characterCount == 1)
				{
					flag2 = false;
					((Vector3)(ref zero4))._002Ector(m_textInfo.characterInfo[j].topRight.x, m_textInfo.characterInfo[j].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num49, 0f);
					DrawUnderlineMesh(zero3, zero4, ref index, num49, num49, num49, num42, underlineColor2);
				}
				else if (flag2 && j == tMP_LineInfo.lastCharacterIndex)
				{
					if (char.IsWhiteSpace(character2))
					{
						int lastVisibleCharacterIndex2 = tMP_LineInfo.lastVisibleCharacterIndex;
						((Vector3)(ref zero4))._002Ector(m_textInfo.characterInfo[lastVisibleCharacterIndex2].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex2].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num49, 0f);
					}
					else
					{
						((Vector3)(ref zero4))._002Ector(m_textInfo.characterInfo[j].topRight.x, m_textInfo.characterInfo[j].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num49, 0f);
					}
					flag2 = false;
					DrawUnderlineMesh(zero3, zero4, ref index, num49, num49, num49, num42, underlineColor2);
				}
				else if (flag2 && j < m_characterCount && (m_textInfo.characterInfo[j + 1].pointSize != num48 || !TMP_Math.Approximately(m_textInfo.characterInfo[j + 1].baseLine + zero5.y, b)))
				{
					flag2 = false;
					int lastVisibleCharacterIndex3 = tMP_LineInfo.lastVisibleCharacterIndex;
					if (j > lastVisibleCharacterIndex3)
					{
						((Vector3)(ref zero4))._002Ector(m_textInfo.characterInfo[lastVisibleCharacterIndex3].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex3].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num49, 0f);
					}
					else
					{
						((Vector3)(ref zero4))._002Ector(m_textInfo.characterInfo[j].topRight.x, m_textInfo.characterInfo[j].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num49, 0f);
					}
					DrawUnderlineMesh(zero3, zero4, ref index, num49, num49, num49, num42, underlineColor2);
				}
				else if (flag2 && !flag10)
				{
					flag2 = false;
					((Vector3)(ref zero4))._002Ector(m_textInfo.characterInfo[j - 1].topRight.x, m_textInfo.characterInfo[j - 1].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num49, 0f);
					DrawUnderlineMesh(zero3, zero4, ref index, num49, num49, num49, num42, underlineColor2);
				}
			}
			else if (flag2)
			{
				flag2 = false;
				((Vector3)(ref zero4))._002Ector(m_textInfo.characterInfo[j - 1].topRight.x, m_textInfo.characterInfo[j - 1].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * m_fontScale, 0f);
				DrawUnderlineMesh(zero3, zero4, ref index, num49, num49, num49, num42, underlineColor2);
			}
			num38 = lineNumber;
		}
		m_textInfo.characterCount = (short)m_characterCount;
		m_textInfo.spriteCount = m_spriteCount;
		m_textInfo.lineCount = (short)num37;
		m_textInfo.wordCount = ((num36 == 0 || m_characterCount <= 0) ? 1 : ((short)num36));
		m_textInfo.pageCount = m_pageNumber + 1;
		if (m_renderMode == TextRenderFlags.Render)
		{
			m_mesh.MarkDynamic();
			m_mesh.vertices = m_textInfo.meshInfo[0].vertices;
			m_mesh.uv = m_textInfo.meshInfo[0].uvs0;
			m_mesh.uv2 = m_textInfo.meshInfo[0].uvs2;
			m_mesh.colors32 = m_textInfo.meshInfo[0].colors32;
			m_mesh.RecalculateBounds();
			m_canvasRenderer.SetMesh(m_mesh);
			for (int k = 1; k < m_textInfo.materialCount; k++)
			{
				m_textInfo.meshInfo[k].ClearUnusedVertices();
				if (!((Object)(object)m_subTextObjects[k] == (Object)null))
				{
					m_subTextObjects[k].mesh.MarkDynamic();
					m_subTextObjects[k].mesh.vertices = m_textInfo.meshInfo[k].vertices;
					m_subTextObjects[k].mesh.uv = m_textInfo.meshInfo[k].uvs0;
					m_subTextObjects[k].mesh.uv2 = m_textInfo.meshInfo[k].uvs2;
					m_subTextObjects[k].mesh.colors32 = m_textInfo.meshInfo[k].colors32;
					m_subTextObjects[k].mesh.RecalculateBounds();
					m_subTextObjects[k].canvasRenderer.SetMesh(m_subTextObjects[k].mesh);
				}
			}
		}
		TMPro_EventManager.ON_TEXT_CHANGED((Object)(object)this);
	}

	protected override Vector3[] GetTextContainerLocalCorners()
	{
		if ((Object)(object)m_rectTransform == (Object)null)
		{
			m_rectTransform = base.rectTransform;
		}
		m_rectTransform.GetLocalCorners(m_RectTransformCorners);
		return m_RectTransformCorners;
	}

	private void ClearMesh()
	{
		m_canvasRenderer.SetMesh((Mesh)null);
		for (int i = 1; i < m_subTextObjects.Length && (Object)(object)m_subTextObjects[i] != (Object)null; i++)
		{
			m_subTextObjects[i].canvasRenderer.SetMesh((Mesh)null);
		}
	}

	protected override void SetActiveSubMeshes(bool state)
	{
		for (int i = 1; i < m_subTextObjects.Length && (Object)(object)m_subTextObjects[i] != (Object)null; i++)
		{
			if (((Behaviour)m_subTextObjects[i]).enabled != state)
			{
				((Behaviour)m_subTextObjects[i]).enabled = state;
			}
		}
	}

	protected override Bounds GetCompoundBounds()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		Bounds val = m_mesh.bounds;
		Vector2 val2 = Vector2.op_Implicit(((Bounds)(ref val)).min);
		Vector2 val3 = Vector2.op_Implicit(((Bounds)(ref val)).max);
		for (int i = 1; i < m_subTextObjects.Length && (Object)(object)m_subTextObjects[i] != (Object)null; i++)
		{
			Bounds val4 = m_subTextObjects[i].mesh.bounds;
			val2.x = ((!(val2.x < ((Bounds)(ref val4)).min.x)) ? ((Bounds)(ref val4)).min.x : val2.x);
			val2.y = ((!(val2.y < ((Bounds)(ref val4)).min.y)) ? ((Bounds)(ref val4)).min.y : val2.y);
			val3.x = ((!(val3.x > ((Bounds)(ref val4)).max.x)) ? ((Bounds)(ref val4)).max.x : val3.x);
			val3.y = ((!(val3.y > ((Bounds)(ref val4)).max.y)) ? ((Bounds)(ref val4)).max.y : val3.y);
		}
		Vector2 val5 = (val2 + val3) / 2f;
		Vector2 val6 = val3 - val2;
		return new Bounds(Vector2.op_Implicit(val5), Vector2.op_Implicit(val6));
	}

	private void UpdateSDFScale(float lossyScale)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		lossyScale = ((lossyScale != 0f) ? lossyScale : 1f);
		float num = 0f;
		float scaleFactor = m_canvas.scaleFactor;
		num = (((int)m_canvas.renderMode == 0) ? (lossyScale / scaleFactor) : (((int)m_canvas.renderMode != 1) ? lossyScale : ((!((Object)(object)m_canvas.worldCamera != (Object)null)) ? 1f : lossyScale)));
		for (int i = 0; i < m_textInfo.characterCount; i++)
		{
			if (m_textInfo.characterInfo[i].isVisible && m_textInfo.characterInfo[i].elementType == TMP_TextElementType.Character)
			{
				float num2 = num * m_textInfo.characterInfo[i].scale * (1f - m_charWidthAdjDelta);
				if (!m_textInfo.characterInfo[i].isUsingAlternateTypeface && (m_textInfo.characterInfo[i].style & FontStyles.Bold) == FontStyles.Bold)
				{
					num2 *= -1f;
				}
				int materialReferenceIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
				int vertexIndex = m_textInfo.characterInfo[i].vertexIndex;
				m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex].y = num2;
				m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 1].y = num2;
				m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 2].y = num2;
				m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 3].y = num2;
			}
		}
		for (int j = 0; j < m_textInfo.materialCount; j++)
		{
			if (j == 0)
			{
				m_mesh.uv2 = m_textInfo.meshInfo[0].uvs2;
				m_canvasRenderer.SetMesh(m_mesh);
			}
			else
			{
				m_subTextObjects[j].mesh.uv2 = m_textInfo.meshInfo[j].uvs2;
				m_subTextObjects[j].canvasRenderer.SetMesh(m_subTextObjects[j].mesh);
			}
		}
	}

	protected override void AdjustLineOffset(int startIndex, int endIndex, float offset)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, offset, 0f);
		for (int i = startIndex; i <= endIndex; i++)
		{
			ref TMP_CharacterInfo reference = ref m_textInfo.characterInfo[i];
			reference.bottomLeft -= val;
			ref TMP_CharacterInfo reference2 = ref m_textInfo.characterInfo[i];
			reference2.topLeft -= val;
			ref TMP_CharacterInfo reference3 = ref m_textInfo.characterInfo[i];
			reference3.topRight -= val;
			ref TMP_CharacterInfo reference4 = ref m_textInfo.characterInfo[i];
			reference4.bottomRight -= val;
			m_textInfo.characterInfo[i].ascender -= val.y;
			m_textInfo.characterInfo[i].baseLine -= val.y;
			m_textInfo.characterInfo[i].descender -= val.y;
			if (m_textInfo.characterInfo[i].isVisible)
			{
				ref TMP_Vertex vertex_BL = ref m_textInfo.characterInfo[i].vertex_BL;
				vertex_BL.position -= val;
				ref TMP_Vertex vertex_TL = ref m_textInfo.characterInfo[i].vertex_TL;
				vertex_TL.position -= val;
				ref TMP_Vertex vertex_TR = ref m_textInfo.characterInfo[i].vertex_TR;
				vertex_TR.position -= val;
				ref TMP_Vertex vertex_BR = ref m_textInfo.characterInfo[i].vertex_BR;
				vertex_BR.position -= val;
			}
		}
	}
}
