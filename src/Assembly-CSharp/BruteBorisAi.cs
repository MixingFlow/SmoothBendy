using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class BruteBorisAi : TMGMonoBehaviour
{
	public enum BorisThought
	{
		NONE,
		PHASE1,
		PHASE2,
		PHASE3,
		SMASH
	}

	[Header("AI References")]
	[SerializeField]
	private Transform m_KneeTransform;

	[SerializeField]
	private LayerMask m_ObstructionLayers;

	[Header("General References")]
	[SerializeField]
	private Animator m_AnimationController;

	[SerializeField]
	private float m_CameraShakeDistance = 30f;

	[SerializeField]
	private float m_GravityMiultiplier;

	[Header("Attack")]
	[SerializeField]
	private WeaponInfo m_FistWeaponInfo;

	[SerializeField]
	private LayerMask m_AttackableLayers;

	[SerializeField]
	private ParticleSystem m_dustCloud;

	[Header("Guts")]
	[SerializeField]
	private Collider m_GutsCollider;

	[SerializeField]
	private ParticleSystem[] m_HitInk;

	[Header("Reveal Objects")]
	[SerializeField]
	private Animator m_CartRevealAnimator;

	[SerializeField]
	private Transform m_RevealPosition;

	[SerializeField]
	private Transform m_CartParentTransform;

	[SerializeField]
	private Transform m_PlayerLandPoint;

	[SerializeField]
	private Transform m_BorisWarpPoint;

	[SerializeField]
	private DestructibleObject m_CartDestroy;

	[Header("[==PHASE 1==]")]
	[SerializeField]
	private float m_ChargeSpeed = 20f;

	[SerializeField]
	private float m_ChargeCooldown = 5f;

	[Header("[==PHASE 2==]")]
	[SerializeField]
	private float m_JumpForwardForce = 20f;

	[SerializeField]
	private float m_JumpPower = 30f;

	[SerializeField]
	private float m_JumpCooldown = 5f;

	[Header("[==PHASE 3==]")]
	[SerializeField]
	private GameObject m_ThrowableCartObject;

	[SerializeField]
	private Transform m_ThrowLocation;

	[SerializeField]
	private Transform m_GrabLocation;

	[SerializeField]
	private Transform m_GrabHand;

	[SerializeField]
	private Transform[] m_CartPath;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClip m_Anim_Walk;

	[SerializeField]
	private AnimationClip m_Anim_BattleDeath;

	[SerializeField]
	private AnimationClip m_Anim_Reveal;

	[SerializeField]
	private AnimationClip m_Anim_Attack1;

	[SerializeField]
	private AnimationClip m_Anim_Attack2;

	[SerializeField]
	private AnimationClip m_Anim_Charge;

	[SerializeField]
	private AnimationClip m_Anim_JumpStart;

	[SerializeField]
	private AnimationClip m_Anim_GroundPound;

	[SerializeField]
	private AnimationClip m_Anim_PickupCart;

	[SerializeField]
	private AnimationClip m_Anim_ThrowCart;

	[SerializeField]
	private AnimationClip m_Anim_ThrowWalk;

	[Header("Doors")]
	[SerializeField]
	private Transform m_SmashDoorPosition;

	[Header("Easter Egg Objects")]
	[SerializeField]
	private GameObject m_Bone;

	private CharacterController m_CharacterController;

	private Transform m_MoveToTargetTransform;

	private Transform m_CartObjectToParent;

	private bool m_DoingSpecialAttack;

	private float m_SpecialAttackCooldown = 5f;

	private int m_SpecialAttackCount;

	private bool m_DidJump;

	private float m_ChargeSafetyTimer;

	public bool CanSpawnInk = true;

	private bool m_IsTired;

	private Sequence m_InkSequence;

	private S13Switch m_AudioSwitch;

	private float m_AvoidanceCooldownTime = 0.5f;

	private float m_AvoidanceDistance = 4f;

	private Vector3 m_TrackObjectPosition;

	protected Action m_WaitCallback;

	private float m_WaitTimer;

	private bool m_IsCartPickedUp;

	private GameObject m_ActiveCart;

	private GameObject m_GrabbedObject;

	private bool m_CartRestocked = true;

	private Vector3 m_MoveDir;

	private Vector3 m_GravityPower;

	private BorisThought CurrentPhase = BorisThought.PHASE1;

	private BorisThought CurrentThought;

	private BorisThought PreviousThought;

	private float m_TurnSpeed = 5f;

	private bool m_IsDoingPickupAnimation;

	private Sequence m_CartRestockSequence;

	private bool m_HasSmashedDoor;

	private bool m_LockPhysics;

	private AudioClip m_RumbleClip;

	private AudioObject m_RumbleAudio;

	public Transform CartParent => m_CartParentTransform;

	public event EventHandler OnBegin;

	public event EventHandler OnCartSmashed;

	public event EventHandler OnDoorSmashed;

	public event EventHandler OnTired;

	public event EventHandler OnHit;

	public event EventHandler OnDeath;

	public event EventHandler OnComplete;

	public override void Init()
	{
		base.Init();
		m_CharacterController = ((Component)this).GetComponent<CharacterController>();
	}

	public override void InitOnComplete()
	{
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		GameManager.Instance.CharacterManager.BruteBoris = this;
		m_Bone.SetActive(false);
		m_ThrowableCartObject.SetActive(false);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Walk).name, "Stomp", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Walk).name, "Stomp", 20);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_BattleDeath).name, "ShakeCamera", 117);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Charge).name, "Stomp", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Charge).name, "Stomp", 19);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_ThrowWalk).name, "Stomp", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_ThrowWalk).name, "Stomp", 20);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Attack1).name, "Attack1", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Attack2).name, "Attack2", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Attack1).name, "DoAttack", 15);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Attack2).name, "DoAttack", 15);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_JumpStart).name, "DoJump", 22);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_GroundPound).name, "DoSmash", 30);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_GroundPound).name, "PlaySmashAudio", 0);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_PickupCart).name, "PickupCart", 30);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_ThrowCart).name, "ThrowCart", 26);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Reveal).name, "RevealGrabCart", 15);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Reveal).name, "HenryDialogue", 80);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Reveal).name, "UnlockPlayer", 430);
		m_LockPhysics = true;
		base.transform.position = m_RevealPosition.position + m_RevealPosition.forward * -5f;
		base.transform.rotation = m_RevealPosition.rotation;
		m_GutsCollider.enabled = false;
		m_RumbleClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Rumble_Loop_01");
		m_AudioSwitch = ((Component)this).GetComponentInChildren<S13Switch>();
	}

	public void DoRevealSequence()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		bool active = false;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data != null)
		{
			active = GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HasBorisBone;
		}
		m_Bone.SetActive(active);
		m_ThrowableCartObject.SetActive(true);
		GameManager.Instance.Player.transform.SetParent((Transform)null);
		GameManager.Instance.Player.GoToAndLookAt(m_PlayerLandPoint);
		base.transform.position = m_RevealPosition.position;
		m_AnimationController.speed = 1f;
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_Anim_Reveal).name, "Reveal", 0);
		m_AnimationController.SetTrigger("Reveal");
		m_CartRevealAnimator.SetTrigger("Reveal");
		m_LockPhysics = true;
		DoWait(15f, delegate
		{
			SetThought(BorisThought.PHASE1);
		});
	}

	public void Reset()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		m_AnimationController.SetTrigger("Reset");
		m_DoingSpecialAttack = false;
		m_SpecialAttackCooldown = 5f;
		m_SpecialAttackCount = 0;
		m_DidJump = false;
		m_IsTired = false;
		CanSpawnInk = true;
		m_AvoidanceCooldownTime = 0.5f;
		m_TrackObjectPosition = Vector3.zero;
		m_WaitCallback = null;
		m_WaitTimer = 0f;
		m_IsCartPickedUp = false;
		m_MoveDir = Vector3.zero;
		m_GravityPower = Vector3.zero;
		CurrentPhase = BorisThought.PHASE1;
		CurrentThought = BorisThought.NONE;
		m_IsDoingPickupAnimation = false;
		m_CartRestocked = true;
		m_HasSmashedDoor = false;
		m_ThrowableCartObject.transform.localPosition = m_CartPath[0].localPosition;
		if (Object.op_Implicit((Object)(object)m_GrabbedObject))
		{
			Object.Destroy((Object)(object)m_GrabbedObject);
		}
		if (Object.op_Implicit((Object)(object)m_ActiveCart))
		{
			ShortcutExtensions.DOKill((Component)(object)m_ActiveCart.transform, false);
			Object.Destroy((Object)(object)m_ActiveCart);
			m_ActiveCart = null;
		}
	}

	public void WarpToStartLocation()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.transform.position = m_BorisWarpPoint.position;
		base.transform.eulerAngles = m_BorisWarpPoint.eulerAngles;
	}

	public void SetCartActive(bool active)
	{
		((Component)m_CartParentTransform).gameObject.SetActive(active);
	}

	public void SetCartParent(bool Unparent = false)
	{
		if (Unparent)
		{
			m_CartObjectToParent.SetParent((Transform)null);
		}
		else if (Object.op_Implicit((Object)(object)m_CartObjectToParent))
		{
			m_CartObjectToParent.SetParent(m_CartParentTransform);
		}
	}

	public void Reveal()
	{
		m_AudioSwitch.Play("reveal");
	}

	public void UnlockPlayer()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		this.OnBegin.Send(this);
		Transform freeRoamCam = GameManager.Instance.GameCamera.FreeRoamCam;
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			freeRoamCam.SetParent((Transform)null);
			m_CartDestroy.transform.position = m_CartParentTransform.position;
			m_CartDestroy.Destroy(m_CartDestroy.transform.position, 30f, 15f, 2f);
			S13AudioManager.Instance.InvokeEvent("evt_haunted_house_cart_smashed");
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOMoveX(freeRoamCam, m_PlayerLandPoint.position.x, 1f, false));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOMoveZ(freeRoamCam, m_PlayerLandPoint.position.z, 1f, false));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOMoveY(freeRoamCam, m_PlayerLandPoint.position.y - 2f, 1f, false));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DORotate(freeRoamCam, new Vector3(-90f, 0f, -90f), 1f, (RotateMode)0));
		TweenSettingsExtensions.InsertCallback(val, 1f, (TweenCallback)delegate
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			base.transform.position = m_BorisWarpPoint.position;
		});
		num += 1.5f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(freeRoamCam, m_PlayerLandPoint.eulerAngles, 1f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(freeRoamCam, GameManager.Instance.Player.HeadContainer.position, 1f, false), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(val, (TweenCallback)delegate
		{
			this.OnCartSmashed.Send(this);
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
			GameManager.Instance.Player.SetLock(active: false);
			if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
			{
				GameManager.Instance.Player.WeaponGameObject.SetActive(true);
			}
		});
	}

	public void SetTarget(Transform NewTarget)
	{
		m_MoveToTargetTransform = NewTarget;
	}

	public void SetThought(BorisThought Thought)
	{
		PreviousThought = CurrentThought;
		CurrentThought = Thought;
	}

	private void SetTired()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsTired)
		{
			this.OnTired.Send(this);
			m_TurnSpeed = 8f;
			m_IsTired = true;
			m_AnimationController.SetInteger("MovementMode", 0);
			m_AnimationController.SetBool("Tired", true);
			m_GutsCollider.enabled = true;
			SetTarget(null);
			if (CurrentThought != BorisThought.PHASE3)
			{
				m_TrackObjectPosition = GameManager.Instance.Player.transform.position;
			}
			else
			{
				m_TrackObjectPosition = m_ThrowLocation.position + m_ThrowLocation.forward * 10f;
			}
			PreviousThought = CurrentThought;
			SetThought(BorisThought.NONE);
			SpawnInkSequence();
		}
	}

	private void Phase1()
	{
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		m_LockPhysics = false;
		m_TurnSpeed = 5f;
		if (m_DoingSpecialAttack)
		{
			SetTarget(null);
			m_TrackObjectPosition = base.transform.position + base.transform.forward * 5f;
			m_MoveDir = base.transform.forward * m_ChargeSpeed * Time.fixedDeltaTime;
			m_SpecialAttackCooldown = m_ChargeCooldown;
			m_ChargeSafetyTimer += Time.deltaTime;
			if (Physics.CheckSphere(m_KneeTransform.position + base.transform.forward, 1.4f, LayerMask.op_Implicit(m_AttackableLayers)))
			{
				m_AnimationController.SetTrigger("ChargeThrough");
				AttackTarget(base.transform.forward * 4f, 2f);
				m_AnimationController.SetInteger("ChargeAnim", Random.Range(0, 3));
			}
			else
			{
				if (!Physics.CheckSphere(m_KneeTransform.position + Vector3.up + base.transform.forward * 3f, 2f, LayerMask.op_Implicit(m_ObstructionLayers) + LayerMask.GetMask(new string[1] { "Player" })) && !(m_ChargeSafetyTimer > 6f))
				{
					return;
				}
				m_AnimationController.SetTrigger("ChargeEnd");
				m_DoingSpecialAttack = false;
				m_ChargeSafetyTimer = 0f;
				AttackTarget(base.transform.forward * 4f);
				DoWait(1.6f, delegate
				{
					m_SpecialAttackCount++;
					if (m_SpecialAttackCount >= 3)
					{
						SetTired();
					}
				});
			}
			return;
		}
		m_AnimationController.ResetTrigger("ChargeEnd");
		SetTarget(GameManager.Instance.Player.transform);
		if (Vector3.Distance(base.transform.position, m_MoveToTargetTransform.position) > 12f && m_SpecialAttackCooldown <= 0f)
		{
			if (Vector3.Angle(base.transform.forward, m_MoveToTargetTransform.position - base.transform.position) < 15f)
			{
				m_AnimationController.SetInteger("MovementMode", 0);
				m_AnimationController.SetTrigger("ChargeStart");
				m_AudioSwitch.Play("charge");
				m_DoingSpecialAttack = true;
				DoWait(2f, delegate
				{
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					m_TrackObjectPosition = GameManager.Instance.Player.transform.position;
					base.transform.rotation = FaceDirection(isSmooth: false);
				});
			}
			else
			{
				WalkAndPunch();
			}
		}
		else
		{
			WalkAndPunch();
		}
	}

	private void Phase2()
	{
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		m_LockPhysics = false;
		m_TurnSpeed = 5f;
		float num = m_JumpForwardForce;
		if (m_DoingSpecialAttack)
		{
			if (m_DidJump)
			{
				SetTarget(null);
				if (!m_HasSmashedDoor)
				{
					m_TrackObjectPosition = m_SmashDoorPosition.position;
					num = m_JumpForwardForce * 1.3f;
				}
				else
				{
					m_TrackObjectPosition = GameManager.Instance.Player.transform.position;
				}
				if (!m_CharacterController.isGrounded)
				{
					m_MoveDir = base.transform.forward * num * Vector3.Distance(base.transform.position, m_TrackObjectPosition) * Time.fixedDeltaTime;
				}
				else if (m_GravityPower.y < 0f)
				{
					m_dustCloud.Emit(30);
					AttackTarget(Vector3.zero, 10f);
					m_DoingSpecialAttack = false;
					m_AnimationController.SetTrigger("JumpEnd");
					m_DidJump = false;
					m_AudioSwitch.Play("land");
					if (!m_HasSmashedDoor)
					{
						this.OnDoorSmashed.Send(this);
						m_HasSmashedDoor = true;
					}
					ApplyShake(2f);
					DoWait(1f, delegate
					{
						m_SpecialAttackCount++;
						if (m_SpecialAttackCount >= 3)
						{
							SetTired();
						}
					});
				}
			}
			m_AnimationController.SetInteger("MovementMode", 0);
			m_SpecialAttackCooldown = m_JumpCooldown;
		}
		else
		{
			m_AnimationController.ResetTrigger("JumpEnd");
			SetTarget(GameManager.Instance.Player.transform);
			if (Vector3.Distance(base.transform.position, m_MoveToTargetTransform.position) > 12f && m_SpecialAttackCooldown <= 0f)
			{
				m_AnimationController.SetInteger("MovementMode", 0);
				m_AnimationController.SetTrigger("JumpStart");
				m_DoingSpecialAttack = true;
			}
			else
			{
				WalkAndPunch();
			}
		}
	}

	public void DoJump()
	{
		m_DidJump = true;
		Jump(m_JumpPower);
		m_AudioSwitch.Play("jump");
	}

	private void Phase3()
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		m_LockPhysics = false;
		m_TurnSpeed = 5f;
		SetTarget(null);
		if (m_IsCartPickedUp)
		{
			if (Vector3.Distance(base.transform.position, m_ThrowLocation.position) > 2f)
			{
				WalkAndPunch(canPunch: false);
				return;
			}
			m_TrackObjectPosition = GameManager.Instance.Player.transform.position;
			FaceDirection();
			if (!m_IsDoingPickupAnimation)
			{
				m_IsDoingPickupAnimation = true;
				base.transform.position = m_ThrowLocation.position;
				m_AnimationController.ResetTrigger("PickupCart");
				m_AnimationController.SetInteger("MovementMode", 0);
				m_AnimationController.SetTrigger("ThrowCart");
				m_GrabbedObject.GetComponentInChildren<S13Switch>().Play("toss");
				DoCartRestockSequence();
				DoWait(2f);
			}
			return;
		}
		if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) < 5f)
		{
			if (!m_IsDoingPickupAnimation)
			{
				m_TrackObjectPosition = GameManager.Instance.Player.transform.position;
				WalkAndPunch();
			}
			return;
		}
		if (Vector3.Distance(base.transform.position, m_GrabLocation.position) > 2f || !m_CartRestocked)
		{
			m_TrackObjectPosition = m_GrabLocation.position;
			WalkAndPunch();
			return;
		}
		base.transform.position = m_GrabLocation.position;
		m_TrackObjectPosition = m_GrabLocation.position + m_GrabLocation.forward * 5f;
		if (!m_IsDoingPickupAnimation)
		{
			m_IsDoingPickupAnimation = true;
			m_AnimationController.ResetTrigger("ThrowCart");
			m_AnimationController.SetInteger("MovementMode", 0);
			m_AnimationController.SetTrigger("PickupCart");
		}
		else if (Object.op_Implicit((Object)(object)m_GrabbedObject) && m_CartRestocked)
		{
			m_GrabbedObject.transform.SetParent(m_GrabHand);
			m_GrabbedObject.transform.localPosition = new Vector3(0f, 2.7f, 4f);
			m_GrabbedObject.transform.localEulerAngles = new Vector3(178f, 124f, -5.2f);
			m_GrabbedObject.GetComponentInChildren<S13Switch>().Play("pickup");
			if (!m_IsCartPickedUp)
			{
				DoCartRestockSequence();
				m_TrackObjectPosition = m_ThrowLocation.position;
				m_IsCartPickedUp = true;
				m_IsDoingPickupAnimation = false;
			}
		}
	}

	public void PickupCart()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		m_GrabbedObject = Object.Instantiate<GameObject>(m_ActiveCart);
		m_GrabbedObject.transform.position = m_ActiveCart.transform.position;
		m_GrabbedObject.transform.eulerAngles = m_ActiveCart.transform.eulerAngles;
		Object.Destroy((Object)(object)m_ActiveCart);
		m_ActiveCart = null;
	}

	public void ThrowCart()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		m_IsCartPickedUp = false;
		m_GrabbedObject.transform.SetParent((Transform)null);
		Rigidbody component = m_GrabbedObject.GetComponent<Rigidbody>();
		ThrowableObject component2 = m_GrabbedObject.GetComponent<ThrowableObject>();
		component.isKinematic = false;
		component2.Initialize(component2.WeaponInfo, base.transform.forward * 50f + Vector3.up * 10f);
		component2.Throw();
		m_GrabbedObject = null;
		DoWait(1f, delegate
		{
			m_SpecialAttackCount++;
			m_IsDoingPickupAnimation = false;
			if (m_SpecialAttackCount >= 3)
			{
				SetTired();
			}
		});
	}

	public void HandleOnCartDestroyed(object sender, EventArgs e)
	{
		m_ActiveCart = null;
		DoCartRestockSequence();
	}

	private void DoCartRestockSequence()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		if (!Object.op_Implicit((Object)(object)m_ActiveCart))
		{
			m_ActiveCart = Object.Instantiate<GameObject>(m_ThrowableCartObject);
			m_ActiveCart.GetComponent<EnemyHittableObject>().OnDestroyEvent += HandleOnCartDestroyed;
			m_ActiveCart.transform.SetParent(m_ThrowableCartObject.transform.parent);
			m_ActiveCart.transform.localPosition = m_CartPath[0].localPosition;
			m_CartRestocked = false;
			if (m_CartRestockSequence != null)
			{
				TweenExtensions.Kill((Tween)(object)m_CartRestockSequence, false);
				m_CartRestockSequence = null;
			}
			m_CartRestockSequence = DOTween.Sequence();
			float num = 1f;
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < m_CartPath.Length; i++)
			{
				list.Add(m_CartPath[i].localPosition);
			}
			TweenSettingsExtensions.Insert(m_CartRestockSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Path, PathOptions>>(TweenSettingsExtensions.SetLookAt(ShortcutExtensions.DOLocalPath(m_ActiveCart.transform, list.ToArray(), 10f, (PathType)1, (PathMode)1, 10, (Color?)null), 0.01f, (Vector3?)(-m_ActiveCart.transform.right), (Vector3?)null), (Ease)1));
			TweenSettingsExtensions.OnComplete<Sequence>(m_CartRestockSequence, (TweenCallback)delegate
			{
				m_CartRestocked = true;
			});
		}
	}

	private void WalkAndPunch(bool canPunch = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (Physics.CheckSphere(((Component)m_AnimationController).transform.position, 3f, LayerMask.op_Implicit(m_AttackableLayers)) && canPunch)
		{
			m_AnimationController.SetInteger("MovementMode", 0);
			int num = Random.Range(1, 3);
			m_AnimationController.SetTrigger("Attack_" + num);
			DoWait(1f);
		}
		else if (Vector3.Distance(m_TrackObjectPosition, base.transform.position) > 2f)
		{
			m_MoveDir = base.transform.forward * 5f * Time.fixedDeltaTime;
			m_AnimationController.SetInteger("MovementMode", 1);
		}
		else
		{
			m_AnimationController.SetInteger("MovementMode", 0);
		}
	}

	private void Update()
	{
		if (GameManager.Instance.isPaused)
		{
			return;
		}
		if (m_WaitTimer <= 0f && !m_IsTired)
		{
			if (CurrentThought == BorisThought.PHASE1)
			{
				Phase1();
			}
			else if (CurrentThought == BorisThought.PHASE2)
			{
				Phase2();
			}
			else if (CurrentThought == BorisThought.PHASE3)
			{
				Phase3();
			}
		}
		if (m_SpecialAttackCooldown > 0f && CurrentThought != BorisThought.NONE && !m_IsTired && m_WaitTimer <= 0f)
		{
			m_SpecialAttackCooldown -= Time.deltaTime;
		}
		if (m_WaitTimer <= 0f)
		{
			if (m_WaitCallback != null)
			{
				Action waitCallback = m_WaitCallback;
				m_WaitTimer = 0f;
				m_WaitCallback = null;
				waitCallback();
			}
		}
		else
		{
			m_WaitTimer -= Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (m_LockPhysics)
		{
			return;
		}
		if (m_GravityMiultiplier < 10f)
		{
			m_GravityPower += Vector3.down * m_GravityMiultiplier * Time.fixedDeltaTime;
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
		base.transform.rotation = FaceDirection();
		m_CharacterController.Move(m_MoveDir + m_GravityPower);
		m_MoveDir = Vector3.zero;
	}

	private void Jump(float JumpPower)
	{
		m_GravityPower.y = JumpPower;
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

	public void AttackTarget(Vector3 _AttackPosition, float _AttackRadious = 4f, bool NoDamage = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		Collider[] array = Physics.OverlapSphere(m_KneeTransform.position + _AttackPosition, _AttackRadious, LayerMask.op_Implicit(m_AttackableLayers));
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
				for (int j = 0; j < m_FistWeaponInfo.Damage; j++)
				{
					GameManager.Instance.ShowHurtBorder(isSilent: true);
				}
			}
			S13AudioManager.Instance.InvokeEvent("evt_player_hit_by_boris");
		}
	}

	private void AvoidObstacle(Vector3 targetPosition)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (m_AvoidanceCooldownTime <= 0f)
		{
			Vector3 position = m_KneeTransform.position;
			Vector3 val = m_TrackObjectPosition + Vector3.up - base.transform.position;
			RaycastHit val2 = default(RaycastHit);
			if (!Physics.Raycast(position, ((Vector3)(ref val)).normalized, ref val2, m_AvoidanceDistance * 2f, LayerMask.op_Implicit(m_ObstructionLayers)))
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			Debug.DrawLine(m_KneeTransform.position, m_TrackObjectPosition + Vector3.up, Color.yellow, 2f);
			Vector3 val3 = default(Vector3);
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if ((i != 0 || j != 0) && Mathf.Abs(i) != Mathf.Abs(j))
					{
						Vector3 position2 = m_KneeTransform.position;
						((Vector3)(ref val3))._002Ector((float)i, 0f, (float)j);
						Vector3 val4 = position2 + ((Vector3)(ref val3)).normalized * m_AvoidanceDistance;
						if (!Physics.Linecast(m_KneeTransform.position, val4, LayerMask.op_Implicit(m_ObstructionLayers)))
						{
							list.Add(val4);
							Debug.DrawLine(m_KneeTransform.position, val4, Color.cyan, 1f);
						}
					}
				}
			}
			float num = -1f;
			Vector3 trackObjectPosition = Vector3.zero;
			for (int k = 0; k < list.Count; k++)
			{
				float num2 = Vector3.Distance(targetPosition, list[k]);
				if (k == 0 || num2 < num)
				{
					num = num2;
					trackObjectPosition = list[k];
				}
			}
			m_TrackObjectPosition = trackObjectPosition;
			m_AvoidanceCooldownTime = 1f;
		}
		else
		{
			m_AvoidanceCooldownTime -= Time.deltaTime;
		}
	}

	public void Hit(bool isBaconSoup)
	{
		this.OnHit.Send(this);
		if (CurrentPhase != BorisThought.PHASE3)
		{
			Hurt();
		}
		else
		{
			Death(isBaconSoup);
		}
	}

	private void Hurt()
	{
		ResetTiredBoris();
		m_AnimationController.SetTrigger("Hit");
		m_AudioSwitch.Play("hit");
		ActivateParticles(1f);
		SpawnInkSequence();
		m_SpecialAttackCooldown = 5f;
		m_SpecialAttackCount = 0;
		m_GutsCollider.enabled = false;
		DoWait(2f, delegate
		{
			SetThought(CurrentPhase);
			PreviousThought = CurrentPhase;
		});
		m_AnimationController.SetTrigger("GroundPound");
		if (CurrentPhase == BorisThought.PHASE1)
		{
			CurrentPhase = BorisThought.PHASE2;
			m_SpecialAttackCooldown = 2f;
		}
		else if (CurrentPhase == BorisThought.PHASE2)
		{
			CurrentPhase = BorisThought.PHASE3;
			DoCartRestockSequence();
		}
	}

	private void Death(bool isBaconSoup)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Expected O, but got Unknown
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		if (Object.op_Implicit((Object)(object)m_CharacterController))
		{
			((Collider)m_CharacterController).enabled = false;
		}
		SetThought(BorisThought.NONE);
		base.transform.rotation = m_ThrowLocation.rotation;
		base.transform.position = m_ThrowLocation.position;
		if (isBaconSoup && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[5] != -1)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[5] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[5] * 414;
			bool flag = false;
			if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[0] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[0] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[1] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[1] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[2] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[2] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[3] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[3] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[4] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[4] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[5] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[5] * 414)
			{
				GameManager.Instance.GameData.CurrentSaveFile.Internecions[3] = 4;
				flag = true;
			}
			GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
			if (flag)
			{
				GameManager.Instance.GameCamera.VisionEffect.BeginEffect();
				TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
				{
					GameManager.Instance.GameCamera.VisionEffect.EndEffect();
				});
				m_RumbleAudio = GameManager.Instance.AudioManager.Play(m_RumbleClip, AudioObjectType.SOUND_EFFECT, -1);
				TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 5f, 0.1f, 15, 90f, false, false), new TweenCallback(ScreenRumbleOnComplete));
			}
		}
		this.OnDeath.Send(this);
		m_Bone.SetActive(false);
		m_LockPhysics = true;
		m_GutsCollider.enabled = false;
		ActivateParticles(6f);
		m_AnimationController.SetBool("Tired", false);
		m_AnimationController.SetTrigger("Dead");
		m_AudioSwitch.Play("battle_fall");
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 3.8f, new TweenCallback(ActualDeath));
	}

	private void ScreenRumbleOnComplete()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		ShortcutExtensions.DOLocalMove(GameManager.Instance.GameCamera.transform, Vector3.zero, 0.5f, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_RumbleAudio.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
		{
			if ((Object)(object)m_RumbleAudio != (Object)null)
			{
				m_RumbleAudio.Clear();
				m_RumbleAudio = null;
			}
		});
	}

	private void ActualDeath()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		base.transform.rotation = m_ThrowLocation.rotation;
		base.transform.position = m_ThrowLocation.position;
		DisableParticles();
		m_dustCloud.Emit(30);
		AttackTarget(-base.transform.forward * 4f, 10f, NoDamage: true);
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.4f, 1.2f, 18, 90f, false, true), (Ease)1), (TweenCallback)delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		});
		this.OnComplete.Send(this);
	}

	private void ResetTiredBoris()
	{
		m_AnimationController.SetBool("Tired", false);
		m_IsTired = false;
		m_GutsCollider.enabled = false;
		SetThought(PreviousThought);
	}

	public void DoSmash()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		m_dustCloud.Emit(30);
		if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) < 10f)
		{
			GameManager.Instance.Player.AddForce((-base.transform.right + base.transform.forward + Vector3.up * 0.5f) * 25f);
			ApplyShake(2f);
		}
		DoWait(1f, delegate
		{
			CurrentThought = CurrentPhase;
		});
	}

	private void SpawnInkSequence()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		ActivateParticles(3f);
		m_AudioSwitch.Play("gush");
		TweenExtensions.Kill((Tween)(object)m_InkSequence, false);
		m_InkSequence = DOTween.Sequence();
		float num = 5f;
		TweenSettingsExtensions.InsertCallback(m_InkSequence, num, new TweenCallback(ResetTiredBoris));
	}

	private void ActivateParticles(float duration)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_HitInk.Length; i++)
		{
			ParticleSystem val = m_HitInk[i];
			MainModule main = val.main;
			val.Stop();
			((MainModule)(ref main)).duration = duration;
			val.Play();
		}
	}

	private void DisableParticles()
	{
		for (int i = 0; i < m_HitInk.Length; i++)
		{
			m_HitInk[i].Stop();
		}
	}

	private void DoWait(float waitTime, Action callback = null)
	{
		if (m_WaitCallback == null)
		{
			m_WaitTimer = waitTime;
			m_WaitCallback = callback;
			PreviousThought = CurrentThought;
		}
	}

	public void ApplyAttack(int attackIndex)
	{
		m_AudioSwitch.Play((attackIndex != 0) ? "attack2" : "attack1");
	}

	public void ApplyStomp()
	{
		m_AudioSwitch.Play("footsteps");
		ApplyShake();
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

	public void PlaySmashAudio()
	{
		m_AudioSwitch.Play("pound");
	}

	protected override void OnDisposed()
	{
		if (m_InkSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_InkSequence, false);
			m_InkSequence = null;
		}
		m_AudioSwitch = null;
		base.OnDisposed();
	}
}
