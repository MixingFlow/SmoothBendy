using UnityEngine;
using UnityEngine.UI;

namespace TMPro;

public class InlineGraphic : MaskableGraphic
{
	public Texture texture;

	private InlineGraphicManager m_manager;

	private RectTransform m_RectTransform;

	private RectTransform m_ParentRectTransform;

	public override Texture mainTexture
	{
		get
		{
			if ((Object)(object)texture == (Object)null)
			{
				return (Texture)(object)Graphic.s_WhiteTexture;
			}
			return texture;
		}
	}

	protected override void Awake()
	{
		m_manager = ((Component)this).GetComponentInParent<InlineGraphicManager>();
	}

	protected override void OnEnable()
	{
		if ((Object)(object)m_RectTransform == (Object)null)
		{
			m_RectTransform = ((Component)this).gameObject.GetComponent<RectTransform>();
		}
		if ((Object)(object)m_manager != (Object)null && (Object)(object)m_manager.spriteAsset != (Object)null)
		{
			texture = m_manager.spriteAsset.spriteSheet;
		}
	}

	protected override void OnDisable()
	{
		((MaskableGraphic)this).OnDisable();
	}

	protected override void OnTransformParentChanged()
	{
	}

	protected override void OnRectTransformDimensionsChange()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_RectTransform == (Object)null)
		{
			m_RectTransform = ((Component)this).gameObject.GetComponent<RectTransform>();
		}
		if ((Object)(object)m_ParentRectTransform == (Object)null)
		{
			m_ParentRectTransform = ((Component)((Transform)m_RectTransform).parent).GetComponent<RectTransform>();
		}
		if (m_RectTransform.pivot != m_ParentRectTransform.pivot)
		{
			m_RectTransform.pivot = m_ParentRectTransform.pivot;
		}
	}

	public void UpdateMaterial()
	{
		((Graphic)this).UpdateMaterial();
	}

	protected override void UpdateGeometry()
	{
	}
}
