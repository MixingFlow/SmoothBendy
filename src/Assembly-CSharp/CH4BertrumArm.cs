using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4BertrumArm : TMGMonoBehaviour, IHittable
{
	private const string ARM_ACTIVE = "Active";

	private const string ARM_IDLE_BLEND = "IdleBlend";

	private const string ARM_IDLE_BLEND_DIRECTION = "IdleBlendDirection";

	private const string ARM_INDEX = "ArmIndex";

	private const string ARM_HIT = "Hit";

	private const string AUDIO_HURT = "hurt";

	[SerializeField]
	private CH4BertrumController m_Controller;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private int m_ArmIndex;

	[SerializeField]
	private float m_IdleBlend;

	[SerializeField]
	private Renderer m_ArmRenderer;

	[SerializeField]
	private List<CH4BertrumNut> m_Bolts;

	[SerializeField]
	private GameObject m_DeadArm;

	[SerializeField]
	private List<ParticleSystem> m_Dead_SteamJets;

	[SerializeField]
	private List<LightBulbController> m_Lights;

	[SerializeField]
	private AnimationClip m_Attack1Clip;

	[SerializeField]
	private AnimationClip m_Attack2Clip;

	[SerializeField]
	private GameObject audioSwitchLocation;

	private bool m_HasAttackedWorkbench;

	private bool m_CanBeVulnerable;

	private bool m_CanBeHit;

	private bool m_IsMoving;

	private Vector3 m_CurrentVelocity;

	private Vector3 m_LastVelocity;

	private List<Collider> m_Colliders = new List<Collider>();

	private S13Switch m_AudioSwitch;

	private AudioClip[] m_AxeHitClips;

	public bool IsActive { get; private set; }

	public event EventHandler OnDestroyed;

	public override void Init()
	{
		base.Init();
		m_Animator.logWarnings = false;
		m_Animator.SetInteger("ArmIndex", m_ArmIndex);
		m_Animator.SetFloat("IdleBlend", m_IdleBlend);
		m_ArmRenderer.material.SetInt("_Highlight", 0);
		m_ArmRenderer.material.SetInt("_Shimmer", 0);
		if (m_ArmIndex == 0)
		{
			AnimationEventUtil.AddAnimationEvent(ref m_Animator, ((Object)m_Attack1Clip).name, "HitWorkbench", 27);
			AnimationEventUtil.AddAnimationEvent(ref m_Animator, ((Object)m_Attack1Clip).name, "Vulnerable", 30);
			AnimationEventUtil.AddAnimationEvent(ref m_Animator, ((Object)m_Attack2Clip).name, "Vulnerable", 30);
		}
		GetAllColliders(base.gameObject);
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AudioSwitch = audioSwitchLocation.GetComponentInChildren<S13Switch>();
		m_AxeHitClips = GameManager.Instance.GetAudioClips("Audio/SFX/Weapons/Axe/Hit/");
		for (int i = 0; i < m_Bolts.Count; i++)
		{
			m_Bolts[i].OnHit += HandleBoltOnHit;
		}
		for (int j = 0; j < m_Lights.Count; j++)
		{
			m_Lights[j].TurnOff();
		}
	}

	private void HandleBoltOnHit(object sender, EventArgs e)
	{
		CH4BertrumNut cH4BertrumNut = sender as CH4BertrumNut;
		cH4BertrumNut.OnHit -= HandleBoltOnHit;
		if (m_Bolts.Contains(cH4BertrumNut))
		{
			m_Bolts.Remove(cH4BertrumNut);
		}
		if (m_Bolts.Count <= 0)
		{
			DoArmDeath();
			this.OnDestroyed.Send(this);
		}
	}

	private void GetAllColliders(GameObject g)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider item in array)
		{
			if (!m_Colliders.Contains(item))
			{
				m_Colliders.Add(item);
			}
		}
		foreach (Transform item2 in g.transform)
		{
			Transform val = item2;
			GetAllColliders(((Component)val).gameObject);
		}
	}

	private void FixedUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Instance.isPaused && IsActive)
		{
			GetBlend();
			m_CurrentVelocity = base.transform.eulerAngles;
			Vector3 val = m_CurrentVelocity - m_LastVelocity;
			m_IsMoving = ((Vector3)(ref val)).magnitude > 0.15f;
			m_LastVelocity = m_CurrentVelocity;
		}
	}

	private void GetBlend()
	{
		if (m_Animator.GetBool("Active"))
		{
			float num = m_Animator.GetFloat("IdleBlend");
			int num2 = m_Animator.GetInteger("IdleBlendDirection");
			if (num >= 1f)
			{
				num2 = -1;
			}
			else if (num <= 0f)
			{
				num2 = 1;
			}
			num += Random.Range(0.001f, 0.01f) * (float)num2;
			m_Animator.SetFloat("IdleBlend", num);
			m_Animator.SetInteger("IdleBlendDirection", num2);
		}
	}

	public void Activate()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		IsActive = true;
		m_Animator.SetBool("Active", true);
		Sequence val = DOTween.Sequence();
		float num = 0.25f;
		for (int i = 0; i < m_Lights.Count; i++)
		{
			LightBulbController lightBulbController = m_Lights[i];
			TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(lightBulbController.TurnOn));
			num += 0.2f;
		}
	}

	public void SetRandomSpeed()
	{
		m_Animator.speed = Random.Range(0.9f, 1.05f);
	}

	public void ResetSpeed()
	{
		m_Animator.speed = 1f;
	}

	public void SetAttackArm(bool active)
	{
		m_Animator.SetBool("AttackingArm", active);
	}

	public void PlaySpinSound()
	{
		m_AudioSwitch.Play("spin");
	}

	public void Attack()
	{
		m_Animator.SetBool("IsHurt", true);
		m_Animator.SetInteger("ArmAttack", 1);
		m_Animator.SetTrigger("Attack");
		m_AudioSwitch.Play("attack1");
	}

	public void PhaseAttack()
	{
		m_Animator.SetBool("IsHurt", false);
		m_CanBeVulnerable = false;
		m_Animator.SetInteger("ArmAttack", 2);
		m_Animator.SetTrigger("Attack");
		m_AudioSwitch.Play("attack2");
	}

	public void AttackWorkbench()
	{
		if (!m_HasAttackedWorkbench && m_ArmIndex == 1)
		{
			m_Animator.SetInteger("ArmAttack", 1);
			m_Animator.SetTrigger("Attack");
			m_AudioSwitch.Play("attack1");
		}
	}

	public void HitWorkbench()
	{
		if (!m_HasAttackedWorkbench && m_ArmIndex == 1)
		{
			m_HasAttackedWorkbench = true;
			m_Controller.DestroyWorkbench();
		}
	}

	public void MakeVulnerable()
	{
		m_CanBeVulnerable = true;
	}

	public void Vulnerable()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		if (!m_CanBeVulnerable || m_CanBeHit)
		{
			return;
		}
		for (int i = 0; i < m_Bolts.Count; i++)
		{
			m_Bolts[i].Enable();
		}
		m_CanBeHit = true;
		m_ArmRenderer.material.SetInt("_Highlight", 1);
		m_AudioSwitch.Play("hurt" + (m_ArmIndex + 1));
		S13AudioManager.Instance.InvokeEvent("evt_bert_arms_tired");
		m_Animator.ResetTrigger("Hit");
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 9f, (TweenCallback)delegate
		{
			if (!base.IsDisposed)
			{
				m_Animator.SetBool("IsHurt", false);
				m_CanBeHit = false;
				m_ArmRenderer.material.SetInt("_Highlight", 0);
				S13AudioManager.Instance.InvokeEvent("evt_bert_arms_restored");
				m_Animator.SetTrigger("Hit");
				for (int j = 0; j < m_Bolts.Count; j++)
				{
					m_Bolts[j].Disable();
				}
			}
		});
	}

	public void Death()
	{
		m_Animator.SetTrigger("Dead");
	}

	private void DoArmDeath()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		m_DeadArm.SetActive(true);
		Rigidbody[] componentsInChildren = m_DeadArm.GetComponentsInChildren<Rigidbody>();
		GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 2f, 2f, 15, 90f, false, true), (TweenCallback)delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		});
		for (int num = 0; num < componentsInChildren.Length; num++)
		{
			((Component)componentsInChildren[num]).transform.SetParent((Transform)null);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		if (m_CanBeHit && m_IsMoving)
		{
			return;
		}
		PlayerController component = collision.gameObject.GetComponent<PlayerController>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			return;
		}
		for (int i = 0; i < m_Colliders.Count; i++)
		{
			m_Colliders[i].enabled = false;
		}
		Vector3 point = ((ContactPoint)(ref collision.contacts[0])).point;
		point.y = component.transform.position.y;
		Vector3 val = component.transform.position - point;
		component.AddForce(((Vector3)(ref val)).normalized * 35f + Vector3.up * 5f);
		GameManager.Instance.ShowHurtBorder();
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 1f, 1f, 15, 90f, false, true), (TweenCallback)delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		});
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
		{
			for (int j = 0; j < m_Colliders.Count; j++)
			{
				m_Colliders[j].enabled = true;
			}
		});
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Bolts.Count; i++)
		{
			CH4BertrumNut cH4BertrumNut = m_Bolts[i];
			if ((Object)(object)cH4BertrumNut.Collider == (Object)(object)((RaycastHit)(ref hit)).collider)
			{
				m_Animator.speed = 1f;
				cH4BertrumNut.Hit(hit, weaponInfo);
				return;
			}
		}
		GameManager.Instance.AudioManager.Play(m_AxeHitClips[Random.Range(0, m_AxeHitClips.Length)]);
	}

	protected override void OnDisposed()
	{
		if (m_Colliders != null)
		{
			m_Colliders.Clear();
			m_Colliders = null;
		}
		m_AxeHitClips = null;
		m_AudioSwitch = null;
		base.OnDisposed();
	}
}
