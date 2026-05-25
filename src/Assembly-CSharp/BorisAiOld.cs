using System;
using System.Collections.Generic;
using Ai;
using UnityEngine;

public class BorisAiOld : BaseAiController
{
	[Header("<< Boris Options >>")]
	[SerializeField]
	private float m_LookAroundAnimTime = 2.66f;

	[SerializeField]
	private Interactable m_Interact;

	[SerializeField]
	private Transform m_Head;

	[SerializeField]
	private Transform m_HeadLookForward;

	[SerializeField]
	private Vector3 m_HeadOffset;

	[SerializeField]
	private Transform m_ToolboxParent;

	[SerializeField]
	private Transform m_Toolbox;

	[SerializeField]
	private Transform m_Hand;

	[SerializeField]
	private Transform m_Table;

	[SerializeField]
	private Transform m_GetUp;

	[Header("Hand Holders")]
	[SerializeField]
	private Transform m_FlashlightHand;

	[SerializeField]
	private Transform m_ToolboxHand;

	[SerializeField]
	private Transform m_PipeHand;

	[Header("Mouth")]
	[SerializeField]
	private Transform m_Mouth;

	private Transform m_Target;

	private bool m_IsInVent;

	private bool m_IsLooking;

	private bool m_IsSitting = true;

	private bool m_IsWaypointPathing;

	private bool m_IsFollowing;

	private bool m_IsCowering;

	private float m_OriginWalkSpeed;

	private float m_OriginRunSpeed;

	private Quaternion m_LastLookRotation;

	public Transform FlashlightHand => m_FlashlightHand;

	public Transform ToolboxHand => m_ToolboxHand;

	public Transform PipeHand => m_PipeHand;

	public Transform Mouth => m_Mouth;

	public Interactable Interact => m_Interact;

	public event EventHandler OnToolboxPlaced;

	public event EventHandler OnGetUp;

	public override void Init()
	{
		base.Init();
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_RunMode = 3;
		((Collider)m_CharacterController).enabled = false;
		m_OriginWalkSpeed = m_WalkSpeed;
		m_OriginRunSpeed = m_RunSpeed;
	}

	public override void Activate()
	{
		base.Activate();
		GameManager.Instance.AiGlobalNetwork.RemoveAi(this);
		StopStateMachine();
	}

	protected override void Update()
	{
		if (!m_IsSitting)
		{
			base.Update();
		}
	}

	protected override void LateUpdate()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		base.LateUpdate();
		if (!((Object)(object)m_Target == (Object)null) && m_IsLooking && !m_IsCowering && !m_IsInVent)
		{
			float num = Vector3.Angle(base.transform.forward, m_Target.position - base.transform.position);
			if (Vector3.Distance(m_Target.position, base.transform.position) < 15f && num < 70f)
			{
				Vector3 val = m_Target.position - m_Head.position;
				Quaternion val2 = Quaternion.LookRotation(val) * Quaternion.Euler(m_HeadOffset);
				m_Head.rotation = Quaternion.Slerp(m_LastLookRotation, val2, 2f * Time.deltaTime);
			}
			else
			{
				Quaternion val3 = Quaternion.LookRotation(m_HeadLookForward.forward) * Quaternion.Euler(m_HeadOffset);
				m_Head.rotation = Quaternion.Slerp(m_LastLookRotation, val3, 2f * Time.deltaTime);
			}
			m_LastLookRotation = m_Head.rotation;
		}
	}

	protected override void T_UseWaypoints()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		SetMoveDirection(Vector3.zero);
		if (m_IsCowering)
		{
			return;
		}
		if (m_WaypointMode == AiWaypointMode.Incremental && m_WaypointIndex >= m_CurrentWaypointList.Count)
		{
			if (m_UseRotateAtEndWaypoint && m_CurrentWaypointList.Count > 0)
			{
				base.transform.rotation = m_CurrentWaypointList[m_CurrentWaypointList.Count - 1].transform.rotation;
			}
			SetThought(AiThought.Idle);
			m_CurrentWaypointList.Clear();
			m_WaypointIndex = 0;
			m_IsWaypointPathing = true;
			SendOnWaypointComplete();
			return;
		}
		if (!m_UseRunForWaypoints && Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) < 5f)
		{
			SetThought(AiThought.Idle);
			return;
		}
		m_WaypointIndex = Mathf.Clamp(m_WaypointIndex, 0, m_CurrentWaypointList.Count);
		Transform val = m_CurrentWaypointList[m_WaypointIndex].transform;
		if (m_CurrentWaypointList == null || m_CurrentWaypointList.Count <= 0)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, val.position) < m_TimescaleDistanceIncrease)
		{
			if (m_WaypointMode == AiWaypointMode.Roam)
			{
				m_WaypointIndex = Random.Range(0, m_CurrentWaypointList.Count);
			}
			else if (m_WaypointMode == AiWaypointMode.Loop)
			{
				m_WaypointIndex++;
				if (m_WaypointIndex == m_CurrentWaypointList.Count)
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
		m_TrackObjectPosition = val.position;
		CheckSpeed(m_UseRunForWaypoints);
		StopLooking();
		SetMoveDirection(base.transform.forward);
	}

	protected override void T_EnterRetreat()
	{
		base.T_EnterRetreat();
		SetThought(AiThought.Idle);
	}

	protected override void T_EnterIdle()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		LookReset();
		m_LastLookRotation = m_Head.rotation;
		base.T_EnterIdle();
	}

	protected override void T_Idle()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		SetMoveDirection(Vector3.zero);
		SetAnimatiorMovement(0);
		if (m_IsCowering)
		{
			return;
		}
		if (!m_IsFollowing && !m_IsWaypointPathing)
		{
			base.T_Idle();
			return;
		}
		if (m_IsWaypointPathing && Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > 5f)
		{
			SetThought(AiThought.UseWaypoints);
			return;
		}
		LookReset();
		m_TrackObjectPosition = base.transform.position + base.transform.forward * 10f;
		if ((Object)(object)base.CurrentTarget == (Object)null)
		{
			SetTarget(TestTargetVisibility());
		}
		else
		{
			SetThought(AiThought.Follow);
		}
	}

	protected override void T_Follow()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		SetMoveDirection(Vector3.zero);
		if (m_IsCowering)
		{
			return;
		}
		if ((Object)(object)base.CurrentTarget == (Object)null)
		{
			SetTarget(TestTargetVisibility());
		}
		if ((Object)(object)base.CurrentTarget == (Object)null)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, base.CurrentTarget.position) < m_AttackDistance)
		{
			SetThought(AiThought.Idle);
			return;
		}
		SolvePathMovement(base.CurrentTarget.position);
		if ((Object)(object)base.CurrentTarget != (Object)null)
		{
			CheckSpeed(Vector3.Distance(base.transform.position, base.CurrentTarget.position) >= m_RunDistance);
		}
		StopLooking();
		SetMoveDirection(base.transform.forward);
	}

	public void SetCower(bool active)
	{
		int num = (active ? 2 : 0);
		m_IsCowering = active;
		m_AnimationController.SetTrigger("Cower");
		m_AnimationController.SetInteger("AnimationMode", num);
	}

	public void LookAround()
	{
		m_AnimationController.SetInteger("AnimationMode", 1);
		DoWait(m_LookAroundAnimTime, base.CurrentThought, delegate
		{
			m_AnimationController.SetInteger("AnimationMode", 0);
		});
	}

	public void FollowPlayer()
	{
		m_IsFollowing = true;
		SetThought(AiThought.Follow);
	}

	public void StopFollowing()
	{
		m_IsFollowing = false;
		SetThought(AiThought.Idle);
	}

	public void SlowSpeeds()
	{
		m_WalkMode = 2;
		m_RunSpeed = 6f;
		m_WalkSpeed = 3f;
	}

	public void ResetSpeeds()
	{
		m_WalkMode = 1;
		m_RunSpeed = m_OriginRunSpeed;
		m_WalkSpeed = m_OriginWalkSpeed;
	}

	public void EnableInteract()
	{
		m_Interact.SetActive(active: true);
	}

	public void EnterVent()
	{
		StopLooking();
		m_IsInVent = true;
		((Collider)m_CharacterController).enabled = false;
		m_AnimationController.SetInteger("AnimationMode", 3);
		SetThought(AiThought.Inactive);
	}

	public void ResetAll()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		m_IsInVent = false;
		((Collider)m_CharacterController).enabled = true;
		m_TrackObjectPosition = base.transform.position + base.transform.forward * 10f;
		SetMoveDirection(Vector3.zero);
		m_AnimationController.SetInteger("AnimationMode", 0);
		SetAnimatiorMovement(0);
		SetThought(AiThought.Idle);
	}

	public void IsAlreadyActive()
	{
		m_AnimationController.SetBool("IsActive", true);
		SetThought(AiThought.Idle);
	}

	public void GetToolbox()
	{
		StopLooking();
		m_AnimationController.SetTrigger("GetToolbox");
		AnimationClip val = null;
		for (int i = 0; i < m_AnimationController.runtimeAnimatorController.animationClips.Length; i++)
		{
			if (((Object)m_AnimationController.runtimeAnimatorController.animationClips[i]).name.Contains("toolbox"))
			{
				val = m_AnimationController.runtimeAnimatorController.animationClips[i];
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			val.AddEvent(AddEvent("GrabToolbox", 2f / 3f));
			val.AddEvent(AddEvent("PlaceToolbox", (float)Math.PI * 113f / 150f));
			val.AddEvent(AddEvent("LookReset", 3.5f));
		}
	}

	private AnimationEvent AddEvent(string functionName, float time)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		AnimationEvent val = new AnimationEvent();
		val.functionName = functionName;
		val.time = time;
		val.objectReferenceParameter = (Object)(object)this;
		return val;
	}

	public void GrabToolbox()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		m_Toolbox.SetParent(m_Hand);
		m_Toolbox.localPosition = Vector3.zero;
		m_Toolbox.localEulerAngles = Vector3.zero;
	}

	public void PlaceToolbox()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		m_Toolbox.SetParent(m_ToolboxParent);
		m_Toolbox.position = m_Table.position;
		m_Toolbox.eulerAngles = m_Table.eulerAngles;
		GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/CH3/SFX_CH3_borisplacetoolbox", m_Toolbox.position);
		this.OnToolboxPlaced.Send(this);
	}

	public void LookReset()
	{
		if ((Object)(object)m_Target != (Object)null)
		{
			LookAtTarget(m_Target);
		}
	}

	public void ForceStand()
	{
		m_AnimationController.SetBool("IsActive", true);
		m_IsSitting = false;
		StartStateMachine();
		SetThought(AiThought.Idle);
		((Collider)m_CharacterController).enabled = true;
	}

	public void GetUp()
	{
		m_AnimationController.SetBool("IsSitting", false);
		AnimationClip val = null;
		for (int i = 0; i < m_AnimationController.runtimeAnimatorController.animationClips.Length; i++)
		{
			if (((Object)m_AnimationController.runtimeAnimatorController.animationClips[i]).name.Contains("getup"))
			{
				val = m_AnimationController.runtimeAnimatorController.animationClips[i];
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			val.AddEvent(AddEvent("EnableBorisPathing", 2f));
		}
	}

	private void EnableBorisPathing()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		EnableWaypointPathing();
		base.transform.position = m_GetUp.position;
		this.OnGetUp.Send(this);
	}

	public void EnableWaypointPathing()
	{
		m_IsSitting = false;
		StartStateMachine();
		SetThought(AiThought.Idle);
		((Collider)m_CharacterController).enabled = true;
	}

	public void UpdateWaypointList(List<WaypointNode> waypointList, bool isRunning = false)
	{
		m_CurrentWaypointList.Clear();
		m_UseRunForWaypoints = isRunning;
		m_WaypointIndex = 0;
		m_IsWaypointPathing = true;
		m_CurrentWaypointList = waypointList;
		SetThought(AiThought.UseWaypoints);
	}

	public void AddToWaypointList(List<WaypointNode> waypointList, bool isRunning = false)
	{
		m_UseRunForWaypoints = isRunning;
		m_IsWaypointPathing = true;
		for (int i = 0; i < waypointList.Count; i++)
		{
			m_CurrentWaypointList.Add(waypointList[i]);
		}
	}

	protected override void CheckIfStuck(Vector3 inputPosition)
	{
	}

	public void StopWaypointPathing()
	{
		m_IsWaypointPathing = false;
	}

	public void LookAtTarget(Transform target)
	{
		m_IsLooking = true;
		m_Target = target;
	}

	public void StopLooking()
	{
		m_IsLooking = false;
	}

	public void ClearTarget()
	{
		m_Target = null;
	}
}
