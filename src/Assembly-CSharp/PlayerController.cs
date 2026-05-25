using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

public class PlayerController : TMGMonoBehaviour
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_HeadContainer;

	[SerializeField]
	private Transform m_HandContainer;

	[SerializeField]
	private Transform m_WeaponParent;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private Transform m_SeeingTool;

	[Header("Movement Options")]
	[SerializeField]
	private float m_SlowedMoveSpeed = 4f;

	[SerializeField]
	private float m_MoveSpeed = 7f;

	[SerializeField]
	private float m_RunSpeed = 10f;

	[Header("Run Options")]
	[SerializeField]
	private bool m_EnableRun = true;

	[Header("Jump Options")]
	[SerializeField]
	private bool m_EnableJump = true;

	[SerializeField]
	private float m_JumpSpeed = 10f;

	[SerializeField]
	private List<AudioClip> m_JumpClips;

	[Header("Gravity")]
	[SerializeField]
	private bool m_EnableGravity = true;

	[SerializeField]
	private float m_StickToGroundForce = 10f;

	[SerializeField]
	private float m_GravityMultiplier = 3f;

	[Space]
	[SerializeField]
	private CharacterLook m_PlayerLook;

	[Space]
	[SerializeField]
	private CameraMovements m_CameraMovement;

	[Space]
	[SerializeField]
	private CameraFOV m_CameraFOV;

	[Space]
	[SerializeField]
	private CharacterFootsteps m_PlayerFootsteps;

	[Space]
	[SerializeField]
	private FirstPersonHeadBob m_HeadBob;

	[Space]
	[SerializeField]
	private InteractableInputController m_Interaction;

	[Space]
	[SerializeField]
	private PlayerDeath m_Death;

	public GameObject WeaponGameObject;

	public GameObject InactiveWeapon;

	private bool m_IsSeeingToolEnabled;

	private bool m_CanHaveSeeingTool;

	private CharacterController m_CharacterController;

	private Vector3 m_GroundNormal;

	private Vector2 m_Input;

	private Vector3 m_MoveDir = Vector3.zero;

	private float m_GravityPower;

	private Vector3 m_ExternalForce = Vector3.zero;

	private bool m_IsRunning;

	private bool m_CanMoveBack = true;

	private bool m_PreviouslyGrounded = true;

	private bool m_JumpInput;

	private float m_OriginWalkSpeed;

	private float m_OriginRunSpeed;

	private float m_InkWalkSpeed = 4f;

	private float m_InkRunSpeed = 7f;

	private bool m_LandingAudioBS;

	private bool m_ToggleRun;

	private Sequence m_SeeingToolSequence;

	private bool m_IsPaused;

	public CharacterController CharacterController => m_CharacterController;

	public Transform HeadContainer => m_HeadContainer;

	public Transform CameraParent => m_CameraContainer;

	public Transform WeaponParent => m_WeaponParent;

	public bool isLocked { get; private set; }

	public bool useGravity => m_EnableGravity;

	public bool isSlowed { get; private set; }

	public bool isMoveLocked { get; set; }

	public bool canJump { get; private set; }

	public bool canCameraSway { get; private set; }

	public bool canRun { get; private set; }

	public bool isSeeingToolActive { get; private set; }

	public CombatStatus CurrentStatus { get; private set; }

	public float CurrentSpeed { get; private set; }

	public FootstepTypes CurrentFootstepType => m_PlayerFootsteps.CurrentFootstepType;

	public float deltaTimeMultiplier { get; set; }

	public event EventHandler OnSeeingToolActive;

	public event EventHandler OnDeath;

	public override void Init()
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		base.Init();
		GameManager.Instance.Player = this;
		m_CharacterController = ((Component)this).GetComponent<CharacterController>();
		Cursor.lockState = (CursorLockMode)1;
		Cursor.visible = false;
		m_OriginWalkSpeed = m_MoveSpeed;
		m_OriginRunSpeed = m_RunSpeed;
		CurrentStatus = CombatStatus.Idle;
		canJump = m_EnableJump;
		canRun = m_EnableRun;
		m_CameraMovement.Init(m_HeadContainer, m_CameraContainer);
		m_CameraFOV.Init(GameManager.Instance.GameCamera.Camera, GameManager.Instance.GameCamera.WeaponCamera);
		m_PlayerLook.Init(base.transform, m_HeadContainer);
		m_Interaction.Init();
		if ((Object)(object)Object.FindObjectOfType<RigidbodyOptimizer>() == (Object)null)
		{
			GameObject val = new GameObject("Rigidbody_Optimizer_System");
			val.AddComponent<RigidbodyOptimizer>();
			Object.DontDestroyOnLoad((Object)val);
		}
	}

	public override void InitOnComplete()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		base.InitOnComplete();
		List<FootstepsDataVO> list = new List<FootstepsDataVO>();
		list.Add(FootstepsDataVO.Create(FootstepTypes.WOOD, "step_wood"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.WOOD_STAIRS, "step_wood"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.INK, "step_wood"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.INK_STAIRS, "step_wood"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.INK_DEEP, "step_wood"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.VENT, "vent"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.DIRT, "step_dirt"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.METAL, "step_metal"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.TILE, "step_tile"));
		list.Add(FootstepsDataVO.Create(FootstepTypes.INK_DEEP, "step_water"));
		m_PlayerFootsteps.Init(list);
		m_HeadBob.Init(m_HeadContainer, m_PlayerFootsteps.StepInterval);
		GameManager.Instance.AudioManager.ListenerSetActive(active: false);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.5f, (TweenCallback)delegate
		{
			m_LandingAudioBS = true;
		});
	}

	private void Update()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		deltaTimeMultiplier = Time.fixedDeltaTime / Time.deltaTime;
		if (GameManager.Instance.isPaused)
		{
			m_IsPaused = true;
			return;
		}
		if (m_IsPaused)
		{
			TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
			{
				m_IsPaused = false;
			});
			return;
		}
		float speed = 0f;
		float num = 10f;
		GetInput(out speed);
		CurrentSpeed = speed;
		CheckGrounding();
		if (m_CharacterController.isGrounded)
		{
			num = 60f;
		}
		if (!isMoveLocked)
		{
			Move(CurrentSpeed);
			if (((Vector3)(ref m_ExternalForce)).magnitude > 0f)
			{
				if (((Collider)m_CharacterController).enabled)
				{
					m_CharacterController.Move(m_ExternalForce * Time.fixedDeltaTime);
				}
				m_ExternalForce = Vector3.MoveTowards(m_ExternalForce, Vector3.zero, 3f * Time.fixedDeltaTime * num);
			}
		}
		if (!GameManager.Instance.isPaused)
		{
			m_PlayerLook.GetInput();
			if (GameManager.Instance.PlayerSettings.ToggleRun && PlayerInput.RunDown())
			{
				m_ToggleRun = !m_ToggleRun;
			}
			if (m_CharacterController.isGrounded && !m_JumpInput && m_EnableJump && canJump && !isLocked && !isMoveLocked)
			{
				m_JumpInput = PlayerInput.Jump();
			}
			m_Interaction.UpdateInteraction(m_CameraContainer.position, m_CameraContainer.forward);
			if (m_CanHaveSeeingTool && m_CharacterController.isGrounded && CurrentStatus != CombatStatus.Hiding && !isLocked && m_IsSeeingToolEnabled && PlayerInput.SeeingTool())
			{
				isSeeingToolActive = !isSeeingToolActive;
				UseSeeingTool(isSeeingToolActive);
			}
			if (isSeeingToolActive)
			{
				m_HeadBob.UpdateCameraPosition(0f, 0f);
			}
			GetRotations();
			m_PlayerLook.UpdateCursorLock();
		}
	}

	private void GetRotations()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (canCameraSway)
		{
			m_CameraMovement.Sway(m_HeadContainer);
		}
		if (!isLocked)
		{
			m_PlayerLook.Rotation(base.transform, m_HeadContainer, m_HandContainer);
		}
		if (m_CharacterController.isGrounded && !m_PreviouslyGrounded && m_LandingAudioBS)
		{
			((MonoBehaviour)this).StartCoroutine(m_HeadBob.DoJumpBob());
			m_PlayerFootsteps.PlayLandAudio();
			m_ExternalForce = Vector3.zero;
		}
	}

	private void GetInput(out float speed)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (isLocked)
		{
			speed = 0f;
			return;
		}
		float num = PlayerInput.MoveX() * 0.8f;
		float num2 = PlayerInput.MoveY();
		if (num2 > 0f)
		{
			if (GameManager.Instance.PlayerSettings.ToggleRun)
			{
				m_IsRunning = m_EnableRun && m_ToggleRun;
			}
			else
			{
				m_IsRunning = m_EnableRun && PlayerInput.Run();
			}
		}
		else
		{
			if (!m_CanMoveBack)
			{
				num2 = 0f;
			}
			m_IsRunning = false;
		}
		speed = ((!m_IsRunning) ? m_MoveSpeed : m_RunSpeed);
		if (isSlowed)
		{
			speed = m_SlowedMoveSpeed;
			m_IsRunning = false;
		}
		else if (!canRun)
		{
			speed = m_MoveSpeed;
			m_IsRunning = false;
		}
		m_Input = new Vector2(num, num2);
		if (((Vector2)(ref m_Input)).magnitude > 1f)
		{
			((Vector2)(ref m_Input)).Normalize();
		}
		if (num2 < 0f)
		{
			speed *= 0.75f;
		}
	}

	private void CheckGrounding()
	{
		if (m_CharacterController.isGrounded && !m_PreviouslyGrounded && m_GravityPower <= 0f)
		{
			m_GravityPower = 0f - m_StickToGroundForce;
		}
		m_PreviouslyGrounded = m_CharacterController.isGrounded;
	}

	private void Move(float speed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.transform.forward * m_Input.y + base.transform.right * m_Input.x;
		float num = Vector3.Angle(Vector3.up, m_GroundNormal);
		bool flag = num < m_CharacterController.slopeLimit || num > 85f;
		m_MoveDir.x = val.x * speed;
		m_MoveDir.z = val.z * speed;
		if (!flag)
		{
			m_MoveDir.x += (1f - m_GroundNormal.y) * m_GroundNormal.x * (speed / 2f);
			m_MoveDir.z += (1f - m_GroundNormal.y) * m_GroundNormal.z * (speed / 2f);
		}
		m_GroundNormal = Vector3.up;
		if (m_CharacterController.isGrounded)
		{
			if (m_JumpInput && flag)
			{
				m_GravityPower = m_JumpSpeed / deltaTimeMultiplier;
				m_PlayerFootsteps.PlayJumpAudio();
			}
			m_JumpInput = false;
		}
		else if (m_EnableGravity)
		{
			m_GravityPower += Physics.gravity.y * m_GravityMultiplier * Time.deltaTime / deltaTimeMultiplier;
		}
		if (((Collider)m_CharacterController).enabled)
		{
			m_CharacterController.Move(m_MoveDir * Time.deltaTime + Vector3.up * m_GravityPower);
		}
		if (!m_CharacterController.isGrounded)
		{
			return;
		}
		Vector3 velocity = m_CharacterController.velocity;
		float magnitude = ((Vector3)(ref velocity)).magnitude;
		FootstepTypes footstepType = FootstepTypes.WOOD;
		Vector3 position = base.transform.position;
		ref float y = ref position.y;
		float num2 = y;
		Bounds bounds = ((Collider)m_CharacterController).bounds;
		y = num2 + (((Bounds)(ref bounds)).extents.y - 0.1f);
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(position, Vector3.down, ref val2, m_CharacterController.height + 1f, ~(1 << LayerMask.NameToLayer("Player"))))
		{
			if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("Ink"))
			{
				footstepType = FootstepTypes.INK;
			}
			else if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("DeepInk"))
			{
				footstepType = FootstepTypes.INK_DEEP;
			}
			else if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("Stairs"))
			{
				footstepType = FootstepTypes.WOOD_STAIRS;
			}
			else if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("StairsInk"))
			{
				footstepType = FootstepTypes.INK_STAIRS;
			}
			else if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("Vent"))
			{
				footstepType = FootstepTypes.VENT;
			}
			else if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("Dirt"))
			{
				footstepType = FootstepTypes.DIRT;
			}
			else if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("Metal"))
			{
				footstepType = FootstepTypes.METAL;
			}
			else if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("Tile"))
			{
				footstepType = FootstepTypes.TILE;
			}
		}
		m_PlayerFootsteps.SetFootstepType(footstepType);
		m_PlayerFootsteps.ProgressStepCycle(magnitude, speed);
		m_HeadBob.UpdateCameraPosition(magnitude, speed);
		m_CameraFOV.UpdateVOD(magnitude, speed, m_IsRunning);
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		m_GroundNormal = hit.normal;
		if (!hit.gameObject.isStatic)
		{
			Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
			if (Object.op_Implicit((Object)(object)attachedRigidbody) && !attachedRigidbody.isKinematic)
			{
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(hit.moveDirection.x, 0f, hit.moveDirection.z);
				attachedRigidbody.velocity = val * 2f;
				m_ExternalForce = Vector3.zero;
			}
		}
	}

	public void AddForce(Vector3 force)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ExternalForce += force;
	}

	public void EquipWeapon()
	{
		ScaleWeapon();
		m_Interaction.HasWeapon = true;
		if (Object.op_Implicit((Object)(object)WeaponGameObject))
		{
			BaseWeapon component = WeaponGameObject.GetComponent<BaseWeapon>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.CleanEquip();
			}
		}
	}

	public void UnEquipWeapon()
	{
		m_Interaction.HasWeapon = false;
		if (Object.op_Implicit((Object)(object)WeaponGameObject))
		{
			BaseWeapon component = WeaponGameObject.GetComponent<BaseWeapon>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.UnEquip();
			}
		}
	}

	public void EnableSeeingTool(bool active)
	{
		m_IsSeeingToolEnabled = active;
	}

	public void AllowSeeingTool(bool active)
	{
		m_CanHaveSeeingTool = active;
	}

	public void UseSeeingTool(bool active)
	{
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		if (active && isMoveLocked)
		{
			isSeeingToolActive = !isSeeingToolActive;
			return;
		}
		TweenExtensions.Kill((Tween)(object)m_SeeingToolSequence, false);
		m_SeeingToolSequence = DOTween.Sequence();
		float num = 0f;
		if (active)
		{
			this.OnSeeingToolActive.Send(this);
			SetInteraction(active: false);
			GameManager.Instance.HideCrosshair();
			m_SeeingTool.localPosition = new Vector3(0f, -5f, 0f);
			if (Object.op_Implicit((Object)(object)WeaponGameObject))
			{
				TweenSettingsExtensions.Insert(m_SeeingToolSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(WeaponGameObject.transform, -5f, 0.4f, false), (Ease)5));
				num += 0.35f;
				TweenSettingsExtensions.InsertCallback(m_SeeingToolSequence, num, (TweenCallback)delegate
				{
					WeaponGameObject.SetActive(false);
					((Component)m_SeeingTool).gameObject.SetActive(true);
				});
			}
			else
			{
				((Component)m_SeeingTool).gameObject.SetActive(true);
			}
			TweenSettingsExtensions.Insert(m_SeeingToolSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_SeeingTool, 0f, 0.4f, false), (Ease)6));
			S13AudioManager.Instance.InvokeEvent("evt_seeing_tool_on");
			return;
		}
		TweenSettingsExtensions.Insert(m_SeeingToolSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_SeeingTool, -5f, 0.4f, false), (Ease)5));
		num += 0.4f;
		TweenSettingsExtensions.InsertCallback(m_SeeingToolSequence, num, (TweenCallback)delegate
		{
			((Component)m_SeeingTool).gameObject.SetActive(false);
			if (Object.op_Implicit((Object)(object)WeaponGameObject))
			{
				WeaponGameObject.SetActive(true);
			}
			SetInteraction(active: true);
			GameManager.Instance.ShowCrosshair();
		});
		if (Object.op_Implicit((Object)(object)WeaponGameObject))
		{
			TweenSettingsExtensions.Insert(m_SeeingToolSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(WeaponGameObject.transform, 0f, 0.4f, false), (Ease)6));
		}
		S13AudioManager.Instance.InvokeEvent("evt_seeing_tool_off");
	}

	public void ForceHideSeeingTool(bool justHide = false)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		isSeeingToolActive = false;
		Vector3 localPosition = m_SeeingTool.localPosition;
		localPosition.y = -5f;
		m_SeeingTool.localPosition = localPosition;
		((Component)m_SeeingTool).gameObject.SetActive(false);
		if (Object.op_Implicit((Object)(object)WeaponGameObject))
		{
			Vector3 localPosition2 = WeaponGameObject.transform.localPosition;
			localPosition2.y = 0f;
			WeaponGameObject.transform.localPosition = localPosition2;
		}
		if (!justHide)
		{
			SetInteraction(active: true);
			SetLockedMovement(active: false);
		}
	}

	public void GoToAndLookAt(Transform target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		base.transform.position = target.position;
		Vector3 zero = Vector3.zero;
		zero.x = target.eulerAngles.x;
		Vector3 zero2 = Vector3.zero;
		zero2.y = target.localEulerAngles.y;
		LookRotation(Quaternion.Euler(zero2), Quaternion.Euler(zero));
	}

	public void LookRotation(Quaternion playerRotation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_PlayerLook.ForceRotation(playerRotation);
	}

	public void LookRotation(Quaternion playerRotation, Quaternion cameraRotation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_PlayerLook.ForceRotation(playerRotation);
		m_PlayerLook.ForceCameraRotation(cameraRotation);
	}

	public void LockRotation(float x, float y)
	{
		m_PlayerLook.HorizontalClampSetActive(active: true);
		m_PlayerLook.SetHorizontalClamp(x);
		m_PlayerLook.SetVerticalClamp(y);
	}

	public void UnlockRotation()
	{
		m_PlayerLook.HorizontalClampSetActive(active: false);
		m_PlayerLook.ResetVerticalClamp();
	}

	public void SetInteraction(bool active)
	{
		m_Interaction.SetActive(active);
	}

	public void SetSensitivity(float value)
	{
		m_PlayerLook.SetSensitivity(value);
	}

	public void SetLockedMovement(bool active)
	{
		isMoveLocked = active;
	}

	public void SetLock(bool active, bool ignoreSeeingTool = false)
	{
		if (!ignoreSeeingTool)
		{
			ForceHideSeeingTool(justHide: true);
		}
		isLocked = active;
	}

	public void SetSlowed(bool active)
	{
		isSlowed = active;
	}

	public void SetInkSpeed(bool active)
	{
		m_MoveSpeed = ((!active) ? m_OriginWalkSpeed : m_InkWalkSpeed);
		m_RunSpeed = ((!active) ? m_OriginRunSpeed : m_InkRunSpeed);
	}

	public void SetRun(bool active)
	{
		canRun = active;
	}

	public void SetJump(bool active)
	{
		canJump = active;
	}

	public void SetCollision(bool active)
	{
		((Collider)m_CharacterController).enabled = active;
	}

	public void SetCameraSway(bool active)
	{
		canCameraSway = active;
	}

	public void SetCombatStatus(CombatStatus playerStatus)
	{
		if (CurrentStatus != playerStatus)
		{
			CurrentStatus = playerStatus;
		}
	}

	public void SetVent(bool active)
	{
		SetSlowed(active);
		m_HeadBob.CrawlSetActive(active);
		SetJump(!active);
	}

	public void SetFOVValue(float value, float duration)
	{
		m_CameraFOV.DOFov(value, duration);
	}

	public void SetActiveFOV(bool active)
	{
		m_CameraFOV.SetActiveFOV(active);
	}

	public void PlayPickUpSound()
	{
		m_PlayerFootsteps.AudioSwitch.Play("pickup");
	}

	public void SetBackMovement(bool active)
	{
		m_CanMoveBack = active;
	}

	public void PlayRespawnEffects()
	{
		S13AudioManager.Instance.InvokeEvent("evt_player_respawned");
		GameManager.Instance.AudioManager.Play("Audio/SFX/SFX_Respawn_01");
		m_Death.Play();
	}

	public void Die()
	{
		SetCombatStatus(CombatStatus.None);
		S13AudioManager.Instance.InvokeEvent("evt_player_dead");
		this.OnDeath.Send(this);
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.Player = null;
		base.OnDisposed();
	}

	public void ScaleWeapon()
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			return;
		}
		float weaponScale = GameManager.Instance.PlayerSettings.weaponScale;
		Transform[] componentsInChildren = GameManager.Instance.Player.gameObject.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			GameObject val = ((Component)componentsInChildren[i]).gameObject;
			if (((Object)val).name.Contains("Axe_Model"))
			{
				Transform parent = val.transform.parent;
				if ((Object)(object)parent != (Object)null && ((Object)parent).name.Contains("Weapon_Axe_New"))
				{
					Transform parent2 = parent.parent;
					if ((Object)(object)parent2 != (Object)null && ((Object)parent2).name.Contains("WeaponAnimator"))
					{
						Transform parent3 = parent2.parent;
						if ((Object)(object)parent3 != (Object)null && ((Object)parent3).name.Contains("Hand"))
						{
							float num = 1.575f - weaponScale * 2f;
							float num2 = weaponScale - 0.4f;
							val.transform.localPosition = new Vector3(0f, num, 0f);
							val.transform.localScale = new Vector3(num2, num2, num2);
							break;
						}
					}
				}
			}
			if (((Object)val).name.Contains("Axe") && (Object)(object)val.transform.parent != (Object)null && ((Object)val.transform.parent).name.Contains("Weapon_Axe"))
			{
				val.transform.localScale = new Vector3(weaponScale, weaponScale, weaponScale);
			}
			else if (((Object)val).name.Contains("Flashlight Model") && (Object)(object)val.transform.parent != (Object)null && ((Object)val.transform.parent).name.Contains("Prop_Flashlight"))
			{
				val.transform.localScale = new Vector3(weaponScale, weaponScale, weaponScale);
				Transform val2 = val.transform.parent.Find("SpotLight");
				if ((Object)(object)val2 != (Object)null)
				{
					Light component = ((Component)val2).GetComponent<Light>();
					if ((Object)(object)component != (Object)null)
					{
						Resolution currentResolution = Screen.currentResolution;
						float num3 = ((Resolution)(ref currentResolution)).width;
						currentResolution = Screen.currentResolution;
						float num4 = num3 / (float)((Resolution)(ref currentResolution)).height - 1f;
						float num5 = 85f * num4;
						num5 = Mathf.Clamp(num5, 85f, 130f);
						component.spotAngle = num5;
					}
				}
			}
			else if (((Object)val).name.Contains("Model_Gent_Pipe") && (Object)(object)val.transform.parent != (Object)null && ((Object)val.transform.parent).name.Contains("Weapon_Gent"))
			{
				float num6 = 1f - weaponScale;
				val.transform.localPosition = new Vector3(0f, num6, 0f);
				val.transform.localScale = new Vector3(weaponScale, weaponScale, weaponScale);
			}
			else if (((Object)val).name.Contains("CH3_Wrench_Weapon") && (Object)(object)val.transform.parent != (Object)null && ((Object)val.transform.parent).name.Contains("Weapon_Wrench"))
			{
				val.transform.localScale = new Vector3(weaponScale, weaponScale, weaponScale);
			}
			else if (((Object)val).name.Contains("CH3_PumpTool") && (Object)(object)val.transform.parent != (Object)null && ((Object)val.transform.parent).name.Contains("Pump Tool Model"))
			{
				val.transform.localScale = new Vector3(weaponScale, weaponScale, weaponScale);
			}
			else if (((Object)val).name.Contains("CH3_PumpTool_lever") && (Object)(object)val.transform.parent != (Object)null && ((Object)val.transform.parent).name.Contains("Pump Tool Model"))
			{
				val.transform.localScale = new Vector3(weaponScale, weaponScale, weaponScale);
				val.transform.localPosition = new Vector3(0f, weaponScale, 0f);
			}
			else if (((Object)val).name.Contains("CH3_Plunger_Weapon") && (Object)(object)val.transform.parent != (Object)null && ((Object)val.transform.parent).name.Contains("Weapon_Plunger"))
			{
				float num7 = 0.75f - weaponScale;
				val.transform.localPosition = new Vector3(0f, num7, 0f);
				val.transform.localScale = new Vector3(weaponScale, weaponScale, weaponScale);
			}
			else if (((Object)val).name.Contains("CH3_TommyGun") && (Object)(object)val.transform.parent != (Object)null && ((Object)val.transform.parent).name.Contains("Weapon_TommyGun"))
			{
				val.transform.localScale = new Vector3(weaponScale, weaponScale, weaponScale);
			}
		}
	}
}
