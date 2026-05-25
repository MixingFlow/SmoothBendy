using TMG.Core;
using TMPro;
using UnityEngine;

public class MenuItemCategory : TMGMonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI m_Category;

	public void Init(Transform parent, string category)
	{
		SetParentAndAlignWithScale(parent);
		m_Category.text = category;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
