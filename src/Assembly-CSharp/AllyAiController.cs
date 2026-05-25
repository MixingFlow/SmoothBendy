using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class AllyAiController : BaseAiController
{
	private Collider[] fnt_NearbyEntities = (Collider[])(object)new Collider[10];

	[SerializeField]
	private float m_FollowPlayerBufferDistance = 8f;

	[Header("Head Tracking")]
	[SerializeField]
	private bool m_HasHeadTracking;

	[SerializeField]
	private Transform m_Head;

	[SerializeField]
	private Transform m_HeadLookForward;

	[SerializeField]
	private Vector3 m_HeadOffset;

	private Quaternion m_LastLookRotation;

	private Quaternion SnapRotation;

	private Vector3 SnapPosition;

	private Vector3 m_AnimatedHeadPosition;

	private float m_FollowPlayerBuffer;

	public event EventHandler OnDoorSmashed;

	protected override void Update()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (m_HasHeadTracking)
		{
			m_AnimatedHeadPosition = m_HeadLookForward.forward;
		}
	}

	protected override void LateUpdate()
	{
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		base.LateUpdate();
		if (m_HasHeadTracking)
		{
			Transform val = GameManager.Instance.Player.transform;
			bool flag = Object.op_Implicit((Object)(object)val) && m_AnimationController.GetInteger("MovementMode") < 1;
			float num = 0f;
			if (flag && Object.op_Implicit((Object)(object)val))
			{
				num = Vector3.Angle(base.transform.forward, val.position - base.transform.position);
			}
			if (flag && Object.op_Implicit((Object)(object)val) && Vector3.Distance(val.position, base.transform.position) < 15f && num < 55f)
			{
				Vector3 val2 = val.position - m_Head.position;
				Quaternion val3 = Quaternion.LookRotation(val2) * Quaternion.Euler(m_HeadOffset);
				m_Head.rotation = Quaternion.Slerp(m_LastLookRotation, val3, 5f * Time.deltaTime);
			}
			else
			{
				Quaternion val4 = Quaternion.LookRotation(m_AnimatedHeadPosition) * Quaternion.Euler(m_HeadOffset);
				m_Head.rotation = Quaternion.Slerp(m_LastLookRotation, val4, 5f * Time.deltaTime);
			}
			m_LastLookRotation = m_Head.rotation;
		}
	}

	public override Transform FineNearbyTargets()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, m_VisibilityDistance, fnt_NearbyEntities, LayerMask.op_Implicit(m_TargetableEntityLayers), (QueryTriggerInteraction)1);
		if (num > 0)
		{
			Transform val = null;
			float num2 = -1f;
			for (int i = 0; i < num; i++)
			{
				Transform val2 = ((Component)fnt_NearbyEntities[i]).transform;
				float num3 = Vector3.Distance(base.transform.position, val2.position);
				if (((Component)val2).gameObject.tag != "Dead" && (num2 < 0f || num3 < num2) && (num == 1 || !GameManager.Instance.AiGlobalNetwork.CheckAllyTarget(val2)))
				{
					num2 = num3;
					val = val2;
				}
			}
			if ((Object)(object)val != (Object)null)
			{
				GameManager.Instance.AiGlobalNetwork.CleanAllyTargets();
				GameManager.Instance.AiGlobalNetwork.AddAllyTarget(val);
			}
			return val;
		}
		return null;
	}

	public void SetHeadTracking(bool active)
	{
		m_HasHeadTracking = active;
	}

	protected override void T_Idle()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)base.CurrentTarget) || Vector3.Distance(base.transform.position, base.CurrentTarget.position) > m_AttackDistance)
		{
			SetAnimatiorMovement(0);
		}
		else if (WasInThought(AiThought.Follow) && base.CurrentThought == AiThought.Attack)
		{
			SetAnimatiorMovement(1);
		}
		SetMoveDirection(Vector3.zero);
		m_TrackObjectPosition = base.transform.position + base.transform.forward * 10f;
		if (!m_PassiveAi)
		{
			if ((Object)(object)GameManager.Instance.Player != (Object)null && GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding)
			{
				SetThought(AiThought.Retreat);
			}
			else if (!Object.op_Implicit((Object)(object)base.CurrentTarget))
			{
				SetTarget(TestTargetVisibility());
			}
			else
			{
				SetThought(AiThought.Attack);
			}
		}
	}

	protected override void T_EnterRetreat()
	{
		base.T_EnterRetreat();
		SetThought(AiThought.Idle);
	}

	protected override void T_Attack()
	{
		if ((Object)(object)base.CurrentTarget == (Object)(object)GameManager.Instance.Player.transform)
		{
			SetThought(AiThought.Follow);
		}
		else
		{
			base.T_Attack();
		}
	}

	protected override void T_Follow()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)base.CurrentTarget == (Object)(object)GameManager.Instance.Player.transform)
		{
			Transform val = FineNearbyTargets();
			if ((Object)(object)val != (Object)null)
			{
				base.CurrentTarget = val;
			}
			if (Vector3.Distance(base.transform.position, base.CurrentTarget.position) < m_FollowPlayerBuffer)
			{
				m_FollowPlayerBuffer = m_FollowPlayerBufferDistance + 3f;
				SetThought(AiThought.Idle);
				return;
			}
			m_FollowPlayerBuffer = m_FollowPlayerBufferDistance;
		}
		base.T_Follow();
	}

	public void ForceStartIdle()
	{
		while (m_AnimationSettings.Count > 1)
		{
			m_AnimationSettings.RemoveAt(1);
		}
		SetAnimationTrigger("Birth");
		SetThought(AiThought.Idle);
	}

	public void UpdateWaypointList(List<WaypointNode> waypointList)
	{
		m_CurrentWaypointList.Clear();
		m_WaypointIndex = 0;
		m_CurrentWaypointList = waypointList;
		SetThought(AiThought.UseWaypoints);
	}

	public override void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (base.CurrentThought == AiThought.Die)
		{
			return;
		}
		m_HitPosition = ((RaycastHit)(ref hit)).point;
		if (base.CurrentThought != AiThought.Attack && LayerMask.op_Implicit(m_TargetableEntityLayers) == (LayerMask.op_Implicit(m_TargetableEntityLayers) | (1 << weaponInfo.Attacker.layer)))
		{
			if ((Object)(object)base.CurrentTarget != (Object)null)
			{
				GameManager.Instance.AiGlobalNetwork.RemoveAllyTarget(base.CurrentTarget);
			}
			SetTarget(weaponInfo.Attacker.transform);
			SetThought(AiThought.Attack);
		}
		if (!m_IsInvincible)
		{
			GameObject fromPool = GameManager.Instance.PoolingManager.GetFromPool("GamePlay/Particles/InkHit");
			fromPool.transform.position = ((RaycastHit)(ref hit)).point;
			fromPool.transform.localScale = Vector3.one;
			m_CurrentHealth -= weaponInfo.Damage;
			if (m_CurrentHealth <= 0)
			{
				SetThought(AiThought.Die);
			}
		}
	}

	public override void AttackTarget()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)base.CurrentTarget))
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)base.CurrentTarget.parent))
		{
			CH5FenceDoor component = ((Component)base.CurrentTarget.parent).GetComponent<CH5FenceDoor>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.Open();
				ExitCombat();
				SetTarget(GameManager.Instance.Player.transform);
				return;
			}
			CH5VaultDoor component2 = ((Component)base.CurrentTarget.parent).GetComponent<CH5VaultDoor>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				this.OnDoorSmashed.Send(this);
				component2.Open();
				ExitCombat();
				SetTarget(GameManager.Instance.Player.transform);
				base.gameObject.layer = LayerMask.NameToLayer("Ally");
				return;
			}
		}
		m_TrackObjectPosition = base.CurrentTarget.position;
		base.transform.rotation = FaceDirection(isSmooth: true, 10f);
		Vector3 val = m_KneeLocation.position + base.transform.forward * (m_AttackDistance + 1f);
		Collider[] array = Physics.OverlapCapsule(m_KneeLocation.position, val, 1f, LayerMask.op_Implicit(m_TargetableEntityLayers));
		if (array.Length == 0)
		{
			SetAnimatiorMovement(0);
		}
		else if (((Component)array[0]).gameObject.GetComponent<IHittable>() != null)
		{
			((RaycastHit)(ref m_RaycastHit)).point = val;
			((Component)array[0]).gameObject.GetComponent<IHittable>().Hit(m_RaycastHit, m_WeaponInfo);
		}
	}

	protected override bool TrackingLostEvents(AiThought _repalcementThought)
	{
		if ((Object)(object)base.CurrentTarget == (Object)(object)GameManager.Instance.Player.transform)
		{
			m_CurrentPath = null;
			SetThought(AiThought.Idle);
			return true;
		}
		return base.TrackingLostEvents(_repalcementThought);
	}

	public void ExitCombat()
	{
		m_AnimationController.SetBool("IsNotInCombat", true);
	}

	public void EnterCombat()
	{
		m_AnimationController.SetBool("IsNotInCombat", false);
	}

	public void PlayAnimation(string animation)
	{
		m_AnimationController.Play(animation);
	}

	public void DoSpeak(float _Duration)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		m_AnimationController.SetBool("IsSpeaking", true);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), _Duration, (TweenCallback)delegate
		{
			m_AnimationController.SetBool("IsSpeaking", false);
		});
	}

	public void SetCollider(bool active)
	{
		((Collider)m_CharacterController).enabled = active;
		CapsuleCollider component = ((Component)this).GetComponent<CapsuleCollider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			((Collider)component).enabled = active;
		}
	}

	public void SetTrigger(string trigger)
	{
		SetAnimationTrigger(trigger);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
