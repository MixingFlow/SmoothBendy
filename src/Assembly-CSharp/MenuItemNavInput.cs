using I2.Loc;
using TMG.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemNavInput : TMGMonoBehaviour
{
	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI m_InputLbl;

	[SerializeField]
	private Localize m_InputDescriptionLbl;

	[Header("Images")]
	[SerializeField]
	private Image m_CircleImg;

	private NavInputDataVO m_DataVO;

	private RectTransform m_RectTransform;

	public RectTransform rectTransform
	{
		get
		{
			if ((Object)(object)m_RectTransform == (Object)null)
			{
				m_RectTransform = ((Component)this).GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public void Init(NavInputDataVO vo)
	{
		m_DataVO = vo;
		m_InputLbl.text = m_DataVO.Input;
		m_InputDescriptionLbl.SetTerm(m_DataVO.Description);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
