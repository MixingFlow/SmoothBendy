using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class SammyLawrence_Ai : BaseAiController
{
	[SerializeField]
	private Transform m_CameraParent;

	[SerializeField]
	private AnimationClip m_Attack1Clip;

	[SerializeField]
	private AnimationClip m_Attack2Clip;

	[SerializeField]
	private AnimationClip m_NoMaskIdleClip;

	[SerializeField]
	private AnimationClip m_DeathClip;

	[SerializeField]
	private GameObject m_Mask;

	[SerializeField]
	private GameObject m_MaskPhys;

	[SerializeField]
	private Transform m_AudioProxy;

	private S13AnimationSwitch m_AudioSwitch;

	private GameObject m_SammyAttackAudio;

	private AudioClip[] m_BattleClips;

	private AudioObject m_BattleDialogue;

	private float m_WalkPeriod;

	private bool m_ReadyToFight;

	public event EventHandler OnBreakPlank;

	public event EventHandler OnRemoveMask;

	public event EventHandler OnGetAxe;

	public override void InitOnComplete()
	{
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		base.InitOnComplete();
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack1Clip).name, "Attack", 10);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack1Clip).name, "Attack_NoDamage", 20);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack1Clip).name, "Attack", 30);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack1Clip).name, "Attack_NoDamage", 40);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack1Clip).name, "Attack", 50);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack1Clip).name, "Attack_NoDamage", 60);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack2Clip).name, "Attack", 18);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack2Clip).name, "Attack_NoDamage", 32);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack2Clip).name, "Attack", 48);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Attack2Clip).name, "Attack_NoDamage", 62);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_DeathClip).name, "GetAxe", 980);
		m_BattleClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Sammy/Battle");
		m_AudioSwitch = ((Component)m_AudioProxy).GetComponentInChildren<S13AnimationSwitch>();
		if (Object.op_Implicit((Object)(object)m_AudioSwitch))
		{
			foreach (Transform item in ((Component)m_AudioSwitch).transform)
			{
				Transform val = item;
				if (((Object)((Component)val).gameObject).name == "vo_sammy_attack")
				{
					m_SammyAttackAudio = ((Component)val).gameObject;
					m_SammyAttackAudio.SetActive(false);
					break;
				}
			}
		}
		m_PassiveAi = true;
	}

	public string GetFirstAttackAnimName()
	{
		return ((Object)m_Attack1Clip).name;
	}

	public void BreakPlank()
	{
		this.OnBreakPlank.Send(this);
	}

	protected override void T_Activate()
	{
		SetAnimationTrigger("Birth");
		SetPhysicsEnabled(AllowPhysics: false);
		DoWait(m_AwakeAnimationWaitTime, AiThought.MoveToPoint, delegate
		{
			SetPhysicsEnabled(AllowPhysics: true);
		});
	}

	public void EnableAttackAudio()
	{
		m_SammyAttackAudio.SetActive(true);
	}

	protected override void T_MoveToPoint()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		SolvePathMovement(m_MoveToVectorPoint);
		CheckSpeed(m_UseRunForWaypoints);
		SetMoveDirection(base.transform.forward);
		if (Vector3.Distance(base.transform.position, m_MoveToVectorPoint) < m_DistanceToAcceptNodeReached)
		{
			OnMoveToPointReached();
		}
	}

	public void UpdateWaypointList(List<WaypointNode> waypointList)
	{
		m_CurrentWaypointList.Clear();
		m_WaypointIndex = 0;
		m_CurrentWaypointList = waypointList;
	}

	public override void Hit(RaycastHit hit, WeaponInfo weaponInfo)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!m_PassiveAi)
		{
			if (base.CurrentThought == AiThought.MoveToPoint)
			{
				DoWait(0f);
			}
			base.Hit(hit, weaponInfo);
			RemoveMask();
		}
	}

	private void RemoveMask()
	{
		if (m_CurrentHealth == 1 && !m_PassiveAi)
		{
			SetThought(AiThought.Idle);
			SetPassive(IsPassive: true);
			m_IsInvincible = true;
			m_CurrentHealth = 100;
			m_UseRunForWaypoints = true;
			this.OnRemoveMask.Send(this);
			SetAnimatiorMovement(0);
			PlayAnimation(((Object)m_NoMaskIdleClip).name, 3f, AiThought.UseWaypoints);
			m_Mask.SetActive(false);
			m_MaskPhys.SetActive(true);
			m_MaskPhys.transform.SetParent((Transform)null);
		}
	}

	public void SetMoveToVectorPoint(Vector3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_MoveToVectorPoint = point;
	}

	protected override void OnMoveToPointReached()
	{
		DoWait(2f);
		if (!m_ReadyToFight)
		{
			m_ReadyToFight = true;
			m_PassiveAi = false;
		}
	}

	public void SetInactive()
	{
		SetThought(AiThought.Inactive);
		SetAnimatiorMovement(0);
		Debug.Log((object)"INACTIVE");
	}

	public void TriggerDeath()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		m_AnimationController.SetTrigger("Dead");
		((Component)m_AnimationController).transform.localScale = Vector3.one;
		((Collider)m_CharacterController).enabled = false;
		CapsuleCollider component = ((Component)this).GetComponent<CapsuleCollider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			((Collider)component).enabled = false;
		}
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		val.SetParent(m_CameraParent);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(val, Vector3.zero, 0.5f, false), (Ease)1);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(val, Vector3.zero, 0.5f, (RotateMode)0), (Ease)1);
	}

	public void GetAxe()
	{
		this.OnGetAxe.Send(this);
	}

	private AudioClip PlayRandom()
	{
		AudioClip val = m_BattleClips[0];
		m_BattleClips[0] = m_BattleClips[m_BattleClips.Length - 1];
		m_BattleClips[m_BattleClips.Length - 1] = val;
		return val;
	}

	protected override void T_EnterRetreat()
	{
		SetTarget(null);
		SetThought(AiThought.Idle);
	}

	protected override void OnDisposed()
	{
		m_BattleClips = null;
		base.OnDisposed();
	}
}
