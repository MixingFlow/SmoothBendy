using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro;

[ExecuteInEditMode]
public class TMP_SubMeshUI : MaskableGraphic, ITextElement, IClippable, IMaskable, IMaterialModifier
{
	[SerializeField]
	private TMP_FontAsset m_fontAsset;

	[SerializeField]
	private TMP_SpriteAsset m_spriteAsset;

	[SerializeField]
	private Material m_material;

	[SerializeField]
	private Material m_sharedMaterial;

	private Material m_fallbackMaterial;

	private Material m_fallbackSourceMaterial;

	[SerializeField]
	private bool m_isDefaultMaterial;

	[SerializeField]
	private float m_padding;

	[SerializeField]
	private CanvasRenderer m_canvasRenderer;

	private Mesh m_mesh;

	[SerializeField]
	private TextMeshProUGUI m_TextComponent;

	[NonSerialized]
	private bool m_isRegisteredForEvents;

	private bool m_materialDirty;

	[SerializeField]
	private int m_materialReferenceIndex;

	public TMP_FontAsset fontAsset
	{
		get
		{
			return m_fontAsset;
		}
		set
		{
			m_fontAsset = value;
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

	public override Texture mainTexture
	{
		get
		{
			if ((Object)(object)sharedMaterial != (Object)null)
			{
				return sharedMaterial.mainTexture;
			}
			return null;
		}
	}

	public override Material material
	{
		get
		{
			return GetMaterial(m_sharedMaterial);
		}
		set
		{
			if (!((Object)(object)m_sharedMaterial != (Object)null) || ((Object)m_sharedMaterial).GetInstanceID() != ((Object)value).GetInstanceID())
			{
				m_sharedMaterial = (m_material = value);
				m_padding = GetPaddingForMaterial();
				((Graphic)this).SetVerticesDirty();
				((Graphic)this).SetMaterialDirty();
			}
		}
	}

	public Material sharedMaterial
	{
		get
		{
			return m_sharedMaterial;
		}
		set
		{
			SetSharedMaterial(value);
		}
	}

	public Material fallbackMaterial
	{
		get
		{
			return m_fallbackMaterial;
		}
		set
		{
			if (!((Object)(object)m_fallbackMaterial == (Object)(object)value))
			{
				if ((Object)(object)m_fallbackMaterial != (Object)null && (Object)(object)m_fallbackMaterial != (Object)(object)value)
				{
					TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
				}
				m_fallbackMaterial = value;
				TMP_MaterialManager.AddFallbackMaterialReference(m_fallbackMaterial);
				SetSharedMaterial(m_fallbackMaterial);
			}
		}
	}

	public Material fallbackSourceMaterial
	{
		get
		{
			return m_fallbackSourceMaterial;
		}
		set
		{
			m_fallbackSourceMaterial = value;
		}
	}

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

	public bool isDefaultMaterial
	{
		get
		{
			return m_isDefaultMaterial;
		}
		set
		{
			m_isDefaultMaterial = value;
		}
	}

	public float padding
	{
		get
		{
			return m_padding;
		}
		set
		{
			m_padding = value;
		}
	}

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

	public Mesh mesh
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if ((Object)(object)m_mesh == (Object)null)
			{
				m_mesh = new Mesh();
				((Object)m_mesh).hideFlags = (HideFlags)61;
			}
			return m_mesh;
		}
		set
		{
			m_mesh = value;
		}
	}

	public static TMP_SubMeshUI AddSubTextObject(TextMeshProUGUI textComponent, MaterialReference materialReference)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("TMP UI SubObject [" + ((Object)materialReference.material).name + "]");
		val.transform.SetParent(textComponent.transform, false);
		val.layer = ((Component)textComponent).gameObject.layer;
		RectTransform val2 = val.AddComponent<RectTransform>();
		val2.anchorMin = Vector2.zero;
		val2.anchorMax = Vector2.one;
		val2.sizeDelta = Vector2.zero;
		val2.pivot = textComponent.rectTransform.pivot;
		TMP_SubMeshUI tMP_SubMeshUI = val.AddComponent<TMP_SubMeshUI>();
		tMP_SubMeshUI.m_canvasRenderer = tMP_SubMeshUI.canvasRenderer;
		tMP_SubMeshUI.m_TextComponent = textComponent;
		tMP_SubMeshUI.m_materialReferenceIndex = materialReference.index;
		tMP_SubMeshUI.m_fontAsset = materialReference.fontAsset;
		tMP_SubMeshUI.m_spriteAsset = materialReference.spriteAsset;
		tMP_SubMeshUI.m_isDefaultMaterial = materialReference.isDefaultMaterial;
		tMP_SubMeshUI.SetSharedMaterial(materialReference.material);
		return tMP_SubMeshUI;
	}

	protected override void OnEnable()
	{
		if (!m_isRegisteredForEvents)
		{
			m_isRegisteredForEvents = true;
		}
		base.m_ShouldRecalculateStencil = true;
		((MaskableGraphic)this).RecalculateClipping();
		((MaskableGraphic)this).RecalculateMasking();
	}

	protected override void OnDisable()
	{
		TMP_UpdateRegistry.UnRegisterCanvasElementForRebuild((ICanvasElement)(object)this);
		if ((Object)(object)base.m_MaskMaterial != (Object)null)
		{
			TMP_MaterialManager.ReleaseStencilMaterial(base.m_MaskMaterial);
			base.m_MaskMaterial = null;
		}
		if ((Object)(object)m_fallbackMaterial != (Object)null)
		{
			TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
			m_fallbackMaterial = null;
		}
		((MaskableGraphic)this).OnDisable();
	}

	protected override void OnDestroy()
	{
		if ((Object)(object)m_mesh != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)m_mesh);
		}
		if ((Object)(object)base.m_MaskMaterial != (Object)null)
		{
			TMP_MaterialManager.ReleaseStencilMaterial(base.m_MaskMaterial);
		}
		if ((Object)(object)m_fallbackMaterial != (Object)null)
		{
			TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
			m_fallbackMaterial = null;
		}
		m_isRegisteredForEvents = false;
		((MaskableGraphic)this).RecalculateClipping();
	}

	protected override void OnTransformParentChanged()
	{
		if (((UIBehaviour)this).IsActive())
		{
			base.m_ShouldRecalculateStencil = true;
			((MaskableGraphic)this).RecalculateClipping();
			((MaskableGraphic)this).RecalculateMasking();
		}
	}

	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		Material val = baseMaterial;
		if (base.m_ShouldRecalculateStencil)
		{
			base.m_StencilValue = TMP_MaterialManager.GetStencilID(((Component)this).gameObject);
			base.m_ShouldRecalculateStencil = false;
		}
		if (base.m_StencilValue > 0)
		{
			val = TMP_MaterialManager.GetStencilMaterial(baseMaterial, base.m_StencilValue);
			if ((Object)(object)base.m_MaskMaterial != (Object)null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(base.m_MaskMaterial);
			}
			base.m_MaskMaterial = val;
		}
		return val;
	}

	public float GetPaddingForMaterial()
	{
		return ShaderUtilities.GetPadding(m_sharedMaterial, m_TextComponent.extraPadding, m_TextComponent.isUsingBold);
	}

	public float GetPaddingForMaterial(Material mat)
	{
		return ShaderUtilities.GetPadding(mat, m_TextComponent.extraPadding, m_TextComponent.isUsingBold);
	}

	public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
	{
		m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, isExtraPadding, isUsingBold);
	}

	public override void SetAllDirty()
	{
	}

	public override void SetVerticesDirty()
	{
		if (((UIBehaviour)this).IsActive() && (Object)(object)m_TextComponent != (Object)null)
		{
			m_TextComponent.havePropertiesChanged = true;
			((Graphic)m_TextComponent).SetVerticesDirty();
		}
	}

	public override void SetLayoutDirty()
	{
	}

	public override void SetMaterialDirty()
	{
		m_materialDirty = true;
		((Graphic)this).UpdateMaterial();
	}

	public void SetPivotDirty()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (((UIBehaviour)this).IsActive())
		{
			((Graphic)this).rectTransform.pivot = m_TextComponent.rectTransform.pivot;
		}
	}

	protected override void UpdateGeometry()
	{
	}

	public override void Rebuild(CanvasUpdate update)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)update == 3 && m_materialDirty)
		{
			((Graphic)this).UpdateMaterial();
			m_materialDirty = false;
		}
	}

	public void RefreshMaterial()
	{
		((Graphic)this).UpdateMaterial();
	}

	protected override void UpdateMaterial()
	{
		if ((Object)(object)m_canvasRenderer == (Object)null)
		{
			m_canvasRenderer = canvasRenderer;
		}
		m_canvasRenderer.materialCount = 1;
		m_canvasRenderer.SetMaterial(((Graphic)this).materialForRendering, 0);
		m_canvasRenderer.SetTexture(((Graphic)this).mainTexture);
	}

	public override void RecalculateClipping()
	{
		((MaskableGraphic)this).RecalculateClipping();
	}

	public override void RecalculateMasking()
	{
		base.m_ShouldRecalculateStencil = true;
		((Graphic)this).SetMaterialDirty();
	}

	private Material GetMaterial()
	{
		return m_sharedMaterial;
	}

	private Material GetMaterial(Material mat)
	{
		if ((Object)(object)m_material == (Object)null || ((Object)m_material).GetInstanceID() != ((Object)mat).GetInstanceID())
		{
			m_material = CreateMaterialInstance(mat);
		}
		m_sharedMaterial = m_material;
		m_padding = GetPaddingForMaterial();
		((Graphic)this).SetVerticesDirty();
		((Graphic)this).SetMaterialDirty();
		return m_sharedMaterial;
	}

	private Material CreateMaterialInstance(Material source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Material val = new Material(source);
		val.shaderKeywords = source.shaderKeywords;
		((Object)val).name = ((Object)val).name + " (Instance)";
		return val;
	}

	private Material GetSharedMaterial()
	{
		if ((Object)(object)m_canvasRenderer == (Object)null)
		{
			m_canvasRenderer = ((Component)this).GetComponent<CanvasRenderer>();
		}
		return m_canvasRenderer.GetMaterial();
	}

	private void SetSharedMaterial(Material mat)
	{
		m_sharedMaterial = mat;
		((Graphic)this).m_Material = m_sharedMaterial;
		m_padding = GetPaddingForMaterial();
		((Graphic)this).SetMaterialDirty();
	}

	int ITextElement.GetInstanceID()
	{
		return ((Object)this).GetInstanceID();
	}
}
