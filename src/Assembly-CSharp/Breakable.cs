using System;
using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class Breakable : TMGMonoBehaviour, IHittable
{
	[SerializeField]
	private List<Rigidbody> m_Pieces;

	[SerializeField]
	private List<AudioClip> m_AudioClips;

	[SerializeField]
	private bool m_AxeOnly;

	[SerializeField]
	private bool m_BendyCutout;

	private Collider m_Collider;

	private AudioClip m_BreakClip;

	private int m_HitCount;

	private int m_HitMax = 2;

	public event EventHandler OnBroken;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Collider = ((Component)this).GetComponent<Collider>();
		if (m_AudioClips.Count > 0)
		{
			int index = Random.Range(0, m_AudioClips.Count);
			m_BreakClip = m_AudioClips[index];
		}
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo == null)
		{
			return;
		}
		if (m_AxeOnly && (weaponInfo.ImpactType == ImpactType.AXE || weaponInfo.IsBullet))
		{
			Destroy(((RaycastHit)(ref hit)).point);
		}
		else if (!m_BendyCutout && !m_AxeOnly)
		{
			m_HitCount++;
			if (m_HitCount > -m_HitMax)
			{
				Destroy(((RaycastHit)(ref hit)).point);
			}
		}
	}

	public void Destroy(Vector3 fromPosition)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_Collider))
		{
			m_Collider.enabled = false;
		}
		if (Object.op_Implicit((Object)(object)m_BreakClip))
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_BreakClip, fromPosition);
		}
		SilentDestroy(fromPosition);
	}

	public void SilentDestroy(Vector3 fromPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GameManager.Instance.GameCamera.transform.position - fromPosition;
		Vector3 val2 = fromPosition + ((Vector3)(ref val)).normalized;
		for (int i = 0; i < m_Pieces.Count; i++)
		{
			Rigidbody val3 = m_Pieces[i];
			if (Object.op_Implicit((Object)(object)val3))
			{
				((Component)val3).transform.SetParent(base.transform.parent);
				val3.angularVelocity = Random.insideUnitSphere * Random.Range(-360f, 360f);
				val3.AddExplosionForce(15f, val2, 15f, -0.25f, (ForceMode)1);
			}
		}
		this.OnBroken.Send(this);
		Dispose();
	}

	protected override void OnDisposed()
	{
		this.OnBroken = null;
		base.OnDisposed();
	}
}
