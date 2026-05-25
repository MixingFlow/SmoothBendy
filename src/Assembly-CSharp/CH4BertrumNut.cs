using System;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4BertrumNut : TMGMonoBehaviour
{
	[SerializeField]
	private Rigidbody m_Bolt;

	[SerializeField]
	private MeshRenderer m_Highlight;

	[SerializeField]
	private GameObject m_InkSplat;

	[SerializeField]
	private ParticleSystem m_InkParticles;

	private GravityModifier m_GravityModifier;

	public Collider Collider { get; private set; }

	public event EventHandler OnHit;

	public override void Init()
	{
		base.Init();
		m_Bolt.isKinematic = true;
		m_GravityModifier = ((Component)m_Bolt).GetComponent<GravityModifier>();
		((Behaviour)m_GravityModifier).enabled = false;
		((Component)m_InkParticles).gameObject.SetActive(false);
		((Component)m_Bolt).GetComponent<Collider>().enabled = false;
		Collider = ((Component)this).GetComponent<Collider>();
		Collider.enabled = false;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_InkSplat.SetActive(false);
		((Renderer)m_Highlight).material.SetFloat("_Highlight", 0f);
	}

	public void Enable()
	{
		Collider.enabled = true;
		((Renderer)m_Highlight).material.SetFloat("_Highlight", 1f);
		((Renderer)m_Highlight).material.SetFloat("_Shimmer", 1f);
	}

	public void Disable()
	{
		Collider.enabled = false;
		((Renderer)m_Highlight).material.SetFloat("_Highlight", 0f);
		((Renderer)m_Highlight).material.SetFloat("_Shimmer", 0f);
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		Collider.enabled = false;
		((Renderer)m_Highlight).material.SetFloat("_Highlight", 0f);
		((Renderer)m_Highlight).material.SetFloat("_Shimmer", 0f);
		((Component)m_InkParticles).gameObject.SetActive(true);
		m_InkParticles.Emit(10);
		m_InkSplat.SetActive(true);
		((Component)m_Bolt).GetComponent<Collider>().enabled = true;
		((Component)m_Bolt).transform.SetParent((Transform)null);
		m_Bolt.isKinematic = false;
		m_Bolt.velocity = -((Component)m_Bolt).transform.right * 25f + Vector3.up;
		m_Bolt.angularVelocity = Random.insideUnitSphere * 25f;
		((Behaviour)m_GravityModifier).enabled = true;
		S13AudioManager.Instance.PlayAudio("sfx_axe_hit_metal");
		this.OnHit.Send(this);
	}

	protected override void OnDisposed()
	{
		DynamicDecals.System.RemoveProjection(m_InkSplat.GetComponent<Decal>());
		m_GravityModifier = null;
		base.OnDisposed();
	}
}
