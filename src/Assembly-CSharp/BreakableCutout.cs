using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class BreakableCutout : TMGMonoBehaviour, IHittable
{
	private class BrokenCutout
	{
		public Rigidbody Rigidbody;

		public MeshRenderer Renderer;

		public Vector3 Origin;
	}

	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_StaticCutout;

	[SerializeField]
	private GameObject m_BrokenCutout;

	[Header("MeshRenderer")]
	[SerializeField]
	private MeshRenderer m_StaticRenderer;

	private List<BrokenCutout> m_BrokenCutouts = new List<BrokenCutout>();

	private Collider m_Collider;

	private Collider m_RendererCollider;

	private int m_BrokenCuroutsCount;

	private bool m_IsVisible;

	private bool m_IsBroken;

	public override void Init()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		m_Collider = ((Component)this).GetComponent<Collider>();
		m_RendererCollider = ((Component)m_StaticRenderer).GetComponent<Collider>();
		m_BrokenCutout.SetActive(false);
		foreach (Transform item in m_BrokenCutout.transform)
		{
			Transform val = item;
			Rigidbody component = ((Component)val).GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				m_BrokenCutouts.Add(new BrokenCutout
				{
					Rigidbody = component,
					Renderer = ((Component)val).GetComponent<MeshRenderer>(),
					Origin = val.position
				});
			}
		}
		m_BrokenCuroutsCount = m_BrokenCutouts.Count;
	}

	public void Update()
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsBroken)
		{
			return;
		}
		m_IsVisible = false;
		for (int i = 0; i < m_BrokenCuroutsCount; i++)
		{
			if (((Renderer)m_BrokenCutouts[i].Renderer).isVisible)
			{
				m_IsVisible = true;
				break;
			}
		}
		if (!m_IsVisible && !((Renderer)m_StaticRenderer).isVisible)
		{
			m_BrokenCutout.SetActive(false);
			for (int j = 0; j < m_BrokenCuroutsCount; j++)
			{
				BrokenCutout brokenCutout = m_BrokenCutouts[j];
				brokenCutout.Rigidbody.velocity = Vector3.zero;
				((Component)brokenCutout.Rigidbody).transform.position = brokenCutout.Origin;
			}
			((Component)m_StaticRenderer).gameObject.layer = LayerMask.NameToLayer("Default");
			m_IsBroken = false;
			m_Collider.enabled = true;
			m_RendererCollider.enabled = true;
		}
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo != null && weaponInfo.ImpactType == ImpactType.AXE)
		{
			Break(((RaycastHit)(ref hit)).point);
		}
	}

	public void Break(Vector3 fromPosition)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		m_Collider.enabled = false;
		m_RendererCollider.enabled = false;
		((Component)m_StaticRenderer).gameObject.layer = LayerMask.NameToLayer("Invisible");
		GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/SFX_Bendy_Cutout_Impact_01", base.transform.position);
		m_BrokenCutout.SetActive(true);
		for (int i = 0; i < m_BrokenCuroutsCount; i++)
		{
			m_BrokenCutouts[i].Rigidbody.AddExplosionForce(1500f, fromPosition, 10f, 0.25f);
		}
		m_IsBroken = true;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
