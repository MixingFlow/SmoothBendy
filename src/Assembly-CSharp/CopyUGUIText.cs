using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyUGUIText : MonoBehaviour
{
	[SerializeField]
	private Text m_Text;

	private TextMeshProUGUI m_TMPro;

	private void Awake()
	{
		m_TMPro = ((Component)this).GetComponent<TextMeshProUGUI>();
	}

	private void Start()
	{
		m_TMPro.text = m_Text.text;
	}
}
