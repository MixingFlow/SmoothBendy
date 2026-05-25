using System;
using S13Audio;
using TMG.Core;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableObject : TMGMonoBehaviour
{
	[Header("Input")]
	[SerializeField]
	private float m_Force = 40f;

	[SerializeField]
	private WeaponInfo m_WeaponInfo;

	[Header("Properties")]
	[SerializeField]
	private LayerMask m_IgnoreLayers;

	[SerializeField]
	private bool m_Sticky;

	[SerializeField]
	private bool m_HitSelfOnColision;

	[SerializeField]
	private bool m_CanHitPlayer;

	private Rigidbody m_RigidBody;

	private RaycastHit m_HitInfo = default(RaycastHit);

	private IHittable m_MyHittable;

	private S13Switch m_AudioSwitch;

	private S13ObjectSimple m_AudioSimple;

	private Vector3 m_ThrowForce;

	protected ThrowWeaponRespawner m_Respawner;

	public WeaponInfo WeaponInfo => m_WeaponInfo;

	public float Force => m_Force;

	public event EventHandler OnHit;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		SetupComponents();
	}

	private void LateUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!m_RigidBody.isKinematic)
		{
			Vector3 velocity = m_RigidBody.velocity;
			if (((Vector3)(ref velocity)).magnitude > 50f)
			{
				base.transform.rotation = Quaternion.LookRotation(m_RigidBody.velocity, Vector3.up);
			}
		}
	}

	public void Initialize(WeaponInfo weaponInfo, Vector3 force, bool isKinematic = false)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		SetupComponents();
		m_WeaponInfo = weaponInfo;
		m_ThrowForce = force;
		m_RigidBody.isKinematic = isKinematic;
	}

	public void Throw()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_RigidBody.velocity = m_ThrowForce;
	}

	public void Reset(Vector3 positoion, Quaternion rotation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.transform.position = positoion;
		base.transform.rotation = rotation;
		m_RigidBody.velocity = Vector3.zero;
	}

	public void SetRespawner(ThrowWeaponRespawner respawner)
	{
		m_Respawner = respawner;
	}

	private void SetupComponents()
	{
		m_RigidBody = ((Component)this).GetComponent<Rigidbody>();
		m_AudioSwitch = ((Component)this).GetComponentInChildren<S13Switch>();
		m_AudioSimple = ((Component)this).GetComponentInChildren<S13ObjectSimple>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		if (LayerMask.op_Implicit(m_IgnoreLayers) == (LayerMask.op_Implicit(m_IgnoreLayers) | (1 << ((Component)collision.transform).gameObject.layer)))
		{
			return;
		}
		Vector3 relativeVelocity = collision.relativeVelocity;
		if (((Vector3)(ref relativeVelocity)).magnitude > 15f && !m_RigidBody.isKinematic)
		{
			m_MyHittable = ((Component)this).GetComponent<IHittable>();
			PlayerController component = ((Component)collision.transform).GetComponent<PlayerController>();
			IHittable component2 = ((Component)collision.transform).GetComponent<IHittable>();
			base.gameObject.layer = LayerMask.NameToLayer("SelfDefault");
			if (m_Sticky)
			{
				m_RigidBody.isKinematic = true;
			}
			Vector3 val = ((ContactPoint)(ref collision.contacts[0])).point - base.transform.position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			Physics.Raycast(base.transform.position, normalized, ref m_HitInfo, 1f);
			DoHitStuff(((RaycastHit)(ref m_HitInfo)).point);
			this.OnHit.Send(this);
			if (m_HitSelfOnColision && m_MyHittable != null)
			{
				m_MyHittable.Hit(m_HitInfo, m_WeaponInfo);
			}
			if (m_CanHitPlayer && Object.op_Implicit((Object)(object)component))
			{
				GameManager.Instance.Player.AddForce((base.transform.forward + Vector3.up * 0.5f) * 10f);
				for (int i = 0; i < m_WeaponInfo.Damage; i++)
				{
					GameManager.Instance.ShowHurtBorder();
				}
			}
			component2?.Hit(m_HitInfo, m_WeaponInfo);
		}
		else
		{
			Vector3 relativeVelocity2 = collision.relativeVelocity;
			if (((Vector3)(ref relativeVelocity2)).magnitude > 0f && !m_RigidBody.isKinematic)
			{
				HitPlaySoundSimple();
			}
		}
	}

	public void HitPlaySound(string id)
	{
		m_AudioSwitch.Play(id);
	}

	public void HitPlaySoundSimple()
	{
		if (!((Object)(object)m_AudioSimple == (Object)null))
		{
			m_AudioSimple.Play();
		}
	}

	public void SendOnHit()
	{
		this.OnHit.Send(this);
	}

	public virtual void DoHitStuff(Vector3 hitPosition)
	{
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		m_AudioSwitch = null;
		m_RigidBody = null;
		base.OnDisposed();
	}
}
