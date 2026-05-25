using TMG.Core;
using UnityEngine;

public class CollapsableCeiling : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_Active;

	[SerializeField]
	private GameObject m_Broken;

	[SerializeField]
	private Transform m_RigidbodyParent;

	private Rigidbody[] m_Rigidbodies;

	public override void Init()
	{
		base.Init();
		m_Broken.SetActive(false);
		m_Rigidbodies = ((Component)m_RigidbodyParent).GetComponentsInChildren<Rigidbody>();
	}

	public void Activate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		m_Active.SetActive(false);
		m_Broken.SetActive(true);
		for (int i = 0; i < m_Rigidbodies.Length; i++)
		{
			m_Rigidbodies[i].AddExplosionForce(35f, base.transform.position + Vector3.up, 20f, 0f, (ForceMode)1);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
