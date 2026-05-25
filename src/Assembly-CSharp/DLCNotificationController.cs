using DG.Tweening;
using TMPro;
using UnityEngine;

public class DLCNotificationController : AbstractGameMenuController
{
	[Header("TextMeshPro")]
	[SerializeField]
	private TextMeshProUGUI m_MessageLbl;

	public override void InitController(object _data)
	{
		base.InitController(_data);
		m_MessageLbl.text = (string)_data;
		m_PlayInDelay = 1f;
		m_PlayoutDelay = 4f;
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		Kill();
	}

	public void KillNow()
	{
		m_PlayoutDelay = 0f;
		ShortcutExtensions.DOKill((Component)(object)m_Visuals, false);
		Kill();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
