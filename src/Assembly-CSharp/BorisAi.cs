using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class BorisAi : TMGMonoBehaviour, IHittable
{
	[Header("<< Boris Options >>")]
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

	[SerializeField]
	private Animator m_AnimationController;

	[SerializeField]
	private Transform m_AudioProxy;

	[SerializeField]
	private AnimationClip m_LookAroundClip;

	[Header("<< Boris Moement Options >>")]
	[SerializeField]
	private float m_WalkSpeed;

	[SerializeField]
	private float m_RunSpeed;

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

	private List<WaypointNode> m_CurrentWaypointList = new List<WaypointNode>();

	private int m_WaypointIndex;

	private bool m_UseRun;

	protected float m_WaitTimer;

	private S13Switch m_AudioSwitch;

	private bool m_IsInVent;

	private bool m_IsSitting = true;

	private Quaternion SnapRotation;

	private Vector3 SnapPosition;

	private Vector3 m_AnimatedHeadPosition;

	private bool m_IsCowering;

	private bool m_IsSlow;

	private bool m_IsAnimating;

	private float m_OriginWalkSpeed;

	private float m_OriginRunSpeed;

	private float m_CowerApproachDistance = 10f;

	private CharacterController m_CharacterController;

	protected Action m_WaitCallback;

	private Quaternion m_LastLookRotation;

	public Transform FlashlightHand => m_FlashlightHand;

	public Transform ToolboxHand => m_ToolboxHand;

	public Transform PipeHand => m_PipeHand;

	public Transform Mouth => m_Mouth;

	public Interactable Interact => m_Interact;

	public event EventHandler OnToolboxPlaced;

	public event EventHandler OnGetUp;

	public event EventHandler OnWaypointComplete;

	public override void Init()
	{
		base.Init();
		GameManager.Instance.CharacterManager.Boris = this;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_CharacterController = ((Component)this).GetComponent<CharacterController>();
		((Collider)m_CharacterController).enabled = false;
		LookAtPlayer();
		m_OriginWalkSpeed = m_WalkSpeed;
		m_OriginRunSpeed = m_RunSpeed;
		if (Object.op_Implicit((Object)(object)m_LookAroundClip))
		{
			((Component)m_AnimationController).gameObject.AddComponent<AnimationEventController>();
			AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_LookAroundClip).name, "ResetMovement", 76);
		}
		if (Object.op_Implicit((Object)(object)m_AudioProxy))
		{
			m_AudioSwitch = ((Component)m_AudioProxy).GetComponentInChildren<S13Switch>();
		}
	}

	protected void Update()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (m_WaitTimer <= 0f)
		{
			if (m_WaitCallback != null)
			{
				m_WaitCallback();
				m_WaitCallback = null;
			}
		}
		else
		{
			m_WaitTimer -= Time.deltaTime;
		}
		m_AnimatedHeadPosition = m_HeadLookForward.forward;
	}

	protected void LateUpdate()
	{
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		if (m_WaitTimer <= 0f)
		{
			SolveMovement();
		}
		if (!m_IsCowering && !m_IsInVent && !m_IsAnimating)
		{
			bool flag = Object.op_Implicit((Object)(object)m_Target) && m_AnimationController.GetInteger("MovementMode") < 1;
			float num = 0f;
			if (flag && Object.op_Implicit((Object)(object)m_Target))
			{
				num = Vector3.Angle(base.transform.forward, m_Target.position - base.transform.position);
			}
			if (flag && Object.op_Implicit((Object)(object)m_Target) && Vector3.Distance(m_Target.position, base.transform.position) < 15f && num < 70f)
			{
				Vector3 val = m_Target.position - m_Head.position;
				Quaternion val2 = Quaternion.LookRotation(val) * Quaternion.Euler(m_HeadOffset);
				m_Head.rotation = Quaternion.Slerp(m_LastLookRotation, val2, 5f * Time.deltaTime);
			}
			else
			{
				Quaternion val3 = Quaternion.LookRotation(m_AnimatedHeadPosition) * Quaternion.Euler(m_HeadOffset);
				m_Head.rotation = Quaternion.Slerp(m_LastLookRotation, val3, 5f * Time.deltaTime);
			}
			m_LastLookRotation = m_Head.rotation;
		}
	}

	public void UpdateWaypointList(List<WaypointNode> waypointList, bool isRunning = false)
	{
		m_CurrentWaypointList.Clear();
		m_UseRun = isRunning;
		m_WaypointIndex = 0;
		m_CurrentWaypointList = waypointList;
	}

	public void AddToWaypointList(List<WaypointNode> waypointList, bool isRunning = false)
	{
		m_UseRun = isRunning;
		for (int i = 0; i < waypointList.Count; i++)
		{
			m_CurrentWaypointList.Add(waypointList[i]);
		}
	}

	private void PauseForAnimation(float AnimLength)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		m_IsAnimating = true;
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), AnimLength, (TweenCallback)delegate
		{
			if (m_IsCowering)
			{
				m_AnimationController.SetBool("Cower", true);
			}
			m_IsAnimating = false;
		});
	}

	private void SolveMovement()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		if (m_CurrentWaypointList == null || m_CurrentWaypointList.Count == 0 || m_IsAnimating || m_IsSitting)
		{
			return;
		}
		if (m_IsSlow)
		{
			if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > m_CowerApproachDistance)
			{
				SetCower(active: true);
				m_CowerApproachDistance = 7f;
				return;
			}
			SetCower(active: false);
			m_CowerApproachDistance = 10f;
		}
		if (m_WaypointIndex >= m_CurrentWaypointList.Count)
		{
			if (m_CurrentWaypointList.Count > 0)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, m_CurrentWaypointList[m_CurrentWaypointList.Count - 1].transform.rotation, 6f * Time.deltaTime);
				base.transform.position = Vector3.MoveTowards(base.transform.position, m_CurrentWaypointList[m_CurrentWaypointList.Count - 1].transform.position, m_WalkSpeed * Time.deltaTime);
			}
			if (Vector3.Distance(base.transform.position, m_CurrentWaypointList[m_CurrentWaypointList.Count - 1].transform.position) < 0.2f)
			{
				m_AnimationController.SetInteger("MovementMode", 0);
				SendOnWaypointComplete();
			}
		}
		else if (!m_IsSlow && !m_UseRun && Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) < 4f)
		{
			m_AnimationController.SetInteger("MovementMode", 0);
		}
		else if (Vector3.Distance(base.transform.position, m_CurrentWaypointList[m_WaypointIndex].transform.position) > 1.5f)
		{
			float num = m_WalkSpeed;
			if (m_IsSlow)
			{
				m_AnimationController.SetInteger("MovementMode", 2);
			}
			else if (m_UseRun)
			{
				m_AnimationController.SetInteger("MovementMode", 3);
				num = m_RunSpeed;
			}
			else
			{
				m_AnimationController.SetInteger("MovementMode", 1);
			}
			Vector3 val = m_CurrentWaypointList[m_WaypointIndex].transform.position - base.transform.position;
			val.y = 0f;
			Quaternion val2 = Quaternion.LookRotation(val);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, val2, 3f * Time.deltaTime);
			m_CharacterController.Move(base.transform.forward * num * Time.deltaTime);
			Vector3 velocity = m_CharacterController.velocity;
			if (((Vector3)(ref velocity)).magnitude < 2f)
			{
				m_AnimationController.SetInteger("MovementMode", 0);
			}
			m_CharacterController.Move(Vector3.down * 10f * Time.fixedDeltaTime);
		}
		else
		{
			m_WaypointIndex++;
		}
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsCowering)
		{
			m_AnimationController.SetTrigger("Hit");
		}
		m_AudioSwitch.Play("hit");
		GameObject fromPool = GameManager.Instance.PoolingManager.GetFromPool("GamePlay/Particles/InkHit");
		fromPool.transform.position = ((RaycastHit)(ref hit)).point;
		fromPool.transform.localScale = Vector3.one * 0.2f;
		DoWait(0.5f);
	}

	protected void SendOnWaypointComplete()
	{
		m_CurrentWaypointList.Clear();
		m_WaypointIndex = 0;
		this.OnWaypointComplete.Send(this);
	}

	protected void DoWait(float timer, Action callback = null)
	{
		m_WaitCallback = callback;
		m_WaitTimer = timer;
	}

	public void SetDancing(bool active)
	{
		m_AnimationController.SetBool("IsDancing", active);
	}

	public void SetCower(bool active)
	{
		int num = (active ? 2 : 0);
		m_IsCowering = active;
		m_AnimationController.SetBool("Cower", active);
		m_AnimationController.SetInteger("AnimationMode", num);
	}

	public void LookAround()
	{
		m_IsAnimating = true;
		m_AnimationController.SetBool("Cower", false);
		m_AnimationController.SetTrigger("LookAround");
		PauseForAnimation(m_LookAroundClip.length);
	}

	public void ResetMovement()
	{
		m_IsAnimating = false;
		m_AnimationController.SetTrigger("Reset");
	}

	public void SlowSpeeds()
	{
		m_IsSlow = true;
		m_RunSpeed = 6f;
		m_WalkSpeed = 3f;
	}

	public void ResetSpeeds()
	{
		m_IsSlow = false;
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
	}

	public void ResetAll()
	{
		m_IsInVent = false;
		((Collider)m_CharacterController).enabled = true;
		m_AnimationController.SetInteger("AnimationMode", 0);
	}

	public void IsAlreadyActive()
	{
		m_AnimationController.SetBool("IsActive", true);
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
			val.AddEvent(AddEvent("PlaceToolboxEvent", (float)Math.PI * 113f / 150f));
			val.AddEvent(AddEvent("LookAtPlayer", 3.5f));
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

	public void PlaceToolboxEvent()
	{
		PlaceToolbox();
	}

	public void PlaceToolbox(bool isSilent = false)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		m_Toolbox.SetParent(m_ToolboxParent);
		m_Toolbox.position = m_Table.position;
		m_Toolbox.eulerAngles = m_Table.eulerAngles;
		if (!isSilent)
		{
			GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/CH3/SFX_CH3_borisplacetoolbox", m_Toolbox.position);
			this.OnToolboxPlaced.Send(this);
		}
	}

	public void ForceStand()
	{
		m_AnimationController.SetBool("IsActive", true);
		m_IsSitting = false;
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
		((Collider)m_CharacterController).enabled = true;
	}

	public void StopWaypointPathing()
	{
		UpdateWaypointList(new List<WaypointNode>());
		m_AnimationController.SetInteger("MovementMode", 0);
	}

	public void LookAtTarget(Transform target)
	{
		m_Target = target;
	}

	public void LookAtPlayer()
	{
		LookAtTarget(GameManager.Instance.GameCamera.transform);
	}

	public void StopLooking()
	{
		m_Target = null;
	}

	private AudioObject PlayAudio(ref AudioClip[] audioClips, bool is2D = false)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (audioClips == null || audioClips.Length <= 0)
		{
			return null;
		}
		AudioObject audioObject = null;
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		audioObject = ((!is2D) ? GameManager.Instance.AudioManager.PlayAtPosition(val, base.transform.position) : GameManager.Instance.AudioManager.Play(val));
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
		return audioObject;
	}
}
