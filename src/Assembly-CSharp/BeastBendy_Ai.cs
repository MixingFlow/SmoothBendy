using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class BeastBendy_Ai : TMGMonoBehaviour
{
	[Header("AI References")]
	[SerializeField]
	private Transform m_KneeTransform;

	[SerializeField]
	private LayerMask m_ObstructionLayers;

	[SerializeField]
	private Animator m_AnimationController;

	[SerializeField]
	private float m_WalkSpeed = 5f;

	[SerializeField]
	private float m_RunSpeed = 15f;

	[SerializeField]
	private float m_GravityMiultiplier;

	[SerializeField]
	private float m_CameraShakeDistance = 30f;

	[Header("Attack")]
	[SerializeField]
	private WeaponInfo m_FistWeaponInfo;

	[SerializeField]
	private LayerMask m_AttackableLayers;

	[SerializeField]
	private ParticleSystem m_dustCloud;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClip m_Anim_Walk;

	[SerializeField]
	private AnimationClip m_Anim_Charge;

	[SerializeField]
	private AnimationClip m_Anim_Attack1;

	[SerializeField]
	private ParticleSystem[] m_PortalParticles;

	private CharacterController m_CharacterController;

	private float m_TurnSpeed = 5f;

	private float m_AttackDistance = 5f;

	private float m_AttackRadious = 2f;

	private Vector3 m_MoveDir;

	private Vector3 m_GravityPower;

	private Vector3 m_TrackObjectPosition;

	private Transform m_MoveToTargetTransform;

	protected Vector3 m_AvoidanceLastPosition;

	protected float m_AvoidanceCooldownTime;

	protected Action m_WaitCallback;

	private float m_WaitTimer;

	private bool m_HasAttackedPlayer;

	[SerializeField]
	private bool m_IsCharging;

	private bool m_IsIntro;

	public override void Init()
	{
		base.Init();
		m_CharacterController = ((Component)this).GetComponent<CharacterController>();
	}

	public override void InitOnComplete()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		base.InitOnComplete();
		m_MoveToTargetTransform = GameManager.Instance.Player.transform;
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Walk).name, "Stomp", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Walk).name, "Stomp", 30);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Charge).name, "Stomp", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Charge).name, "Stomp", 15);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.01f, (TweenCallback)delegate
		{
			AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Attack1).name, "DoAttack", 25);
		});
	}

	public override void OnEnable()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		m_IsIntro = true;
		for (int i = 0; i < m_PortalParticles.Length; i++)
		{
			ParticleSystem val = m_PortalParticles[i];
			ParticleSystem val2 = Object.Instantiate<ParticleSystem>(m_PortalParticles[i]);
			((Component)val2).transform.position = ((Component)val).transform.position;
			((Component)val2).transform.eulerAngles = ((Component)val).transform.eulerAngles;
			val2.Emit(15);
		}
		base.transform.rotation = FaceDirection(isSmooth: false);
		DoWait(1.5f, delegate
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			m_IsIntro = false;
			m_TrackObjectPosition = m_MoveToTargetTransform.position;
			base.transform.rotation = FaceDirection();
			m_AnimationController.SetTrigger("Roar");
			TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1.9f, (TweenCallback)delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				m_TrackObjectPosition = m_MoveToTargetTransform.position;
				base.transform.rotation = FaceDirection(isSmooth: false);
				m_IsCharging = true;
				m_TurnSpeed = 0.25f;
			});
		});
	}

	public override void OnDisable()
	{
		m_IsIntro = false;
	}

	private void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsIntro)
		{
			m_MoveDir = base.transform.forward * m_WalkSpeed * Time.deltaTime;
			m_AnimationController.SetInteger("MovementMode", 1);
		}
		if (m_WaitTimer <= 0f)
		{
			WalkAndPunch();
			if (m_WaitCallback != null)
			{
				Action waitCallback = m_WaitCallback;
				m_WaitTimer = 0f;
				m_WaitCallback = null;
				waitCallback();
			}
			else if (m_IsCharging)
			{
				if (Physics.CheckSphere(m_KneeTransform.position + base.transform.forward * m_AttackDistance, 1f, LayerMask.GetMask(new string[1] { "Player" })))
				{
					ApplyShake(0.25f);
					if (!m_HasAttackedPlayer)
					{
						AttackTarget(base.transform.forward * m_AttackDistance, m_AttackRadious + 0.5f);
						m_HasAttackedPlayer = true;
					}
				}
				else if (Physics.CheckSphere(m_KneeTransform.position + base.transform.forward * m_AttackDistance, 1f, LayerMask.op_Implicit(m_ObstructionLayers)))
				{
					m_AnimationController.SetTrigger("Dazed");
					m_IsCharging = false;
					if (!m_HasAttackedPlayer)
					{
						AttackTarget(base.transform.forward * m_AttackDistance, m_AttackRadious + 0.5f);
					}
					m_HasAttackedPlayer = false;
					ApplyShake(0.25f);
					DoWait(3.6f);
				}
			}
			else if (!Physics.Linecast(base.transform.position, m_MoveToTargetTransform.position, LayerMask.op_Implicit(m_ObstructionLayers)) && Vector3.Angle(base.transform.forward, m_MoveToTargetTransform.position - base.transform.position) < 15f && m_AvoidanceCooldownTime <= 0f && !m_IsCharging)
			{
				m_TrackObjectPosition = m_MoveToTargetTransform.position;
				base.transform.rotation = FaceDirection();
				m_AnimationController.SetTrigger("Roar");
				DoWait(1.9f, delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					m_TrackObjectPosition = m_MoveToTargetTransform.position;
					base.transform.rotation = FaceDirection(isSmooth: false);
					m_IsCharging = true;
					m_TurnSpeed = 0.25f;
				});
			}
		}
		else
		{
			m_WaitTimer -= Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsCharging)
		{
			m_TurnSpeed -= 0.01f;
		}
		if (m_GravityMiultiplier < 10f)
		{
			m_GravityPower += Vector3.down * m_GravityMiultiplier * Time.deltaTime;
		}
		if (m_CharacterController.isGrounded && m_GravityPower.y <= 0f)
		{
			m_GravityPower.y = -10f;
		}
		if (Object.op_Implicit((Object)(object)m_MoveToTargetTransform))
		{
			if (m_AvoidanceCooldownTime <= 0f && m_WaitTimer <= 0f)
			{
				m_TrackObjectPosition = m_MoveToTargetTransform.position;
			}
			AvoidObstacle(m_MoveToTargetTransform.position);
		}
		if (m_IsIntro || m_WaitTimer <= 0f)
		{
			base.transform.rotation = FaceDirection();
			m_CharacterController.Move(m_MoveDir + m_GravityPower);
			m_MoveDir = Vector3.zero;
		}
	}

	public Quaternion FaceDirection(bool isSmooth = true)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(m_TrackObjectPosition.x, base.transform.position.y, m_TrackObjectPosition.z) - base.transform.position;
		Quaternion val2 = ((!(val == Vector3.zero)) ? Quaternion.LookRotation(val) : Quaternion.identity);
		return (val == Vector3.zero) ? Quaternion.identity : ((!isSmooth) ? val2 : Quaternion.Slerp(base.transform.rotation, val2, m_TurnSpeed * Time.deltaTime));
	}

	private void WalkAndPunch()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsCharging)
		{
			m_MoveDir = base.transform.forward * m_RunSpeed * Time.fixedDeltaTime;
			m_AnimationController.SetInteger("MovementMode", 2);
		}
		else if (Vector3.Distance(m_TrackObjectPosition, base.transform.position) > 3f)
		{
			if (Physics.CheckSphere(((Component)m_KneeTransform).transform.position + Vector3.forward * 3f, 5f, LayerMask.op_Implicit(m_AttackableLayers)))
			{
				m_AnimationController.SetInteger("MovementMode", 0);
				m_AnimationController.SetTrigger("Attack_1");
				m_TrackObjectPosition = m_MoveToTargetTransform.position;
				base.transform.rotation = FaceDirection(isSmooth: false);
				DoWait(3f);
			}
			else
			{
				m_TurnSpeed = 5f;
				m_MoveDir = base.transform.forward * m_WalkSpeed * Time.fixedDeltaTime;
				m_AnimationController.SetInteger("MovementMode", 1);
			}
		}
		else
		{
			m_AnimationController.SetInteger("MovementMode", 0);
		}
	}

	private void DoWait(float waitTime, Action callback = null)
	{
		if (m_WaitCallback == null)
		{
			m_WaitTimer = waitTime;
			m_WaitCallback = callback;
		}
	}

	private void AvoidObstacle(Vector3 targetPosition)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (m_AvoidanceCooldownTime <= 0f)
		{
			m_AvoidanceLastPosition = Vector3.zero;
			bool flag = false;
			if (Physics.CheckSphere(m_KneeTransform.position + base.transform.forward * (m_AttackDistance + 2f), m_AttackRadious, LayerMask.op_Implicit(m_ObstructionLayers), (QueryTriggerInteraction)1))
			{
				flag = true;
			}
			Debug.DrawRay(m_KneeTransform.position, base.transform.forward * m_AttackDistance, Color.white);
			if (flag)
			{
				List<Vector3> list = new List<Vector3>();
				Vector3 val = default(Vector3);
				Vector3 val3 = default(Vector3);
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if ((i != 0 || j != 0) && Mathf.Abs(i) != Mathf.Abs(j))
						{
							Vector3 position = m_KneeTransform.position;
							((Vector3)(ref val))._002Ector((float)i, 0f, (float)j);
							Vector3 val2 = position + ((Vector3)(ref val)).normalized * (m_AttackDistance + 2f);
							if (!Physics.CheckSphere(val2, 0.6f, LayerMask.op_Implicit(m_ObstructionLayers)) && !Physics.Linecast(m_KneeTransform.position, val2, LayerMask.op_Implicit(m_ObstructionLayers)))
							{
								((Vector3)(ref val3))._002Ector((float)i, 0f, (float)j);
								list.Add(val2 + ((Vector3)(ref val3)).normalized * 10f);
								Debug.DrawLine(m_KneeTransform.position, val2, Color.cyan, 1f);
							}
						}
					}
				}
				float num = -1f;
				Vector3 avoidanceLastPosition = Vector3.zero;
				for (int k = 0; k < list.Count; k++)
				{
					float num2 = Vector3.Distance(targetPosition, list[k]);
					if (k == 0 || num2 < num)
					{
						num = num2;
						avoidanceLastPosition = list[k];
					}
				}
				m_AvoidanceLastPosition = avoidanceLastPosition;
				m_AvoidanceCooldownTime = 0.5f;
			}
		}
		else
		{
			m_AvoidanceCooldownTime -= Time.deltaTime;
		}
		if (m_AvoidanceLastPosition != Vector3.zero)
		{
			m_TrackObjectPosition = m_AvoidanceLastPosition;
		}
	}

	public void ApplyShake(float shakePower = 0.5f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		float num = Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) / m_CameraShakeDistance;
		num = 1f - Mathf.Clamp01(num);
		GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.5f, shakePower * num, 15, 90f, false, true), (TweenCallback)delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		});
	}

	public void AttackTarget(Vector3 _AttackPosition, float _AttackRadious = 4f, bool NoDamage = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		Collider[] array = Physics.OverlapSphere(m_KneeTransform.position + _AttackPosition, _AttackRadious);
		RaycastHit hit = default(RaycastHit);
		((RaycastHit)(ref hit)).point = m_KneeTransform.position;
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			IHittable componentInParent = ((Component)array[i]).gameObject.GetComponentInParent<IHittable>();
			if (componentInParent != null)
			{
				Vector3 val = ((Component)array[i]).transform.position - Vector3.up;
				Vector3 val2 = ((Component)array[i]).transform.position - m_KneeTransform.position;
				((RaycastHit)(ref hit)).point = val + ((Vector3)(ref val2)).normalized;
				componentInParent.Hit(hit, m_FistWeaponInfo);
			}
			if (flag || ((Component)array[i]).gameObject.layer != LayerMask.NameToLayer("Player"))
			{
				continue;
			}
			GameManager.Instance.Player.AddForce((-base.transform.right + base.transform.forward + Vector3.up * 0.5f) * 25f);
			ApplyShake(2f);
			flag = true;
			DebugLog("[Ai] - (" + ((Object)base.gameObject).name + ") - Hit Player");
			if (!NoDamage)
			{
				if (!m_IsCharging)
				{
					for (int j = 0; j < m_FistWeaponInfo.Damage; j++)
					{
						GameManager.Instance.ShowHurtBorder(isSilent: true);
					}
				}
				else
				{
					for (int k = 0; k < 5; k++)
					{
						GameManager.Instance.ShowHurtBorder(isSilent: true);
					}
					GameManager.Instance.ShowHurtBorder();
				}
			}
			S13AudioManager.Instance.InvokeEvent("evt_player_hit_by_boris");
		}
	}
}
