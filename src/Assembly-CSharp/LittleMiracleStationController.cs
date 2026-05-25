using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class LittleMiracleStationController : TMGMonoBehaviour
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_CameraLook;

	[SerializeField]
	private Transform m_Door;

	[SerializeField]
	private Transform m_BackupPosition;

	[SerializeField]
	private Transform m_HidePosition;

	[SerializeField]
	private Transform m_ExitPosition;

	[Header("Interactable")]
	[SerializeField]
	private Interactable m_DoorInteractable;

	[SerializeField]
	private CharacterLook m_CharacterLook;

	private PlayerController m_Player;

	private bool m_IsDead;

	public bool isHiding { get; private set; }

	public event EventHandler OnInteract;

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_DoorInteractable.OnInteracted += HandleDoorInteractableOnInteracted;
	}

	private void Update()
	{
		if (isHiding && !m_IsDead && !GameManager.Instance.isPaused && (Object)(object)GameManager.Instance.GameCamera != (Object)null)
		{
			m_CharacterLook.Rotation(m_CameraLook, GameManager.Instance.GameCamera.FreeRoamCam);
			m_CharacterLook.GetInput();
		}
	}

	private void HandleDoorInteractableOnInteracted(object sender, EventArgs e)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		m_DoorInteractable.OnInteracted -= HandleDoorInteractableOnInteracted;
		GameManager.Instance.CurrentChapter.DeathController.OnDeath += HandlePlayerOnDeath;
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned += HandlePlayerOnSpawned;
		m_Player = GameManager.Instance.Player;
		m_Player.SetLockedMovement(active: true);
		m_Player.SetCollision(active: false);
		GameManager.Instance.HideCrosshair();
		Transform freeRoamCam = GameManager.Instance.GameCamera.FreeRoamCam;
		freeRoamCam.position = GameManager.Instance.GameCamera.CameraContainer.position;
		freeRoamCam.rotation = GameManager.Instance.GameCamera.CameraContainer.rotation;
		GameManager.Instance.GameCamera.CameraContainer.SetParent(freeRoamCam);
		GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
		Sequence val = DOTween.Sequence();
		float num = 0f;
		if ((Object)(object)GameManager.Instance.Player.WeaponGameObject != (Object)null)
		{
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(GameManager.Instance.Player.WeaponGameObject.transform, -5f, 0.25f, false), (Ease)5));
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(GameManager.Instance.Player.WeaponGameObject.transform, new Vector3(180f, 0f, 0f), 0.2f, (RotateMode)3), (Ease)5));
			TweenSettingsExtensions.InsertCallback(val, num + 0.25f, (TweenCallback)delegate
			{
				GameManager.Instance.Player.WeaponGameObject.SetActive(false);
				GameManager.Instance.Player.UnEquipWeapon();
			});
		}
		num += 0.25f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(freeRoamCam, m_BackupPosition.position, 0.4f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(freeRoamCam, m_BackupPosition.eulerAngles, 0.4f, (RotateMode)0), (Ease)7));
		num += 0.4f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Door, new Vector3(0f, 90f, 0f), 0.8f, (RotateMode)0), (Ease)7));
		S13AudioManager.Instance.InvokeEvent("evt_miracle_station_enter");
		num += 0.5f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(freeRoamCam, m_CameraLook.position, 0.65f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(freeRoamCam, m_CameraLook.eulerAngles, 0.7f, (RotateMode)0), (Ease)7));
		num += 0.25f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Door, Vector3.zero, 0.7f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(HandleEntranceOnComplete));
		this.OnInteract.Send(this);
	}

	private void HandleEntranceOnComplete()
	{
		if (!m_IsDead)
		{
			GameManager.Instance.GameCamera.FreeRoamCam.SetParent(m_CameraLook);
			m_CharacterLook.Init(m_CameraLook, GameManager.Instance.GameCamera.FreeRoamCam);
			m_CharacterLook.HorizontalClampSetActive(active: true);
			m_CharacterLook.SetHorizontalClamp(5f);
			isHiding = true;
			GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hiding);
			this.OnEnter.Send(this);
			m_DoorInteractable.OnInteracted += HandleDoorExitOnInteracted;
		}
	}

	private void HandleDoorExitOnInteracted(object sender, EventArgs e)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		m_DoorInteractable.OnInteracted -= HandleDoorExitOnInteracted;
		isHiding = false;
		m_Player.transform.position = new Vector3(m_ExitPosition.position.x, m_Player.transform.position.y, m_ExitPosition.position.z);
		m_Player.transform.rotation = m_ExitPosition.rotation;
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOLocalRotate(m_Door, new Vector3(0f, 90f, 0f), 1f, (RotateMode)0));
		S13AudioManager.Instance.InvokeEvent("evt_miracle_station_exit");
		num += 0.5f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOMove(GameManager.Instance.GameCamera.FreeRoamCam, GameManager.Instance.GameCamera.HeadContainer.position, 0.75f, false));
		num += 0.75f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.CameraContainer.SetParent(GameManager.Instance.GameCamera.HeadContainer);
			GameManager.Instance.GameCamera.CameraContainer.localPosition = Vector3.zero;
			GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
			Vector3 zero = Vector3.zero;
			zero.x = GameManager.Instance.GameCamera.FreeRoamCam.eulerAngles.x;
			Vector3 eulerAngles = m_ExitPosition.eulerAngles;
			eulerAngles.y += m_CameraLook.localEulerAngles.y;
			GameManager.Instance.Player.LookRotation(Quaternion.Euler(eulerAngles), Quaternion.Euler(zero));
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOLocalRotate(m_Door, Vector3.zero, 1f, (RotateMode)0));
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(EnablePlayer));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(HandleExitOnComplete));
	}

	private void EnablePlayer()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		m_Player.SetLockedMovement(active: false);
		m_Player.SetCollision(active: true);
		GameManager.Instance.GameCamera.FreeRoamCam.SetParent((Transform)null);
		GameManager.Instance.ShowCrosshair();
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
		isHiding = false;
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Idle);
	}

	private void HandleExitOnComplete()
	{
		this.OnExit.Send(this);
		m_DoorInteractable.OnInteracted += HandleDoorInteractableOnInteracted;
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.CurrentChapter.DeathController.OnDeath -= HandlePlayerOnDeath;
		GameManager.Instance.GameCamera.CameraContainer.SetParent(GameManager.Instance.GameCamera.HeadContainer);
		GameManager.Instance.GameCamera.CameraContainer.localPosition = Vector3.zero;
		GameManager.Instance.GameCamera.CameraContainer.localScale = Vector3.one;
		m_IsDead = true;
		EnablePlayer();
	}

	private void HandlePlayerOnSpawned(object sender, EventArgs e)
	{
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		m_IsDead = false;
		m_DoorInteractable.OnInteracted -= HandleDoorExitOnInteracted;
		m_DoorInteractable.OnInteracted -= HandleDoorInteractableOnInteracted;
		m_DoorInteractable.OnInteracted += HandleDoorInteractableOnInteracted;
	}

	protected override void OnDisposed()
	{
		m_Player = null;
		base.OnDisposed();
	}
}
