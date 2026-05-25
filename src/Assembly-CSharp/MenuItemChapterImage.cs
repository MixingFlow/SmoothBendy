using I2.Loc;
using TMG.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemChapterImage : TMGMonoBehaviour
{
	[SerializeField]
	private Image m_Overlay;

	[SerializeField]
	private Image m_Keys;

	[SerializeField]
	private Localize m_LockLbl;

	public void Init(bool isUnlocked)
	{
		((Behaviour)m_Overlay).enabled = !isUnlocked;
		((Behaviour)m_Keys).enabled = !isUnlocked;
		((Component)m_LockLbl).gameObject.SetActive(!isUnlocked);
		string Translation = "MENU/MENU_LOCK";
		if (LocalizationManager.TryGetTranslation("MENU/MENU_LOCK", out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			((Component)m_LockLbl).GetComponent<TextMeshProUGUI>().text = Translation.ToUpper();
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
