using System;
using TMG.Core;
using UnityEngine;

public class CH4FairGameBottle : TMGMonoBehaviour
{
	private Rigidbody m_Rigidbody;

	private Vector3 m_OriginalPosition;

	private Vector3 m_OriginalRotation;

	public bool IsHit { get; private set; }

	public event EventHandler OnHit;

	public override void Init()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_Rigidbody = ((Component)this).GetComponent<Rigidbody>();
		m_OriginalPosition = base.transform.position;
		m_OriginalRotation = base.transform.eulerAngles;
		IsHit = false;
	}

	public void Reset()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		IsHit = false;
		m_Rigidbody.velocity = Vector3.zero;
		base.transform.position = m_OriginalPosition;
		base.transform.eulerAngles = m_OriginalRotation;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (((Component)col).CompareTag("FairGame") && !IsHit)
		{
			IsHit = true;
			this.OnHit.Send(this);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
