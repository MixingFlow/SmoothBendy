using TMG.Core;
using TMG.UI.Controls;
using TMPro;
using UnityEngine;

public class MenuItemSave : TMGMonoBehaviour
{
	[Header("Button")]
	[SerializeField]
	private BaseUIButton m_Button;

	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI m_SaveLbl;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLbl;

	[Header("Arrows")]
	[SerializeField]
	private GameObject m_Arrows;

	public BaseUIButton Button => m_Button;
}
