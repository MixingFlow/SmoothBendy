using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

public class BaseWeapon : TMGMonoBehaviour
{
	[Header("Layer Masks")]
	[SerializeField]
	private LayerMask m_IgnoreLayers;

	[Header("Interaction")]
	[SerializeField]
	private Interactable m_Interaction;

	[Header("Weapon Model")]
	[SerializeField]
	private bool m_IsHoldToAttack;

	[SerializeField]
	protected GameObject m_WeaponModel;

	[SerializeField]
	private float m_WeaponRange;

	[Header("WeaponInfo")]
	[SerializeField]
	protected AudioClip m_EquipSound;

	[SerializeField]
	protected List<AudioClip> m_AttackClips;

	[SerializeField]
	private WeaponInfo m_WeaponInfo;

	[Header("Debug")]
	[SerializeField]
	private bool m_debugHitObject;

	protected Sequence m_AttackSequence;

	protected bool m_CanAttack;

	protected bool m_IsEquipped;

	private bool m_HasAttackedThisSwing;

	public Interactable Interaction => m_Interaction;

	public event EventHandler OnInteract;

	public event EventHandler OnEquipped;

	public WeaponInfo GetWeaponInfo()
	{
		return m_WeaponInfo;
	}

	public override void Init()
	{
		base.Init();
		m_Interaction.OnInteracted += HandleOnInteracted;
	}

	public virtual void Update()
	{
		Attack();
	}

	private void Attack()
	{
		if (m_CanAttack && m_IsEquipped && !GameManager.Instance.Player.isLocked && !GameManager.Instance.isPaused && ((!m_IsHoldToAttack) ? PlayerInput.Attack() : PlayerInput.AttackHold()))
		{
			m_CanAttack = false;
			OnAttack();
		}
	}

	public virtual void OnAttack()
	{
	}

	public void Equip()
	{
		GameManager.Instance.ShowCrosshair();
		GameManager.Instance.Player.EquipWeapon();
		m_WeaponInfo.Attacker = GameManager.Instance.Player.gameObject;
		CleanEquip();
		OnEquip();
	}

	protected virtual void OnEquip()
	{
	}

	private IEnumerator DelayEquip()
	{
		if (base.IsDisposed)
		{
			yield return null;
		}
		yield return (object)new WaitForEndOfFrame();
		if (!base.IsDisposed)
		{
			Equip();
		}
	}

	public void UnEquip()
	{
		m_IsEquipped = false;
		m_CanAttack = false;
		UpdateLayer("Default");
	}

	public void CleanEquip()
	{
		m_IsEquipped = true;
		m_CanAttack = true;
		UpdateLayer("Weapon");
	}

	public void KillInteraction()
	{
		m_Interaction.TurnOfAllEFfects();
		m_Interaction.Dispose();
	}

	private void HandleOnInteracted(object sender, EventArgs e)
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject) && Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			Interaction.ResetInteraction();
			return;
		}
		m_Interaction.OnInteracted -= HandleOnInteracted;
		Interactable interactable = sender as Interactable;
		if (!Object.op_Implicit((Object)(object)interactable))
		{
			return;
		}
		OnInteracted();
		this.OnInteract.Send(this);
		if (!base.IsDisposed)
		{
			Object.Destroy((Object)(object)interactable.gameObject);
			m_Interaction = null;
			if ((Object)(object)m_EquipSound != (Object)null)
			{
				GameManager.Instance.AudioManager.Play(m_EquipSound);
			}
			if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
			{
				GameManager.Instance.Player.InactiveWeapon = GameManager.Instance.Player.WeaponGameObject;
				GameManager.Instance.Player.WeaponGameObject = null;
				GameManager.Instance.Player.InactiveWeapon.SetActive(false);
			}
			GameManager.Instance.Player.WeaponGameObject = base.gameObject;
			base.transform.SetParent(GameManager.Instance.Player.WeaponParent);
			base.transform.localEulerAngles = Vector3.zero;
			base.transform.localPosition = Vector3.zero;
			((MonoBehaviour)this).StartCoroutine(DelayEquip());
		}
	}

	protected virtual void OnInteracted()
	{
	}

	protected virtual void HandleSwingBegin()
	{
		if (m_AttackClips != null && m_AttackClips.Count > 0)
		{
			int index = Random.Range(0, m_AttackClips.Count);
			AudioClip val = m_AttackClips[index];
			GameManager.Instance.AudioManager.Play(val);
			m_AttackClips[index] = m_AttackClips[0];
			m_AttackClips[0] = val;
			m_HasAttackedThisSwing = false;
		}
	}

	protected virtual void HandleSwingHit()
	{
		if (!m_HasAttackedThisSwing)
		{
			CheckHitRaycast();
			m_HasAttackedThisSwing = true;
		}
	}

	protected virtual void HandleSwingEnd()
	{
	}

	protected virtual void HandleSwingComplete()
	{
		m_CanAttack = true;
	}

	public void CheckHitRaycast()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Transform val = GameManager.Instance.GameCamera.transform;
		RaycastHit hitData = default(RaycastHit);
		if (Physics.SphereCast(val.position, 0.7f, val.forward, ref hitData, m_WeaponRange + 1.5f, ~LayerMask.op_Implicit(m_IgnoreLayers)))
		{
			OnRaycastHit(hitData);
			if (m_debugHitObject)
			{
				Debug.Log((object)("Weapon Hit Object: " + ((Object)((RaycastHit)(ref hitData)).transform).name));
			}
		}
	}

	private void OnRaycastHit(RaycastHit _hitData)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = ((Component)((RaycastHit)(ref _hitData)).transform).gameObject;
		IHittable componentInParent = val.GetComponentInParent<IHittable>();
		Debug.Log((object)("OnRaycastHit() " + val), (Object)(object)val);
		componentInParent?.Hit(_hitData, m_WeaponInfo);
	}

	private void UpdateLayer(string layer)
	{
		if (!Object.op_Implicit((Object)(object)m_WeaponModel))
		{
			return;
		}
		m_WeaponModel.layer = LayerMask.NameToLayer(layer);
		Transform[] componentsInChildren = m_WeaponModel.GetComponentsInChildren<Transform>(true);
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((Component)componentsInChildren[i]).gameObject.layer = LayerMask.NameToLayer(layer);
			}
		}
	}

	public void SetDamage(int damage)
	{
		m_WeaponInfo.Damage = damage;
	}

	protected void ResetAttackSequence()
	{
		KillAttackSequence();
		m_AttackSequence = DOTween.Sequence();
	}

	protected void KillAttackSequence()
	{
		if (m_AttackSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_AttackSequence, false);
			m_AttackSequence = null;
		}
	}

	protected void SendOnEquipped()
	{
		this.OnEquipped.Send(this);
	}

	protected override void OnDisposed()
	{
		KillAttackSequence();
		base.OnDisposed();
	}
}
