using System;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TMP_SubMesh : MonoBehaviour
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
	private Renderer m_renderer;

	[SerializeField]
	private MeshFilter m_meshFilter;

	private Mesh m_mesh;

	[SerializeField]
	private TextMeshPro m_TextComponent;

	[NonSerialized]
	private bool m_isRegisteredForEvents;

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

	public Material material
	{
		get
		{
			return GetMaterial(m_sharedMaterial);
		}
		set
		{
			if (((Object)m_sharedMaterial).GetInstanceID() != ((Object)value).GetInstanceID())
			{
				m_sharedMaterial = (m_material = value);
				m_padding = GetPaddingForMaterial();
				SetVerticesDirty();
				SetMaterialDirty();
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

	public Renderer renderer
	{
		get
		{
			if ((Object)(object)m_renderer == (Object)null)
			{
				m_renderer = ((Component)this).GetComponent<Renderer>();
			}
			return m_renderer;
		}
	}

	public MeshFilter meshFilter
	{
		get
		{
			if ((Object)(object)m_meshFilter == (Object)null)
			{
				m_meshFilter = ((Component)this).GetComponent<MeshFilter>();
			}
			return m_meshFilter;
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
				meshFilter.mesh = m_mesh;
			}
			return m_mesh;
		}
		set
		{
			m_mesh = value;
		}
	}

	private void OnEnable()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!m_isRegisteredForEvents)
		{
			m_isRegisteredForEvents = true;
		}
		meshFilter.sharedMesh = mesh;
		if ((Object)(object)m_sharedMaterial != (Object)null)
		{
			m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, new Vector4(-10000f, -10000f, 10000f, 10000f));
		}
	}

	private void OnDisable()
	{
		m_meshFilter.sharedMesh = null;
		if ((Object)(object)m_fallbackMaterial != (Object)null)
		{
			TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
			m_fallbackMaterial = null;
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)m_mesh != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)m_mesh);
		}
		if ((Object)(object)m_fallbackMaterial != (Object)null)
		{
			TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
			m_fallbackMaterial = null;
		}
		m_isRegisteredForEvents = false;
	}

	public static TMP_SubMesh AddSubTextObject(TextMeshPro textComponent, MaterialReference materialReference)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("TMP SubMesh [" + ((Object)materialReference.material).name + "]");
		TMP_SubMesh tMP_SubMesh = val.AddComponent<TMP_SubMesh>();
		val.transform.SetParent(textComponent.transform, false);
		val.transform.localPosition = Vector3.zero;
		val.transform.localRotation = Quaternion.identity;
		val.transform.localScale = Vector3.one;
		val.layer = ((Component)textComponent).gameObject.layer;
		tMP_SubMesh.m_meshFilter = val.GetComponent<MeshFilter>();
		tMP_SubMesh.m_TextComponent = textComponent;
		tMP_SubMesh.m_fontAsset = materialReference.fontAsset;
		tMP_SubMesh.m_spriteAsset = materialReference.spriteAsset;
		tMP_SubMesh.m_isDefaultMaterial = materialReference.isDefaultMaterial;
		tMP_SubMesh.SetSharedMaterial(materialReference.material);
		tMP_SubMesh.renderer.sortingLayerID = textComponent.renderer.sortingLayerID;
		tMP_SubMesh.renderer.sortingOrder = textComponent.renderer.sortingOrder;
		return tMP_SubMesh;
	}

	public void DestroySelf()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject, 1f);
	}

	private Material GetMaterial(Material mat)
	{
		if ((Object)(object)m_renderer == (Object)null)
		{
			m_renderer = ((Component)this).GetComponent<Renderer>();
		}
		if ((Object)(object)m_material == (Object)null || ((Object)m_material).GetInstanceID() != ((Object)mat).GetInstanceID())
		{
			m_material = CreateMaterialInstance(mat);
		}
		m_sharedMaterial = m_material;
		m_padding = GetPaddingForMaterial();
		SetVerticesDirty();
		SetMaterialDirty();
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
		if ((Object)(object)m_renderer == (Object)null)
		{
			m_renderer = ((Component)this).GetComponent<Renderer>();
		}
		return m_renderer.sharedMaterial;
	}

	private void SetSharedMaterial(Material mat)
	{
		m_sharedMaterial = mat;
		m_padding = GetPaddingForMaterial();
		SetMaterialDirty();
	}

	public float GetPaddingForMaterial()
	{
		return ShaderUtilities.GetPadding(m_sharedMaterial, m_TextComponent.extraPadding, m_TextComponent.isUsingBold);
	}

	public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
	{
		m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, isExtraPadding, isUsingBold);
	}

	public void SetVerticesDirty()
	{
		if (((Behaviour)this).enabled && (Object)(object)m_TextComponent != (Object)null)
		{
			m_TextComponent.havePropertiesChanged = true;
			((Graphic)m_TextComponent).SetVerticesDirty();
		}
	}

	public void SetMaterialDirty()
	{
		UpdateMaterial();
	}

	protected void UpdateMaterial()
	{
		if ((Object)(object)m_renderer == (Object)null)
		{
			m_renderer = renderer;
		}
		m_renderer.sharedMaterial = m_sharedMaterial;
	}
}
