using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4CartRide : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_CameraPoint;

	[SerializeField]
	private Transform m_Handle;

	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private CharacterLook m_CharacterLook;

	[SerializeField]
	private float m_ViewAngleLock;

	private List<Transform> m_Waypoints = new List<Transform>();

	private Transform m_FreeRoamCam;

	public bool IsInCart { get; private set; }

	public event EventHandler OnEnter;

	public virtual void Update()
	{
		if (!GameManager.Instance.isPaused && (!Object.op_Implicit((Object)(object)GameManager.Instance.Player) || GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding) && IsInCart)
		{
			m_CharacterLook.Rotation(m_CameraPoint, GameManager.Instance.GameCamera.FreeRoamCam);
			m_CharacterLook.GetInput();
		}
	}

	private void HandleInteractableOnInteracted(object sender, EventArgs e)
	{
		m_Interactable.OnInteracted -= HandleInteractableOnInteracted;
		m_Interactable.Dispose();
		EnterCart();
	}

	private void EnterCart()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		S13AudioManager.Instance.PlayAudio("sfx_haunted_house_cart_enter");
		m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			GameManager.Instance.Player.WeaponGameObject.SetActive(false);
		}
		GameManager.Instance.Player.GoToAndLookAt(m_FreeRoamCam);
		GameManager.Instance.Player.transform.SetParent(m_FreeRoamCam);
		GameManager.Instance.Player.SetLock(active: true);
		GameManager.Instance.Player.transform.localEulerAngles = Vector3.zero;
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_CameraPoint.position, 2f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_CameraPoint.eulerAngles, 2f, (RotateMode)0), (Ease)7));
		num += 2f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_Handle, new Vector3(-45f, 0f, 0f), 2f, (RotateMode)0), (Ease)5));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(OnCartEntered));
	}

	private void OnCartEntered()
	{
		m_FreeRoamCam.SetParent(m_CameraPoint);
		m_CharacterLook.Init(m_CameraPoint, m_FreeRoamCam);
		m_CharacterLook.HorizontalClampSetActive(active: true);
		m_CharacterLook.SetHorizontalClamp(m_ViewAngleLock);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hiding);
		IsInCart = true;
		this.OnEnter.Send(this);
	}

	public void SetWaypoints(List<Transform> waypoints)
	{
		m_Waypoints.Clear();
		m_Waypoints = waypoints;
	}

	public void SetActive(bool active)
	{
		m_Interactable.SetActive(active);
		if (active)
		{
			m_Interactable.OnInteracted += HandleInteractableOnInteracted;
		}
		else
		{
			m_Interactable.OnInteracted -= HandleInteractableOnInteracted;
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
