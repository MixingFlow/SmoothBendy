using TMG.Core;
using UnityEngine;

namespace TMG.UI;

public class UILayer : TMGMonoBehaviour
{
	public bool isActive;

	private RectTransform m_RectTransform;

	public string Name { get; private set; }

	public float Order { get; private set; }

	public static UILayer Create(string layerName, float layerOrder, Transform layerParent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		GameObject val = new GameObject();
		((Object)val).name = "[UI Layer] - " + layerName;
		UILayer uILayer = val.AddComponent<UILayer>();
		uILayer.Name = layerName;
		uILayer.Order = layerOrder;
		uILayer.Init(layerParent);
		return uILayer;
	}

	public void Init(Transform parent)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		m_RectTransform = base.gameObject.AddComponent<RectTransform>();
		((Transform)m_RectTransform).SetParent(parent);
		((Transform)m_RectTransform).localPosition = new Vector3(0f, 0f, Order);
		((Transform)m_RectTransform).localEulerAngles = Vector3.zero;
		((Transform)m_RectTransform).localScale = Vector3.one;
		m_RectTransform.pivot = new Vector2(0.5f, 0.5f);
		m_RectTransform.anchorMax = new Vector2(1f, 1f);
		m_RectTransform.anchorMin = new Vector2(0f, 0f);
		m_RectTransform.offsetMax = new Vector2(0f, 0f);
		m_RectTransform.offsetMin = new Vector2(0f, 0f);
	}
}
