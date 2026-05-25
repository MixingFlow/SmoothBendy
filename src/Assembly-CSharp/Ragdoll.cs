using TMG.Core;
using UnityEngine;

public class Ragdoll : TMGMonoBehaviour
{
	[SerializeField]
	private bool m_IsSleep = true;

	private Rigidbody m_Rigidbody;

	private Collider m_Collider;

	private bool m_HasRigidbody => (Object)(object)m_Rigidbody != (Object)null;

	private bool m_HasCollider => (Object)(object)m_Collider != (Object)null;

	public override void Init()
	{
		base.Init();
		m_Rigidbody = ((Component)this).GetComponent<Rigidbody>();
		m_Collider = ((Component)this).GetComponent<Collider>();
	}

	public void Initialize()
	{
		if (m_IsSleep)
		{
			if (m_HasRigidbody)
			{
				m_Rigidbody.isKinematic = true;
				m_Rigidbody.detectCollisions = false;
				m_Rigidbody.Sleep();
			}
			if (m_HasCollider)
			{
				m_Collider.enabled = false;
			}
		}
	}

	public void Activate(float force, Vector3 position, float radius, float upwardsModifier)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (m_HasCollider)
		{
			m_Collider.enabled = true;
		}
		if (m_HasRigidbody)
		{
			m_Rigidbody.velocity = Vector3.zero;
			m_Rigidbody.detectCollisions = true;
			m_Rigidbody.isKinematic = false;
			base.gameObject.AddComponent<GravityModifier>().TriggerKinematic();
			m_Rigidbody.AddExplosionForce(force, position, radius, upwardsModifier, (ForceMode)1);
		}
	}

	protected override void OnDisposed()
	{
		m_Rigidbody = null;
		m_Collider = null;
		base.OnDisposed();
	}
}
