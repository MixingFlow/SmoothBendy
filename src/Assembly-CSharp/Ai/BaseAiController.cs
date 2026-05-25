using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ai;

public class BaseAiController : AiThoughtMachine, IHittable
{
	[Serializable]
	public class InkDeathEffect
	{
		public InkExplosionEffect InkExplosion;

		public Renderer Renderer;
	}

	[Serializable]
	public class AnimationSettings
	{
		public AnimationClip Clip;

		public int Frame;
	}

	[Header("Layer Options")]
	[SerializeField]
	protected LayerMask m_TargetableEntityLayers;

	[SerializeField]
	protected LayerMask m_ObstructionLayers;

	[Header("Animation Options")]
	[SerializeField]
	protected Animator m_AnimationController;

	[SerializeField]
	protected float m_AwakeAnimationWaitTime;

	[Header("Movement Options")]
	[SerializeField]
	protected bool m_PassiveAi;

	[SerializeField]
	protected AiThought m_StartingThought;

	[SerializeField]
	protected float m_WalkSpeed = 3f;

	[SerializeField]
	protected float m_WalkTurnSpeed = 3f;

	[SerializeField]
	protected float m_RunSpeed = 6f;

	[SerializeField]
	protected float m_RunTurnSpeed = 3f;

	[Header("Gravity Options")]
	[SerializeField]
	protected bool m_EnableGravity = true;

	[SerializeField]
	protected float m_GravityMultiplier = 3f;

	[Header("Avoidance Options")]
	[SerializeField]
	private bool m_EnableAvoidance = true;

	[Header("Tracking Options")]
	[SerializeField]
	protected Transform m_EyeLocation;

	[SerializeField]
	protected Transform m_KneeLocation;

	[SerializeField]
	protected float m_ActivationDistance = 10f;

	[SerializeField]
	protected float m_VisibilityDistance = 50f;

	[SerializeField]
	[Range(0f, 180f)]
	protected float m_TargetVisibilityAngle = 80f;

	[SerializeField]
	protected float m_LoseTrackingVisibilityDistance = 200f;

	[SerializeField]
	protected float m_RunDistance = 15f;

	[SerializeField]
	protected float m_UseNodePathDistance = 15f;

	[SerializeField]
	protected bool m_EnableSpottedAnimation;

	[SerializeField]
	protected float m_SpottedAnimationWaitTime;

	[Header("Health Options")]
	[SerializeField]
	protected bool m_IsInvincible;

	[SerializeField]
	protected int m_Health;

	[Header("Attack Options")]
	[SerializeField]
	protected float m_AttackDistance = 5f;

	[SerializeField]
	protected float m_AttackPower = 1f;

	[SerializeField]
	protected List<AnimationSettings> m_AnimationSettings;

	[Header("Waypoint Options")]
	[SerializeField]
	protected AiWaypointMode m_WaypointMode;

	[SerializeField]
	protected bool m_UseRunForWaypoints;

	[SerializeField]
	protected bool m_UseRotateAtEndWaypoint;

	[SerializeField]
	protected Vector3 m_MoveToVectorPoint;

	[SerializeField]
	protected List<WaypointNode> m_CurrentWaypointList;

	[SerializeField]
	protected BaseAiController m_CurrentAiToFollow;

	[Header("Death Options")]
	[SerializeField]
	protected Transform m_RagdollParent;

	[SerializeField]
	protected List<InkDeathEffect> m_InkDeathEffect;

	protected Collider m_CurrentTargetCollider;

	protected CharacterController m_CharacterController;

	protected CollisionFlags m_CollisionFlags;

	protected int m_CurrentHealth;

	protected Vector3 m_MoveDir = Vector3.zero;

	protected bool m_PreviouslyGrounded;

	protected float m_CurrentMoveSpeed;

	protected float m_CurrentTurnRate = 3f;

	protected float m_RunDistanceBuffer;

	protected float m_AttackDistanceBuffer;

	protected List<PathfinderNode> m_CurrentPath;

	protected Vector3 m_TrackObjectPosition;

	protected Vector3 m_PlayerLastCheckedPosition;

	protected Vector3 m_MyLastCheckedPosition;

	protected float m_StuckCheckTimer;

	protected int m_WaypointIndex;

	protected float m_TimescaleDistanceIncrease;

	protected Vector3 AiToFollow_Offset;

	protected PathfinderNode m_LastEndNode;

	protected float m_PlayerMoveDistanceBeforeRepathing = 5f;

	protected float m_DistanceToAcceptNodeReached = 3f;

	protected float m_WaitTimer;

	protected AiThought m_NextThought;

	protected Vector3 m_AvoidanceLastPosition;

	protected float m_AvoidanceCooldownTime;

	protected List<Ragdoll> m_Ragdolls;

	protected Vector3 m_HitPosition;

	protected Action m_WaitCallback;

	private AudioClip[] m_HitAudio;

	protected int m_WalkMode = 1;

	protected int m_RunMode = 2;

	protected LayerMask m_SightMask;

	protected WeaponInfo m_WeaponInfo;

	protected RaycastHit m_RaycastHit;

	private Vector3 m_RaiseFromGroundOffset = new Vector3(0f, 0.5f, 0f);

	private Collider[] fnt_NearbyEntities = (Collider[])(object)new Collider[10];

	public Animator AnimationController => m_AnimationController;

	public Transform CurrentTarget { get; protected set; }

	public Transform PreviousTarget { get; protected set; }

	public Vector3 OriginPosition { get; protected set; }

	public bool isGrounded => m_CharacterController.isGrounded;

	public GameObject Attacker { get; private set; }

	public event EventHandler OnWaypointComplete;

	public event EventHandler OnDeath;

	public event EventHandler OnHide;

	public event EventHandler OnRespawn;

	public event EventHandler OnDistanceActivate;

	public event EventHandler OnActivate;

	public event EventHandler OnSpotted;

	public event EventHandler OnRetreat;

	public override void Init()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_CharacterController = ((Component)this).GetComponent<CharacterController>();
		m_SightMask = LayerMask.op_Implicit(1 << LayerMask.NameToLayer("Default"));
		m_AnimationController.logWarnings = false;
		m_WeaponInfo = new WeaponInfo();
		m_WeaponInfo.Attacker = base.gameObject;
		m_WeaponInfo.Damage = (int)m_AttackPower;
		m_RaycastHit = default(RaycastHit);
	}

	public override void InitOnComplete()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_HitAudio = GameManager.Instance.GetAudioClips("Audio/SFX/Weapons/HitEnemy/");
		((Component)m_AnimationController).gameObject.AddComponent<AnimationEventController>().Init(this);
		for (int i = 0; i < m_AnimationSettings.Count; i++)
		{
			AnimationSettings animationSettings = m_AnimationSettings[i];
			AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)animationSettings.Clip).name, "AttackTarget", animationSettings.Frame);
		}
		if (Object.op_Implicit((Object)(object)m_RagdollParent))
		{
			Ragdoll[] componentsInChildren = ((Component)m_RagdollParent).GetComponentsInChildren<Ragdoll>();
			m_Ragdolls = new List<Ragdoll>();
			Ragdoll[] array = componentsInChildren;
			foreach (Ragdoll ragdoll in array)
			{
				ragdoll.Initialize();
				m_Ragdolls.Add(ragdoll);
			}
		}
		SetThought(m_StartingThought);
		m_TrackObjectPosition = base.transform.position + base.transform.forward * 10f;
		Activate();
	}

	public override void Activate()
	{
		base.Activate();
		m_CurrentHealth = m_Health;
		SetAnimatiorMovement(0);
		GameManager.Instance.AiGlobalNetwork.AddAi(this);
	}

	protected override void T_EnterDie()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		base.T_EnterDie();
		SetTarget(null);
		((Component)this).tag = "Dead";
		SetMoveDirection(Vector3.zero);
		((Collider)m_CharacterController).enabled = false;
		CapsuleCollider component = ((Component)this).GetComponent<CapsuleCollider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			((Collider)component).enabled = false;
		}
		if (Object.op_Implicit((Object)(object)m_RagdollParent))
		{
			((Behaviour)m_AnimationController).enabled = false;
			for (int i = 0; i < m_Ragdolls.Count; i++)
			{
				Ragdoll ragdoll = m_Ragdolls[i];
				Vector3 position = m_HitPosition + new Vector3(0f, -0.5f, 0f);
				ragdoll.Activate(50f, position, 10f, -1f);
			}
		}
		else
		{
			SetAnimationTrigger("Dead");
		}
		if (m_InkDeathEffect == null || m_InkDeathEffect.Count <= 0)
		{
			return;
		}
		InkDeathEffect inkDeathEffect = m_InkDeathEffect[0];
		if (Object.op_Implicit((Object)(object)inkDeathEffect.InkExplosion))
		{
			inkDeathEffect.InkExplosion.OnExplode += HandleDeathOnComplete;
		}
		for (int j = 0; j < m_InkDeathEffect.Count; j++)
		{
			InkDeathEffect inkDeathEffect2 = m_InkDeathEffect[j];
			if (Object.op_Implicit((Object)(object)inkDeathEffect2.InkExplosion))
			{
				inkDeathEffect2.InkExplosion.Activate(inkDeathEffect2.Renderer);
			}
		}
	}

	protected virtual void HandleDeathOnComplete(object sender, EventArgs e)
	{
		(sender as InkExplosionEffect).OnExplode -= HandleDeathOnComplete;
		Dispose();
	}

	protected override void T_Wait()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.T_Wait();
		SetMoveDirection(Vector3.zero);
		SetAnimatiorMovement(0);
		if (m_WaitTimer <= 0f)
		{
			if (m_WaitCallback != null)
			{
				m_WaitCallback();
			}
			SetThought(m_NextThought);
		}
	}

	protected override void T_DistanceActivation()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = GameManager.Instance.Player.transform.position;
		if (Vector3.Distance(base.transform.position, position) < m_ActivationDistance && !Physics.Linecast(m_EyeLocation.position, position, LayerMask.op_Implicit(m_SightMask)))
		{
			SetThought(AiThought.Activate);
			SetTarget(GameManager.Instance.Player.transform);
		}
	}

	protected override void T_Activate()
	{
		SetAnimationTrigger("Birth");
		SetPhysicsEnabled(AllowPhysics: false);
		DoWait(m_AwakeAnimationWaitTime);
	}

	protected override void T_Idle()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckPhysicsEnabled())
		{
			SetPhysicsEnabled(AllowPhysics: true);
		}
		if (!Object.op_Implicit((Object)(object)CurrentTarget) || Vector3.Distance(base.transform.position, CurrentTarget.position) > m_AttackDistance)
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
			else if (!Object.op_Implicit((Object)(object)CurrentTarget))
			{
				SetTarget(TestTargetVisibility());
			}
			else
			{
				SetThought(AiThought.Attack);
			}
		}
	}

	protected override void T_EnterAttack()
	{
		base.T_EnterAttack();
		m_WaypointIndex = 0;
	}

	protected override void T_Attack()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)CurrentTarget) || m_PassiveAi)
		{
			SetThought(AiThought.Idle);
			return;
		}
		if (((Component)CurrentTarget).tag == "Dead")
		{
			CurrentTarget = null;
			SetThought(AiThought.Idle);
			return;
		}
		if ((Object)(object)CurrentTarget == (Object)(object)GameManager.Instance.Player.transform && GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding)
		{
			SetThought(AiThought.Retreat);
			return;
		}
		if (m_AvoidanceCooldownTime <= 0f)
		{
			m_TrackObjectPosition = CurrentTarget.position;
		}
		if (Vector3.Angle(base.transform.forward, new Vector3(CurrentTarget.position.x, base.transform.position.y, CurrentTarget.position.z) - base.transform.position) > 40f)
		{
			SetThought(AiThought.Follow);
		}
		else if (Vector3.Distance(base.transform.position, CurrentTarget.position) < m_AttackDistanceBuffer && !Physics.Linecast(m_KneeLocation.position, OffsetTargetPosition(), LayerMask.op_Implicit(m_ObstructionLayers)))
		{
			int num = Random.Range(0, m_AnimationSettings.Count);
			float length = m_AnimationSettings[num].Clip.length;
			m_AttackDistanceBuffer = m_AttackDistance;
			base.transform.rotation = FaceDirection();
			DoWait(length + 0.25f);
			StartAttack(num);
		}
		else
		{
			SetThought(AiThought.Follow);
		}
	}

	public virtual void SetVectorPoint(Vector3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_MoveToVectorPoint = point;
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
		if (!m_PassiveAi)
		{
			if (!Object.op_Implicit((Object)(object)CurrentTarget))
			{
				SetTarget(TestTargetVisibility());
			}
			else
			{
				SetThought(AiThought.Attack);
			}
		}
	}

	protected virtual void OnMoveToPointReached()
	{
		SetThought(base.PreviousThought);
	}

	protected override void T_EnterUseWaypoints()
	{
		base.T_EnterUseWaypoints();
		CurrentTarget = null;
	}

	protected override void T_UseWaypoints()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		if (m_WaypointMode == AiWaypointMode.None)
		{
			SetThought(AiThought.Idle);
			return;
		}
		if (m_WaypointMode == AiWaypointMode.Incremental && m_WaypointIndex >= m_CurrentWaypointList.Count)
		{
			if (m_UseRotateAtEndWaypoint && m_CurrentWaypointList.Count > 0)
			{
				Transform val = m_CurrentWaypointList[m_CurrentWaypointList.Count - 1].transform;
				m_TrackObjectPosition = val.position + val.forward;
				Quaternion rotation = Quaternion.LookRotation(val.forward);
				base.transform.rotation = rotation;
			}
			SetThought(AiThought.Idle);
			SendOnWaypointComplete();
			return;
		}
		SetMoveDirection(Vector3.zero);
		Transform val2 = null;
		m_WaypointIndex = Mathf.Clamp(m_WaypointIndex, 0, m_CurrentWaypointList.Count);
		Vector3 val3 = Vector3.zero;
		if (m_CurrentWaypointList != null && m_CurrentWaypointList.Count > 0)
		{
			val2 = m_CurrentWaypointList[m_WaypointIndex].transform;
			if (Vector3.Distance(base.transform.position, val2.position) < m_TimescaleDistanceIncrease)
			{
				if (m_WaypointMode == AiWaypointMode.Roam)
				{
					m_WaypointIndex = Random.Range(0, m_CurrentWaypointList.Count);
				}
				else if (m_WaypointMode == AiWaypointMode.Loop)
				{
					m_WaypointIndex++;
					if (m_WaypointIndex >= m_CurrentWaypointList.Count)
					{
						m_WaypointIndex = 0;
					}
				}
				else if (m_WaypointMode == AiWaypointMode.PingPing)
				{
					m_WaypointIndex++;
					if (m_WaypointIndex >= m_CurrentWaypointList.Count)
					{
						m_CurrentWaypointList.Reverse();
						m_WaypointIndex = 0;
					}
				}
				else if (m_WaypointMode == AiWaypointMode.Incremental)
				{
					m_WaypointIndex++;
				}
			}
		}
		if (m_WaypointMode == AiWaypointMode.Follow_Ai)
		{
			if (Object.op_Implicit((Object)(object)m_CurrentAiToFollow))
			{
				if (AiToFollow_Offset == Vector3.zero)
				{
					AiToFollow_Offset = new Vector3((float)Random.Range(-1, 2), 0f, (float)Random.Range(-1, 1));
					if (AiToFollow_Offset.x == 0f && AiToFollow_Offset.z == 0f)
					{
						AiToFollow_Offset.z = -1f;
					}
					AiToFollow_Offset *= 4f;
				}
				val2 = m_CurrentAiToFollow.transform;
				val3 = m_CurrentAiToFollow.transform.TransformDirection(AiToFollow_Offset);
			}
			else
			{
				m_WaypointMode = AiWaypointMode.Roam;
			}
		}
		if (Object.op_Implicit((Object)(object)val2))
		{
			CheckSpeed(m_UseRunForWaypoints);
			if (!SolvePathMovement(val2.position + val3))
			{
				if (Vector3.Distance(base.transform.position, val2.position) < m_TimescaleDistanceIncrease)
				{
					SetMoveDirection(Vector3.zero);
					m_WaypointIndex++;
				}
			}
			else if (m_CurrentPath != null && m_CurrentPath.Count > 0)
			{
				SetMoveDirection(base.transform.forward);
			}
			else
			{
				SetMoveDirection(base.transform.forward);
				SolvePathMovement(val2.position);
			}
		}
		else if (Object.op_Implicit((Object)(object)CurrentTarget))
		{
			SolvePathMovement(OffsetTargetPosition());
			SetMoveDirection(base.transform.forward);
		}
		else
		{
			SetThought(AiThought.Idle);
		}
		if (!m_PassiveAi)
		{
			if (!Object.op_Implicit((Object)(object)CurrentTarget))
			{
				SetTarget(TestTargetVisibility());
			}
			else
			{
				SetThought(AiThought.Attack);
			}
		}
	}

	protected override void T_EnterFollow()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base.T_EnterFollow();
		if (Object.op_Implicit((Object)(object)CurrentTarget) && !m_PassiveAi)
		{
			SolvePathMovement(OffsetTargetPosition());
		}
		m_AttackDistanceBuffer = m_AttackDistance;
	}

	protected override void T_Follow()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		SetMoveDirection(Vector3.zero);
		if ((Object)(object)CurrentTarget == (Object)(object)GameManager.Instance.Player.transform && GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding)
		{
			SetThought(AiThought.UseWaypoints);
		}
		else
		{
			if (TestLoseTracking())
			{
				return;
			}
			if (Vector3.Distance(base.transform.position, CurrentTarget.position) < m_AttackDistanceBuffer)
			{
				m_AttackDistanceBuffer = m_AttackDistance + 8f;
				if (!Physics.Linecast(m_KneeLocation.position, OffsetTargetPosition(), LayerMask.op_Implicit(m_ObstructionLayers)))
				{
					SetThought(AiThought.Idle);
					return;
				}
			}
			else
			{
				m_AttackDistanceBuffer = m_AttackDistance;
			}
			if (!Physics.Linecast(m_KneeLocation.position, OffsetTargetPosition(), LayerMask.op_Implicit(m_ObstructionLayers)))
			{
				Debug.DrawLine(m_KneeLocation.position, CurrentTarget.position);
				m_CurrentPath = null;
				if (m_AvoidanceCooldownTime > 0f)
				{
					m_TrackObjectPosition = CurrentTarget.position;
				}
			}
			else
			{
				SolvePathMovement(OffsetTargetPosition());
				if (TestLoseTracking())
				{
					return;
				}
			}
			CheckSpeed(Vector3.Distance(base.transform.position, CurrentTarget.position) >= m_RunDistanceBuffer);
			SetMoveDirection(base.transform.forward);
		}
	}

	protected virtual void FixedUpdate()
	{
	}

	protected virtual void Update()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsDisposed && !GameManager.Instance.isPaused && base.IsActive && !IsInThought(AiThought.Die) && !IsInThought(AiThought.Inactive) && !IsInThought(AiThought.DistanceActivation) && !IsInThought(AiThought.Activate))
		{
			if (IsInThought(AiThought.Wait))
			{
				m_WaitTimer -= Time.deltaTime;
				base.transform.rotation = FaceDirection();
			}
			else
			{
				base.transform.rotation = FaceDirection();
				m_TimescaleDistanceIncrease = Mathf.Clamp(Time.unscaledDeltaTime / 0.016f / 2f, 1f, 3f);
			}
		}
	}

	protected virtual void LateUpdate()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsDisposed || GameManager.Instance.isPaused || !base.IsActive || IsInThought(AiThought.Die) || IsInThought(AiThought.Inactive) || IsInThought(AiThought.DistanceActivation) || IsInThought(AiThought.Activate) || IsInThought(AiThought.Wait))
		{
			return;
		}
		if (m_EnableAvoidance)
		{
			AvoidObstacle(m_TrackObjectPosition);
			CheckIfStuck(m_TrackObjectPosition);
			if (Vector3.Distance(base.transform.position, m_TrackObjectPosition) < 0.5f)
			{
				SetAnimatiorMovement(0);
				m_MoveDir = Vector3.zero;
			}
		}
		CheckGrounding();
		Move();
	}

	public virtual void SetPhysicsEnabled(bool AllowPhysics)
	{
		((Collider)m_CharacterController).enabled = AllowPhysics;
		CapsuleCollider component = ((Component)this).GetComponent<CapsuleCollider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			((Collider)component).enabled = AllowPhysics;
		}
	}

	private bool CheckPhysicsEnabled()
	{
		if ((Object)(object)m_CharacterController != (Object)null)
		{
			CapsuleCollider component = ((Component)this).GetComponent<CapsuleCollider>();
			if ((Object)(object)component != (Object)null && ((Collider)m_CharacterController).enabled)
			{
				return ((Collider)component).enabled;
			}
			return false;
		}
		return false;
	}

	private void CheckGrounding()
	{
		if (isGrounded && !m_PreviouslyGrounded)
		{
			m_MoveDir.y = 0f;
		}
		if (!isGrounded && m_PreviouslyGrounded)
		{
			m_MoveDir.y = 0f;
		}
		m_PreviouslyGrounded = isGrounded;
	}

	public virtual void Move()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (isGrounded)
		{
			m_MoveDir.y = -10f;
		}
		else if (m_EnableGravity)
		{
			m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime / GameManager.Instance.Player.deltaTimeMultiplier;
		}
		if (((Collider)m_CharacterController).enabled)
		{
			m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime / GameManager.Instance.Player.deltaTimeMultiplier);
		}
	}

	public Quaternion FaceDirection(bool isSmooth = true, float speedModifier = 1f)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(m_TrackObjectPosition.x, base.transform.position.y, m_TrackObjectPosition.z) - base.transform.position;
		Quaternion val2 = ((!(val == Vector3.zero)) ? Quaternion.LookRotation(val) : Quaternion.identity);
		if (val == Vector3.zero)
		{
			return Quaternion.identity;
		}
		if (isSmooth)
		{
			return Quaternion.Slerp(base.transform.rotation, val2, m_CurrentTurnRate * speedModifier * Time.deltaTime);
		}
		return val2;
	}

	public Quaternion ForceFaceDirection()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_TrackObjectPosition = CurrentTarget.position;
		return FaceDirection(isSmooth: false);
	}

	public void PlayAnimation(string _AnimationName, float _waitTime, AiThought _followupThought)
	{
		m_AnimationController.Play(_AnimationName);
		SetThought(AiThought.PlaySingleAnimation);
		DoWait(_waitTime, _followupThought);
	}

	protected bool SolvePathMovement(Vector3 targetPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return CheckPath(targetPosition);
	}

	private bool CheckPath(Vector3 targetPosition)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (m_AvoidanceCooldownTime > 0f)
		{
			return true;
		}
		Vector3 position = base.transform.position;
		position.y += 1f;
		Vector3 val = targetPosition;
		val.y += 1f;
		if (Vector3.Distance(base.transform.position, targetPosition) > m_UseNodePathDistance)
		{
			return GetPath(targetPosition);
		}
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Linecast(position, val, ref val2, LayerMask.op_Implicit(m_ObstructionLayers), (QueryTriggerInteraction)1))
		{
			return GetPath(targetPosition);
		}
		m_CurrentPath = null;
		m_TrackObjectPosition = targetPosition;
		return true;
	}

	private bool GetPath(Vector3 targetPosition, bool TrackingLostValidationCheck = false)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		if (m_CurrentPath == null || Vector3.Distance(m_PlayerLastCheckedPosition, targetPosition) > m_PlayerMoveDistanceBeforeRepathing)
		{
			m_CurrentPath = GameManager.Instance.AiGlobalNetwork.SolvePath(base.transform.position, targetPosition);
			if (m_CurrentPath == null || m_CurrentPath.Count == 0)
			{
				if (!m_PassiveAi && base.CurrentThought == AiThought.Follow && !TrackingLostValidationCheck)
				{
					TestLoseTracking();
				}
				m_LastEndNode = null;
				return false;
			}
			m_LastEndNode = m_CurrentPath[m_CurrentPath.Count - 1];
			if (m_CurrentPath != null && m_CurrentPath.Count > 1)
			{
				m_CurrentPath.RemoveAt(0);
			}
			m_PlayerLastCheckedPosition = targetPosition;
		}
		if (m_CurrentPath != null && m_CurrentPath.Count > 0 && Vector3.Distance(base.transform.position, m_CurrentPath[0].Position) < m_DistanceToAcceptNodeReached)
		{
			if (m_CurrentPath.Count > 0)
			{
				m_CurrentPath.RemoveAt(0);
			}
			else
			{
				m_CurrentPath = null;
			}
		}
		if (m_CurrentPath != null && m_CurrentPath.Count > 0)
		{
			m_TrackObjectPosition = m_CurrentPath[0].Position;
		}
		return true;
	}

	public PathfinderNode GetCurrentEndNode()
	{
		return m_LastEndNode;
	}

	private void AvoidObstacle(Vector3 targetPosition)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (m_AvoidanceCooldownTime <= 0f)
		{
			m_AvoidanceLastPosition = Vector3.zero;
			bool flag = false;
			if (Physics.CheckSphere(m_KneeLocation.position + base.transform.forward * 1.5f, 0.4f, LayerMask.op_Implicit(m_ObstructionLayers), (QueryTriggerInteraction)1))
			{
				flag = true;
			}
			if (flag)
			{
				List<Vector3> list = new List<Vector3>();
				Debug.DrawRay(m_KneeLocation.position, base.transform.forward * 1.5f, Color.yellow, 2f);
				if (Vector3.Distance(base.transform.position, m_TrackObjectPosition) > 20f)
				{
					m_CurrentPath = null;
				}
				Vector3 val = default(Vector3);
				Vector3 val4 = default(Vector3);
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if ((i != 0 || j != 0) && Mathf.Abs(i) != Mathf.Abs(j))
						{
							Vector3 position = m_KneeLocation.position;
							((Vector3)(ref val))._002Ector((float)i, 0f, (float)j);
							Vector3 val2 = position + ((Vector3)(ref val)).normalized * 1.7f;
							if (!Physics.CheckSphere(val2, 0.6f, LayerMask.op_Implicit(m_ObstructionLayers)) && !Physics.Linecast(m_KneeLocation.position, val2, LayerMask.op_Implicit(m_ObstructionLayers)))
							{
								Vector3 val3 = val2;
								((Vector3)(ref val4))._002Ector((float)i, 0f, (float)j);
								list.Add(val3 + ((Vector3)(ref val4)).normalized * 10f);
								Debug.DrawLine(m_KneeLocation.position, val2, Color.cyan, 1f);
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

	public virtual Transform FineNearbyTargets()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, m_VisibilityDistance, fnt_NearbyEntities, LayerMask.op_Implicit(m_TargetableEntityLayers));
		if (num > 0)
		{
			Transform result = null;
			float num2 = -1f;
			for (int i = 0; i < num; i++)
			{
				Transform val = ((Component)fnt_NearbyEntities[i]).transform;
				float num3 = Vector3.Distance(base.transform.position, val.position);
				if (((Component)val).gameObject.tag != "Dead" && (num2 < 0f || num3 < num2))
				{
					num2 = num3;
					result = val;
				}
			}
			return result;
		}
		return null;
	}

	public virtual Vector3 OffsetTargetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return CurrentTarget.position + m_RaiseFromGroundOffset;
	}

	public virtual Transform TestTargetVisibility()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Transform val = FineNearbyTargets();
		if (Object.op_Implicit((Object)(object)val) && (Vector3.Angle(base.transform.forward, val.position - base.transform.position) < m_TargetVisibilityAngle || Vector3.Distance(val.position, base.transform.position) < 10f) && !Physics.Linecast(m_EyeLocation.position, val.position + m_RaiseFromGroundOffset, LayerMask.op_Implicit(m_SightMask)))
		{
			if (!Object.op_Implicit((Object)(object)CurrentTarget) && m_EnableSpottedAnimation)
			{
				DoWait(m_SpottedAnimationWaitTime, AiThought.Follow);
				m_TrackObjectPosition = val.position;
				SendOnSpotted();
				SetAnimationTrigger("EnemySpotted");
			}
			return val;
		}
		return null;
	}

	public void ForceOnSpotted()
	{
		SendOnSpotted();
	}

	public virtual void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (base.CurrentThought == AiThought.Die)
		{
			return;
		}
		m_HitPosition = ((RaycastHit)(ref hit)).point;
		if (base.CurrentThought != AiThought.Attack && LayerMask.op_Implicit(m_TargetableEntityLayers) == (LayerMask.op_Implicit(m_TargetableEntityLayers) | (1 << weaponInfo.Attacker.layer)))
		{
			SetTarget(weaponInfo.Attacker.transform);
			SetThought(AiThought.Attack);
		}
		if (m_IsInvincible)
		{
			return;
		}
		PlayAudio(ref m_HitAudio);
		GameObject fromPool = GameManager.Instance.PoolingManager.GetFromPool("GamePlay/Particles/InkHit");
		fromPool.transform.position = ((RaycastHit)(ref hit)).point;
		fromPool.transform.localScale = Vector3.one;
		m_CurrentHealth -= weaponInfo.Damage;
		if (m_CurrentHealth <= 0)
		{
			if (weaponInfo != null)
			{
				Attacker = weaponInfo.Attacker;
			}
			SetThought(AiThought.Die);
		}
	}

	public void Reset()
	{
		SetAnimationTrigger("Respawn");
		SetThought(m_StartingThought);
		((Collider)m_CharacterController).enabled = true;
		m_CurrentHealth = m_Health;
		SetTarget(null);
	}

	public void SetPassive(bool IsPassive)
	{
		m_PassiveAi = IsPassive;
	}

	protected void StartAttack(int attackNumber)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetMoveDirection(Vector3.zero);
		SetAnimatiorAttack(attackNumber + 1);
	}

	public virtual void AttackTarget()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)CurrentTarget))
		{
			return;
		}
		m_TrackObjectPosition = CurrentTarget.position;
		base.transform.rotation = FaceDirection(isSmooth: true, 10f);
		Vector3 val = m_KneeLocation.position + base.transform.forward * (m_AttackDistance + 1f);
		Collider[] array = Physics.OverlapCapsule(m_KneeLocation.position, val, 1f, LayerMask.op_Implicit(m_TargetableEntityLayers));
		if (array.Length == 0)
		{
			SetAnimatiorMovement(0);
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (((Component)array[i]).gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				for (int j = 0; (float)j < m_AttackPower; j++)
				{
					GameManager.Instance.ShowHurtBorder();
				}
			}
			if (((Component)array[i]).gameObject.GetComponent<IHittable>() != null)
			{
				((RaycastHit)(ref m_RaycastHit)).point = val;
				((Component)array[i]).gameObject.GetComponent<IHittable>().Hit(m_RaycastHit, m_WeaponInfo);
			}
		}
	}

	protected bool TestLoseTracking()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (!m_PassiveAi && Object.op_Implicit((Object)(object)CurrentTarget) && (m_CurrentPath == null || m_CurrentPath.Count == 0))
		{
			if (!Physics.Linecast(m_EyeLocation.position, OffsetTargetPosition(), LayerMask.op_Implicit(m_SightMask)))
			{
				m_TrackObjectPosition = CurrentTarget.position;
				return false;
			}
			if (!Object.op_Implicit((Object)(object)m_LastEndNode) || Physics.Linecast(m_LastEndNode.Position + Vector3.up * 5f, OffsetTargetPosition(), LayerMask.op_Implicit(m_ObstructionLayers)))
			{
				if (!GetPath(OffsetTargetPosition(), TrackingLostValidationCheck: true))
				{
					return TrackingLostEvents(AiThought.Retreat);
				}
				if (m_AvoidanceCooldownTime <= 0f)
				{
					m_TrackObjectPosition = CurrentTarget.position;
				}
				return false;
			}
			if (m_AvoidanceCooldownTime <= 0f)
			{
				m_TrackObjectPosition = m_LastEndNode.Position;
				m_LastEndNode = null;
				return false;
			}
		}
		if (GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding || !Object.op_Implicit((Object)(object)CurrentTarget) || (Object.op_Implicit((Object)(object)CurrentTarget) && Vector3.Distance(base.transform.position, CurrentTarget.position) > m_LoseTrackingVisibilityDistance))
		{
			return TrackingLostEvents(AiThought.Retreat);
		}
		return false;
	}

	protected virtual bool TrackingLostEvents(AiThought _repalcementThought)
	{
		if (!m_PassiveAi)
		{
			m_CurrentPath = null;
			SetTarget(null);
		}
		SetThought(_repalcementThought);
		return true;
	}

	protected virtual void CheckIfStuck(Vector3 inputPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (m_MyLastCheckedPosition == Vector3.zero)
		{
			m_MyLastCheckedPosition = base.transform.position;
		}
		else if (m_StuckCheckTimer > 5f)
		{
			if (Vector3.Distance(m_MyLastCheckedPosition, base.transform.position) < 5f)
			{
				m_CurrentPath = null;
			}
			m_MyLastCheckedPosition = base.transform.position;
			m_StuckCheckTimer = 0f;
		}
		else
		{
			m_StuckCheckTimer += Time.deltaTime;
		}
	}

	protected void DoWait(float timer, AiThought nextThought = AiThought.Idle, Action callback = null)
	{
		m_WaitCallback = callback;
		SetThought(AiThought.Wait);
		m_WaitTimer = timer;
		m_NextThought = nextThought;
	}

	public void SetUseRunForWaypoints(bool useRun)
	{
		m_UseRunForWaypoints = useRun;
	}

	protected void CheckSpeed(bool shouldRun)
	{
		if (shouldRun)
		{
			SetRun();
			m_RunDistanceBuffer = m_RunDistance - 1.5f;
		}
		else
		{
			SetWalk();
			m_RunDistanceBuffer = m_RunDistance;
		}
	}

	protected void SetRun()
	{
		m_CurrentMoveSpeed = m_RunSpeed;
		m_CurrentTurnRate = m_RunTurnSpeed;
		SetAnimatiorMovement(m_RunMode);
	}

	protected void SetWalk()
	{
		m_CurrentMoveSpeed = m_WalkSpeed;
		m_CurrentTurnRate = m_WalkTurnSpeed;
		SetAnimatiorMovement(m_WalkMode);
	}

	protected void SetAnimatiorMovement(int _movemetType)
	{
		m_AnimationController.SetInteger("MovementMode", _movemetType);
	}

	protected void SetAnimatiorAttack(int _attackType)
	{
		if (_attackType > 0 && !m_PassiveAi)
		{
			m_AnimationController.SetTrigger("Attack_" + _attackType);
		}
	}

	protected void SetAnimationTrigger(string _Trigger)
	{
		m_AnimationController.SetTrigger(_Trigger);
	}

	protected void SetMoveDirection(Vector3 moveDirection)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		m_MoveDir.x = moveDirection.x * m_CurrentMoveSpeed;
		m_MoveDir.z = moveDirection.z * m_CurrentMoveSpeed;
	}

	public void SetTarget(Transform newTarget)
	{
		if (!((Object)(object)CurrentTarget != (Object)(object)newTarget))
		{
			return;
		}
		PreviousTarget = CurrentTarget;
		CurrentTarget = newTarget;
		if (!m_PassiveAi)
		{
			GameManager.Instance.AiGlobalNetwork.CheckCombatStatus();
		}
		if (!(this is Ch4_SoundBasedAI))
		{
			return;
		}
		for (int i = 0; i < GameManager.Instance.AiGlobalNetwork.ListeningAi.Count; i++)
		{
			if ((Object)(object)this != (Object)(object)GameManager.Instance.AiGlobalNetwork.ListeningAi[i])
			{
				GameManager.Instance.AiGlobalNetwork.ListeningAi[i].SetTarget(newTarget);
			}
		}
	}

	public virtual void SetAiFollowTarget(BaseAiController aiToFollow)
	{
		m_CurrentAiToFollow = aiToFollow;
		m_WaypointMode = AiWaypointMode.Follow_Ai;
		SetThought(AiThought.UseWaypoints);
	}

	public void SetStartingThought(AiThought startingThought)
	{
		m_StartingThought = startingThought;
	}

	private AudioObject PlayAudio(ref AudioClip[] audioClips, bool is2D = false)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (audioClips == null || audioClips.Length == 0)
		{
			return null;
		}
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		AudioObject result = ((!is2D) ? GameManager.Instance.AudioManager.PlayAtPosition(val, m_EyeLocation.position) : GameManager.Instance.AudioManager.Play(val));
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
		return result;
	}

	public void ForceDeath(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_HitPosition = position;
		CurrentTarget = null;
		m_CurrentHealth = 0;
		SetThought(AiThought.Die);
	}

	protected void SendOnWaypointComplete()
	{
		this.OnWaypointComplete.Send(this);
	}

	protected void SendOnDeath()
	{
		this.OnDeath.Send(this);
	}

	protected void SendOnHide()
	{
		this.OnHide.Send(this);
	}

	protected void SendOnRespawn()
	{
		this.OnRespawn.Send(this);
	}

	protected void SendOnDistanceActivate()
	{
		this.OnDistanceActivate.Send(this);
	}

	protected void SendOnActive()
	{
		this.OnActivate.Send(this);
	}

	protected void SendOnSpotted()
	{
		this.OnSpotted.Send(this);
	}

	protected void SendOnRetreat()
	{
		this.OnRetreat.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnWaypointComplete = null;
		this.OnDeath = null;
		this.OnHide = null;
		this.OnRespawn = null;
		this.OnDistanceActivate = null;
		this.OnActivate = null;
		this.OnSpotted = null;
		this.OnRetreat = null;
		base.OnDisposed();
	}
}
