using System;
using TMG.Core;
using UnityEngine;

public class CH5BoatClogger : TMGMonoBehaviour, IHittable
{
	[SerializeField]
	private GameObject m_Ink;

	[SerializeField]
	private GameObject m_ParticlePrefab;

	public event EventHandler OnHit;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Ink.SetActive(false);
	}

	public void Activate()
	{
		m_Ink.SetActive(true);
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		m_Ink.SetActive(false);
		GameObject val = Object.Instantiate<GameObject>(m_ParticlePrefab, base.transform.position, base.transform.rotation);
		val.SetActive(true);
		this.OnHit.Send(this);
	}

	public void Reset()
	{
		m_Ink.SetActive(false);
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		base.OnDisposed();
	}
}
