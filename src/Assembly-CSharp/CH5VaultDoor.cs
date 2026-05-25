using TMG.Core;
using UnityEngine;

public class CH5VaultDoor : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_ClosedDoor;

	[SerializeField]
	private GameObject m_SmashedDoor;

	[SerializeField]
	private Transform m_AttackLocation;

	private OcclusionPortal m_OcclusionPortal;

	public Transform AttackLocation => m_AttackLocation;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_SmashedDoor.SetActive(false);
		m_OcclusionPortal = base.gameObject.GetComponent<OcclusionPortal>();
		if (Object.op_Implicit((Object)(object)m_OcclusionPortal))
		{
			m_OcclusionPortal.open = false;
		}
	}

	public void Open()
	{
		ForceOpen();
	}

	public void ForceOpen()
	{
		if (Object.op_Implicit((Object)(object)m_OcclusionPortal))
		{
			m_OcclusionPortal.open = true;
		}
		SmashDoor();
	}

	private void SmashDoor()
	{
		m_ClosedDoor.SetActive(false);
		m_SmashedDoor.SetActive(true);
	}

	protected override void OnDisposed()
	{
		m_OcclusionPortal = null;
		base.OnDisposed();
	}
}
