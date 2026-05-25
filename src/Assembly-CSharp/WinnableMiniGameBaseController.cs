using System;
using DG.Tweening;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

public class WinnableMiniGameBaseController : TMGMonoBehaviour
{
	public enum MiniGameState
	{
		INACTIVE,
		PREPPING,
		ACTIVE,
		LOCKED
	}

	[Header("Interactibles")]
	[SerializeField]
	protected Interactable m_InteractGameStart;

	[Header("Visuals")]
	[SerializeField]
	private MeshRenderer[] m_HeldObjectVisuals;

	[Header("Reference points")]
	[SerializeField]
	private Transform m_LocalLookTransform;

	[SerializeField]
	private Transform m_PlayerEndPosition;

	[Header("Look Transform Override: leave blank for camera")]
	[SerializeField]
	private Transform m_LookTransformOverrideY;

	[SerializeField]
	private Transform m_LookTransformOverrideZ;

	[Header("Held Object Transforms")]
	[SerializeField]
	private Transform m_HeldObjectAnimatedTransform;

	[SerializeField]
	private Transform m_HeldItemPositionTransform;

	[Header("Game Properties")]
	[SerializeField]
	private float m_ViewAngleLock;

	[SerializeField]
	private bool m_PickUpInteractionObject;

	[SerializeField]
	private CharacterLook m_CharacterLook;

	private Vector3 m_PickupStartingPosition;

	private Vector3 m_HeldItemReferencePosition;

	private Transform m_FreeRoamCam;

	protected AudioObject m_MusicObject;

	protected Sequence ExitGameSequence;

	public Transform HeldObject => m_HeldObjectAnimatedTransform;

	public MiniGameState CurrentState { get; private set; }

	public event EventHandler OnWin;

	public override void Init()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		SetHeldObjectVisible(isVisible: false);
		m_PickupStartingPosition = m_InteractGameStart.transform.position;
		if ((Object)(object)m_HeldItemPositionTransform != (Object)null)
		{
			m_HeldItemReferencePosition = m_HeldItemPositionTransform.localPosition;
		}
		m_CharacterLook.Init(m_LocalLookTransform);
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_InteractGameStart.OnInteracted += HandleOnGameStart;
	}

	public virtual void Update()
	{
		if (!GameManager.Instance.isPaused && (!Object.op_Implicit((Object)(object)GameManager.Instance.Player) || GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding) && (CurrentState == MiniGameState.ACTIVE || CurrentState == MiniGameState.PREPPING) && CurrentState == MiniGameState.ACTIVE)
		{
			if (PlayerInput.InteractOnPressed())
			{
				CurrentState = MiniGameState.INACTIVE;
				ForceExitGame();
			}
			m_CharacterLook.GetInput();
		}
	}

	private void LateUpdate()
	{
		if (!GameManager.Instance.isPaused && CurrentState == MiniGameState.ACTIVE && Object.op_Implicit((Object)(object)m_FreeRoamCam))
		{
			m_CharacterLook.Rotation(m_LocalLookTransform, m_FreeRoamCam);
		}
	}

	private void HandleOnGameStart(object sender, EventArgs e)
	{
		m_InteractGameStart.OnInteracted -= HandleOnGameStart;
		m_InteractGameStart.SetActive(active: false);
		PrepPlayerForGame();
	}

	private void PrepPlayerForGame()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		Vector3 val = GameManager.Instance.Player.transform.position - GameManager.Instance.Player.transform.forward * 5f;
		GameManager.Instance.Player.SetCollision(active: false);
		m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		m_LocalLookTransform.localRotation = Quaternion.identity;
		Sequence val2 = DOTween.Sequence();
		float num = 0f;
		if ((Object)(object)m_HeldItemPositionTransform != (Object)null)
		{
			TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_HeldItemPositionTransform, -5f, 0.65f, false), (Ease)7));
		}
		if (m_PickUpInteractionObject)
		{
			TweenSettingsExtensions.InsertCallback(val2, num, new TweenCallback(OnPickupObject));
			TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_InteractGameStart.transform, val, 0.65f, false), (Ease)7));
			num += 0.3f;
			TweenSettingsExtensions.InsertCallback(val2, num, (TweenCallback)delegate
			{
				m_InteractGameStart.gameObject.SetActive(false);
			});
		}
		if ((Object)(object)GameManager.Instance.Player.WeaponGameObject != (Object)null)
		{
			TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(GameManager.Instance.Player.WeaponGameObject.transform, -5f, 0.25f, false), (Ease)5));
			TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(GameManager.Instance.Player.WeaponGameObject.transform, new Vector3(180f, 0f, 0f), 0.2f, (RotateMode)3), (Ease)5));
			TweenSettingsExtensions.InsertCallback(val2, num + 0.1f, (TweenCallback)delegate
			{
				GameManager.Instance.Player.WeaponGameObject.SetActive(false);
				GameManager.Instance.Player.UnEquipWeapon();
			});
		}
		num += 0.15f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_LocalLookTransform.position, 0.65f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_LocalLookTransform.eulerAngles, 0.7f, (RotateMode)0), (Ease)7));
		num += 0.5f;
		TweenSettingsExtensions.OnComplete<Sequence>(val2, new TweenCallback(HandleOnPlayerPrepped));
	}

	private void HandleOnPlayerPrepped()
	{
		m_FreeRoamCam.SetParent(m_LocalLookTransform);
		m_CharacterLook.Init(m_LocalLookTransform, m_FreeRoamCam);
		m_CharacterLook.HorizontalClampSetActive(active: true);
		m_CharacterLook.SetHorizontalClamp(m_ViewAngleLock);
		m_CharacterLook.SetHorizontalClamp(m_ViewAngleLock);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hiding);
		CurrentState = MiniGameState.PREPPING;
		HandleGameStartupSequence();
	}

	public virtual void HandleGameStartupSequence()
	{
	}

	public void HandleOnPrepHeldObject()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_HeldItemPositionTransform != (Object)null)
		{
			SetHeldObjectVisible(isVisible: true);
			m_HeldItemPositionTransform.SetParent(m_FreeRoamCam);
		}
		Sequence val = DOTween.Sequence();
		float num = 0f;
		if ((Object)(object)m_HeldItemPositionTransform != (Object)null)
		{
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_HeldItemPositionTransform, m_HeldItemReferencePosition, 0.65f, false), (Ease)7));
		}
		num += 0.25f;
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(HandleBeginGame));
	}

	protected virtual void OnPickupObject()
	{
	}

	public void HandleBeginGame()
	{
		CurrentState = MiniGameState.ACTIVE;
		BeginGameLoops();
	}

	public virtual void BeginGameLoops()
	{
	}

	public virtual void AddScore(int Ammount)
	{
	}

	protected void ExitGame()
	{
		HandleOnQuitGame();
	}

	private void HandleOnQuitGame()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			m_MusicObject.Stop();
			m_MusicObject = null;
		}
		m_FreeRoamCam.SetParent((Transform)null);
		GameManager.Instance.Player.GoToAndLookAt(m_PlayerEndPosition);
		Sequence val = DOTween.Sequence();
		float num = 0f;
		if ((Object)(object)m_HeldItemPositionTransform != (Object)null)
		{
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_HeldItemPositionTransform, -10f, 0.65f, false), (Ease)7));
		}
		num += 0.05f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, GameManager.Instance.Player.HeadContainer.position, 0.65f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_PlayerEndPosition.eulerAngles, 0.7f, (RotateMode)0), (Ease)7));
		num += 0.25f;
		if (m_PickUpInteractionObject)
		{
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				m_InteractGameStart.gameObject.SetActive(true);
				OnPlaceObject();
			});
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_InteractGameStart.transform, m_PickupStartingPosition, 0.35f, false), (Ease)7));
			num += 0.25f;
		}
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(EnablePlayer));
	}

	protected virtual void OnPlaceObject()
	{
	}

	protected virtual void ForceExitGame()
	{
		TweenExtensions.Kill((Tween)(object)ExitGameSequence, false);
		SetState(MiniGameState.INACTIVE);
		ExitGame();
	}

	private void HandelOnResetMinigame()
	{
		SetHeldObjectVisible(isVisible: false);
		m_InteractGameStart.SetActive(active: true);
		m_InteractGameStart.OnInteracted += HandleOnGameStart;
	}

	private void EnablePlayer()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		GameManager.Instance.Player.SetCollision(active: true);
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		if ((Object)(object)GameManager.Instance.Player.WeaponGameObject != (Object)null)
		{
			GameManager.Instance.Player.WeaponGameObject.SetActive(true);
			GameManager.Instance.Player.WeaponGameObject.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(GameManager.Instance.Player.WeaponGameObject.transform, new Vector3(-180f, 0f, 0f), 0.45f, (RotateMode)3), (Ease)5), (TweenCallback)delegate
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				GameManager.Instance.Player.WeaponGameObject.transform.localEulerAngles = Vector3.zero;
			});
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(GameManager.Instance.Player.WeaponGameObject.transform, 0f, 0.5f, false), (Ease)6), (TweenCallback)delegate
			{
				GameManager.Instance.Player.EquipWeapon();
			});
		}
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Idle);
		HandelOnResetMinigame();
	}

	private void SetHeldObjectVisible(bool isVisible)
	{
		for (int i = 0; i < m_HeldObjectVisuals.Length; i++)
		{
			((Renderer)m_HeldObjectVisuals[i]).enabled = isVisible;
			((Component)m_HeldObjectVisuals[i]).gameObject.layer = LayerMask.NameToLayer("Weapon");
		}
	}

	public void SetState(MiniGameState newState)
	{
		CurrentState = newState;
	}

	public void SendOnWin()
	{
		this.OnWin.Send(this);
	}
}
