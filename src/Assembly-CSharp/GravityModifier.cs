using TMG.Core;
using UnityEngine;

public class GravityModifier : TMGMonoBehaviour
{
	[SerializeField]
	private float modifier = 4f;

	[SerializeField]
	private bool m_UseKinematic;

	private Rigidbody m_Rigidbody;

	private float m_KillYValue = -1000f;

	private float m_Timer;

	private float m_TimerMax = 3f;

	private bool m_IsKinematic;

	public override void Init()
	{
		base.Init();
		m_Rigidbody = ((Component)this).GetComponent<Rigidbody>();
		m_Timer = 0f;
	}

	private void Update()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (m_UseKinematic && !m_IsKinematic)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_TimerMax)
			{
				Vector3 velocity = m_Rigidbody.velocity;
				if (((Vector3)(ref velocity)).magnitude <= 0.1f)
				{
					LockRigidbody();
				}
			}
		}
		if (base.transform.position.y < m_KillYValue && !m_IsKinematic)
		{
			LockRigidbody();
		}
	}

	private void LockRigidbody()
	{
		m_Rigidbody.Sleep();
		m_IsKinematic = true;
		m_Rigidbody.isKinematic = m_IsKinematic;
		((Behaviour)this).enabled = false;
	}

	private void FixedUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsKinematic)
		{
			Vector3 velocity = m_Rigidbody.velocity;
			if (((Vector3)(ref velocity)).magnitude > 0.25f)
			{
				m_Rigidbody.AddForce(Physics.gravity * modifier);
			}
		}
	}

	public void TriggerKinematic()
	{
		m_UseKinematic = true;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
