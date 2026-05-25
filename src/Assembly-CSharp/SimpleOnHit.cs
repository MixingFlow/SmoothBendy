using System;
using TMG.Core;
using UnityEngine;

public class SimpleOnHit : TMGMonoBehaviour
{
	private bool m_IsHit;

	public event EventHandler OnHit;

	public event EventHandler OnHitGeneric;

	private void OnCollisionEnter(Collision collision)
	{
		if (!m_IsHit)
		{
			m_IsHit = true;
			if (Object.op_Implicit((Object)(object)collision.gameObject.GetComponent<Rigidbody>()))
			{
				this.OnHit.Send(this);
			}
			else if (Object.op_Implicit((Object)(object)collision.gameObject.GetComponent<IgnoreObject>()))
			{
				m_IsHit = false;
			}
			else
			{
				this.OnHitGeneric.Send(this);
			}
		}
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		this.OnHitGeneric = null;
		base.OnDisposed();
	}
}
