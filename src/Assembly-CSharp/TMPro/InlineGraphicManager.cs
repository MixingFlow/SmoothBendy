using System;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro;

[ExecuteInEditMode]
public class InlineGraphicManager : MonoBehaviour
{
	[SerializeField]
	private TMP_SpriteAsset m_spriteAsset;

	[SerializeField]
	[HideInInspector]
	private InlineGraphic m_inlineGraphic;

	[SerializeField]
	[HideInInspector]
	private CanvasRenderer m_inlineGraphicCanvasRenderer;

	private UIVertex[] m_uiVertex;

	private RectTransform m_inlineGraphicRectTransform;

	private TMP_Text m_textComponent;

	private bool m_isInitialized;

	public TMP_SpriteAsset spriteAsset
	{
		get
		{
			return m_spriteAsset;
		}
		set
		{
			LoadSpriteAsset(value);
		}
	}

	public InlineGraphic inlineGraphic
	{
		get
		{
			return m_inlineGraphic;
		}
		set
		{
			if ((Object)(object)m_inlineGraphic != (Object)(object)value)
			{
				m_inlineGraphic = value;
			}
		}
	}

	public CanvasRenderer canvasRenderer => m_inlineGraphicCanvasRenderer;

	public UIVertex[] uiVertex => m_uiVertex;

	private void Awake()
	{
		if (!TMP_Settings.warningsDisabled)
		{
			Debug.LogWarning((object)("InlineGraphicManager component is now Obsolete and has been removed from [" + ((Object)((Component)this).gameObject).name + "] along with its InlineGraphic child."), (Object)(object)this);
		}
		if ((Object)(object)((Component)inlineGraphic).gameObject != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)((Component)inlineGraphic).gameObject);
			inlineGraphic = null;
		}
		Object.DestroyImmediate((Object)(object)this);
	}

	private void OnEnable()
	{
		((Behaviour)this).enabled = false;
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
	}

	private void LoadSpriteAsset(TMP_SpriteAsset spriteAsset)
	{
		if ((Object)(object)spriteAsset == (Object)null)
		{
			spriteAsset = ((!((Object)(object)TMP_Settings.defaultSpriteAsset != (Object)null)) ? (Resources.Load("Sprite Assets/Default Sprite Asset") as TMP_SpriteAsset) : TMP_Settings.defaultSpriteAsset);
		}
		m_spriteAsset = spriteAsset;
		m_inlineGraphic.texture = m_spriteAsset.spriteSheet;
		if ((Object)(object)m_textComponent != (Object)null && m_isInitialized)
		{
			m_textComponent.havePropertiesChanged = true;
			((Graphic)m_textComponent).SetVerticesDirty();
		}
	}

	public void AddInlineGraphicsChild()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)m_inlineGraphic != (Object)null))
		{
			GameObject val = new GameObject("Inline Graphic");
			m_inlineGraphic = val.AddComponent<InlineGraphic>();
			m_inlineGraphicRectTransform = val.GetComponent<RectTransform>();
			m_inlineGraphicCanvasRenderer = val.GetComponent<CanvasRenderer>();
			((Transform)m_inlineGraphicRectTransform).SetParent(((Component)this).transform, false);
			((Transform)m_inlineGraphicRectTransform).localPosition = Vector3.zero;
			m_inlineGraphicRectTransform.anchoredPosition3D = Vector3.zero;
			m_inlineGraphicRectTransform.sizeDelta = Vector2.zero;
			m_inlineGraphicRectTransform.anchorMin = Vector2.zero;
			m_inlineGraphicRectTransform.anchorMax = Vector2.one;
			m_textComponent = ((Component)this).GetComponent<TMP_Text>();
		}
	}

	public void AllocatedVertexBuffers(int size)
	{
		if ((Object)(object)m_inlineGraphic == (Object)null)
		{
			AddInlineGraphicsChild();
			LoadSpriteAsset(m_spriteAsset);
		}
		if (m_uiVertex == null)
		{
			m_uiVertex = (UIVertex[])(object)new UIVertex[4];
		}
		int num = size * 4;
		if (num > m_uiVertex.Length)
		{
			m_uiVertex = (UIVertex[])(object)new UIVertex[Mathf.NextPowerOfTwo(num)];
		}
	}

	public void UpdatePivot(Vector2 pivot)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_inlineGraphicRectTransform == (Object)null)
		{
			m_inlineGraphicRectTransform = ((Component)m_inlineGraphic).GetComponent<RectTransform>();
		}
		m_inlineGraphicRectTransform.pivot = pivot;
	}

	public void ClearUIVertex()
	{
		if (uiVertex != null && uiVertex.Length > 0)
		{
			Array.Clear(uiVertex, 0, uiVertex.Length);
			m_inlineGraphicCanvasRenderer.Clear();
		}
	}

	public void DrawSprite(UIVertex[] uiVertices, int spriteCount)
	{
		if ((Object)(object)m_inlineGraphicCanvasRenderer == (Object)null)
		{
			m_inlineGraphicCanvasRenderer = ((Component)m_inlineGraphic).GetComponent<CanvasRenderer>();
		}
		m_inlineGraphicCanvasRenderer.SetVertices(uiVertices, spriteCount * 4);
		m_inlineGraphic.UpdateMaterial();
	}

	public TMP_Sprite GetSprite(int index)
	{
		if ((Object)(object)m_spriteAsset == (Object)null)
		{
			Debug.LogWarning((object)"No Sprite Asset is assigned.", (Object)(object)this);
			return null;
		}
		if (m_spriteAsset.spriteInfoList == null || index > m_spriteAsset.spriteInfoList.Count - 1)
		{
			Debug.LogWarning((object)"Sprite index exceeds the number of sprites in this Sprite Asset.", (Object)(object)this);
			return null;
		}
		return m_spriteAsset.spriteInfoList[index];
	}

	public int GetSpriteIndexByHashCode(int hashCode)
	{
		if ((Object)(object)m_spriteAsset == (Object)null || m_spriteAsset.spriteInfoList == null)
		{
			Debug.LogWarning((object)"No Sprite Asset is assigned.", (Object)(object)this);
			return -1;
		}
		return m_spriteAsset.spriteInfoList.FindIndex((TMP_Sprite item) => item.hashCode == hashCode);
	}

	public int GetSpriteIndexByIndex(int index)
	{
		if ((Object)(object)m_spriteAsset == (Object)null || m_spriteAsset.spriteInfoList == null)
		{
			Debug.LogWarning((object)"No Sprite Asset is assigned.", (Object)(object)this);
			return -1;
		}
		return m_spriteAsset.spriteInfoList.FindIndex((TMP_Sprite item) => item.id == index);
	}

	public void SetUIVertex(UIVertex[] uiVertex)
	{
		m_uiVertex = uiVertex;
	}
}
