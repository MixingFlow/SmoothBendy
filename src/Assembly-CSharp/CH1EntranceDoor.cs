using TMG.Core;
using UnityEngine;

public class CH1EntranceDoor : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_ActiveDoor;

	[SerializeField]
	private GameObject m_EndingDoor;

	private OcclusionPortal m_Portal;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ActiveDoor.SetActive(true);
		m_EndingDoor.SetActive(false);
		m_Portal = base.gameObject.GetComponent<OcclusionPortal>();
	}

	public void Activate()
	{
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = true;
		}
		m_ActiveDoor.SetActive(false);
		m_EndingDoor.SetActive(true);
	}

	protected override void OnDisposed()
	{
		m_Portal = null;
		base.OnDisposed();
	}
}
