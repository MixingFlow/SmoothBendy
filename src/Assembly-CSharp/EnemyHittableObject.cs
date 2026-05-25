using System;
using System.Collections.Generic;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class EnemyHittableObject : TMGMonoBehaviour, IHittable
{
	[Header("Objects")]
	[SerializeField]
	private GameObject m_Active;

	[SerializeField]
	private GameObject m_Broken;

	[SerializeField]
	private Transform m_AudioProxy;

	private List<Rigidbody> m_BrokenPieces = new List<Rigidbody>();

	private S13Switch m_AudioSwitch;

	public event EventHandler OnDestroyEvent;

	public override void Init()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		base.Init();
		foreach (Transform item in m_Broken.transform)
		{
			Transform val = item;
			Rigidbody component = ((Component)val).GetComponent<Rigidbody>();
			if (Object.op_Implicit((Object)(object)component) && !m_BrokenPieces.Contains(component))
			{
				m_BrokenPieces.Add(component);
			}
		}
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Broken.SetActive(false);
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo.ImpactType != ImpactType.INSTANT_DESTROY)
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)m_AudioProxy))
		{
			m_AudioSwitch = ((Component)m_AudioProxy).GetComponentInChildren<S13Switch>();
		}
		if (Object.op_Implicit((Object)(object)m_AudioSwitch) && Object.op_Implicit((Object)(object)m_Broken))
		{
			((Component)m_AudioSwitch).transform.SetParent(m_Broken.transform);
			((Component)m_AudioSwitch).transform.localPosition = Vector3.zero;
		}
		m_Active.SetActive(false);
		m_Broken.SetActive(true);
		m_Broken.transform.SetParent((Transform)null);
		for (int i = 0; i < m_BrokenPieces.Count; i++)
		{
			m_BrokenPieces[i].AddExplosionForce(20f, ((RaycastHit)(ref hit)).point, 10f, 2f, (ForceMode)1);
		}
		if (Object.op_Implicit((Object)(object)m_AudioSwitch))
		{
			if (m_AudioSwitch.Contains("smash"))
			{
				m_AudioSwitch.Play("smash");
			}
			if (m_AudioSwitch.Contains("impact"))
			{
				m_AudioSwitch.Play("impact");
			}
			if (m_AudioSwitch.Contains("gush"))
			{
				m_AudioSwitch.Play("gush");
			}
		}
		this.OnDestroyEvent.Send(this);
		Dispose();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
